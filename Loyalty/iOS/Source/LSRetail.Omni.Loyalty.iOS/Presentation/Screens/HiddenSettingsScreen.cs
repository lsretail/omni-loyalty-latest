using System;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using Domain.Utils;
using Domain.Debugs;
using Presentation.Models;
using Presentation.Utils;
using Infrastructure.Data.WS.Utils;
using Infrastructure.Data.WS.Debug;
using Infrastructure.Data.SQLite2.Webservice;
using Infrastructure.Data.SQLite2.DTO;

namespace Presentation.Screens
{
	/*
	public class HiddenSettingsScreen : UIViewController
	{
		private UIButton btnOK;
		private UILabel urlLabel;
		private UITextField urlTextField;
		private string urlAtBeginning;
		private UILabel lblVersion;

		public HiddenSettingsScreen()
		{
			this.Title = NSBundle.MainBundle.LocalizedString("Hidden_Settings_ChangeWS", "Change Webservice");
		}
									
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = UIColor.White;

			UIBarButtonItem doneButton = new UIBarButtonItem ();
			doneButton.Title = NSBundle.MainBundle.LocalizedString("General_Done", "Done");
			doneButton.Clicked += (object sender, EventArgs e) => {

				if(urlAtBeginning != this.urlTextField.Text)
				{
					var Confirm  = new UIAlertView(NSBundle.MainBundle.LocalizedString ("General_Confirmation", "Confirmation"),
						NSBundle.MainBundle.LocalizedString("HiddenSettings_AreYouSure","Are you sure that you want to change the web service"),
						null,
						NSBundle.MainBundle.LocalizedString ("General_No", "No"),
						NSBundle.MainBundle.LocalizedString ("General_Yes", "Yes"));
					Confirm.Show();
					Confirm.Clicked += (object senders, UIButtonEventArgs es) => 
					{
						if (es.ButtonIndex == 0 ) 
						{
							SaveUrl(urlAtBeginning);
							AppDelegate.InitWebService();

						}
						else
						{
							SaveUrl(this.urlTextField.Text);
							AppDelegate.InitWebService();
						}
						this.DismissViewController(true, null);
					};				
				}
				else
				{
					this.DismissViewController(true, null);		
				}

			};
			this.NavigationItem.RightBarButtonItem = doneButton;

			// UI elements

			this.urlLabel = new UILabel();
			urlLabel.Text = NSBundle.MainBundle.LocalizedString("Hidden_Settings_URL", "URL");
			urlLabel.Font = UIFont.SystemFontOfSize(14);
			urlLabel.TextAlignment = UITextAlignment.Left;
			urlLabel.TextColor = Utils.AppColors.PrimaryColor;
			this.View.AddSubview(this.urlLabel);

			this.urlTextField = new UITextField();
			urlTextField.Text = NSBundle.MainBundle.LocalizedString("Hidden_Settings_URL", "URL");
			urlTextField.Delegate = new CustomTextFieldDelegate();
			this.View.AddSubview(urlTextField);

			this.btnOK = new UIButton();
			btnOK.SetTitle(NSBundle.MainBundle.LocalizedString("HiddenSettings_Ping", "Ping"), UIControlState.Normal);
			btnOK.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnOK.Layer.CornerRadius = 2;
			btnOK.TouchUpInside += (object sender, EventArgs e) => {

				SaveUrl(this.urlTextField.Text);
				AppDelegate.InitWebService();
				Utils.UI.ShowLoadingIndicator();
				new DebugModel().Ping(
					(message)=> {
						Utils.UI.HideLoadingIndicator();
						string title = NSBundle.MainBundle.LocalizedString ("HiddenSettings_Ping", "Ping");
						string text = message;
						string okButtonText = NSBundle.MainBundle.LocalizedString ("General_OK", "OK");
						new UIAlertView(title, text, null, okButtonText, null).Show();
					},
					(message) => {
						Utils.UI.HideLoadingIndicator(); 
						string title = NSBundle.MainBundle.LocalizedString("HiddenSettings_Ping", "Ping");
						string text = message;
						string okButtonText = NSBundle.MainBundle.LocalizedString("General_OK", "OK");
						new UIAlertView(title, text, null, okButtonText, null).Show();
					});
			};
			this.View.AddSubview(btnOK);

			this.lblVersion = new UILabel();
			this.lblVersion.TextAlignment = UITextAlignment.Center;
			this.View.AddSubview(this.lblVersion);

			SetTextFieldValues();
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			float margin = 20f;
			this.urlLabel.Frame = new CGRect(margin, this.TopLayoutGuide.Length + 30f, this.View.Frame.Width - 2 * margin, 20f);
			this.urlTextField.Frame = new CGRect(margin, this.urlLabel.Frame.Bottom, this.View.Frame.Width - 2 * margin, 20f);

			float buttonHeight = 50f;
			float buttonMargin = 20f;
			this.btnOK.Frame = new CGRect(buttonMargin, this.urlTextField.Frame.Bottom + buttonHeight, this.View.Frame.Width - 2 * buttonMargin, buttonHeight);

			this.lblVersion.Frame = new CGRect(
				0,
				this.btnOK.Frame.Bottom + 30f,
				this.View.Frame.Width,
				20f
			);
		}


		// Code that we do not use any more. Instead of saving the url to database we save it to memory
		private void SetTextFieldValuesFromDatabase()
		{
			WebserviceData wsData = (new WebserviceModel().GetWebserviceData());

			if (wsData != null)
			{
				if (!string.IsNullOrEmpty(wsData.BaseUrl))
				{
					this.urlTextField.Text = wsData.BaseUrl;
				}
				else
				{
					this.urlTextField.Text = Infrastructure.Data.WS.Utils.Utils.DefaultUrl;  //TODO : ??
				}
			}
			else
			{
				this.urlTextField.Text = "";
			}
		}

		private void SaveUrl(string url)
		{
			Settings.SetBaseURL (url);
		}


		private void SetTextFieldValues()
		{
			var url = Settings.GetBaseURL ();
			this.urlAtBeginning = url;
			this.urlTextField.Text = url;
			this.lblVersion.Text = "Version " + Utils.Util.AssemblyVersion;
		}
	}

	class CustomTextFieldDelegate : UITextFieldDelegate
	{
		public override bool ShouldReturn (UITextField textField)
		{
			textField.EndEditing(true);
			return true;
		}
	}
	*/
}