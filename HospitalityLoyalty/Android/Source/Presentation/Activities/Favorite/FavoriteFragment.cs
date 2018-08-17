using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Models;
using Xamarin.Astuetz;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Favorite
{
    public class FavoriteFragment : BaseFragment, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private ViewPager pager;
        private TabLayout tablayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.LoginScreen, null);

            pager = view.FindViewById<ViewPager>(Resource.Id.LoginScreenPager);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.LoginScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            pager.Adapter = new FavoritePagerAdapter(ChildFragmentManager, Activity);

            tablayout = view.FindViewById<TabLayout>(Resource.Id.LoginScreenTabLayout);

            Utils.Utils.ViewUtils.AddOnGlobalLayoutListener(view, this);

            return view;
        }

        public void OnGlobalLayout()
        {
            Utils.Utils.ViewUtils.RemoveOnGlobalLayoutListener(View, this);

            tablayout.SetupWithViewPager(pager);
        }
    }
}