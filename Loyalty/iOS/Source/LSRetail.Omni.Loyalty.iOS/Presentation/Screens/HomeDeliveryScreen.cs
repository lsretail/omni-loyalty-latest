using System;
using UIKit;
using Foundation;
using Presentation.Utils;
using CoreGraphics;
using System.Linq;
using Presentation.Models;

namespace Presentation.Screens
{
	/*
	public class HomeDeliveryScreen : UIViewController
	{
		private UITextView homeDeliveryTitle;
		private UITextView homeDeliveryText;

		private UITextView orderSuccessTitle;
		private UITextView orderSuccessMessage;
		private UIButton btnPlaceOrder;
		private UIButton btnDone;

		private UIView placeOrderContainerView;
		private UIView orderPlacedContainerView;

		public HomeDeliveryScreen ()
		{
			this.Title = NSBundle.MainBundle.LocalizedString("Checkout_HomeDelivery", "Home Delivery");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = Utils.AppColors.BackgroundGray;

			this.placeOrderContainerView = new UIView();
			this.placeOrderContainerView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.placeOrderContainerView.TranslatesAutoresizingMaskIntoConstraints = false;
			this.View.AddSubview(this.placeOrderContainerView);

			this.homeDeliveryTitle = new UITextView();
			homeDeliveryTitle.UserInteractionEnabled = true;
			homeDeliveryTitle.Editable = false;
			homeDeliveryTitle.Text = NSBundle.MainBundle.LocalizedString("Checkout_HomeDeliveryTitle", "Home Delivery - Demo Screen");
			homeDeliveryTitle.TextColor = Utils.AppColors.PrimaryColor;
			homeDeliveryTitle.Font = UIFont.SystemFontOfSize (20);
			homeDeliveryTitle.TextAlignment = UITextAlignment.Center;
			homeDeliveryTitle.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.placeOrderContainerView.AddSubview(this.homeDeliveryTitle);

			this.homeDeliveryText = new UITextView();
			homeDeliveryText.UserInteractionEnabled = true;
			homeDeliveryText.Editable = false;
			homeDeliveryText.Text = NSBundle.MainBundle.LocalizedString("Checkout_HomeDeliveryMsg", "This process needs to be localized for each region");
			homeDeliveryText.TextColor = AppColors.PrimaryColor;
			homeDeliveryText.Font = UIFont.SystemFontOfSize (16);
			homeDeliveryText.TextAlignment = UITextAlignment.Center;
			homeDeliveryText.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.placeOrderContainerView.AddSubview(this.homeDeliveryText);


			this.btnPlaceOrder = new UIButton();
			btnPlaceOrder.SetTitle(NSBundle.MainBundle.LocalizedString("Checkout_PlaceOrder", "Place order"), UIControlState.Normal);
			btnPlaceOrder.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnPlaceOrder.Layer.CornerRadius = 2;
			btnPlaceOrder.TouchUpInside += (object sender, EventArgs e) => {

				Utils.UI.ShowLoadingIndicator ();

				new Models.BasketModel().SendOrder(
					()=>
					{
						//success
						Utils.UI.HideLoadingIndicator();

						AppData.ShouldRefreshPublishedOffers = true;
						AppData.ShouldRefreshPoints = true;

						Utils.UI.AddFadeTransitionToView(this.orderPlacedContainerView);
						this.placeOrderContainerView.Hidden = true;
						this.orderPlacedContainerView.Hidden = false;

						this.NavigationItem.HidesBackButton = true;
					},
					() =>
					{
						Utils.UI.HideLoadingIndicator();
						//failure
					}
				);

			};
			this.placeOrderContainerView.AddSubview(this.btnPlaceOrder);


			// orderPlacedContinerView

			this.orderPlacedContainerView = new UIView();
			this.orderPlacedContainerView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.orderPlacedContainerView.Hidden = true;
			this.View.AddSubview(this.orderPlacedContainerView);

			this.orderSuccessTitle = new UITextView();
			orderSuccessTitle.UserInteractionEnabled = true;
			orderSuccessTitle.Editable = false;
			orderSuccessTitle.Text = NSBundle.MainBundle.LocalizedString("Checkout_ThankYou", "Thank You!");
			orderSuccessTitle.TextColor = Utils.AppColors.PrimaryColor;
			orderSuccessTitle.Font = UIFont.SystemFontOfSize (24);
			orderSuccessTitle.TextAlignment = UITextAlignment.Center;
			orderSuccessTitle.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.orderPlacedContainerView.AddSubview(orderSuccessTitle);


			this.orderSuccessMessage = new UITextView();
			orderSuccessMessage.UserInteractionEnabled = true;
			orderSuccessMessage.Editable = false;
			orderSuccessMessage.Text = NSBundle.MainBundle.LocalizedString("Checkout_OrderProcessed", "Your order has been successfully processed.");
			orderSuccessMessage.TextColor = Utils.AppColors.PrimaryColor;
			orderSuccessMessage.Font = UIFont.SystemFontOfSize (16);
			orderSuccessMessage.TextAlignment = UITextAlignment.Center;
			orderSuccessMessage.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.orderPlacedContainerView.AddSubview(orderSuccessMessage);

			this.btnDone = new UIButton();
			btnDone.SetTitle(NSBundle.MainBundle.LocalizedString("General_Done", "Done"), UIControlState.Normal);
			btnDone.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnDone.Layer.CornerRadius = 2;
			btnDone.TouchUpInside += (object sender, EventArgs e) => {

				DonePressed();

			};
			this.orderPlacedContainerView.AddSubview(btnDone);

			// Set layout constraints

			this.View.ConstrainLayout(() => 

				this.placeOrderContainerView.Frame.Top == this.View.Bounds.Top &&
				this.placeOrderContainerView.Frame.Left == this.View.Bounds.Left &&
				this.placeOrderContainerView.Frame.Right == this.View.Bounds.Right &&
				this.placeOrderContainerView.Frame.Bottom == this.View.Bounds.Bottom &&

				this.orderPlacedContainerView.Frame.Top == this.View.Bounds.Top &&
				this.orderPlacedContainerView.Frame.Left == this.View.Bounds.Left &&
				this.orderPlacedContainerView.Frame.Right == this.View.Bounds.Right &&
				this.orderPlacedContainerView.Frame.Bottom == this.View.Bounds.Bottom

			);

			const float margin = 20f;

			this.placeOrderContainerView.ConstrainLayout(() => 

				this.homeDeliveryTitle.Frame.Top == placeOrderContainerView.Bounds.Top + 140f &&
				this.homeDeliveryTitle.Frame.Width == placeOrderContainerView.Bounds.Width - margin &&
				this.homeDeliveryTitle.Frame.GetCenterX() == placeOrderContainerView.Frame.GetCenterX() &&
				this.homeDeliveryTitle.Frame.Height == 40f &&

				this.homeDeliveryText.Frame.Top == homeDeliveryTitle.Frame.Bottom &&
				this.homeDeliveryText.Frame.Width == placeOrderContainerView.Bounds.Width - margin &&
				this.homeDeliveryText.Frame.GetCenterX() == placeOrderContainerView.Frame.GetCenterX() &&
				this.homeDeliveryText.Frame.Height == 60f &&

				this.btnPlaceOrder.Frame.Bottom == placeOrderContainerView.Bounds.Bottom - margin &&
				this.btnPlaceOrder.Frame.Left == placeOrderContainerView.Bounds.Left + margin &&
				this.btnPlaceOrder.Frame.Right == placeOrderContainerView.Bounds.Right - margin &&
				this.btnPlaceOrder.Frame.Height == 50f
			);

			this.orderPlacedContainerView.ConstrainLayout(() => 

				this.orderSuccessTitle.Frame.Top == this.orderPlacedContainerView.Bounds.Top + 140f &&
				this.orderSuccessTitle.Frame.Left == this.orderPlacedContainerView.Bounds.Left &&
				this.orderSuccessTitle.Frame.Right == this.orderPlacedContainerView.Bounds.Right &&
				this.orderSuccessTitle.Frame.Height == 40f &&

				this.btnDone.Frame.Bottom == this.orderPlacedContainerView.Bounds.Bottom - margin &&
				this.btnDone.Frame.Left == this.orderPlacedContainerView.Bounds.Left + margin &&
				this.btnDone.Frame.Right == this.orderPlacedContainerView.Bounds.Right - margin &&
				this.btnDone.Frame.Height == 50f &&

				this.orderSuccessMessage.Frame.Top == this.orderSuccessTitle.Frame.Bottom &&
				this.orderSuccessMessage.Frame.Left == this.orderPlacedContainerView.Bounds.Left &&
				this.orderSuccessMessage.Frame.Right == this.orderPlacedContainerView.Bounds.Right &&
				this.orderSuccessMessage.Frame.Height == 60f

			);
		}

		private void DonePressed()
		{
			Utils.UI.ShowLoadingIndicator();

			new BasketModel().ClearBasket(
				() => 
				{
					// Success
					Utils.UI.HideLoadingIndicator();

					// TODO: Go back to the first screen in the RootTabBarController (home or menu if home not enabled)
					this.DismissViewController(true, null);
				},
				() => 
				{
					// Failure
					Utils.UI.HideLoadingIndicator();
					Utils.UI.ShowAlertView(
						NSBundle.MainBundle.LocalizedString("General_Error", "Error"),
						NSBundle.MainBundle.LocalizedString("Checkout_ClearBasketErrorTryAgain", "Could not clear basket, please try again."),
						null,
						null,
						false
					);
				}
			);
		}
	}
	*/
}


