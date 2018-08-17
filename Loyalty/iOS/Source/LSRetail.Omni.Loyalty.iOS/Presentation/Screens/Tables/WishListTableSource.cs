using System;
using UIKit;
using Presentation.Utils;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class WishListTableSource : UITableViewSource
	{			
		private OneList wishList 
		{ 
			get 
			{ 
				if (AppData.UserLoggedIn)
					return AppData.Device.UserLoggedOnToDevice.WishList;
				else
					return new OneList();
			} 
		}
		public bool HasData { get { return this.wishList.Items.Count > 0; } }

		public delegate void AddItemToBasketEventHandler(OneListItem itemToAdd);
		public AddItemToBasketEventHandler AddItemToBasket;

		public delegate void ItemSelectedEventHandler(OneListItem item);
		public ItemSelectedEventHandler ItemSelected;

		public delegate void RemoveItemFromWishListEventHandler(int itemPosition);
		public RemoveItemFromWishListEventHandler RemoveItemFromWishList;

		public WishListTableSource ()
		{}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.wishList.Items.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			WishListTableViewCell cell = tableView.DequeueReusableCell (WishListTableViewCell.Key) as WishListTableViewCell;
			if (cell == null)
				cell = new WishListTableViewCell();

			OneListItem wishListItem = this.wishList.Items[indexPath.Row];

			// Extra info 
            string extraInfo = wishListItem.VariantReg != null ? wishListItem.VariantReg.ToString() : string.Empty;

			// Price
			string formattedPrice = string.Empty;

            var mon = new Money(wishListItem.Price, AppData.Device.UserLoggedOnToDevice.Environment.Currency);
            formattedPrice = mon.RoundForDisplay(true);

			// Image
			ImageView imageView = wishListItem.Image;
			string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
			string imageId = (imageView != null ? imageView.Id : string.Empty);

			cell.SetValues(
				indexPath.Row,
				wishListItem.Item.Description, 
				extraInfo, 
				wishListItem.Quantity.ToString(), 
				formattedPrice, 
				imageAvgColor, 
				imageId
			);

			cell.AddToBasket = HandleAddToBasketButtonPress;
			cell.RemoveItemFromWishList = HandleRemoveItemFromWishListButtonPress;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var wishListItem = this.wishList.Items[indexPath.Row];

			if (this.ItemSelected != null)
				this.ItemSelected(wishListItem);

			tableView.DeselectRow(indexPath, true);
		}

		public void HandleAddToBasketButtonPress(int cellIndexPathRow)
		{
			var wishListItem = this.wishList.Items[cellIndexPathRow];

			if (this.AddItemToBasket != null)
				this.AddItemToBasket(wishListItem);
		}

		public void HandleRemoveItemFromWishListButtonPress(int cellIndexPathRow)
		{				
			if (this.RemoveItemFromWishList != null)
				this.RemoveItemFromWishList(cellIndexPathRow);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{				
			return 100;
		}
	}
}

