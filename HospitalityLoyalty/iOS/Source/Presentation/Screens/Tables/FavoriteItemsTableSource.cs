using System;
using System.Linq;
using UIKit;
using Presentation.Screens;
using Foundation;
using Presentation.Utils;
using Presentation.Models;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
	public class FavoriteItemsTableSource : UITableViewSource
	{
		public bool HasData { get { return this.listener.GetItems().Count > 0; } }

		private readonly FavouriteView.IFavouritesListeners listener;


		public FavoriteItemsTableSource(FavouriteView.IFavouritesListeners listener)
		{
			this.listener = listener;
			RefreshData();
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.listener.GetItems().Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			MenuItem favMenuItem = this.listener.GetItem(indexPath.Row) as MenuItem;

			FavoriteItemCell cell = tableView.DequeueReusableCell(ItemOverviewCell.Key) as FavoriteItemCell;
			if (cell == null)
				cell = new FavoriteItemCell(listener);


			// Extra info
			//TODO laga
			string extraInfo = Util.GenerateItemExtraInfo(favMenuItem); ;//Utils.Util.GenerateItemExtraInfo(favMenuItem as Deal);//SlideoutBasket2.GenerateItemExtraInfo(favMenuItem);

			// Price
			decimal price = 0;

			if (favMenuItem is MenuDeal)
				price = (favMenuItem as MenuDeal).Price.Value;
			else if (favMenuItem is Product)
				price = (favMenuItem as Product).Price.Value;
			else if (favMenuItem is Recipe)
				price = (favMenuItem as Recipe).Price.Value;

			string formattedPrice = string.Empty;
			if (AppData.MobileMenu != null)
				formattedPrice = AppData.MobileMenu.Currency.FormatDecimal(price);
			else
				formattedPrice = price.ToString();

			// Image
			ImageView imageView = favMenuItem.Images.FirstOrDefault();
			string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
			string imageId = (imageView != null ? imageView.Id : string.Empty);

			cell.SetValues(indexPath.Row, favMenuItem.Description, extraInfo, "1", formattedPrice, imageAvgColor, imageId, CheckIfFavorited(indexPath.Row));

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var itemInCell = this.listener.GetItem(indexPath.Row) as MenuItem;
			this.listener.ItemSelected(itemInCell);

			tableView.DeselectRow(indexPath, true);
		}

		/*public void HandleAddToBasketButtonPress(int cellIndexPathRow)
		{
			this.listener.AddFavoriteToBasket(cellIndexPathRow);
		}

		public void HandleFavoriteButtonPress(int cellIndexPathRow)
		{
			this.listener.OnToggleFavourite(cellIndexPathRow);
		}*/

		public bool CheckIfFavorited(int cellIndexPathRow)
		{
			// When we un-favorite cells the favorite item has already been removed from the favoriteItems list
			// Doesn't matter since we remove unfavorited cells
			if (cellIndexPathRow > this.listener.GetItems().Count - 1)
				return false;

			MenuItem menuItem = this.listener.GetItem(cellIndexPathRow) as MenuItem;
			return new FavoriteModel().IsFavorite(menuItem);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return ItemOverviewCell.GetCellHeight(Util.GenerateItemExtraInfo((this.listener.GetItem(indexPath.Row) as MenuItem)));
		}

		public void RefreshData()
		{
			listener.RefreshItemData();
		}
	}
}

