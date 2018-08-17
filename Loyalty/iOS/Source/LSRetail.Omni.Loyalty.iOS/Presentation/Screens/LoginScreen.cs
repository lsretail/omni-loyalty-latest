using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;
using MonoTouch.Dialog;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using Firebase.InstanceID;

namespace Presentation.Screens
{
    public class LoginScreen : DialogViewController
	{
		private EntryElement loginEmailElement;
		private EntryElement loginPasswordElement;

		private int changeWSButtonPressCounter;
		private string passwordPolicy;

		public delegate void LoginSuccessDelegate(Action dismissSelf);
		private LoginSuccessDelegate LoginSuccess;

		public LoginScreen (LoginSuccessDelegate onLoginSuccess) : base (UITableViewStyle.Grouped, null)
		{
			this.Title = LocalizationUtilities.LocalizedString("Account_Login", "Log in");

			this.passwordPolicy = string.Empty;
			this.LoginSuccess = onLoginSuccess;
		}

		public void BuildLoginTable()
		{
			// Table elements

			this.loginEmailElement = new EntryElement("", LocalizationUtilities.LocalizedString("Account_Username", "Username"), String.Empty, false);
			loginEmailElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			loginEmailElement.AutocorrectionType = UITextAutocorrectionType.No;

			this.loginPasswordElement = new EntryElement ("", LocalizationUtilities.LocalizedString("Account_Password", "Password"), String.Empty, true);
			loginPasswordElement.AutocapitalizationType = UITextAutocapitalizationType.None;
			loginPasswordElement.AutocorrectionType = UITextAutocorrectionType.No;

			// HeaderView
			UIView headerView = new UIView ();
			nfloat yPos = 0;

			headerView.Frame = new CGRect (0, 0, this.TableView.Frame.Width, 180);

			nfloat imageViewHeight = 140;
			yPos = 10;

			UIImageView imageView = new UIImageView();
			imageView.Frame = new CGRect (10, yPos, this.View.Frame.Width - 20, imageViewHeight);
			imageView.Image = ImageUtilities.FromFile ("/Branding/Standard/StoreBannerTransparent.png");
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			yPos = headerView.Frame.Bottom - 38;
			headerView.AddSubview(imageView);
				
			// Footerview
			UIView footerView = new UIView();
			footerView.Frame = new CGRect(0, 0, this.TableView.Frame.Width, 100);
			footerView.BackgroundColor = UIColor.Clear;

			UIButton btnOK = new UIButton();
			btnOK.SetTitle(LocalizationUtilities.LocalizedString("Account_Login", "Log in"), UIControlState.Normal);
			btnOK.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnOK.Frame = new CGRect(20, 40, footerView.Frame.Width - 2 * 20, 50);
			btnOK.Layer.CornerRadius = 2;
			btnOK.TouchUpInside += async (object sender, EventArgs e) => {

				/*#if DEBUG
				Login ("tom", "tom.1");
				return;
				#endif*/

				if (loginEmailElement.Value == string.Empty || loginPasswordElement.Value == string.Empty)
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_PleaseFillAllFields", "Please fill all fields"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
					return;
				}

				Login(loginEmailElement, loginPasswordElement);
			};
			footerView.AddSubview(btnOK);

			UIButton btnForgotPassword = new UIButton();
			btnForgotPassword.Frame = new CGRect(20, yPos, btnOK.Frame.Width/2, 28f);
			btnForgotPassword.TitleLabel.Font = UIFont.SystemFontOfSize(13);
			btnForgotPassword.BackgroundColor = UIColor.Clear;

			UILabel forgotPasswordTitle = new UILabel (btnForgotPassword.Bounds);
			forgotPasswordTitle.Text = LocalizationUtilities.LocalizedString ("Account_ForgotPassword", "Forgot password");
			forgotPasswordTitle.TextColor = Utils.AppColors.PrimaryColor;
			btnForgotPassword.AddSubview (forgotPasswordTitle);
			btnForgotPassword.TouchUpInside += (object sender, EventArgs e) => 
			{
				ForgotPassword();
			};
			headerView.AddSubview(btnForgotPassword);

