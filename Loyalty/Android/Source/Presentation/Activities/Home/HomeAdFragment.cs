using System;

using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Activities.Home
{
    public class HomeAdFragment : LoyaltyFragment
    {
        private Advertisement advertisement;
        private View adImageContainer;
        private ImageView adImage;

        public static Fragment NewInstance(Advertisement advertisement)
        {
            var fragment = new HomeAdFragment();
            fragment.advertisement = advertisement;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.HomeAd);

            adImageContainer = view.FindViewById<View>(Resource.Id.HomeAdImageContainer);
            adImage = view.FindViewById<Android.Widget.ImageView>(Resource.Id.HomeAdImage);
            var adDescription = view.FindViewById<TextView>(Resource.Id.HomeAdDescription);

            if (advertisement != null)
            {
                adImageContainer.SetBackgroundColor(Color.ParseColor(advertisement.ImageView.GetAvgColor()));
                adDescription.Text = advertisement.Description;

                LoadImage();
            }

            return view;
        }

        private async void LoadImage()
        {
            var imageModel = new ImageModel(Activity, null);
            var image = await imageModel.ImageGetById(advertisement.ImageView.Id, new ImageSize(500, 500));

            if (image == null)
            {
                adImageContainer.SetBackgroundResource(Resource.Color.transparent);
            }
            else
            {
                Utils.ImageUtils.CrossfadeImage(adImage, Utils.ImageUtils.DecodeImage(image.Image), adImageContainer, image.Crossfade);
            }
        }
    }
}