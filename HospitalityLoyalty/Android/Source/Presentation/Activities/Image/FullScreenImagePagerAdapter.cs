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
using Android.Widget;
using Presentation.Utils;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Presentation.Activities.Image
{
    public class FullScreenImagePagerAdapter : FragmentStatePagerAdapter
    {
        private readonly string[] imageIds;

        public FullScreenImagePagerAdapter(FragmentManager fm, string[] imageIds) : base(fm)
        {
            this.imageIds = imageIds;
        }

        public FullScreenImagePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
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
            fragment.Arguments.PutString(BundleUtils.ImageId, imageIds[position]);
            return fragment;
        }
    }
}