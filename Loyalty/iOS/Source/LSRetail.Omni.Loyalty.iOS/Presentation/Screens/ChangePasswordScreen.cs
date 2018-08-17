using System;
using UIKit;
using MonoTouch.Dialog;
using CoreGraphics;
using Foundation;
using Presentation.Models;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public partial class ChangePasswordScreen : DialogViewController
	{
		private EntryElement oldPasswordElement;
		private EntryElement newPasswordElement;
		private EntryElement retypeNewPasswordElement;

		public ChangePasswordScreen () : base (UITableViewStyle.Grouped, null, true)
		{
		}

		public void BuildChangePasswordTable()
		{
			this.oldPasswordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_OldPassword", "Old password"), string.Empty, true);
			oldPasswordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			oldPasswordElement.AutocorrectionType = UITextAutocorrectionType.No;

			this.newPasswordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_NewPassword", "New password"), string.Empty, true);
			newPasswordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			newPasswordElement.AutocorrectionType = UITextAutocorrectionType.No;

			this.retypeNewPasswordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_ConfirmNewPassword", "Confirm new password"), string.Empty, true);
			retypeNewPasswordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			retypeNewPasswordElement.AutocorrectionType = UITextAutocorrectionType.No;

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

				if (oldPasswordElement.Value == string.Empty || newPasswordElement.Value == string.Empty || retypeNewPasswordElement.Value == string.Empty)
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

				ChangePassword(oldPasswordElement.Value, newPasswordElement.Value);
			};
			footerView.AddSubview(btnChange);

			// Table itself

			Root = new RootElement (LocalizationUtilities.LocalizedString("Account_ChangePassword", "Change password"))
			{
				new Section (null, footerView)
				{
					oldPasswordElement,
					newPasswordElement,
					retypeNewPasswordElement
				}
			};

			this.TableView.ContentInset = new UIEdgeInsets(0f, 0, 0, 0);
			this.TableView.ScrollEnabled = true;
		}


		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

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

			BuildChangePasswordTable ();

			RegisterKeyboardNotificationHandling();
		}

		private async void ChangePassword(string oldPassword, string newPassword)
		{
			Utils.UI.ShowLoadingIndicator();
                bool success = await new ContactModel().ChangePassword(AppData.Device.UserLoggedOnToDevice.UserName, oldPassword, newPassword);
                if (success)
                {
                    Utils.UI.HideLoadingIndicator();
                    await AlertView.ShowAlert(
                        this,
                        LocalizationUtilities.LocalizedString("Account_PasswordSuccess", "Password successfully changed"),
                        string.Empty,
                        LocalizationUtilities.LocalizedString("General_OK", "OK")
                    );
                    this.NavigationController.PopViewController(true);
                }
                else
                {
                    Utils.UI.HideLoadingIndicator();
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

