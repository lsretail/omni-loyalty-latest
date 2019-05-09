using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Iid;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Presentation.Models;
using Presentation.Util;

namespace Presentation.Service
{
    [Service(Exported = false)]
    [IntentFilter(new string[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyInstanceIDListenerService : FirebaseInstanceIdService
    {
        private const string Tag = "MyInstanceIDLS";

        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Utils.PreferenceUtils.SetString(this, Utils.PreferenceUtils.FcmRegistrationId, refreshedToken);
            Utils.LogUtils.Log("Refreshed token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }

        void SendRegistrationToServer(string token)
        {
            // Add custom implementation, as needed.
            if (AppData.Device.UserLoggedOnToDevice != null)
            {
                var model = new MemberContactModel(this);
                model.PushNotificationSave(token);
            }
        }
    }
}