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
using Presentation.Activities.Base;
using Presentation.Activities.Menu;
using Presentation.Activities.Offer;
using Presentation.Activities.Store;
using Presentation.Utils;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Presentation.Activities.SlideControl
{
    class SlideControlPagerAdapter : FragmentStatePagerAdapter
    {
        public class SlideControlItem
        {
            public string Id1 { get; set; }
            public string Id2 { get; set; }
            public int Type { get; set; }
            public SlideControlFragment.ViewTypes ViewType { get; set; }
        }

        private readonly IList<SlideControlItem> items;
        private Dictionary<int, Fragment> fragments = new Dictionary<int, Fragment>(); 

        public SlideControlPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SlideControlPagerAdapter(FragmentManager fm, IList<SlideControlItem> items)
            : base(fm)
        {
            this.items = items;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override Fragment GetItem(int position)
        {
            Fragment fragment = null;

            var item = items[position];

            if (item.ViewType == SlideControlFragment.ViewTypes.Store)
            {
                fragment = new StoreDetailFragment();
                fragment.Arguments = new Bundle();
                fragment.Arguments.PutString(BundleUtils.Id, item.Id1);
            }
            else if (item.ViewType == SlideControlFragment.ViewTypes.MenuItem)
            {
                fragment = new MenuItemFragment();
                fragment.Arguments = new Bundle();
                fragment.Arguments.PutString(BundleUtils.NodeId, item.Id1);
                fragment.Arguments.PutString(BundleUtils.MenuId, item.Id2);
                fragment.Arguments.PutInt(BundleUtils.Type, item.Type);
            }
            else if (item.ViewType == SlideControlFragment.ViewTypes.MenuGroup)
            {
                fragment = new MenuNodeFragment();
                fragment.Arguments = new Bundle();
                fragment.Arguments.PutString(BundleUtils.NodeId, item.Id1);
                fragment.Arguments.PutString(BundleUtils.MenuId, item.Id2);
            }
            else if (item.ViewType == SlideControlFragment.ViewTypes.Offer)
            {
                fragment = new OfferDetailFragment();
                fragment.Arguments = new Bundle();
                fragment.Arguments.PutString(BundleUtils.OfferId, item.Id1);
            }
            else if (item.ViewType == SlideControlFragment.ViewTypes.Coupon)
            {
                fragment = new CouponDetailFragment();
                fragment.Arguments = new Bundle();
                fragment.Arguments.PutString(BundleUtils.CouponId, item.Id1);
            }

            return fragment;
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            var fragment = base.InstantiateItem(container, position);

            if (fragment is Fragment)
            {
                fragments.Add(position, fragment as Fragment);
            }

            return fragment;
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {

            var fragment = base.InstantiateItem(container, position);

            if (fragment is Fragment)
            {
                fragments.Add(position, fragment as Fragment);
            }

            return fragment;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            fragments.Remove(position);

            base.DestroyItem(container, position, @object);
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object @object)
        {
            fragments.Remove(position);

            base.DestroyItem(container, position, @object);
        }

        public Fragment GetFragment(int position)
        {
            if (fragments.ContainsKey(position))
            {
                return fragments[position];
            }
            return null;
        }
    }
}