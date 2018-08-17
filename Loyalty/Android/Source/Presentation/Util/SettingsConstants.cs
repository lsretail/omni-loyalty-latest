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

namespace Presentation.Util
{
    class SettingsConstants
    {
        public const string SharedPreferencedName = "LSLoyaltySettings";
        public const string GetAllPreferences = "GetAllPreferences";
        public const string NumberOfPreferencesToGet = "NumberOfPreferencesToGet";
    }
}