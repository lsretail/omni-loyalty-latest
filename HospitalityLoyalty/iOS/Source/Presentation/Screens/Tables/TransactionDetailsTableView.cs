using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
public class TransactionDetailsTableSource : UITableViewSource
	{
		private bool showQRCodeButton;
		private bool showEditButton;
		private Transaction transaction;
		private TransactionDetailView.ITransactionDetailListeners listener;

		private UIView headerView;
		private UILabel lblTransactionTitle;

		public TransactionDetailsTableSource(TransactionDetailView.ITransactionDetailListeners listener ,Transaction transaction,bool showQRCodeButton, bool showEditButton)
		{
			this.listener = listener;
			this.transaction = transaction;
			this.showQRCodeButton = showQRCodeButton;
			this.showEditButton = showEditButton;

			BuildHeaderView();
		}

		private void BuildHeaderView()
		{
			// NOTE: Have to keep the header view in memory, store it as an instance variable
			headerView = new UIView();
			headerView.BackgroundColor = AppColors.TransparentWhite;

			this.lblTransactionTitle = new UILabel();
			lblTransactionTitle.TextColor = AppColors.PrimaryColor;
			lblTransactionTitle.BackgroundColor = UIColor.Clear;
			lblTransactionTitle.Text = this.transaction.Name;
			headerView.AddSubview(lblTransactionTitle);

			UILabel lblPrice = new UILabel();
			lblPrice.TextColor = UIColor.Gray;
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.Text = transaction.AmountForDisplay;
			lblPrice.Font = UIFont.SystemFontOfSize(12f);
			headerView.AddSubview(lblPrice);

			UIButton btnFavorite = new UIButton();
			btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
			btnFavorite.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnFavorite.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			btnFavorite.BackgroundColor = UIColor.Clear;
			btnFavorite.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.ToggleFavoriteTransaction(
					() =>
					{
						//on success
						btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
					}
				);
			};
			headerView.AddSubview(btnFavorite);

			UIButton btnReorder = new UIButton();
			btnReorder.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.Gray), UIControlState.Normal);
			btnReorder.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnReorder.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			btnReorder.BackgroundColor = UIColor.Clear;
			btnReorder.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.AddTransactionToBasket();
			};
			if (!Util.AppDelegate.BasketEnabled)
				btnReorder.Hidden = true;
			headerView.AddSubview(btnReorder);

			UIButton btnQRCode = new UIButton();
			btnQRCode.SetImage(Utils.UI.GetColoredImage(Image.FromFile("/Icons/iconQRCodeWhite.png"), UIColor.Gray), UIControlState.Normal);
			btnQRCode.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnQRCode.ImageEdgeInsets = new UIEdgeInsets(2, 2, 2, 2);
			btnQRCode.BackgroundColor = UIColor.Clear;
			btnQRCode.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.ShowTransactionQRCode();
			};
			if (!this.showQRCodeButton)
				btnQRCode.Hidden = true;
			headerView.AddSubview(btnQRCode);

			UIButton btnEditName = new UIButton();
			btnEditName.SetImage(Utils.UI.GetColoredImage(Image.FromFile("/Icons/IconEdit.png"), UIColor.Gray), UIControlState.Normal);
			btnEditName.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnEditName.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			btnEditName.BackgroundColor = UIColor.Clear;
			btnEditName.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.EditTransactionButtonClicked();
			};
			if (!this.showEditButton)
				btnEditName.Hidden = true;
			headerView.AddSubview(btnEditName);

			const float buttonDimensions = 40f;

			headerView.ConstrainLayout(() =>

				lblTransactionTitle.Frame.Bottom == headerView.Bounds.GetCenterY() &&
				lblTransactionTitle.Frame.Left == headerView.Bounds.Left + 10f &&
				lblTransactionTitle.Frame.Right == btnFavorite.Frame.Left - 5f &&
				lblTransactionTitle.Frame.Height == 20f &&

				lblPrice.Frame.Top == lblTransactionTitle.Frame.Bottom &&
				lblPrice.Frame.Left == lblTransactionTitle.Frame.Left &&
				lblPrice.Frame.Right == lblTransactionTitle.Frame.Right &&
				lblPrice.Frame.Height == 20f &&

				btnQRCode.Frame.Top == btnFavorite.Frame.Top &&
				btnQRCode.Frame.Right == btnFavorite.Frame.Left &&
				btnQRCode.Frame.Width == buttonDimensions &&
				btnQRCode.Frame.Height == buttonDimensions &&

				//we should never be displaying QR-Code button and edit button at the same time

				btnEditName.Frame.Top == btnFavorite.Frame.Top &&
				btnEditName.Frame.Right == btnFavorite.Frame.Left &&
				btnEditName.Frame.Width == buttonDimensions &&
				btnEditName.Frame.Height == buttonDimensions &&

				btnFavorite.Frame.Top == btnReorder.Frame.Top &&
				btnFavorite.Frame.Right == btnReorder.Frame.Left &&
				btnFavorite.Frame.Width == buttonDimensions &&
				btnFavorite.Frame.Height == buttonDimensions &&

				btnReorder.Frame.GetCenterY() == headerView.Bounds.GetCenterY() &&
				btnReorder.Frame.Right == headerView.Bounds.Right &&
				btnReorder.Frame.Width == buttonDimensions &&
				btnReorder.Frame.Height == buttonDimensions

			);
		}

		private UIImage GetFavoriteButtonIcon()
		{
			if (this.listener.IsTransactionFavorited())
				return Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOnIcon"), UIColor.Red);
			else
				return Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), UIColor.Red);
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.transaction.SaleLines.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			TransactionDetailCell cell = tableView.DequeueReusableCell(TransactionDetailCell.Key) as TransactionDetailCell;
			if (cell == null)
				cell = new TransactionDetailCell(this.listener);

			SaleLine saleLine = this.transaction.SaleLines[indexPath.Row];
			string extraInfo = Util.GenerateItemExtraInfo(saleLine.Item);

			// Image
			ImageView imageView = saleLine.Item.Images.FirstOrDefault();
			string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
			string imageId = (imageView != null ? imageView.Id : string.Empty);

			cell.SetValues(indexPath.Row,saleLine.Item.Description, extraInfo, saleLine.Quantity.ToString(),
			               saleLine.Amount, imageAvgColor, imageId, this.listener.MenuItemCheckIfFavorite(indexPath.Row));

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			this.listener.SaleLineSelected(indexPath.Row);
			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return ItemOverviewCell.GetCellHeight(Util.GenerateItemExtraInfo(this.transaction.SaleLines[indexPath.Row].Item));
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			return this.headerView;
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			return 60f;
		}

		public void RefreshHeader(string transactionName)
		{
			lblTransactionTitle.Text = transactionName;
		}
	}
}

