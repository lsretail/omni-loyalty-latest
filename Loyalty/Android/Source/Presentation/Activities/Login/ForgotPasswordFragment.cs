using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using Presentation.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Login
{
    public class ForgotPasswordFragment : LoyaltyFragment, View.IOnClickListener, IRefreshableActivity, TextView.IOnEditorActionListener
    {
        private EditText username;
        private ProgressButton sendCodeButton;
        private TextView alreadyHaveCode;

        private MemberContactModel contactModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            contactModel = new MemberContactModel(Activity, this);

            var view = Utils.ViewUtils.Inflate(inflater, Resource.Layout.ForgotPasswordScreen);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.ForgotPasswordScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            username = view.FindViewById<EditText>(Resource.Id.ForgotPasswordScreenUsername);
            sendCodeButton = view.FindViewById<ProgressButton>(Resource.Id.ForgotPasswordScreenSendCodeButton);
            alreadyHaveCode = view.FindViewById<TextView>(Resource.Id.ForgotPasswordScreenAlreadyHaveCode);

            username.SetOnEditorActionListener(this);

            sendCodeButton.SetOnClickListener(this);
            alreadyHaveCode.SetOnClickListener(this);

            return view;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.ForgotPasswordScreenUsername:
                    if (actionId == ImeAction.Done)
                        ResetPassword();
                    break;
            }

            return false;
        }

        private async void ResetPassword()
        {
            if (Validate())
            {
                var success = await contactModel.ForgotPasswordForDeviceAsync(username.Text);
                if (string.IsNullOrEmpty(success) == false)
                {
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(ResetPasswordActivity));
                    intent.PutExtra(BundleConstants.Username, username.Text);
                    StartActivity(intent);
                    Activity.Finish();
                }
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(username.Text))
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = GetString(Resource.String.AccountViewUserNameEmpty);
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