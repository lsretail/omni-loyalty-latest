using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using Presentation.Screens;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation
{
    public class ItemDetailsController : UIViewController
    {
        private ItemDetailsView rootView;
        private LoyItem item;
        private string variantId;
        private string uomId;
        private List<PublishedOffer> relatedPublishedOffers;
        private bool addToBasketAfterVariantIsChosen;
        private bool addToWishListAfterVariantIsChosen;
        private bool seeAvailabilityAfterVariantIsChosen;
        private decimal quantityToAddToBasket;

        //This event is only active if this is a modal controller
        public delegate void RelatedPublishedOfferSelectedEventHandler(PublishedOffer publishedOffer, Action<bool> dismissSelf);
        public event RelatedPublishedOfferSelectedEventHandler RelatedPublishedOfferSelected;

        public ItemDetailsController(LoyItem item) : this(item, string.Empty, string.Empty)
        { 
        }

        public ItemDetailsController(LoyItem item, string variantId, string uomId)
        {
            this.item = item;

            this.relatedPublishedOffers = new List<PublishedOffer>();
            this.variantId = variantId;
            this.uomId = uomId;
            this.quantityToAddToBasket = 1;

            this.addToBasketAfterVariantIsChosen = false;
            this.addToWishListAfterVariantIsChosen = false;
            this.seeAvailabilityAfterVariantIsChosen = false;

            this.rootView = new ItemDetailsView();
            this.rootView.AddToBasket += AddToBasket;
            this.rootView.AddToWishlist += AddToWishlist;
            this.rootView.ViewAvailability += ViewAvailability;
            this.rootView.ImageSelected += ViewImages;
            this.rootView.SelectVariantUomAndQuantity += SelectVariantUomAndQuantity;
            this.rootView.PublishedOfferSelected += ViewPublishedOffer;
            this.rootView.SeeAllRelatedOffersAndCoupons += SeeAllRelatedOffersAndCoupons;

            this.Title = this.item != null ? this.item.Description : string.Empty;

            VariantRegistration variant = this.item.VariantsRegistration.FirstOrDefault(x => x.Id == variantId);
            UnitOfMeasure uom = this.item.UnitOfMeasures.FirstOrDefault(x => x.Id == uomId);

            this.rootView.UpdateView(this.item, this.quantityToAddToBasket, variant, uom, this.relatedPublishedOffers);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
            this.View = this.rootView;
            SetRightBarButtonItems();
            GetItemData();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
            this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
        }

        private async void GetItemData()
        {
            // This is supposed to be a transparent operation to the user so let's just use the network activity indicator	
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
            LoyItem itemFromGetItem = await new Models.ItemModel().GetItem(this.item.Id);
            if (itemFromGetItem != null)
            {
                this.item = itemFromGetItem;
                VariantRegistration variant = this.item.VariantsRegistration.FirstOrDefault(x => x.Id == variantId);
                UnitOfMeasure uom = this.item.UnitOfMeasures.FirstOrDefault(x => x.Id == uomId);

                if (variant == null && this.item.VariantsRegistration.Count > 0)
                {
                    variant = this.item.VariantsRegistration[0];
                    variantId = variant.Id;
                }
                if (uom == null && this.item.UnitOfMeasures.Count > 0)
                {
                    uom = this.item.UnitOfMeasures.FirstOrDefault(x => x.Id == itemFromGetItem.SalesUomId);
                    uomId = uom.Id;
                }

                List<PublishedOffer> publishedOffers = await new Models.OfferModel().GetPublishedOffersByItemId(this.item.Id, AppData.Device.CardId);
                if (publishedOffers != null)
                {
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;

                    this.relatedPublishedOffers = publishedOffers;
                    this.rootView.UpdateView(this.item, this.quantityToAddToBasket, variant, uom, this.relatedPublishedOffers);
                    this.rootView.HideProcessIndicator();
                }
                else
                {
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;

                    this.rootView.UpdateView(this.item, this.quantityToAddToBasket, variant, uom, this.relatedPublishedOffers);
                    this.rootView.HideProcessIndicator();
                }
            }
            else
            {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
        }

        private void AddToBasket(Action onVariantNotSelected)
        {
            if (variantId == "ForceSelectVariant")
            {
                this.addToBasketAfterVariantIsChosen = true;
                onVariantNotSelected();
            }
            else
            {
                Utils.UI.ShowLoadingIndicator();
                new Models.BasketModel().AddItemToBasket(
                    this.quantityToAddToBasket,
                    this.item,
                    variantId,
                    uomId,
                    () =>
                    {
                        Utils.UI.HideLoadingIndicator();
                        Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("AddToBasket_ItemAddedToBasket", "Item added to basket!"), ImageUtilities.FromFile("/Branding/Standard/MapLocationIcon.png"));
                    },
                    async () =>
                    {
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
        }

        private void AddToWishlist(Action onVariantNotSelected)
        {
            if (variantId == "ForceSelectVariant")
            {
                this.addToWishListAfterVariantIsChosen = true;
                onVariantNotSelected();
            }
            else
            {
                Utils.UI.ShowLoadingIndicator();
                new Models.WishListModel().AddItemToWishList(
                    1,  // Makes little sense to add anything other than 1 item to wishlist
                    this.item,
                    variantId,
                    uomId,
                    () =>
                    {
                        Utils.UI.HideLoadingIndicator();
                        Utils.UI.ShowAddedToWishListBannerView(LocalizationUtilities.LocalizedString("ItemDetails_ItemAddedToWishList", "Item added to wish list!"), ImageUtilities.FromFile("/Branding/Standard/MapLocationIcon.png"));
                    },
                    async () =>
                    {
                        Utils.UI.HideLoadingIndicator();
                        await AlertView.ShowAlert(
                            this,
                            LocalizationUtilities.LocalizedString("General_Error", "Error"),
                            LocalizationUtilities.LocalizedString("WishList_AddItemToWishListErrorTryAgain", "Could not add item to wish list, please try again."),
                            LocalizationUtilities.LocalizedString("General_OK", "OK")
                        );
                    }
                );
            }
        }

        private async void ViewAvailability(Action onVariantNotSelected)
        {
            if (variantId == "ForceSelectVariant")
            {
                this.seeAvailabilityAfterVariantIsChosen = true;
                onVariantNotSelected();
            }
            else
            {
                Utils.UI.ShowLoadingIndicator();

                List<Store> foundStores = await new Models.StoreModel().GetItemsInStock(
                     this.item.Id,
                     variantId,
                     0,
                     0,
                     100,
                     20);

                if (foundStores != null)
                {
                    Utils.UI.HideLoadingIndicator();

                    if (foundStores.Count > 0)
                    {
                        this.NavigationController.PushViewController(
                            new LocationsScreen(
                                new UICollectionViewFlowLayout(),
                                foundStores,
                                true
                            ),
                            true
                        );
                    }
                    else
                    {
                        await AlertView.ShowAlert(
                            this,
                            string.Empty,
                            LocalizationUtilities.LocalizedString("ItemDetails_ItemNotInStock", "Item not in stock"),
                            LocalizationUtilities.LocalizedString("General_OK", "OK")
                        );
                    }
                }
                else
                {
                    Utils.UI.HideLoadingIndicator();
                }
            }
        }

        private void ViewImages(List<ImageView> imageViews, nint selectedImageViewIndex)
        {
            if (selectedImageViewIndex > imageViews.Count() - 1)
                return;

            ImageView imageViewToShow = imageViews[(int)selectedImageViewIndex];
            if (imageViewToShow != null)
            {
                var imageZoomController = new ImageZoomController(imageViewToShow);
                this.NavigationController.PushViewController(imageZoomController, true);
            }
        }

        private void SelectVariantUomAndQuantity(VariantRegistration variantRegistration, string uomid, decimal qty)
        {
            this.quantityToAddToBasket = qty;
            this.rootView.UpdateView(this.item, this.quantityToAddToBasket, variantRegistration, this.item.UnitOfMeasures.FirstOrDefault(x => x.Id == uomId), this.relatedPublishedOffers);

            if (variantRegistration != null)
                this.variantId = variantRegistration.Id;

            if (this.addToBasketAfterVariantIsChosen)
            {
                AddToBasket(() => { });
                this.addToBasketAfterVariantIsChosen = false;
            }
            if (this.addToWishListAfterVariantIsChosen)
            {
                AddToWishlist(() => { });
                this.addToWishListAfterVariantIsChosen = false;
            }
            if (this.seeAvailabilityAfterVariantIsChosen)
            {
                ViewAvailability(() => { });
                this.seeAvailabilityAfterVariantIsChosen = false;
            }
        }

        public void SetRightBarButtonItems()
        {
            List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

            // Share button
            UIBarButtonItem shareItem = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            shareItem.Clicked += (sender, e) =>
            {
                UIImageView imageViewToShare = this.rootView.GetSelectedUIImageView();
                UIImage imageToShare = null;

                if (imageViewToShare != null)
                    imageToShare = imageViewToShare.Image;

                if (imageToShare != null)
                {
                    var activityItems = new NSObject[]
                    {
                        new NSString(item.Details != null ? item.Details : item.Description),
                        imageToShare
                    };

                    UIActivityViewController shareController = new UIActivityViewController(activityItems, null);
                    this.PresentViewController(shareController, true, null);
                }
                else
                {
                    var activityItems = new NSObject[]
                    {
                        new NSString(item.Details != null ? item.Details : item.Description)
                    };

                    UIActivityViewController shareController = new UIActivityViewController(activityItems, null);
                    this.PresentViewController(shareController, true, null);
                }
            };

            barButtonItemList.Add(shareItem);
            this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
        }

        private void ViewPublishedOffer(string id)
        {
            PublishedOffer publishedOffer = relatedPublishedOffers.FirstOrDefault(x => x.Id == id);

            if (publishedOffer != null)
            {
                if (this.RelatedPublishedOfferSelected != null)
                {
                    //This is a modal controlle
                    this.RelatedPublishedOfferSelected(publishedOffer, (dismissSelf) =>
                        {
                            this.DismissViewController(dismissSelf, null);
                        }
                    );
                }
                else
                {
                    var publishedOfferDetailsController = new PublishedOfferDetailController(publishedOffer);
                    publishedOfferDetailsController.RelatedItemSelected += HandleRelatedItemSelectedFromPublishedOffersScreen;
                    Utils.UI.AddDismissSelfButtonToController(publishedOfferDetailsController, true);
                    this.PresentViewController(new UINavigationController(publishedOfferDetailsController), true, null);
                }
            }
        }

        /*
		private void ViewCoupon(Coupon coupon)
		{
			if(this.RelatedCouponSelected != null)
			{
				//This is a modal controller

				this.RelatedCouponSelected(coupon,
				(dismissSelf) =>
					{
						this.DismissViewController(dismissSelf, null);
					}
				);
			}
			else
			{
				var couponDetailsController = new CouponDetailsController(coupon);
				couponDetailsController.RelatedItemSelected += HandleRelatedItemSelectedFromOffersAndCouponScreen;
				Utils.UI.AddDismissSelfButtonToController(couponDetailsController, true);
				this.PresentViewController(new UINavigationController(couponDetailsController), true, null);
			}
		}
		*/

        private void HandleRelatedItemSelectedFromPublishedOffersScreen(LoyItem item, Action<bool> dismissSelf)
        {
            UINavigationController nc = this.NavigationController;
            ItemDetailsController itemDetailsController = new ItemDetailsController(item);
            nc.PopToRootViewController(false);
            nc.PushViewController(itemDetailsController, false);

            dismissSelf(true);

            /*
			if(itemLink.Type == ItemLinkType.Item)
			{
				if(this.item.Id != itemLink.Id)
				{
					Item item = new Item(itemLink.Id);
					item.Description = itemLink.Description;
					item.Images.Add(itemLink.Image);

					UINavigationController nc = this.NavigationController;
					ItemDetailsController itemDetailsController = new ItemDetailsController(item);
					nc.PopToRootViewController(false);
					nc.PushViewController (itemDetailsController, false);
				}

				dismissSelf(true);
			}
			else if(itemLink.Type == ItemLinkType.ProductGroup)
			{
				ItemCategory itemCategory = AppData.ItemCategories.FirstOrDefault(x => x.Id == itemLink.ParentId);
				ProductGroup productGroup = itemCategory.ProductGroups.FirstOrDefault(x => x.Id == itemLink.Id);

				UINavigationController nc = this.NavigationController;
				ItemScreen itemScreen = new ItemScreen (new UICollectionViewFlowLayout(), ItemScreen.ItemListType.Group, itemCategory.Description, itemCategory, null, AppData.CellSize);
				ItemScreen nextItemScreen = new ItemScreen (new UICollectionViewFlowLayout(), ItemScreen.ItemListType.Item, productGroup.Description, itemCategory, productGroup, AppData.CellSize);
				nc.PopToRootViewController(false);
				nc.PushViewController(itemScreen, false);
				nc.PushViewController(nextItemScreen, false);

				dismissSelf(true);

			}
			else if(itemLink.Type == ItemLinkType.ItemCategory)
			{
				ItemCategory itemCategory = AppData.ItemCategories.FirstOrDefault(x => x.Id == itemLink.Id);

				UINavigationController nc = this.NavigationController;
				ItemScreen itemScreen = new ItemScreen (new UICollectionViewFlowLayout(), ItemScreen.ItemListType.Group, itemCategory.Description, itemCategory, null, AppData.CellSize);
				nc.PopToRootViewController(false);
				nc.PushViewController (itemScreen, false);

				dismissSelf(true);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Unknown ItemLink selected: " + itemLink.Description);
			}
			*/
        }

        private void SeeAllRelatedOffersAndCoupons()
        {
            this.PresentViewController(new UINavigationController(new RelatedController(RelatedTableSource.RelatedType.offer, null, this.relatedPublishedOffers, (id) =>
            {
                ViewPublishedOffer(id);
            })), true, null);
        }
    }
}

