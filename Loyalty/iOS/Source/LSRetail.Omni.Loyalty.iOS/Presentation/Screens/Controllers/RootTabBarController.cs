using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Screens
{
    public class RootTabBarController : UITabBarController
	{				
		public List<UIViewController> controllersToShow;
		public List<UIViewController> controllersForTabBar;

		public RootTabBarController ()
		{			
			InitViewControllers();
			this.TabBar.SelectedImageTintColor = Utils.AppColors.PrimaryColor;
		}
			
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (EnabledItems.ForceLogin && !AppData.UserLoggedIn)
			{				
				PresentLoginScreen(false, () => {});
			}
		}
			
		private List<UIViewController> ArrangeViewControllers(List<UIViewController> controllers)
		{
			var screenArrangement = new List<Type>()
			{
				typeof (HomeController),
				typeof (ItemScreen),
				typeof (OffersAndCouponsScreen2),
				typeof (BasketController),
				typeof (LocationsScreen),
				typeof (AccountController),
				typeof (LoginScreen),
				typeof (NotificationsController),
				typeof (HistoryController),
				typeof (WishListController),
				typeof (SearchController),
				typeof (ContactUsController)
			};
				
			var arrangedList = 
				from screenType in screenArrangement 
				join controller in controllers
				on screenType equals controller.GetType()
				select controller;

			string sArrangement = "Controllers arranged - arrangement: ";
			foreach (var x in arrangedList) sArrangement += "[" + x.Title + "]" + " ";
			System.Diagnostics.Debug.WriteLine(sArrangement);

			return arrangedList.ToList();
		}
			
		private void InitViewControllers()
		{						
			this.controllersToShow = new List<UIViewController>();

			// Home
			if (EnabledItems.HasHome)
			{
				var homeScreen = new HomeController ();
				homeScreen.TabBarItem = new UITabBarItem(
					homeScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/Home.png"),
					null
				);
				controllersToShow.Add(homeScreen);
			}

			// Items
			if (EnabledItems.HasItemCatalog)
			{
				var itemScreen = new ItemScreen (new UICollectionViewFlowLayout(), ItemScreen.ItemListType.Category, LocalizationUtilities.LocalizedString("SlideoutMenu_Items", "Items"), null, null, CardCollectionCell.CellSizes.TallWide);
				itemScreen.TabBarItem = new UITabBarItem(
					itemScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/items.png"),
					null
				);
				controllersToShow.Add(itemScreen);
			}

			// Basket
			if (EnabledItems.HasBasket)
			{
				var basketController = new BasketController();
				basketController.TabBarItem = new UITabBarItem (
					basketController.Title,
					ImageUtilities.FromFile ("IconsForTabBar/ShoppingBasket.png"),
					null
				);
				controllersToShow.Add (basketController);
			}

			// Notifications
			if (EnabledItems.HasNotifications)
			{
				var notificationsScreen = new NotificationsController();
				notificationsScreen.TabBarItem = new UITabBarItem(
					notificationsScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/notification.png"), // Images added here
					null
				);
				controllersToShow.Add (notificationsScreen);
			}

			// Locations
			if (EnabledItems.HasStoreLocator)
			{
				var locationsScreen = new LocationsScreen (new UICollectionViewFlowLayout(), AppData.Stores, false);
				locationsScreen.TabBarItem = new UITabBarItem(
					locationsScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/Locations.png"),
					null
				);
				controllersToShow.Add(locationsScreen);
			}

			// Offers and coupons
			if (EnabledItems.HasOffers || EnabledItems.HasCoupons)
			{
				var offersAndCouponsScreen = new OffersAndCouponsScreen2 (new UICollectionViewFlowLayout());
				offersAndCouponsScreen.TabBarItem = new UITabBarItem(
					offersAndCouponsScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/offers.png"),
					null
				);
				controllersToShow.Add(offersAndCouponsScreen);
			}

			// History
			if (EnabledItems.HasHistory)
			{
				var historyScreen = new HistoryController ();
				historyScreen.TabBarItem = new UITabBarItem(
					historyScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/History.png"),
					null
				);
				controllersToShow.Add(historyScreen);
			}

			// Search
			if (EnabledItems.HasSearch)
			{
				var searchController = new SearchController();
				searchController.TabBarItem = new UITabBarItem(
					searchController.Title,
					ImageUtilities.FromFile ("IconsForTabBar/search.png"),
					null
				);
				controllersToShow.Add (searchController);
			}

			// Wish List
			if (EnabledItems.HasWishLists)
			{
				var wishListScreen = new WishListController ();
				wishListScreen.TabBarItem = new UITabBarItem(
					wishListScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/shoppingList.png"),
					null
				);
				controllersToShow.Add (wishListScreen);
			}

			// Contact Us
			if (EnabledItems.HasContactUs)
			{
				var contactUsScreen = new ContactUsController ();
				contactUsScreen.TabBarItem = new UITabBarItem(
					contactUsScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/Info.png"),
					null
				);
				controllersToShow.Add (contactUsScreen);
			}

			/*if (EnabledItems.ForceLogin || AppData.UserLoggedIn) 
			{*/
				var accountScreen = new AccountController (
					new AccountController.LogoutSuccessDelegate(delegate(Action<bool> dismissSelf) 
						{
							if(EnabledItems.ForceLogin)
							{
								PresentLoginScreen(true, () => 
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
					ImageUtilities.FromFile ("IconsForTabBar/Account.png"),
					null
				);
				controllersToShow.Add (accountScreen);
			/*} 
			else 
			{*/
				var LoginScreen = new LoginScreen (
					new LoginScreen.LoginSuccessDelegate(delegate(Action dismissSelf) {

						if (Utils.Util.AppDelegate.UsePushNotifications)
						{
							//Models.NotificationModel.RegisterForRemoteNotifications();						
						}
						
						dismissSelf();
					})
				);
				LoginScreen.TabBarItem = new UITabBarItem(
					LoginScreen.Title,
					ImageUtilities.FromFile ("IconsForTabBar/Account.png"),
					null
				);
				controllersToShow.Add (LoginScreen);
			//}

			// Let's arrange the controllers
			controllersToShow = ArrangeViewControllers(controllersToShow);

			// Let's add the controllers to the tab bar

			if (controllersToShow.Count > 5)
			{
				// We can't fit all the controllers on the tab bar at once, let's put the first four on the tab bar and the rest into
				// our custom "More" controller. The "More" controller will then be the fifth entry on the tab bar.

				var controllersForMoreController = controllersToShow.GetRange(4, controllersToShow.Count - 4);
				CustomMoreController moreController = new CustomMoreController(controllersForMoreController);
				moreController.TabBarItem =  new UITabBarItem(
					moreController.Title,
					ImageUtilities.FromFile ("IconsForTabBar/More.png"),
					null
				);

				this.controllersForTabBar = controllersToShow.GetRange(0, 4);
				controllersForTabBar.Add(moreController);

				// Let's wrap the controllers in their own navigation controller
				var navigationControllersToShow = new List<UINavigationController>();
				foreach (var controller in controllersForTabBar)
					navigationControllersToShow.Add(new UINavigationController(controller));

				this.ViewControllers = navigationControllersToShow.ToArray();
			}
			else
			{
				// We can fit all controllers on the tab bar

				// Let's wrap the controllers in their own navigation controller
				var navigationControllersToShow = new List<UINavigationController>();
				foreach (var controller in controllersToShow)
					navigationControllersToShow.Add(new UINavigationController(controller));

				this.ViewControllers = navigationControllersToShow.ToArray();
			}
		}

		public void PresentLoginScreen(bool animated, Action onFinished)
		{
			LoginScreen loginScreen = new LoginScreen (
				new LoginScreen.LoginSuccessDelegate(delegate(Action dismissSelf) {

					if (Utils.Util.AppDelegate.UsePushNotifications)
					{
						//Models.NotificationModel.RegisterForRemoteNotifications();						
					}

					dismissSelf();
				})
			);

			this.PresentViewController(new UINavigationController(loginScreen), animated, onFinished);
		}			
	}
}

