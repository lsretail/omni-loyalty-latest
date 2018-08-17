using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Domain.Advertisements;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Models;
using Presentation.Utils;
using Xamarin.ViewPagerIndicator;

namespace Presentation.Adapters
{
    class HomeAdapter : BaseCardAdapter<Object>, ViewPager.IOnPageChangeListener, View.IOnTouchListener
    {
        private readonly FragmentManager fragmentManager;
        private const int TimeBetweenAds = 5000;
        private Timer adTimer;

        private ImageModel model;
        private List<Advertisement> advertisements;

        private float downX;
        private float downY;
        private bool headerTouchCancelled;
        private int tapScroll;

        private bool paused;

        private ViewPager adPager;
        private ProgressBar adProgress;
        private View adPagerContainer;

        public HomeAdapter(IItemClickListener listener, Context context, FragmentManager fragmentManager)
            : base(listener, context)
        {
            this.fragmentManager = fragmentManager;
            adTimer = new Timer(TimeBetweenAds / 100);

            Display display = (context as BaseFragmentActivity).WindowManager.DefaultDisplay;
            tapScroll = Convert.ToInt32(display.Width * 0.03);

            model = new ImageModel(context);

            Items = new List<Card<Object>>();

            LineTypes.Add(LineType.SmallExtraSmallTwoCards);
            LineTypes.Add(LineType.SingleWrappedCard);
            LineTypes.Add(LineType.MediumSingleCard);
            LineTypes.Add(LineType.SmallSingleCard);

            CreateItems();
        }

        public void SetAds(List<Advertisement> advertisements)
        {
            this.advertisements = advertisements;

            CreateItems();
        }

        private void CreateItems()
        {
            Items.Clear();

            Items.Add(new Card<Object>() { ContentType = Card<Object>.CardContentType.Menu });
            Items.Add(new Card<Object>() { ContentType = Card<Object>.CardContentType.Stores });

            if(AppData.Contact != null && !PreferenceUtils.GetBool(Context, PreferenceUtils.PointPullDownHintDismissed) && AppData.Transactions != null && AppData.Transactions.Count > 0)
                Items.Add(new Card<object>() {ContentType = Card<object>.CardContentType.Hint});

            if (AppData.Advertisements != null && AppData.Advertisements.Count > 0)
            {
                Items.Add(new Card<object>() { ContentType = Card<object>.CardContentType.HomeAd, Item = advertisements });
                Items.Add(new Card<object>() { ContentType = Card<object>.CardContentType.LogoPoints });
            }

            NotifyDataSetChanged();
        }

        /*public HomeAdapter(IItemClickListener listener, Context context, List<HomeItem> homeItems)
            : base(listener, context)
        {
            model = new ImageModel(context);

            Items = new List<Card<HomeItem>>();

            SetHomeItems(homeItems);

            LineTypes.Add(LineType.SmallCards);
            LineTypes.Add(LineType.SmallExtraSmallTwoCards);
            LineTypes.Add(LineType.MediumCardAndSmallCards);
        }*/

        /*public void SetHomeItems(List<HomeItem> homeItems)
        {
            Items.Clear();

            Items.Add(new Card<HomeItem>() { ContentType = Card<HomeItem>.CardContentType.Menu });
            Items.Add(new Card<HomeItem>() { ContentType = Card<HomeItem>.CardContentType.Stores });

            foreach (var homeItem in homeItems)
            {
                Items.Add(new Card<HomeItem>()
                {
                    ContentType = Card<HomeItem>.CardContentType.MenuItem,
                    Item = homeItem
                });
            }
        }*/

        public void OnResume()
        {
            paused = false;

            if (adPager != null)
            {
                adTimer.Start();
            }
        }

        public void OnPause()
        {
            if (adPager != null)
            {
                adTimer.Stop();
            }

            paused = true;
        }

        public override int GetViewType(int position)
        {
            if (position == 0)
                return LineTypes.IndexOf(LineType.SmallExtraSmallTwoCards);

            var hasHint = AppData.Contact != null && !PreferenceUtils.GetBool(Context, PreferenceUtils.PointPullDownHintDismissed) && AppData.Transactions != null && AppData.Transactions.Count > 0;

            if (position == 1 && hasHint)
                return LineTypes.IndexOf(LineType.SingleWrappedCard);

            if(AppData.Advertisements != null && AppData.Advertisements.Count > 0 && ((position == 1 && !hasHint) || (hasHint && position == 2)))
                return LineTypes.IndexOf(LineType.MediumSingleCard);

            return LineTypes.IndexOf(LineType.SmallSingleCard);
            /*else if(position == 1)
                return LineTypes.IndexOf(LineType.MediumCardAndSmallCards);
            else
                return LineTypes.IndexOf(LineType.SmallCards);*/
        }

        public override void OnClick(View v)
        {
            var tag = (string) v.Tag;

            var row = GetRowFromTag(tag);
            var col = GetColFromTag(tag);

            var item = GetItem(row, col);

            switch (v.Id)
            {
                case Resource.Id.CardButton:
                    PreferenceUtils.SetBool(Context, PreferenceUtils.PointPullDownHintDismissed, true);
                    CreateItems();
                    break;

                default:
                    if (item.ContentType == Card<Object>.CardContentType.Stores)
                        Listener.ItemClicked(ItemType.Stores, "", "", v);
                    else if (item.ContentType == Card<Object>.CardContentType.Menu)
                        Listener.ItemClicked(ItemType.Menu, "", "", v);
                    else if (item.ContentType == Card<Object>.CardContentType.HomeAd)
                    {
                        var ads = item.Item as List<Advertisement>;

                        if (ads != null)
                        {
                            var currentAd = ads[adPager.CurrentItem];

                            Listener.ItemClicked(ItemType.HomeItem, currentAd.AdType.ToString(), currentAd.AdValue, v);
                        }
                    }
                    break;
            }
        }

