using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Infrastructure.Data.SQLite.DB.DTO;
using Infrastructure.Data.SQLite.Webservice;
using Presentation.Activities.Base;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;

namespace Presentation.Activities.Settings
{
    public class SettingsFragment : LoyaltyFragment, View.IOnClickListener
    {
        private EditText url;

        public static SettingsFragment NewInstance()
        {
            var settingsDetail = new SettingsFragment() { Arguments = new Bundle() };
            return settingsDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            View view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.SettingsWS);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.SettingsScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            url = view.FindViewById<EditText>(Resource.Id.SettingsUrl);

            var currentUrl = Utils.PreferenceUtils.GetString(Activity, Utils.PreferenceUtils.WSUrl);

            if (string.IsNullOrEmpty(currentUrl))
            {
                url.Text = LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.DefaultUrlLoyalty;
            }
            else
            {
                url.Text = currentUrl;
            }

            view.FindViewById<ColoredButton>(Resource.Id.SettingsSave).SetOnClickListener(this);
            view.FindViewById<ColoredButton>(Resource.Id.SettingsPing).SetOnClickListener(this);

            var pInfo = Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0);

            var version = view.FindViewById<TextView>(Resource.Id.SettingsVersion);
            version.Text = string.Format("Build: {0}", pInfo.VersionName + "." + pInfo.VersionCode);

            return view;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.SettingsSave:
                    Utils.PreferenceUtils.SetString(Activity, Utils.PreferenceUtils.WSUrl, url.Text);
                    LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(new DeviceUuidFactory(Activity).getDeviceUuid(), LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.Loyalty, url.Text, languageCode:Resources.Configuration.Locale.Language);
                    Utils.PreferenceUtils.SetString(Activity, Utils.PreferenceUtils.FcmRegistrationId, string.Empty);
                    Toast.MakeText(Activity, "Web service changed", ToastLength.Short).Show();
                    break;

                case Resource.Id.SettingsPing:
                    Ping();
                    break;
            }
        }

        private async void Ping()
        {
            var progressDialog = new CustomProgressDialog(Context);
            progressDialog.Title = "Ping";
            progressDialog.Message = "Pinging";
            progressDialog.Show();
            var result = string.Empty;

            try
            {
                result = await Task.Run(() => new LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils().PingServer());
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }

            progressDialog.Dismiss();

            var warningDialog = new WarningDialog(Context, "Ping");
            warningDialog.Message = result;
            warningDialog.Show();
        }
    }
}