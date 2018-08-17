using System;
using System.Linq;
using UIKit;
using Presentation.Screens;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;

namespace Presentation
{
	public class FavoriteTransactionsTableSource : UITableViewSource
	{
		private readonly FavouriteView.IFavouritesListeners listener;

		public bool HasData { get { return this.listener.GetTransactions().Count > 0; } }

		public FavoriteTransactionsTableSource(FavouriteView.IFavouritesListeners listener)
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
			return this.listener.GetTransactions().Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			TransactionOverviewTableCell cell = tableView.DequeueReusableCell(TransactionOverviewTableCell.Key) as TransactionOverviewTableCell;
			if (cell == null)
				cell = new TransactionOverviewTableCell();

			Transaction favTransaction = this.listener.GetTransaction(indexPath.Row) as Transaction;

			string itemCountString = favTransaction.SaleLines.Count().ToString();
			if (favTransaction.SaleLines.Count() == 1)
				itemCountString += " " + LocalizationUtilities.LocalizedString("Favorites_Item_Lowercase", "item");
			else
				itemCountString += " " + LocalizationUtilities.LocalizedString("Favorites_Items_Lowercase", "items");

			cell.SetValues(indexPath.Row, favTransaction.Name, itemCountString, favTransaction.AmountForDisplay);

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var transactionInCell = this.listener.GetTransaction(indexPath.Row) as Transaction;
			this.listener.TransactionSelected(transactionInCell);

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 70f;
		}

		public void RefreshData()
		{
			listener.RefreshTransactionData();
		}
	}
}

