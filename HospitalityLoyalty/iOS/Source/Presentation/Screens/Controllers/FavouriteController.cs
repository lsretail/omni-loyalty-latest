using System;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using System.Collections.Generic;
using Presentation.Utils;
using Domain.Transactions;
using Presentation.Models;
using Presentation.Screens;
using System.Timers;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;

namespace Presentation
{
	public class FavouriteController : UIViewController, FavouriteView.IFavouritesListeners
	{
		private FavouriteView rootView;
		private UIImageView navBarHairLine;
		private List<IFavorite> favoriteItems;
		private List<IFavorite> favoriteTransactions;

		#region Constructor
		public FavouriteController()
		{
			this.Title = LocalizationUtilities.LocalizedString("Favorites_Favorites", "Favorites");

			rootView = new FavouriteView(this);
			SetRightBarButtonItems();
		}
		#endregion

		#region overrides
		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
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

			this.navBarHairLine = GetNavBarHairLineImageView(this.NavigationController.NavigationBar);
			this.navBarHairLine.Hidden = true;
			rootView.ReloadTables();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			// Navigationbar
			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes(false);
			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;

			this.rootView.RefreshNoDataView();
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			// Attempt to fix 'random' segfaulting error by nulling the tableview sources
			this.rootView.EmptyTables();

			this.navBarHairLine.Hidden = false;

			// TODO: find a way to dispose the view early; maybe with an overload constructor receiving the boolean parameter 'disposable'? If the view is disposable, we dispose it?
			// Note: we don't want the view created within RootTabController to be disposed because it is reused every time the user taps the option menu
			//if ((IsMovingFromParentViewController || IsBeingDismissed) && ParentViewController == null)
			//{
			//	if (View != null)
			//	{
			//		View.Dispose();
			//		View = null;
			//		this.rootView = null;
			//	}
			//}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.View.BackgroundColor = UIColor.White;
			this.navBarHairLine = GetNavBarHairLineImageView(this.NavigationController.NavigationBar);

			this.NavigationController.Toolbar.BarTintColor = AppColors.PrimaryColor;
			this.View = rootView;
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
		}
		#endregion

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			/*if (Utils.Util.AppDelegate.BasketEnabled)
				barButtonItemList.Add(Utils.UI.GetBasketBarButtonItem());*/

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		public void AddFavoriteToBasket(int index, bool isItem)
		{
			var favorite = isItem ? favoriteItems[index] : favoriteTransactions[index];

			if (favorite is MenuItem)
			{
				new Models.BasketModel().AddItemToBasket((favorite as MenuItem).Clone(), 1);
				//Utils.Util.AppDelegate.SlideoutBasket.Refresh();

				Utils.UI.bannerViewTimer.Start();
				Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Utils.Image.FromFile("/Branding/Standard/default_map_location_image.png"));
			}
			else
			{
				new Models.BasketModel().AddTransactionToBasket((favorite as Transaction).Clone(), () =>
				{
					Utils.UI.bannerViewTimer.Start();
					Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Utils.Image.FromFile("/Branding/Standard/default_map_location_image.png"));
				}, () => 
				{
					Utils.UI.bannerViewTimer.Start();
					Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("TransactionDetails_ErrorAddingItemToBasket", "Vara bættist ekki í körfuna"), Utils.Image.FromFile("/Branding/Standard/default_map_location_image.png"));
				});
				//Utils.Util.AppDelegate.SlideoutBasket.Refresh();

			}
		}

		public void ItemSelected(MenuItem item)
		{
			ItemDetailController detailsScreen = new ItemDetailController(item.Clone());
			this.NavigationController.PushViewController(detailsScreen, true);
		}

		public void TransactionSelected(Transaction transaction)
		{
			TransactionDetailController transactionDetailsScreen = new TransactionDetailController(transaction, true);
			this.NavigationController.PushViewController(transactionDetailsScreen, true);
		}

		public bool IsFavourite(IFavorite item)
		{
			return new FavoriteModel().IsFavorite(item);
		}

		public bool IsSaleLineFavourite(SaleLine item)
		{
			return new FavoriteModel().IsFavorite(item.Item);
		}

		public void ToggleFavourite(IFavorite item)
		{
			new FavoriteModel().ToggleFavorite(item);
		}

		public void OnToggleFavourite(int index, bool isItem)
		{
			if (isItem)
				rootView.ToggleFavorite(favoriteItems[index]);
			else
				rootView.ToggleFavorite(favoriteTransactions[index]);
		}

		public void RefreshItemData()
		{
			List<IFavorite> favoriteItems = AppData.Favorites.Where(fav => fav is MenuItem).ToList();
			favoriteItems.Reverse();
			this.favoriteItems = favoriteItems;
		}

		public void RefreshTransactionData()
		{
			List<IFavorite> favoriteTransactions = AppData.Favorites.Where(fav => fav is Transaction).ToList();
			favoriteTransactions.Reverse();
			this.favoriteTransactions = favoriteTransactions;
		}

		public List<IFavorite> GetItems()
		{
			return favoriteItems;
		}

		public IFavorite GetItem(int index)
		{
			return favoriteItems[index];
		}

		public List<IFavorite> GetTransactions()
		{
			return favoriteTransactions;
		}

		public IFavorite GetTransaction(int index)
		{
			return favoriteTransactions[index];
		}

		// Finds the small grey seperator view at the bottom of the NavBar so we can hide it
		private UIImageView GetNavBarHairLineImageView(UIView view)
		{
			if (view is UIImageView && (view as UIImageView).Bounds.Size.Height <= 1f)
			{
				return view as UIImageView;
			}

			foreach (UIView subView in view.Subviews)
			{
				UIImageView imageView = GetNavBarHairLineImageView(subView);

				if (imageView != null)
				{
					return imageView;
				}
			}

			return null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (View != null)
				{
					View.Dispose();
					View = null;
					this.rootView = null;
				}
			}

			base.Dispose(disposing);
		}
	}
}