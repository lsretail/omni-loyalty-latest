using System;
using UIKit;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace Presentation.Screens
{
	public class AccountView : BaseView
	{
		private UIScrollView containerScrollView;
		private UIButton btnLogout;
		private UIButton btnChangePassword;
		private UIButton btnManageAccount;
		private UIImageView qrCodeImageView;
		private UILabel nameLabel;
		private UILabel emailLabel;

		//These public elements are modified by the controller during runtime. Therefore public.
		public UIRefreshControl refreshControl;
		public UILabel pointLabel;
		public UIButton btnClose;

		public AccountView(MemberContact contact, string pointBalance, string qr, IAccountListeners listeners)
		{
			BackgroundColor = UIColor.White;

			btnClose = new UIButton(UIButtonType.Custom);
			btnClose.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.White), UIControlState.Normal);
			btnClose.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			btnClose.Frame = new CGRect(0, 0, 30, 30);
			btnClose.TouchUpInside += (sender, e) =>
			{
				if (listeners != null)
					listeners.Close();
				//DismissViewController(true, null);

			};

			//NavigationItem.LeftBarButtonItem = new UIBarButtonItem(btnClose);

			containerScrollView = new UIScrollView
			{
				AlwaysBounceVertical = true,
				ShowsVerticalScrollIndicator = false
			};
			AddSubview(containerScrollView);

			refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += (object sender, EventArgs e) =>
			{
				if (listeners != null)
				{
					listeners.RefreshControl();
				}

			};

			nameLabel = new UILabel
			{
				Text = contact.FirstName,
				Font = UIFont.BoldSystemFontOfSize(24),
				TextAlignment = UITextAlignment.Left,
				TextColor = Utils.AppColors.PrimaryColor,
				Hidden = string.IsNullOrEmpty(contact.FirstName)
			};

			containerScrollView.AddSubview(nameLabel);

			emailLabel = new UILabel
			{
				Text = contact.Email,
				Font = (nameLabel.Hidden ? UIFont.SystemFontOfSize(18) : UIFont.SystemFontOfSize(14)),
				TextAlignment = UITextAlignment.Left,
				TextColor = Utils.AppColors.PrimaryColor,
			};


			pointLabel = new UILabel
			{
				Text = pointBalance,
				Font = UIFont.SystemFontOfSize(12),
				TextAlignment = UITextAlignment.Left,
				TextColor = UIColor.Gray,
				Hidden = !Utils.Util.AppDelegate.ShowLoyaltyPoints,
			};


			qrCodeImageView = new UIImageView
			{
				ContentMode = UIViewContentMode.ScaleAspectFit,
				Image = Utils.QRCode.QRCode.GenerateQRCode(qr),
				// TODO Re-enable QR code here when the backend gets support for it
				Hidden = true,
			};


			btnChangePassword = new UIButton
			{
				Font = UIFont.SystemFontOfSize(14),
				BackgroundColor = Utils.AppColors.PrimaryColor,
			};

			btnChangePassword.Layer.CornerRadius = 2;
			btnChangePassword.SetTitle(LocalizationUtilities.LocalizedString("Account_ChangePassword", "Change password"), UIControlState.Normal);

			btnChangePassword.TouchUpInside += (object sender, EventArgs e) =>
			{
				if (listeners != null)
				{
					listeners.ChangePassword();
				}
			};

			btnManageAccount = new UIButton
			{
				Font = UIFont.SystemFontOfSize(14),
				BackgroundColor = Utils.AppColors.PrimaryColor,
			};

			btnManageAccount.SetTitle(LocalizationUtilities.LocalizedString("Account_ManageAccount", "Manage account"), UIControlState.Normal);
			btnManageAccount.Layer.CornerRadius = 2;
			btnManageAccount.TouchUpInside += (object sender, EventArgs e) =>
			{
				if (listeners != null)
				{
					listeners.ManageAccount();
				}
				/*ManageAccountScreen manageAccountScreen = new ManageAccountScreen();
				NavigationController.PushViewController(manageAccountScreen, true);*/
			};

			btnLogout = new UIButton();
			btnLogout.SetTitle(LocalizationUtilities.LocalizedString("Account_Logout", "Log out"), UIControlState.Normal);
			btnLogout.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnLogout.Layer.CornerRadius = 2;

			btnLogout.TouchUpInside += (object sender, EventArgs e) =>
			{
				if (listeners != null)
				{
					listeners.Logout();
				}
			};

			containerScrollView.AddSubviews(refreshControl, nameLabel, emailLabel, pointLabel, qrCodeImageView, btnChangePassword, btnManageAccount, btnLogout);
		}

		public override void LayoutSubviews()
		{
			float margin = 20f;
			float buttonHeight = 50f;
			float buttonSpace = 5f;

			base.LayoutSubviews();

			containerScrollView.Frame = Bounds;

			// TODO I moved the UI elements down while the QR code is hidden.
			// Have to move them up again when the QR code is re-enabled.

			//nameLabel.Frame = new RectangleF(0, 30, containerScrollView.Frame.Width, 20f);
			nameLabel.Frame = new CGRect(20f, 40f, containerScrollView.Frame.Width - 20f, 20f);

			if (nameLabel.Hidden)
				emailLabel.Frame = nameLabel.Frame;
			else
				emailLabel.Frame = new CGRect(20f, nameLabel.Frame.Bottom + 5f, containerScrollView.Frame.Width - 20f, 20f);

			pointLabel.Frame = new CGRect(20f, emailLabel.Frame.Bottom, containerScrollView.Frame.Width - 20f, 20f);

			// NOTE: The QR code image has its own margins ("quiet zones") that are necessary for scanners when decoding the code
			qrCodeImageView.Frame = new CGRect(0, pointLabel.Frame.Bottom, containerScrollView.Frame.Width, 260f);


			btnChangePassword.Frame = new CGRect(margin, pointLabel.Frame.Bottom + 40f, (containerScrollView.Frame.Width - 2 * margin) / 2 - buttonSpace / 2, buttonHeight);
			btnManageAccount.Frame = new CGRect(btnChangePassword.Frame.Right + buttonSpace, pointLabel.Frame.Bottom + 40f, (containerScrollView.Frame.Width - 2 * margin) / 2 - buttonSpace / 2, buttonHeight);

			//btnLogout.Frame = new RectangleF(margin, containerScrollView.Frame.Bottom - BottomLayoutGuide.Length - margin - buttonHeight, containerScrollView.Frame.Width - 2 * margin, buttonHeight);
			btnLogout.Frame = new CGRect(margin, btnManageAccount.Frame.Bottom + buttonSpace, containerScrollView.Frame.Width - 2 * margin, buttonHeight);
		}

		public interface IAccountListeners
		{
			void Logout();

			void ChangePassword();

			void ManageAccount();

			void RefreshControl();

			void Close();
		}
	}
}

