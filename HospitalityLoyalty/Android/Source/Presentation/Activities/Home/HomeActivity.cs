using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Contact;
using Presentation.Activities.Favorite;
using Presentation.Activities.Login;
using Presentation.Activities.Menu;
using Presentation.Activities.Offer;
using Presentation.Activities.Store;
using Presentation.Activities.Transaction;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;

using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Home
{
    [Activity(Label = "", Theme = "@style/BaseThemeNoActionBar")]
    public class HomeActivity : HospActivity
    {
        private string title;
        private ContactModel contactModel;
        //private MyActionBarDrawerToggle DrawerToggle;

        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;
            contactModel = new ContactModel(this);

            base.OnCreate(bundle);

            if (bundle == null)
            {
                SelectItem(ActivityTypes.DefaultItem);
            }
            else
            {
                title = bundle.GetString(BundleUtils.Title);
                SetTitle(title);
            }

            //DrawerToggle.SyncState();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(BundleUtils.Title, title);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Pass the event to ActionBarDrawerToggle, if it returns
            // true, then it has handled the app icon touch event
            if (item.ItemId == Android.Resource.Id.Home)
            {
                if (DrawerLayout.IsDrawerOpen(LeftDrawerLayout))
                {
                    DrawerLayout.CloseDrawer(LeftDrawerLayout);
                }
                else
                {
                    if (RightDrawer && DrawerLayout.IsDrawerOpen(RightDrawerLayout))
                    {
                        DrawerLayout.CloseDrawer(RightDrawerLayout);
                    }
                    else
                    {
                        DrawerLayout.OpenDrawer(LeftDrawerLayout);
                    }
                }
            }
            // Handle your other action bar items...

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent.Extras != null && intent.Extras.ContainsKey(BundleUtils.ChosenMenuBundleName))
            {
                var chosenItem = (BundleUtils.ChosenMenu) intent.Extras.GetInt(BundleUtils.ChosenMenuBundleName);

                switch (chosenItem)
                {
                    case BundleUtils.ChosenMenu.Login:
                        SelectItem(ActivityTypes.Login, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.Home:
                        SelectItem(ActivityTypes.Home, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.Menu:
                        SelectItem(ActivityTypes.Menu, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.OffersAndCoupons:
                        SelectItem(ActivityTypes.OfferAndCoupons, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.Locations:
                        SelectItem(ActivityTypes.Locations, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.Transactions:
                        SelectItem(ActivityTypes.Transactions, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.Logout:
                        SelectItem(ActivityTypes.Logout, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.DefaultMenu:
                        SelectItem(ActivityTypes.DefaultItem, intent.Extras);
                        break;

                    case BundleUtils.ChosenMenu.Favorites:
                        SelectItem(ActivityTypes.Favorite, intent.Extras);
                        break;
                }
            }
        }

        public override void OnDrawerItemSelected(DrawerMenuItem item)
        {
            SelectItem(item.ActivityType);
        }

        public void SelectItem(ActivityTypes type, Bundle extras = null)
        {
            BaseFragment fragment = null;
            string tag = string.Empty;

            //SupportActionBar.SetDisplayShowTitleEnabled(true);
            //SupportActionBar.NavigationMode = (int)ActionBarNavigationMode.Standard;

            var position = drawerMenuItems.IndexOf(drawerMenuItems.FirstOrDefault(x => x.ActivityType == type));

            switch (type)
            {
                case ActivityTypes.Login:
                    ActivityType = ActivityTypes.Login;
                    if (AppData.Contact == null)
                    {
                        fragment = new LoginFragment();
                    }
                    else
                    {
                        fragment = new ContactFragment();
                    }
                    break;

                case ActivityTypes.Home:
                    ActivityType = ActivityTypes.Home;
                    fragment = new HomeFragment();
                    break;

                case ActivityTypes.Menu:
                    ActivityType = ActivityTypes.Menu;
                    fragment = new MenuNodeFragment();
                    break;

                case ActivityTypes.OfferAndCoupons:
                    ActivityType = ActivityTypes.OfferAndCoupons;
                    fragment = new OfferAndCouponFragment();
                    break;

                case ActivityTypes.Locations:
                    ActivityType = ActivityTypes.Locations;
                    fragment = new StoreFragment();
                    break;

                case ActivityTypes.Transactions:
                    ActivityType = ActivityTypes.Transactions;
                    fragment = new TransactionFragment();
                    break;

                case ActivityTypes.Favorite:
                    ActivityType = ActivityTypes.Favorite;
                    fragment = new FavoriteFragment();
                    break;

                case ActivityTypes.Logout:
                    Logout();
                    return;

                case ActivityTypes.DefaultItem:
                    if (AppData.HasHome)
                    {
                        SelectItem(ActivityTypes.Home);
                    }
                    else
                    {
                        SelectItem(ActivityTypes.Menu);
                    }
                    return;
            }

            if (extras != null)
            {
                if (extras.ContainsKey(BundleUtils.ChosenMenuBundleName))
                {
                    extras.Remove(BundleUtils.ChosenMenuBundleName);
                }

                fragment.Arguments = extras;
            }

            // Insert the fragment by replacing any existing fragment
            var fragmentManager = SupportFragmentManager;
            var ft = fragmentManager.BeginTransaction();
            ft.SetTransition((int) FragmentTransit.FragmentOpen);
            ft.Replace(Resource.Id.BaseActivityScreenContentFrame, fragment).Commit();

            // Highlight the selected item, update the title, and close the drawer
            //DrawerList.SetItemChecked(position, true);

            SetTitle(drawerMenuItems[position].Title);
            DrawerLayout.CloseDrawer(LeftDrawerLayout);

            FillDrawerList();
        }

        private async void Logout()
        {
            await contactModel.Logout();

            SelectItem(ActivityTypes.DefaultItem);
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

        private void SetTitle()
        {
            //var position = drawerMenuItems.IndexOf(drawerMenuItems.FirstOrDefault(x => x.ActivityType == ActivityType));
            //asd
        }

        public void SetTitle(string title)
        {
            this.title = title;
            //SupportActionBar.Title = title;
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_24dp);
            SupportActionBar.Title = title;
        }
    }
}