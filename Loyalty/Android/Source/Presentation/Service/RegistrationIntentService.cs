using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Iid;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Iid;
using Presentation.Models;
using Presentation.Util;

namespace Presentation.Service
{
    [Service(Exported = false)]
    public class RegistrationIntentService : IntentService
    {
        private const string Tag = "RegIntentService";

        public RegistrationIntentService() : base(Tag)
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                var token = FirebaseInstanceId.Instance.Token;

                Utils.PreferenceUtils.SetString(this, Utils.PreferenceUtils.FcmRegistrationId, token);

                if (AppData.Device.UserLoggedOnToDevice != null)
                {
                    var model = new MemberContactModel(this);
                    model.PushNotificationSave(token);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}