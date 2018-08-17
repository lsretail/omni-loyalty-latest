using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using System.Linq;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;

namespace Presentation.Screens
{
	public class PublishedOfferDetailController : UIViewController
	{
		private PublishedOfferDetailView rootView;
		private PublishedOffer publishedOffer;
		private List<LoyItem> relatedItems;

		//This event is only active if this is a modal controller
		public delegate void RelatedItemSelectedEventHandler(LoyItem item, Action<bool> dismissSelf);
		public event RelatedItemSelectedEventHandler RelatedItemSelected;

		public PublishedOfferDetailController(PublishedOffer publishedOffer)
		{
			this.publishedOffer = publishedOffer;
			this.relatedItems = new List<LoyItem>();

			if (this.publishedOffer.Type == OfferType.PointOffer)
			{
				this.Title = LocalizationUtilities.LocalizedString("OffersAndCoupons_PointOffers", "Point offers");
			}
			else if (this.publishedOffer.Type == OfferType.SpecialMember)
			{
				this.Title = LocalizationUtilities.LocalizedString("OffersAndCoupons_MemberOffers", "Member offers");
			}
			else if (this.publishedOffer.Type == OfferType.Club)
			{
				this.Title = LocalizationUtilities.LocalizedString("OffersAndCoupons_ClubOffers", "Club offers");
			}
			else
			{
				this.Title = LocalizationUtilities.LocalizedString("OffersAndCoupons_GeneralOffers", "General offers");
			}

			this.rootView = new PublishedOfferDetailView();
			this.rootView.RelatedItemSelected += ViewRelatedItem;
			this.rootView.SeeAllRelatedItems += SeeAllRelatedItems;
			this.rootView.ToggleOfferInBasket += ToggleOfferInBasket;
			this.rootView.UpdateData(this.publishedOffer, this.relatedItems);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Navigationbar
			this.Title = this.publishedOffer != null ? this.publishedOffer.Description : string.Empty;
			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes(false);
			this.NavigationController.NavigationBar.BarTintColor = AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;
			this.View = rootView;

			SetRightBarButtonItems();
			SetAddToBasketButtonVisibility();
			SetAddToBasketButtonTitle();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			SetAddToBasketButtonTitle();
			GetRelatedItems();
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		public void ViewRelatedItem(string id)
		{
			LoyItem item = relatedItems.FirstOrDefault(x => x.Id == id);

			if (item != null)
			{
				if (this.RelatedItemSelected != null)
				{
					//This is a modal screen

					this.RelatedItemSelected(item,
						(dismissSelf) =>
						{
							this.DismissViewController(dismissSelf, null);
						}
					);
				}
				else
				{
					/*
					var itemDetailsController = new ItemDetailsController(item);
					itemDetailsController.RelatedPublishedOfferSelected += HandleRelatedPublishedOfferSelectedFromItemDetailsScreen;
					Utils.UI.AddDismissSelfButtonToController(itemDetailsController, true);
					this.PresentViewController(new UINavigationController(itemDetailsController), true, null);
					*/
				}
			}
		}

		public void SeeAllRelatedItems()
		{
			//this.PresentViewController (new UINavigationController (new RelatedController (RelatedTableSource.RelatedType.item, this.relatedItems, null)), true, null);
		}

		private void HandleRelatedPublishedOfferSelectedFromItemDetailsScreen(PublishedOffer publishedOffer, Action<bool> dismissSelf)
		{
			UINavigationController nc = this.NavigationController;
			PublishedOfferDetailController publishedOfferDetailController = new PublishedOfferDetailController(publishedOffer);
			nc.PopToRootViewController(false);
			nc.PushViewController(publishedOfferDetailController, false);

			dismissSelf(true);
		}

		private string GenerateQRCodeXML()
		{
			BasketQrCode qrModel = new BasketQrCode(AppData.MobileMenu);

			qrModel.Contact = AppData.Contact;
			qrModel.PublishedOffers = new List<PublishedOffer>() { this.publishedOffer };

			return qrModel.Serialize();
		}

		private void GetRelatedItems()
		{
			//ToDo 
		}

		private void SetAddToBasketButtonTitle()
		{
			if (!this.publishedOffer.Selected)
				this.rootView.SetBtnAddToBasketTitle(LocalizationUtilities.LocalizedString("Coupon_Details_AddToBasket", "Add to basket"));
			else
				this.rootView.SetBtnAddToBasketTitle(LocalizationUtilities.LocalizedString("Coupon_Details_RemoveFromBasket", "Remove from basket"));
		}

		private void SetAddToBasketButtonVisibility()
		{
			if (this.publishedOffer.Code == OfferDiscountType.Coupon || this.publishedOffer.Type == OfferType.PointOffer)
			{
				this.rootView.BtnAddToBasketVisibility(false);
			}
			else
			{
				this.rootView.BtnAddToBasketVisibility(true);
			}
		}

		public void ToggleOfferInBasket()
		{
			new BasketModel().TogglePublishedOffer(this.publishedOffer);
			//Utils.Util.AppDelegate.SlideoutBasket.Refresh ();
			SetAddToBasketButtonTitle();

			string bannerText = string.Empty;

			if (this.publishedOffer.Selected && this.publishedOffer.Code == OfferDiscountType.Coupon)
			{
				bannerText = LocalizationUtilities.LocalizedString("SlideoutBasket_CouponAddedToBasket", "Coupon added to basket!");
			}
			else if (!this.publishedOffer.Selected && this.publishedOffer.Code == OfferDiscountType.Coupon)
			{
				bannerText = LocalizationUtilities.LocalizedString("SlideoutBasket_CouponRemovedFromBasket", "Coupon removed from basket!");
			}
			else if (this.publishedOffer.Selected)
			{
				bannerText = LocalizationUtilities.LocalizedString("SlideoutBasket_OfferAddedToBasket", "Offer added to basket!");
			}
			else
			{
				bannerText = LocalizationUtilities.LocalizedString("SlideoutBasket_OfferRemovedFromBasket", "Offer removed from basket!");
			}

			Utils.UI.bannerViewTimer.Start();
			Utils.UI.ShowAddedToBasketBannerView(
				bannerText,
				Image.FromFile("/Branding/Standard/default_map_location_image.png")
			);
		}
	}
}

