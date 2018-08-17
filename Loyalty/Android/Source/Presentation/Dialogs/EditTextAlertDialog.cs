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
using Android.Views.InputMethods;
using Android.Widget;

namespace Presentation.Dialogs
{
    public class EditTextAlertDialog : BaseAlertDialog
    {
        private View view;
        
        public EditText EditText
        {
            get { return view.FindViewById<EditText>(Resource.Id.AlertDialogEditTextViewEditText); }
        }

        public EditTextAlertDialog(Context context, string title, string message = "") : base(context)
        {
            Message = message;
            Title = title;

            view = Util.Utils.ViewUtils.Inflate(LayoutInflater, Resource.Layout.AlertDialogEditText);
        }

        public override void Show()
        {
            CreateDialog(view, true);
        }
    }
}