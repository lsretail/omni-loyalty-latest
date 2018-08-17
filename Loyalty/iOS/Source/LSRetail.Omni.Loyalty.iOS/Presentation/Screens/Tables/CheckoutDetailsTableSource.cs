using System;
using UIKit;
using Presentation.Utils;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class CheckoutDetailsTableSource : UITableViewSource
    {
        private UIView headerView;

        //footer view
        private UIView footerView;
        private UILabel lblSubTotalLeft;
        private UILabel lblSubTotalRight;
        private UILabel lblDiscountLeft;
        private UILabel lblDiscountRight;
        private UILabel lblTaxLeft;
        private UILabel lblTaxRight;
        private UIView totalSeperator;
        private UILabel lblTotalLeft;
        private UILabel lblTotalRight;

        private UIButton btnProceed;
        private Currency currency;

        Action ProceedToShippingMethods;

        public CheckoutDetailsTableSource(Action proceedToShippingMethods)
        {
            this.ProceedToShippingMethods = proceedToShippingMethods;
            this.currency = AppData.Device.UserLoggedOnToDevice.Environment.Currency;

            BuildHeaderView();
            BuildFooterView();
        }

        private void BuildHeaderView()
        {
            headerView = new UIView();
            headerView.BackgroundColor = Utils.AppColors.TransparentWhite;

            UILabel lblVerify = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("Checkout_Verify", "Please verify your order before you proceed"),
                Lines = 0,
                TextColor = AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(16)
            };
            lblVerify.SizeToFit();
            headerView.AddSubview(lblVerify);

            // Total
          /* UILabel lblTotal = new UILabel()
            {
                Text = GetFormattedOrderTotalString(),
                TextColor = UIColor.FromRGB(101, 109, 124),
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Right,
                Font = UIFont.BoldSystemFontOfSize(16)
            };
            headerView.AddSubview(lblTotal);*/

            const float margin = 5f;

            headerView.ConstrainLayout(() =>

                lblVerify.Frame.Top == headerView.Frame.Top + margin &&
                lblVerify.Frame.Width == headerView.Frame.Width - 2 * margin &&
                lblVerify.Frame.GetCenterX() == headerView.Frame.GetCenterX() 

                /*lblTotal.Frame.Top == lblVerify.Frame.Bottom &&
                lblTotal.Frame.Right == headerView.Frame.Right - 2 * margin &&
                lblTotal.Frame.Width == headerView.Frame.Width*/
            );
        }

        private void BuildFooterView()
        {
            footerView = new UIView();
            footerView.BackgroundColor = UIColor.Clear;

            UIView containerView = new UIView();
            containerView.BackgroundColor = UIColor.White;
            footerView.AddSubview(containerView);

            UIView containerBtnView = new UIView();
            containerBtnView.BackgroundColor = UIColor.Clear;
            footerView.AddSubview(containerBtnView);

            //Subtotal - NetAmount
            this.lblSubTotalLeft = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("Checkout_SubTotal", "Sub-Total:"),
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(15)
            };
            containerView.AddSubview(this.lblSubTotalLeft);

            this.lblSubTotalRight = new UILabel()
            {
                Text = this.currency.FormatDecimal(AppData.Device.UserLoggedOnToDevice.Basket.TotalNetAmount),
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(15)
            };
            containerView.AddSubview(this.lblSubTotalRight);

            //Discount
            this.lblDiscountLeft = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("Checkout_Discount", "Discount: "),
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(15)
            };
            containerView.AddSubview(this.lblDiscountLeft);

            this.lblDiscountRight = new UILabel()
            {
                Text = this.currency.FormatDecimal(AppData.Device.UserLoggedOnToDevice.Basket.TotalDiscAmount),
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(15)
            };
            containerView.AddSubview(this.lblDiscountRight);


            //VAT - Net Tax Amount
            this.lblTaxLeft = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("Checkout_VAT", "VAT: "),
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(15)
            };
            containerView.AddSubview(this.lblTaxLeft);

            this.lblTaxRight = new UILabel()
            {
                Text = this.currency.FormatDecimal(AppData.Device.UserLoggedOnToDevice.Basket.TotalTaxAmount),
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(15)
            };
            containerView.AddSubview(this.lblTaxRight);

            this.totalSeperator = new UIView()
            {
                BackgroundColor = UIColor.FromRGB(224, 224, 224),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            containerView.AddSubview(this.totalSeperator);


            this.lblTotalLeft = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("SlideoutBasket_Total", "Total") + ": ",
                TextColor = AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.BoldSystemFontOfSize(17)
            };
            containerView.AddSubview(this.lblTotalLeft);

            this.lblTotalRight = new UILabel()
            {
                Text = this.currency.FormatDecimal(AppData.Device.UserLoggedOnToDevice.Basket.TotalAmount),
                TextColor = AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Right,
                Font = UIFont.BoldSystemFontOfSize(17)
            };
            containerView.AddSubview(this.lblTotalRight);

            this.btnProceed = new UIButton();
            btnProceed.SetTitle(LocalizationUtilities.LocalizedString("General_Proceed", "Proceed"), UIControlState.Normal);
            btnProceed.BackgroundColor = Utils.AppColors.PrimaryColor;
            btnProceed.Layer.CornerRadius = 2;
            btnProceed.TouchUpInside += (object sender, EventArgs e) =>
            {

                ProceedToShippingMethods();

            };
            containerBtnView.AddSubview(this.btnProceed);

            const float lblLeftWidth = 150f;
            const float lblRightWidth = 90f;
            const float lblHeightSmall = 16f;
            const float lblHeightLarge = 20f;
            const float leftFromCenter = 120f;
            const float totalSeperatorWidth = 240f;
            const float totalSeperatorHeight = 2f;

            const float totalFooterHeight = 94f;

            footerView.ConstrainLayout(() =>

                containerView.Frame.Left == footerView.Frame.Left &&
                containerView.Frame.Right == footerView.Frame.Right &&
                containerView.Frame.Top == footerView.Frame.Top + 10 &&
                containerView.Frame.Height == totalFooterHeight &&

                containerBtnView.Frame.Top == containerView.Bounds.Bottom &&
                containerBtnView.Frame.Left == footerView.Frame.Left &&
                containerBtnView.Frame.Right == footerView.Frame.Right &&
                containerBtnView.Frame.Bottom == footerView.Bounds.Bottom &&

                lblSubTotalLeft.Frame.Left == containerView.Bounds.GetCenterX() - leftFromCenter &&
                lblSubTotalLeft.Frame.Top == containerView.Bounds.Top + 10 &&
                lblSubTotalLeft.Frame.Width == lblLeftWidth &&
                lblSubTotalLeft.Frame.Height == lblHeightSmall &&

                lblSubTotalRight.Frame.Left == lblSubTotalLeft.Frame.Right &&
                lblSubTotalRight.Frame.Top == containerView.Bounds.Top + 10 &&
                lblSubTotalRight.Frame.Width == lblRightWidth &&
                lblSubTotalRight.Frame.Height == lblHeightSmall &&

                lblDiscountLeft.Frame.Left == containerView.Bounds.GetCenterX() - leftFromCenter &&
                lblDiscountLeft.Frame.Top == lblSubTotalLeft.Frame.Bottom &&
                lblDiscountLeft.Frame.Width == lblLeftWidth &&
                lblDiscountLeft.Frame.Height == lblHeightSmall &&

                lblDiscountRight.Frame.Left == lblDiscountLeft.Frame.Right &&
                lblDiscountRight.Frame.Top == lblSubTotalLeft.Frame.Bottom &&
                lblDiscountRight.Frame.Width == lblRightWidth &&
                lblDiscountRight.Frame.Height == lblHeightSmall &&

                lblTaxLeft.Frame.Left == containerView.Bounds.GetCenterX() - leftFromCenter &&
                lblTaxLeft.Frame.Top == lblDiscountLeft.Frame.Bottom &&
                lblTaxLeft.Frame.Width == lblLeftWidth &&
                lblTaxLeft.Frame.Height == lblHeightSmall &&

                lblTaxRight.Frame.Left == lblTaxLeft.Frame.Right &&
                lblTaxRight.Frame.Top == lblDiscountLeft.Frame.Bottom &&
                lblTaxRight.Frame.Width == lblRightWidth &&
                lblTaxRight.Frame.Height == lblHeightSmall &&

                totalSeperator.Frame.Left == containerView.Bounds.GetCenterX() - leftFromCenter &&
                totalSeperator.Frame.Top == lblTaxLeft.Frame.Bottom + 2 &&
                totalSeperator.Frame.Width == totalSeperatorWidth &&
                totalSeperator.Frame.Height == totalSeperatorHeight &&

                lblTotalLeft.Frame.Left == containerView.Bounds.GetCenterX() - leftFromCenter &&
                lblTotalLeft.Frame.Top == totalSeperator.Frame.Bottom + 2 &&
                lblTotalLeft.Frame.Width == lblLeftWidth &&
                lblTotalLeft.Frame.Height == lblHeightLarge &&

                lblTotalRight.Frame.Left == lblTotalLeft.Frame.Right &&
                lblTotalRight.Frame.Top == totalSeperator.Frame.Bottom + 2 &&
                lblTotalRight.Frame.Width == lblRightWidth &&
                lblTotalRight.Frame.Height == lblHeightLarge &&

                btnProceed.Frame.Top == containerBtnView.Bounds.Top + 10f &&
                btnProceed.Frame.Left == containerBtnView.Bounds.Left + 20f &&
                btnProceed.Frame.Right == containerBtnView.Bounds.Right - 20f &&
                btnProceed.Frame.Height == 44f
            );

        }

        private string GetFormattedOrderTotalString()
        {
            string formattedTotal = AppData.Device.UserLoggedOnToDevice.Environment != null ? AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Device.UserLoggedOnToDevice.Basket.TotalAmount) : AppData.Device.UserLoggedOnToDevice.Basket.TotalAmount.ToString();
            return LocalizationUtilities.LocalizedString("SlideoutBasket_Total", "Total") + ": " + formattedTotal;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 3 + 1;   // One dummy section that we put our footer in
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == (int)Sections.Items)
            {
                return AppData.Device.UserLoggedOnToDevice.Basket.Items.Count;
            }
            else
            {
                return 0;
            }
            /*
				else if (section == (int)Sections.Coupons)
				{
					// TODO: Current implementation uses the selected coupons that the contact has, not the coupon list the basket has
					return (AppData.SelectedCoupons != null ? AppData.SelectedCoupons.Count : 0);
				}
				else if (section == (int)Sections.Offers)
				{
					// TODO: We're just using offers that the contact has, should be able to use general offers not connected to contact?
					return (AppData.SelectedOffers != null ? AppData.SelectedOffers.Count : 0);
				}*/
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            CheckoutDetailsItemOverviewCell cell = tableView.DequeueReusableCell(CheckoutDetailsItemOverviewCell.Key) as CheckoutDetailsItemOverviewCell;
            if (cell == null)
                cell = new CheckoutDetailsItemOverviewCell();

            // Set default values
            string description = string.Empty;
            string extraInfo = string.Empty;
            string quantity = string.Empty;
            string formattedPriceString = string.Empty;
            string discount = string.Empty;
            string imageAvgColor = string.Empty;
            string imageId = string.Empty;

            //CheckoutDetailsItemOverviewCell.CellType cellType = CheckoutDetailsItemOverviewCell.CellType.Item;

            if (indexPath.Section == (int)Sections.Items)
            {
                OneListItem basketItem = AppData.Device.UserLoggedOnToDevice.Basket.Items[indexPath.Row];

                //cellType = CheckoutDetailsItemOverviewCell.CellType.Item;

                description = basketItem.Item.Description;
                extraInfo = BasketController.GenerateItemExtraInfo(basketItem);
                quantity = basketItem.Quantity.ToString();

                if (basketItem.OnelistItemDiscounts.Count > 0)
                {
                    discount = LocalizationUtilities.LocalizedString("TransactionView_Discount", "Discount:") + " " + AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketItem.GetDiscount());
                }

                formattedPriceString = AppData.Device.UserLoggedOnToDevice.Environment != null ? AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketItem.Amount) : basketItem.Amount.ToString();

                ImageView imageView = basketItem.Image;
                imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
                imageId = (imageView != null ? imageView.Id : string.Empty);
            }
            /*
				else if (indexPath.Section == (int)Sections.Coupons)
				{
					Coupon coupon = AppData.SelectedCoupons[indexPath.Row];

					cellType = CheckoutDetailsItemOverviewCell.CellType.Coupon;

					description = coupon.Description;
					quantity = "1";

					Domain.Images.ImageView imageView = coupon.ImageViews.FirstOrDefault();
					imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					imageId = (imageView != null ? imageView.Id : string.Empty);
				}
				else if (indexPath.Section == (int)Sections.Offers)
				{
					Offer offer = AppData.SelectedOffers[indexPath.Row];

					cellType = CheckoutDetailsItemOverviewCell.CellType.Offer;

					description = offer.Description;
					quantity = "1";

					Domain.Images.ImageView imageView = offer.ImageViews.FirstOrDefault();
					imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					imageId = (imageView != null ? imageView.Id : string.Empty);
				}
				*/

            cell.SetValues(indexPath.Row, description, extraInfo, quantity, discount, formattedPriceString, imageAvgColor, imageId);

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == (int)Sections.Items)
                return CheckoutDetailsItemOverviewCell.GetCellHeight(BasketController.GenerateItemExtraInfo(Utils.AppData.Device.UserLoggedOnToDevice.Basket.Items[indexPath.Row]));
            else
                return CheckoutDetailsItemOverviewCell.GetCellHeight(string.Empty);
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
                return 55f;
            else
                return 0f;
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            // TODO The footerview should really be a footerview for the entire table, not just the last section.+
            // Should never be able to scroll the footerview off screen.

            if (section == NumberOfSections(tableView) - 1)
                return this.footerView;
            else
                return null;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            if (section == NumberOfSections(tableView) - 1)
                return 158f + 10f;
            else
                return 0f;
        }

        private enum Sections
        {
            Items,
            Coupons,
            Offers
        }
    }
}

