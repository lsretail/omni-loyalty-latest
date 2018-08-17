using System;
using UIKit;
using Foundation;
using Presentation.Utils;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class HomeDeliveryView : BaseView
    {
        private UITextView homeDeliveryTitle;
        private UITextView homeDeliveryText;

        private UITextView orderSuccessTitle;
        private UITextView orderSuccessMessage;
        private UIButton btnPlaceOrder;
        private UIButton btnDone;

        private UIView placeOrderContainerView;
        private UIView orderPlacedContainerView;

        public delegate void DoneButtonPressedEventHandler();
        public DoneButtonPressedEventHandler DoneButtonPressed;

        public delegate void SendOrderEventHandler(Action onSuccess);
        public SendOrderEventHandler SendOrder;

        public HomeDeliveryView()
        {
            this.BackgroundColor = Utils.AppColors.BackgroundGray;

            this.placeOrderContainerView = new UIView();
            this.placeOrderContainerView.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.placeOrderContainerView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.AddSubview(this.placeOrderContainerView);

            this.homeDeliveryTitle = new UITextView();
            this.homeDeliveryTitle.UserInteractionEnabled = true;
            this.homeDeliveryTitle.Editable = false;
            this.homeDeliveryTitle.Text = LocalizationUtilities.LocalizedString("Checkout_HomeDeliveryTitle", "Home Delivery - Demo Screen");
            this.homeDeliveryTitle.TextColor = Utils.AppColors.PrimaryColor;
            this.homeDeliveryTitle.Font = UIFont.SystemFontOfSize(20);
            this.homeDeliveryTitle.TextAlignment = UITextAlignment.Center;
            this.homeDeliveryTitle.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.placeOrderContainerView.AddSubview(this.homeDeliveryTitle);

            this.homeDeliveryText = new UITextView();
            this.homeDeliveryText.UserInteractionEnabled = true;
            this.homeDeliveryText.Editable = false;
            this.homeDeliveryText.Text = LocalizationUtilities.LocalizedString("Checkout_HomeDeliveryMsg", "This process needs to be localized for each region");
            this.homeDeliveryText.TextColor = AppColors.PrimaryColor;
            this.homeDeliveryText.Font = UIFont.SystemFontOfSize(16);
            this.homeDeliveryText.TextAlignment = UITextAlignment.Center;
            this.homeDeliveryText.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.placeOrderContainerView.AddSubview(this.homeDeliveryText);

            this.btnPlaceOrder = new UIButton();
            this.btnPlaceOrder.SetTitle(LocalizationUtilities.LocalizedString("Checkout_PlaceOrder", "Place order"), UIControlState.Normal);
            this.btnPlaceOrder.BackgroundColor = Utils.AppColors.PrimaryColor;
            this.btnPlaceOrder.Layer.CornerRadius = 2;
            this.btnPlaceOrder.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (this.SendOrder != null)
                {
                    this.SendOrder(() =>
                    {
                        Utils.UI.AddFadeTransitionToView(this.orderPlacedContainerView);
                        this.placeOrderContainerView.Hidden = true;
                        this.orderPlacedContainerView.Hidden = false;
                    });
                }
            };
            this.placeOrderContainerView.AddSubview(this.btnPlaceOrder);

            this.orderPlacedContainerView = new UIView();
            this.orderPlacedContainerView.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.orderPlacedContainerView.Hidden = true;
            this.AddSubview(this.orderPlacedContainerView);

            this.orderSuccessTitle = new UITextView();
            this.orderSuccessTitle.UserInteractionEnabled = true;
            this.orderSuccessTitle.Editable = false;
            this.orderSuccessTitle.Text = LocalizationUtilities.LocalizedString("Checkout_ThankYou", "Thank You!");
            this.orderSuccessTitle.TextColor = Utils.AppColors.PrimaryColor;
            this.orderSuccessTitle.Font = UIFont.SystemFontOfSize(24);
            this.orderSuccessTitle.TextAlignment = UITextAlignment.Center;
            this.orderSuccessTitle.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.orderPlacedContainerView.AddSubview(orderSuccessTitle);


            this.orderSuccessMessage = new UITextView();
            this.orderSuccessMessage.UserInteractionEnabled = true;
            this.orderSuccessMessage.Editable = false;
            this.orderSuccessMessage.Text = LocalizationUtilities.LocalizedString("Checkout_OrderProcessed", "Your order has been successfully processed.");
            this.orderSuccessMessage.TextColor = Utils.AppColors.PrimaryColor;
            this.orderSuccessMessage.Font = UIFont.SystemFontOfSize(16);
            this.orderSuccessMessage.TextAlignment = UITextAlignment.Center;
            this.orderSuccessMessage.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.orderPlacedContainerView.AddSubview(orderSuccessMessage);

            this.btnDone = new UIButton();
            this.btnDone.SetTitle(LocalizationUtilities.LocalizedString("General_Done", "Done"), UIControlState.Normal);
            this.btnDone.BackgroundColor = Utils.AppColors.PrimaryColor;
            this.btnDone.Layer.CornerRadius = 2;
            this.btnDone.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (this.DoneButtonPressed != null)
                {
                    this.DoneButtonPressed();
                }
            };
            this.orderPlacedContainerView.AddSubview(btnDone);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            const float margin = 20f;
            const float titleHeight = 40f;
            const float textHeight = 60f;
            const float buttonHeight = 50f;

            // Place order view and subviews
            this.placeOrderContainerView.Frame = this.Frame;

            this.homeDeliveryTitle.Frame = new CGRect(
                margin,
                7 * margin,
                this.Frame.Width - 2 * margin,
                titleHeight
            );

            this.homeDeliveryText.Frame = new CGRect(
                margin,
                homeDeliveryTitle.Frame.Bottom,
                this.Frame.Width - 2 * margin,
                textHeight
            );

            this.btnPlaceOrder.Frame = new CGRect(
                margin,
                this.Frame.Bottom - margin - buttonHeight,
                this.Frame.Width - 2 * margin,
                buttonHeight
            );

            // Order placed view and subviews
            this.orderPlacedContainerView.Frame = this.Frame;

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
    }
}

