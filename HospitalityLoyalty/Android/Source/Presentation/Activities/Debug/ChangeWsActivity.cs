using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Infrastructure.Data.SQLite2.DTO;
using Infrastructure.Data.SQLite2.Webservice;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Utils;
using Presentation.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Debug
{
    [Activity(Label = "Change URL", Theme = "@style/BaseThemeNoActionBar")]
    public class ChangeWsActivity : HospActivityNoStatusBar, View.IOnClickListener
    {
        private EditText url;
        private WebserviceRepository repo;

        private ProgressButton pingButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ChangeWsScreen);

            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.ChangeWsScreenToolbar));

            url = FindViewById<EditText>(Resource.Id.ChangeWsUrl);

            FindViewById<ColoredButton>(Resource.Id.ChangeWsSave).SetOnClickListener(this);
            pingButton = FindViewById<ProgressButton>(Resource.Id.ChangeWsPing);
            pingButton.SetOnClickListener(this);

            repo = new WebserviceRepository();
            var currentUrl = Utils.PreferenceUtils.GetString(this, PreferenceUtils.WSUrl);

            if (string.IsNullOrEmpty(currentUrl))
            {
                url.Text = LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.DefaultUrlLoyalty;
            }
            else
            {
                url.Text = currentUrl;
            }

            var pInfo = PackageManager.GetPackageInfo(PackageName, 0);

            var version = FindViewById<TextView>(Resource.Id.ChangeWSVersion);
            version.Text = string.Format("Build: {0}", pInfo.VersionName + "." + pInfo.VersionCode);
        }

        public async void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ChangeWsSave:
                    Utils.PreferenceUtils.SetString(this, PreferenceUtils.WSUrl, url.Text);

                    AppData.Basket.Clear();
                    AppData.Stores = null;
                    AppData.MobileMenu = null;
                    AppData.Contact = null;
                    AppData.Favorites.Clear();
                    AppData.Transactions.Clear();

                    if(AppData.Advertisements != null)
                        AppData.Advertisements.Clear();

                    //todo clear db

                    LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(Utils.Utils.GetPhoneUUID(this), LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.HospitalityLoyalty , url.Text);

                    Toast.MakeText(this, "Web service changed", ToastLength.Short).Show();

                    break;

                case Resource.Id.ChangeWsPing:
                    string message = string.Empty;
                    pingButton.State = ProgressButton.ProgressButtonState.Loading;

                    try
                    {
                        message = await Task.Run(() => new LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils().PingServer());
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                    }
                    finally
                    {
                        pingButton.State = ProgressButton.ProgressButtonState.Normal;
                    }

                    var dialogBuilder = new AlertDialog.Builder(this);

                    dialogBuilder.SetMessage(message).SetPositiveButton("ok", (sender, args) => { }).Create().Show();


                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            var upIntent = new Intent();
            upIntent.SetClass(this, typeof(HomeActivity));
            upIntent.AddFlags(ActivityFlags.ClearTop);
            upIntent.AddFlags(ActivityFlags.SingleTop);

            upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.DefaultMenu);

            StartActivity(upIntent);

            Finish();
        }
    }
}