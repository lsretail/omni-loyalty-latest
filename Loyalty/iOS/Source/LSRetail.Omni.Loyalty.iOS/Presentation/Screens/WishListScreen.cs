using System;
using UIKit;
using Presentation.Utils;
using System.Collections.Generic;
using Domain.Items;
using Domain.OneList;
using CoreAnimation;
using Foundation;
using CoreGraphics;
using System.Linq;
using Domain.Utils;

namespace Presentation.Screens
{
	/*
	public class WishListScreen : UIViewController
	{
		private UITableView itemTableView;

		private UIView footerContainerView;
		private UIButton btnAddWishListToBasket;

		private UIView noDataView;

		private const float footerContainerViewHeight = 50f;

		public WishListScreen()
		{
			this.Title = NSBundle.MainBundle.LocalizedString("WishList_WishList", "Wish list");

			this.itemTableView = new UITableView();
			this.itemTableView.Source = new WishListTableSource();
			(this.itemTableView.Source as WishListTableSource).AddItemToBasket += AddItemToBasket;
			(this.itemTableView.Source as WishListTableSource).ItemSelected += ItemSelected;
			(this.itemTableView.Source as WishListTableSource).RemoveItemFromWishList += RemoveItemFromWishList;
		}
			
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (this.itemTableView.Source == null)
			{
				this.itemTableView.Source = new WishListTableSource();
				(this.itemTableView.Source as WishListTableSource).AddItemToBasket += AddItemToBasket;
				(this.itemTableView.Source as WishListTableSource).ItemSelected += ItemSelected;
				(this.itemTableView.Source as WishListTableSource).RemoveItemFromWishList += RemoveItemFromWishList;
			}
			if(AppData.UserLoggedIn)
				this.itemTableView.ReloadData();

			RefreshNoDataView ();


		}
			
		public override void ViewDidDisappear (bool animated)
		{
			// Attempt to fix 'random' segfaulting error by nulling the tableview sources
			this.itemTableView.Source = null;

			base.ViewDidDisappear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = UIColor.White;

			this.itemTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			itemTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			itemTableView.Tag = 100;
			itemTableView.Hidden = false;
			this.View.AddSubview(itemTableView);

			this.footerContainerView = new UIView();
			footerContainerView.BackgroundColor = Utils.AppColors.TransparentWhite;
			footerContainerView.Tag = 300;
			this.View.AddSubview(footerContainerView);

			this.btnAddWishListToBasket = new UIButton();
			btnAddWishListToBasket.SetTitle(NSBundle.MainBundle.LocalizedString("WishList_AddToBasket", "Add wish list to basket"), UIControlState.Normal);
			btnAddWishListToBasket.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			btnAddWishListToBasket.BackgroundColor = UIColor.Clear;
			btnAddWishListToBasket.TouchUpInside += (object sender, EventArgs e) => {
				this.btnAddWishListToBasket.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
				OnAddWishListToBasketButtonPressed();
			};
			footerContainerView.AddSubview(btnAddWishListToBasket);

			if (!EnabledItems.HasBasket)
			{
				this.footerContainerView.Hidden = true;
				this.btnAddWishListToBasket.Hidden = true;
			}

			this.View.ConstrainLayout(() =>

				itemTableView.Frame.Top == this.View.Bounds.Top &&
				itemTableView.Frame.Left == this.View.Bounds.Left &&
				itemTableView.Frame.Right == this.View.Bounds.Right &&
				itemTableView.Frame.Bottom == this.View.Bounds.Bottom &&

				footerContainerView.Frame.Bottom == itemTableView.Frame.Bottom &&
				footerContainerView.Frame.Left == itemTableView.Frame.Left &&
				footerContainerView.Frame.Right == itemTableView.Frame.Right &&
				footerContainerView.Frame.Height == footerContainerViewHeight

			);

			footerContainerView.ConstrainLayout(() =>

				btnAddWishListToBasket.Frame.Top == footerContainerView.Bounds.Top &&
				btnAddWishListToBasket.Frame.Left == footerContainerView.Bounds.Left &&
				btnAddWishListToBasket.Frame.Right == footerContainerView.Bounds.Right &&
				btnAddWishListToBasket.Frame.Bottom == footerContainerView.Bounds.Bottom

			);

			SetRightBarButtonItems();
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			// Set the tableviews' contentInsets so that the viewswitcherview doesn't obscure the content
			this.itemTableView.ContentInset = new UIEdgeInsets(itemTableView.ContentInset.Top, itemTableView.ContentInset.Left, this.BottomLayoutGuide.Length + footerContainerViewHeight + 10f, itemTableView.ContentInset.Right);
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			/*
			// Basket button
			if (EnabledItems.HasBasket)
				barButtonItemList.Add(Utils.UI.GetBasketBarButtonItem());
			

			// Clear wish list button
			UIButton btnClearWishList = new UIButton (UIButtonType.Custom);
			btnClearWishList.SetImage(Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconTrash.png"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
			btnClearWishList.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnClearWishList.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			btnClearWishList.Frame = new CGRect (0, 0, 30, 30);
			btnClearWishList.TouchUpInside += (sender, e) => {
				ClearWishList();
			};
			barButtonItemList.Add(new UIBarButtonItem(btnClearWishList));

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private void OnAddWishListToBasketButtonPressed()
		{									
			Utils.UI.ShowLoadingIndicator();
			new Models.BasketModel().AddWishListToBasket(				
				() =>
				{
					// Success
					Utils.UI.HideLoadingIndicator();
					Utils.UI.ShowAddedToBasketBannerView (NSBundle.MainBundle.LocalizedString("AddToBasket_ItemsAddedToBasket", "Items added to basket!"), Utils.Image.FromFile("/Branding/Standard/MapLocationIcon.png"));
				},
				() =>
				{
					// Failure
					Utils.UI.HideLoadingIndicator();
					Utils.UI.ShowAlertView(
						NSBundle.MainBundle.LocalizedString("General_Error", "Error"),
						NSBundle.MainBundle.LocalizedString("AddToBasket_AddWishListToBasketErrorTryAgain", "Could not add wish list to basket, please try again."),
						null,
						null,
						false
					);
				}
			);
		}

		public void ClearWishList()
		{
			if (AppData.Device.UserLoggedOnToDevice.WishList.IsEmpty)
				return;

			Utils.UI.ShowAlertView(
				NSBundle.MainBundle.LocalizedString("WishList_ClearWishList", "Clear wish list"),
				NSBundle.MainBundle.LocalizedString("WishList_ClearWishListAreYouSure", "Are you sure you want to clear the wish list?"),
				() => 
				{				
					Utils.UI.ShowLoadingIndicator();
					new Models.WishListModel().ClearWishList(						
						() =>
						{
							// Success
							Utils.UI.HideLoadingIndicator();
							RefreshWithAnimation();
						},
						() =>
						{
							// Failure
							Utils.UI.HideLoadingIndicator();
							Utils.UI.ShowAlertView(
								NSBundle.MainBundle.LocalizedString("General_Error", "Error"),
								NSBundle.MainBundle.LocalizedString("WishList_ClearWishListErrorTryAgain", "Could not clear the wish list, please try again."),
								null,
								null,
								false
							);
						}
					);	
				},
				() => 
				{
					// Cancel pressed - do nothing
				}
			);
		}
			
		public void AddItemToBasket(OneListItem wishListItem)
		{
			Utils.UI.ShowLoadingIndicator();
			new Models.BasketModel().AddItemToBasket(
				wishListItem.Quantity <= 0 ? 1 : wishListItem.Quantity, 
				wishListItem.Item, 
				wishListItem.Variant != null ? wishListItem.Variant.Id : string.Empty, 
				wishListItem.Uom != null ? wishListItem.Uom.Id : string.Empty, 
				() =>
				{
					// Success
					Utils.UI.HideLoadingIndicator();
					Utils.UI.ShowAddedToBasketBannerView (NSBundle.MainBundle.LocalizedString("AddToBasket_ItemAddedToBasket", "Item added to basket!"), Utils.Image.FromFile("/Branding/Standard/MapLocationIcon.png"));
				},
				() =>
				{
					// Failure
					Utils.UI.HideLoadingIndicator();
					Utils.UI.ShowAlertView(
						NSBundle.MainBundle.LocalizedString("General_Error", "Error"),
						NSBundle.MainBundle.LocalizedString("AddToBasket_AddToBasketErrorTryAgain", "Could not add item to basket, please try again."),
						null,
						null,
						false
					);
				}
			);
		}
			
		public void RemoveItemFromWishList(int itemPosition)
		{									
			Utils.UI.ShowAlertView(
				NSBundle.MainBundle.LocalizedString("WishList_RemoveFromWishList", "Remove from wish list"),
				NSBundle.MainBundle.LocalizedString("WishList_AreYouSureRemoveItem", "Are you sure you want to remove this item from your wish list?"),
				() => 
				{
					Utils.UI.ShowLoadingIndicator();
					new Models.WishListModel().RemoveItemFromWishList(
						itemPosition,
						() =>
						{
							// Success
							Utils.UI.HideLoadingIndicator();
							RefreshWithAnimation();
						},
						() =>
						{
							// Failure
							Utils.UI.HideLoadingIndicator();
							Utils.UI.ShowAlertView(
								NSBundle.MainBundle.LocalizedString("General_Error", "Error"),
								NSBundle.MainBundle.LocalizedString("WishList_RemoveItemFromWishListErrorTryAgain", "Could not remove item from wish list, please try again."),
								null,
								null,
								false
							);
						}
					);
				},
				() => 
				{
					// Do nothing
				}
			);					
		}

		public void ItemSelected(OneListItem wishListItem)
		{
			// Let's clone the item into the itemdetailsscreen ...
			// ... since we can unfavorite the transaction in the transactiondetailsscreen, thereby removing it from memory

			ItemDetailsController itemDetailsController = new ItemDetailsController(wishListItem.Item.ShallowCopy(), wishListItem.Variant != null ? wishListItem.Variant.Id : string.Empty, wishListItem.Uom != null ? wishListItem.Uom.Id : string.Empty);
			this.NavigationController.PushViewController (itemDetailsController, true);
		}

		public void RefreshWithAnimation()
		{
			if (this.itemTableView.Source == null)
				return;

			CATransition transition = new CATransition ();
			transition.Duration = 0.3;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			transition.Type = CATransition.TransitionPush;
			transition.Subtype = CATransition.TransitionFade;
			transition.FillMode = CAFillMode.Both;

			this.itemTableView.Layer.AddAnimation (transition, null);

			this.itemTableView.ReloadData();

			if (!(this.itemTableView.Source as WishListTableSource).HasData)
				ShowNoDataView(NSBundle.MainBundle.LocalizedString("WishList_NoItems", "No items in the wish list."));
			else
				HideNoDataView();
		}

		#region No data view

		private void RefreshNoDataView()
		{
			if (this.itemTableView.Source == null)
				return;
				
			if (!(this.itemTableView.Source as WishListTableSource).HasData)
				ShowNoDataView(NSBundle.MainBundle.LocalizedString("WishList_NoItems", "No items in the wish list."));
			else
				HideNoDataView();
		}

		private void ShowNoDataView(string displayText)
		{
			if (this.noDataView == null)
			{
				UIView ndView = new UIView();
				ndView.Frame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);
				ndView.BackgroundColor = UIColor.Clear;

				UILabel noDataText = new UILabel();
				float labelHeight = 20;
				noDataText.Frame = new CGRect(0, ndView.Bounds.Height/2 - labelHeight/2, ndView.Bounds.Width, labelHeight);
				noDataText.Text = displayText;
				noDataText.TextColor = UIColor.Gray;
				noDataText.TextAlignment = UITextAlignment.Center;
				noDataText.Font = UIFont.SystemFontOfSize(14);
				noDataText.Tag = 10;
				ndView.AddSubview(noDataText);

				this.noDataView = ndView;
				this.View.AddSubview(this.noDataView);
			}
			else
			{
				UILabel noDataText = this.noDataView.ViewWithTag(10) as UILabel;
				noDataText.Text = displayText;
				this.noDataView.Hidden = false;
			}

			this.footerContainerView.Hidden = true;

			this.NavigationItem.RightBarButtonItem.Enabled = false;
		}

		private void HideNoDataView()
		{
			if (this.noDataView != null)
				this.noDataView.Hidden = true;

			if (EnabledItems.HasBasket)
				this.footerContainerView.Hidden = false;

			this.NavigationItem.RightBarButtonItem.Enabled = true;
		}

		#endregion

		#region Table sources

		// TODO Move to separate .cs file
		private class WishListTableSource : UITableViewSource
		{			
			private OneList wishList 
			{ 
				get 
				{ 
					if(AppData.UserLoggedIn)
						return AppData.Device.UserLoggedOnToDevice.WishList;
					else
						return new OneList();
				} 
			}
			public bool HasData { get { return this.wishList.Items.Count > 0; } }

			public delegate void AddItemToBasketEventHandler(OneListItem itemToAdd);
			public AddItemToBasketEventHandler AddItemToBasket;

			public delegate void ItemSelectedEventHandler(OneListItem item);
			public ItemSelectedEventHandler ItemSelected;

			public delegate void RemoveItemFromWishListEventHandler(int itemPosition);
			public RemoveItemFromWishListEventHandler RemoveItemFromWishList;

			public WishListTableSource ()
			{}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return this.wishList.Items.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				WishListTableViewCell cell = tableView.DequeueReusableCell (WishListTableViewCell.Key) as WishListTableViewCell;
				if (cell == null)
					cell = new WishListTableViewCell();

				OneListItem wishListItem = this.wishList.Items[indexPath.Row];

				// Extra info 
				string extraInfo = wishListItem.Variant != null ? wishListItem.Variant.Description : string.Empty;

				// Price
				string formattedPrice = wishListItem.Item.PriceForDisplay; //TODO : Do we need to multiply by qty?

				// Image
				Domain.Images.ImageView imageView = wishListItem.Image;
				string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
				string imageId = (imageView != null ? imageView.Id : string.Empty);

				cell.SetValues(
					indexPath.Row,
					wishListItem.Item.Description, 
					extraInfo, 
					wishListItem.Quantity.ToString(), 
					formattedPrice, 
					imageAvgColor, 
					imageId
				);

				cell.AddToBasket = HandleAddToBasketButtonPress;
				cell.RemoveItemFromWishList = HandleRemoveItemFromWishListButtonPress;

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var wishListItem = this.wishList.Items[indexPath.Row];

				if (this.ItemSelected != null)
					this.ItemSelected(wishListItem);

				tableView.DeselectRow(indexPath, true);
			}

			public void HandleAddToBasketButtonPress(int cellIndexPathRow)
			{
				var wishListItem = this.wishList.Items[cellIndexPathRow];

				if (this.AddItemToBasket != null)
					this.AddItemToBasket(wishListItem);
			}
				
			public void HandleRemoveItemFromWishListButtonPress(int cellIndexPathRow)
			{				
				if (this.RemoveItemFromWishList != null)
					this.RemoveItemFromWishList(cellIndexPathRow);
			}
				
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{				
				return 100;
			}
		}

		#endregion
	}
*/
}

