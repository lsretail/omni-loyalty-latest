using System;
using UIKit;
using CoreGraphics;
using Presentation.Screens;
using Presentation.Utils;
using Presentation.Models;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Profiles;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;

namespace Presentation
{
    public class AccountController : UIViewController
    {
        private AccountView rootView;

        public delegate void LogoutSuccessDelegate(Action<bool> dismissSelf);
        private LogoutSuccessDelegate logoutSuccess;

        private IProfileRepository profileRepository;
        private ProfileService profileService;

        public AccountController(LogoutSuccessDelegate onLogoutSuccess)
        {
            logoutSuccess = onLogoutSuccess;
            this.Title = LocalizationUtilities.LocalizedString("Account_Account", "Account");
            profileRepository = new ProfileRepository();
            profileService = new ProfileService(profileRepository);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

            this.rootView = new AccountView(
                MemberContactAttributes.Registration.Username ? AppData.Device.UserLoggedOnToDevice.UserName : AppData.Device.UserLoggedOnToDevice.Name,
                AppData.Device.UserLoggedOnToDevice.Email,
                GetPointBalanceString(),
                GetMemberSchemeString(),
                GetNextSchemeString()
            );
            this.rootView.RefreshAccount += RefreshAccount;
            this.rootView.ChangePassword += ChangePassword;
            this.rootView.ManageAccount += ManageAccount;
            this.rootView.LogOut += Logout;
            this.View = this.rootView;

            UIButton generateQRCodeButton = new UIButton();
            generateQRCodeButton.Frame = new CGRect(0, 0, 30, 30);
            generateQRCodeButton.SetImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("iconQRCodeWhite.png"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
            generateQRCodeButton.TouchUpInside += (sender, e) =>
            {
                UINavigationController QRCodeController = new UINavigationController(new QRCodeController(GenerateQRCodeXML(), QRCodeViewType.Account));
                this.PresentViewController(QRCodeController, true, () => { });
            };
            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(generateQRCodeButton);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
            this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.rootView.Refresh(
                MemberContactAttributes.Registration.Username ? AppData.Device.UserLoggedOnToDevice.UserName : AppData.Device.UserLoggedOnToDevice.Name,
                AppData.Device.UserLoggedOnToDevice.Email,
                GetPointBalanceString(),
                GetMemberSchemeString(),
                GetNextSchemeString()
            );
        }

        private void ChangePassword()
        {
            ChangePasswordScreen changePasswordScreen = new ChangePasswordScreen();
            this.NavigationController.PushViewController(changePasswordScreen, true);
        }

        private async void ManageAccount()
        {
            if (MemberContactAttributes.Manage.Profiles)
            {
                List<Profile> allProfiles = await new Models.ProfileModel().GetAllProfiles();
                List<Profile> selectedProfiles = await profileService.ProfilesGetByContactIdAsync(AppData.Device.UserLoggedOnToDevice.Id);

                if (allProfiles != null)
                {
                    Utils.UI.HideLoadingIndicator();

                    //Assing correct values to the profiles before being displayed
                    foreach (var profile in allProfiles)
                    {
                        foreach (var selectedProfile in selectedProfiles)
                        {
                            if (profile.Id == selectedProfile.Id)
                            {
                                profile.ContactValue = true;
                                break;
                            }
                            profile.ContactValue = false;
                        }
                    }

                    this.NavigationController.PushViewController(new ManageAccountController(allProfiles), true);
                }
                else
                {
                    Utils.UI.HideLoadingIndicator();
                    this.NavigationController.PushViewController(new ManageAccountController(new List<Profile>()), true);
                }
            }
            else
            {
                Utils.UI.HideLoadingIndicator();
                this.NavigationController.PushViewController(new ManageAccountController(new List<Profile>()), true);
            }
        }

        private async void Logout()
        {
            Utils.UI.ShowLoadingIndicator();

            bool success = await new ContactModel().MemberContactLogout(AppData.Device.UserLoggedOnToDevice.UserName, AppData.Device.UserLoggedOnToDevice.Id);
            if (success)
            {
                // Logout success
                Utils.UI.HideLoadingIndicator();

                if (logoutSuccess != null)
                    logoutSuccess((animated) => { this.NavigationController.PopViewController(animated); });
            }
            else
            {
                // Logout failure
                Utils.UI.HideLoadingIndicator();

                // TODO Should we also be sending the user to the login screen here, even if the logout operation failed?
                if (logoutSuccess != null)
                    logoutSuccess((animated) => { this.NavigationController.PopViewController(animated); });
            }
        }

        private async void RefreshAccount()
        {
            System.Diagnostics.Debug.WriteLine("Refreshing account ...");

            bool success = await new Models.ContactModel().MemberContactUpdatePointBalance();
            if (success)
            {
                AppData.ShouldRefreshPoints = false;
                this.rootView.Refresh(
                    MemberContactAttributes.Registration.Username ? AppData.Device.UserLoggedOnToDevice.UserName : AppData.Device.UserLoggedOnToDevice.Name,
                    AppData.Device.UserLoggedOnToDevice.Email,
                    GetPointBalanceString(),
                    GetMemberSchemeString(),
                    GetNextSchemeString()
                );
            }
        }

        private string GetMemberSchemeString()
        {
            return AppData.Device.UserLoggedOnToDevice.Account.Scheme.Description + " " + LocalizationUtilities.LocalizedString("Account_Member", "member");
        }

        private string GetPointBalanceString()
        {
            return AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0") + " " + LocalizationUtilities.LocalizedString("Account_Points_Lowercase", "points");
        }

        private string GetNextSchemeString()
        {
            string result = LocalizationUtilities.LocalizedString("Account_NextSchemeLevel", "Next level at {0}.\nYou need another {1} points\nto become a {2} member.");
            decimal points = AppData.Device.UserLoggedOnToDevice.Account.Scheme.PointsNeeded - AppData.Device.UserLoggedOnToDevice.Account.PointBalance;
            if (points < 0)
                points = 0;

            return string.Format(result, AppData.Device.UserLoggedOnToDevice.Account.Scheme.PointsNeeded.ToString("N0"),
                                 points.ToString("N0"), AppData.Device.UserLoggedOnToDevice.Account.Scheme.NextScheme.Description);
        }

        private string GenerateQRCodeXML()
        {
            string xml = string.Format("<mobiledevice><contactid>{0}</contactid><accountid>{1}</accountid><cardid>{2}</cardid>",
                AppData.Device.UserLoggedOnToDevice.Id,
                AppData.Device.UserLoggedOnToDevice.Account.Id,
                AppData.Device.UserLoggedOnToDevice.Card.Id);

            xml += "<coupons>";
            xml += "</coupons>";

            xml += "<offers>";
            xml += "</offers>";

            xml += "</mobiledevice>";

            return xml;
        }
    }
}

