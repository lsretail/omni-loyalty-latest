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
using Presentation.Activities.Store;
using Presentation.Utils;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Presentation.Activities.Base
{
    public class BasePagerAdapter : FragmentStatePagerAdapter
    {
        private readonly List<string> ids;
        private readonly ViewTypes viewType;

        public enum ViewTypes
        {
            None = 0,
            Store = 1
        }

        public BasePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public BasePagerAdapter(FragmentManager fm, List<string> ids, ViewTypes viewType) : base(fm)
        {
            this.ids = ids;
            this.viewType = viewType;
        }

        public override int Count
        {
            get { return ids.Count; }
        }

        public override Fragment GetItem(int position)
        {
            Fragment fragment = null;

            if (viewType == ViewTypes.Store)
            {
                fragment = new StoreDetailFragment();
                fragment.Arguments.PutString(BundleUtils.Id, ids[position]);
            }

            return fragment;
        }
    }
}