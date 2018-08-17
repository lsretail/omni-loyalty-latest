using System;
using System.Linq;
using Presentation.Utils;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Infrastructure.Data.SQLite2.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Models
{
	public class BasketModel : BaseModel
	{
		private LocalBasketService localBasketService;
		private BasketRepository basketRepository;
		public BasketModel()
		{
			basketRepository = new BasketRepository();
			this.localBasketService = new LocalBasketService();
		}

		public void AddItemToBasket(MenuItem item, decimal qty)
		{

			AppData.Basket.AddItemToBasket(
				new BasketItem()
				{
					Item = item,
					Quantity = qty
				}
			);

			CalculateBasket();
		}
			
		/// <summary>
		/// Adds the sale line to basket.
		/// </summary>
		/// <returns><c>true</c>, if sale line was added to basket, <c>false</c> otherwise.</returns>
		/// <param name="saleLine">Sale line.</param>
		private bool AddSaleLineToBasket(SaleLine saleLine)
		{
			// TODO Have to verify that you can actually reorder the transaction item more vigorously
			// Or should we just let the backend handle this and throw us an error?
			// The price might also have changed ???

			/* NOT IN USE BUT WORKS - BASIC CHECK IF ITEM AND MENU ARE STILL AVAILABLE
			// Have to check that the saleline (item and menu) is still available ...
			// Let's check if the menu that the menuitem in the saleline was a part of still contains said menuitem.

			// This is just a very shallow check, we use IDs to check this.
			// It is possible that the transactionItem has some modifiers that aren't available anymore, 
			// might even be a completely different item behind the itemId.
			// The item could also have been moved to a different menuID, and still be available there?

			bool itemAvailable = false;

			Menu menu = Utils.Util.AppDelegate.AppData.MobileMenu.Menus.Where(x => x.Id == saleLine.Item.MenuId).FirstOrDefault();

			if (menu != null)
			{
				if (menu.GetMenuItem(saleLine.Item.Id) != null)
					itemAvailable = true;
			}

			if (itemAvailable)
			{
				AddItemToBasket(saleLine.Item.Clone(), saleLine.Quantity);
				return true;
			}
			else
			{
				return false;
			}
			*/

			AddItemToBasket(saleLine.Item, saleLine.Quantity);
			return true;
		}

		public void AddSaleLineToBasket(SaleLine saleLine, Action onSuccess, Action onFailure)
		{
			if (AddSaleLineToBasket(saleLine))
				onSuccess();
			else
				onFailure();
		}

		public void AddTransactionToBasket(Transaction transaction, Action onSuccess, Action onFailure)
		{
			bool success = true;

			// BasketItems are added to the front of the basket list, so the oldest basketitem is at the bottom.
			// The saleline list is also stored this way. Let's add it in reverse to preserve the order (start with the oldest item).
			for (int i = transaction.SaleLines.Count - 1; i >= 0 ; i--)
			{
				if (!AddSaleLineToBasket(transaction.SaleLines[i]))
					success = false;
			}

			if (success)
				onSuccess();
			else
				onFailure();
		}

		public void UpdateBasketItem(string basketItemId, MenuItem item, decimal qty)
		{
			var basketItem = AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId);

			basketItem.Item = item;
			basketItem.Quantity = qty;

			CalculateBasket();
		}

		public void RemoveItemFromBasket(string basketItemId)
		{
			RemoveItemFromBasket(AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId));
		}

		public void RemoveItemFromBasket(BasketItem item)
		{
			AppData.Basket.RemoveItemFromBasket(item);
			CalculateBasket();
		}

		/*
		public void AddCouponToBasket(Coupon coupon)
		{
			Utils.Util.AppDelegate.AppData.Basket.AddCouponToBasket(coupon);
		}

		public void RemoveCouponFromBasket(Coupon coupon)
		{
			Utils.Util.AppDelegate.AppData.Basket.RemoveCouponFromBasket(coupon);
		}
		*/

		public void TogglePublishedOffer(string id)
		{
			PublishedOffer item = AppData.Contact.PublishedOffers.FirstOrDefault(x => x.Id == id);
			TogglePublishedOffer(item);
		}

		public void TogglePublishedOffer(PublishedOffer publishedOffer)
		{
			publishedOffer.Selected = !publishedOffer.Selected;
		}
			
		public void ChangeQty(string basketItemId, decimal qty)
		{
			ChangeQty(AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId), qty);
		}

		public void ChangeQty(BasketItem item, decimal qty)
		{
			item.Quantity = qty;
			CalculateBasket();
		}

		public void ClearBasket()
		{
			AppData.Basket.Clear();
			// Let's also deselect all coupons/offers (we're not using the coupon list in the basket, but rather selecting coupons/offers that the contact has)
			if (AppData.SelectedPublishedOffers != null)
				AppData.SelectedPublishedOffers.ForEach(x => x.Selected = false);

			CalculateBasket();
		}
			
		public async void CalculateBasket()
		{
			Basket basket = AppData.Basket;
			this.localBasketService.CalculateBasket(AppData.MobileMenu, basket);
			await SaveBasketLocally();
		}

		private async Task SaveBasketLocally()
		{
			try
			{
				Basket basket = await this.localBasketService.SyncBasketAsync(basketRepository, AppData.Basket);

				System.Diagnostics.Debug.WriteLine("Basket synced locally");
			}
				
			catch(Exception exception) 
			{
				HandleException(exception, "BasketModel.SaveBasketLocally()", false);
			}

		}

		public async void LoadLocalBasket()
		{
			try
			{
				Basket basket = await this.localBasketService.GetBasketAsync(basketRepository, AppData.MobileMenu);
					
					if (basket != null)
					{
						AppData.Basket = basket;
						CalculateBasket();
						System.Diagnostics.Debug.WriteLine("Local basket fetched successfully");
					}
			}

												   
			catch(Exception exception) 
			{
				HandleException(exception, "BasketModel.LoadLocalBasket()", false);
			}

		}

	}
}