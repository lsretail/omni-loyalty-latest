using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Presentation.Dialogs
{
    class WarningDialog : BaseAlertDialog
    {

        public WarningDialog(Context context, string title)
            : base(context)
        {
            Title = title;
        }
    }
}