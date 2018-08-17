using System;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreAnimation;
using Domain.Utils;
using Domain.Transactions;
using Domain.Menus;
using Domain.Offers;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Screens
{
	public class CheckoutScreen2 : UIViewController
	{
		private UIView orderSentContainerView;
		private UIImageView qrCodeImageView;
		private UITextView qrCodeInstructions;
		private UITextView qrCodeInfo;
		private UIButton btnDone;
		private UIBarButtonItem doneBarButton;

		private UIView sendOrderContainerView;
		private UITableView transactionOverviewTableView;
		private CheckoutDetailsTableSource checkoutDetailsTableSource;
		private UIBarButtonItem cancelBarButton;
		private UIBarButtonItem placeOrderBarButton;

		private nfloat initialScreenBrightness;
		private string backendTransactionId;

		public CheckoutScreen2()
		{}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = Utils.AppColors.BackgroundGray;

			// Navigationbar
			this.Title = NSBundle.MainBundle.LocalizedString("Checkout_Checkout", "Checkout");
			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes (false);
			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;

			// Bar buttons


			UIButton btnCancel = new UIButton (UIButtonType.Custom);
			btnCancel.SetImage (Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconCancel.png"), UIColor.White), UIControlState.Normal);
			btnCancel.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			btnCancel.Frame = new CGRect (0, 0, 30, 30);
			btnCancel.TouchUpInside += (sender, e) => 
			{
				DismissScreen();
			};

			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (btnCancel);

			/*
			this.doneBarButton = new UIBarButtonItem ();
			doneBarButton.Title = NSBundle.MainBundle.LocalizedString("General_Done", "Done");
			doneBarButton.Clicked += (object sender, EventArgs e) => {

				DonePressed();

			};
			*/

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
			this.checkoutDetailsTableSource = new CheckoutDetailsTableSource(SendOrder, OnBasketItemEditDone, BasketItemPressed);
			this.transactionOverviewTableView.Source = this.checkoutDetailsTableSource;
			this.transactionOverviewTableView.TranslatesAutoresizingMaskIntoConstraints = false;
			this.sendOrderContainerView.AddSubview(this.transactionOverviewTableView);

			// Order sent container view

			this.orderSentContainerView = new UIView();
			this.orderSentContainerView.BackgroundColor = UIColor.White;
			this.orderSentContainerView.Hidden = true;
			this.View.AddSubview(this.orderSentContainerView);

			this.qrCodeImageView = new UIImageView();
			qrCodeImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			qrCodeImageView.Image = null;	// Initialize empty QR code, don't have an orderID to put in it yet
			qrCodeImageView.BackgroundColor = UIColor.Clear;
			this.orderSentContainerView.AddSubview(qrCodeImageView);

			this.qrCodeInstructions = new UITextView();
			qrCodeInstructions.UserInteractionEnabled = true;
			qrCodeInstructions.Editable = false;
			qrCodeInstructions.Text = NSBundle.MainBundle.LocalizedString("Checkout_ScanQRCodeInstructions", "");
			qrCodeInstructions.TextColor = Utils.AppColors.PrimaryColor;
			qrCodeInstructions.Font = UIFont.SystemFontOfSize (16);
			qrCodeInstructions.TextAlignment = UITextAlignment.Center;
			this.orderSentContainerView.AddSubview(qrCodeInstructions);

			this.qrCodeInfo = new UITextView();
			qrCodeInfo.UserInteractionEnabled = true;
			qrCodeInfo.Editable = false;
			qrCodeInfo.Text = NSBundle.MainBundle.LocalizedString("Checkout_QRCodeInfo", "");
			qrCodeInfo.TextColor = Utils.AppColors.PrimaryColor;
			qrCodeInfo.Font = UIFont.SystemFontOfSize (12);
			qrCodeInfo.TextAlignment = UITextAlignment.Center;
			this.orderSentContainerView.AddSubview(qrCodeInfo);

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
			
				this.qrCodeImageView.Frame.Top == this.orderSentContainerView.Bounds.Top &&
				this.qrCodeImageView.Frame.Left == this.orderSentContainerView.Bounds.Left &&
				this.qrCodeImageView.Frame.Right == this.orderSentContainerView.Bounds.Right &&
				this.qrCodeImageView.Frame.Height == 260f &&

				this.btnDone.Frame.Bottom == this.orderSentContainerView.Bounds.Bottom - 20f &&
				this.btnDone.Frame.Left == this.orderSentContainerView.Bounds.Left + 20f &&
				this.btnDone.Frame.Right == this.orderSentContainerView.Bounds.Right - 20f &&
				this.btnDone.Frame.Height == 50f &&

				this.qrCodeInfo.Frame.Bottom == this.btnDone.Frame.Top - 20f &&
				this.qrCodeInfo.Frame.Left == this.orderSentContainerView.Bounds.Left + 20f &&
				this.qrCodeInfo.Frame.Right == this.orderSentContainerView.Bounds.Right - 20f &&
				this.qrCodeInfo.Frame.Height == 40f &&

				this.qrCodeInstructions.Frame.Top == this.qrCodeImageView.Frame.Bottom &&
				this.qrCodeInstructions.Frame.Left == this.orderSentContainerView.Bounds.Left + 20f &&
				this.qrCodeInstructions.Frame.Right == this.orderSentContainerView.Bounds.Right - 20f &&
				this.qrCodeInstructions.Frame.Bottom == this.qrCodeInfo.Frame.Top

			);
		}
						
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			this.initialScreenBrightness = UIScreen.MainScreen.Brightness;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			UIScreen.MainScreen.Brightness = this.initialScreenBrightness;
		}
			
		private void SendOrder()
		{
			System.Diagnostics.Debug.WriteLine("Sending order...");
			Utils.UI.ShowLoadingIndicator();

			new OrderModel().OrderSave(
				(orderId) => {
				
					Utils.UI.HideLoadingIndicator();

					System.Diagnostics.Debug.WriteLine("Order sent successfully, ID: " + orderId);
					this.backendTransactionId = orderId;
					this.qrCodeImageView.Image = Utils.QRCode.QRCode.GenerateQRCode(GenerateOrderIdQRCodeXML(orderId));

					Utils.UI.AddFadeTransitionToView(this.orderSentContainerView);
					this.sendOrderContainerView.Hidden = true;
					this.orderSentContainerView.Hidden = false;

					this.NavigationItem.LeftBarButtonItem = null;
					this.NavigationItem.SetRightBarButtonItem(this.doneBarButton, true);

					// Increase screen brightness to help with QR code scanning
					this.initialScreenBrightness = UIScreen.MainScreen.Brightness;
					UIScreen.MainScreen.Brightness = 1;
				
				}, 
				() => {
				
					Utils.UI.HideLoadingIndicator();

					var alert = new UIAlertView(
						NSBundle.MainBundle.LocalizedString("General_Error", "Error"),
						NSBundle.MainBundle.LocalizedString("Checkout_ErrorPlacingOrder", "Couldn't place the order.\r\nPlease try again."),
						null,
						NSBundle.MainBundle.LocalizedString("General_OK", "OK"),
						null
					);
					alert.Show();
				}
			);
		}

		private void DonePressed()
		{
			SaveTransaction(() => {

				new BasketModel().ClearBasket();
				Utils.Util.AppDelegate.SlideoutBasket.Refresh();

			});
				
			// Go back to the first screen in the slideoutmenu (home or menu if home not enabled)
			Util.AppDelegate.SlideoutMenu.NavigationController.PopToRootViewController (false);
			Util.AppDelegate.SlideoutMenu.NavigationController.PushViewController (Util.AppDelegate.SlideoutMenu.Screens.First(), true);

			AppData.ShouldRefreshPublishedOffers = true;
			AppData.ShouldRefreshPoints = true;

			DismissScreen();
		}

		private void BasketItemPressed(BasketItem basketItem)
		{
			EditBasketItemScreen2 editScreen = new EditBasketItemScreen2 (basketItem, OnBasketItemEditDone);
			editScreen.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
			this.PresentViewController(new UINavigationController(editScreen), true, null);
		}

		private void OnBasketItemEditDone()
		{
			Utils.Util.AppDelegate.SlideoutBasket.Refresh();

			if (AppData.Basket.Items.Count <= 0 && AppData.SelectedPublishedOffers.Count <= 0)
			{
				DismissScreen();
			}
			else
			{
				CATransition transition = new CATransition ();
				transition.Duration = 0.3;
				transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				transition.Type = CATransition.TransitionPush;
				transition.Subtype = CATransition.TransitionFade;
				transition.FillMode = CAFillMode.Both;

				this.transactionOverviewTableView.Layer.AddAnimation (transition, null);

				this.checkoutDetailsTableSource.RefreshTotalLabel();
				this.transactionOverviewTableView.ReloadData();
			}
		}

		private string GenerateOrderIdQRCodeXML(string orderId)
		{
			OrderQRCode orderQRCodeModel = new OrderQRCode();
			orderQRCodeModel.Id = orderId;

			return orderQRCodeModel.Serialize();
		}

		private void SaveTransaction(Action onFinish)
		{
			Models.TransactionModel transactionModel = new Models.TransactionModel();
			Transaction newTransaction = transactionModel.CreateTransaction();

			// Overwrite the transaction ID with the backendID (GUID, will be unique)
			if (!String.IsNullOrEmpty(this.backendTransactionId))
				newTransaction.Id = this.backendTransactionId;	

			AppData.Transactions.Add(newTransaction.Clone());

			transactionModel.SyncTransactionsLocally(() => {
				System.Diagnostics.Debug.WriteLine("Transactions synced locally successfully");
				onFinish();
			}, () => {
				System.Diagnostics.Debug.WriteLine("Error while syncing transactions locally");
				onFinish();
			});
		}
			
		private void DismissScreen()
		{
			this.DismissViewController(true, () => {});
		}

		private class CheckoutDetailsTableSource : UITableViewSource
		{
			private UIView headerView;
			private UIView footerView;
			Action SendOrder;
			Action OnBasketItemEdited;
			Action<BasketItem> BasketItemPressed;
	
			public CheckoutDetailsTableSource (Action sendOrder, Action onBasketItemEdited, Action<BasketItem> basketItemPressed)
			{
				this.SendOrder = sendOrder;
				this.OnBasketItemEdited = onBasketItemEdited;
				this.BasketItemPressed = basketItemPressed;

				BuildHeaderView();
				BuildFooterView();
			}

			private void BuildHeaderView()
			{
				headerView = new UIView();
				headerView.BackgroundColor = Utils.AppColors.TransparentWhite;

				UILabel lblVerify = new UILabel()
				{
					Text = NSBundle.MainBundle.LocalizedString("Checkout_Verify", "Please verify your order"),
					Lines = 0,
					TextColor = AppColors.PrimaryColor,
					BackgroundColor = UIColor.Clear,
					TextAlignment = UITextAlignment.Center,
					Font = UIFont.SystemFontOfSize(16)
				};
				lblVerify.SizeToFit();
				headerView.AddSubview(lblVerify);

				const float margin = 5f;

				headerView.ConstrainLayout(() =>

					lblVerify.Frame.Top == headerView.Frame.Top + 2 * margin &&
					lblVerify.Frame.Width == headerView.Frame.Width - 2 * margin &&
					lblVerify.Frame.GetCenterX() == headerView.Frame.GetCenterX()
				);
			}

			private void BuildFooterView()
			{
				footerView = new UIView();
				footerView.BackgroundColor = UIColor.Clear;

				UIView containerView = new UIView();
				containerView.BackgroundColor = Utils.AppColors.TransparentWhite;
				footerView.AddSubview(containerView);

				UIView containerBtnView = new UIView();
				containerBtnView.BackgroundColor = UIColor.Clear;
				footerView.AddSubview(containerBtnView);

				UILabel lblPrice = new UILabel();
				lblPrice.TextColor = Utils.AppColors.PrimaryColor;
				lblPrice.BackgroundColor = UIColor.Clear;
				lblPrice.Text = GetFormattedOrderTotalString();
				lblPrice.BackgroundColor = UIColor.Clear;
				lblPrice.TextAlignment = UITextAlignment.Right;
				lblPrice.TranslatesAutoresizingMaskIntoConstraints = false;
				lblPrice.Tag = 100;
				containerView.AddSubview(lblPrice);

				UIButton btnFavorite = new UIButton();
				btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
				btnFavorite.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				btnFavorite.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
				btnFavorite.BackgroundColor = UIColor.Clear;
				btnFavorite.TranslatesAutoresizingMaskIntoConstraints = false;
				btnFavorite.TouchUpInside += (object sender, EventArgs e) => { 
					
					Transaction newTransaction = new TransactionModel().CreateTransaction();
					Transaction transactionClone = newTransaction.Clone();

					if (new FavoriteModel().IsFavorite(newTransaction))
					{
						new FavoriteModel().ToggleFavorite(transactionClone);
						btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
						Utils.Util.AppDelegate.SlideoutMenu.FavoritesScreen.RefreshWithAnimation();
					}
					else
					{
						UIAlertView alert = new UIAlertView(
							NSBundle.MainBundle.LocalizedString("SlideoutBasket_NameTransaction", "Name transaction"),
							string.Empty,
							null,
							NSBundle.MainBundle.LocalizedString("General_Cancel", "Cancel"),
							new string[]{ NSBundle.MainBundle.LocalizedString("General_OK", "OK") }
						);

						alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
						alert.Clicked += (sender2, e2) => 
						{
							if(e2.ButtonIndex == 0)  //Cancel button pressed
							{
								//do nothing
							}
							else if(e2.ButtonIndex == 1)  //OK button pressed
							{
								transactionClone.Name = alert.GetTextField(0).Text;
								new FavoriteModel().ToggleFavorite(transactionClone);
								btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
								Utils.Util.AppDelegate.SlideoutMenu.FavoritesScreen.RefreshWithAnimation();
							}
						};

						alert.GetTextField(0).Placeholder = NSBundle.MainBundle.LocalizedString("SlideoutBasket_EnterName", "Enter a name (optional)");
						alert.Show();
					}
				};
				containerView.AddSubview(btnFavorite);

				UIButton btnSendOrder = new UIButton();
				btnSendOrder.SetTitle(NSBundle.MainBundle.LocalizedString("Checkout_PlaceOrder", "Place order"), UIControlState.Normal);  
				btnSendOrder.SetTitleColor(UIColor.White, UIControlState.Normal);
				btnSendOrder.BackgroundColor = Utils.AppColors.PrimaryColor;
				btnSendOrder.Layer.CornerRadius = 2;
				btnSendOrder.TouchUpInside += (object sender, EventArgs e) => {

					if (AppData.Basket.Items.Count > 0)
					{
						SendOrder();
					}
					else
					{
						var alert = new UIAlertView(
							NSBundle.MainBundle.LocalizedString("Checkout_PlaceOrder", "Place order"),
							NSBundle.MainBundle.LocalizedString("Checkout_PlaceOrderPleaseAddItems", "Please add some items to your order before placing it."),
							null,
							NSBundle.MainBundle.LocalizedString("General_OK", "OK"),
							null);
						alert.Show();
					}

				};
				containerBtnView.AddSubview(btnSendOrder);

				const float buttonDimensions = 40f;

				footerView.ConstrainLayout(() => 

					containerView.Frame.Left == footerView.Frame.Left &&
					containerView.Frame.Right == footerView.Frame.Right &&
					containerView.Frame.Top == footerView.Frame.Top + 10f &&
					containerView.Frame.Height == buttonDimensions &&

					containerBtnView.Frame.Top == containerView.Bounds.Bottom &&
					containerBtnView.Frame.Left == footerView.Frame.Left &&
					containerBtnView.Frame.Right == footerView.Frame.Right &&
					containerBtnView.Frame.Bottom == footerView.Bounds.Bottom &&
				
					lblPrice.Frame.Top == containerView.Bounds.Top &&
					lblPrice.Frame.Right == containerView.Bounds.Right - 10f &&
					lblPrice.Frame.Left == btnFavorite.Frame.Right &&
					lblPrice.Frame.Bottom == containerView.Bounds.Bottom &&

					btnFavorite.Frame.Left == containerView.Bounds.Left &&
					btnFavorite.Frame.Top == containerView.Bounds.Top &&
					btnFavorite.Frame.Bottom == containerView.Bounds.Bottom &&
					btnFavorite.Frame.Width == buttonDimensions &&

					btnSendOrder.Frame.Top == containerBtnView.Bounds.Top + 20f &&
					btnSendOrder.Frame.Left == containerBtnView.Bounds.Left + 20f &&
					btnSendOrder.Frame.Right == containerBtnView.Bounds.Right - 20f &&
					btnSendOrder.Frame.Height == 40f
				);

			}

			private string GetFormattedOrderTotalString()
			{
				string formattedCurrencyPriceString = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal(AppData.Basket.Amount) : AppData.Basket.Amount.ToString();
				return NSBundle.MainBundle.LocalizedString("Checkout_Total", "Total") + ": " + formattedCurrencyPriceString;
			}

			private UIImage GetFavoriteButtonIcon()
			{
				if (new FavoriteModel().IsFavorite(new TransactionModel().CreateTransaction()))
					return Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconFavoriteON.png"), UIColor.Red);
				else
					return Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconFavoriteOFF.png"), UIColor.Red);
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 3 + 1;	// One dummy section that we put our footer in
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				if (section == (int)Sections.Items)
				{
					return AppData.Basket.Items.Count;
				}
				else if (section == (int)Sections.Offers)
				{
					// TODO: We're just using offers that the contact has, should be able to use general offers not connected to contact?
					return (AppData.SelectedPublishedOffers != null ? AppData.SelectedPublishedOffers.Count : 0);
				}
				else
				{
					return 0;
				}
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				// TODO Different cell type for coupons and offers, now we're reusing the same celltype too much

				CheckoutDetailsItemOverviewCell cell = tableView.DequeueReusableCell (CheckoutDetailsItemOverviewCell.Key) as CheckoutDetailsItemOverviewCell;
				if (cell == null)
					cell = new CheckoutDetailsItemOverviewCell();

				// Set default values
				string description = string.Empty;
				string extraInfo = string.Empty;
				string quantity = string.Empty;
				string formattedPriceString = string.Empty;
				string imageAvgColor = string.Empty;
				string imageId = string.Empty;
				Action<int> onFavoriteButtonPressed = (x) => {};
				Func<int, bool> isFavorited = (x) => { return false; };
				CheckoutDetailsItemOverviewCell.CellType cellType = CheckoutDetailsItemOverviewCell.CellType.Item;

				if (indexPath.Section == (int)Sections.Items)
				{
					BasketItem basketItem = AppData.Basket.Items[indexPath.Row];

					cellType = CheckoutDetailsItemOverviewCell.CellType.Item;

					description = basketItem.Item.Description;
					extraInfo = SlideoutBasket2.GenerateItemExtraInfo(basketItem.Item);
					quantity = basketItem.Quantity.ToString();
					formattedPriceString = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal(basketItem.Price) : basketItem.Price.ToString();

					onFavoriteButtonPressed = HandleFavoriteButtonPress;
					isFavorited = CheckIfFavorited;

					Domain.Images.ImageView imageView = basketItem.Item.Images.FirstOrDefault();
					imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					imageId = (imageView != null ? imageView.Id : string.Empty);
				}
				else if (indexPath.Section == (int)Sections.Offers)
				{
					PublishedOffer publishedOffer = AppData.SelectedPublishedOffers[indexPath.Row];

					cellType = CheckoutDetailsItemOverviewCell.CellType.Offer;

					description = publishedOffer.Description;
					quantity = "1";

					Domain.Images.ImageView imageView = publishedOffer.DefaultImage;
					imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					imageId = (imageView != null ? imageView.Id : string.Empty);
				}
					
				cell.SetValues(indexPath.Row, null, onFavoriteButtonPressed, isFavorited,
					description, extraInfo, quantity, formattedPriceString, imageAvgColor, imageId,
					HandleDeleteButtonPress, cellType);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				BasketItem basketItem = Utils.AppData.Basket.Items[indexPath.Row];
				this.BasketItemPressed(basketItem);

				tableView.DeselectRow(indexPath, true);
			}

			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if (indexPath.Section == (int)Sections.Items)
					return CheckoutDetailsItemOverviewCell.GetCellHeight(SlideoutBasket2.GenerateItemExtraInfo(Utils.AppData.Basket.Items[indexPath.Row].Item));
				else
					return CheckoutDetailsItemOverviewCell.GetCellHeight(string.Empty);
			}

			public override UIView GetViewForHeader (UITableView tableView, nint section)
			{
				if (section == 0)
					return this.headerView;
				else
					return null;
			}

			public override nfloat GetHeightForHeader (UITableView tableView, nint section)
			{
				if (section == 0)
					return 38f;
				else
					return 0f;
			}

			public override UIView GetViewForFooter (UITableView tableView, nint section)
			{
				// TODO The footerview should really be a footerview for the entire table, not just the last section.+
				// Should never be able to scroll the footerview off screen.
				if (section == NumberOfSections(tableView) - 2)
					return this.footerView;
				else 
					return null;
			}

			public override nfloat GetHeightForFooter (UITableView tableView, nint section)
			{
				if (section == NumberOfSections(tableView) - 2)
					return 44f + 40f + 44f;
				else
					return 0f;
			}

			public void HandleDeleteButtonPress(CheckoutDetailsItemOverviewCell.CellType cellType, int cellIndexPathRow)
			{
				System.Diagnostics.Debug.WriteLine("Deleting row of type " + cellType.ToString() + " with index path " + cellIndexPathRow);

				var alert = new UIAlertView(
					NSBundle.MainBundle.LocalizedString("Checkout_RemoveItem", "Remove item"),
					NSBundle.MainBundle.LocalizedString("Checkout_AreYouSureRemoveItem", "Are you sure you want to remove this item?"),
					null,
					NSBundle.MainBundle.LocalizedString("General_OK", "OK"),
					null);
				alert.AddButton(NSBundle.MainBundle.LocalizedString("General_Cancel", "Cancel"));
				alert.Clicked += (object alertSender, UIButtonEventArgs alertEventArgs) => {
					if (alertEventArgs.ButtonIndex == 0)
					{
						// OK pressed

						if (cellType == CheckoutDetailsItemOverviewCell.CellType.Item)
						{
							BasketItem basketItem = Utils.AppData.Basket.Items[cellIndexPathRow];
							new BasketModel().RemoveItemFromBasket (basketItem);
						}
						else if (cellType == CheckoutDetailsItemOverviewCell.CellType.Offer)
						{
							PublishedOffer publishedOffer = AppData.SelectedPublishedOffers[cellIndexPathRow];
							new BasketModel().TogglePublishedOffer (publishedOffer);
						}
							
						RefreshTotalLabel();
						OnBasketItemEdited();
					}
				};
				alert.Show();
			}

			public void HandleFavoriteButtonPress(int cellIndexPathRow)
			{
				BasketItem basketItem = Utils.AppData.Basket.Items[cellIndexPathRow];
				new FavoriteModel().ToggleFavorite(basketItem.Item.Clone());
			}

			public bool CheckIfFavorited(int cellIndexPathRow)
			{
				BasketItem basketItem = Utils.AppData.Basket.Items[cellIndexPathRow];
				return new FavoriteModel().IsFavorite(basketItem.Item);
			}

			public void RefreshTotalLabel()
			{
				UILabel lblOrderTotal = this.footerView.ViewWithTag(100) as UILabel;
				lblOrderTotal.Text = GetFormattedOrderTotalString();
			}

			private enum Sections
			{
				Items,
				Coupons,
				Offers
			}
		}

		private class CheckoutDetailsItemOverviewCell : ItemOverviewTableViewCell
		{
			private Action<CellType, int> onDeleteButtonPressed;
			private CellType cellType;

			public CheckoutDetailsItemOverviewCell() : base()
			{}

			public enum CellType
			{
				Item,
				Offer
			}
		
			public override void SetLayout ()
			{
				base.SetLayout ();

				UIView customContentView = this.ContentView.ViewWithTag(1);

				// Hide reorder button
				UIButton btnReorder = this.ContentView.ViewWithTag(600) as UIButton;
				btnReorder.Hidden = true;

				// Show delete button instead
				UIButton btnDelete = new UIButton();
				btnDelete.SetImage(Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("iconDeleteWhite.png"), UIColor.Gray), UIControlState.Normal);
				btnDelete.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
				btnDelete.TouchUpInside += (object sender, EventArgs e) => { this.onDeleteButtonPressed(this.cellType, this.id); };
				customContentView.AddSubview(btnDelete);

				customContentView.ConstrainLayout(() => 
				
					btnDelete.Frame.Top == btnReorder.Frame.Top &&
					btnDelete.Frame.Left == btnReorder.Frame.Left &&
					btnDelete.Frame.Bottom == btnReorder.Frame.Bottom &&
					btnDelete.Frame.Right == btnReorder.Frame.Right

				);
			}

			public void SetValues(int id, Action<int> onAddToBasketButtonPressed, Action<int> onFavoriteButtonPressed, Func<int, bool> isFavorited,
				string title, string extraInfo, string quantity, string formattedPrice, string imageAvgColorHex, string imageId, 
				Action<CellType, int> onDeleteButtonPressed, CellType cellType)
			{
				base.SetValues(id, onAddToBasketButtonPressed, onFavoriteButtonPressed, isFavorited,
					title, extraInfo, quantity, formattedPrice, imageAvgColorHex, imageId);

				this.onDeleteButtonPressed = onDeleteButtonPressed;
				this.cellType = cellType;

				if (cellType != CellType.Item)
				{
					UIView btnFavorite = this.ContentView.ViewWithTag(300);
					btnFavorite.Hidden = true;
				}
			}
		}
	}
}

