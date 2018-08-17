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
    public class UpdateContactFragment : BaseFragment, View.IOnClickListener, IRefreshableActivity, TextView.IOnEditorActionListener
    {
        private EditText name;
        private EditText email;

        private ProgressButton updateButton;

        private ContactModel contactModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            contactModel = new ContactModel(Activity, this);

            var view = Inflate(inflater, Resource.Layout.UpdateContactScreen);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.UpdateContactScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            name = view.FindViewById<EditText>(Resource.Id.UpdateContactScreenName);
            email = view.FindViewById<EditText>(Resource.Id.UpdateContactScreenEmail);

            name.Text = AppData.Contact.Name;
            email.Text = AppData.Contact.Email;

            updateButton = view.FindViewById<ProgressButton>(Resource.Id.UpdateContactScreenUpdateButton);
            updateButton.SetOnClickListener(this);

            email.SetOnEditorActionListener(this);

            return view;
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                updateButton.State = ProgressButton.ProgressButtonState.Loading;
            }
            else
            {
                updateButton.State = ProgressButton.ProgressButtonState.Normal;
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.UpdateContactScreenUpdateButton:
                    UpdateContact();
                    break;
            }
        }

        public async void UpdateContact()
        {
            if (Validate())
            {
                var success = await contactModel.ContactUpdate(name.Text, email.Text);

                if (success)
                {
                    Activity.Finish();
                }
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(name.Text))
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = Resources.GetString(Resource.String.LoginNameEmpty);
                dialog.Show();

                return false;
            }

            if (string.IsNullOrEmpty(email.Text))
            {
                var dialog = new WarningDialog(Activity, "");
                dialog.Message = Resources.GetString(Resource.String.LoginEmailEmpty);
                dialog.Show();

                return false;
            }

            return true;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.UpdateContactScreenEmail:
                    UpdateContact();
                    break;
            }

            return false;
        }
    }
}