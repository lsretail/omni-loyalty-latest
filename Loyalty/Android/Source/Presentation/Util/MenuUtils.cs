using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Account;
using Presentation.Activities.Base;
using Presentation.Activities.Contact;
using Presentation.Activities.Settings;

namespace Presentation.Util
{
    public class MenuUtils
    {
        public static void SettingsClicked(Context context)
        {
            var intent = new Intent();
            intent.SetClass(context, typeof(SettingsActivity));
            context.StartActivity(intent);
        }

        public static void AccountManagementClicked(Context context)
        {
            var intent = new Intent();
            intent.SetClass(context, typeof(AccountActivity));
            intent.PutExtra(BundleConstants.EditAccount, true);
            context.StartActivity(intent);
        }

        public static void HelpClicked(Context context)
        {
            var intent = new Intent();
            intent.SetClass(context, typeof(SettingsActivity));
            context.StartActivity(intent);
        }

        public static void ContactUsClicked(Context context)
        {
            var intent = new Intent();
            intent.SetClass(context, typeof(ContactUsActivity));
            context.StartActivity(intent);
        }
    }
}