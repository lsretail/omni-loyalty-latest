using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Home;
using Presentation.Activities.Items;
using Presentation.Util;
using DrawerMenuItemAdapter = Presentation.Adapters.DrawerMenuItemAdapter;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.Base
{
    public abstract class LoyaltyFragmentActivity : AppCompatActivity, IBroadcastObserver, AdapterView.IOnItemClickListener, DrawerLayout.IDrawerListener//, Session.IStatusCallback, Request.ICallback, WebDialog.IOnCompleteListener
    {
        public class ActivityTypes
        {
            public const int None = 0;
            public const int Login = 1;
            public const int Items = 2;
            public const int Offer = 3;
            public const int Coupons = 4;
            public const int Locations = 5;
            public const int Home = 6;
            public const int Transactions = 7;
            public const int Logout = 8;
            public const int DefaultItem = 9;
            public const int Account = 10;
            public const int Notifications = 11;
            public const int ShoppingLists = 12;
            public const int Search = 13;
        }

        private List<IBroadcastObserver> observers;
        private bool hasLeftDrawer = true;
        private bool hasRightDrawer = true;
        private Display display;
        private List<int> requestedWindowFeatures;
        private bool rightDrawerHasBeenLoaded = false;

        private int statusBarHeight;
        protected ColorDrawable ActionBarBackgroundDrawable { get; set; }

        protected List<DrawerMenuItem> DrawerMenuItems { get; private set; }
        protected DrawerLayout DrawerLayout { get; private set; }
        protected ListView DrawerLeft { get; private set; }
        protected View DrawerRight { get; private set; }
        protected int ActivityType { get; set; }
        protected BroadcastReceiver Receiver { get; private set; }

        protected int LeftDrawerId { get; private set; }
        protected int RightDrawerId { get; private set; }
        protected int ContentId { get; private set; }
        //protected Android.Support.V7.Widget.Toolbar LoyaltyToolbar { get; private set; }

        //public bool TransparentActionBar { get; protected set; }
        public int HeaderImageResource { get; protected set; }

        //Social Media
        public bool HasSocialMediaConnection { get; protected set; }

        protected class PendingAction
        {
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string Details { get; set; }
            public string ImageLink { get; set; }
        }

        protected string AccountName { get; set; }
        protected string FirstName { get; set; }
        protected string MiddleName { get; set; }
        protected string LastName { get; set; }
        protected PendingAction pendingAction;

        protected bool StartSocialMediaOnResume { get; set; }

        protected List<int> RequestedWindowFeatures
        {
            get
            {
                if (requestedWindowFeatures == null)
                    requestedWindowFeatures = new List<int>();

                return requestedWindowFeatures;
            }
            private set { requestedWindowFeatures = value; }
        }

        public int StatusBarHeight
        {
            get { return statusBarHeight; }
        }

        public bool HasLeftDrawer
        {
            get { return hasLeftDrawer; }
            protected set { hasLeftDrawer = value; }
        }

        public virtual bool HasRightDrawer
        {
            get
            {
                if (EnabledItems.HasBasket)
                {
                    if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
                    {
                        return hasRightDrawer;
                    }
                }

                return false;
            }
            protected set { hasRightDrawer = value; }
        }

        private const string BasketFragmentLabel = "BasketFragmentLabel";

        protected int DefaultActivityType
        {
            get
            {
                if (EnabledItems.HasHome)
                {
                    return ActivityTypes.Home;
                }

                if (EnabledItems.HasItemCatalog)
                {
                    return ActivityTypes.Items;
                }

                return ActivityTypes.Account;
            }
        }

        private bool hasMenu = true;

        public bool HasMenu
        {
            get { return hasMenu; }
            set { hasMenu = value; }
        }



        protected override void OnCreate(Bundle savedInstanceState)
        {
            HeaderImageResource = Resource.Drawable.logo_drawer;

            RequestedWindowFeatures.Add((int)WindowFeatures.IndeterminateProgress);

            base.OnCreate(savedInstanceState);

            LeftDrawerId = Resource.Id.BaseActivityScreenLeftDrawer;
            RightDrawerId = Resource.Id.BaseActivityScreenRightDrawer;
            ContentId = Resource.Id.BaseActivityScreenContentFrame;

            statusBarHeight = Resources.GetDimensionPixelSize(Resource.Dimension.StatusbarHeight);

            var view = LayoutInflater.Inflate(Resource.Layout.BaseScreenLayoutDualDrawer, null);

            foreach (var windowFeature in RequestedWindowFeatures)
            {
                SupportRequestWindowFeature(windowFeature);
            }

            SetContentView(view);

            //LoyaltyToolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.BaseActivityScreenToolbar);
            //SetSupportActionBar(LoyaltyToolbar);

            //todo better
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                //SupportActionBar.Elevation = 0;
            }

            observers = new List<IBroadcastObserver>();

            DrawerMenuItems = new List<DrawerMenuItem>();

            DrawerLayout = FindViewById<DrawerLayout>(Resource.Id.BaseActivityScreenDrawerLayout);
            DrawerLeft = FindViewById<ListView>(Resource.Id.BaseActivityScreenLeftDrawer);
            DrawerRight = FindViewById<View>(Resource.Id.BaseActivityScreenRightDrawer);

            display = WindowManager.DefaultDisplay;

            var actionBarHeight = Resources.GetDimensionPixelSize(Resource.Dimension.ActionBarHeight);
            var maxDrawerWidth = Resources.GetDimensionPixelSize(Resource.Dimension.MaxDrawerWidth);

            var drawerWidth = Math.Min(display.Width - actionBarHeight, maxDrawerWidth);

            DrawerLeft.LayoutParameters.Width = drawerWidth;
            DrawerRight.LayoutParameters.Width = drawerWidth;

            if (HeaderImageResource > 0)
            {
                var imageView = new Android.Widget.ImageView(this);
                imageView.LayoutParameters = new AbsListView.LayoutParams(AbsListView.LayoutParams.MatchParent, AbsListView.LayoutParams.WrapContent);
                imageView.SetScaleType(Android.Widget.ImageView.ScaleType.CenterInside);
                imageView.SetImageResource(HeaderImageResource);
                imageView.SetAdjustViewBounds(true);

                var padding = Resources.GetDimensionPixelSize(Resource.Dimension.BasePadding);

                imageView.SetPadding(padding, padding, padding, padding);
                imageView.SetMaxHeight(Resources.GetDimensionPixelSize(Resource.Dimension.MaxDrawerHeaderHeight));

                imageView.SetBackgroundResource(Resource.Color.accent);

                DrawerLeft.AddHeaderView(imageView, null, false);
            }

            // Set the adapter for the list view
            DrawerLeft.Adapter = new DrawerMenuItemAdapter(this, DrawerMenuItems);
            // Set the list's click listener
            DrawerLeft.OnItemClickListener = this;

            DrawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow_left, GravityCompat.Start);
            DrawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow_right, GravityCompat.End);

            Receiver = new Receiver(AlertObservers);

            if (HasLeftDrawer)
            {
                EnableDrawer(GravityCompat.Start);
            }
            else
            {
                DisableDrawer(GravityCompat.Start);
            }

            if (savedInstanceState != null)
            {
                ActivityType = savedInstanceState.GetInt("ActivityType");
            }

            DrawerLayout.SetDrawerListener(this);

            if (HasSocialMediaConnection)
            {
                //StartSocialMediaOnResume = true;

                //uiHelper = new UiLifecycleHelper(this, this);
                //uiHelper.OnCreate(savedInstanceState);

                //TODO
                /*if (savedInstanceState != null)
                {
                    string name = savedInstanceState.GetString(PENDING_ACTION_BUNDLE_KEY);
                    pendingAction = (PendingAction)Enum.Parse(typeof(PendingAction), name);
                }*/

                /*plusClient = new PlusClient.Builder(this, this, this)
                    .SetActions("http://schemas.google.com/AddActivity", "http://schemas.google.com/BuyActivity")
                    .SetScopes(Scopes.PlusLogin).Build();*/

                //var session = Session.ActiveSession;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            CheckDrawerStatus();

            FillDrawerList();

            var filter = new IntentFilter();

            foreach (var broadcastAction in Utils.BroadcastUtils.BroadcastActions)
            {
                filter.AddAction(broadcastAction);
            }

            RegisterReceiver(Receiver, filter);

            AddObserver(this);
        }

        protected override void OnPause()
        {
            RemoveObserver(this);

            UnregisterReceiver(Receiver);

            base.OnPause();
        }

        public void CheckDrawerStatus()
        {
            if (HasRightDrawer && !rightDrawerHasBeenLoaded)
            {
                var basketFragment = new BasketFragment();

                var ft = SupportFragmentManager.BeginTransaction();
                ft.Replace(RightDrawerId, basketFragment, BasketFragmentLabel);
                ft.Commit();

                rightDrawerHasBeenLoaded = true;
            }
            else if (!HasRightDrawer && rightDrawerHasBeenLoaded)
            {
                var basketFragment = SupportFragmentManager.FindFragmentByTag(BasketFragmentLabel);

                if (basketFragment != null)
                {
                    var ft = SupportFragmentManager.BeginTransaction();
                    ft.Remove(basketFragment);
                    ft.Commit();
                }

                rightDrawerHasBeenLoaded = false;
            }

            if (HasRightDrawer)
            {
                EnableDrawer(GravityCompat.End);
            }
            else
            {
                DisableDrawer(GravityCompat.End);
            }
        }

        public override void SetContentView(int layoutResID)
        {
            //base.SetContentView(layoutResID);
            var content = FindViewById<FrameLayout>(ContentId);
            Util.Utils.ViewUtils.Inflate(LayoutInflater, layoutResID, content);
        }

        public void DisableDrawer(int gravity)
        {
            DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed, gravity);
        }

        public void EnableDrawer(int gravity)
        {
            DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked, gravity);
        }

        public void OpenDrawer(int gravity)
        {
            if ((int)GravityFlags.Start == gravity)
            {
                if (IsOpen((int)GravityFlags.End))
                    CloseDrawer((int)GravityFlags.End);
            }
            else
            {
                if (IsOpen((int)GravityFlags.Start))
                    CloseDrawer((int)GravityFlags.Start);
            }

            DrawerLayout.OpenDrawer(gravity);
        }

        public void CloseDrawer(int gravity)
        {
            DrawerLayout.CloseDrawer(gravity);
        }

        public bool IsOpen(int gravity)
        {
            return DrawerLayout.IsDrawerOpen(gravity);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Clear();

            if (HasMenu)
            {
                MenuInflater.Inflate(Resource.Menu.BaseMenu, menu);

                if (AppData.Device.UserLoggedOnToDevice != null)
                {
                    MenuInflater.Inflate(Resource.Menu.AccountManagementMenu, menu);
                }

#if HockeyApp
                MenuInflater.Inflate(Resource.Menu.FeedbackMenu, menu);
#endif
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewAccountManagement:
                    MenuUtils.AccountManagementClicked(this);
                    return true;

                case Resource.Id.MenuViewContactUs:
                    MenuUtils.ContactUsClicked(this);
                    return true;

                case Resource.Id.MenuViewSendFeedback:
                    break;

                case Android.Resource.Id.Home:
                    if (IsOpen((int)GravityFlags.End))
                    {
                        CloseDrawer((int)GravityFlags.End);
                        return true;
                    }

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void FillDrawerList()
        {
            if (!HasLeftDrawer)
                return;

            DrawerMenuItems.Clear();

            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                DrawerMenuItems.Add(new DrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.accounttier)),
                    ActivityType = ActivityTypes.Login,
                    Color = ActivityType == ActivityTypes.Account,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_person_colored,
                    IsLoading = false,
                    Title = GetString(Resource.String.LoginViewLoginButton)
                });
            }
            else if (AppData.Device != null && AppData.Device.UserLoggedOnToDevice != null && AppData.Device.UserLoggedOnToDevice.Account != null)
            {
                DrawerMenuItems.Add(new DrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.accounttier)),
                    ActivityType = ActivityTypes.Account,
                    Color = ActivityType == ActivityTypes.Account,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_person_colored,
                    IsLoading = false,
                    Title = AppData.Device.UserLoggedOnToDevice.UserName,
                    SubTitle = AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0")
                });
            }

            if (EnabledItems.HasHome)
            {
                DrawerMenuItems.Add(new DrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.accent)),
                    ActivityType = ActivityTypes.Home,
                    Color = ActivityType == ActivityTypes.Home,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_home,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarHome),
                });
            }

            if (EnabledItems.HasItemCatalog)
            {
                if (EnabledItems.HasBarcodeScanner)
                {
                    DrawerMenuItems.Add(new SecondaryActionDrawerMenuItem()
                    {
                        Accent = new Color(ContextCompat.GetColor(this, Resource.Color.barcode)),
                        ActivityType = ActivityTypes.Items,
                        Color = ActivityType == ActivityTypes.Items,
                        Enabled = true,
                        Image = Resource.Drawable.ic_action_items_colored,
                        IsLoading = false,
                        Title = GetString(Resource.String.ActionbarItems),
                        SecondaryActionResource = Resource.Drawable.ic_action_barcode_colored,
                        SecondaryAction = async () =>
                            {
                                var scanner = new ZXing.Mobile.MobileBarcodeScanner();

                                scanner.TopText = GetString(Resource.String.ScannerViewScannerTopText);
                                scanner.BottomText = GetString(Resource.String.ScannerViewScannerBottomText);

                                var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
                                options.PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.EAN_13, ZXing.BarcodeFormat.EAN_8 };

                                var result = await scanner.Scan(options);

                                if (result != null && !string.IsNullOrEmpty(result.Text))
                                {
                                    var barcode = result.Text;

                                    var intent = new Intent();

                                    intent.PutExtra(BundleConstants.Barcode, barcode);

                                    intent.SetClass(this, typeof(ItemActivity));
                                    StartActivity(intent);
                                }
                            }
                    });
                }
                else
                {
                    DrawerMenuItems.Add(new DrawerMenuItem()
                    {
                        Accent = new Color(ContextCompat.GetColor(this, Resource.Color.barcode)),
                        ActivityType = ActivityTypes.Items,
                        Color = ActivityType == ActivityTypes.Items,
                        Enabled = true,
                        Image = Resource.Drawable.ic_action_items_colored,
                        IsLoading = false,
                        Title = GetString(Resource.String.ActionbarItems),
                    });
                }
            }

            if (EnabledItems.HasSearch)
            {
                DrawerMenuItems.Add(new DrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.search)),
                    ActivityType = ActivityTypes.Search,
                    Color = ActivityType == ActivityTypes.Search,
                    Enabled = true,
                    Image = Resource.Drawable.ic_search_24dp,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarSearch),
                });
            }

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasNotifications)
            {
                var notificationCount = string.Empty;

                var unreadNotifications = AppData.Device.UserLoggedOnToDevice.Notifications.Count(x => x.Status == NotificationStatus.New);
                if (unreadNotifications > 0)
                {
                    notificationCount = unreadNotifications.ToString();
                }

                DrawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.notifications)),
                    ActivityType = ActivityTypes.Notifications,
                    Color = ActivityType == ActivityTypes.Notifications,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_notifications_colored,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarNotifications),
                    SecondaryText = notificationCount
                });
            }

            if (EnabledItems.HasOffers)
            {
                var offerCount = string.Empty;

                if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
                {
                    if (AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code != OfferDiscountType.Coupon) > 0)
                    {
                        offerCount = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code != OfferDiscountType.Coupon).ToString();
                    }
                }

                DrawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.offers)),
                    ActivityType = ActivityTypes.Offer,
                    Color = ActivityType == ActivityTypes.Offer,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_offers_colored,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarOffers),
                    SecondaryText = offerCount
                });
            }

            if (EnabledItems.HasCoupons)
            {
                var couponCount = string.Empty;

                if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
                {
                    if (AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code == OfferDiscountType.Coupon) > 0)
                    {
                        couponCount = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count(x => x.Code == OfferDiscountType.Coupon).ToString();
                    }
                }

                DrawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.coupons)),
                    ActivityType = ActivityTypes.Coupons,
                    Color = ActivityType == ActivityTypes.Coupons,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_coupons_colored,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarCoupons),
                    SecondaryText = couponCount
                });
            }

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasWishLists)
            {
                var itemCount = string.Empty;
                if (AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Items.Count > 0)
                {
                    itemCount = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Items.Count.ToString();
                }

                DrawerMenuItems.Add(new SecondaryTextDrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.shoppinglists)),
                    ActivityType = ActivityTypes.ShoppingLists,
                    Color = ActivityType == ActivityTypes.ShoppingLists,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_shoplists_colored,
                    IsLoading = false,
                    Title = GetString(Resource.String.ShoppingListDetailViewWishlist),
                    SecondaryText = itemCount
                });
            }

            if (EnabledItems.HasStoreLocator)
            {
                DrawerMenuItems.Add(new DrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.storelocator)),
                    ActivityType = ActivityTypes.Locations,
                    Color = ActivityType == ActivityTypes.Locations,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_map,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarStores),
                });
            }

            if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && EnabledItems.HasHistory)
            {
                DrawerMenuItems.Add(new DrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.transactions)),
                    ActivityType = ActivityTypes.Transactions,
                    Color = ActivityType == ActivityTypes.Transactions,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_history_colored,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarTransactions),
                });
            }

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {
                DrawerMenuItems.Add(new DrawerMenuItem()
                {
                    Accent = new Color(ContextCompat.GetColor(this, Resource.Color.logout)),
                    ActivityType = ActivityTypes.Logout,
                    Color = ActivityType == ActivityTypes.Logout,
                    Enabled = true,
                    Image = Resource.Drawable.ic_action_logout_colored,
                    IsLoading = false,
                    Title = GetString(Resource.String.ActionbarLogout),
                });
            }

            Util.Utils.ListUtils.NotifyAdapterChanged(DrawerLeft.Adapter);
        }

        public virtual void SelectItem(int activityType)
        {
            var upIntent = new Intent();
            upIntent.SetClass(this, typeof(HomeActivity));
            upIntent.AddFlags(ActivityFlags.ClearTop);
            upIntent.AddFlags(ActivityFlags.SingleTop);

            upIntent.PutExtra(BundleConstants.ChosenMenuBundleName, activityType);

            StartActivity(upIntent);
            Finish();
        }

        protected bool IsDefaultItem(int activityType)
        {
            if (activityType == DefaultActivityType)
            {
                return true;
            }

            return false;
        }

        public virtual void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.NotificationsUpdated:
                case Utils.BroadcastUtils.DomainModelUpdated:
                case Utils.BroadcastUtils.OffersUpdated:
                case Utils.BroadcastUtils.CouponsUpdated:
                case Utils.BroadcastUtils.ShoppingListUpdated:
                case Utils.BroadcastUtils.ShoppingListsUpdated:
                case Utils.BroadcastUtils.ShoppingListDeleted:
                case Utils.BroadcastUtils.PointsUpdated:
                    FillDrawerList();
                    break;
            }
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            if (HasLeftDrawer && !Utils.PreferenceUtils.GetBool(this, Utils.PreferenceUtils.NavigationDrawerHasBeenShown))
            {
                OpenDrawer((int)GravityFlags.Start);
                Utils.PreferenceUtils.SetBool(this, Utils.PreferenceUtils.NavigationDrawerHasBeenShown, true);
            }
        }

        /*protected override void OnStart()
        {
            base.OnStart();

            if (HasSocialMediaConnection && StartSocialMediaOnResume)
            {
                if (Utils.SocialMediaUtils.CurrentSocialMediaConnection == Utils.SocialMediaUtils.SocialMediaConnection.Google)
                    plusClient.Connect();
            }
        }*/

        /*protected override void OnStop()
        {
            if (HasSocialMediaConnection && Utils.SocialMediaUtils.CurrentSocialMediaConnection == Utils.SocialMediaUtils.SocialMediaConnection.Google)
                plusClient.Disconnect();

            base.OnStop();
        }*/

        protected override void OnDestroy()
        {
            /*if (HasSocialMediaConnection)
            {
                uiHelper.OnDestroy();
            }*/

            base.OnDestroy();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            /*if (HasSocialMediaConnection)
            {
                if (requestCode == ResolveGooglePlusLoginRequestCode && resultCode == Result.Ok)
                    Connect(Utils.SocialMediaUtils.SocialMediaConnection.Google);
                else
                {
                    uiHelper.OnActivityResult(requestCode, (int)resultCode, data);
                }
            }*/
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            /*if (HasSocialMediaConnection)
            {
                uiHelper.OnSaveInstanceState(outState);
            }*/

            outState.PutInt("ActivityType", ActivityType);
        }

        public void AddObserver(IBroadcastObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IBroadcastObserver observer)
        {
            observers.Remove(observer);
        }

        private void AlertObservers(string action)
        {
            observers.ForEach(x => x.BroadcastReceived(action));
        }

        public virtual void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var realPos = position - DrawerLeft.HeaderViewsCount;

            SelectItem(DrawerMenuItems[realPos].ActivityType);
        }

        public override void OnLowMemory()
        {
            System.GC.Collect();

            base.OnLowMemory();
        }

        public virtual void OnDrawerClosed(View drawerView)
        {
            Utils.BroadcastUtils.SendBroadcast(this, Utils.BroadcastUtils.DrawerClosed);
        }

        public virtual void OnDrawerOpened(View drawerView)
        {
            Utils.BroadcastUtils.SendBroadcast(this, Utils.BroadcastUtils.DrawerOpened);
        }

        public virtual void OnDrawerSlide(View drawerView, float slideOffset)
        {
        }

        public virtual void OnDrawerStateChanged(int newState)
        {
        }

        #region SOCIAL MEDIA CONNECTION

        /*public void Connect(Utils.SocialMediaUtils.SocialMediaConnection connection, bool openPublishConnection = false)
        {
            if (connection == Utils.SocialMediaUtils.SocialMediaConnection.None)
                return;

            switch (connection)
            {
                case Utils.SocialMediaUtils.SocialMediaConnection.Google:
                    if (!plusClient.IsConnected && !plusClient.IsConnecting)
                        plusClient.Connect();
                    break;

                case Utils.SocialMediaUtils.SocialMediaConnection.Facebook:
                    var session = Session.ActiveSession;

                    if (session == null)
                    {
                        session = new Session.Builder(this).SetApplicationId(GetString(Resource.String.AppId)).Build();
                        Session.ActiveSession = session;
                    }
                    else if (!session.IsOpened || (openPublishConnection && !session.Permissions.Contains("publish_actions")))
                    {
                        if (openPublishConnection)
                            if (!session.IsOpened && !session.IsClosed)
                            {
                                session.OpenForPublish(new Session.OpenRequest(this).SetPermissions(new List<string>() {"publish_actions"}));
                            }
                            else
                            {
                                session.CloseAndClearTokenInformation();
                                session = null; 
                                session = new Session(this);
                                Session.ActiveSession = session;

                                session.OpenForPublish(new Session.OpenRequest(this).SetPermissions(new List<string>() { "publish_actions" }));
                                //session.RequestNewPublishPermissions(new Session.NewPermissionsRequest(this, new List<string>() { "publish_actions" }));
                            }
                            //session.OpenForPublish(new Session.OpenRequest(this).SetPermissions(new List<string>() { "publish_actions" }));
                        else
                            session.OpenForRead(new Session.OpenRequest(this).SetPermissions(new List<string>() { "email" }));
                    }
                    else
                    {
                        new Request(session, "/me", null, HttpMethod.Get, this).ExecuteAsync();
                    }
                    break;
            }
        }*/

        /*private void ClearData()
        {
            AccountName = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            LastName = string.Empty;
        }*/

        /*private void SocialMediaConnected(Utils.SocialMediaUtils.SocialMediaConnection connection)
        {
            Utils.SocialMediaUtils.CurrentSocialMediaConnection = connection;

            var firstName = string.Empty;
            var middleName = string.Empty;
            var lastName = string.Empty;

            var accountName = AccountName;

            if (!string.IsNullOrEmpty(FirstName))
                firstName = FirstName;

            if (!string.IsNullOrEmpty(MiddleName))
                firstName = MiddleName;

            if (!string.IsNullOrEmpty(LastName))
                firstName = LastName;

            SocialMediaConnected(accountName, firstName, middleName, lastName);
        }*/

        /*public void Share(Utils.SocialMediaUtils.SocialMediaConnection connection, string title, string subtitle, string details, string imageLink)
        {
            pendingAction = null;
            switch (connection)
            {
                case Utils.SocialMediaUtils.SocialMediaConnection.Google:
                    if (!plusClient.IsConnected)
                    {
                        pendingAction = new PendingAction() { Title = title, SubTitle = subtitle, Details = details, ImageLink = imageLink };
                        Connect(connection);
                    }
                    else
                    {
                        var builder = new PlusShare.Builder(this, plusClient);

                        builder.SetText("Lemon Cheesecake recipe")
                               .SetType("text/plain")
                               .SetContentDeepLinkId("/cheesecake/lemon", 
                                                     "Lemon Cheesecake recipe", 
                                                     "A tasty recipe for making lemon cheesecake.",
                                                     Android.Net.Uri.Parse(
                                                         "https://lh4.googleusercontent.com/-bm7Hl_MjWLo/UqXbHvoLUVI/AAAAAAAAAA0/8Tw4cBGtJ5w/w500-h450-no/2013+-+1"));

                        StartActivityForResult(builder.Intent, 0);
                    }
                    break;
                case Utils.SocialMediaUtils.SocialMediaConnection.Facebook:
                    var session = Session.ActiveSession;

                    if (session == null || !session.IsOpened || !session.Permissions.Contains("publish_actions"))
                    {
                        pendingAction = new PendingAction() { Title = title, SubTitle = subtitle, Details = details, ImageLink = imageLink};
                        Connect(Utils.SocialMediaUtils.SocialMediaConnection.Facebook, true);
                    }
                    else
                    {
                        var bundle = new Bundle();
                        bundle.PutString("name", title);
                        bundle.PutString("caption", subtitle);
                        bundle.PutString("description", details);
                        //bundle.PutString("link", "http://www.lsretail.com/products/ls-omni");
                        bundle.PutString("link", imageLink);
                        bundle.PutString("picture", imageLink);

                        var dialog = new WebDialog.FeedDialogBuilder(this, session, bundle).Build();
                        dialog.OnCompleteListener = this;
                        dialog.Show();

                    }
                    break;
            }
        }*/

        /*protected virtual void SocialMediaConnected(string accountName, string firstName, string middleName, string lastName)
        {

        }*/

        #region PLUS CLIENT

        /*private ConnectionResult connectionResult;
        private PlusClient plusClient;

        protected const int ResolveGooglePlusLoginRequestCode = 9000;*/

        /*public void OnConnected(Bundle p0)
        {
            if (Utils.SocialMediaUtils.CurrentSocialMediaConnection != Utils.SocialMediaUtils.SocialMediaConnection.Google)
                ClearData();

            Utils.SocialMediaUtils.CurrentSocialMediaConnection = Utils.SocialMediaUtils.SocialMediaConnection.Google;

            AccountName = plusClient.AccountName;

            if (plusClient.CurrentPerson != null && plusClient.CurrentPerson.HasName)
            {
                if (plusClient.CurrentPerson.Name.HasGivenName)
                    FirstName = plusClient.CurrentPerson.Name.GivenName;

                if (plusClient.CurrentPerson.Name.HasMiddleName)
                    MiddleName = plusClient.CurrentPerson.Name.MiddleName;

                if (plusClient.CurrentPerson.Name.HasFamilyName)
                    LastName = plusClient.CurrentPerson.Name.FamilyName;

            }

            if (pendingAction != null)
            {
                Share(Utils.SocialMediaUtils.SocialMediaConnection.Google, pendingAction.Title, pendingAction.SubTitle, pendingAction.Details, pendingAction.ImageLink);
            }

            SocialMediaConnected(AccountName, FirstName, MiddleName, LastName);
        }*/

        /*public void OnDisconnected()
        {
        }*/

        /*public void OnConnectionFailed(ConnectionResult result)
        {
            if (result.HasResolution)
            {
                try
                {
                    result.StartResolutionForResult(this, ResolveGooglePlusLoginRequestCode);
                }
                catch (IntentSender.SendIntentException)
                {
                    plusClient.Connect();
                }
            }
            // Save the result and resolve the connection failure upon a user click.
            connectionResult = result;
        }*/

        #endregion

        #region FACEBOOK

        /*protected UiLifecycleHelper uiHelper;

        public void OnUserInfoFetched(IGraphUser user)
        {
            if (Utils.SocialMediaUtils.CurrentSocialMediaConnection != Utils.SocialMediaUtils.SocialMediaConnection.Facebook)
                ClearData();

            Utils.SocialMediaUtils.CurrentSocialMediaConnection = Utils.SocialMediaUtils.SocialMediaConnection.Facebook;

            Session session = Session.ActiveSession;

            if (user != null)
            {
                AccountName = user.GetProperty("email").ToString();
                FirstName = user.FirstName;
                MiddleName = user.MiddleName;
                LastName = user.LastName;

                SocialMediaConnected(AccountName, FirstName, MiddleName, LastName);
            }
        }*/

        /*public void Call(Session session, SessionState state, Java.Lang.Exception exception)
        {
            if (pendingAction != null &&         //TODO posting
                (exception is FacebookOperationCanceledException ||
                exception is FacebookAuthorizationException))
            {       //TODO cancelled action???
                
            }
            else if (state == SessionState.OpenedTokenUpdated || state == SessionState.Opened)
            {
                HandlePendingAction();
            }
        }*/

        //not used
        /*public void Call(Session session, SessionState state, Exception exception)
        {
            if (pendingAction != null &&         //TODO posting
                (exception is FacebookOperationCanceledException ||
                exception is FacebookAuthorizationException))
            {       //TODO cancelled action???
            }
            else if (state == SessionState.OpenedTokenUpdated || state == SessionState.Opened)
            {
                HandlePendingAction();
            }
        }*/

        /*public void OnCompleted(Response response)
        {
            AccountName = response.GraphObject.GetProperty("email").ToString();
            FirstName = (string)response.GraphObject.GetProperty("first_name");
            MiddleName = (string)response.GraphObject.GetProperty("middle_name");
            LastName = (string)response.GraphObject.GetProperty("last_name");

            SocialMediaConnected(Utils.SocialMediaUtils.SocialMediaConnection.Facebook);
        }*/

        /*private void HandlePendingAction()
        {
            PendingAction previouslyPendingAction = pendingAction;
            // These actions may re-set pendingAction if they are still pending, but we assume they
            // will succeed.
            pendingAction = null;

            if (previouslyPendingAction != null)
            {
                Share(Utils.SocialMediaUtils.SocialMediaConnection.Facebook, previouslyPendingAction.Title, previouslyPendingAction.SubTitle, previouslyPendingAction.Details, previouslyPendingAction.ImageLink);
            }
        }*/

        /*public void OnComplete(Bundle bundle, FacebookException exception)
        {
            var x = exception;
        }*/

        #endregion

        #region TWITTER

        #endregion

        #endregion
    }
}
