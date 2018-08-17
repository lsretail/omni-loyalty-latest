using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using Presentation.Activities.Base;
using Presentation.Activities.Menu;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;
using Xamarin.ViewPagerIndicator;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Home
{
    public class HomeFragment : BaseFragment, View.IOnClickListener, ViewPager.IOnPageChangeListener, View.IOnTouchListener, IBroadcastObserver, IRefreshableActivity
//, IItemClickListener, IRefreshableActivity, IBroadcastObserver
    {
        private enum OptionButtonType
        {
            Menu,
            Store
        }

        private const OptionButtonType optionOne = OptionButtonType.Menu;
        private const OptionButtonType optionTwo = OptionButtonType.Store;

        private HomeModel homeModel;

        private View optionsButtonOne;
        private View optionsButtonTwo;
        private ViewGroup hintContainer;

        private const int TimeBetweenAds = 5000;
        private Timer adTimer;

        private float downX;
        private float downY;
        private bool headerTouchCancelled;
        private int tapScroll;

        private ViewPager adPager;
        private ProgressBar adProgress;
        private View adPagerContainer;
        private LinePageIndicator pagerIndicator;
        private View progress;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.HomeScreen, null);

            var content = view.FindViewById(Resource.Id.HomeScreenContent);
            content.SetMinimumHeight(Utils.Utils.ViewUtils.GetContentViewHeight(Activity));

            var toolBar = view.FindViewById<Toolbar>(Resource.Id.HomeScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolBar);

            homeModel = new HomeModel(Activity, this);

            adTimer = new Timer(TimeBetweenAds / 100);

            tapScroll = Convert.ToInt32(Resources.DisplayMetrics.WidthPixels * 0.03);

            optionsButtonOne = view.FindViewById(Resource.Id.HomeScreenOptionOne);
            optionsButtonTwo = view.FindViewById(Resource.Id.HomeScreenOptionTwo);

            SetOptionsButton(optionsButtonOne.FindViewById<ImageView>(Resource.Id.HomeScreenOptionOneImage), optionsButtonOne.FindViewById<TextView>(Resource.Id.HomeScreenOptionOneText), optionOne);
            SetOptionsButton(optionsButtonTwo.FindViewById<ImageView>(Resource.Id.HomeScreenOptionTwoImage), optionsButtonTwo.FindViewById<TextView>(Resource.Id.HomeScreenOptionTwoText), optionTwo);

            optionsButtonOne.SetOnClickListener(this);
            optionsButtonTwo.SetOnClickListener(this);

            hintContainer = view.FindViewById<ViewGroup>(Resource.Id.HomeScreenHintContainer);
            CreateHintCards(hintContainer, inflater);

            adPager = view.FindViewById<ViewPager>(Resource.Id.HomeAdItemViewItemPager);
            adPagerContainer = view.FindViewById<View>(Resource.Id.HomeScreenAdContainer);
            adProgress = view.FindViewById<ProgressBar>(Resource.Id.HomeAdItemViewItemImagePagerProgress);
            pagerIndicator = view.FindViewById<LinePageIndicator>(Resource.Id.HomeAdItemViewItemPagerIndicator);
            progress = view.FindViewById(Resource.Id.HomeScreenAdProgress);

            adPagerContainer.SetOnClickListener(this);

            var contactCardText = view.FindViewById<TextView>(Resource.Id.HomeScreenContactCardTitle);

            if (AppData.Contact == null)
            {
                if (contactCardText != null)
                    contactCardText.Text = Resources.GetString(Resource.String.HomeLogInToReceivePoints);
            }
            else
            {
                if (contactCardText != null)
                {
                    if (AppData.Contact.Account == null)
                    {
                        contactCardText.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        contactCardText.Visibility = ViewStates.Visible;
                        contactCardText.Text = string.Format(Resources.GetString(Resource.String.ContactYouHavePoints), AppData.Contact.Account.PointBalance.ToString("n0"));
                    }
                }
            }

            view.FindViewById(Resource.Id.HomeScreenContactCard).SetOnClickListener(this);

            if (AppData.Advertisements == null || AppData.Advertisements.Count == 0)
            {
                homeModel.GetAdvertisements();
            }
            else
            {
                CreateAds();
            }

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            adTimer.Start();

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            adTimer.Stop();

            base.OnPause();
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.AdsUpdated)
            {
                CreateAds();
            }
        }

        private void CreateHintCards(ViewGroup container, LayoutInflater inflater)
        {
            if(container == null)
                return;

            container.RemoveAllViews();

            if (AppData.Contact != null && !PreferenceUtils.GetBool(Activity, PreferenceUtils.PointPullDownHintDismissed) && AppData.Transactions != null && AppData.Transactions.Count > 0)
            {
                var hintCard = Inflate(inflater, Resource.Layout.HomeHintCardItem);

                var image = hintCard.FindViewById<ImageView>(Resource.Id.HomeHintItemViewItemImage);
                var title = hintCard.FindViewById<TextView>(Resource.Id.HomeHintItemViewItemTitle);
                var subTitle = hintCard.FindViewById<TextView>(Resource.Id.HomeHintItemViewItemSubtitle);
                var gotIt = hintCard.FindViewById<Button>(Resource.Id.HomeHintItemViewItemGotIt);

                image.SetImageResource(Resource.Drawable.info);
                title.Text = Resources.GetString(Resource.String.HomeAccountPoints);
                subTitle.Text = Resources.GetString(Resource.String.HomeAccountPointsHint);

                gotIt.SetOnClickListener(this);
                gotIt.GetCompoundDrawables()[0].SetColorFilter(Utils.Utils.GetColorFilter(new Color(ContextCompat.GetColor(Activity, Resource.Color.accent))));

                container.AddView(hintCard);
            }
        }

        private void CreateAds()
        {
            if (AppData.Advertisements == null || AppData.Advertisements.Count == 0)
            {
                adPagerContainer.Visibility = ViewStates.Gone;
                return;
            }
            else
            {
                adPagerContainer.Visibility = ViewStates.Visible;
            }
            
            if (adPager.Adapter == null)
            {
                var adapter = new HomeAdPagerAdapter(ChildFragmentManager);
                adapter.Advertisements = AppData.Advertisements;
                adPager.Adapter = adapter;
            }
            else
            {
                adPager.Adapter.NotifyDataSetChanged();
            }

            adPager.SetPageTransformer(false, new ParallaxPageTransformer(0.5f, 0.5f, new int[] { Resource.Id.HomeAdScreenDescrioptionContainer, Resource.Id.HomeAdScreenDescription }));

            pagerIndicator.SetViewPager(adPager);
            pagerIndicator.SetOnPageChangeListener(this);

            adPager.SetOnTouchListener(this);
            adPagerContainer.SetOnClickListener(this);

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

        private void SetOptionsButton(ImageView imageView, TextView textView, OptionButtonType type)
        {
            if (type == OptionButtonType.Menu)
            {
                imageView.SetImageResource(Resource.Drawable.ic_view_quilt_white_24dp);
                textView.Text = Resources.GetString(Resource.String.ActionBarMenu);
            }
            else if (type == OptionButtonType.Store)
            {
                imageView.SetImageResource(Resource.Drawable.ic_place_white_24dp);
                textView.Text = Resources.GetString(Resource.String.ActionBarStores);
            }

            imageView.SetColorFilter(Utils.Utils.GetColorFilter(new Color(ContextCompat.GetColor(Activity, Resource.Color.accent))));
        }

        private void OptionsButtonSelected(OptionButtonType type)
        {
            switch (type)
            {
                case OptionButtonType.Menu:
                    if (Activity is HomeActivity)
                        (Activity as HomeActivity).SelectItem(HospActivity.ActivityTypes.Menu);
                    break;

                case OptionButtonType.Store:
                    if (Activity is HomeActivity)
                        (Activity as HomeActivity).SelectItem(HospActivity.ActivityTypes.Locations);
                    break;
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.HomeScreenOptionOne:
                    OptionsButtonSelected(optionOne);
                    break;

                case Resource.Id.HomeScreenOptionTwo:
                    OptionsButtonSelected(optionTwo);
                    break;

                case Resource.Id.HomeHintItemViewItemGotIt:
                    PreferenceUtils.SetBool(Activity, PreferenceUtils.PointPullDownHintDismissed, true);
                    CreateHintCards(hintContainer, LayoutInflater.FromContext(Activity));
                    break;

                case Resource.Id.HomeScreenContactCard:
                    if (Activity is HomeActivity)
                        (Activity as HomeActivity).SelectItem(HospActivity.ActivityTypes.Login);

                    break;

                case Resource.Id.HomeScreenAdContainer:
                    var ad = AppData.Advertisements[adPager.CurrentItem];

                    if (ad.AdType == AdvertisementType.Url)
                    {
                        var intent = new Intent(Intent.ActionView);

                        if (!ad.AdValue.StartsWith("http://") && !ad.AdValue.StartsWith("https://"))
                        {
                            ad.AdValue = "http://" + ad.AdValue;
                        }

                        intent.SetData(Android.Net.Uri.Parse(ad.AdValue));
                        ActivityUtils.StartActivityWithAnimation(Activity, intent, v);
                    }
                    else if (ad.AdType == AdvertisementType.MenuNodeId)
                    {
                        var intent = new Intent();
                        intent.SetClass(Activity, typeof(MenuNodeActivity));
                        intent.PutExtra(BundleUtils.NodeId, ad.AdValue);
                        ActivityUtils.StartActivityWithAnimation(Activity, intent, v);
                    }
                    else if (ad.AdType == AdvertisementType.ItemId)
                    {
                        var intent = new Intent();
                        intent.SetClass(Activity, typeof(MenuItemActivity));
                        intent.PutExtra(BundleUtils.ItemId, ad.AdValue);
                        intent.PutExtra(BundleUtils.Type, (int)NodeLineType.ProductOrRecipe);
                        //ActivityUtils.StartActivityWithAnimation(Activity, intent, v); 
                        ActivityUtils.StartActivityForResultWithAnimation(Activity, HospActivity.OpenMenuItemRequestCode, intent, v);

                    }
                    else if (ad.AdType == AdvertisementType.Deal)
                    {
                        var intent = new Intent();
                        intent.SetClass(Activity, typeof(MenuItemActivity));
                        intent.PutExtra(BundleUtils.ItemId, ad.AdValue);
                        intent.PutExtra(BundleUtils.Type, (int)NodeLineType.Deal);
                        //ActivityUtils.StartActivityWithAnimation(Activity, intent, v);
                        ActivityUtils.StartActivityForResultWithAnimation(Activity, HospActivity.OpenMenuItemRequestCode, intent, v);
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

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.HomeAdItemViewItemPager:
                    var returnValue = adPager.OnTouchEvent(e);

                    if (e.Action == MotionEventActions.Down)
                    {
                        adPagerContainer.OnTouchEvent(e);

                        downX = e.GetX();
                        downY = e.GetY();

                        headerTouchCancelled = false;

                        adTimer.Stop();
                    }
                    else if (e.Action == MotionEventActions.Move)
                    {
                        if (Math.Abs(downX - e.GetX()) > tapScroll || Math.Abs(downY - e.GetY()) > 100)
                        {
                            headerTouchCancelled = true;
                            e.Action = MotionEventActions.Cancel;
                            adPagerContainer.OnTouchEvent(e);
                        }
                    }
                    else
                    {
                        if (!adTimer.Enabled)
                            adTimer.Start();

                        if (!headerTouchCancelled)
                        {
                            adPagerContainer.OnTouchEvent(e);
                        }
                    }
                    return returnValue;
            }

            return false;
        }

        private void NextPage()
        {
            (Activity as HospActivity).RunOnUiThread(() =>
            {
                if (adPager != null)
                {
                    if (adPager.CurrentItem == adPager.Adapter.Count - 1)
                    {
                        adPager.SetCurrentItem(0, true);
                    }
                    else
                    {
                        adPager.CurrentItem = adPager.CurrentItem + 1;
                    }
                }
            });

        }

        public class HomeAdPagerAdapter : FragmentStatePagerAdapter
        {
            public List<Advertisement> Advertisements { get; set; }

            public HomeAdPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public HomeAdPagerAdapter(FragmentManager fm)
                : base(fm)
            {
            }

            public override int Count
            {
                get
                {
                    if (Advertisements == null)
                        return 0;
                    return Advertisements.Count;
                }
            }

            public override Fragment GetItem(int position)
            {
                var fragment = new HomeAdFragment();

                fragment.Arguments = new Bundle();
                fragment.Arguments.PutString(BundleUtils.Id, Advertisements[position].Id);

                return fragment;
            }
        }


        public void ShowIndicator(bool show)
        {
            if (show)
            {
                adPagerContainer.Visibility = ViewStates.Gone;
                progress.Visibility = ViewStates.Visible;
            }
            else
            {
                progress.Visibility = ViewStates.Gone;
            }
        }
    }
}