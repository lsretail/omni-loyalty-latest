using System;
using System.Collections.Generic;
using Domain.Transactions;
using Foundation;
using Presentation.Models;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;

namespace Presentation.Screens
{
	public class TransactionDetailController : UIViewController, TransactionDetailView.ITransactionDetailListeners
	{
		private TransactionDetailView rootView;
		public Transaction transaction;
		private bool dontShowQRCodeButton;

		public TransactionDetailController(Transaction transaction, bool dontShowQRCodeButton = false)
		{
			this.Title = LocalizationUtilities.LocalizedString("TransactionDetails_Transaction", "Transaction");

			this.rootView = new TransactionDetailView(this);
			this.transaction = transaction;
			this.dontShowQRCodeButton = dontShowQRCodeButton;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			this.rootView.UpdateData(this.transaction, ShouldShowQRCodeButton(), this.dontShowQRCodeButton);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			SetRightBarButtonItems();
			this.View = this.rootView;
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		public void SaleLineSelected(int index)
		{
			SaleLine saleLine = this.transaction.SaleLines[index] as SaleLine;
			ItemDetailController detailController = new ItemDetailController(saleLine.Item.Clone());
			this.NavigationController.PushViewController(detailController, true);
		}

		private bool ShouldShowQRCodeButton()
		{
			// Only show QR code button for transactions that are <= 2 days old.
			// Can't assume that the transaction will still be present in the backend if it is older than that

			// HACK: Can't differentiate between favorite transactions and historical transactions right now.
			// Makes no sense to show QR code for favorite transactions. Hack this away by setting a flag that
			// forbids us from displaying the QR code.
			if (this.dontShowQRCodeButton)
				return false;

			if (!this.transaction.Date.HasValue)
				return false;

			TimeSpan transactionAge = DateTime.Now - this.transaction.Date.Value;

			TimeSpan maximumAge = new TimeSpan(2, 0, 0, 0); // 2 days

			if (transactionAge.CompareTo(maximumAge) > 0)
				return false;
			else
				return true;
		}

		private string GenerateTransactionIdQRCodeXML()
		{
			OrderQRCode orderQRCodeModel = new OrderQRCode();
			orderQRCodeModel.Id = this.transaction.Id;

			return orderQRCodeModel.Serialize();
		}

		public void ShowTransactionQRCode()
		{
			this.PresentViewController(new UINavigationController(new QRCodeController(GenerateTransactionIdQRCodeXML())), true, () => { });
		}

		public void AddTransactionToBasket()
		{
			new BasketModel().AddTransactionToBasket(this.transaction.Clone(),
				() =>
				{
					System.Diagnostics.Debug.WriteLine("Transaction added to basket successfully");
					//Utils.Util.AppDelegate.SlideoutBasket.Refresh();

					Utils.UI.bannerViewTimer.Start();
					Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("SlideoutBasket_ItemsAddedToBasket", "Vörum var bætt í körfuna!"), Image.FromFile("/Branding/Standard/default_map_location_image.png"));

				},
				async () =>
				{
					System.Diagnostics.Debug.WriteLine("Couldn't add (entire) transaction to basket");

					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("General_Error", "Error"),
						LocalizationUtilities.LocalizedString("TransactionDetails_ErrorAddingSomeItemsToBasket", "Couldn't add all items to basket"),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}
			);
		}

		public async void ToggleFavoriteTransaction(Action onSuccess)
		{
			if (new FavoriteModel().IsFavorite(this.transaction))
			{
				new FavoriteModel().ToggleFavorite(this.transaction);
				onSuccess();
			}
			else
			{
				var alertResult = await AlertView.ShowAlertWithTextInput(
					this,
					LocalizationUtilities.LocalizedString("SlideoutBasket_NameTransaction", "Name transaction"),
					string.Empty,
					LocalizationUtilities.LocalizedString("SlideoutBasket_EnterName", "Enter a name (optional)"),
					string.Empty,
					LocalizationUtilities.LocalizedString("General_OK", "OK"),
					LocalizationUtilities.LocalizedString("General_Cancel", "Cancel")
				);

				if (alertResult.ButtonResult == AlertView.AlertButtonResult.PositiveButton)
				{
					this.transaction.Name = alertResult.TextInput.Trim();
					new FavoriteModel().ToggleFavorite(this.transaction);
					onSuccess();
				}
				else if (alertResult.ButtonResult == AlertView.AlertButtonResult.NegativeButton)
				{
					//do nothing
				}
			}
		}

		public bool IsTransactionFavorited()
		{
			return new FavoriteModel().IsFavorite(this.transaction);
		}

		public void ToggleFavoriteSaleLine(object saleLine)
		{
			new FavoriteModel().ToggleFavorite((saleLine as SaleLine).Item.Clone());
		}

		public async void EditTransactionButtonClicked()
		{
			var alertResult = await AlertView.ShowAlertWithTextInput(
				this,
				LocalizationUtilities.LocalizedString("TransactionDetails_RenameTransaction", "Rename transaction"),
				LocalizationUtilities.LocalizedString("TransactionDetails_NewName", "Enter a new name for the transaction"),
				string.Empty,
				string.Empty,
				LocalizationUtilities.LocalizedString("General_OK", "OK"),
				LocalizationUtilities.LocalizedString("General_Cancel", "Cancel")
			);

			if (alertResult.ButtonResult == AlertView.AlertButtonResult.PositiveButton)
			{
				string newTitle = alertResult.TextInput.Trim();
				this.transaction = new FavoriteModel().EditFavorite(this.transaction, newTitle) as Transaction;
				this.rootView.RefreshHeader(this.transaction.Name);
			}
			else if (alertResult.ButtonResult == AlertView.AlertButtonResult.NegativeButton)
			{
				//do nothing
			}
		}

		#region Handle CellAction
		public void MenuItemAddToBasket(int index)
		{
			SaleLine saleLine = this.transaction.SaleLines[index] as SaleLine;
			new BasketModel().AddSaleLineToBasket((saleLine as SaleLine).Clone(),
				() =>
				{
					System.Diagnostics.Debug.WriteLine("Saleline added to basket successfully");
					//Utils.Util.AppDelegate.SlideoutBasket.Refresh();

					Utils.UI.bannerViewTimer.Start();
					Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Image.FromFile("/Branding/Standard/default_map_location_image.png"));
				},
				async () =>
				{
					System.Diagnostics.Debug.WriteLine("Couldn't add saleline to basket");

					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("General_Error", "Error"),
						LocalizationUtilities.LocalizedString("TransactionDetails_ErrorAddingItemToBasket", "Couldn't add item to basket"),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}
			);
		}

		public bool MenuItemToggleFavorite(int index)
		{
			var favoriteModel = new FavoriteModel();

			SaleLine saleLine = this.transaction.SaleLines[index] as SaleLine;
			favoriteModel.ToggleFavorite((saleLine as SaleLine).Item.Clone());
			return favoriteModel.IsFavorite((saleLine as SaleLine).Item.Clone());
		}

		public bool MenuItemCheckIfFavorite(int index)
		{
			SaleLine saleLine = this.transaction.SaleLines[index] as SaleLine;
			return new FavoriteModel().IsFavorite(saleLine.Item);
		}
		#endregion
	}
}

