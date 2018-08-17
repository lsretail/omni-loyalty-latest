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

namespace Presentation.Activities.Contact
{
    public class ChangePasswordFragment : BaseFragment, View.IOnClickListener, IRefreshableActivity, TextView.IOnEditorActionListener
    {
        private EditText oldPassword;
        private EditText newPassword;
        private EditText newPasswordConfirm;

        private ProgressButton changePasswordButton;

        private ContactModel contactModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            contactModel = new ContactModel(Activity, this);

            var view = Inflate(inflater, Resource.Layout.ChangePasswordScreen);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.ChangePasswordScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            oldPassword = view.FindViewById<EditText>(Resource.Id.ChangePasswordScreenOldPassword);
            newPassword = view.FindViewById<EditText>(Resource.Id.ChangePasswordScreenNewPassword);
            newPasswordConfirm = view.FindViewById<EditText>(Resource.Id.ChangePasswordScreenNewPasswordConfirm);
            changePasswordButton = view.FindViewById<ProgressButton>(Resource.Id.ChangePasswordScreenChangePasswordButton);

            newPasswordConfirm.SetOnEditorActionListener(this);

            changePasswordButton.SetOnClickListener(this);

            return view;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ChangePasswordScreenChangePasswordButton:
                    ChangePassword();
                    break;
            }
        }

        private async void ChangePassword()
        {
            if (Validate())
            {
                var success = await contactModel.ChangePassword(oldPassword.Text, newPassword.Text);

                if (success)
                {
                    Activity.Finish();
                }
            }
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.ChangePasswordScreenNewPasswordConfirm:
                    ChangePassword();
                    break;
            }

            return false;
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(oldPassword.Text))
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = Resources.GetString(Resource.String.ContactOldPasswordEmpty);
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
                changePasswordButton.State = ProgressButton.ProgressButtonState.Loading;
            }
            else
            {
                changePasswordButton.State = ProgressButton.ProgressButtonState.Normal;
            }
        }
    }
}