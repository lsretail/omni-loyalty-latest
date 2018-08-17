using System;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using System.Collections.Generic;
using Presentation.Utils;
using Domain.Transactions;
using Domain.Menus;
using Presentation.Models;

namespace Presentation.Screens
{
	/*
	public class TransactionDetailsTableSource : UITableViewSource
	{
		private bool showQRCodeButton;
		private bool showEditButton;
		private List<SaleLine> saleLines;

		private UIView headerView;
		private UILabel lblTransactionTitle;

		public TransactionDetailsTableSource (TransactionDetailsScreen controller, bool showQRCodeButton, bool showEditButton)
		{
			this.saleLines = this.controller.Transaction.SaleLines;

			this.showQRCodeButton = showQRCodeButton;
			this.showEditButton = showEditButton;

			BuildHeaderView();
		}

		private void BuildHeaderView()
		{
			// NOTE: Have to keep the header view in memory, store it as an instance variable

			headerView = new UIView();
			headerView.BackgroundColor = Utils.AppColors.TransparentWhite;

			this.lblTransactionTitle = new UILabel();
			lblTransactionTitle.TextColor = Utils.AppColors.PrimaryColor;
			lblTransactionTitle.BackgroundColor = UIColor.Clear;
			lblTransactionTitle.Text = this.controller.Transaction.Name;
			headerView.AddSubview(lblTransactionTitle);

			UILabel lblPrice = new UILabel();
			lblPrice.TextColor = UIColor.Gray;
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.Text = this.controller.Transaction.AmountForDisplay;
			lblPrice.Font = UIFont.SystemFontOfSize(12f);
			headerView.AddSubview(lblPrice);

			UIButton btnFavorite = new UIButton();
			btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
			btnFavorite.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnFavorite.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			btnFavorite.BackgroundColor = UIColor.Clear;
			btnFavorite.TouchUpInside += (object sender, EventArgs e) => { 
			
				this.controller.ToggleFavoriteTransaction(
					() =>
					{
						//on success
						btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);	
					}
				);
			};
			headerView.AddSubview(btnFavorite);

			UIButton btnReorder = new UIButton();
			btnReorder.SetImage(Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconShoppingBasketAdd.png"), UIColor.Gray), UIControlState.Normal);
			btnReorder.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnReorder.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			btnReorder.BackgroundColor = UIColor.Clear;
			btnReorder.TouchUpInside += (object sender, EventArgs e) => { 
			
				this.controller.AddTransactionToBasket();
			
			};
			if (!Utils.Util.AppDelegate.BasketEnabled)
				btnReorder.Hidden = true;
			headerView.AddSubview(btnReorder);

			UIButton btnQRCode = new UIButton();
			btnQRCode.SetImage(Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/iconQRCodeWhite.png"), UIColor.Gray), UIControlState.Normal);
			btnQRCode.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnQRCode.ImageEdgeInsets = new UIEdgeInsets(2, 2, 2, 2);
			btnQRCode.BackgroundColor = UIColor.Clear;
			btnQRCode.TouchUpInside += (object sender, EventArgs e) => { 

				this.controller.ShowTransactionQRCodeScreen();

			};
			if (!this.showQRCodeButton)
				btnQRCode.Hidden = true;
			headerView.AddSubview(btnQRCode);

			UIButton btnEditName = new UIButton();
			btnEditName.SetImage(Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconEdit.png"), UIColor.Gray), UIControlState.Normal);
			btnEditName.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnEditName.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			btnEditName.BackgroundColor = UIColor.Clear;
			btnEditName.TouchUpInside += (object sender, EventArgs e) => { 

				this.controller.EditTransaction();

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
			if (this.controller.IsTransactionFavorited())
				return Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconFavoriteON.png"), UIColor.Red);
			else
				return Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconFavoriteOFF.png"), UIColor.Red);
		}
			
		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.saleLines.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			ItemOverviewCell cell = tableView.DequeueReusableCell (ItemOverviewCell.Key) as ItemOverviewCell;
			if (cell == null)
				cell = new ItemOverviewCell();
				
			SaleLine saleLine = this.saleLines[indexPath.Row];

			// Extra info
			//TODO Laga
			string extraInfo = "TODO laga"; //SlideoutBasket2.GenerateItemExtraInfo(saleLine.Item);

			// Image
			Domain.Images.ImageView imageView = saleLine.Item.Images.FirstOrDefault();
			string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
			string imageId = (imageView != null ? imageView.Id : string.Empty);

			cell.SetValues(indexPath.Row, HandleAddToBasketButtonPress, HandleFavoriteButtonPress, CheckIfFavorited,
				saleLine.Item.Description, extraInfo, saleLine.Quantity.ToString(), saleLine.Amount, imageAvgColor, imageId);
				
			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var saleLineInCell = this.saleLines[indexPath.Row];
			this.controller.CellSelected(saleLineInCell);

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			//TODO laga
			return 30f;
			//return ItemOverviewTableViewCell.GetCellHeight(SlideoutBasket2.GenerateItemExtraInfo(this.saleLines[indexPath.Row].Item));
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			return this.headerView;
		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return 60f;
		}

		public void HandleAddToBasketButtonPress(int cellIndexPathRow)
		{
			SaleLine saleLine = this.saleLines[cellIndexPathRow] as SaleLine;
			this.controller.AddSaleLineToBasket(saleLine);
		}

		public void HandleFavoriteButtonPress(int cellIndexPathRow)
		{
			SaleLine saleLine = this.saleLines[cellIndexPathRow] as SaleLine;
			this.controller.ToggleFavoriteSaleLine(saleLine);
		}

		public bool CheckIfFavorited(int cellIndexPathRow)
		{
			SaleLine saleLine = this.saleLines[cellIndexPathRow] as SaleLine;
			return new FavoriteModel().IsFavorite(saleLine.Item);
		}

		public void RefreshHeader()
		{
			lblTransactionTitle.Text = this.controller.Transaction.Name;
		}
	}
	*/
}

