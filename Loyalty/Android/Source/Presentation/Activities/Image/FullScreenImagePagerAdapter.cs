using System;

using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Presentation.Util;

namespace Presentation.Activities.Image
{
    public class FullScreenImagePagerAdapter : FragmentStatePagerAdapter
    {
        private readonly string[] imageIds;

        public FullScreenImagePagerAdapter(FragmentManager fm, string[] imageIds)
            : base(fm)
        {
            this.imageIds = imageIds;
        }

        public FullScreenImagePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override int Count
        {
            get { return imageIds.Length; }
        }

        public override Fragment GetItem(int position)
        {
            var fragment = new FullScreenImageFragment();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString(BundleConstants.ImageId, imageIds[position]);
            return fragment;
        }
    }
}