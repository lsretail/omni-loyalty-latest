using System;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation
{
	public class AboutUsController : UIViewController, AboutUsView.IAboutUsViewListener
	{
		private AboutUsView rootView;

		public AboutUsController()
		{
			this.Title = LocalizationUtilities.LocalizedString("AboutUs_Title", "About us");
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.rootView = new AboutUsView(this);
			this.View = this.rootView;

			GetAboutUsInformation();
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public async void GetAboutUsInformation()
		{
			Utils.UI.ShowLoadingIndicator ();

			string contactUsInfoString = await new Models.AppSettingsModel().AppSettingsGetByKey(
				AppSettingsKey.ContactUs,
				"IS");
			if (contactUsInfoString != null)  
			{
				// On success
				Utils.UI.HideLoadingIndicator();
				this.rootView.HideErrorGettingDataView();
				this.rootView.UpdateData(contactUsInfoString);
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

