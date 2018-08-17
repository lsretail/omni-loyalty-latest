using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class BasketTableSource : UITableViewSource
	{
		public bool HasData
		{
			get
			{
				if (AppData.Basket.Items.Count > 0
					|| AppData.SelectedPublishedOffers.Count > 0)
					return true;
				else
					return false;
			}
		}

		private readonly BasketView.IBasketListeners listener;

		public BasketTableSource(BasketView.IBasketListeners listener)
		{
			this.listener = listener;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 3;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			if (section == (int)BasketSections.Items)
			{
				return AppData.Basket.Items.Count;
			}
			else if (section == (int)BasketSections.Offers)
			{
				// TODO: Current implementation uses the selected coupons that the contact has, not the coupon list the basket has
				return (AppData.SelectedPublishedOffers != null ? AppData.SelectedPublishedOffers.Count : 0);
			}
			else
			{
				return 0;
			}
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			BasketCell cell = (BasketCell)tableView.DequeueReusableCell(BasketCell.KEY);

			if (cell == null)
			{

				if (indexPath.Section == (int)BasketSections.Items)
				{

					BasketItem basketItem = AppData.Basket.Items[indexPath.Row];
					ImageView imageView = basketItem.Item.Images[0];
					string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					string imageId = (imageView != null ? imageView.Id : string.Empty);

					cell = new BasketCell(BasketView.BasketType.Item, this.listener);
					string extraLines = Utils.Util.GenerateItemExtraInfo(basketItem.Item);
					cell.UpdateData(
						indexPath.Row,
						basketItem.Item.Description,
						basketItem.Quantity,
						AppData.MobileMenu.Currency.FormatDecimal(new LocalBasketService().GetBasketItemFullPrice(AppData.MobileMenu, basketItem)),
						extraLines,
						imageAvgColor,
						imageId,
						BasketView.BasketType.Item
					); 
				}
				else if (indexPath.Section == (int)BasketSections.Offers)
				{

					PublishedOffer basketItem = AppData.SelectedPublishedOffers[indexPath.Row];
					ImageView imageView = basketItem.Images[0];
					string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					string imageId = (imageView != null ? imageView.Id : string.Empty);

					cell = new BasketCell(BasketView.BasketType.Offer, this.listener);
					PublishedOffer publishedOffer = AppData.SelectedPublishedOffers[indexPath.Row];
					cell.UpdateData(
						indexPath.Row,
						publishedOffer.Description,
						1,
						string.Empty,
						string.Empty,
						imageAvgColor,
						imageId,
						BasketView.BasketType.Offer
					);
				}
			}

			return cell;
		}


		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			// TODO Do something when we press coupons and offers?
			if (indexPath.Section == (int)BasketSections.Items)
			{
				this.listener.ItemClicked(indexPath.Row);
			}
			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			if (section == (int)BasketSections.Items)
			{
				return 0;
			}
			else if (section == (int)BasketSections.Offers && RowsInSection(tableView, section) > 0)
			{
				return 30f;
			}
			else
			{
				return 0;
			}
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			if (section == (int)BasketSections.Items)
			{
				return null;
			}
			else if (section == (int)BasketSections.Offers)
			{
				UIView headerView = new UIView();
				headerView.BackgroundColor = UIColor.Clear;

				UILabel lblOffers = new UILabel();
				lblOffers.Frame = new CGRect(5f, 5f, tableView.Bounds.Width - 10f, GetHeightForHeader(tableView, section));
				lblOffers.BackgroundColor = UIColor.Clear;
				lblOffers.Text = LocalizationUtilities.LocalizedString("SlideoutBasket_Offers", "Offers");
				lblOffers.Font = UIFont.BoldSystemFontOfSize(14);
				lblOffers.TextColor = AppColors.PrimaryColor;
				headerView.AddSubview(lblOffers);

				return headerView;
			}
			else
			{
				return null;
			}
		}

		public override nfloat GetHeightForFooter(UITableView tableView, nint section)
		{
			return 0f;
		}

		public override UIView GetViewForFooter(UITableView tableView, nint section)
		{
			// We're not using the footer (height: 0f)

			//			UIView footer = new UIView ();
			//			footer.BackgroundColor = Utils.AppColors.TransparentWhite;
			//
			//			UIView separator = new UIView ();
			//			separator.Frame = new RectangleF (0f, 0f, tableView.Bounds.Width, 2f);
			//			separator.BackgroundColor = Utils.AppColors.KFCRed;
			//			footer.AddSubview (separator);
			//
			//			UILabel lblTotal = new UILabel ();
			//			lblTotal.Text = "Total: " + "$" + OrderTotal();
			//			lblTotal.TextAlignment = UITextAlignment.Right;
			//			lblTotal.BackgroundColor = UIColor.Clear;
			//			float xyMargin = 5f;
			//			lblTotal.Frame = new RectangleF (xyMargin, separator.Frame.Height, tableView.Bounds.Width - 2 * xyMargin, GetHeightForFooter(tableView, section) - separator.Frame.Height);
			//			footer.AddSubview (lblTotal);
			//
			//			return footer;

			return null;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == (int)BasketSections.Items)
			{
				BasketItem selectedBasketItem = AppData.Basket.Items[indexPath.Row];
				return BasketCell.GetCellHeight(Utils.Util.GenerateItemExtraInfo(selectedBasketItem.Item));
			}
			else
			{
				return BasketCell.GetCellHeight(string.Empty);
			}
		}

		/*
		 * TODO laga þetta
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete)
			{
				if (indexPath.Section == (int)BasketSections.Items)
				{
					BasketItem deletedBasketItem = AppData.Basket.Items [indexPath.Row];
					new Models.BasketModel().RemoveItemFromBasket(deletedBasketItem.Id);
					Util.AppDelegate.SlideoutBasket.Refresh();
				}
				else if (indexPath.Section == (int)BasketSections.Coupons)
				{
					Coupon coupon = AppData.SelectedCoupons[indexPath.Row];
					new Models.BasketModel().ToggleCoupon(coupon.Id);
					Utils.Util.AppDelegate.SlideoutBasket.Refresh();
				}
				else if (indexPath.Section == (int)BasketSections.Offers)
				{
					Offer offer = AppData.SelectedOffers[indexPath.Row];
					new Models.BasketModel().ToggleOffer(offer.Id);
					Utils.Util.AppDelegate.SlideoutBasket.Refresh();
				}
			}
		}
		*/

		private enum BasketSections
		{
			Items = 0,
			Offers = 1
		}
	}
}