			UIButton btnCreateAccount = new UIButton();
			btnCreateAccount.Frame = new CGRect(btnForgotPassword.Frame.Right, yPos, btnOK.Frame.Width/2, 28f);
			btnCreateAccount.TitleLabel.Font = UIFont.SystemFontOfSize(13);
			btnCreateAccount.BackgroundColor = UIColor.Clear;

			UILabel createAccountTitle = new UILabel (btnCreateAccount.Bounds);
			createAccountTitle.Text = LocalizationUtilities.LocalizedString("Account_SignUp", "Sign Up");
			createAccountTitle.TextColor = Utils.AppColors.PrimaryColor;
			createAccountTitle.TextAlignment = UITextAlignment.Right;
			btnCreateAccount.AddSubview (createAccountTitle);
			btnCreateAccount.TouchUpInside += async (object sender, EventArgs e) => {
			
				Utils.UI.ShowLoadingIndicator();

                OmniEnvironment  environment = await new Models.ContactModel().GetEnvironment();
                if (environment != null)
					{
						//success
						if(!string.IsNullOrEmpty(environment.PasswordPolicy))
							this.passwordPolicy = LocalizationUtilities.LocalizedString("Account_Password", "Password") + ": " + environment.PasswordPolicy;

						if(MemberContactAttributes.Registration.Profiles)
						{
                            List<Profile> profiles = await new Models.ProfileModel().GetAllProfiles();
                            if (profiles != null) 
						    {
							    Utils.UI.HideLoadingIndicator();

							    this.NavigationController.PushViewController(
							    	new RegistrationController (
							    		profiles, 
							    		passwordPolicy, 
								    	() => 
								    	{
								    		if (this.LoginSuccess != null)
								    			this.LoginSuccess(() => { this.DismissViewController(true, null); });
									    }
								    ),
								    true
							    );
						}
                            else
						    {
						    	Utils.UI.HideLoadingIndicator();

							    this.NavigationController.PushViewController(
							    	new RegistrationController (
							    		new List<Profile>(), 
							    		passwordPolicy,
								    	() =>
								    	{
								    		if (this.LoginSuccess != null)
								    			this.LoginSuccess(() => { this.DismissViewController(true, null); });
									    }
								    ),
								    true
							    );
						    }
							
						}
						else
						{
							Utils.UI.HideLoadingIndicator();

							this.NavigationController.PushViewController(
								new RegistrationController (
									new List<Profile>(), 
									passwordPolicy,
									() =>
									{
										if (this.LoginSuccess != null)
											this.LoginSuccess(() => { this.DismissViewController(true, null); });
									}
								),
								true
							);
						}
					}
                else
					{
						//failure - don't display the password policy
						if(MemberContactAttributes.Registration.Profiles)
						{
                            List<Profile> profiles = await new Models.ProfileModel().GetAllProfiles();
                         if (profiles != null) 
						    {
							    Utils.UI.HideLoadingIndicator();

							    this.NavigationController.PushViewController(
							    	new RegistrationController (
							    		profiles, 
								    	string.Empty,
							    		() =>
								    	{
									    	if (this.LoginSuccess != null)
									    		this.LoginSuccess(() => { this.DismissViewController(true, null); });
									    }
								    ), 
								    true
								    	);
						}
                        else
						{
							Utils.UI.HideLoadingIndicator();

							this.NavigationController.PushViewController(
								new RegistrationController (
									new List<Profile>(), 
									string.Empty,
									() =>
									{
										if (this.LoginSuccess != null)
											this.LoginSuccess(() => { this.DismissViewController(true, null); });
									}
								), 
								true
							);
						}
							
						}
						else
						{
							Utils.UI.HideLoadingIndicator();

							this.NavigationController.PushViewController(
								new RegistrationController (
									new List<Profile>(), 
									string.Empty,
									() =>
									{
										if (this.LoginSuccess != null)
											this.LoginSuccess(() => { this.DismissViewController(true, null); });
									}
								), 
								true
							);
						}
					}
				
			};
			headerView.AddSubview(btnCreateAccount);

