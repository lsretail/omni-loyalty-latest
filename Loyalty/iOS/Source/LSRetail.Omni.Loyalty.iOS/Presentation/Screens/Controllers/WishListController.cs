using UIKit;
using Presentation.Utils;
using CoreGraphics;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using System.Threading.Tasks;

namespace Presentation
{
    public class WishListController : UIViewController
	{
		private WishListView rootView;

		public WishListController ()
		{
			this.rootView = new WishListView ();
			this.rootView.AddItemToBasket += AddItemToBasket;
			this.rootView.ItemSelected += ItemSelected;
			this.rootView.RemoveItemFromWishList += RemoveItemFromWishList;
			this.Title = LocalizationUtilities.LocalizedString("WishList_WishList", "Wish list");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.View = this.rootView;
			SetRightBarButtonItems ();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			this.rootView.UpdateData();
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			/*
			// Basket button
			if (EnabledItems.HasBasket)
				barButtonItemList.Add(Utils.UI.GetBasketBarButtonItem());
			*/

			// Clear wish list button
			UIButton btnClearWishList = new UIButton (UIButtonType.Custom);
			btnClearWishList.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("TrashIcon"), UI.NavigationBarContentColor), UIControlState.Normal);
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
					Utils.UI.ShowAddedToBasketBannerView (LocalizationUtilities.LocalizedString("AddToBasket_ItemsAddedToBasket", "Items added to basket!"), ImageUtilities.FromFile("/Branding/Standard/MapLocationIcon.png"));
				},
				async () =>
				{
					// Failure
					Utils.UI.HideLoadingIndicator();
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("General_Error", "Error"),
						LocalizationUtilities.LocalizedString("AddToBasket_AddWishListToBasketErrorTryAgain", "Could not add wish list to basket, please try again."),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}
			);
		}

		public async void ClearWishList()
		{
			if (AppData.UserLoggedIn)
			{
				if (AppData.Device.UserLoggedOnToDevice.WishList.IsEmpty)
					return;

				var alertResult = await AlertView.ShowAlert(
					this,
					LocalizationUtilities.LocalizedString("WishList_ClearWishList", "Clear wish list"),
					LocalizationUtilities.LocalizedString("WishList_ClearWishListAreYouSure", "Are you sure you want to clear the wish list?"),
					LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
					LocalizationUtilities.LocalizedString("General_No", "No")
				);

				if (alertResult == AlertView.AlertButtonResult.PositiveButton)
				{
					Utils.UI.ShowLoadingIndicator();
					new Models.WishListModel().ClearWishList(
						() =>
						{
							// Success
							Utils.UI.HideLoadingIndicator();
							this.rootView.RefreshWithAnimation();
						},
						async() =>
						{
							// Failure
							Utils.UI.HideLoadingIndicator();
							await AlertView.ShowAlert(
								this,
								LocalizationUtilities.LocalizedString("General_Error", "Error"),
								LocalizationUtilities.LocalizedString("WishList_ClearWishListErrorTryAgain", "Could not clear the wish list, please try again."),
								LocalizationUtilities.LocalizedString("General_OK", "OK")
							);
						}
					);
				}
				else if (alertResult == AlertView.AlertButtonResult.NegativeButton)
				{
				}
			}
		}

		public async Task AddItemToBasket(OneListItem wishListItem)
		{
            // Get the last data for the selected item, including its price
            var item = await new Models.ItemModel().GetItem(wishListItem.Item.Id);

			Utils.UI.ShowLoadingIndicator();
			new Models.BasketModel().AddItemToBasket(
				wishListItem.Quantity <= 0 ? 1 : wishListItem.Quantity, 
                item, 
                wishListItem.VariantReg != null ? wishListItem.VariantReg.Id : string.Empty, 
                wishListItem.UnitOfMeasure != null ? wishListItem.UnitOfMeasure.Id : string.Empty, 
				() =>
				{
					// Success
					Utils.UI.HideLoadingIndicator();
					Utils.UI.ShowAddedToBasketBannerView (LocalizationUtilities.LocalizedString("AddToBasket_ItemAddedToBasket", "Item added to basket!"), ImageUtilities.FromFile("/Branding/Standard/MapLocationIcon.png"));
				},
				async () =>
				{
					// Failure
					Utils.UI.HideLoadingIndicator();
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("General_Error", "Error"),
						LocalizationUtilities.LocalizedString("AddToBasket_AddToBasketErrorTryAgain", "Could not add item to basket, please try again."),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}
			);
		}

		public async void RemoveItemFromWishList(int itemPosition)
		{
			var alertResult = await AlertView.ShowAlert(
				this,
				LocalizationUtilities.LocalizedString("WishList_RemoveFromWishList", "Remove from wish list"),
				LocalizationUtilities.LocalizedString("WishList_AreYouSureRemoveItem", "Are you sure you want to remove this item from your wish list?"),
				LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
				LocalizationUtilities.LocalizedString("General_No", "No")
			);

			if (alertResult == AlertView.AlertButtonResult.PositiveButton)
			{
				Utils.UI.ShowLoadingIndicator();
                bool success = await new Models.WishListModel().RemoveItemFromWishList(
                    itemPosition);
                if (success)
					{
						// Success
						this.rootView.RefreshWithAnimation();
					    Utils.UI.HideLoadingIndicator ();
					}
                else
					{
						// Failure
						Utils.UI.HideLoadingIndicator ();
						await AlertView.ShowAlert(
							this,
							LocalizationUtilities.LocalizedString("General_Error", "Error"),
							LocalizationUtilities.LocalizedString("WishList_RemoveItemFromWishListErrorTryAgain", "Could not remove item from wish list, please try again."),
							LocalizationUtilities.LocalizedString("General_OK", "OK")
						);
					}
				
			}
			else if (alertResult == AlertView.AlertButtonResult.NegativeButton)
			{
			}
		}

		public void ItemSelected(OneListItem wishListItem)
		{
			// Let's clone the item into the itemdetailsscreen ...
			// ... since we can unfavorite the transaction in the transactiondetailsscreen, thereby removing it from memory

            ItemDetailsController itemDetailsController = new ItemDetailsController(wishListItem.Item.ShallowCopy(), wishListItem.VariantReg != null ? wishListItem.VariantReg.Id : string.Empty, wishListItem.UnitOfMeasure != null ? wishListItem.UnitOfMeasureId : string.Empty);
			this.NavigationController.PushViewController (itemDetailsController, true);
		}
	}
}

