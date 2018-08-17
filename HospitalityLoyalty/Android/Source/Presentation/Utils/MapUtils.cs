using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Globalization;

namespace Presentation.Utils
{
    public class MapUtils
    {
        public static void ShowDirections(Context context, string storeId)
        {
            var store = AppData.Stores.FirstOrDefault(x => x.Id == storeId);

            Intent intent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?saddr=&daddr=" + store.Latitude.ToString(CultureInfo.InvariantCulture) + "," + store.Longitude.ToString(CultureInfo.InvariantCulture)));
            context.StartActivity(intent);
        }
    }
}