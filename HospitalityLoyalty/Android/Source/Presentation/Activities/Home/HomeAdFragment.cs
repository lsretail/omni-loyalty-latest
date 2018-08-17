using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Utils;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Activities.Home
{
    public class HomeAdFragment : BaseFragment
    {
        private Advertisement advertisement;
        private ImageModel imageModel;

        private View adImageContainer;
        private ImageView adImage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            imageModel = new ImageModel(Activity);

            var view = Inflate(inflater, Resource.Layout.HomeAdScreen, null);

            if (Arguments != null)
            {
                var id = Arguments.GetString(BundleUtils.Id);
                advertisement = AppData.Advertisements.FirstOrDefault(ad => ad.Id == id);
            }

            adImageContainer = view.FindViewById<View>(Resource.Id.HomeAdScreenImageContainer);
            adImage = view.FindViewById<ImageView>(Resource.Id.HomeAdScreenImage);
            var adDescription = view.FindViewById<TextView>(Resource.Id.HomeAdScreenDescription);

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
            var image = await imageModel.ImageGetById(advertisement.ImageView.Id, new ImageSize(500, 500));

            if (image != null && adImage != null)
            {
                if (image.Crossfade)
                {
                    ImageUtils.CrossfadeImage(adImage, ImageUtils.DecodeImage(image.Image), adImageContainer);
                }
                else
                {
                    ImageUtils.DisplayImage(adImage, ImageUtils.DecodeImage(image.Image), adImageContainer);
                }
            }
        }
    }
}