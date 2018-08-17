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

namespace Presentation.Dialogs
{
    public class CustomProgressDialog : ProgressDialog
    {
        private string title;
        private string message;

        public CustomProgressDialog(Context context) : base(context, Android.Resource.Style.ThemeDialog)
        {
            RequestWindowFeature((int)WindowFeatures.NoTitle);
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public override void Show()
        {
            base.SetCancelable(false);

            base.Show();

            SetContentView(Resource.Layout.CustomProgressDialog);

            var titleView = (TextView)FindViewById(Resource.Id.CustomProgressDialogTitle);
            var messageView = (TextView)FindViewById(Resource.Id.CustomProgressDialogMessage);
            
            titleView.Text = title;
            messageView.Text = message;
        }
    }
}