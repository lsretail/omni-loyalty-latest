using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base;

namespace Presentation
{

	public class ContactUsController : UIViewController
	{
		private ContactUsView rootView;

		public ContactUsController ()
		{
			this.Title = NSBundle.MainBundle.GetLocalizedString("SlideoutMenu_ContactUs", "Contact us");
			this.rootView = new ContactUsView ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			this.View = this.rootView;
			GetData ();
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.BottomLayoutGuideLength = BottomLayoutGuide.Length;
			this.rootView.TopLayoutGuideLength = TopLayoutGuide.Length;
		}

		private async void GetData()
		{
			Utils.UI.ShowLoadingIndicator ();

            string contactUsInfoString = await new Models.SearchModel().GetAppSettings(AppSettingsKey.ContactUs, "EN");
            if (contactUsInfoString != "") 
				{
					// On success
					Utils.UI.HideLoadingIndicator();
					this.rootView.HideErrorGettingDataView();
					this.rootView.UpdateData (contactUsInfoString);
				}
            else
				{
					// On failure
					Utils.UI.HideLoadingIndicator();
					this.rootView.ShowErrorGettingDataView();
				}
			
		}
	}

}

