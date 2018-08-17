using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Infrastructure.Data.SQLite.Devices;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Activities.Login;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.Welcome
{
    public class WelcomeFragment : LoyaltyFragment, View.IOnClickListener
    {
        private DeviceService service;

        public static WelcomeFragment NewInstance()
        {
            var welcomeDetail = new WelcomeFragment() {Arguments = new Bundle()};
            return welcomeDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }
            
            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Welcome);

            service = new DeviceService(new DeviceRepository());

            var loginButtom = view.FindViewById<ColoredButton>(Resource.Id.WelcomeViewLoginButton);

            loginButtom.SetOnClickListener(this);

            return view;
        }
        
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.WelcomeViewLoginButton:
                    SaveDevice();

                    StartLogin();
                    break;
            }
        }

        private void StartLogin()
        {
            if (EnabledItems.ForceLogin)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof (LoginActivity));
                StartActivity(intent);

                Activity.Finish();
            }
            else
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(HomeActivity));
                StartActivity(intent);

                Activity.Finish();
            }
        }

        private void SaveDevice()
        {
            var factory = new DeviceUuidFactory(Activity);

            Device device = new Device(factory.getDeviceUuid());
            Utils.FillDeviceInfo(device);

            service.SaveDevice(device);

            AppData.Device = device;
        }
    }
}