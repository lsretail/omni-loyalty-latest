using System;
using System.Collections.Generic;
using CoreGraphics;
using Domain.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using Presentation;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class TransactionDetailView : BaseView
	{
		private UITableView transDetailsTableView;
		private ITransactionDetailListeners listener;

		public TransactionDetailView(ITransactionDetailListeners listener)
		{
			this.BackgroundColor = UIColor.White;
			this.listener = listener;

			this.transDetailsTableView = new UITableView();
			this.transDetailsTableView.BackgroundColor = AppColors.BackgroundGray;
			this.transDetailsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.AddSubview(transDetailsTableView);	
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.transDetailsTableView.Frame = new CGRect(
				0,
				0,
				this.Frame.Width, 
				this.Frame.Height
			);
		}

		public void UpdateData(Transaction transactions, bool showQrCodeButton, bool showEditButton)
		{
			this.transDetailsTableView.Source = new TransactionDetailsTableSource(listener, transactions, showQrCodeButton, showEditButton);
		}

		public void RefreshData()
		{
			this.transDetailsTableView.ReloadData();
		}

		public void RefreshHeader(string transactionName)
		{
			(this.transDetailsTableView.Source as TransactionDetailsTableSource).RefreshHeader(transactionName);
		}

		public interface ITransactionDetailListeners
		{
			void ToggleFavoriteTransaction(Action onSuccess);
			bool IsTransactionFavorited();
			void AddTransactionToBasket();
			void EditTransactionButtonClicked();
			void ShowTransactionQRCode();
			void SaleLineSelected(int index);

			//Cell
			bool MenuItemToggleFavorite(int index);
			bool MenuItemCheckIfFavorite(int index);
			void MenuItemAddToBasket(int index);
		} 
	}
}

