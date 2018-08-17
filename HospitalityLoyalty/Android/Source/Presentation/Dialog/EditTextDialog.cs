using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace Presentation.Dialog
{
    class EditTextDialog : BaseAlertDialog, View.IOnFocusChangeListener
    {
        private View view;
        private EditText editText;
        
        public EditText EditText
        {
            get
            {
                if (editText == null)
                {
                    editText = view.FindViewById<EditText>(Resource.Id.AlertDialogEditTextViewEditText);
                }

                return editText;
            }
        }

        public EditTextDialog(Context context, string title, string message = "")
            : base(context)
        {
            Delay = true;

            Message = message;
            Title = title;

            view = Utils.Utils.ViewUtils.Inflate(LayoutInflater, Resource.Layout.AlertDialogEditText);

            EditText.OnFocusChangeListener = this;
        }

        public override void Show()
        {
            CreateDialog(view, true);
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
        }

        public override void OnClick(View v)
        {
            EditText.ClearFocus();

            base.OnClick(v);
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            if (!hasFocus)
            {
                InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
                //imm.HideSoftInputFromInputMethod(EditText.WindowToken, 0);
                imm.HideSoftInputFromWindow(EditText.WindowToken, 0);
            }
        }
    }
}