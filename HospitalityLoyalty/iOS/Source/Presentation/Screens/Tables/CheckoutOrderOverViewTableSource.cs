using System;
using System.Linq;
using Domain.Transactions;
using Foundation;
using Presentation.Models;
using Presentation.Utils;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
	public class CheckoutOrderOverViewTableSource : UITableViewSource
	{
		private UIView headerView;
		private UIView footerView;

		private readonly CheckoutOrderOverView.ICheckoutOrderOverViewListener listener;

		private Transaction currTransaction; // For favorites

		public CheckoutOrderOverViewTableSource(CheckoutOrderOverView.ICheckoutOrderOverViewListener listener)
		{
			this.listener = listener;

			currTransaction = new TransactionModel().CreateTransaction();

			BuildHeaderView();
			BuildFooterView();

		}

		private void BuildHeaderView()
		{
			headerView = new UIView();
			headerView.BackgroundColor = Utils.AppColors.TransparentWhite;

			UILabel lblVerify = new UILabel()
			{
				Text = LocalizationUtilities.LocalizedString("Checkout_Verify", "Please verify your order"),
				Lines = 0,
				TextColor = AppColors.PrimaryColor,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize(16)
			};
			lblVerify.SizeToFit();
			headerView.AddSubview(lblVerify);

			const float margin = 5f;

			headerView.ConstrainLayout(() =>
				lblVerify.Frame.Top == headerView.Frame.Top + 2 * margin &&
				lblVerify.Frame.Width == headerView.Frame.Width - 2 * margin &&
				lblVerify.Frame.GetCenterX() == headerView.Frame.GetCenterX()
			);
		}

		private void BuildFooterView()
		{
			footerView = new UIView();
			footerView.BackgroundColor = UIColor.Clear;

			UIView containerView = new UIView();
			containerView.BackgroundColor = Utils.AppColors.TransparentWhite;
			footerView.AddSubview(containerView);

			UIView containerBtnView = new UIView();
			containerBtnView.BackgroundColor = UIColor.Clear;
			footerView.AddSubview(containerBtnView);

			UILabel lblPrice = new UILabel();
			lblPrice.TextColor = Utils.AppColors.PrimaryColor;
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.Text = GetFormattedOrderTotalString();
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.TextAlignment = UITextAlignment.Right;
			lblPrice.TranslatesAutoresizingMaskIntoConstraints = false;
			lblPrice.Tag = 100;
			containerView.AddSubview(lblPrice);

			UIButton btnFavorite = new UIButton();
			btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
			btnFavorite.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnFavorite.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			btnFavorite.BackgroundColor = UIColor.Clear;
			btnFavorite.TranslatesAutoresizingMaskIntoConstraints = false;
			btnFavorite.TouchUpInside += async (object sender, EventArgs e) =>
			{
				//Transaction transactionClone = currTransaction.Clone();

				if (new FavoriteModel().IsFavorite(currTransaction))
				{
					new FavoriteModel().ToggleFavorite(currTransaction);
					btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);

					//Utils.Util.AppDelegate.SlideoutMenu.FavoritesScreen.RefreshWithAnimation();
				}
				else
				{
					var alertResult = await AlertView.ShowAlertWithTextInput(
						null,
						LocalizationUtilities.LocalizedString("SlideoutBasket_NameTransaction", "Name transaction"),
						string.Empty,
						LocalizationUtilities.LocalizedString("SlideoutBasket_EnterName", "Enter a name (optional)"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK"),
						LocalizationUtilities.LocalizedString("General_Cancel", "Cancel")
					);

					if (alertResult.ButtonResult == AlertView.AlertButtonResult.PositiveButton)
					{
						currTransaction.Name = alertResult.TextInput.Trim();
						new FavoriteModel().ToggleFavorite(currTransaction);
						btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
						//Utils.Util.AppDelegate.SlideoutMenu.FavoritesScreen.RefreshWithAnimation();
					}
					else if (alertResult.ButtonResult == AlertView.AlertButtonResult.NegativeButton)
					{
						//do nothing
					}
				}
			};
			containerView.AddSubview(btnFavorite);

			UIButton btnSendOrder = new UIButton();
			btnSendOrder.SetTitle(LocalizationUtilities.LocalizedString("Checkout_PlaceOrder", "Place order"), UIControlState.Normal);
			btnSendOrder.SetTitleColor(UIColor.White, UIControlState.Normal);
			btnSendOrder.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnSendOrder.Layer.CornerRadius = 2;
			btnSendOrder.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.SendOrder();
			};
			containerBtnView.AddSubview(btnSendOrder);

			const float buttonDimensions = 40f;

			footerView.ConstrainLayout(() =>

			   containerView.Frame.Left == footerView.Frame.Left &&
			   containerView.Frame.Right == footerView.Frame.Right &&
			   containerView.Frame.Top == footerView.Frame.Top + 10f &&
			   containerView.Frame.Height == buttonDimensions &&

			   containerBtnView.Frame.Top == containerView.Bounds.Bottom &&
			   containerBtnView.Frame.Left == footerView.Frame.Left &&
			   containerBtnView.Frame.Right == footerView.Frame.Right &&
			   containerBtnView.Frame.Bottom == footerView.Bounds.Bottom &&

			   lblPrice.Frame.Top == containerView.Bounds.Top &&
			   lblPrice.Frame.Right == containerView.Bounds.Right - 10f &&
			   lblPrice.Frame.Left == btnFavorite.Frame.Right &&
			   lblPrice.Frame.Bottom == containerView.Bounds.Bottom &&

			   btnFavorite.Frame.Left == containerView.Bounds.Left &&
			   btnFavorite.Frame.Top == containerView.Bounds.Top &&
			   btnFavorite.Frame.Bottom == containerView.Bounds.Bottom &&
			   btnFavorite.Frame.Width == buttonDimensions &&

			   btnSendOrder.Frame.Top == containerBtnView.Bounds.Top + 20f &&
			   btnSendOrder.Frame.Left == containerBtnView.Bounds.Left + 20f &&
			   btnSendOrder.Frame.Right == containerBtnView.Bounds.Right - 20f &&
			   btnSendOrder.Frame.Height == 40f
			  );

		}

		private string GetFormattedOrderTotalString()
		{
			string formattedCurrencyPriceString = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal(AppData.Basket.Amount) : AppData.Basket.Amount.ToString();
			return LocalizationUtilities.LocalizedString("Checkout_Total", "Total") + ": " + formattedCurrencyPriceString;
		}

		private UIImage GetFavoriteButtonIcon() 
		{
			if (new FavoriteModel().IsFavorite(currTransaction))
				return Utils.UI.GetColoredImage((UIImage.FromBundle("FavoriteOnIcon")), UIColor.Red);
			else
				return Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), UIColor.Red);
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 3 + 1;   // One dummy section that we put our footer in
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			if (section == (int)Sections.Items)
			{
				return AppData.Basket.Items.Count;
			}
			else if (section == (int)Sections.Offers)
			{
				// TODO: We're just using offers that the contact has, should be able to use general offers not connected to contact?
				return (AppData.SelectedPublishedOffers != null ? AppData.SelectedPublishedOffers.Count : 0);
			}
			else
			{
				return 0;
			}
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			CheckoutOrderOverViewCell cell = tableView.DequeueReusableCell(CheckoutOrderOverViewCell.Key) as CheckoutOrderOverViewCell;
			if (cell == null)
				cell = new CheckoutOrderOverViewCell(this.listener);

			// Set default values
			string description = string.Empty;
			string extraInfo = string.Empty;
			string quantity = string.Empty;
			string formattedPriceString = string.Empty;
			string imageAvgColor = string.Empty;
			string imageId = string.Empty;
			bool isFavorited = false;
			CheckoutOrderOverViewCell.CellType cellType = CheckoutOrderOverViewCell.CellType.Item;

			if (indexPath.Section == (int)Sections.Items)
			{
				BasketItem basketItem = AppData.Basket.Items[indexPath.Row];

				cellType = CheckoutOrderOverViewCell.CellType.Item;

				description = basketItem.Item.Description;
				extraInfo = Utils.Util.GenerateItemExtraInfo(basketItem.Item);
				quantity = basketItem.Quantity.ToString();
				formattedPriceString = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal(basketItem.Item.Price.Value) : basketItem.Item.Price.ToString();

				isFavorited = this.listener.BasketItemCheckIfFavorite(indexPath.Row);

				ImageView imageView = basketItem.Item.Images.FirstOrDefault();
				imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
				imageId = (imageView != null ? imageView.Id : string.Empty);
			}
			else if (indexPath.Section == (int)Sections.Offers)
			{
				PublishedOffer publishedOffer = AppData.SelectedPublishedOffers[indexPath.Row];

				cellType = CheckoutOrderOverViewCell.CellType.Offer;

				description = publishedOffer.Description;
				quantity = "1";

				ImageView imageView = publishedOffer.Images.Count > 0 ? publishedOffer.Images[0] : null;
				imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
				imageId = (imageView != null ? imageView.Id : string.Empty);
			}

			cell.SetValues(indexPath.Row, description, extraInfo, quantity, formattedPriceString, imageAvgColor, imageId, isFavorited, cellType);

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			BasketItem basketItem = Utils.AppData.Basket.Items[indexPath.Row];
			this.listener.BasketItemPressed(basketItem);

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == (int)Sections.Items)
				return CheckoutOrderOverViewCell.GetCellHeight(Utils.Util.GenerateItemExtraInfo(Utils.AppData.Basket.Items[indexPath.Row].Item));
			else
				return CheckoutOrderOverViewCell.GetCellHeight(string.Empty);
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			if (section == 0)
				return this.headerView;
			else
				return null;
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			if (section == 0)
				return 38f;
			else
				return 0f;
		}

		public override UIView GetViewForFooter(UITableView tableView, nint section)
		{
			// TODO The footerview should really be a footerview for the entire table, not just the last section.+
			// Should never be able to scroll the footerview off screen.
			if (section == NumberOfSections(tableView) - 2)
				return this.footerView;
			else
				return null;
		}

		public override nfloat GetHeightForFooter(UITableView tableView, nint section)
		{
			if (section == NumberOfSections(tableView) - 2)
				return 44f + 40f + 44f;
			else
				return 0f;
		}

		public bool CheckIfFavorited(int cellIndexPathRow)
		{
			BasketItem basketItem = Utils.AppData.Basket.Items[cellIndexPathRow];
			return new FavoriteModel().IsFavorite(basketItem.Item);
		}

		public void RefreshTotalLabel()
		{
			UILabel lblOrderTotal = this.footerView.ViewWithTag(100) as UILabel;
			lblOrderTotal.Text = GetFormattedOrderTotalString();
		}

		private enum Sections
		{
			Items,
			Coupons,
			Offers
		}
	}
}

