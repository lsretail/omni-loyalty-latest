using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using LSRetail.Omni.Domain.DataModel.Base;
using Presentation.Activities.Base;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Toolbar = Android.Support.V7.Widget.Toolbar;

using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Activities.Account
{
    public class AccountFragment : LoyaltyFragment
    {
        public enum ShownPage
        {
            Information = 0,
            Profile = 1
        }

        private const string ProfileFragmentTag = "ProfileFragmentTag";
        private const string InformationFragmentTag = "InformationFragmentTag";

        private MemberContactModel model;

        private AccountInformationFragment informationFragment;
        private AccountProfilesFragment profilesFragment;
        
        private CustomProgressDialog progressDialog;

        private bool editAccount;
        private bool isBigScreen;

        public ShownPage CurrentPage { get; set; }

        public static AccountFragment NewInstance()
        {
            var accountFragment = new AccountFragment() { Arguments = new Bundle() };
            return accountFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            View view;

            if (isBigScreen)
            {
                view = Inflate(inflater, Resource.Layout.AccountTabletScreen);
            }
            else
            {
                view = Inflate(inflater, Resource.Layout.AccountScreen);
            }

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.AccountScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);
            
            model = new MemberContactModel(Activity);

            if (Arguments != null)
                editAccount = Arguments.GetBoolean(BundleConstants.EditAccount, false);

            progressDialog = new CustomProgressDialog(Activity);

            if (savedInstanceState == null)
            {
                informationFragment = AccountInformationFragment.NewInstance();
                informationFragment.OnNextPressed = DisplayProfiles;
                informationFragment.HideUsernamePassword = editAccount;

                profilesFragment = AccountProfilesFragment.NewInstance();
                profilesFragment.OnCancelPressed = DisplayInformation;
                profilesFragment.OnCreatePressed = CreateContact;
                profilesFragment.LoadAccountProfiles = editAccount;
            }
            else
            {
                informationFragment = ChildFragmentManager.FindFragmentByTag(InformationFragmentTag) as AccountInformationFragment;
                profilesFragment = ChildFragmentManager.FindFragmentByTag(ProfileFragmentTag) as AccountProfilesFragment;

                if (informationFragment != null)
                {
                    informationFragment.HideUsernamePassword = editAccount;
                    informationFragment.OnNextPressed = DisplayProfiles;
                }
                if (profilesFragment != null)
                {
                    profilesFragment.LoadAccountProfiles = editAccount;
                    profilesFragment.OnCancelPressed = DisplayInformation;
                    profilesFragment.OnCreatePressed = CreateContact;
                }
            }

            string securityKey = string.Empty;

            if (AppData.Device != null)
                securityKey = AppData.Device.SecurityToken;

            Display();

            if (Arguments != null)
            {
                if (Arguments.ContainsKey(BundleConstants.AccountName))
                {
                    informationFragment.AccountName = Arguments.GetString(BundleConstants.AccountName);
                }

                if (Arguments.ContainsKey(BundleConstants.FirstName))
                {
                    informationFragment.FirstNameText = Arguments.GetString(BundleConstants.FirstName);
                }

                if (Arguments.ContainsKey(BundleConstants.MiddleName))
                {
                    informationFragment.MiddleNameText = Arguments.GetString(BundleConstants.MiddleName);
                }

                if (Arguments.ContainsKey(BundleConstants.LastName))
                {
                    informationFragment.LastNameText = Arguments.GetString(BundleConstants.LastName);
                }
            }

            return view;
        }

        public void Display(ShownPage page = ShownPage.Information)
        {
            if (page == ShownPage.Information)
            {
                if (isBigScreen)
                    DisplayBoth();
                else
                    DisplayInformation();
            }
            else
            {
                DisplayProfiles();
            }
        }

        private void DisplayBoth()
        {
            ShowInformation();
            ShowProfiles();
        }

        private void DisplayInformation()
        {
            CurrentPage = ShownPage.Information;

            ShowInformation();
        }

        private void DisplayProfiles()
        {
            if(!ValidateData())
                return;

            ((InputMethodManager)Activity.GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(informationFragment.View.WindowToken, 0);

            CurrentPage = ShownPage.Profile;

            ShowProfiles();
        }

        private async void CreateContact()
        {
            if (ValidateData())
            {
                MemberContact memberContact;
                if (editAccount)
                {
                    memberContact = new MemberContact(AppData.Device.UserLoggedOnToDevice.Id);
                }
                else
                {
                    memberContact = new MemberContact();
                    memberContact.Id = string.Empty;
                }

                memberContact.Email = informationFragment.Email.Text;
                memberContact.Name = informationFragment.Name.Text;
                var address = new Address()
                {
                    Address1 = informationFragment.AddressOne.Text,
                    Address2 = informationFragment.AddressTwo.Text,
                    City = informationFragment.City.Text,
                    StateProvinceRegion = informationFragment.State.Text,
                    PostCode = informationFragment.PostCode.Text,
                    Country = informationFragment.Country.Text,
                    PhoneNumber = informationFragment.Phone.Text
                };

                memberContact.Addresses.Add(address);

                memberContact.Profiles = profilesFragment.Profiles;

                memberContact.LoggedOnToDevice = AppData.Device;

                if (editAccount)
                {
                    if (MemberContactAttributes.Manage.Username)
                    {
                        memberContact.UserName = AppData.Device.UserLoggedOnToDevice.UserName;
                    }
                    else
                    {
                        memberContact.UserName = informationFragment.Email.Text;
                    }

                    memberContact.Password = AppData.Device.UserLoggedOnToDevice.Password;


                    try
                    {
                        var success = await model.UpdateMemberContact(memberContact);

                        if (success)
                        {
                            UpdateMemberContactSuccess();
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateMemberContactError(ex);
                    }
                }
                else
                {
                    if (MemberContactAttributes.Manage.Username)
                    {
                        memberContact.UserName = informationFragment.UserName.Text;
                    }
                    else
                    {
                        memberContact.UserName = informationFragment.Email.Text;
                    }

                    memberContact.Password = informationFragment.Password.Text;

                    try
                    {
                        var success = await model.CreateMemberContact(memberContact);

                        if (success)
                        {
                            CreateMemberContactSuccess();
                        }
                    }
                    catch (Exception ex)
                    {
                        CreateMemberContactError(ex);
                    }
                }
            }
        }

        private void UpdateMemberContactSuccess()
        {
            Activity.Finish();
        }

        private async void UpdateMemberContactError(Exception ex)
        {
            WarningDialog dialog = new WarningDialog(Activity, "");
            AccountInformationFragment.InputBox inputBox = AccountInformationFragment.InputBox.None;

            if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailInvalid)
            {
                inputBox = AccountInformationFragment.InputBox.Email;
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailExists)
            {
                inputBox = AccountInformationFragment.InputBox.Email;
            }

            dialog.Message = await new ExceptionModel(Activity).HandleUIExceptionAsync(ex, displayAlert:false, checkSecurityTokenException:false);

            if (inputBox != AccountInformationFragment.InputBox.None)
            {
                if (!isBigScreen)
                    DisplayInformation();
                informationFragment.FocusView(inputBox);
            }

            dialog.Show();
        }

        private void CreateMemberContactSuccess()
        {
            Activity.SetResult(Result.Ok);
            Activity.Finish();
        }

        private async void CreateMemberContactError(Exception ex)
        {
            WarningDialog dialog = new WarningDialog(Activity, "");
            AccountInformationFragment.InputBox inputBox = AccountInformationFragment.InputBox.None;

            if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.UserNameInvalid || ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.UserNameExists)
            {
                inputBox = AccountInformationFragment.InputBox.UserName;
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.PasswordInvalid)
            {
                inputBox = AccountInformationFragment.InputBox.Password;
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailExists || ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailInvalid)
            {
                inputBox = AccountInformationFragment.InputBox.Email;
            }

            dialog.Message = await new ExceptionModel(Activity).HandleUIExceptionAsync(ex, displayAlert: false, checkSecurityTokenException: false);

            if (inputBox != AccountInformationFragment.InputBox.None)
            {
                DisplayInformation();
                informationFragment.FocusView(inputBox);
            }

            dialog.Show();
        }

        private bool ValidateData()
        {
            var emailRestriction = MemberContactAttributes.Registration.Email;
            var firstNameRestriction = MemberContactAttributes.Registration.FirstName;
            var lastNameRestriction = MemberContactAttributes.Registration.LastName;
            var address1Restriction = MemberContactAttributes.Registration.Address1;
            var address2Restriction = MemberContactAttributes.Registration.Address2;
            var cityRestriction = MemberContactAttributes.Registration.City;
            var stateRestriction = MemberContactAttributes.Registration.State;
            var postCodeRestriction = MemberContactAttributes.Registration.PostCode;
            var countryRestriction = MemberContactAttributes.Registration.Country;
            var phoneRestriction = MemberContactAttributes.Registration.Phone;

            if (editAccount) //manage account
            {
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

            bool error = false;
            AccountInformationFragment.InputBox errorBox = AccountInformationFragment.InputBox.None;

            if (!editAccount && MemberContactAttributes.Registration.Username && string.IsNullOrEmpty(informationFragment.UserName.Text))
            {
                errorBox = AccountInformationFragment.InputBox.UserName;
                error = true;
            }
            else if (!editAccount && string.IsNullOrEmpty(informationFragment.Password.Text))
            {
                errorBox = AccountInformationFragment.InputBox.Password;
                error = true;
            }
            else if (!editAccount && string.IsNullOrEmpty(informationFragment.PasswordConfirmation.Text))
            {
                errorBox = AccountInformationFragment.InputBox.PasswordConfirmation;
                error = true;
            }
            else if (!editAccount && informationFragment.Password.Text != informationFragment.PasswordConfirmation.Text)
            {
                errorBox = AccountInformationFragment.InputBox.PasswordConfirmation;
                error = true;
            }
            else if (emailRestriction.Item1 && emailRestriction.Item2 && string.IsNullOrEmpty(informationFragment.Email.Text))
            {
                errorBox = AccountInformationFragment.InputBox.Email;
                error = true;
            }
            else if (((firstNameRestriction.Item1 && firstNameRestriction.Item2) || (lastNameRestriction.Item1 && lastNameRestriction.Item2)) && string.IsNullOrEmpty(informationFragment.Name.Text))
            {
                errorBox = AccountInformationFragment.InputBox.Name;
                error = true;
            }
            else if (firstNameRestriction.Item1 && firstNameRestriction.Item2 && lastNameRestriction.Item1 && lastNameRestriction.Item2 && !MemberContact.NameContainsLastName(informationFragment.Name.Text))
            {
                errorBox = AccountInformationFragment.InputBox.Name;
                error = true;
            }
            else if (address1Restriction.Item1 && address1Restriction.Item2 && string.IsNullOrEmpty(informationFragment.AddressOne.Text))
            {
                errorBox = AccountInformationFragment.InputBox.AddressOne;
                error = true;
            }
            else if (address2Restriction.Item1 && address2Restriction.Item2 && string.IsNullOrEmpty(informationFragment.City.Text))
            {
                errorBox = AccountInformationFragment.InputBox.AddressTwo;
                error = true;
            }
            else if (postCodeRestriction.Item1 && postCodeRestriction.Item2 && string.IsNullOrEmpty(informationFragment.PostCode.Text))
            {
                errorBox = AccountInformationFragment.InputBox.PostCode;
                error = true;
            }
            else if (stateRestriction.Item1 && stateRestriction.Item2 && string.IsNullOrEmpty(informationFragment.State.Text))
            {
                errorBox = AccountInformationFragment.InputBox.State;
                error = true;
            }
            else if (countryRestriction.Item1 && countryRestriction.Item2 && string.IsNullOrEmpty(informationFragment.Country.Text))
            {
                errorBox = AccountInformationFragment.InputBox.State;
                error = true;
            }
            else if (phoneRestriction.Item1 && phoneRestriction.Item2 && string.IsNullOrEmpty(informationFragment.Phone.Text))
            {
                errorBox = AccountInformationFragment.InputBox.Phone;
                error = true;
            }
            else if (cityRestriction.Item1 && cityRestriction.Item2 && string.IsNullOrEmpty(informationFragment.City.Text))
            {
                errorBox = AccountInformationFragment.InputBox.City;
                error = true;
            }

            if (error)
            {
                if (!isBigScreen)
                    DisplayInformation();

                informationFragment.FocusView(errorBox);
            }


            return !error;
        }

        private void ShowInformation()
        {
            var ftCurrent = ChildFragmentManager.BeginTransaction();
            ftCurrent.Replace(Resource.Id.AccountScreenContent, informationFragment, InformationFragmentTag);
            ftCurrent.SetTransition(FragmentTransaction.TransitFragmentFade);
            ftCurrent.Commit();
        }

        private void ShowProfiles()
        {
            var ftCurrent = ChildFragmentManager.BeginTransaction();

            if (isBigScreen)
            {
                ftCurrent.Replace(Resource.Id.AccountScreenSecondaryContent, profilesFragment, ProfileFragmentTag);
            }
            else
            {
                ftCurrent.Replace(Resource.Id.AccountScreenContent, profilesFragment, ProfileFragmentTag);
            }
            ftCurrent.SetTransition(FragmentTransaction.TransitFragmentFade);
            ftCurrent.Commit();
        }
    }
}