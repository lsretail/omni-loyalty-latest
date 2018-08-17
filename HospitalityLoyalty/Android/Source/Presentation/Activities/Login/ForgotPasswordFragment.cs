using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Dialog;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Login
{
    public class ForgotPasswordFragment : BaseFragment, View.IOnClickListener, IRefreshableActivity, TextView.IOnEditorActionListener
    {
        private EditText email;
        private ProgressButton sendCodeButton;
        private TextView alreadyHaveCode;

        private ContactModel contactModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            contactModel = new ContactModel(Activity, this);

            var view = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ForgotPasswordScreen);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.ForgotPasswordScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            email = view.FindViewById<EditText>(Resource.Id.ForgotPasswordScreenUsername);
            sendCodeButton = view.FindViewById<ProgressButton>(Resource.Id.ForgotPasswordScreenSendCodeButton);
            alreadyHaveCode = view.FindViewById<TextView>(Resource.Id.ForgotPasswordScreenAlreadyHaveCode);

            email.SetOnEditorActionListener(this);

            sendCodeButton.SetOnClickListener(this);
            alreadyHaveCode.SetOnClickListener(this);

            return view;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.ForgotPasswordScreenUsername:
                    if(actionId == ImeAction.Done)
                        ResetPassword();
                    break;
            }

            return false;
        }

        private async void ResetPassword()
        {
            if (Validate())
            {
                var success = await contactModel.ForgotPasswordForDeviceAsync(email.Text);

                if (success)
                {
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(ResetPasswordActivity));
                    intent.PutExtra(BundleUtils.Email, email.Text);
                    StartActivity(intent);
                    Activity.Finish();
                }
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(email.Text))
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = Resources.GetString(Resource.String.LoginEmailEmpty);
                dialog.Show();
                return false;
            }

            return true;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ForgotPasswordScreenSendCodeButton:
                    ResetPassword();
                    break;

                case Resource.Id.ForgotPasswordScreenAlreadyHaveCode:
                    var resetIntent = new Intent();
                    resetIntent.SetClass(Activity, typeof(ResetPasswordActivity));
                    StartActivity(resetIntent);
                    Activity.Finish();
                    break;
            }
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                sendCodeButton.State = ProgressButton.ProgressButtonState.Loading;
            }
            else
            {
                sendCodeButton.State = ProgressButton.ProgressButtonState.Normal;
            }
        }
    }
}