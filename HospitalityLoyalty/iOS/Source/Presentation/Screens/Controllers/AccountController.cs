using System;
using System.Linq;
using UIKit;
using Foundation;
using Presentation.Utils;
using Presentation.Screens;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;

namespace Presentation.Screens
{
	public class AccountController : UIViewController, AccountView.IAccountListeners
	{
		private AccountView rootView;
		private MemberContact contact;


		public delegate void LogoutSuccessDelegate(Action<bool> dismissSelf);
		private LogoutSuccessDelegate LogoutSuccess;

		public AccountController(MemberContact contact, LogoutSuccessDelegate onLogoutSuccess)
		{
			this.Title = LocalizationUtilities.LocalizedString("Account_Account", "Account");
			this.contact = contact;
			this.rootView = new AccountView(contact, GetPointBalanceString(), GenerateContactQRCodeXML(), this);

			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(this.rootView.btnClose);

			this.LogoutSuccess += onLogoutSuccess;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes(false);
			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;
			this.View = rootView;
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		private string GetPointBalanceString()
		{
			return AppData.Contact.Account.PointBalance.ToString("N0") + " " + LocalizationUtilities.LocalizedString("Account_Points_Lowercase", "points");
		}

		private string GenerateContactQRCodeXML()
		{
			BasketQrCode qrModel = new BasketQrCode(AppData.MobileMenu);

			qrModel.Contact = this.contact;

			return qrModel.Serialize();
		}

		public async void RefreshControl()
		{
			System.Diagnostics.Debug.WriteLine("Refreshing point balance ...");

			bool success = await new Models.ContactModel().ContactUpdatePointBalance();
			if (success) 
			{

				AppData.ShouldRefreshPoints = false;
				this.rootView.pointLabel.Text = GetPointBalanceString();
				this.rootView.refreshControl.EndRefreshing();

			}
			else
			{

				this.rootView.refreshControl.EndRefreshing();

			}
		}

		public async void Logout()
		{
			var alertResult = await AlertView.ShowAlert(
				this,
				LocalizationUtilities.LocalizedString("Account_Logout", "Log out"),
				LocalizationUtilities.LocalizedString("Account_AreYouSureLogout", "Are you sure you want to log out?"),
				LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
				LocalizationUtilities.LocalizedString("General_No", "No")
			);

			if (alertResult == AlertView.AlertButtonResult.PositiveButton)
			{
				bool success = await new ContactModel().Logout();
				if (success) 
				{
					// Logout success
					Utils.UI.HideLoadingIndicator();

					if (this.LogoutSuccess != null)
						this.LogoutSuccess((animated) => { this.NavigationController.PopViewController(animated); });
				}
				else
				{
					// Logout failure
					Utils.UI.HideLoadingIndicator();

					// TODO Should we also be sending the user to the login screen here, even if the logout operation failed?
					if (this.LogoutSuccess != null)
					this.LogoutSuccess((animated) => { this.NavigationController.PopViewController(animated); });
				}

			}
			else if (alertResult == AlertView.AlertButtonResult.NegativeButton)
			{
			}
		}
		public void ChangePassword()
		{
			ChangePasswordDialogController changePasswordDialogController = new ChangePasswordDialogController();
			this.NavigationController.PushViewController(changePasswordDialogController, true);
		}

		public void ManageAccount()
		{
			ManageAccountDialogController manageAccountDialogController = new ManageAccountDialogController();
			this.NavigationController.PushViewController(manageAccountDialogController, true);
		}

		public void Close()
		{
			this.NavigationController.PopViewController(true);
			//this.DismissViewController(true, null);
		}
	}
}

