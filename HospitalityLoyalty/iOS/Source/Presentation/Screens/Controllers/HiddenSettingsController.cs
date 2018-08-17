using System;
using Foundation;
using Presentation;
using Presentation.Models;
using Presentation.Utils;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using System.Threading.Tasks;

namespace Presentation.Screens
{
	public class HiddenSettingsController : UIViewController, HiddenSettingsView.IHiddenSettingsListeners
	{
		private HiddenSettingsView rootView;
		private string urlAtBeginning;

		public HiddenSettingsController()
		{
			this.Title = LocalizationUtilities.LocalizedString("Hidden_Settings_Change_URL" , "Change URL");
			this.rootView = new HiddenSettingsView(this);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UIBarButtonItem doneButton = new UIBarButtonItem();
			doneButton.Title = LocalizationUtilities.LocalizedString("General_Done", "Done");
			doneButton.Clicked += DoneButtonClicked;
			this.NavigationItem.RightBarButtonItem = doneButton;

			var url = Settings.GetBaseURL();
			this.urlAtBeginning = url;

			this.rootView.UpdateData(url);
			this.View = this.rootView;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
		}

		public async void DoneButtonClicked(object sender, EventArgs e)
		{
			if (urlAtBeginning != this.rootView.GetUrlText())
			{
				var alertResult = await AlertView.ShowAlert(
					this,
					LocalizationUtilities.LocalizedString("General_Confirmation", "Confirmation"),
					LocalizationUtilities.LocalizedString("HiddenSettings_AreYouSure", "Are you sure that you want to change the web service"),
					LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
					LocalizationUtilities.LocalizedString("General_No", "No")
				);

				if (alertResult == AlertView.AlertButtonResult.PositiveButton)
				{
				    SaveUrl(this.rootView.GetUrlText());
					AppDelegate.InitWebService();
				}
				else
				{
					SaveUrl(urlAtBeginning);
					AppDelegate.InitWebService();
				}
				this.DismissViewController(true, null);
			}
			else
			{
				this.DismissViewController(true, null);
			}
		}

		public async void PingButtonClicked()
		{
			SaveUrl(this.rootView.GetUrlText());
			AppDelegate.InitWebService();
			Utils.UI.ShowLoadingIndicator();

			try
			{
					string message = "";
					var utils = new LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils();
					message = await Task.Run(() => utils.PingServer());
					Utils.UI.HideLoadingIndicator();
					string title = LocalizationUtilities.LocalizedString("HiddenSettings_Ping", "Ping");
					string text = message;
					string okButtonText = LocalizationUtilities.LocalizedString("General_OK", "OK");
					await AlertView.ShowAlert(
						this,
						title,
						text,
						okButtonText

					);
			}

			catch(Exception ex)
			{
				string message = ex.Message;

				Utils.UI.HideLoadingIndicator();
				string title = LocalizationUtilities.LocalizedString("HiddenSettings_Ping", "Ping");
				string text = message;
				string okButtonText = LocalizationUtilities.LocalizedString("General_OK", "OK");
				await AlertView.ShowAlert(
				    this,
					title,
					text,
					okButtonText
				);
			}
		}

		private void SaveUrl(string url)
		{
			Settings.SetBaseURL(url);
		}
	}
}

