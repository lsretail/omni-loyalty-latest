using System;
using CoreGraphics;
using Presentation.Models;
using Presentation.Utils;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
	public class CheckoutController : UIViewController, CheckoutOrderOverView.ICheckoutOrderOverViewListener, CheckoutOrderPlacedView.ICheckoutOrderPlacedListener
	{
		private CheckoutOrderOverView checkoutOrderOverView;
		private CheckoutOrderPlacedView checkoutOrderPlacedView;

		private string backendOrderId;
		private nfloat initialScreenBrightness;

		public CheckoutController()
		{
			this.Title = LocalizationUtilities.LocalizedString("Checkout_Checkout", "Checkout");
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.checkoutOrderOverView = new CheckoutOrderOverView(this);
			this.checkoutOrderPlacedView = new CheckoutOrderPlacedView(this);
			this.View = checkoutOrderOverView;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			this.checkoutOrderOverView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.checkoutOrderOverView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;

			this.checkoutOrderPlacedView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.checkoutOrderPlacedView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;

			SetLeftBarButtonItems();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		private void SetLeftBarButtonItems()
		{
			UIButton btnCancel = new UIButton(UIButtonType.Custom);
			btnCancel.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.White), UIControlState.Normal);
			btnCancel.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			btnCancel.Frame = new CGRect(0, 0, 30, 30);
			btnCancel.TouchUpInside += (sender, e) =>
			{
				this.DismissViewController(true, () => { });
			};
			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(btnCancel);
		}

		private void HideLeftBarButtonItems()
		{
			this.NavigationItem.LeftBarButtonItem = null;
		}

		private string GenerateOrderIdQRCodeXML(string orderId)
		{
			OrderQRCode orderQRCodeModel = new OrderQRCode();
			orderQRCodeModel.Id = orderId;

			return orderQRCodeModel.Serialize();
		}

		private void OnBasketItemEditDone()
		{
			//Utils.Util.AppDelegate.SlideoutBasket.Refresh();

			if (AppData.Basket.Items.Count <= 0 && AppData.SelectedPublishedOffers.Count <= 0)
			{
				this.DismissViewController(true, () => { });
			}
			else
			{
				this.checkoutOrderOverView.RefreshData(true);
			}
		}

		private async void SaveTransaction(Action onFinish)
		{
			Models.TransactionModel transactionModel = new Models.TransactionModel();
			Transaction newTransaction = transactionModel.CreateTransaction();

			// Overwrite the transaction ID with the backendID (GUID, will be unique)
			if (!String.IsNullOrEmpty(this.backendOrderId))
				newTransaction.Id = this.backendOrderId;

			AppData.Transactions.Add(newTransaction.Clone());

			bool success = await transactionModel.SyncTransactionsLocally();
			if (success) 
			{
				System.Diagnostics.Debug.WriteLine("Transactions synced locally successfully");
				onFinish();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Error while syncing transactions locally");
				onFinish();
			}
		}

		#region CheckoutOrderOverViewListener

		public async void SendOrder()
		{
			if (AppData.Basket.Items.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine("Sending order...");
				Utils.UI.ShowLoadingIndicator();

				string orderId = await new OrderModel().OrderSave();
				if (orderId != null) 
				{

					Utils.UI.HideLoadingIndicator();

					System.Diagnostics.Debug.WriteLine("Order sent successfully, ID: " + orderId);
					this.backendOrderId = orderId;
					this.checkoutOrderPlacedView.SetQRCodeImage(Utils.QRCode.QRCode.GenerateQRCode(GenerateOrderIdQRCodeXML(orderId)));

					Utils.UI.AddFadeTransitionToView(this.checkoutOrderPlacedView);
					this.View = this.checkoutOrderPlacedView;

					HideLeftBarButtonItems();

					// Increase screen brightness to help with QR code scanning
					this.initialScreenBrightness = UIScreen.MainScreen.Brightness;
					UIScreen.MainScreen.Brightness = 1;

				}
				else
				{
					Utils.UI.HideLoadingIndicator();

					await AlertView.ShowAlert(
						this,
						LocalizationUtilities.LocalizedString("General_Error", "Error"),
						LocalizationUtilities.LocalizedString("Checkout_ErrorPlacingOrder", "Couldn't place the order.\r\nPlease try again."),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}

			}
			else
			{
				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("Checkout_PlaceOrder", "Place order"),
					LocalizationUtilities.LocalizedString("Checkout_PlaceOrderPleaseAddItems", "Please add some items to your order before placing it."),
					LocalizationUtilities.LocalizedString("General_OK", "OK")
				);
			}
		}

		public async void RemoveBasketItemPressed(CheckoutOrderOverViewCell.CellType cellType, int indexPath)
		{
			System.Diagnostics.Debug.WriteLine("Deleting row of type " + cellType.ToString() + " with index path " + indexPath);

			var alertResult = await AlertView.ShowAlert(
			    this,
				LocalizationUtilities.LocalizedString("Checkout_RemoveItem", "Remove item"),
				LocalizationUtilities.LocalizedString("Checkout_AreYouSureRemoveItem", "Are you sure you want to remove this item?"),
				LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
			    LocalizationUtilities.LocalizedString("General_No", "No")
			);

			if (alertResult == AlertView.AlertButtonResult.PositiveButton)
			{
				if (cellType == CheckoutOrderOverViewCell.CellType.Item)
				{
					BasketItem basketItem = Utils.AppData.Basket.Items[indexPath];
					new BasketModel().RemoveItemFromBasket(basketItem);
				}
				else if (cellType == CheckoutOrderOverViewCell.CellType.Offer)
				{
					PublishedOffer publishedOffer = AppData.SelectedPublishedOffers[indexPath];
					new BasketModel().TogglePublishedOffer(publishedOffer);
				}

				this.checkoutOrderOverView.RefreshData();
				OnBasketItemEditDone();
			}
		}

		public void BasketItemToggleFavorite(int index)
		{
			BasketItem basketItem = Utils.AppData.Basket.Items[index];
			new FavoriteModel().ToggleFavorite(basketItem.Item.Clone());
		}

		public bool BasketItemCheckIfFavorite(int index)
		{
			BasketItem basketItem = Utils.AppData.Basket.Items[index];
			return new FavoriteModel().IsFavorite(basketItem.Item);
		}

		public void BasketItemPressed(BasketItem item)
		{
			EditBasketItemController editController = new EditBasketItemController(item, OnBasketItemEditDone);
			editController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
			this.PresentViewController(new UINavigationController(editController), true, null);
		}

		#endregion

		#region ICheckoutOrderPlacedListener

		public void DonePressed()
		{
			SaveTransaction(() =>
			{

				new BasketModel().ClearBasket();

				AppData.ShouldRefreshPublishedOffers = true;
				AppData.ShouldRefreshPoints = true;

				UIScreen.MainScreen.Brightness = this.initialScreenBrightness;

				this.DismissViewController(true, () => { });

			});
		}

		#endregion
	}
}

