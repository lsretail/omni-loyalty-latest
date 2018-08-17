using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using MonoTouch.Dialog;
using Presentation.Models;
using System.Globalization;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base;

namespace Presentation.Screens
{
	/// <summary>
	/// Modal view controller
	/// </summary>
	public partial class LoginDialogController : DialogViewController
	{
		private string passwordPolicy;

		public delegate void LoginSuccessDelegate(Action dismissSelf);
		private LoginSuccessDelegate LoginSuccess;

		public LoginDialogController (LoginSuccessDelegate onLoginSuccess) : base (UITableViewStyle.Grouped, null)
		{
			this.passwordPolicy = string.Empty;
			this.LoginSuccess = onLoginSuccess;
			this.Title = LocalizationUtilities.LocalizedString("Account_Login", "Log in");
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UIButton cancelButton = new UIButton(UIButtonType.Custom);
			cancelButton.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.White), UIControlState.Normal);
			cancelButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			cancelButton.Frame = new CGRect(0, 0, 30, 30);
			cancelButton.TouchUpInside += (sender, e) =>
			{
				this.DismissViewController(true, null);
			};

			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(cancelButton);

			// Hidden button to access the change WS screen
			bool allowHiddenSettings = true;
			if (allowHiddenSettings)
			{
				UIBarButtonItem changeWSButton = new UIBarButtonItem();
				changeWSButton.Title = "URL";
				changeWSButton.Clicked += (object sender, EventArgs e) =>
				{

#if CHANGEWS
					this.PresentViewController(new UINavigationController(new HiddenSettingsController()), true, null);
#endif

				};
				this.NavigationItem.RightBarButtonItem = changeWSButton;
			}

			// Background view
			// "Blur" effect - using a semi-transparent white overlay

			UIImageView blurredBackground = new UIImageView();
			blurredBackground.Frame = new CGRect(0f, 0f, this.View.Frame.Width, this.View.Frame.Height);
			blurredBackground.Image = Utils.Image.FromFile("/Branding/Standard/slideoutmenubackground2.png");
			blurredBackground.ContentMode = UIViewContentMode.BottomLeft;
			blurredBackground.ClipsToBounds = true;

			UIView whiteBackgroundOverlay = new UIView();
			whiteBackgroundOverlay.Frame = new CGRect(0f, 0f, blurredBackground.Frame.Width, blurredBackground.Frame.Height);
			whiteBackgroundOverlay.BackgroundColor = Utils.AppColors.TransparentWhite3;
			blurredBackground.AddSubview(whiteBackgroundOverlay);

			TableView.BackgroundColor = UIColor.Clear;
			TableView.BackgroundView = blurredBackground;

