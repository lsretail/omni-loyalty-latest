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
    public class LogUtils
    {
        public static void Log(string text)
        {
#if DEBUG
            Android.Util.Log.Debug("LS Retail Casual Dining", text);
#endif
        }
    }
}