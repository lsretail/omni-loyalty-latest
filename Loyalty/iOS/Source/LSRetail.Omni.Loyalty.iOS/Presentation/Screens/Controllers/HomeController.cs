using System;
using UIKit;
using Presentation.Utils;
using Presentation.Screens;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation
{
    public class HomeController : UIViewController
	{
		private HomeView rootView;

		public HomeController ()
		{
			Title = LocalizationUtilities.LocalizedString("Home_Home", "Home");

			rootView = new HomeView ();
			rootView.btnShortcut1Pressed += ShortCut1Pressed;
			rootView.btnShortcut2Pressed += ShortCut2Pressed;
			rootView.ContainerViewClicked += NavigateToAccountOrLogIn;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			SetRightBarButtonItems ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			rootView.UpdateData (
				LocalizationUtilities.LocalizedString("Location_Locations", "Locations"),
				ImageUtilities.FromFile ("IconsForTabBar/Locations.png"),
				LocalizationUtilities.LocalizedString("Notifications_Notifications", "Notifications"),
				ImageUtilities.FromFile ("IconsForTabBar/notification.png")
			);
			this.View = rootView;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			rootView.UpdateData (
				LocalizationUtilities.LocalizedString("Location_Locations", "Locations"),
				ImageUtilities.FromFile ("IconsForTabBar/Fullsize/Locations.png"),
				LocalizationUtilities.LocalizedString("Notifications_Notifications", "Notifications"),
				ImageUtilities.FromFile ("IconsForTabBar/Fullsize/notification.png")
			);

			this.View = rootView;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}	

		public void SetRightBarButtonItems()
		{

		}


		public void NavigateToAccountOrLogIn()
		{
			if (AppData.UserLoggedIn) {
				var accountScreen = new AccountController (
					new AccountController.LogoutSuccessDelegate(delegate(Action<bool> dismissSelf) 
						{
							if(EnabledItems.ForceLogin)
							{
								Utils.Util.AppDelegate.RootTabBarController.PresentLoginScreen(true, () => 
									{
										dismissSelf(false);
									}
								);
							}
							else
							{
								dismissSelf(true);
							}
						}
					)
				);

				this.NavigationController.PushViewController(accountScreen, true);

			} 
			else
			{
				Utils.Util.AppDelegate.RootTabBarController.PresentLoginScreen (true, () => {});
			}
		}

		public void ShortCut1Pressed()
		{
			var locationsScreen = new LocationsScreen (new UICollectionViewFlowLayout(), AppData.Stores, false);
			this.NavigationController.PushViewController ( locationsScreen, true);
		}

		public void ShortCut2Pressed()
		{			
			var notificationsController = new NotificationsController ();
			this.NavigationController.PushViewController (notificationsController, true);
		}
	}
}

