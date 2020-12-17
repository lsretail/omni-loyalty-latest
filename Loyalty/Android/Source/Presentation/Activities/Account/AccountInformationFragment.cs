using System;

using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;

using Java.Lang;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace Presentation.Activities.Account
{
    public class AccountInformationFragment : LoyaltyFragment, View.IOnClickListener, ITextWatcher
    {
        public enum InputBox
        {
            None = 0,
            UserName = 1,
            Password = 2,
            PasswordConfirmation = 3,
            Email = 4,
            Name = 5,
            AddressOne = 8,
            AddressTwo = 9,
            PostCode = 10,
            City = 11,
            State = 12,
            Country = 13,
            Phone = 14
        }

        private bool userNameRestriction;
        private Tuple<bool, bool> emailRestriction;
        private Tuple<bool, bool> firstNameRestriction;
        private Tuple<bool, bool> lastNameRestriction;
        private Tuple<bool, bool> address1Restriction;
        private Tuple<bool, bool> address2Restriction;
        private Tuple<bool, bool> cityRestriction;
        private Tuple<bool, bool> stateRestriction;
        private Tuple<bool, bool> postCodeRestriction;
        private Tuple<bool, bool> countryRestriction;
        private Tuple<bool, bool> phoneRestriction;

        private EditText userName;
        private EditText password;
        private EditText passwordConfirmation;
        private EditText email;
        private EditText name;
        private EditText addressOne;
        private EditText addressTwo;
        private EditText postCode;
        private EditText city;
        private EditText state;
        private EditText country;
        private EditText phone;
        //private TextView passwordPolicy;
        private ColoredButton changePassword;

        private TextInputLayout userNameInputLayout;
        private TextInputLayout emailNameInputLayout;
        private TextInputLayout passwordInputLayout;
        private TextInputLayout passwordConfirmInputLayout;
        private TextInputLayout nameInputLayout;
        private TextInputLayout addressOneInputLayout;
        private TextInputLayout addressTwoInputLayout;
        private TextInputLayout postCodeInputLayout;
        private TextInputLayout cityInputLayout;
        private TextInputLayout stateInputLayout;
        private TextInputLayout countryInputLayout;
        private TextInputLayout phoneInputLayout;

        private string currentUserNameError = string.Empty;
        private string currentEmailError = string.Empty;
        private string currentPasswordError = string.Empty;
        private string currentPasswordConfirmError = string.Empty;
        private string currentNameError = string.Empty;
        private string currentPhoneError = string.Empty;
        private string currentAddressOneError = string.Empty;
        private string currentAddressTwoError = string.Empty;
        private string currentPostCodeError = string.Empty;
        private string currentCityError = string.Empty;
        private string currentStateError = string.Empty;
        private string currentCountryError = string.Empty;

        private GeneralSearchModel searchModel;
        private bool valueSet;
        private bool isBigScreen;

        private string passwordRules = string.Empty;

        public bool HideUsernamePassword { get; set; }

        public Action OnNextPressed { get; set; }
        public string AccountName { get; set; }
        public string FirstNameText { get; set; }
        public string MiddleNameText { get; set; }
        public string LastNameText { get; set; }

        public static AccountInformationFragment NewInstance()
        {
            var accountInformationDetail = new AccountInformationFragment() { Arguments = new Bundle() };
            return accountInformationDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null )
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            if (HideUsernamePassword) //manage account
            {
                userNameRestriction = MemberContactAttributes.Manage.Username;
                emailRestriction = MemberContactAttributes.Manage.Email;
                firstNameRestriction = MemberContactAttributes.Manage.FirstName;
                lastNameRestriction = MemberContactAttributes.Manage.LastName;
                address1Restriction = MemberContactAttributes.Manage.Address1;
                address2Restriction = MemberContactAttributes.Manage.Address2;
                cityRestriction = MemberContactAttributes.Manage.City;
                stateRestriction = MemberContactAttributes.Manage.State;
                postCodeRestriction = MemberContactAttributes.Manage.PostCode;
                countryRestriction = MemberContactAttributes.Manage.Country;
                phoneRestriction = MemberContactAttributes.Manage.Phone;
            }
            else
            {
                userNameRestriction = MemberContactAttributes.Registration.Username;
                emailRestriction = MemberContactAttributes.Registration.Email;
                firstNameRestriction = MemberContactAttributes.Registration.FirstName;
                lastNameRestriction = MemberContactAttributes.Registration.LastName;
                address1Restriction = MemberContactAttributes.Registration.Address1;
                address2Restriction = MemberContactAttributes.Registration.Address2;
                cityRestriction = MemberContactAttributes.Registration.City;
                stateRestriction = MemberContactAttributes.Registration.State;
                postCodeRestriction = MemberContactAttributes.Registration.PostCode;
                countryRestriction = MemberContactAttributes.Registration.Country;
                phoneRestriction = MemberContactAttributes.Registration.Phone;
            }

            searchModel = new GeneralSearchModel(Activity, null);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.AccountInformation);

            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            userName = view.FindViewById<EditText>(Resource.Id.AccountInformationViewUserName);
            password = view.FindViewById<EditText>(Resource.Id.AccountInformationViewPassword);
            //passwordPolicy = view.FindViewById<TextView>(Resource.Id.AccountInformationPasswordPolicy);
            passwordConfirmation = view.FindViewById<EditText>(Resource.Id.AccountInformationViewPasswordConfirm);
            changePassword = view.FindViewById<ColoredButton>(Resource.Id.AccountInformationViewChangePasswordButton);
            email = view.FindViewById<EditText>(Resource.Id.AccountInformationViewEmail);
            name = view.FindViewById<EditText>(Resource.Id.AccountInformationViewName);
            addressOne = view.FindViewById<EditText>(Resource.Id.AccountInformationViewAddressOne);
            addressTwo = view.FindViewById<EditText>(Resource.Id.AccountInformationViewAddressTwo);
            city = view.FindViewById<EditText>(Resource.Id.AccountInformationViewCity);
            postCode = view.FindViewById<EditText>(Resource.Id.AccountInformationViewPostCode);
            state = view.FindViewById<EditText>(Resource.Id.AccountInformationViewState);
            country = view.FindViewById<EditText>(Resource.Id.AccountInformationViewCountry);
            phone = view.FindViewById<EditText>(Resource.Id.AccountInformationViewPhone);

            userNameInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewUserNameInputLayout);
            emailNameInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewEmailInputLayout);
            passwordInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewPasswordInputLayout);
            passwordConfirmInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewPasswordConfirmInputLayout);
            nameInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewNameInputLayout);
            addressOneInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewAddressOneInputLayout);
            addressTwoInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewAddressTwoInputLayout);
            postCodeInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewPostCodeInputLayout);
            cityInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewCityInputLayout);
            stateInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewStateInputLayout);
            countryInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewCountryInputLayout);
            phoneInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.AccountInformationViewPhoneInputLayout);

            var nextButton = view.FindViewById<ColoredButton>(Resource.Id.AccountInformationNext);
            if (isBigScreen)
            {
                nextButton.Visibility = ViewStates.Gone;
            }
            else
            {
                nextButton.SetOnClickListener(this);
            }

            changePassword.SetOnClickListener(this);

            LoadAppsettings();

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            userName.AddTextChangedListener(this);
            email.AddTextChangedListener(this);
            password.AddTextChangedListener(this);
            passwordConfirmation.AddTextChangedListener(this);
            name.AddTextChangedListener(this);
            addressOne.AddTextChangedListener(this);
            addressTwo.AddTextChangedListener(this);
            city.AddTextChangedListener(this);
            postCode.AddTextChangedListener(this);
            state.AddTextChangedListener(this);
            country.AddTextChangedListener(this);
            phone.AddTextChangedListener(this);

            InitializeTextFields();
        }

        public override void OnPause()
        {
            userName.RemoveTextChangedListener(this);
            email.RemoveTextChangedListener(this);
            password.RemoveTextChangedListener(this);
            passwordConfirmation.RemoveTextChangedListener(this);
            name.RemoveTextChangedListener(this);
            addressOne.RemoveTextChangedListener(this);
            addressTwo.RemoveTextChangedListener(this);
            city.RemoveTextChangedListener(this);
            postCode.RemoveTextChangedListener(this);
            state.RemoveTextChangedListener(this);
            country.RemoveTextChangedListener(this);
            phone.RemoveTextChangedListener(this);

            currentUserNameError = string.Empty;
            currentEmailError = string.Empty;
            currentPasswordError = string.Empty;
            currentPasswordConfirmError = string.Empty;
            currentNameError = string.Empty;
            currentPhoneError = string.Empty;
            currentAddressOneError = string.Empty;
            currentAddressTwoError = string.Empty;
            currentPostCodeError = string.Empty;
            currentCityError = string.Empty;
            currentStateError = string.Empty;
            currentCountryError = string.Empty;

            base.OnPause();
        }

        private async void LoadAppsettings()
        {
            var results = await searchModel.AppSettings(ConfigKey.Password_Policy);

            passwordRules = results;
            ValidateTextFields();
        }

        public void AfterTextChanged(IEditable s)
        {
            ValidateTextFields();
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            //nothing
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            //nothing
        }

        private void InitializeTextFields()
        {
            if (!userNameRestriction)
            {
                userNameInputLayout.Visibility = ViewStates.Gone;
            }

            if (!emailRestriction.Item1)
            {
                emailNameInputLayout.Visibility = ViewStates.Gone;
            }

            if (!firstNameRestriction.Item1 && !lastNameRestriction.Item1)
            {
                nameInputLayout.Visibility = ViewStates.Gone;
            }

            if (!phoneRestriction.Item1)
            {
                phoneInputLayout.Visibility = ViewStates.Gone;
            }

            if (!address1Restriction.Item1)
            {
                addressOne.Visibility = ViewStates.Gone;
            }

            if (!address2Restriction.Item1)
            {
                addressTwo.Visibility = ViewStates.Gone;
            }

            if (!cityRestriction.Item1)
            {
                cityInputLayout.Visibility = ViewStates.Gone;
            }

            if (!stateRestriction.Item1)
            {
                stateInputLayout.Visibility = ViewStates.Gone;
            }

            if (!postCodeRestriction.Item1)
            {
                postCodeInputLayout.Visibility = ViewStates.Gone;
            }

            if (!countryRestriction.Item1)
            {
                countryInputLayout.Visibility = ViewStates.Gone;
            }

            ValidateTextFields();
        }

        private void ValidateTextFields()
        {
            //username
            if (userNameRestriction && string.IsNullOrEmpty(userName.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentUserNameError != errormessage)
                {
                    currentUserNameError = errormessage;
                    userNameInputLayout.Error = errormessage;
                }

                userNameInputLayout.ErrorEnabled = true;
            }
            else
            {
                currentUserNameError = string.Empty;

                userNameInputLayout.ErrorEnabled = (false);
            }

            //email
            if (emailRestriction.Item1 && emailRestriction.Item2 && string.IsNullOrEmpty(email.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentEmailError != errormessage)
                {
                    currentEmailError = errormessage;
                    emailNameInputLayout.Error = (errormessage);
                }

                emailNameInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentEmailError = string.Empty;

                emailNameInputLayout.ErrorEnabled = (false);
            }

            //password
            if (string.IsNullOrEmpty(password.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentPasswordError != errormessage)
                {
                    currentPasswordError = errormessage;
                    passwordInputLayout.Error = (errormessage);
                }

                passwordInputLayout.ErrorEnabled = (true);
            }
            else
            {
                var errormessage = passwordRules;

                if (currentPasswordError != errormessage)
                {
                    currentPasswordError = errormessage;
                    passwordInputLayout.Error = (errormessage);
                }

                passwordInputLayout.ErrorEnabled = (true);
            }

            //password confirm
            if (string.IsNullOrEmpty(passwordConfirmation.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentPasswordConfirmError != errormessage)
                {
                    currentPasswordConfirmError = errormessage;
                    passwordConfirmInputLayout.Error = (errormessage);
                }

                passwordConfirmInputLayout.ErrorEnabled = (true);
            }
            else if (password.Text != passwordConfirmation.Text)
            {
                var errormessage = GetString(Resource.String.ChangePasswordPasswordsDontMatch);

                if (currentPasswordConfirmError != errormessage)
                {
                    currentPasswordConfirmError = errormessage;
                    passwordConfirmInputLayout.Error = (errormessage);
                }

                passwordConfirmInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentPasswordConfirmError = string.Empty;

                passwordConfirmInputLayout.ErrorEnabled = (false);
            }

            //Name
            if (((firstNameRestriction.Item1 && firstNameRestriction.Item2)
                || (lastNameRestriction.Item1 && lastNameRestriction.Item2))
                && string.IsNullOrEmpty(name.Text)) //if a name is required
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentNameError != errormessage)
                {
                    currentNameError = errormessage;
                    nameInputLayout.Error = (errormessage);
                }

                nameInputLayout.ErrorEnabled = (true);
            }
            else if (lastNameRestriction.Item1 && lastNameRestriction.Item2 && !MemberContact.NameContainsLastName(name.Text)) //if last name is missing
            {
                var errormessage = GetString(Resource.String.AccountViewNameMustContainLastName);

                if (currentNameError != errormessage)
                {
                    currentNameError = errormessage;
                    nameInputLayout.Error = (errormessage);
                }

                nameInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentNameError = string.Empty;

                nameInputLayout.ErrorEnabled = (false);
            }

            //Phone
            if (phoneRestriction.Item1 && phoneRestriction.Item2 && string.IsNullOrEmpty(phone.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentPhoneError != errormessage)
                {
                    currentPhoneError = errormessage;
                    phoneInputLayout.Error = (errormessage);
                }

                phoneInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentPhoneError = string.Empty;

                phoneInputLayout.ErrorEnabled = (false);
            }

            //Address1
            if (address1Restriction.Item1 && address1Restriction.Item2 && string.IsNullOrEmpty(addressOne.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentAddressOneError != errormessage)
                {
                    currentAddressOneError = errormessage;
                    addressOneInputLayout.Error = (errormessage);
                }

                addressOneInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentAddressOneError = string.Empty;

                addressOneInputLayout.ErrorEnabled = (false);
            }

            //Address2
            if (address2Restriction.Item1 && address2Restriction.Item2 && string.IsNullOrEmpty(addressTwo.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentAddressTwoError != errormessage)
                {
                    currentAddressTwoError = errormessage;
                    addressTwoInputLayout.Error = (errormessage);
                }

                addressTwoInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentAddressTwoError = string.Empty;

                addressTwoInputLayout.ErrorEnabled = (false);
            }

            //City
            if (cityRestriction.Item1 && cityRestriction.Item2 && string.IsNullOrEmpty(city.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentCityError != errormessage)
                {
                    currentCityError = errormessage;
                    cityInputLayout.Error = (errormessage);
                }

                cityInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentCityError = string.Empty;

                cityInputLayout.ErrorEnabled = (false);
            }

            //State
            if (stateRestriction.Item1 && stateRestriction.Item2 && string.IsNullOrEmpty(state.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentStateError != errormessage)
                {
                    currentStateError = errormessage;
                    stateInputLayout.Error = (errormessage);
                }

                stateInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentStateError = string.Empty;

                stateInputLayout.ErrorEnabled = (false);
            }

            //Post code
            if (postCodeRestriction.Item1 && postCodeRestriction.Item2 && string.IsNullOrEmpty(postCode.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentPostCodeError != errormessage)
                {
                    currentPostCodeError = errormessage;
                    postCodeInputLayout.Error = (errormessage);
                }

                postCodeInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentPostCodeError = string.Empty;

                postCodeInputLayout.ErrorEnabled = (false);
            }

            //country
            if (countryRestriction.Item1 && countryRestriction.Item2 && string.IsNullOrEmpty(country.Text))
            {
                var errormessage = GetString(Resource.String.ApplicationRequired);

                if (currentCountryError != errormessage)
                {
                    currentCountryError = errormessage;
                    countryInputLayout.Error = (errormessage);
                }

                countryInputLayout.ErrorEnabled = (true);
            }
            else
            {
                currentCountryError = string.Empty;

                countryInputLayout.ErrorEnabled = (false);
            }
        }

        public override void OnActivityCreated(Bundle p0)
        {
            base.OnActivityCreated(p0);

            if (HideUsernamePassword)           //EDIT ACCOUNT
            {
                var view = View.FindViewById<LinearLayout>(Resource.Id.AccountInformationViewPasswordFields);
                view.Visibility = ViewStates.Gone;

                if (!valueSet)
                {
                    var contact = AppData.Device.UserLoggedOnToDevice;
                    userName.Text = contact.UserName;
                    email.Text = contact.Email;
                    name.Text = contact.Name;
                    if (contact.Addresses != null && contact.Addresses.Count > 0)
                    {
                        addressOne.Text = contact.Addresses[0].Address1;
                        addressTwo.Text = contact.Addresses[0].Address2;
                        city.Text = contact.Addresses[0].City;
                        postCode.Text = contact.Addresses[0].PostCode;
                        state.Text = contact.Addresses[0].StateProvinceRegion;
                        country.Text = contact.Addresses[0].Country;
                        phone.Text = contact.Addresses[0].PhoneNumber;
                    }

                    valueSet = true;
                }

                userName.Enabled = false;
            }
            else
            {
                var changePasswordView = View.FindViewById<LinearLayout>(Resource.Id.AccountInformationViewChangePassword);
                changePasswordView.Visibility = ViewStates.Gone;

                if (!string.IsNullOrEmpty(AccountName))
                {
                    var passwordFieldView = View.FindViewById<LinearLayout>(Resource.Id.AccountInformationViewPasswordFields);
                    passwordFieldView.Visibility = ViewStates.Gone;

                    userName.Text = AccountName;
                    email.Text = AccountName;
                    name.Text = FirstNameText;

                    if(!string.IsNullOrEmpty(MiddleNameText))
                        name.Text += " " + MiddleNameText;

                    if (!string.IsNullOrEmpty(LastNameText))
                        name.Text += " " + LastNameText;

                    userName.Enabled = false;

                    //todo
                    //View.FindViewById<View>(Resource.Id.AccountInformationViewEmailLabel).Visibility = ViewStates.Gone;
                    email.Visibility = ViewStates.Gone;
                }
            }
        }

        public void FocusView(InputBox input)
        {
            switch (input)
            {
                case InputBox.UserName:
                    userName.RequestFocus();
                    break;
                case InputBox.Password:
                    password.RequestFocus();
                    break;
                case InputBox.PasswordConfirmation:
                    passwordConfirmation.RequestFocus();
                    break;
                case InputBox.Email:
                    email.RequestFocus();
                    break;
                case InputBox.Name:
                    name.RequestFocus();
                    break;
                case InputBox.AddressOne:
                    addressOne.RequestFocus();
                    break;
                case InputBox.AddressTwo:
                    addressTwo.RequestFocus();
                    break;
                case InputBox.PostCode:
                    postCode.RequestFocus();
                    break;
                case InputBox.City:
                    city.RequestFocus();
                    break;
                case InputBox.State:
                    state.RequestFocus();
                    break;
                case InputBox.Country:
                    country.RequestFocus();
                    break;
            }
        }

        public EditText UserName
        {
            get { return userName; }
        }

        public EditText Password
        {
            get { return password; }
        }

        public EditText PasswordConfirmation
        {
            get { return passwordConfirmation; }
        }

        public EditText Email
        {
            get { return email; }
        }

        public EditText Name
        {
            get { return name; }
        }

        public EditText AddressOne
        {
            get { return addressOne; }
        }

        public EditText AddressTwo
        {
            get { return addressTwo; }
        }

        public EditText City
        {
            get { return city; }
        }

        public EditText PostCode
        {
            get { return postCode; }
        }

        public EditText State
        {
            get { return state; }
        }

        public EditText Country
        {
            get { return country; }
        }

        public EditText Phone
        {
            get { return phone; }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.AccountInformationViewChangePasswordButton:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(ChangePasswordActivity));
                    StartActivity(intent);
                    break;

                case Resource.Id.AccountInformationNext:
                    if (OnNextPressed != null)
                    {
                        OnNextPressed();
                    }
                    break;
            }
        }
    }
}