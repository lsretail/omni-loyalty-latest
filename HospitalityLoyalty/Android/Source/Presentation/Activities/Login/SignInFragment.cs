using System;
using System.Collections.Generic;
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
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;

namespace Presentation.Activities.Login
{
    public class SignInFragment : BaseFragment, TextView.IOnEditorActionListener, View.IOnClickListener, IRefreshableActivity, IBroadcastObserver
    {
        private EditText email;
        private EditText password;
        private ProgressButton progressButton;

        private ContactModel contactModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //var view = inflater.Inflate(Resource.Layout.SignInScreen, null);
            var view = Inflate(inflater, Resource.Layout.SignInScreen, null);

            email = view.FindViewById<EditText>(Resource.Id.SignInScreenEmail);
            password = view.FindViewById<EditText>(Resource.Id.SignInScreenPassword);
            progressButton = view.FindViewById<ProgressButton>(Resource.Id.SignInScreenSignInButton);

            contactModel = new ContactModel(Activity, this);

            password.SetOnEditorActionListener(this);
            view.FindViewById(Resource.Id.SignInScreenForgotPassword).SetOnClickListener(this);
            progressButton.SetOnClickListener(this);

            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.GetAccounts) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Activity, new[] { Manifest.Permission.GetAccounts }, 0);
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

        public bool OnEditorAction(TextView v, ImeAction action, KeyEvent e)
        {
            switch (action)
            {
                case ImeAction.Done:
                    Login();

                    break;
            }

            return false;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.SignInScreenSignInButton:
                    Login();

                    break;
                case Resource.Id.SignInScreenForgotPassword:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof (ForgotPasswordActivity));
                    StartActivity(intent);
                    break;
            }
        }

        private bool Validate()
        {
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
            /*if (!RegexUtilities.IsValidEmail(email.Text))
            {
                Toast.MakeText(Activity, Resource.String.LoginEmailValid, ToastLength.Long).Show();
                return false;
            }*/

            return true;
        }

        private void Login()
        {
            if (Validate())
            {
                contactModel.Login(email.Text, password.Text);
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

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                email.Enabled = false;
                password.Enabled = false;

                progressButton.State = ProgressButton.ProgressButtonState.Loading;
            }
            else
            {
                email.Enabled = true;
                password.Enabled = true;

                progressButton.State = ProgressButton.ProgressButtonState.Normal;
            }
        }
    }
}