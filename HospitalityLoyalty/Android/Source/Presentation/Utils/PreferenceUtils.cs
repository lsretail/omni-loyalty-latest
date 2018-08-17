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

namespace Presentation.Utils
{
    public class PreferenceUtils
    {
        private const string myPrefs = "myPrefs";

        public const string NavigationDrawerHasBeenShown = "NavigationDrawerHasBeenShown";
        public const string WelcomeHasBeenShown = "WelcomeHasBeenShown";
        public const string ShowListAsList = "ShowListAsList";

        public const string PointPullDownHintDismissed = "PointPullDownHintDismissed";

        public const string WSUrl = "WSUrl";

        public static bool GetBool(Context context, string key)
        {
            var settings = context.GetSharedPreferences(myPrefs, 0);
            return settings.GetBoolean(key, false);
        }

        public static void SetBool(Context context, string key, bool value)
        {
            // We need an Editor object to make preference changes.
            // All objects are from android.context.Context
            var settings = context.GetSharedPreferences(myPrefs, 0);
            var editor = settings.Edit();
            editor.PutBoolean(key, value);

            // Commit the edits!
            editor.Commit();
        }

        public static string GetString(Context context, string key)
        {
            var settings = context.GetSharedPreferences(myPrefs, 0);
            return settings.GetString(key, string.Empty);
        }

        public static void SetString(Context context, string key, string value)
        {
            // We need an Editor object to make preference changes.
            // All objects are from android.context.Context
            var settings = context.GetSharedPreferences(myPrefs, 0);
            var editor = settings.Edit();
            editor.PutString(key, value);

            // Commit the edits!
            editor.Commit();
        }
    }
}