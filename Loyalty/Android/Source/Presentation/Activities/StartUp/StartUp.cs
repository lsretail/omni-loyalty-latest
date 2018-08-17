using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Security;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Presentation.Activities.Home;
using Presentation.Activities.Login;
using Presentation.Activities.Welcome;
using Presentation.Util;

namespace Presentation.Activities.StartUp
{
    [Activity(MainLauncher = true, Theme = "@style/ApplicationTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new []{Intent.ActionView}, Categories = new []{Intent.CategoryDefault, Intent.CategoryBrowsable})]
    public class StartUp : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
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

            imageView.SetImageResource(Resource.Drawable.logo_drawer);

            view.AddView(imageView);

            SetContentView(view);
#if DEBUG
            Util.Utils.LogUtils.Log("Facebook key hash");

            try {
                var info = PackageManager.GetPackageInfo("lsretail.omni.loyalty.android", PackageInfoFlags.Signatures);
                foreach (var signature in info.Signatures) {
                    MessageDigest md = MessageDigest.GetInstance("SHA");
                    md.Update(signature.ToByteArray());
                    Util.Utils.LogUtils.Log("KeyHash:" + Base64.EncodeToString(md.Digest(), Base64Flags.Default));
                }
            } catch (PackageManager.NameNotFoundException) {

            } catch (NoSuchAlgorithmException) {

            }
#endif

            var currentUrl = Utils.PreferenceUtils.GetString(this, Utils.PreferenceUtils.WSUrl);

            if (string.IsNullOrEmpty(currentUrl))
            {
                LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(new DeviceUuidFactory(this).getDeviceUuid(), LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.Loyalty, LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.DefaultUrlLoyalty, languageCode:Resources.Configuration.Locale.Language);
            }
            else
            {
                LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(new DeviceUuidFactory(this).getDeviceUuid(), LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.Loyalty, currentUrl, languageCode:Resources.Configuration.Locale.Language);
            }

            AppData.IsDualScreen = Utils.IsTablet(this);

            var device = AppData.Device;

            if (device is UnknownDevice)
            {
                var intent = new Intent();
                intent.SetClass(this, typeof(WelcomeActivity));
                StartActivity(intent);

                Finish();
            }
            else
            {
                if (device.UserLoggedOnToDevice == null || string.IsNullOrEmpty(device.UserLoggedOnToDevice.Email))
                {
                    if (EnabledItems.ForceLogin)
                    {
                        var intent = new Intent();
                        intent.SetClass(this, typeof(LoginActivity));
                        StartActivity(intent);

                        Finish();
                    }
                    else
                    {
                        GoToMain();
                    }
                }
                else
                {
                    Login();
                }
            }
        }



        private void Login()
        {
            GoToMain();
        }

        private async void GoToMain()
        {
            await Task.Delay(2000);
            var intent = new Intent();

            //intent.SetClass(this, typeof(SlideControlActivity));
            intent.SetClass(this, typeof(HomeActivity));

            intent.PutExtra(BundleConstants.HasNewData, true);

            if (Intent.Extras != null && Intent.Extras.ContainsKey(BundleConstants.LoadNotificationsFromService))
            {
                intent.PutExtra(BundleConstants.LoadNotificationsFromService, Intent.Extras.GetBoolean(BundleConstants.LoadNotificationsFromService));
            }

            StartActivity(intent);

            Finish();
        }
    }
}