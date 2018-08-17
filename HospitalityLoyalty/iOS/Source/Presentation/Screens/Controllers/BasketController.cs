using System;
using System.Linq;
using Foundation;
using Presentation.Models;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
	public class BasketController : UIViewController, BasketView.IBasketListeners
	{
		private BasketView rootView;

		public BasketController()
		{
			this.Title = LocalizationUtilities.LocalizedString("Basket_YourOrder", "Your order");
			this.rootView = new BasketView (this);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			this.rootView.Refresh(GetFormattedOrderTotalString());
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.View = this.rootView;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		public void ItemClicked(int index)
		{
			EditBasketItemController editController = new EditBasketItemController(AppData.Basket.Items[index], Refresh);
			editController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
			this.PresentViewController(new UINavigationController(editController), true, null);	
		}

		public bool ToggleFavorite(int index, BasketView.BasketType type)
		{
			var favoriteModel = new FavoriteModel();
			System.Diagnostics.Debug.WriteLine("Favoriting... " + AppData.Basket.Items[index].Item.Description);

			if (type == BasketView.BasketType.Item)
			{
				favoriteModel.ToggleFavorite(AppData.Basket.Items[index].Item);
				this.rootView.tblBasket.ReloadData();
				return favoriteModel.IsFavorite(AppData.Basket.Items[index].Item);
			}
			/*else if (type == BasketView.BasketType.Offer)
			{
				favoriteModel.ToggleFavorite(AppData.SelectedPublishedOffers[index]);
				this.rootView.tblBasket.ReloadData();
				return favoriteModel.IsFavorite(AppData.SelectedPublishedOffers[index]);
			}*/
			return false;
		}

		public async void RemoveItemFromBasket(int index, BasketView.BasketType basketType)
		{
			System.Diagnostics.Debug.WriteLine("Deleting row of type " + basketType.ToString() + " with index path " + index);

			var alertResult = await AlertView.ShowAlert(
				this,
				LocalizationUtilities.LocalizedString("Checkout_RemoveItem", "Remove item"),
				LocalizationUtilities.LocalizedString("Checkout_AreYouSureRemoveItem", "Are you sure you want to remove this item?"),
				LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
				LocalizationUtilities.LocalizedString("General_No", "No")
			);

			if (alertResult == AlertView.AlertButtonResult.PositiveButton)
			{
				if (basketType == BasketView.BasketType.Item)
				{
					new BasketModel().RemoveItemFromBasket(AppData.Basket.Items[index].Id);
					Refresh();
				}
				else if (basketType == BasketView.BasketType.Offer)
				{
					PublishedOffer publishedOffer = AppData.SelectedPublishedOffers[index];
					new BasketModel().TogglePublishedOffer(publishedOffer);
					Refresh();
				}
			}
			else if (alertResult == AlertView.AlertButtonResult.NegativeButton)
			{
			}
		}

		public bool IsFavorite(int index)
		{ 
			return new FavoriteModel().IsFavorite(AppData.Basket.Items[index].Item);
		}

		public void Refresh()
		{
			this.rootView.Refresh(GetFormattedOrderTotalString());
		}

		public void ChekoutButtonClicked()
		{
			CheckoutController checkoutController = new CheckoutController();
			this.PresentViewController(new UINavigationController(checkoutController), true, null);
		}

		private string GetFormattedOrderTotalString()
		{
			string formattedCurrencyPriceString = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal(AppData.Basket.Amount) : AppData.Basket.Amount.ToString();
			return formattedCurrencyPriceString;
		}
	}
}