			// Table itself
		
			Root = new RootElement (LocalizationUtilities.LocalizedString("Account_Login", "Log in"))
			{
				new Section (headerView, footerView)
				{
					loginEmailElement,
					loginPasswordElement
				}
			};

			this.TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
			this.TableView.ScrollEnabled = true;

			// Back button (in navigation bar)
			/*
			UIBarButtonItem backButton = new UIBarButtonItem ();
			backButton.Title = LocalizationUtilities.LocalizedString("General_Back", "Back");
			backButton.Clicked += (object sender, EventArgs e) => {

				BuildCreateAccountTable();
				this.NavigationItem.LeftBarButtonItem = null;

			};
			this.NavigationItem.LeftBarButtonItem = backButton;*/
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(() => { BaseViewTapped(); }));

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			if(!EnabledItems.ForceLogin)
			{
				UIBarButtonItem cancelButton = new UIBarButtonItem ();
				cancelButton.Title = LocalizationUtilities.LocalizedString("General_Cancel", "Cancel");
				cancelButton.Clicked += (object sender, EventArgs e) => {

					this.DismissViewController(true, null);

				};
				this.NavigationItem.RightBarButtonItem = cancelButton;
			}

			// Hidden button to access the change WS screen
			bool allowHiddenSettings = true;
			if (allowHiddenSettings)
			{
				UIBarButtonItem changeWSButton = new UIBarButtonItem ();
				changeWSButton.Title = "URL";
				changeWSButton.Clicked += (object sender, EventArgs e) => {
				
					this.changeWSButtonPressCounter++;

					if (this.changeWSButtonPressCounter >= 1)
					{
						this.PresentViewController(new UINavigationController(new HiddenSettingsController ()), true, null);
					}

				};
				this.NavigationItem.LeftBarButtonItem = changeWSButton;
			}

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

			BuildLoginTable();

			//RegisterKeyboardNotificationHandling();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			this.changeWSButtonPressCounter = 0;

			if (this.NavigationController != null) 
			{
				this.NavigationController.NavigationBarHidden = false;
				Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			}
		}

		private async void Login(EntryElement email, EntryElement password)
		{
			System.Diagnostics.Debug.WriteLine("Logging in user: " + email + ", pass: " + password);

			Utils.UI.ShowLoadingIndicator();
            bool success = await new ContactModel().MemberContactLogon(email.Value, password.Value, Utils.Util.PhoneId);
            if (success) 
			{
                if(!string.IsNullOrEmpty(InstanceId.SharedInstance.Token)){

                    ContactModel model = new ContactModel();
                    await model.RegisterForPushNotificationsInBackendServer(InstanceId.SharedInstance.Token);
                }

				Utils.UI.HideLoadingIndicator();
				email.Value = string.Empty;
				password.Value = string.Empty;
				if (this.LoginSuccess != null)
				{
					this.LoginSuccess(() => { this.DismissViewController(true, null); });
				}
			}
            else
				{
					Utils.UI.HideLoadingIndicator();
					this.loginPasswordElement.Value = string.Empty;
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

		private async void ForgotPassword()
		{
			string title = LocalizationUtilities.LocalizedString ("Account_ResetPasswordTitle", "Reset password");
			string message = LocalizationUtilities.LocalizedString ("Account_ResetPasswordMsg", "Enter your username and a new password will be sent to your email");

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

		}

		private void BaseViewTapped()
		{			
			this.View.EndEditing(true);	// Hide the keyboard
		}
	}
}
