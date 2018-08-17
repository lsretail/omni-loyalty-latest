using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Debug;
using Presentation.Activities.Home;
using Presentation.Activities.Menu;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Base
{
    public abstract class HospActivity : AppCompatActivity, DrawerLayout.IDrawerListener, SwipeRefreshLayout.IOnRefreshListener, IRefreshableActivity
    {
        public const int OpenMenuItemRequestCode = 1001;

        public enum ActivityTypes
        {
            None = 0,
            Login = 1,
            Menu = 2,
            OfferAndCoupons = 3,
            Locations = 4,
            Home = 5,
            Transactions = 6,
            Logout = 7,
            Favorite = 8,
            DefaultItem = 9,
        }

        private const string BasketFragmentLabel = "BasketFragmentLabel";
        public const int LeftDrawerId = Resource.Id.BaseActivityScreenLeftDrawer;
        public const int RightDrawerId = Resource.Id.BaseActivityScreenRightDrawer;
        public const int OpenEndDrawerRequestCode = 1;

        private BroadcastReceiver receiver;
        private List<IBroadcastObserver> observers;

        private ContactModel contactModel;

        private bool rightDrawer;
        private bool leftDrawer = true;

        private TextView actionBarMessage;

        protected List<DrawerMenuItem> drawerMenuItems { get; private set; }

        protected DrawerLayout DrawerLayout { get; private set; }
        protected RecyclerView DrawerList { get; private set; }
        protected SwipeRefreshLayout LeftDrawerRefreshLayout { get; set; }
        protected View LeftDrawerLayout { get; set; }
        protected View RightDrawerLayout { get; set; }
        protected ImageView LeftDrawerLogo { get; set; }

        protected ActivityTypes ActivityType { get; set; }

        public bool LeftDrawer
        {
            get { return leftDrawer; }
            set { leftDrawer = value; }
        }

        public bool RightDrawer
        {
            get { return (rightDrawer && AppData.HasBasket); }
            protected set { rightDrawer = value; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            SetCustomTheme();
            base.OnCreate(bundle);

            CreateView();
            PrepareView();

            observers = new List<IBroadcastObserver>();

            drawerMenuItems = new List<DrawerMenuItem>();

            var actionBarHeight = Resources.GetDimensionPixelSize(Resource.Dimension.ActionBarHeight);
            var maxDrawerWidth = Resources.GetDimensionPixelSize(Resource.Dimension.MaxDrawerWidth);

            var drawerWidth = Math.Min(Resources.DisplayMetrics.WidthPixels - actionBarHeight, maxDrawerWidth);

            LeftDrawerLayout.LayoutParameters.Width = drawerWidth;

            LeftDrawerLogo.LayoutParameters.Height = drawerWidth / 16 * 9;

            DrawerList.SetLayoutManager(new LinearLayoutManager(this));
            DrawerList.HasFixedSize = true;
            contactModel = new ContactModel(this, this);

            LeftDrawerRefreshLayout.SetColorSchemeResources(Resource.Color.accent);
            LeftDrawerRefreshLayout.SetOnRefreshListener(this);

            SetLeftDrawerRefreshEnabled();

            // Set the adapter for the list view
            DrawerList.SetAdapter(new DrawerMenuItemAdapter(drawerMenuItems, OnDrawerItemSelected));

            DrawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow_left, GravityCompat.Start);
            DrawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow_right, GravityCompat.End);

            receiver = new Receiver(AlertObservers);

            if (RightDrawer)
            {
                var basketFragment = new BasketFragment();

                var ft = SupportFragmentManager.BeginTransaction();
                ft.Replace(RightDrawerId, basketFragment, BasketFragmentLabel);
                ft.Commit();

                EnableDrawer((int)GravityFlags.End);

                RightDrawerLayout.LayoutParameters.Width = drawerWidth;
            }
            else
            {
                DisableDrawer((int)GravityFlags.End);
            }

            if (bundle != null)
            {
                ActivityType = (ActivityTypes)bundle.GetInt(BundleUtils.ActivityType);
            }

            if (LeftDrawer)
            {
                FillDrawerList();
            }

            DrawerLayout.SetDrawerListener(this);
        }



        protected virtual void SetCustomTheme()
        {
            
        }

        protected virtual void CreateView()
        {
            SetContentView(Resource.Layout.BaseActivityDualScreen);
        }

        protected virtual void PrepareView()
        {
            LeftDrawerLayout = FindViewById(LeftDrawerId);
            RightDrawerLayout = FindViewById(RightDrawerId);

            LeftDrawerRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.BaseActivityScreenLeftDrawerRefreshContainer);
            LeftDrawerLogo = FindViewById<ImageView>(Resource.Id.BaseActivityScreenLeftDrawerLogo);

            DrawerLayout = FindViewById<DrawerLayout>(Resource.Id.BaseActivityScreenDrawerLayout);
            DrawerList = FindViewById<RecyclerView>(Resource.Id.BaseActivityScreenLeftDrawerList);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            if (LeftDrawer && !PreferenceUtils.GetBool(this, PreferenceUtils.NavigationDrawerHasBeenShown))
            {
                OpenDrawer((int)GravityFlags.Start);
                PreferenceUtils.SetBool(this, PreferenceUtils.NavigationDrawerHasBeenShown, true);
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(BundleUtils.ActivityType, (int)ActivityType);

            base.OnSaveInstanceState(outState);
        }

        private void SetLeftDrawerRefreshEnabled()
        {
            if (AppData.Contact == null || !AppData.HasPoints)
            {
                LeftDrawerRefreshLayout.Enabled = false;
            }
            else
            {
                LeftDrawerRefreshLayout.Enabled = true;
            }
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
            if (action == BroadcastUtils.ContactUpdated || action == BroadcastUtils.ContactPointsUpdated)
            {
                if (LeftDrawer)
                {
                    SetLeftDrawerRefreshEnabled();
                
                    FillDrawerList();

                    if (ActivityType == ActivityTypes.Login)
                    {
                        SupportActionBar.Title = AppData.Contact.FirstName;
                    }
                }
            }

            observers.ForEach(x => x.BroadcastReceived(action));
        }

        public void FillDrawerList()
        {
            drawerMenuItems.Clear();

            if (AppData.HasLogin)
            {
                if (AppData.Contact == null)
                {
                    drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarLogin), Image = Resource.Drawable.ic_person_white_24dp, ActivityType = ActivityTypes.Login, Color = ActivityType == ActivityTypes.Login });
                }
                else
                {
                    var subtitle = string.Empty;
                    var isLoading = false;

                    if (AppData.HasPoints)
                    {
                        subtitle = string.Format(Resources.GetString(Resource.String.ContactPoints), AppData.Contact.Account.PointBalance.ToString("n0"));
                    }

                    if (AppData.ContactUpdateStatus == AppData.Status.Loading)
                    {
                        isLoading = true;
                    }

                    drawerMenuItems.Add(new DrawerMenuItem() { Title = AppData.Contact.FirstName, SubTitle = subtitle, Image = Resource.Drawable.ic_person_white_24dp, IsLoading = isLoading, ActivityType = ActivityTypes.Login, Color = ActivityType == ActivityTypes.Login, Background = new Color(ContextCompat.GetColor(this, Resource.Color.backgroundcolor)) });
                }
            }

            if (AppData.HasHome)
            {
                drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarHome), Image = Resource.Drawable.ic_home_white_24dp, ActivityType = ActivityTypes.Home, Color = ActivityType == ActivityTypes.Home });
            }

            drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarMenu), Image = Resource.Drawable.ic_view_quilt_white_24dp, ActivityType = ActivityTypes.Menu, Color = ActivityType == ActivityTypes.Menu });

            if (AppData.HasOffersAndCoupons && AppData.Contact != null)
            {
                drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarOffersAndCoupons), Image = Resource.Drawable.ic_grade_white_24dp, ActivityType = ActivityTypes.OfferAndCoupons, Color = ActivityType == ActivityTypes.OfferAndCoupons });
            }

            drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarFavorites), Image = Resource.Drawable.ic_favorite_white_24dp, ActivityType = ActivityTypes.Favorite, Color = ActivityType == ActivityTypes.Favorite });

            if (AppData.HasTransactionHistory)
            {
                drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarTransactionHistory), Image = Resource.Drawable.ic_history_white_24dp, ActivityType = ActivityTypes.Transactions, Color = ActivityType == ActivityTypes.Transactions });
            }

            drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarStores), Image = Resource.Drawable.ic_place_white_24dp, ActivityType = ActivityTypes.Locations, Color = ActivityType == ActivityTypes.Locations });

            if (AppData.Contact != null)
            {
                drawerMenuItems.Add(new DrawerMenuItem() { Title = Resources.GetString(Resource.String.ActionBarLogout), Image = Resource.Drawable.ic_power_settings_new_white_24dp, ActivityType = ActivityTypes.Logout, Color = ActivityType == ActivityTypes.Logout });
            }

            DrawerList.GetAdapter().NotifyDataSetChanged();
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
                if (RightDrawer)
                {
                    CloseDrawer((int)GravityFlags.End);
                }
            }
            else
            {
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

        public virtual void OnDrawerItemSelected(DrawerMenuItem item)
        {
            SelectItem(item.ActivityType);
        }

        public virtual void SelectItem(ActivityTypes type)
        {
            var upIntent = new Intent();
            upIntent.SetClass(this, typeof(HomeActivity));
            upIntent.AddFlags(ActivityFlags.ClearTop);
            upIntent.AddFlags(ActivityFlags.SingleTop);

            switch (type)
            {
                case ActivityTypes.Login:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Login);
                    break;

                case ActivityTypes.Home:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Home);
                    break;

                case ActivityTypes.Menu:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Menu);
                    break;

                case ActivityTypes.OfferAndCoupons:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.OffersAndCoupons);
                    break;

                case ActivityTypes.Locations:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Locations);
                    break;

                case ActivityTypes.Transactions:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Transactions);
                    break;

                case ActivityTypes.Logout:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Logout);
                    break;

                case ActivityTypes.Favorite:
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Favorites);
                    break;
            }
            StartActivity(upIntent);
            Finish();
        }

        protected bool IsDefaultItem(ActivityTypes type)
        {
            if (AppData.HasHome && type == ActivityTypes.Home)
            {
                return true;
            }
            else if (!AppData.HasHome && type == ActivityTypes.Menu)
            {
                return true;
            }
            return false;
        }

        protected override void OnResume()
        {
            base.OnResume();

            var filter = new IntentFilter();

            foreach (var broadcastAction in BroadcastUtils.BroadcastActions)
            {
                filter.AddAction(broadcastAction);
            }

            RegisterReceiver(receiver, filter);
        }

        protected override void OnPause()
        {
            UnregisterReceiver(receiver);
            RemoveLoadingMessage();

            //System.GC.Collect();

            base.OnPause();
        }

        public override void OnLowMemory()
        {
            System.GC.Collect();

            base.OnLowMemory();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Clear();

#if CHANGEWS
            MenuInflater.Inflate(Resource.Menu.DebugMenu, menu);
#endif

#if HockeyApp
            MenuInflater.Inflate(Resource.Menu.FeedbackMenu, menu);
#endif

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewChangeWS:
                    var intent = new Intent();
                    intent.SetClass(this, typeof(ChangeWsActivity));

                    StartActivity(intent);

                    break;

                case Resource.Id.MenuViewSendFeedback:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public virtual void ShowIndicator(bool show)
        {
            LeftDrawerRefreshLayout.Refreshing = show;

            if (show)
            {
                ShowLoadingMessage(Resources.GetString(Resource.String.ContactRefreshingPoints));
            }
            else
            {
                RemoveLoadingMessage();
            }
        }

        public void OnRefresh()
        {
            if (AppData.Contact != null)
            {
                contactModel.ContactGetPointBalance(AppData.Contact.Id);
            }
        }

        public void ShowLoadingMessage(string message)
        {
            RemoveLoadingMessage();

            if (actionBarMessage == null)
            {
                actionBarMessage = new TextView(this, null, Resource.Style.Subhead);
                actionBarMessage.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                actionBarMessage.Gravity = GravityFlags.Center;
                actionBarMessage.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.white)));

                actionBarMessage.SetBackgroundResource(Resource.Color.accentdark);
                actionBarMessage.Text = message;
            }

            WindowManager.AddView(actionBarMessage, ActionBarLayoutParams);

            var worker = new BackgroundWorker();

            worker.DoWork += (sender, args) =>
            {
                System.Threading.Thread.Sleep(2000);
            };

            worker.RunWorkerCompleted += (sender, args) =>
            {
                RemoveLoadingMessage();
            };

            worker.RunWorkerAsync();
        }

        public void RemoveLoadingMessage()
        {
            if (actionBarMessage != null && actionBarMessage.WindowToken != null)
            {
                WindowManager.RemoveViewImmediate(actionBarMessage);
                actionBarMessage = null;
            }
        }

        private WindowManagerLayoutParams ActionBarLayoutParams
        {
            get
            {
                var rect = new Rect();

                Window.DecorView.GetWindowVisibleDisplayFrame(rect);
                var statusBarHeight = rect.Top;

                var actionBarSize = ObtainStyledAttributes(new int[] { Resource.Attribute.actionBarSize });   //Android.Resource.Attribute.ActionBarSize
                var actionbarHeight = actionBarSize.GetDimensionPixelSize(0, 0);

                var layoutParams = new WindowManagerLayoutParams(ViewGroup.LayoutParams.MatchParent, actionbarHeight,
                                                                 WindowManagerTypes.ApplicationPanel,
                                                                 WindowManagerFlags.NotFocusable, Format.Translucent);

                layoutParams.Gravity = GravityFlags.Top;
                layoutParams.X = 0;
                layoutParams.Y = statusBarHeight;
                layoutParams.WindowAnimations = Android.Resource.Style.AnimationToast;

                return layoutParams;
            }
        }

        public virtual void OnDrawerClosed(View drawerView)
        {
            BroadcastUtils.SendBroadcast(this, BroadcastUtils.DrawerClosed);
        }

        public virtual void OnDrawerOpened(View drawerView)
        {
            BroadcastUtils.SendBroadcast(this, BroadcastUtils.DrawerOpened);
        }

        public virtual void OnDrawerSlide(View drawerView, float slideOffset)
        {
        }

        public virtual void OnDrawerStateChanged(int newState)
        {
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == OpenMenuItemRequestCode)
            {
                if (resultCode == Result.Ok)
                {
                    BaseModel.ShowToast(FindViewById(Resource.Id.BaseActivityScreenDrawerLayout), Resource.String.BasketItemAdded);
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}