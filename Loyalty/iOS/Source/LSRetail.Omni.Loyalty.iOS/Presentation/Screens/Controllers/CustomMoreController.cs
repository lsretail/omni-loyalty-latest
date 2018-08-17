using UIKit;
using System.Collections.Generic;
using Presentation.Screens;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class CustomMoreController : UIViewController
    {
        public List<UIViewController> moreControllers;
        private CustomMoreView rootView;

        public CustomMoreController(List<UIViewController> controllers)
        {
            Title = LocalizationUtilities.LocalizedString("MoreScreen_More", "More");
            moreControllers = controllers;

            CustomMoreView customMoreView = new CustomMoreView(this.moreControllers);
            customMoreView.OnControllerSelected += OnControllerSelected;
            rootView = customMoreView;
            View = rootView;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
            this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);


            // if the user is logged in, but the more controller only contains the LoginScreen controller,
            // we switch it out for Account controller and vice versa
            /*if(AppData.UserLoggedIn)
			{
				if(this.moreControllers[0] is LoginScreen)
				{
					this.moreControllers.RemoveAt(0);

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
					accountScreen.TabBarItem = new UITabBarItem(
						accountScreen.Title,
						Utils.Image.FromFile ("IconsForTabBar/Account.png"),
						null
					);

					this.moreControllers.Insert(0, accountScreen);
				}
			}
			else
			{
				if(this.moreControllers[0] is AccountController)
				{
					this.moreControllers.RemoveAt(0);

					var loginScreen = new LoginScreen (
						new LoginScreen.LoginSuccessDelegate(delegate(Action dismissSelf) {

							if (Utils.Util.AppDelegate.UsePushNotifications)
							{
								Models.NotificationModel.RegisterForRemoteNotifications();						
							}

							dismissSelf();
						})
					);
					loginScreen.TabBarItem = new UITabBarItem(
						loginScreen.Title,
						Utils.Image.FromFile ("IconsForTabBar/Account.png"),
						null
					);

					this.moreControllers.Insert(0, loginScreen);
				}
			}*/

            this.rootView.Refresh(AppData.UserLoggedIn);
        }

        private void OnControllerSelected(UIViewController controller)
        {
            if (controller is LoginScreen)
            {
                Utils.Util.AppDelegate.RootTabBarController.PresentViewController(new UINavigationController(controller), true, null);
            }
            else
            {
                this.NavigationController.PushViewController(controller, true);
            }
        }

        // Used when navigating to a controller in code
        public void PresentController(int index)
        {
            UIViewController controller = this.moreControllers[index];

            this.NavigationController.PopToRootViewController(false);

            if (controller != null)
                this.NavigationController.PushViewController(controller, true);
        }
    }
}