			RegisterKeyboardNotificationHandling();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar (this.NavigationController.NavigationBar);
			BuildLoginTable();
		}

		public void BuildLoginTable()
		{
			// Table elements

			EntryElement emailElement = new EntryElement("", LocalizationUtilities.LocalizedString("Account_Username", "Username"), String.Empty, false);
			emailElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			emailElement.AutocorrectionType = UITextAutocorrectionType.No;

			EntryElement passwordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_Password", "Password"), String.Empty, true);
			passwordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			passwordElement.AutocorrectionType = UITextAutocorrectionType.No;

			// Footerview

			UIView footerView = new UIView();
			footerView.Frame = new CGRect(0, 0, this.TableView.Frame.Width, 200);
			footerView.BackgroundColor = UIColor.Clear;

			UIButton btnOK = new UIButton();
			btnOK.SetTitle(LocalizationUtilities.LocalizedString("Account_Login", "Log in"), UIControlState.Normal);
			btnOK.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnOK.Frame = new CGRect(20, 40, footerView.Frame.Width - 2*20, 50);
			btnOK.Layer.CornerRadius = 2;
			btnOK.TouchUpInside += async (object sender, EventArgs e) => {

				if (emailElement.Value == string.Empty || passwordElement.Value == string.Empty)
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_PleaseFillAllFields", "Please fill all fields"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
					return;
				}

				Login(emailElement.Value, passwordElement.Value);
			};
			footerView.AddSubview(btnOK);

			UIButton btnCreateAccount = new UIButton();
			var titleAttributes = new UIStringAttributes { 
				UnderlineStyle = NSUnderlineStyle.Single,
				ForegroundColor = Utils.AppColors.PrimaryColor,
				UnderlineColor = Utils.AppColors.PrimaryColor
			};
			NSAttributedString createAccountTitleString = new NSAttributedString(
				LocalizationUtilities.LocalizedString("Account_CreateAccount", "Create an account."), titleAttributes);
			btnCreateAccount.SetAttributedTitle(createAccountTitleString, UIControlState.Normal);
			btnCreateAccount.TitleLabel.Font = UIFont.SystemFontOfSize(15);
			btnCreateAccount.BackgroundColor = UIColor.Clear;
			btnCreateAccount.Frame = new CGRect(20, btnOK.Frame.Bottom, btnOK.Frame.Width, 40f);
			btnCreateAccount.TouchUpInside += async (object sender, EventArgs e) => {
			
				Utils.UI.ShowLoadingIndicator();

				string pwPolicy = await new Models.AppSettingsModel().AppSettingsGetByKey(AppSettingsKey.Password_Policy, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
				if (pwPolicy != null) 
				{
					//success
					Utils.UI.HideLoadingIndicator();

					if(!string.IsNullOrEmpty(pwPolicy))
					{
						if(Utils.Util.AppDelegate.DeviceScreenWidth > 320f)
							this.passwordPolicy = LocalizationUtilities.LocalizedString("Account_Password", "Password") + ": " + pwPolicy;
						else
							this.passwordPolicy = LocalizationUtilities.LocalizedString("Account_PasswordShort", "PW") + ": " + pwPolicy;
					}

					BuildCreateAccountTable();
				}
				else
				{
					//failure - don't display the password policy
					Utils.UI.HideLoadingIndicator();
					BuildCreateAccountTable();
				}

			
			};
			footerView.AddSubview(btnCreateAccount);

			UIButton btnForgotPassword = new UIButton();
			var titleAttributes2 = new UIStringAttributes { 
				UnderlineStyle = NSUnderlineStyle.Single,
				ForegroundColor = Utils.AppColors.PrimaryColor,
				UnderlineColor = Utils.AppColors.PrimaryColor
			};
			NSAttributedString forgotPasswordTitleString = new NSAttributedString(
				LocalizationUtilities.LocalizedString("Account_ForgotPassword", "Forgot password?"), titleAttributes2);
			btnForgotPassword.SetAttributedTitle(forgotPasswordTitleString, UIControlState.Normal);
			btnForgotPassword.TitleLabel.Font = UIFont.SystemFontOfSize(15);
			btnForgotPassword.BackgroundColor = UIColor.Clear;
			btnForgotPassword.Frame = new CGRect(20, btnCreateAccount.Frame.Bottom, btnOK.Frame.Width, 40f);
			btnForgotPassword.TouchUpInside += (object sender, EventArgs e) => {

				ForgotPassword();

			};
			footerView.AddSubview(btnForgotPassword);

			// Table itself
		
			Root = new RootElement (LocalizationUtilities.LocalizedString("Account_Login", "Log in"))
			{
				new Section (null, footerView)
				{
					emailElement,
					passwordElement
				}
			};

			this.TableView.ContentInset = new UIEdgeInsets(10, 0, 0, 0);
			this.TableView.ScrollEnabled = false;
		}

		public void BuildCreateAccountTable()
		{
			// Table elements

			EntryElement nameElement = new EntryElement("", LocalizationUtilities.LocalizedString("Account_Name", "Name"), String.Empty, false);

			EntryElement emailElement = new EntryElement("", LocalizationUtilities.LocalizedString("Account_Email", "Email"), String.Empty, false);
			emailElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			emailElement.AutocorrectionType = UITextAutocorrectionType.No;

			EntryElement passwordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_Password", "Password"), String.Empty, true);
			passwordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			passwordElement.AutocorrectionType = UITextAutocorrectionType.No;

			EntryElement retypePasswordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_ConfirmPassword", "Confirm password"), String.Empty, true);
			retypePasswordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			retypePasswordElement.AutocorrectionType = UITextAutocorrectionType.No;

			// Footer view

			UIView footerView = new UIView();
			footerView.Frame = new CGRect(0, 0, this.TableView.Frame.Width, 200);
			footerView.BackgroundColor = UIColor.Clear;

			UILabel lblPasswordPolicy = new UILabel()
			{
				Text = this.passwordPolicy,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize(13),
				TextColor = UIColor.DarkGray,
				Frame = new CGRect(0, 5, this.TableView.Frame.Width, 20)
			};
			footerView.AddSubview(lblPasswordPolicy);

			UIButton btnOK = new UIButton();
			btnOK.SetTitle(LocalizationUtilities.LocalizedString("Account_CreateAccount", "Create account"), UIControlState.Normal);
			btnOK.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnOK.Frame = new CGRect(20, lblPasswordPolicy.Frame.Bottom + 20, footerView.Frame.Width - 2*20, 50);
			btnOK.Layer.CornerRadius = 2;
			btnOK.TouchUpInside += async (object sender, EventArgs e) => {

				if (nameElement.Value == string.Empty || emailElement.Value == string.Empty || passwordElement.Value == string.Empty || retypePasswordElement.Value == string.Empty)
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_PleaseFillAllFields", "Please fill all fields"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
					return;
				}

				if (passwordElement.Value != retypePasswordElement.Value)
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_PasswordMismatch", "Password mismatch"),
						LocalizationUtilities.LocalizedString("Account_ProvidedPasswordsDontMatch", "The provided passwords do not match"),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);

					return;
				}
					
				CreateContact(emailElement.Value, passwordElement.Value, nameElement.Value);
			};
			footerView.AddSubview(btnOK);

			UIButton btnSignIn = new UIButton();
			var titleAttributes = new UIStringAttributes { 
				UnderlineStyle = NSUnderlineStyle.Single,
				ForegroundColor = Utils.AppColors.PrimaryColor,
				UnderlineColor = Utils.AppColors.PrimaryColor
			};
			NSAttributedString titleString = new NSAttributedString(
				LocalizationUtilities.LocalizedString("Account_AlreadyHaveAnAccountLogIn", "Already have an account? Log in."), titleAttributes);
			btnSignIn.SetAttributedTitle(titleString, UIControlState.Normal);
			btnSignIn.TitleLabel.Font = UIFont.SystemFontOfSize(15);
			btnSignIn.BackgroundColor = UIColor.Clear;
			btnSignIn.Frame = new CGRect(20, btnOK.Frame.Bottom, btnOK.Frame.Width, 40f);
			btnSignIn.TouchUpInside += (object sender, EventArgs e) => {

				BuildLoginTable();

			};
			footerView.AddSubview(btnSignIn);

			// Table itself

			Root = new RootElement (LocalizationUtilities.LocalizedString("Account_CreateAccount", "Create account"))
			{
				new Section (null, footerView)
				{
					emailElement,
					passwordElement,
					retypePasswordElement,
					nameElement
				}
			};

			this.TableView.ContentInset = new UIEdgeInsets(10, 0, 0, 0);
			this.TableView.ScrollEnabled = false;
		}

		private async void Login(string email, string password)
		{
			System.Diagnostics.Debug.WriteLine("Logging in user: " + email + ", pass: " + password);
			Utils.UI.ShowLoadingIndicator();
			bool success = await new ContactModel().Login(email, password);
			if (success) 
			{

				Utils.UI.HideLoadingIndicator();
				if (this.LoginSuccess != null)
					this.LoginSuccess(() => { this.DismissViewController(true, null); });
			
			}
			else 
			{

				Utils.UI.HideLoadingIndicator();

			}
		}

		private async void CreateContact(string email, string password, string name)
		{
			System.Diagnostics.Debug.WriteLine("Creating contact: " + email + ", pass: " + password + ", name: " + name);
			Utils.UI.ShowLoadingIndicator();
			bool success = await new ContactModel().ContactCreate(email, password, name);
			if (success)
			{
				Utils.UI.HideLoadingIndicator();
	
				this.DismissViewController(true, null);
			}
			else
			{
				Utils.UI.HideLoadingIndicator();
			}
		}

		private async void ForgotPassword()
		{
			string title = LocalizationUtilities.LocalizedString ("Account_ForgotPassword", "Forgot password?");
			string message = LocalizationUtilities.LocalizedString ("Account_ResetPasswordMsg", "Enter your username and a password reset code will be sent to your email");

			var alertResult = await AlertView.ShowAlertWithTextInput(
				this,
				title,
				message,
				LocalizationUtilities.LocalizedString("Account_Username", "Username"),
				string.Empty,
				LocalizationUtilities.LocalizedString("General_OK", "OK"),
				LocalizationUtilities.LocalizedString("General_Cancel", "Cancel")
			);

			if (alertResult.ButtonResult == AlertView.AlertButtonResult.PositiveButton)
			{
				if(string.IsNullOrEmpty(alertResult.TextInput.Trim()))
				{
					await AlertView.ShowAlert(
						this,
						LocalizationUtilities.LocalizedString("Account_EmptyUsernameError", "Username cannot be empty"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}
				else
				{
					Utils.UI.ShowLoadingIndicator();
					var username = alertResult.TextInput.Trim();

					bool success = await new ContactModel().ForgotPasswordForDevice(username);
					if (success)
					{
						Utils.UI.HideLoadingIndicator();
						ResetPasswordScreen resetPasswordScreen = new ResetPasswordScreen();
						this.PresentViewController(new UINavigationController(resetPasswordScreen), true, null);
					}
					else

					{
						Utils.UI.HideLoadingIndicator();
					}

				}
			}
			else if (alertResult.ButtonResult == AlertView.AlertButtonResult.NegativeButton)
			{
			}
		}
			
		private void RegisterKeyboardNotificationHandling()
		{
			UIKeyboard.Notifications.ObserveDidHide((sender, e) => {
			
				this.TableView.SetContentOffset(new CGPoint(0, 0), true);

			});
		}
	}
}
