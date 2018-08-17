using System.Collections.Generic;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class HistoryView : BaseView
	{
		private UITableView transactionTableView;
		private NoDataView noDataView;
		private IHistoryListeners listener;

		public HistoryView(IHistoryListeners listener)
		{
			this.listener = listener;
			this.transactionTableView = new UITableView();
			this.transactionTableView.BackgroundColor = AppColors.BackgroundGray;
			this.transactionTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.AddSubview(this.transactionTableView);

			this.noDataView = new NoDataView();
			this.noDataView.TextToDisplay = LocalizationUtilities.LocalizedString("History_NoData", "No previous transactions available");
			this.AddSubview(this.noDataView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.transactionTableView.Frame = new CGRect(
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);

			this.noDataView.TopLayoutGuideLength = this.TopLayoutGuideLength;
			this.noDataView.BottomLayoutGuideLength = this.BottomLayoutGuideLength;
			this.noDataView.Frame = new CGRect(
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);
		}

		public void UpdateData(List<Transaction> transactions)
		{
			if (transactions.Count > 0)
			{
				transactionTableView.Source = new TransactionHistoryTableSource(transactions, this.listener);
				this.transactionTableView.ReloadData();
				this.noDataView.Hidden = true;
			}
			else
			{
				this.noDataView.Hidden = false;
			}
		}

		public interface IHistoryListeners
		{
			void TransactionSelected(int index);
		}
	}
}

