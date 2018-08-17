using System;
using UIKit;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class AccountView : BaseView
    {
        private UIScrollView containerScrollView;
        private UIRefreshControl refreshControl;
        private UIButton btnLogout;
        private UIButton btnChangePassword;
        private UIButton btnManageAccount;
        private UILabel nameLabel;
        private UILabel emailLabel;
        private UILabel memberSchemeLabel;
        private UILabel pointLabel;
        private UILabel nextSchemeLabel;

        public delegate void RefreshAccountEventHandler();
        public event RefreshAccountEventHandler RefreshAccount;

        public delegate void ChangePasswordEventHandler();
        public event ChangePasswordEventHandler ChangePassword;

        public delegate void ManageAccountEventHandler();
        public event ManageAccountEventHandler ManageAccount;

        public delegate void LogoutEventHandler();
        public event LogoutEventHandler LogOut;

        public AccountView(string name, string email, string pointBalance, string memberScheme, string nextScheme)
        {
            this.BackgroundColor = Utils.AppColors.BackgroundGray;

            this.containerScrollView = new UIScrollView();
            this.containerScrollView.AlwaysBounceVertical = true;
            this.containerScrollView.ShowsVerticalScrollIndicator = false;
            this.AddSubview(this.containerScrollView);

            this.refreshControl = new UIRefreshControl();
            this.refreshControl.ValueChanged += (object sender, EventArgs e) =>
            {

                if (this.RefreshAccount != null)
                {
                    this.RefreshAccount();
                }
            };
            this.containerScrollView.AddSubview(refreshControl);

            this.nameLabel = new UILabel();
            this.nameLabel.Text = name;
            if (string.IsNullOrEmpty(this.nameLabel.Text))
                this.nameLabel.Hidden = true;
            nameLabel.Font = UIFont.BoldSystemFontOfSize(24);
            nameLabel.TextAlignment = UITextAlignment.Left;
            nameLabel.TextColor = Utils.AppColors.PrimaryColor;
            this.containerScrollView.AddSubview(nameLabel);

            this.emailLabel = new UILabel();
            this.emailLabel.Text = email;
            this.emailLabel.Font = (this.nameLabel.Hidden ? UIFont.SystemFontOfSize(18) : UIFont.SystemFontOfSize(14));
            emailLabel.Font = UIFont.SystemFontOfSize(14);
            emailLabel.TextAlignment = UITextAlignment.Left;
            emailLabel.TextColor = Utils.AppColors.PrimaryColor;
            this.containerScrollView.AddSubview(emailLabel);

            this.memberSchemeLabel = new UILabel();
            this.memberSchemeLabel.Text = memberScheme;
            memberSchemeLabel.Font = UIFont.SystemFontOfSize(12);
            memberSchemeLabel.TextAlignment = UITextAlignment.Left;
            memberSchemeLabel.TextColor = UIColor.Gray;
            if (!Utils.Util.AppDelegate.ShowLoyaltyPoints)
                memberSchemeLabel.Hidden = true;
            this.containerScrollView.AddSubview(memberSchemeLabel);

            this.pointLabel = new UILabel();
            this.pointLabel.Text = pointBalance;
            pointLabel.Font = UIFont.SystemFontOfSize(12);
            pointLabel.TextAlignment = UITextAlignment.Left;
            pointLabel.TextColor = UIColor.Gray;
            if (!Utils.Util.AppDelegate.ShowLoyaltyPoints)
                pointLabel.Hidden = true;
            this.containerScrollView.AddSubview(pointLabel);

            this.nextSchemeLabel = new UILabel();
            this.nextSchemeLabel.Text = nextScheme;
            nextSchemeLabel.Font = UIFont.SystemFontOfSize(12);
            nextSchemeLabel.TextAlignment = UITextAlignment.Left;
            nextSchemeLabel.TextColor = UIColor.Gray;
            nextSchemeLabel.Lines = 3;
            if (!Utils.Util.AppDelegate.ShowLoyaltyPoints)
                nextSchemeLabel.Hidden = true;
            this.containerScrollView.AddSubview(nextSchemeLabel);

            this.btnChangePassword = new UIButton();
            btnChangePassword.SetTitle(LocalizationUtilities.LocalizedString("Account_ChangePassword", "Change password"), UIControlState.Normal);
            btnChangePassword.Font = UIFont.SystemFontOfSize(14);
            btnChangePassword.BackgroundColor = Utils.AppColors.PrimaryColor;
            btnChangePassword.Layer.CornerRadius = 2;
            btnChangePassword.TouchUpInside += (object sender, EventArgs e) =>
            {

                if (this.ChangePassword != null)
                {
                    this.ChangePassword();
                }
            };
            this.containerScrollView.AddSubview(btnChangePassword);

            this.btnManageAccount = new UIButton();
            btnManageAccount.SetTitle(LocalizationUtilities.LocalizedString("Account_ManageAccount", "Manage account"), UIControlState.Normal);
            btnManageAccount.Font = UIFont.SystemFontOfSize(14);
            btnManageAccount.BackgroundColor = Utils.AppColors.PrimaryColor;
            btnManageAccount.Layer.CornerRadius = 2;
            btnManageAccount.TouchUpInside += (object sender, EventArgs e) =>
            {

                if (this.ManageAccount != null)
                {
                    this.ManageAccount();
                }
            };
            this.containerScrollView.AddSubview(btnManageAccount);

            this.btnLogout = new UIButton();
            btnLogout.SetTitle(LocalizationUtilities.LocalizedString("Account_Logout", "Log out"), UIControlState.Normal);
            btnLogout.BackgroundColor = Utils.AppColors.PrimaryColor;
            btnLogout.Layer.CornerRadius = 2;
            btnLogout.TouchUpInside += async (object sender, EventArgs e) =>
            {
                var alertResult = await AlertView.ShowAlert(
                    null,
                    LocalizationUtilities.LocalizedString("Account_Logout", "Log out"),
                    LocalizationUtilities.LocalizedString("Account_AreYouSureLogout", "Are you sure you want to log out?"),
                    LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
                    LocalizationUtilities.LocalizedString("General_No", "No")
                );

                if (alertResult == AlertView.AlertButtonResult.PositiveButton)
                {
                    if (this.LogOut != null)
                    {
                        this.LogOut();
                    }
                }
            };
            this.containerScrollView.AddSubview(btnLogout);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.containerScrollView.Frame = this.Bounds;

            // TODO I moved the UI elements down while the QR code is hidden.
            // Have to move them up again when the QR code is re-enabled.

            //this.nameLabel.Frame = new RectangleF(0, 30, this.containerScrollView.Frame.Width, 20f);
            this.nameLabel.Frame = new CGRect(20f, 40f, this.containerScrollView.Frame.Width - 20f, 28f);

            if (this.nameLabel.Hidden)
                this.emailLabel.Frame = this.nameLabel.Frame;
            else
                this.emailLabel.Frame = new CGRect(20f, this.nameLabel.Frame.Bottom + 5f, this.containerScrollView.Frame.Width - 20f, 20f);

            this.memberSchemeLabel.Frame = new CGRect(20f, this.emailLabel.Frame.Bottom, this.containerScrollView.Frame.Width - 20f, 20f);
            this.pointLabel.Frame = new CGRect(20f, this.memberSchemeLabel.Frame.Bottom, this.containerScrollView.Frame.Width - 20f, 20f);
            this.nextSchemeLabel.Frame = new CGRect(20f, this.pointLabel.Frame.Bottom, this.containerScrollView.Frame.Width - 20f, 60f);

            float buttonHeight = 44f;
            float buttonMargin = 20f;
            float buttonSpace = 5f;

            this.btnChangePassword.Frame = new CGRect(buttonMargin, this.nextSchemeLabel.Frame.Bottom + 40f, (this.containerScrollView.Frame.Width - 2 * buttonMargin) / 2 - buttonSpace / 2, buttonHeight);
            this.btnManageAccount.Frame = new CGRect(this.btnChangePassword.Frame.Right + buttonSpace, this.nextSchemeLabel.Frame.Bottom + 40f, (this.containerScrollView.Frame.Width - 2 * buttonMargin) / 2 - buttonSpace / 2, buttonHeight);

            //this.btnLogout.Frame = new RectangleF(buttonMargin, this.containerScrollView.Frame.Bottom - this.BottomLayoutGuide.Length - buttonMargin - buttonHeight, this.containerScrollView.Frame.Width - 2 * buttonMargin, buttonHeight);
            this.btnLogout.Frame = new CGRect(buttonMargin, this.btnManageAccount.Frame.Bottom + buttonSpace, this.containerScrollView.Frame.Width - 2 * buttonMargin, buttonHeight);
        }

        public void Refresh(string name, string email, string pointBalance, string memberScheme, string nextScheme)
        {
            this.nameLabel.Text = name;
            if (string.IsNullOrEmpty(this.nameLabel.Text))
                this.nameLabel.Hidden = true;

            this.emailLabel.Text = email;
            this.emailLabel.Font = (this.nameLabel.Hidden ? UIFont.SystemFontOfSize(18) : UIFont.SystemFontOfSize(14));

            this.pointLabel.Text = pointBalance;
            this.memberSchemeLabel.Text = memberScheme;
            this.nextSchemeLabel.Text = nextScheme;

            this.SetNeedsLayout();

            this.refreshControl.EndRefreshing();
        }
    }
}
