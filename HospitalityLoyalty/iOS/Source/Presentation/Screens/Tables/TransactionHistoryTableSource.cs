using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Transactions;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class TransactionHistoryTableSource : UITableViewSource
	{
		private List<Transaction> transactions;
		HistoryView.IHistoryListeners listener;

		public bool HasData { get { return this.transactions.Count > 0; } }

		public TransactionHistoryTableSource(List<Transaction> transaction, HistoryView.IHistoryListeners listener)
		{
			this.transactions = transaction;
			this.listener = listener;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.transactions.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			TransactionOverviewTableCell cell = tableView.DequeueReusableCell(TransactionOverviewTableCell.Key) as TransactionOverviewTableCell;
			if (cell == null)
				cell = new TransactionOverviewTableCell();

			Transaction transaction = this.transactions[indexPath.Row];

			string itemCountString = transaction.SaleLines.Count.ToString();
			if (transaction.SaleLines.Count == 1)
				itemCountString += " " + LocalizationUtilities.LocalizedString("History_Item_Lowercase", "item");
			else
				itemCountString += " " + LocalizationUtilities.LocalizedString("History_Items_Lowercase", "items");

			cell.SetValues(indexPath.Row, transaction.DateToShortFormat, itemCountString, transaction.AmountForDisplay);

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var transactionInCell = this.transactions[indexPath.Row];
			listener.TransactionSelected (indexPath.Row);

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 70f;
		}
	}
}

