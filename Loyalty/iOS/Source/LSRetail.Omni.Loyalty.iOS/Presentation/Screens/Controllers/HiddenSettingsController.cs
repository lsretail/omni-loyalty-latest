using System;
using UIKit;
using Foundation;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using System.Threading.Tasks;

namespace Presentation
{
	public class HiddenSettingsController : UIViewController
	{
		private HiddenSettingsView rootView;
		private string urlAtBeginning;

		public HiddenSettingsController ()
		{
			Title = LocalizationUtilities.LocalizedString("Hidden_Settings_ChangeWS", "Change Webservice");
			rootView = new HiddenSettingsView ();
			rootView.PingButtonClicked += PingButtonClicked;
			urlAtBeginning = Settings.GetBaseURL ();
			
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			this.View.BackgroundColor = UIColor.White;

			UIBarButtonItem doneButton = new UIBarButtonItem ();
			doneButton.Title = LocalizationUtilities.LocalizedString("General_Done", "Done");
			doneButton.Clicked += async (object sender, EventArgs e) => {

				if(urlAtBeginning != this.rootView.GetUrlTextField ())
				{
					var alertResult = await AlertView.ShowAlert(
						this,
						LocalizationUtilities.LocalizedString ("General_Confirmation", "Confirmation"),
						LocalizationUtilities.LocalizedString("HiddenSettings_AreYouSure","Are you sure that you want to change the web service"),
						LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
						LocalizationUtilities.LocalizedString("General_No", "No")
					);

					if (alertResult == AlertView.AlertButtonResult.PositiveButton)
					{
					    SaveUrl(this.rootView.GetUrlTextField ());
						AppDelegate.InitWebService();
					}
					else if (alertResult == AlertView.AlertButtonResult.NegativeButton)
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

			};
			this.NavigationItem.RightBarButtonItem = doneButton;

			this.rootView.UpdateData (Settings.GetBaseURL ());
			this.View = this.rootView;
		}

		private void SaveUrl(string url)
		{
			Settings.SetBaseURL (url);
		}

		private async void PingButtonClicked(object sender, EventArgs args)
		{
			if (rootView.GetUrlTextField() != urlAtBeginning)
			{
				Settings.SetBaseURL(rootView.GetUrlTextField());
			}
					
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
			catch (Exception exception)
            {
                
                Utils.UI.HideLoadingIndicator();
				string title = LocalizationUtilities.LocalizedString("HiddenSettings_Ping", "Ping");
				string text = exception.Message;
				string okButtonText = LocalizationUtilities.LocalizedString("General_OK", "OK");
				await AlertView.ShowAlert(
					this,
					title,
					text,
					okButtonText
				);
            }


				if (rootView.GetUrlTextField() != urlAtBeginning)
				{
					Settings.SetBaseURL(urlAtBeginning);
				}
		}
	}
}

