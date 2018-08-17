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
    public class ResetPasswordFrament : BaseFragment, View.IOnClickListener, IRefreshableActivity, TextView.IOnEditorActionListener
    {
        private EditText email;
        private EditText resetCode;
        private EditText newPassword;
        private EditText newPasswordConfirm;
        private ProgressButton resetPasswordButton;

        private ContactModel contactModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            contactModel = new ContactModel(Activity, this);

            var view = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ResetPasswordScreen);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.ResetPasswordScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            email = view.FindViewById<EditText>(Resource.Id.ResetPasswordScreenEmail);
            resetCode = view.FindViewById<EditText>(Resource.Id.ResetPasswordScreenResetCode);
            newPassword = view.FindViewById<EditText>(Resource.Id.ResetPasswordScreenNewPassword);
            newPasswordConfirm = view.FindViewById<EditText>(Resource.Id.ResetPasswordScreenNewPasswordConfirm);
            resetPasswordButton = view.FindViewById<ProgressButton>(Resource.Id.ResetPasswordScreenResetPasswordButton);

            newPasswordConfirm.SetOnEditorActionListener(this);

            resetPasswordButton.SetOnClickListener(this);

            if (Arguments != null && Arguments.ContainsKey(BundleUtils.Email))
            {
                email.Text = Arguments.GetString(BundleUtils.Email);
            }

            return view;
        }

        private async void ResetPassword()
        {
            if (Validate())
            {
                var success = await contactModel.ResetPassword(email.Text, resetCode.Text, newPassword.Text);

                if (success)
                {
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

            if (string.IsNullOrEmpty(resetCode.Text))
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = Resources.GetString(Resource.String.ResetPasswordResetCodeEmpty);
                dialog.Show();
                return false;
            }

            if (string.IsNullOrEmpty(newPassword.Text))
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = Resources.GetString(Resource.String.ContactNewPasswordEmpty);
                dialog.Show();
                return false;
            }

            if (newPassword.Text != newPasswordConfirm.Text)
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = Resources.GetString(Resource.String.ContactNewPasswordsMustMatch);
                dialog.Show();
                return false;
            }

            return true;
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                resetPasswordButton.State = ProgressButton.ProgressButtonState.Loading;
            }
            else
            {
                resetPasswordButton.State = ProgressButton.ProgressButtonState.Normal;
            }
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.ResetPasswordScreenNewPasswordConfirm:
                    if (actionId == ImeAction.Done)
                        ResetPassword();
                    break;
            }

            return false;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ResetPasswordScreenResetPasswordButton:
                    ResetPassword();
                    break;
            }
        }
    }
}