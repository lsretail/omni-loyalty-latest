using System;
using System.Collections.Generic;
using Presentation.Screens;
using UIKit;
using Presentation.Utils;
using Foundation;
using System.Linq;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public class RootTabBarController : UITabBarController
	{
		public List<UIViewController> controllersToShow;
		public List<UIViewController> controllersForTabBar;

		public HomeController homeScreen;
		public UI.MenuCollectionController menuScreen;
		public BasketController basketController;
		public OffersAndCouponsCardCollectionController offersAndCouponsCardCollectionController;
		public FavouriteController favoritesScreen;
		public LocationsCardCollectionController locationsScreen;
		public HistoryController historyController;
		public AboutUsController aboutUsController;
		public AccountController accountScreen;
		public LoginDialogController loginDialogController;

		public RootTabBarController()
		{
			InitViewControllers();
			this.TabBar.SelectedImageTintColor = AppColors.PrimaryColor;
			Utils.UI.TabBarHeight = TabBar.Frame.Size.Height;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

		private void InitViewControllers()
		{
			this.controllersToShow = new List<UIViewController>();

			//Home
			this.homeScreen = new HomeController();
			this.homeScreen.TabBarItem = new UITabBarItem(
				this.homeScreen.Title,
				Image.FromFile("/IconsForTabBar/TabBarIconHome.png"),
				null
			);
			controllersToShow.Add(this.homeScreen);

			//Menu
			//this.menuScreen = new MenuCollectionController.MenuCollectionController(new UICollectionViewFlowLayout(), null, LocalizationUtilities.LocalizedString("Menu_Menu", "Menu"), CardCollectionCell.CellSizes.TallWide, true);
			this.menuScreen = new UI.MenuCollectionController();

			this.menuScreen.TabBarItem = new UITabBarItem(
				this.menuScreen.Title,
				Image.FromFile("/IconsForTabBar/TabBarIconMenu.png"),
				null
			);
			controllersToShow.Add(this.menuScreen);

			//Basket
			this.basketController = new BasketController();
			this.basketController.TabBarItem = new UITabBarItem(
				this.basketController.Title,
				Image.FromFile("/IconsForTabBar/TabBarIconShoppingBasket.png"),
				null
			);
			controllersToShow.Add(this.basketController);


			//Offer And Coupon
			this.offersAndCouponsCardCollectionController = new OffersAndCouponsCardCollectionController(new UICollectionViewFlowLayout());
			this.offersAndCouponsCardCollectionController.TabBarItem = new UITabBarItem(
				this.offersAndCouponsCardCollectionController.Title,
				Image.FromFile("/IconsForTabBar/TabBarIconOC.png"),
				null
			);
			controllersToShow.Add(this.offersAndCouponsCardCollectionController);

			//Favorites
			this.favoritesScreen = new FavouriteController();
			this.favoritesScreen.TabBarItem = new UITabBarItem(
				this.favoritesScreen.Title,
				Image.FromFile("/IconsForTabBar/TabBarIconFavorite.png"),
				null
			);
			controllersToShow.Add(this.favoritesScreen);

			//Locations
			this.locationsScreen = new LocationsCardCollectionController(new UICollectionViewFlowLayout());
			this.locationsScreen.TabBarItem = new UITabBarItem(
				this.locationsScreen.Title,
				Image.FromFile("/IconsForTabBar/TabBarIconLocations.png"),
				null
			);
			controllersToShow.Add(this.locationsScreen);

			//History
			this.historyController = new HistoryController();
			this.historyController.TabBarItem = new UITabBarItem(
				this.historyController.Title,
				Image.FromFile("/IconsForTabBar/TabBarIconHistory.png"),
				null
			);
			controllersToShow.Add(this.historyController);

			//Information
			if (Util.AppDelegate.AboutUsControllerEnabled)
			{
				this.aboutUsController = new AboutUsController();
				this.aboutUsController.TabBarItem = new UITabBarItem(
					this.aboutUsController.Title,
					Image.FromFile("/IconsForTabBar/TabBarIconInfo.png"),
					null
				);
				controllersToShow.Add(this.aboutUsController);
			}

			if (AppData.UserLoggedIn)
			{
				this.accountScreen = new AccountController(AppData.Contact, delegate (Action<bool> dismissSelf)
				{
					dismissSelf(true);
				});
				this.accountScreen.TabBarItem = new UITabBarItem(
					this.accountScreen.Title,
					Image.FromFile("/IconsForTabBar/TabBarIconAccount.png"),
					null
				);
				controllersToShow.Add(this.accountScreen);
			}
			else
			{
				this.loginDialogController = new LoginDialogController(new LoginDialogController.LoginSuccessDelegate(delegate (Action dismissSelf)
				{
					dismissSelf();
				}));
				this.loginDialogController.TabBarItem = new UITabBarItem(
					this.loginDialogController.Title,
					Image.FromFile("/IconsForTabBar/TabBarIconAccount.png"),
					null
				);
				controllersToShow.Add(this.loginDialogController);
			}

			// Let's arrange the controllers
			controllersToShow = ArrangeViewControllers(controllersToShow);

			// Let's add the controllers to the tab bar

			if (controllersToShow.Count > 5)
			{
				// We can't fit all the controllers on the tab bar at once, let's put the first four on the tab bar and the rest into
				// our custom "More" controller. The "More" controller will then be the fifth entry on the tab bar.

				var controllersForMoreController = controllersToShow.GetRange(4, controllersToShow.Count - 4);
				CustomMoreController moreController = new CustomMoreController(controllersForMoreController);
				moreController.TabBarItem = new UITabBarItem(
					moreController.Title,
					Image.FromFile("IconsForTabBar/TabBarIconMore.png"),
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
		private List<UIViewController> ArrangeViewControllers(List<UIViewController> controllers)
		{
			var screenArrangement = new List<Type>()
			{
				typeof (HomeController),
				typeof (UI.MenuCollectionController),
				typeof (BasketController),
				typeof (OffersAndCouponsCardCollectionController),
				typeof (AccountController),
				typeof (LoginDialogController),
				typeof (FavouriteController),
				typeof (LocationsCardCollectionController),
				typeof (HistoryController),
				typeof (AboutUsController)
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
	}
}

