using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Android;
using Android.Accounts;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;

namespace Presentation.Activities.Login
{
    public class SignUpFragment : BaseFragment, TextView.IOnEditorActionListener, View.IOnClickListener, IRefreshableActivity, IBroadcastObserver
    {
        private EditText email;
        private EditText name;
        private EditText password;
        private EditText passwordConfirm;
        private ProgressButton registerButton;
        private TextView passwordPolicy;

        private ContactModel contactModel;
        private AppSettingsModel appSettingsModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //var view = inflater.Inflate(Resource.Layout.SignUpScreen, null);
            var view = Inflate(inflater, Resource.Layout.SignUpScreen, null);

            email = view.FindViewById<EditText>(Resource.Id.SignUpScreenEmail);
            name = view.FindViewById<EditText>(Resource.Id.SignUpScreenName);
            password = view.FindViewById<EditText>(Resource.Id.SignUpScreenPassword);
            passwordConfirm = view.FindViewById<EditText>(Resource.Id.SignUpScreenPasswordConfirm);
            registerButton = view.FindViewById<ProgressButton>(Resource.Id.SignUpScreenSignUpButton);
            passwordPolicy = view.FindViewById<TextView>(Resource.Id.SignUpScreenPasswordPolicy);

            contactModel = new ContactModel(Activity, this);
            appSettingsModel = new AppSettingsModel(Activity, null);

            passwordConfirm.SetOnEditorActionListener(this);
            registerButton.SetOnClickListener(this);

            GetPasswordPolicy();

            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.GetAccounts) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Activity, new[] {Manifest.Permission.GetAccounts}, 0);
            }

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }

            if (string.IsNullOrEmpty(email.Text))
            {
                var emails = new List<string>();

                var emailPattern = Patterns.EmailAddress;
                var accounts = AccountManager.Get(Activity).GetAccounts();
                foreach (var account in accounts)
                {
                    if (emailPattern.Matcher(account.Name).Matches())
                    {
                        emails.Add(account.Name);
                    }
                }

                if (emails.Count > 0)
                {
                    //email.Threshold = 1;
                    //email.Adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SelectDialogItem, emails.ToArray());
                    email.Text = emails[0];
                }
            }
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        private async void GetPasswordPolicy()
        {
            var policy = await appSettingsModel.AppSettingsGetByKey(AppSettingsKey.Password_Policy,CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            passwordPolicy.Text = policy;
            passwordPolicy.Visibility = ViewStates.Visible;
        }

        public bool OnEditorAction(TextView v, ImeAction action, KeyEvent e)
        {
            switch (action)
            {
                case ImeAction.Done:
                    Register();
                    break;
            }

            return false;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.SignUpScreenSignUpButton:
                    Register();
                    break;
            }
        }

        private void Register()
        {
            if (Validate())
            {
                contactModel.ContactCreate(name.Text, email.Text, password.Text);
            }
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.ContactUpdated)
            {
                if (Activity is HomeActivity)
                {
                    (Activity as HomeActivity).SelectItem(HospActivity.ActivityTypes.DefaultItem);
                }
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(name.Text))
            {
                Toast.MakeText(Activity, Resource.String.LoginNameEmpty, ToastLength.Long).Show();
                return false;
            }
            if (string.IsNullOrEmpty(email.Text))
            {
                Toast.MakeText(Activity, Resource.String.LoginEmailEmpty, ToastLength.Long).Show();
                return false;
            }
            if (string.IsNullOrEmpty(password.Text))
            {
                Toast.MakeText(Activity, Resource.String.LoginPasswordEmpty, ToastLength.Long).Show();
                return false;
            }
            if (password.Text != passwordConfirm.Text)
            {
                Toast.MakeText(Activity, Resource.String.LoginPasswordMatch, ToastLength.Long).Show();
                return false;
            }
            if (!RegexUtilities.IsValidEmail(email.Text))
            {
                Toast.MakeText(Activity, Resource.String.LoginEmailValid, ToastLength.Long).Show();
                return false;
            }

            return true;
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                email.Enabled = false;
                password.Enabled = false;
                passwordConfirm.Enabled = false;
                registerButton.State = ProgressButton.ProgressButtonState.Loading;
            }
            else
            {
                email.Enabled = true;
                password.Enabled = true;
                passwordConfirm.Enabled = true;

                registerButton.State = ProgressButton.ProgressButtonState.Normal;
            }
        }
    }
}