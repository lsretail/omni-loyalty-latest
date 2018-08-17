using System;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreAnimation;
using Domain.Utils;
using Domain.Transactions;
using Domain.Offers;
using Presentation.Models;
using Presentation.Utils;
using Domain.Basket;
using Domain.MemberContacts;
using Domain.Stores;
using MonoTouch.Dialog;

namespace Presentation.Screens
{
	/*
	public class ConfirmOrderScreen : UIViewController
	{
		private UIView orderSentContainerView;
		private UITextView orderSuccessTitle;
		private UITextView orderSuccessMessage;
		private UIButton btnDone;

		private UIView sendOrderContainerView;
		private UITableView transactionOverviewTableView;

		private UIBarButtonItem proceedBarButton;
		private ConfirmOrderTableSource transactionOverviewTableViewSource;

		private Store Store;
		private Basket Basket;
		private List<BasketItem> unavailableItems;
		private bool isFinalizedBasket;

		public ConfirmOrderScreen(Store store, Basket basket, List<BasketItem> unavailableItems, bool isFinalizedBasket)
		{
			// if it's the original basket - we don't need to recalculate or show alert - the user just needs to confirm the order
			// else - show alert of the items that are not available and recalculate the transaction with the available items

			this.Title = NSBundle.MainBundle.LocalizedString("Checkout_ConfirmOrder", "Confirm order");

			this.Store = store;
			this.Basket = basket;
			this.unavailableItems = unavailableItems;
			this.isFinalizedBasket = isFinalizedBasket;
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = Utils.AppColors.BackgroundGray;

			// Bar buttons
			if (!this.isFinalizedBasket)
			{
				this.proceedBarButton = new UIBarButtonItem();
				proceedBarButton.Title = NSBundle.MainBundle.LocalizedString("General_Proceed", "Proceed");
				proceedBarButton.Clicked += (object sender, EventArgs e) => 
				{
					ProceedToPlaceOrder();
				};
				this.NavigationItem.RightBarButtonItem = this.proceedBarButton;
			}

			// Send order container view

			this.sendOrderContainerView = new UIView();
			this.sendOrderContainerView.BackgroundColor = UIColor.Clear;
			this.sendOrderContainerView.TranslatesAutoresizingMaskIntoConstraints = false;
			this.View.AddSubview(this.sendOrderContainerView);

			this.transactionOverviewTableView = new UITableView();
			this.transactionOverviewTableView.BackgroundColor = UIColor.Clear;
			this.transactionOverviewTableView.AlwaysBounceVertical = true;
			this.transactionOverviewTableView.ShowsVerticalScrollIndicator = false;
			this.transactionOverviewTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.transactionOverviewTableViewSource = new ConfirmOrderTableSource(this.Store, this.Basket, this.unavailableItems, this.isFinalizedBasket, PlaceOrderPressed, ProceedToPlaceOrder);
			this.transactionOverviewTableView.Source = this.transactionOverviewTableViewSource;
			this.transactionOverviewTableView.TranslatesAutoresizingMaskIntoConstraints = false;
			this.sendOrderContainerView.AddSubview(this.transactionOverviewTableView);

			// Order sent container view

			this.orderSentContainerView = new UIView();
			this.orderSentContainerView.BackgroundColor = UIColor.Clear;
			this.orderSentContainerView.Hidden = true;
			this.View.AddSubview(this.orderSentContainerView);

			this.orderSuccessTitle = new UITextView();
			orderSuccessTitle.UserInteractionEnabled = true;
			orderSuccessTitle.Editable = false;
			orderSuccessTitle.Text = NSBundle.MainBundle.LocalizedString("Checkout_ThankYou", "Thank You!");
			orderSuccessTitle.TextColor = Utils.AppColors.PrimaryColor;
			orderSuccessTitle.Font = UIFont.SystemFontOfSize (24);
			orderSuccessTitle.TextAlignment = UITextAlignment.Center;
			orderSuccessTitle.BackgroundColor = UIColor.Clear;
			this.orderSentContainerView.AddSubview(orderSuccessTitle);


			this.orderSuccessMessage = new UITextView();
			orderSuccessMessage.UserInteractionEnabled = true;
			orderSuccessMessage.Editable = false;
			orderSuccessMessage.Text = NSBundle.MainBundle.LocalizedString("Checkout_OrderReceived", "We have received your order and are now collecting the item/s. We will notify you as soon as your order is ready.");
			orderSuccessMessage.TextColor = UIColor.DarkGray;
			orderSuccessMessage.Font = UIFont.SystemFontOfSize (16);
			orderSuccessMessage.TextAlignment = UITextAlignment.Center;
			orderSuccessMessage.BackgroundColor = UIColor.Clear;
			this.orderSentContainerView.AddSubview(orderSuccessMessage);

			this.btnDone = new UIButton();
			btnDone.SetTitle(NSBundle.MainBundle.LocalizedString("General_Done", "Done"), UIControlState.Normal);
			btnDone.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnDone.Layer.CornerRadius = 2;
			btnDone.TouchUpInside += (object sender, EventArgs e) => {

				DonePressed();

			};
			this.orderSentContainerView.AddSubview(btnDone);

			// Set layout constraints

			this.View.ConstrainLayout(() => 

				this.sendOrderContainerView.Frame.Top == this.View.Bounds.Top &&
				this.sendOrderContainerView.Frame.Left == this.View.Bounds.Left &&
				this.sendOrderContainerView.Frame.Right == this.View.Bounds.Right &&
				this.sendOrderContainerView.Frame.Bottom == this.View.Bounds.Bottom &&

				this.orderSentContainerView.Frame.Top == this.View.Bounds.Top &&
				this.orderSentContainerView.Frame.Left == this.View.Bounds.Left &&
				this.orderSentContainerView.Frame.Right == this.View.Bounds.Right &&
				this.orderSentContainerView.Frame.Bottom == this.View.Bounds.Bottom

			);

			this.sendOrderContainerView.ConstrainLayout(() => 

				this.transactionOverviewTableView.Frame.Top == this.sendOrderContainerView.Bounds.Top &&
				this.transactionOverviewTableView.Frame.Right == this.sendOrderContainerView.Bounds.Right &&
				this.transactionOverviewTableView.Frame.Left == this.sendOrderContainerView.Bounds.Left &&
				this.transactionOverviewTableView.Frame.Bottom == this.sendOrderContainerView.Bounds.Bottom

			);

			this.orderSentContainerView.ConstrainLayout(() => 

				this.orderSuccessTitle.Frame.Top == this.orderSentContainerView.Bounds.Top + 140f &&
				this.orderSuccessTitle.Frame.Left == this.orderSentContainerView.Bounds.Left &&
				this.orderSuccessTitle.Frame.Right == this.orderSentContainerView.Bounds.Right &&
				this.orderSuccessTitle.Frame.Height == 40f &&

				this.btnDone.Frame.Bottom == this.orderSentContainerView.Bounds.Bottom - 20f &&
				this.btnDone.Frame.Left == this.orderSentContainerView.Bounds.Left + 20f &&
				this.btnDone.Frame.Right == this.orderSentContainerView.Bounds.Right - 20f &&
				this.btnDone.Frame.Height == 50f &&

				this.orderSuccessMessage.Frame.Top == this.orderSuccessTitle.Frame.Bottom &&
				this.orderSuccessMessage.Frame.Left == this.orderSentContainerView.Bounds.Left + 20f &&
				this.orderSuccessMessage.Frame.Right == this.orderSentContainerView.Bounds.Right - 20f &&
				this.orderSuccessMessage.Frame.Height == 120f

			);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public void ProceedToPlaceOrder()
		{
			//Refresh the header view and hide the proceed button

			this.transactionOverviewTableViewSource.isFinalizedBasket = true;
			this.transactionOverviewTableView.ReloadData();

			this.NavigationItem.RightBarButtonItem = null;
		}

		private void PlaceOrderPressed(string email)
		{
			Utils.UI.ShowLoadingIndicator ();

			new Models.ClickCollectModel().CreateOrder(this.Store.Id, email, this.Basket, 
				()=>
				{
					//success
					Utils.UI.HideLoadingIndicator();

					Utils.UI.AddFadeTransitionToView(this.orderSentContainerView);
					this.sendOrderContainerView.Hidden = true;
					this.orderSentContainerView.Hidden = false;

					this.NavigationItem.HidesBackButton = true;
				},
				() =>
				{
					Utils.UI.HideLoadingIndicator();
					//failure
				}
			);
		}

		private void DonePressed()
		{
			Utils.UI.ShowLoadingIndicator();

			new BasketModel().ClearBasket(
				() => 
				{
					// Success
					this.Basket.Clear();

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

