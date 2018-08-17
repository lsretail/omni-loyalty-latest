using UIKit;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public class WelcomeController : UIViewController, WelcomeView.IWelcomeListeners
	{
		private WelcomeView rootView;

		public WelcomeController()
		{
			Title = LocalizationUtilities.LocalizedString("Welcome_Welcome", "Welcome");
			rootView = new WelcomeView(this);
			//rootView.Next += Next;
		}


		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//this.NavigationController.NavigationBarHidden = true;

			this.View = rootView;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			this.View = rootView;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
		}

		public void Next()
		{
			NSUserDefaults.StandardUserDefaults.SetBool(true, "FirstTimeRun");
			//Utils.Util.AppDelegate.SlideoutMenu.HomeScreen.RefreshLayout();
			this.NavigationController.PopViewController(false);
		}
	}
}

