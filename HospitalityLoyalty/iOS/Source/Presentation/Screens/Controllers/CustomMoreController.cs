using System;
using System.Collections.Generic;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class CustomMoreController : UIViewController
	{
		public List<UIViewController> moreControllers;
		private CustomMoreView rootView;

		public CustomMoreController (List<UIViewController> controllers)
		{
			this.Title = LocalizationUtilities.LocalizedString("MoreScreen_More", "More");
			this.moreControllers = controllers;

			CustomMoreView customMoreView = new CustomMoreView (this.moreControllers);
			customMoreView.OnControllerSelected += OnControllerSelected;
			rootView = customMoreView;
			this.View = this.rootView;
		}

		private void OnControllerSelected(UIViewController controller)
		{
			if (controller is LoginDialogController)
			{
				Util.AppDelegate.RootTabBarController.PresentViewController (new UINavigationController(controller), true, null);
			}
			else
			{
				this.NavigationController.PushViewController(controller, true);
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			// if the user is logged in, but the more controller only contains the LoginScreen controller,
			// we switch it out for Account controller and vice versa
			if (AppData.UserLoggedIn)
			{
				if (this.moreControllers[0] is LoginDialogController)
				{
					this.moreControllers.RemoveAt(0);

					var accountScreen = new AccountController( AppData.Contact,
						new AccountController.LogoutSuccessDelegate(delegate (Action<bool> dismissSelf)
							{
								dismissSelf(true);
							}
						)
					);
					accountScreen.TabBarItem = new UITabBarItem(
						accountScreen.Title,
						Image.FromFile("IconsForTabBar/Account.png"),
						null
					);

					this.moreControllers.Insert(0, accountScreen);
				}
			}
			else
			{
				if (this.moreControllers[0] is AccountController)
				{
					this.moreControllers.RemoveAt(0);

					var loginDialogController = new LoginDialogController(
						new LoginDialogController.LoginSuccessDelegate(delegate (Action dismissSelf)
						{
							dismissSelf();
						})
					);
					loginDialogController.TabBarItem = new UITabBarItem(
						loginDialogController.Title,
						Image.FromFile("IconsForTabBar/Account.png"),
						null
					);

					this.moreControllers.Insert(0, loginDialogController);
				}
			}
			this.rootView.Refresh();
		}
	}
}

