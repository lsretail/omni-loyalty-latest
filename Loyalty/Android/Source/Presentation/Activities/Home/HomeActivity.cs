using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

using Presentation.Activities.AccountTier;
using Presentation.Activities.Base;
using Presentation.Activities.Coupons;
using Presentation.Activities.History;
using Presentation.Activities.Items;
using Presentation.Activities.Login;
using Presentation.Activities.Notifications;
using Presentation.Activities.Offers;
using Presentation.Activities.Search;
using Presentation.Activities.ShoppingLists;
using Presentation.Activities.StoreLocator;
using Presentation.Models;
using Presentation.Service;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Activities.Home
{
    [Activity()]
    public class HomeActivity : LoyaltyFragmentActivity
    {
        private MemberContactModel memberContactModel;

        protected override void OnCreate(Bundle bundle)
        {
            memberContactModel = new MemberContactModel(this);

            base.OnCreate(bundle);

            if (bundle == null)
            {
                var selectedItem = ActivityTypes.DefaultItem;

                if (Intent.Extras != null)
                {
                    if (Intent.Extras.ContainsKey(BundleConstants.LoadNotificationsFromService))
                    {
                        var openNotifications = Intent.Extras.GetBoolean(BundleConstants.LoadNotificationsFromService);

                        if (openNotifications)
                        {
                            selectedItem = ActivityTypes.Notifications;
                        }
                    }

                    if ((EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null) && Intent.Extras.ContainsKey(BundleConstants.HasNewData))
                    {
                        var refresh = Intent.Extras.GetBoolean(BundleConstants.HasNewData);

                        if (refresh)
                        {
                            RefreshMemberContact();
                        }
                    }
                }
                
                SelectItem(selectedItem);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                if (string.IsNullOrEmpty(Utils.PreferenceUtils.GetString(this, Utils.PreferenceUtils.FcmRegistrationId)))
                {
                    Intent intent = new Intent(this, typeof(RegistrationIntentService));
                    StartService(intent);
                }
            }
            catch (Exception e)
            {
                Utils.LogUtils.Log("Push notifications not supported");
                Utils.LogUtils.Log(e.Message);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override void BroadcastReceived(string action)
        {
            base.BroadcastReceived(action);

            if (action == Utils.BroadcastUtils.DrawerOpened || action == Utils.BroadcastUtils.DrawerClosed)
            {
                /*SupportActionBar.Title = title;
                SupportInvalidateOptionsMenu();*/
            }
        }

        public override void SetSupportActionBar(Android.Support.V7.Widget.Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            toolbar.Title = string.Empty;

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_24dp);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Pass the event to ActionBarDrawerToggle, if it returns
            // true, then it has handled the app icon touch event
            if (item.ItemId == Android.Resource.Id.Home)
            {
                if (DrawerLayout.IsDrawerOpen(GravityCompat.Start))
                {
                    DrawerLayout.CloseDrawer(GravityCompat.Start);
                }
                else
                {
                    DrawerLayout.OpenDrawer(GravityCompat.Start);
                }
            }
            // Handle your other action bar items...

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent.Extras != null && intent.Extras.ContainsKey(BundleConstants.ChosenMenuBundleName))
            {
                var chosenItem = intent.Extras.GetInt(BundleConstants.ChosenMenuBundleName);

                SelectItem(chosenItem, intent.Extras);
            }
        }

        private async void RefreshMemberContact()
        {
            await memberContactModel.UserGetById(AppData.Device.UserLoggedOnToDevice.Id, null);
        }

        public override void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            SelectItem(DrawerMenuItems[position - DrawerLeft.HeaderViewsCount].ActivityType);
        }

        public override void SelectItem(int activityType)
        {
            SelectItem(activityType, null);
        }

        public void SelectItem(int type, Bundle extras)
        {
            if (type == ActivityTypes.DefaultItem)
            {
                SelectItem(DefaultActivityType, extras);
                return;
            }

            LoyaltyFragment fragment = null;

            //SupportActionBar.SetDisplayShowTitleEnabled(true);
            //SupportActionBar.NavigationMode = (int)ActionBarNavigationMode.Standard;

            switch (type)
            {
                case ActivityTypes.Login:
                    ActivityType = ActivityTypes.Login;
                    fragment = new LoginFragment();
                    break;

                case ActivityTypes.Home:
                    ActivityType = ActivityTypes.Home;
                    fragment = new HomeFragment();
                    break;

                case ActivityTypes.Account:
                    ActivityType = ActivityTypes.Account;
                    fragment = new AccountTierFragment();
                    break;

                case ActivityTypes.Search:
                    ActivityType = ActivityTypes.Search;
                    fragment = new GeneralSearchFragment();
                    break;

                case ActivityTypes.Notifications:
                    ActivityType = ActivityTypes.Notifications;
                    fragment = NotificationFragment.NewInstance();
                    break;

                case ActivityTypes.Coupons:
                    if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
                    {
                        var bundle = new Bundle();
                        bundle.PutString(BundleConstants.ErrorMessage, GetString(Resource.String.ApplicationMustBeLoggedIn));

                        SelectItem(ActivityTypes.Login, bundle);
                        return;
                    }

                    ActivityType = ActivityTypes.Coupons;
                    fragment = new CouponFragment();
                    break;

                case ActivityTypes.Offer:
                    if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
                    {
                        var bundle = new Bundle();
                        bundle.PutString(BundleConstants.ErrorMessage, GetString(Resource.String.ApplicationMustBeLoggedIn));

                        SelectItem(ActivityTypes.Login, bundle);
                        return;
                    }

                    ActivityType = ActivityTypes.Offer;
                    fragment = new OfferFragment();
                    break;

                case ActivityTypes.Items:
                    ActivityType = ActivityTypes.Items;
                    fragment = new ItemCategoriesFragment();
                    break;

                case ActivityTypes.Locations:
                    ActivityType = ActivityTypes.Locations;
                    fragment = new StoreLocatorFragment();
                    break;

                case ActivityTypes.Transactions:
                    ActivityType = ActivityTypes.Transactions;
                    fragment = new TransactionFragment();
                    break;

                case ActivityTypes.ShoppingLists:
                    if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
                    {
                        SelectItem(ActivityTypes.Login);
                        return;
                    }

                    ActivityType = ActivityTypes.ShoppingLists;
                    fragment = WishListFragment.NewInstance();
                    break;

                case ActivityTypes.Logout:
                    new MemberContactModel(this).Logout();

                    if (EnabledItems.ForceLogin)
                    {
                        var intent = new Intent();
                        intent.SetClass(this, typeof (LoginActivity));
                        StartActivity(intent);

                        Finish();
                    }
                    else
                    {
                        CheckDrawerStatus();
                        SelectItem(ActivityTypes.DefaultItem);
                    }
                    return;

                default:
                    ActivityType = ActivityTypes.Locations;
                    fragment = new StoreLocatorFragment();
                    break;
            }

            if (extras != null)
            {
                if (extras.ContainsKey(BundleConstants.ChosenMenuBundleName))
                {
                    extras.Remove(BundleConstants.ChosenMenuBundleName);
                }

                fragment.Arguments = extras;
            }

            // Insert the fragment by replacing any existing fragment
            var fragmentManager = SupportFragmentManager;
            var ft = fragmentManager.BeginTransaction();


            ft.SetTransition((int)FragmentTransit.FragmentOpen);
            ft.Replace(ContentId, fragment).Commit();

            FillDrawerList();

            var position = DrawerMenuItems.IndexOf(DrawerMenuItems.FirstOrDefault(x => x.ActivityType == type));

            // Highlight the selected item, update the title, and close the drawer
            DrawerLeft.SetItemChecked(position, true);
            
            DrawerLayout.CloseDrawer(DrawerLeft);

            SupportInvalidateOptionsMenu();
        }

        public override void OnBackPressed()
        {
            if (!IsDefaultItem(ActivityType))
            {
                SelectItem(ActivityTypes.DefaultItem);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override void OnDrawerClosed(View drawerView)
        {
            base.OnDrawerClosed(drawerView);
            //DrawerToggle.OnDrawerClosed(drawerView);
        }

        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
            //DrawerToggle.OnDrawerOpened(drawerView);
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            base.OnDrawerSlide(drawerView, slideOffset);
            //DrawerToggle.OnDrawerSlide(drawerView, slideOffset);
        }

        public override void OnDrawerStateChanged(int newState)
        {
            base.OnDrawerStateChanged(newState);
            //DrawerToggle.OnDrawerStateChanged(newState);
        }
    }
}