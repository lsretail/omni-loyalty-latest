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
	public partial class ResetPasswordScreen : DialogViewController
	{
		private EntryElement usernameElement;
		private EntryElement resetCodeElement;
		private EntryElement newPasswordElement;
		private EntryElement retypeNewPasswordElement;

		public ResetPasswordScreen () : base (UITableViewStyle.Grouped, null)
		{
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
			headerView.Frame = new CGRect (0f, 0f, this.TableView.Frame.Width, 90f);
			headerView.BackgroundColor = UIColor.Clear;

			UITextView instructionView = new UITextView();
			instructionView.Frame = new CGRect (10f, 10f, this.TableView.Frame.Width - 20f, 80f);
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


		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes(false);
			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = UIColor.White;
			this.Title = LocalizationUtilities.LocalizedString("Account_ResetPassword", "Reset password");

			UIButton btnCancel = new UIButton (UIButtonType.Custom);
			btnCancel.SetImage (Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.White), UIControlState.Normal);
			btnCancel.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			btnCancel.Frame = new CGRect (0, 0, 30, 30);
			btnCancel.TouchUpInside += (sender, e) => 
			{
				this.DismissViewController(true, null);
			};

			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (btnCancel);

			// Background view
			// "Blur" effect - using a semi-transparent white overlay


			UIImageView blurredBackground = new UIImageView ();
			blurredBackground.Frame = new CGRect(0f, 0f, this.View.Frame.Width, this.View.Frame.Height);
			blurredBackground.Image = Utils.Image.FromFile ("/Branding/Standard/slideoutmenubackground2.png");
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
			// Showing the keyboard tended to push the input field off the screen.
			// Probably because we have a footerview and the keyboard is making sure it stays visible above the keyboard, thereby pushing the input fields too high.
			// Fix this by manually scrolling the activated input field into view after the keyboard has been shown.


			UIKeyboard.Notifications.ObserveDidShow((sender, e) => {

				try
				{
					//UIView activeView = Utils.Util.FindFirstResponder(this.View);
					// The first responder should be a UITextField ...
					// which is a subview of a UITableViewCellContentView, which is a subview of a UITableViewCellScrollView which is a subview of a UITableViewCell
					// which is the view (frame) that we actually want

					//this.TableView.ScrollRectToVisible(activeView.Superview.Superview.Superview.Frame, true);

					//this.TableView.ScrollRectToVisible(new CGRect(0, -200f, this.View.Frame.Width, this.View.Frame.Height), true); //TODO : Find better way to control the keyboard/view
				}
				catch (Exception ex)
				{
					// The above code is dangerous ... sometimes we can't find the active view and all those superview references are rather shaky
					// HACK Log and eat the exception
					System.Diagnostics.Debug.WriteLine(ex.ToString());
				}

			});


			// The tableview tended to end up in a weird location (way too low) after the keyboard is hidden.
			// Fix this by scrolling it to the desired location manually after the keyboard is hidden.
			UIKeyboard.Notifications.ObserveDidHide((sender, e) => {

				this.TableView.SetContentOffset(new CGPoint(0, 0), true);

			});

		}
	}
}

