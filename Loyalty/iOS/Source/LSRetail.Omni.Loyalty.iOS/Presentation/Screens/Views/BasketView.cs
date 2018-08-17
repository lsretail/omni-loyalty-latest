using System;
using UIKit;
using CoreGraphics;
using Presentation.Tables;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class BasketView : BaseView
    {
        private UITableView tblBasket;
        private UIRefreshControl refreshControlBasket;
        private BasketFooterView footerView;
        private NoDataView noDataView;
        private ChangeVariantQtyPopUp ChangeVariantQtyPopUp;

        public delegate void RemoveItemFromBasketEventHandler(int itemPosition);
        public event RemoveItemFromBasketEventHandler RemoveItemFromBasket;

        public delegate void ItemPressedEventHandler(int itemPosition, OneListItem item);
        //public event ItemPressedEventHandler ItemPressed;

        public delegate void RefreshBasketEventHandler();
        public event RefreshBasketEventHandler RefreshBasket;

        public delegate void CheckoutEventHandler();
        public event CheckoutEventHandler Checkout;

        public delegate void UpdateBasketEventHandler(int positionInBasketList, OneListItem basketItem, Action onSuccess, Action onFailure);
        public event UpdateBasketEventHandler Update;

        public BasketView()
        {
            this.BackgroundColor = Utils.AppColors.BackgroundGray;

            this.tblBasket = new UITableView();
            this.tblBasket.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.tblBasket.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.tblBasket.Source = new BasketTableSource();
            (this.tblBasket.Source as BasketTableSource).RemoveItemFromBasket = ((int itemPosition) =>
            {
                if (this.RemoveItemFromBasket != null)
                    this.RemoveItemFromBasket(itemPosition);
            });
            (this.tblBasket.Source as BasketTableSource).ItemPressed = BasketItemPressed;
            this.AddSubview(this.tblBasket);

            this.refreshControlBasket = new UIRefreshControl();
            this.refreshControlBasket.ValueChanged += (object sender, EventArgs e) =>
            {
                if (this.RefreshBasket != null)
                    this.RefreshBasket();
            };
            this.tblBasket.AddSubview(this.refreshControlBasket);

            this.footerView = new BasketFooterView(
                new CheckoutEventHandler(delegate
                {
                    if (this.Checkout != null)
                        this.Checkout();
                })
            );
            this.AddSubview(this.footerView);

            this.noDataView = new NoDataView();
            this.noDataView.TextToDisplay = LocalizationUtilities.LocalizedString("Basket_BasketEmpty", "Your basket is empty! Add items and they will show up here.");
            this.AddSubview(this.noDataView);

            this.ChangeVariantQtyPopUp = new ChangeVariantQtyPopUp();
            this.ChangeVariantQtyPopUp.Hidden = true;
            Utils.Util.AppDelegate.Window.AddSubview(this.ChangeVariantQtyPopUp);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.footerView.Frame = new CGRect(
                0,
                this.Frame.Height - this.BottomLayoutGuideLength - BasketFooterView.HEIGHT,
                this.Frame.Width,
                BasketFooterView.HEIGHT
            );

            this.tblBasket.Frame = new CGRect(
                0,
                0,
                this.Frame.Width,
                this.Frame.Height - this.footerView.Frame.Height);

            this.noDataView.TopLayoutGuideLength = this.TopLayoutGuideLength;
            this.noDataView.BottomLayoutGuideLength = this.BottomLayoutGuideLength;
            this.noDataView.Frame = this.Frame;

            // Modify the basket table view's content inset to show the inter cell spacing beneath the last cell in the table
            this.tblBasket.ContentInset = new UIEdgeInsets(
                this.tblBasket.ContentInset.Top,
                this.tblBasket.ContentInset.Left,
                this.BottomLayoutGuideLength + BasketCell.INTER_CELL_SPACING,
                this.tblBasket.ContentInset.Right
            );

            this.ChangeVariantQtyPopUp.TopLayoutGuideLength = this.TopLayoutGuideLength;
            this.ChangeVariantQtyPopUp.BottomLayoutGuideLength = this.BottomLayoutGuideLength;
        }

        public void BasketItemPressed(int position, OneListItem basketItem)
        {
            this.ChangeVariantQtyPopUp.UpdateValues(basketItem.Item.VariantsExt, basketItem.Item.VariantsRegistration, basketItem.Item.Description, basketItem.Quantity, basketItem.VariantReg);
            SetChangeVariantQtyPopUpViewLayout(basketItem);
            this.ChangeVariantQtyPopUp.EditingDone = (decimal quantity, VariantRegistration variantRegistration) =>
            {
                if (variantRegistration != null)
                    basketItem.VariantReg = variantRegistration;
                
                basketItem.Quantity = quantity;
                if (Update != null)
                {
                    Update(position, basketItem,
                        () =>
                        {
                            if (this.RefreshBasket != null)
                                this.RefreshBasket();

                            this.ChangeVariantQtyPopUp.HideWithAnimation();
                        },
                        () =>
                        {
                            this.ChangeVariantQtyPopUp.HideWithAnimation();
                        });
                }
            };

            this.ChangeVariantQtyPopUp.ShowWithAnimation();
        }

        public void Refresh(string formattedTotalString)
        {
            this.tblBasket.ReloadData();
            this.footerView.FormattedTotalString = formattedTotalString;

            if ((this.tblBasket.Source as BasketTableSource).HasData)
                this.noDataView.Hidden = true;
            else
                this.noDataView.Hidden = false;

            if (!this.refreshControlBasket.Hidden)
                this.refreshControlBasket.EndRefreshing();
        }

        public void StopRefreshingIndicator()
        {
            this.refreshControlBasket.EndRefreshing();
        }

        private void SetChangeVariantQtyPopUpViewLayout(OneListItem basketItem)
        {
            nfloat popupMargin = Utils.Util.AppDelegate.Window.Frame.Width / 14;
            nfloat changeVariantQtyPopUpHeight;

            if (basketItem.Item.VariantsExt.Count == 0 || basketItem.Item.VariantsRegistration.Count == 0)
            {
                changeVariantQtyPopUpHeight = this.ChangeVariantQtyPopUp.GetRequiredHeight(true);
            }
            else
            {
                changeVariantQtyPopUpHeight = this.ChangeVariantQtyPopUp.GetRequiredHeight();
            }

            this.ChangeVariantQtyPopUp.SetFrame(
                new CGRect(
                    popupMargin,
                    Utils.Util.AppDelegate.Window.Frame.GetMidY() - changeVariantQtyPopUpHeight / 2,
                    Utils.Util.AppDelegate.Window.Frame.Width - 2 * popupMargin,
                    changeVariantQtyPopUpHeight
                )
            );
        }

        private class BasketFooterView : UIView
        {
            private UIView separatorView;
            private UILabel lblTotal;
            private UIButton btnCheckout;

            public const float HEIGHT = 120f;

            public string FormattedTotalString
            {
                set
                {
                    this.lblTotal.Text = LocalizationUtilities.LocalizedString("Basket_Total", "Total") + ": " + value;
                }
            }

            public BasketFooterView(CheckoutEventHandler onCheckoutPressed)
            {
                this.BackgroundColor = UIColor.White;

                this.separatorView = new UIView();
                this.separatorView.BackgroundColor = Utils.AppColors.PrimaryColor;
                this.AddSubview(this.separatorView);

                this.lblTotal = new UILabel();
                this.lblTotal.BackgroundColor = UIColor.Clear;
                this.lblTotal.TextAlignment = UITextAlignment.Right;
                this.AddSubview(this.lblTotal);

                this.btnCheckout = new UIButton();
                this.btnCheckout.BackgroundColor = Utils.AppColors.PrimaryColor;
                this.btnCheckout.SetTitle(LocalizationUtilities.LocalizedString("Basket_Checkout", "Checkout"), UIControlState.Normal);
                this.btnCheckout.SetTitleColor(UIColor.White, UIControlState.Normal);
                this.btnCheckout.Layer.CornerRadius = 3f;
                this.btnCheckout.TouchUpInside += (object sender, EventArgs e) => { onCheckoutPressed(); };
                this.AddSubview(this.btnCheckout);
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                this.separatorView.Frame = new CGRect(0, 0, this.Frame.Width, 3f);
                this.lblTotal.Frame = new CGRect(10f, this.separatorView.Frame.Bottom, this.Frame.Width - 2 * 10f, 40f);
                this.btnCheckout.Frame = new CGRect(44f, this.lblTotal.Frame.Bottom + 10f, this.Frame.Width - 2 * 44f, 44f);
            }
        }
    }
}
