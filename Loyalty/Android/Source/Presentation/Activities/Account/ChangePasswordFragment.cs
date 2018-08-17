using System;

using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using LSRetail.Omni.Domain.DataModel.Base;

namespace Presentation.Activities.Account
{
    public class ChangePasswordFragment : LoyaltyFragment, TextView.IOnEditorActionListener, View.IOnClickListener
    {
        private MemberContactModel contactModel;
        private GeneralSearchModel searchModel;

        private TextView passwordPolicy;

        public static ChangePasswordFragment NewInstance()
        {
            var changeFragmentDetail = new ChangePasswordFragment() { Arguments = new Bundle() };
            return changeFragmentDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ChangePassword);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.ChangePasswordScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            contactModel = new MemberContactModel(Activity);
            searchModel = new GeneralSearchModel(Activity, null);

            var newPassConfirm = view.FindViewById<EditText>(Resource.Id.ChangePasswordViewPasswordConfirmation);
            newPassConfirm.SetOnEditorActionListener(this);

            view.FindViewById<ColoredButton>(Resource.Id.ChangePasswordViewChangePassword).SetOnClickListener(this);
            passwordPolicy = view.FindViewById<TextView>(Resource.Id.ChangePasswordViewPasswordPolicy);

            LoadAppsettings();

            return view;
        }

        private async void LoadAppsettings()
        {
            var results = await searchModel.AppSettings(AppSettingsKey.Password_Policy);

            passwordPolicy.Text = results;
            passwordPolicy.Visibility = ViewStates.Visible;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (actionId)
            {
                case ImeAction.Done:
                    ChangePassword();
                    return true;
            }
            return false;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ChangePasswordViewChangePassword:
                    ChangePassword();
                    break;
            }
        }

        public async void ChangePassword()
        {
            if (ValidateData())
            {
                var oldPass = View.FindViewById<EditText>(Resource.Id.ChangePasswordViewOldPassword);
                var newPass = View.FindViewById<EditText>(Resource.Id.ChangePasswordViewPassword);

                var success = await contactModel.ChangePassword(AppData.Device.UserLoggedOnToDevice.UserName, newPass.Text, oldPass.Text);

                if (success)
                {
                    OnSuccess();
                }
            }
        }

        private void OnSuccess()
        {
            Activity.Finish();
        }

        private bool ValidateData()
        {
            bool error = false;

            View focusView = null;

            var dialog = new WarningDialog(Activity, "")
                                            .SetPositiveButton(GetString(Resource.String.ApplicationOk), () => focusView.RequestFocus());

            var oldPass = View.FindViewById<EditText>(Resource.Id.ChangePasswordViewOldPassword);
            var newPass = View.FindViewById<EditText>(Resource.Id.ChangePasswordViewPassword);
            var newPassConfirm = View.FindViewById<EditText>(Resource.Id.ChangePasswordViewPasswordConfirmation);

            if (string.IsNullOrEmpty(oldPass.Text))
            {
                dialog.Message = GetString(Resource.String.ChangePasswordOldPasswordEmpty);
                focusView = oldPass;
                error = true;
            }
            else if (string.IsNullOrEmpty(newPass.Text))
            {
                dialog.Message = GetString(Resource.String.ChangePasswordNewPasswordEmpty);
                focusView = newPass;
                error = true;
            }
            else if (newPass.Text != newPassConfirm.Text)
            {
                dialog.Message = GetString(Resource.String.ChangePasswordPasswordsDontMatch);
                focusView = newPass;
                error = true;
            }

            if (error)
            {
                dialog.Show();
            }

            return !error;
        }
    }
}