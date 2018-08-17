using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using Presentation.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation
{
    public class TransactionHistoryTableSource : UITableViewSource
	{
		private List<LoyTransaction> transactions;
		public bool HasData { get { return this.transactions.Count > 0; } }

		public delegate void TransactionSelectedEventHandler (LoyTransaction transaction);
		public event TransactionSelectedEventHandler transactionSelected;

		public TransactionHistoryTableSource ()
		{
			this.transactions = new List<LoyTransaction>();
			RefreshData();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.transactions.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			TransactionHistoryCell cell = tableView.DequeueReusableCell (TransactionHistoryCell.Key) as TransactionHistoryCell;
			if (cell == null)
				cell = new TransactionHistoryCell ();

			LoyTransaction transaction = this.transactions[indexPath.Row];

			string locationString = transaction.Store.Description;

			//You could also display item count here
			/*
			string itemCountString = transaction.SaleLines.Count().ToString();
			if (transaction.SaleLines.Count() == 1)
				itemCountString += " " + LocalizationUtilities.LocalizedString("History_Item_Lowercase", "item");
			else
				itemCountString += " " + LocalizationUtilities.LocalizedString("History_Items_Lowercase", "items");
			*/

			cell.SetValues(indexPath.Row, transaction.DateToShortFormat, locationString, transaction.Amount);

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var transactionInCell = this.transactions[indexPath.Row];
            transactionSelected?.Invoke(transactionInCell);
			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 70f;
		}

		public void RefreshData()
		{
			if (AppData.UserLoggedIn)
				this.transactions = AppData.Device.UserLoggedOnToDevice.Transactions;

			//this.transactions = AppData.Transactions.OrderByDescending(x => x.Date).ToList();
		}
	}
}

