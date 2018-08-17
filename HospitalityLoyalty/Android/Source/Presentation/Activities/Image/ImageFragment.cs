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
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Utils;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Activities.Image
{
    public class ImageFragment : BaseFragment, View.IOnClickListener
    {
        private ImageModel model;
        bool imageLoaded = false;
        private IList<string> imageIds;
        private string imageId;

        private View imageContainer;
        private ImageView image;
        private int width;
        private int height;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            model = new ImageModel(Activity);

            var view = Inflate(inflater, Resource.Layout.HeaderImage, null);

            imageContainer = view.FindViewById<View>(Resource.Id.HeaderImageContainer);
            image = view.FindViewById<ImageView>(Resource.Id.HeaderImage);

            imageId = Arguments.GetString(BundleUtils.Id);
            imageIds = Arguments.GetStringArrayList(BundleUtils.Ids);
            var imageColor = Arguments.GetString(BundleUtils.ImageColor);

            width = Arguments.GetInt(BundleUtils.ImageWidth);
            height = Arguments.GetInt(BundleUtils.ImageHeight);

            imageContainer.SetBackgroundColor(Color.ParseColor(imageColor));

            LoadImage();

            return view;
        }

        private async void LoadImage()
        {
            var imageView = await model.ImageGetById(imageId, new ImageSize(width, height));

            if (image != null && image != null)
            {

                ImageUtils.CrossfadeImage(image, ImageUtils.DecodeImage(imageView.Image), imageContainer, imageView.Crossfade && !imageLoaded);
            }
        }

        public void OnClick(View v)
        {
            var intent = new Intent();
            intent.SetClass(Activity, typeof(FullScreenImageActivity));
            intent.PutExtra(BundleUtils.StartingPos, imageId.IndexOf(imageId));
            intent.PutExtra(BundleUtils.ImageIds, imageIds.ToArray());

            ActivityUtils.StartActivityWithAnimation(Activity, intent, v);
        }
    }
}