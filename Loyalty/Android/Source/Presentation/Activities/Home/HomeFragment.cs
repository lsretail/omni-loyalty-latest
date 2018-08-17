using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Util;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;

namespace Presentation.Activities.Home
{
    public class HomeFragment : LoyaltyFragment, IRefreshableActivity, IBroadcastObserver, View.IOnClickListener, ViewPager.IOnPageChangeListener
    {
        private const int TimeBetweenAds = 5000;
        private Timer adTimer;

        private AdvertisementModel advertisementModel;
        private ViewPager pager;
        private LinePageIndicator pagerIndicator;
        private ProgressBar adProgress;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;

            var view = Inflate(inflater, Resource.Layout.Home);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.HomeScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            adTimer = new Timer(TimeBetweenAds / 100);

            pager = view.FindViewById<ViewPager>(Resource.Id.HomePager);
            pagerIndicator = view.FindViewById<LinePageIndicator>(Resource.Id.HomePagerIndicator);
            adProgress = view.FindViewById<ProgressBar>(Resource.Id.HomePagerProgress);

            var itemButton = view.FindViewById<Button>(Resource.Id.HomeButtonOne);
            var storeButton = view.FindViewById<Button>(Resource.Id.HomeButtonTwo);

            itemButton.SetOnClickListener(this);
            storeButton.SetOnClickListener(this);

            pagerIndicator.SetOnPageChangeListener(this);

            advertisementModel = new AdvertisementModel(Activity, this);

            return view;
        }

        public override void OnPause()
        {
            adTimer.Stop();
            
            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).AddObserver(this);
            }

            if (AppData.Advertisements != null && AppData.Advertisements.Count > 0)
            {
                LoadAdvertisements();
            }
            else
            {
                LoadAdvertisementsFromServer();
            }
        }

        private async void LoadAdvertisementsFromServer()
        {
            await advertisementModel.AdvertisementsGetById(AppData.Device?.UserLoggedOnToDevice?.Id);
        }

        public void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.AdvertisementsUpdated:
                    LoadAdvertisements();
                    break;
            }
        }

        private void NextPage()
        {
            Activity.RunOnUiThread(() =>
            {
                if (pager != null)
                {
                    if (pager.CurrentItem == pager.Adapter.Count - 1)
                    {
                        pager.SetCurrentItem(0, true);
                    }
                    else
                    {
                        pager.CurrentItem = pager.CurrentItem + 1;
                    }
                }
            });
        }

        public void LoadAdvertisements()
        {
            pager.Adapter = new HomeAdPagerAdapter(ChildFragmentManager, AppData.Advertisements);

            if (Build.VERSION.SdkInt >= Util.Utils.ViewPagerUtils.ParallaxPageTransformer.MinVersion)
            {
                pager.SetPageTransformer(false, new Utils.ViewPagerUtils.ParallaxPageTransformer(0.5f, 0.5f, new int[] { Resource.Id.HomeAdDescrioptionContainer, Resource.Id.HomeAdDescription }));
            }

            pagerIndicator.SetViewPager(pager);

            adTimer.Elapsed += (sender, args) =>
            {
                adProgress.Progress = adProgress.Progress + 1;

                if (adProgress.Progress >= 100)
                {
                    adProgress.Progress = 0;

                    NextPage();
                }
            };

            adTimer.Start();
        }

        public void ShowIndicator(bool show)
        {
            
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.HomeButtonOne:
                    if (Activity is LoyaltyFragmentActivity)
                    {
                        (Activity as LoyaltyFragmentActivity).SelectItem(LoyaltyFragmentActivity.ActivityTypes.Items);
                    }
                    break;

                case Resource.Id.HomeButtonTwo:
                    if (Activity is LoyaltyFragmentActivity)
                    {
                        (Activity as LoyaltyFragmentActivity).SelectItem(LoyaltyFragmentActivity.ActivityTypes.Locations);
                    }
                    break;
            }
        }

        public void OnPageScrollStateChanged(int state)
        {
            if ((ScrollState)state == ScrollState.Idle && !adTimer.Enabled)
            {
                adTimer.Start();
            }
            else if (((ScrollState)state == ScrollState.Fling || (ScrollState)state == ScrollState.TouchScroll) && adTimer.Enabled)
            {
                adTimer.Stop();
            }
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected(int position)
        {
            adProgress.Progress = 0;
        }
    }
}