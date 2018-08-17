using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class ConfirmOrderTableSource : UITableViewSource
    {
        //header views
        private UIView alertHeaderView;
        private UIView confirmHeaderView;

        //footer view
        private UIView footerView;
        private UIView containerBtnProceedView;
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

        private Store Store;
        private OneList Basket;
        private List<OneListItem> unavailableItems;
        public bool isFinalizedBasket;
        private Currency currency;

        public delegate void PlaceOrderPressedEventHandler(string email);
        public event PlaceOrderPressedEventHandler PlaceOrderPressed;

        public delegate void ProceedToPlaceOrderEventHandler();
        public event ProceedToPlaceOrderEventHandler ProceedToPlaceOrder;


        public ConfirmOrderTableSource(Store store, OneList basket, List<OneListItem> unavailableItems, bool isFinalizedBasket)
        {
            this.Store = store;
            this.Basket = basket;
            this.unavailableItems = unavailableItems;
            this.isFinalizedBasket = isFinalizedBasket;
            this.currency = AppData.Device.UserLoggedOnToDevice.Environment.Currency;

            BuildConfirmationHeaderView();
            BuildFooterView();

            if (!isFinalizedBasket)
            {
                BuildAlertHeaderView();
            }
        }

        private void BuildConfirmationHeaderView()
        {
            confirmHeaderView = new UIView();
            confirmHeaderView.BackgroundColor = Utils.AppColors.TransparentWhite;

            UILabel lblStore = new UILabel()
            {
                Text = this.Store.Description,
                TextColor = AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                Font = UIFont.BoldSystemFontOfSize(16)
            };
            lblStore.SizeToFit();
            confirmHeaderView.AddSubview(lblStore);

            UILabel lblStoreAddress = new UILabel()
            {
                Text = this.Store.FormatAddress,
                TextColor = AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                Font = UIFont.SystemFontOfSize(14)
            };
            lblStoreAddress.SizeToFit();
            confirmHeaderView.AddSubview(lblStoreAddress);

            UILabel emailLabel = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("ClickCollect_SendOrderDetails", "Order details will be sent to:"),
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                Font = UIFont.SystemFontOfSize(16)
            };
            confirmHeaderView.AddSubview(emailLabel);

            UITextField emailTextField = new UITextField()
            {
                Text = AppData.Device.UserLoggedOnToDevice.Email,
                TextColor = AppColors.MediumGray,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.White
            };
            emailTextField.BorderStyle = UITextBorderStyle.RoundedRect;
            emailTextField.Layer.CornerRadius = 2;
            confirmHeaderView.AddSubview(emailTextField);

            UILabel lblTotal = new UILabel()
            {
                Text = GetFormattedOrderTotalString(),
                TextColor = AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.BoldSystemFontOfSize(16)
            };
            confirmHeaderView.AddSubview(lblTotal);

            UIButton btnPlaceOrder = new UIButton();
            btnPlaceOrder.SetTitle(LocalizationUtilities.LocalizedString("Checkout_PlaceOrder", "Place order"), UIControlState.Normal);
            btnPlaceOrder.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnPlaceOrder.BackgroundColor = AppColors.PrimaryColor;
            btnPlaceOrder.Layer.CornerRadius = 0;
            btnPlaceOrder.TranslatesAutoresizingMaskIntoConstraints = false;
            btnPlaceOrder.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (this.PlaceOrderPressed != null)
                {
                    emailTextField.ResignFirstResponder();
                    this.PlaceOrderPressed(emailTextField.Text);
                }
                //this.placeOrderPressed(emailTextField.Text);
            };
            confirmHeaderView.AddSubview(btnPlaceOrder);

            const float margin = 5f;
            const float btnHeight = 44f;

            confirmHeaderView.ConstrainLayout(() =>

                lblStore.Frame.GetCenterX() == confirmHeaderView.Frame.GetCenterX() &&
                lblStore.Frame.Top == confirmHeaderView.Frame.Top + 2 * margin &&

                lblStoreAddress.Frame.GetCenterX() == confirmHeaderView.Frame.GetCenterX() &&
                lblStoreAddress.Frame.Top == lblStore.Bounds.Bottom &&

                emailLabel.Frame.GetCenterX() == confirmHeaderView.Frame.GetCenterX() &&
                emailLabel.Frame.Top == lblStoreAddress.Frame.Bottom + 2 * margin &&

                emailTextField.Frame.Top == emailLabel.Frame.Bottom + margin &&
                emailTextField.Frame.Left == confirmHeaderView.Bounds.Left - 5f && //do this so we don't see the textfield rounded corners
                emailTextField.Frame.Right == confirmHeaderView.Bounds.Right + 5f &&

                lblTotal.Frame.GetCenterX() == confirmHeaderView.Frame.GetCenterX() &&
                lblTotal.Frame.Top == emailTextField.Frame.Bottom + 2 * margin &&

                btnPlaceOrder.Frame.Top == lblTotal.Frame.Bottom + 2 * margin &&
                btnPlaceOrder.Frame.Height == btnHeight &&
                btnPlaceOrder.Frame.Left == confirmHeaderView.Bounds.Left + 20f &&
                btnPlaceOrder.Frame.Right == confirmHeaderView.Bounds.Right - 20f
            );
        }

        private void BuildAlertHeaderView()
        {
            alertHeaderView = new UIView();
            alertHeaderView.BackgroundColor = AppColors.TransparentWhite;

            //TODO : Make more elegant attention view

            UILabel lblAttention = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("ClickCollect_Attention", "Attention!"),
                TextColor = AppColors.AttentionRed,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(20)
            };
            alertHeaderView.AddSubview(lblAttention);

            UILabel lblAttentionMsg = new UILabel()
            {
                Text = GetAttentionMessage(),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(14)
            };
            lblAttentionMsg.SizeToFit();
            alertHeaderView.AddSubview(lblAttentionMsg);

            UILabel lblUnavailableItems = new UILabel()
            {
                Text = GetFormattedUnavailableItemsString(),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppColors.MediumGray,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(14)
            };
            lblAttentionMsg.SizeToFit();
            alertHeaderView.AddSubview(lblUnavailableItems);

            UILabel lblProceed = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("ClickCollect_Proceed", "Click \"Proceed\" if you wish to continue with the order below."),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(14)
            };
            lblProceed.SizeToFit();
            alertHeaderView.AddSubview(lblProceed);

            //alert boarders
            UIView topAlert = new UIView()
            {
                BackgroundColor = AppColors.AttentionRed
            };
            alertHeaderView.AddSubview(topAlert);

            UIView leftAlert = new UIView()
            {
                BackgroundColor = AppColors.AttentionRed
            };
            alertHeaderView.AddSubview(leftAlert);

            UIView rightAlert = new UIView()
            {
                BackgroundColor = AppColors.AttentionRed
            };
            alertHeaderView.AddSubview(rightAlert);

            UIView bottomAlert = new UIView()
            {
                BackgroundColor = AppColors.AttentionRed
            };
            alertHeaderView.AddSubview(bottomAlert);


            const float margin = 5f;

            alertHeaderView.ConstrainLayout(() =>

                lblAttention.Frame.GetCenterX() == alertHeaderView.Frame.GetCenterX() &&
                lblAttention.Frame.Top == alertHeaderView.Frame.Top + 2 * margin &&

                lblAttentionMsg.Frame.Top == lblAttention.Frame.Bottom + margin &&
                lblAttentionMsg.Frame.Left == alertHeaderView.Bounds.Left + 4 * margin &&
                lblAttentionMsg.Frame.Right == alertHeaderView.Bounds.Right - 4 * margin &&

                lblUnavailableItems.Frame.Top == lblAttentionMsg.Frame.Bottom + 2 * margin &&
                lblUnavailableItems.Frame.Left == alertHeaderView.Bounds.Left + 4 * margin &&
                lblUnavailableItems.Frame.Right == alertHeaderView.Bounds.Right - 4 * margin &&

                lblProceed.Frame.Top == lblUnavailableItems.Frame.Bottom + margin &&
                lblProceed.Frame.Left == alertHeaderView.Frame.Left + 2 * margin &&
                lblProceed.Frame.Right == alertHeaderView.Frame.Right - 2 * margin &&

                topAlert.Frame.Top == alertHeaderView.Bounds.Top &&
                topAlert.Frame.Left == alertHeaderView.Bounds.Left &&
                topAlert.Frame.Right == alertHeaderView.Bounds.Right &&
                topAlert.Frame.Height == 2 &&

                leftAlert.Frame.Left == alertHeaderView.Bounds.Left &&
                leftAlert.Frame.Top == alertHeaderView.Bounds.Top &&
                leftAlert.Frame.Bottom == alertHeaderView.Bounds.Bottom &&
                leftAlert.Frame.Width == 2 &&

                rightAlert.Frame.Right == alertHeaderView.Bounds.Right &&
                rightAlert.Frame.Top == alertHeaderView.Bounds.Top &&
                rightAlert.Frame.Bottom == alertHeaderView.Bounds.Bottom &&
                rightAlert.Frame.Width == 2 &&

                bottomAlert.Frame.Bottom == alertHeaderView.Bounds.Bottom &&
                bottomAlert.Frame.Left == alertHeaderView.Bounds.Left &&
                bottomAlert.Frame.Right == alertHeaderView.Bounds.Right &&
                bottomAlert.Frame.Height == 2
            );
        }

        private void BuildFooterView()
        {
            footerView = new UIView();
            footerView.BackgroundColor = UIColor.Clear;

            UIView containerView = new UIView();
            containerView.BackgroundColor = UIColor.White;
            footerView.AddSubview(containerView);

            this.containerBtnProceedView = new UIView();
            containerBtnProceedView.BackgroundColor = UIColor.Clear;
            footerView.AddSubview(containerBtnProceedView);

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
                Text = this.currency.FormatDecimal(this.Basket.TotalNetAmount),
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
                Text = this.currency.FormatDecimal(this.Basket.TotalDiscAmount),
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
                Text = this.currency.FormatDecimal(this.Basket.TotalTaxAmount),
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
                Text = this.currency.FormatDecimal(this.Basket.TotalAmount),
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
                if (this.ProceedToPlaceOrder != null)
                {
                    this.ProceedToPlaceOrder();
                }
            };
            containerBtnProceedView.AddSubview(this.btnProceed);

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

                containerBtnProceedView.Frame.Top == containerView.Bounds.Bottom &&
                containerBtnProceedView.Frame.Left == footerView.Frame.Left &&
                containerBtnProceedView.Frame.Right == footerView.Frame.Right &&
                containerBtnProceedView.Frame.Bottom == footerView.Bounds.Bottom &&

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

                btnProceed.Frame.Top == containerBtnProceedView.Bounds.Top + 10f &&
                btnProceed.Frame.Left == containerBtnProceedView.Bounds.Left + 20f &&
                btnProceed.Frame.Right == containerBtnProceedView.Bounds.Right - 20f &&
                btnProceed.Frame.Height == 44f
            );
        }

        private string GetFormattedOrderTotalString()
        {
            string formattedTotal = AppData.Device.UserLoggedOnToDevice.Environment != null ? AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(this.Basket.TotalAmount) : this.Basket.TotalAmount.ToString();
            return LocalizationUtilities.LocalizedString("SlideoutBasket_Total", "Total") + ": " + formattedTotal;
        }

        private string GetAttentionMessage()
        {
            string msg;

            if (this.unavailableItems.Count == 1)
            {
                msg = LocalizationUtilities.LocalizedString("ClickCollect_ItemNotAvailable", "This item is not available at ") + Store.Description + ":";
            }
            else
            {
                msg = LocalizationUtilities.LocalizedString("ClickCollect_ItemsNotAvailable", "These items are not available at ") + Store.Description + ":";
            }
            return msg;
        }

        private string GetFormattedUnavailableItemsString()
        {
            string unavailableItemsString = string.Empty;
            foreach (var item in this.unavailableItems)
            {
                unavailableItemsString += "- " + item.Item.Description + ", " + LocalizationUtilities.LocalizedString("ClickCollect_Qty", "Qty: ") + item.Quantity + "\n";

                if (item.VariantReg != null)
                {
                    unavailableItemsString += "\t  " + item.VariantReg.ToString() + "\n";
                }
            }
            return unavailableItemsString;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 3 + 1;   // One dummy section that we put our footer in
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == (int)Sections.Items)
            {
                return this.Basket.Items.Count;
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
				}
				*/

        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ConfirmOrderItemOverviewCell cell = tableView.DequeueReusableCell(ConfirmOrderItemOverviewCell.Key) as ConfirmOrderItemOverviewCell;
            if (cell == null)
                cell = new ConfirmOrderItemOverviewCell();

            // Set default values
            string description = string.Empty;
            string extraInfo = string.Empty;
            string quantity = string.Empty;
            string formattedPriceString = string.Empty;
            string discount = string.Empty;
            string imageAvgColor = string.Empty;
            string imageId = string.Empty;
            //ConfirmOrderItemOverviewCell.CellType cellType = ConfirmOrderItemOverviewCell.CellType.Item;

            if (indexPath.Section == (int)Sections.Items)
            {
                OneListItem basketItem = this.Basket.Items[indexPath.Row];

                //cellType = ConfirmOrderItemOverviewCell.CellType.Item;

                description = basketItem.Item.Description;
                extraInfo = BasketController.GenerateItemExtraInfo(basketItem);
                quantity = basketItem.Quantity.ToString();

                if (basketItem.OnelistItemDiscounts.Count > 0)
                {
                    discount = LocalizationUtilities.LocalizedString("Checkout_Discount", "Discount: ") + AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketItem.GetDiscount());
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
            // TODO Should we let the user edit items here as well? Let's not, for now.
            // The user should do all his editing in the slideoutbasket.
            //BasketItem basketItem = Utils.AppData.Basket.Items[indexPath.Row];
            //this.BasketItemPressed(basketItem);

            tableView.DeselectRow(indexPath, true);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == (int)Sections.Items)
                return ConfirmOrderItemOverviewCell.GetCellHeight(BasketController.GenerateItemExtraInfo(this.Basket.Items[indexPath.Row]));
            else
                return ConfirmOrderItemOverviewCell.GetCellHeight(string.Empty);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            if (section == 0)
            {
                if (this.isFinalizedBasket)
                {
                    return this.confirmHeaderView;
                }
                else
                {
                    return this.alertHeaderView;
                }
            }
            else
            {
                return null;
            }
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (section == 0)
            {
                if (this.isFinalizedBasket)
                {
                    return 225f;
                }
                else
                {
                    float headerHeight = 138f;
                    foreach (var item in this.unavailableItems)
                    {
                        headerHeight += 17f;
                        if (item.VariantReg != null)
                        {
                            headerHeight += 17f;
                        }
                    }
                    return headerHeight;
                }
            }
            else
            {
                return 0f;
            }
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            // TODO The footerview should really be a footerview for the entire table, not just the last section.+
            // Should never be able to scroll the footerview off screen.

            if (section == NumberOfSections(tableView) - 1)
            {
                if (this.isFinalizedBasket)
                {
                    //hide the proceed button from the footer view
                    this.containerBtnProceedView.Hidden = true;
                }

                return this.footerView;
            }
            else
            {
                return null;
            }
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            if (section == NumberOfSections(tableView) - 1)
            {
                if (this.isFinalizedBasket)
                {
                    return 94f + 10f + 10f;
                }
                else
                {
                    return 94f + 64 + 10f;
                }
            }
            else
            {
                return 0f;
            }
        }

        private enum Sections
        {
            Items,
            Coupons,
            Offers
        }
    }
}
