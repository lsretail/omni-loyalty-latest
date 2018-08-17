using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Screens;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.UI;

namespace Presentation.Screens
{
	public class HomeController : UIViewController, HomeView.IHomeListeners
	{
		private HomeView rootView;
		private MenuService menuService;
		public bool IsDataLoaded { get; set; }

		#region Constructor
		public HomeController()
		{
			this.Title = LocalizationUtilities.LocalizedString("Home_Home", "Home");

			rootView = new HomeView(this);
			menuService = new MenuService();
			if (IsDataLoaded)
			{
				this.rootView.RefreshLayout(AppData.UserLoggedIn, AppData.Contact);
			}

			SetRightBarButtonItems();
		}
		#endregion

		#region overrides
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.View = rootView;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			GetData();

			if (this.rootView.timer != null)
				this.rootView.timer.Start();
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			if (this.rootView.timer != null)
				this.rootView.timer.Stop();
		}
		#endregion

		#region helpers
		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private async void GetData()
		{
			Utils.UI.ShowLoadingIndicator();

			List<Advertisement> adList = await new AdvertisementModel().AdvertisementsGetById("HOSPLOY", string.Empty);
			if (adList != null) 
			{
				System.Diagnostics.Debug.WriteLine("Get advertisements success, count: " + adList.Count.ToString());
				Utils.UI.HideLoadingIndicator();
				this.rootView.LoadAdvertisements(adList);
			}
			else
			{
				Utils.UI.HideLoadingIndicator();
			}
		}

		public void RefreshLayout()
		{
			this.rootView.RefreshLayout(AppData.UserLoggedIn, AppData.Contact);
		}
		#endregion

		#region interface functions
		public void MemberInfoPressed()
		{
			if (AppData.UserLoggedIn)
			{
				var accountController = new AccountController(AppData.Contact,
					new AccountController.LogoutSuccessDelegate(delegate (Action<bool> dismissSelf)
						{
							dismissSelf(true);
						}
					)
				);

				this.NavigationController.PushViewController(accountController, true);

			}
			else
			{
				LoginDialogController loginDialogController = new LoginDialogController(
					new LoginDialogController.LoginSuccessDelegate(delegate (Action dismissSelf)
					{
						dismissSelf();
					})
				);
				this.NavigationController.PresentViewController(new UINavigationController(loginDialogController), true, null);
			}
		}

		public void AdvertisementPressed(Advertisement ad)
		{
			System.Diagnostics.Debug.WriteLine("Ad pressed: " + ad.Description);

			if (ad.AdType == AdvertisementType.None)
			{
				// Just an image, do nothing
			}
			else if (ad.AdType == AdvertisementType.ItemId)
			{
				if (AppData.MobileMenu == null)
					return;

				MenuItem menuItem = null;

				/*foreach (Menu menuToSearch in AppData.MobileMenu.Menus)
				{*/
				menuItem = menuService.GetMenuItem(AppData.MobileMenu, ad.AdValue, false);
					//if (menuItem != null)
						//break;
				//}

				if (menuItem == null)
				{
					System.Diagnostics.Debug.WriteLine("COULDN'T FIND MENUITEM: " + ad.AdValue);
					return;
				}

				// We have a MenuItem to display

				ItemDetailController detailsScreen = new ItemDetailController(menuItem);
				this.NavigationController.PushViewController(detailsScreen, true);
			}
			else if (ad.AdType == AdvertisementType.Deal)
			{
				if (AppData.MobileMenu == null)
					return;

				MenuItem menuItem = null;

				/*foreach (Menu menuToSearch in AppData.MobileMenu.Menus)
				{*/
				menuItem = menuService.GetMenuItem(AppData.MobileMenu, ad.AdValue, true);
				/*if (menuItem != null)
					break;
			}*/

				if (menuItem == null)
				{
					System.Diagnostics.Debug.WriteLine("COULDN'T FIND MENUITEM: " + ad.AdValue);
					return;
				}

				// We have a MenuItem to display

				ItemDetailController detailsScreen = new ItemDetailController(menuItem);
				this.NavigationController.PushViewController(detailsScreen, true);
			}
			else if (ad.AdType == AdvertisementType.MenuNodeId)
			{
				if (AppData.MobileMenu == null)
					return;

				MenuNode menuNode = null;

				foreach (Menu menuToSearch in AppData.MobileMenu.MenuNodes)
				{
					menuNode = menuService.GetMenuGroupNode(menuToSearch, ad.AdValue);//menuToSearch.GetMenuNode(ad.AdValue);
					if (menuNode != null)
						break;
				}

				if (menuNode == null)
				{
					System.Diagnostics.Debug.WriteLine("COULDN'T FIND MENUNODE: " + ad.AdValue);
					return;
				}

				// We have a MenuNode to display

				// NOTE: The MenuNode should always be a MenuGroupNode - if we want to advertise an item then we'd use an item advertisement
				if (!menuNode.NodeIsItem)
				{
					MenuCollectionController menuCollectionController = new MenuCollectionController(menuNode.Id);
					this.NavigationController.PushViewController(menuCollectionController, true);
				}
			}
			else if (ad.AdType == AdvertisementType.Url)
			{
				WebViewController webViewController = new WebViewController(ad.AdValue);
				this.NavigationController.PushViewController(webViewController, true);
			}
		}

		public void ShortcutCellSelected(HomeTableSource.ShortcutIds shortcutId)
		{
			System.Diagnostics.Debug.WriteLine("Selected shortcut: " + shortcutId.ToString());

			switch (shortcutId)
			{
				case HomeTableSource.ShortcutIds.Locations:
					this.NavigationController.PushViewController(new LocationsCardCollectionController(new UICollectionViewFlowLayout()), true);
					return;
				case HomeTableSource.ShortcutIds.Menu:
					this.NavigationController.PushViewController(new MenuCollectionController(), true);
					return;
				case HomeTableSource.ShortcutIds.OffersAndCoupons:
					this.NavigationController.PushViewController(new OffersAndCouponsCardCollectionController(new UICollectionViewFlowLayout()), true);
					return;
				case HomeTableSource.ShortcutIds.History:
					this.NavigationController.PushViewController(new HistoryController(), true);
					return;
				case HomeTableSource.ShortcutIds.Favorites:
					this.NavigationController.PushViewController(new FavouriteController(), true);
					return;
				default:
					return;
			}
		}

		#endregion
	}
}

