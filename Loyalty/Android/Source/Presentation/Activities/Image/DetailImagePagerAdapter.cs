using System;
using System.Collections.Generic;
using System.Linq;

using Android.Runtime;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Activities.Image
{
    public class DetailImagePagerAdapter : FragmentStatePagerAdapter
    {
        private bool multipleImages;
        private float height;
        private float width;
        private readonly List<ImageView> imageViews;

        public DetailImagePagerAdapter(FragmentManager fm, List<ImageView> imageViews, float width, float height, bool multipleImages) : base(fm)
        {
            this.imageViews = imageViews;
            this.height = height;
            this.width = width;
            this.multipleImages = multipleImages;
        }

        public DetailImagePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override int Count
        {
            get { return imageViews.Count; }
        }

        public override Fragment GetItem(int position)
        {
            var imageView = DetailImageFragment.NewInstance();
            imageView.ImageView = imageViews[position];
            imageView.Ids = imageViews.Select(x => x.Id).ToArray();
            return imageView;
        }

        public override float GetPageWidth(int position)
        {
            return 1.0f;
        }
    }
}