        public override void FillCard(int row, int col, Card<Object>.CardType cardType, View view)
        {
            if (view == null)
                return;

            var card = GetItem(row, col);

            if (card == null)
            {
                view.Visibility = ViewStates.Invisible;

                view.SetOnClickListener(null);
            }
            else
            {
                view.Visibility = ViewStates.Visible;

                view.SetOnClickListener(this);

                view.Tag = GenerateTag(row, col);

                if (card.ContentType == Card<object>.CardContentType.HomeAd)
                {
                    var ads = card.Item as List<Advertisement>;

                    adPager = view.FindViewById<ViewPager>(Resource.Id.CardHomeAdPager);
                    var pagerIndicator = view.FindViewById<LinePageIndicator>(Resource.Id.CardHomeAdPagerIndicator);
                    //adProgress = view.FindViewById<ProgressBar>(Resource.Id.CardHomeAdPagerProgress);
                    adPagerContainer = view.FindViewById<View>(Resource.Id.CardHomeAdPagerContainer);

                    adPagerContainer.Tag = GenerateTag(row, col);

                    if (adPager.Adapter == null)
                    {
                        var adapter = new HomeAdPagerAdapter(fragmentManager);
                        adapter.Advertisements = ads;
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
                else if (card.ContentType == Card<object>.CardContentType.Hint)
                {
                    var image = view.FindViewById<ImageView>(Resource.Id.CardImage);
                    var desription = view.FindViewById<TextView>(Resource.Id.CardDescription);
                    var details = view.FindViewById<TextView>(Resource.Id.CardDetails);
                    var button = view.FindViewById<Button>(Resource.Id.CardButton);

                    image.SetImageResource(Resource.Drawable.info);
                    image.SetColorFilter(Utils.Utils.GetColorFilter(Context.Resources.GetColor(Resource.Color.white)));

                    button.GetCompoundDrawables()[0].SetColorFilter(Utils.Utils.GetColorFilter(Context.Resources.GetColor(Resource.Color.accent)));
                    button.Tag = GenerateTag(row, col);
                    button.SetOnClickListener(this);

                    desription.Text = "Account points";
                    details.Text = "Pull down on left side menu or contact information screen to refresh points";
                }
                else if (card.ContentType == Card<Object>.CardContentType.Stores)
                {
                    var image = view.FindViewById<ImageView>(Resource.Id.CardSmallExtraSmallItemImage);
                    var description = view.FindViewById<TextView>(Resource.Id.CardSmallExtraSmallItemName);

                    if (image != null)
                    {
                        image.SetImageResource(Resource.Drawable.ic_action_place_dark);
                        image.SetColorFilter(Utils.Utils.GetColorFilter(Context.Resources.GetColor(Resource.Color.accent)));
                    }
                    if (description != null)
                        description.Text = Context.Resources.GetString(Resource.String.ActionBarStores);
                }
                else if (card.ContentType == Card<Object>.CardContentType.Menu)
                {
                    var image = view.FindViewById<ImageView>(Resource.Id.CardSmallExtraSmallItemImage);
                    var description = view.FindViewById<TextView>(Resource.Id.CardSmallExtraSmallItemName);

                    if (image != null)
                    {
                        image.SetImageResource(Resource.Drawable.ic_action_dashboard);
                        image.SetColorFilter(Utils.Utils.GetColorFilter(Context.Resources.GetColor(Resource.Color.accent)));
                    }
                    if (description != null)
                        description.Text = Context.Resources.GetString(Resource.String.ActionBarMenu);
                }
                else if (card.ContentType == Card<object>.CardContentType.LogoPoints)
                {
                    var image = view.FindViewById<ImageView>(Resource.Id.CardImage);
                    var imageContainer = view.FindViewById<View>(Resource.Id.CardImageContainer);
                    var description = view.FindViewById<TextView>(Resource.Id.CardDescription);
                    var details = view.FindViewById<TextView>(Resource.Id.CardDetails);

                    image.SetImageResource(Resource.Drawable.ic_login_picture);
                    //imageContainer.SetBackgroundResource(Resource.Color.accent);

                    if (AppData.Contact == null)
                    {
                        if (description != null)
                            description.Text = Context.Resources.GetString(Resource.String.HomeLogInToReceivePoints);

                        if (details != null)
                            details.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        if (description != null)
                            description.Text = AppData.Contact.FirstName;

                        if (details != null)
                        {
                            if (AppData.Contact.Account == null)
                            {
                                details.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                details.Visibility = ViewStates.Visible;
                                details.Text = string.Format(Context.Resources.GetString(Resource.String.ContactPoints), AppData.Contact.Account.PointBalance.ToString("n0"));
                            }
                        }
                    }
                }
            }
        }

        public void OnPageScrollStateChanged(int state)
        {
            if ((ScrollState) state == ScrollState.Idle && !adTimer.Enabled)
            {
                adTimer.Start();
            }
            else if(((ScrollState) state == ScrollState.Fling || (ScrollState) state == ScrollState.TouchScroll) && adTimer.Enabled)
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
                case Resource.Id.CardHomeAdPager:
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
                        if(!adTimer.Enabled)
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
            (Context as BaseFragmentActivity).RunOnUiThread(() =>
                {
                    if (adPager != null && !paused)
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

            public HomeAdPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public HomeAdPagerAdapter(FragmentManager fm) : base(fm)
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
    }
}