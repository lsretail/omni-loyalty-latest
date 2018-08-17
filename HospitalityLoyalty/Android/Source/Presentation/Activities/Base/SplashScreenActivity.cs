using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Domain.Transactions;
using Infrastructure.Data.SQLite2.Baskets;
using Infrastructure.Data.SQLite2.Favorites;
using Infrastructure.Data.SQLite2.MemberContacts;
using Infrastructure.Data.SQLite2.Menus;
using Infrastructure.Data.SQLite2.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Favorites;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Transactions;
using Presentation.Activities.Home;
using Presentation.Activities.Welcome;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Activities.Base
{
    [Activity(MainLauncher = true, Theme = "@style/BaseThemeNoActionBar")]
    public class SplashScreenActivity : HospActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = false;
            LeftDrawer = false;

            base.OnCreate(bundle);

            var view = new FrameLayout(this);
            view.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            view.SetBackgroundResource(Resource.Color.accent);

            var imageView = new ImageView(this);

            var margin = Resources.GetDimensionPixelSize(Resource.Dimension.BasePadding);

            var layoutParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.Gravity = GravityFlags.Center;
            layoutParams.SetMargins(margin, margin, margin, margin * 8);
            imageView.LayoutParameters = layoutParams;

            imageView.SetImageResource(Resource.Drawable.splash_logo);

            view.AddView(imageView);

            SetContentView(view);

            //CheckPermissions();

            //SetContentView(Resource.Layout.SplashScreen);
        }

        protected override void OnResume()
        {
            base.OnResume();

            GoToLogin();
        }

        private async void GoToLogin()
        {
            await Task.Delay(1000);

            var currentUrl = string.Empty;

#if CHANGEWS
            currentUrl = Utils.PreferenceUtils.GetString(this, Utils.PreferenceUtils.WSUrl);
#endif

            if (string.IsNullOrEmpty(currentUrl))
            {
                LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(Utils.Utils.GetPhoneUUID(this), LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.HospitalityLoyalty, LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.DefaultUrlLoyalty);
            }
            else
            {
                LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(Utils.Utils.GetPhoneUUID(this), LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.HospitalityLoyalty, currentUrl);
            }

            await Task.Run(() => LoadFromDatabase());

            if (AppData.Contact != null)
            {
                new ContactModel(this).ContactGetById(AppData.Contact.Id);
            }
            if (AppData.MobileMenu != null)
            {
                new MenuModel(this).GetMenus();
            }

            var intent = new Intent();

            if (PreferenceUtils.GetBool(this, PreferenceUtils.WelcomeHasBeenShown))
            {
                intent.SetClass(this, typeof(HomeActivity));
            }
            else
            {
                intent.SetClass(this, typeof(WelcomeActivity));
            }

            StartActivity(intent);

            Finish();
        }

        private void LoadFromDatabase()
        {
            Infrastructure.Data.SQLite2.DB.CreateTables();

            try
            {
                AppData.Favorites = new LocalFavoriteService(new FavoriteRepository()).GetFavorites();
                AppData.Transactions = new LocalTransactionService(new TransactionRepository()).GetTransactions();
                AppData.MobileMenu = new LocalMenuService(new MenuRepository()).GetMobileMenu();
                AppData.Contact = new MemberContactService().GetContact(new ContactRepository());
                AppData.Basket = new LocalBasketService().GetBasket(new BasketRepository(), AppData.MobileMenu);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                //ignore, then the values will just be null
            }
        }
    }
}