using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation
{
    public class ConfirmOrderView : BaseView
    {
        private UIView orderSentContainerView;
        private UITextView orderSuccessTitle;
        private UITextView orderSuccessMessage;
        private UIButton btnDone;

        private UIView sendOrderContainerView;
        private UITableView transactionOverviewTableView;

        private ConfirmOrderTableSource transactionOverviewTableViewSource;

        public delegate void PlaceOrderPressedEventHandler(string email);
        public event PlaceOrderPressedEventHandler PlaceOrderPressed;

        public delegate void ProceedToPlaceOrderEventHandler();
        public event ProceedToPlaceOrderEventHandler ProceedToPlaceOrder;

        public delegate void DoneButtonPressedEventHandler();
        public event DoneButtonPressedEventHandler DoneButtonPressed;

        public ConfirmOrderView()
        {
            this.BackgroundColor = Utils.AppColors.BackgroundGray;

            // Send order container view
            this.sendOrderContainerView = new UIView();
            this.sendOrderContainerView.BackgroundColor = UIColor.Clear;
            this.sendOrderContainerView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.AddSubview(this.sendOrderContainerView);

            this.transactionOverviewTableView = new UITableView();
            this.transactionOverviewTableView.BackgroundColor = UIColor.Clear;
            this.transactionOverviewTableView.AlwaysBounceVertical = true;
            this.transactionOverviewTableView.ShowsVerticalScrollIndicator = false;
            this.transactionOverviewTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.transactionOverviewTableView.Source = this.transactionOverviewTableViewSource;
            this.transactionOverviewTableView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.sendOrderContainerView.AddSubview(this.transactionOverviewTableView);

            // Order sent container view

            this.orderSentContainerView = new UIView();
            this.orderSentContainerView.BackgroundColor = UIColor.Clear;
            this.orderSentContainerView.Hidden = true;
            this.AddSubview(this.orderSentContainerView);

            this.orderSuccessTitle = new UITextView();
            orderSuccessTitle.UserInteractionEnabled = true;
            orderSuccessTitle.Editable = false;
            orderSuccessTitle.Text = LocalizationUtilities.LocalizedString("Checkout_ThankYou", "Thank You!");
            orderSuccessTitle.TextColor = Utils.AppColors.PrimaryColor;
            orderSuccessTitle.Font = UIFont.SystemFontOfSize(24);
            orderSuccessTitle.TextAlignment = UITextAlignment.Center;
            orderSuccessTitle.BackgroundColor = UIColor.Clear;
            this.orderSentContainerView.AddSubview(orderSuccessTitle);


            this.orderSuccessMessage = new UITextView();
            orderSuccessMessage.UserInteractionEnabled = true;
            orderSuccessMessage.Editable = false;
            orderSuccessMessage.Text = LocalizationUtilities.LocalizedString("Checkout_OrderReceived", "We have received your order and are now collecting the item/s. We will notify you as soon as your order is ready.");
            orderSuccessMessage.TextColor = UIColor.DarkGray;
            orderSuccessMessage.Font = UIFont.SystemFontOfSize(16);
            orderSuccessMessage.TextAlignment = UITextAlignment.Center;
            orderSuccessMessage.BackgroundColor = UIColor.Clear;
            this.orderSentContainerView.AddSubview(orderSuccessMessage);

            this.btnDone = new UIButton();
            btnDone.SetTitle(LocalizationUtilities.LocalizedString("General_Done", "Done"), UIControlState.Normal);
            btnDone.BackgroundColor = Utils.AppColors.PrimaryColor;
            btnDone.Layer.CornerRadius = 2;
            btnDone.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (this.DoneButtonPressed != null)
                {
                    this.DoneButtonPressed();
                }
            };
            this.orderSentContainerView.AddSubview(btnDone);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            const float margin = 20f;
            const float titleHeight = 40f;
            const float textHeight = 120f;
            const float buttonHeight = 50f;

            // Send order view
            this.sendOrderContainerView.Frame = this.Frame;
            this.transactionOverviewTableView.Frame = this.sendOrderContainerView.Frame;

            // Order sent view
            this.orderSentContainerView.Frame = this.Frame;

            this.orderSuccessTitle.Frame = new CGRect(
                margin,
                7 * margin,
                this.Frame.Width - 2 * margin,
                titleHeight
            );

            this.orderSuccessMessage.Frame = new CGRect(
                margin,
                this.orderSuccessTitle.Frame.Bottom,
                this.Frame.Width - 2 * margin,
                textHeight
            );

            this.btnDone.Frame = new CGRect(
                margin,
                this.Frame.Bottom - margin - buttonHeight,
                this.Frame.Width - 2 * margin,
                buttonHeight
            );
        }

        public void UpdateData(Store store, OneList basket, List<OneListItem> unavailableItems, bool isFinalizedBasket)
        {
            this.transactionOverviewTableViewSource = new ConfirmOrderTableSource(store, basket, unavailableItems, isFinalizedBasket);
            this.transactionOverviewTableViewSource.PlaceOrderPressed += (email) =>
            {
                if (this.PlaceOrderPressed != null)
                {
                    this.PlaceOrderPressed(email);
                }
            };
            this.transactionOverviewTableViewSource.ProceedToPlaceOrder += () =>
            {
                if (this.ProceedToPlaceOrder != null)
                {
                    this.ProceedToPlaceOrder();
                }
            };
            this.transactionOverviewTableView.Source = this.transactionOverviewTableViewSource;
        }

        public void ReloadData(bool isFinalizedBasket)
        {
            //Refresh the header view and hide the proceed button
            this.transactionOverviewTableViewSource.isFinalizedBasket = isFinalizedBasket;
            this.transactionOverviewTableView.ReloadData();
        }

        public void OrderCreated()
        {
            Utils.UI.AddFadeTransitionToView(this.orderSentContainerView);
            this.sendOrderContainerView.Hidden = true;
            this.orderSentContainerView.Hidden = false;
        }
    }
}
