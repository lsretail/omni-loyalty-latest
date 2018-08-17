using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Presentation.Utils;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Presentation.Activities.Image
{
    public class ImagePagerAdapter : FragmentStatePagerAdapter
    {
        private List<ImageView> imageHints;
        private readonly int width;
        private readonly int height;
        private readonly string animationImageId;

        public ImagePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer, List<ImageView> imageHints, int width, int height)
            : base(javaReference, transfer)
        {
            this.imageHints = imageHints;
            this.width = width;
            this.height = height;
        }

        public ImagePagerAdapter(FragmentManager fm, List<ImageView> imageHints, int width, int height, string animationImageId = "")
            : base(fm)
        {
            this.imageHints = imageHints;
            this.width = width;
            this.height = height;
            this.animationImageId = animationImageId;
        }

        public override int Count
        {
            get
            {
                if (imageHints == null)
                    return 0;
                return imageHints.Count;
            }
        }

        public override Fragment GetItem(int position)
        {
            var fragment = new ImageFragment();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString(BundleUtils.Id, imageHints[position].Id);
            fragment.Arguments.PutStringArrayList(BundleUtils.Ids, imageHints.Select(x => x.Id).ToList());
            fragment.Arguments.PutString(BundleUtils.ImageColor, imageHints[position].GetAvgColor());
            fragment.Arguments.PutInt(BundleUtils.ImageWidth, width);
            fragment.Arguments.PutInt(BundleUtils.ImageHeight, height);

            if (position == 0 && !string.IsNullOrEmpty(animationImageId))
            {
                fragment.Arguments.PutString(BundleUtils.AnimationImageId, animationImageId);
            }

            return fragment;
        }
    }
}