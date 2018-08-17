using System;
using System.Collections.Generic;

using Android.Runtime;
using Android.Support.V4.App;

using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using LSRetail.Omni.Domain.DataModel.Base.Utils;

namespace Presentation.Activities.Home
{
    public class HomeAdPagerAdapter : FragmentPagerAdapter
    {
        private readonly List<Advertisement> advertisements;

        public HomeAdPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public HomeAdPagerAdapter(FragmentManager fm, List<Advertisement> advertisements) : base(fm)
        {
            this.advertisements = advertisements;
        }

        public override int Count
        {
            get
            {
                return advertisements.Count;
            }
        }

        public override Fragment GetItem(int position)
        {
            var fragment = HomeAdFragment.NewInstance(advertisements[position]);
            return fragment;
        }

        public override float GetPageWidth(int position)
        {
            return 1.0f;
        }
    }
}