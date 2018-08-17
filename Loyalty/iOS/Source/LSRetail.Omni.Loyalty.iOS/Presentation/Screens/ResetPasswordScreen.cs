using System;
using UIKit;
using MonoTouch.Dialog;
using CoreGraphics;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public partial class ResetPasswordScreen : DialogViewController
	{
		private EntryElement usernameElement;
		private EntryElement resetCodeElement;
		private EntryElement newPasswordElement;
		private EntryElement retypeNewPasswordElement;

		public ResetPasswordScreen () : base (UITableViewStyle.Grouped, null)
		{
			this.Title = LocalizationUtilities.LocalizedString("Account_ResetPassword", "Reset password");
		}

		public void BuildResetPasswordTable()
		{
			this.usernameElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_Username", "Username"), string.Empty, false);
			usernameElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			usernameElement.AutocorrectionType = UITextAutocorrectionType.No;

			this.resetCodeElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_ResetCode", "Reset code"), string.Empty, false);
			resetCodeElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			resetCodeElement.AutocorrectionType = UITextAutocorrectionType.No;

			this.newPasswordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_NewPassword", "New password"), string.Empty, true);
			newPasswordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			newPasswordElement.AutocorrectionType = UITextAutocorrectionType.No;

			this.retypeNewPasswordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_ConfirmNewPassword", "Confirm new password"), string.Empty, true);
			retypeNewPasswordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			retypeNewPasswordElement.AutocorrectionType = UITextAutocorrectionType.No;

			// Headerview

			UIView headerView = new UIView ();
			headerView.Frame = new CGRect (0f, 0f, this.TableView.Frame.Width, 70f);
			headerView.BackgroundColor = UIColor.Clear;

			UITextView instructionView = new UITextView();
			instructionView.Frame = new CGRect (10f, 10f, this.TableView.Frame.Width - 20f, 60f);
			instructionView.Text = LocalizationUtilities.LocalizedString("Account_ResetPasswordInstructions", "Reset code was sent to your email, use that to finish resetting your password");
			instructionView.UserInteractionEnabled = false;
			instructionView.BackgroundColor = UIColor.Clear;
			instructionView.TextColor = Utils.AppColors.PrimaryColor;
			instructionView.TextAlignment = UITextAlignment.Center;
			instructionView.Font = UIFont.SystemFontOfSize(16f);
			headerView.AddSubview(instructionView);

			// Footerview

			UIView footerView = new UIView();
			footerView.Frame = new CGRect(0, 0, this.TableView.Frame.Width, 100);
			footerView.BackgroundColor = UIColor.Clear;

			UIButton btnChange = new UIButton();
			btnChange.SetTitle(LocalizationUtilities.LocalizedString("Account_Change", "Change"), UIControlState.Normal);
			btnChange.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnChange.Frame = new CGRect(20, 40, footerView.Frame.Width - 2*20, 50);
			btnChange.Layer.CornerRadius = 2;
			btnChange.TouchUpInside += async (object sender, EventArgs e) => {

				if (usernameElement.Value == string.Empty || resetCodeElement.Value == string.Empty || newPasswordElement.Value == string.Empty || retypeNewPasswordElement.Value == string.Empty)
				{
					await AlertView.ShowAlert(
						this,
						LocalizationUtilities.LocalizedString("Account_PleaseFillAllFields", "Please fill all fields"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
					return;
				}

				if(newPasswordElement.Value != retypeNewPasswordElement.Value)
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_PasswordMismatch", "Password mismatch"),
						LocalizationUtilities.LocalizedString("Account_ProvidedPasswordsDontMatch", "The provided passwords do not match"),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
					return;
				}

				ResetPassword(usernameElement.Value, resetCodeElement.Value, newPasswordElement.Value);
			};
			footerView.AddSubview(btnChange);

			// Table itself

			Root = new RootElement (LocalizationUtilities.LocalizedString("Account_ChangePassword", "Change password"))
			{
				new Section (headerView, footerView)
				{
					usernameElement,
					resetCodeElement,
					newPasswordElement,
					retypeNewPasswordElement
				}
			};

			this.TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
			this.TableView.ScrollEnabled = true;
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = UIColor.White;

			UIBarButtonItem cancelButton = new UIBarButtonItem ();
			cancelButton.Title = LocalizationUtilities.LocalizedString("General_Cancel", "Cancel");
			cancelButton.Clicked += (object sender, EventArgs e) => {

				this.DismissViewController(true, null);

			};
			this.NavigationItem.LeftBarButtonItem = cancelButton;

			// Background view
			// "Blur" effect - using a semi-transparent white overlay


			UIImageView blurredBackground = new UIImageView ();
			blurredBackground.Frame = new CGRect(0f, 0f, this.View.Frame.Width, this.View.Frame.Height);
			blurredBackground.Image = ImageUtilities.FromFile ("/Branding/Standard/StoreBackground.png");
			blurredBackground.ContentMode = UIViewContentMode.BottomLeft;
			blurredBackground.ClipsToBounds = true;

			UIView whiteBackgroundOverlay = new UIView ();
			whiteBackgroundOverlay.Frame = new CGRect (0f, 0f, blurredBackground.Frame.Width, blurredBackground.Frame.Height);
			whiteBackgroundOverlay.BackgroundColor = Utils.AppColors.TransparentWhite3;
			blurredBackground.AddSubview (whiteBackgroundOverlay);

			TableView.BackgroundColor = UIColor.Clear;
			TableView.BackgroundView = blurredBackground;

			BuildResetPasswordTable ();

			RegisterKeyboardNotificationHandling();
		}

		private async void ResetPassword(string username, string resetCode, string newPassword)
		{
			Utils.UI.ShowLoadingIndicator();

            bool success = await new ContactModel().ResetPassword(username, resetCode, newPassword);
            if (success) 
            {
				Utils.UI.HideLoadingIndicator ();
				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("Account_PasswordSuccess", "Password successfully changed"),
					string.Empty,
					LocalizationUtilities.LocalizedString("General_OK", "OK")
				);
				this.DismissViewController(true, null);
			}
            else
            {
				Utils.UI.HideLoadingIndicator ();
			}
			
		}

		private void RegisterKeyboardNotificationHandling()
		{
			UIKeyboard.Notifications.ObserveWillShow((sender, e) => {

				//var keyboard = UIKeyboard.FrameBeginFromNotification(e.Notification);
				//var keyboardHeight = keyboard.Height;

				var contentInsets = new UIEdgeInsets(64f, 0, 0, 0);
				this.TableView.ContentInset = contentInsets;
				this.TableView.ScrollIndicatorInsets = contentInsets;
			});

			UIKeyboard.Notifications.ObserveDidHide((sender, e) => {

				var contentInsets = new UIEdgeInsets(64f, 0, 0, 0);
				this.TableView.ContentInset = contentInsets;
				this.TableView.ScrollIndicatorInsets = contentInsets;

			});
		}
	}
}

