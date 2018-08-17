using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation
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
            this.rootView.ImageSelected += ViewImages;
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
            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
            this.View = rootView;

            SetRightBarButtonItems();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            GetRelatedItems();
        }

        public void SetRightBarButtonItems()
        {
            List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();
            if (this.NavigationItem.RightBarButtonItems != null && this.NavigationItem.RightBarButtonItems.Count() > 0)
            {
                // There are some bar button items already present
                foreach (var currentItem in this.NavigationItem.RightBarButtonItems)
                    barButtonItemList.Add(currentItem);
            }

            if (this.publishedOffer.Type == OfferType.PointOffer)
            {
                UIButton scanBarcodeButton = new UIButton();
                scanBarcodeButton.Frame = new CGRect(0, 0, 30, 30);
                scanBarcodeButton.SetImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("iconQRCodeWhite.png"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
                scanBarcodeButton.TouchUpInside += (sender, e) =>
               {
                   this.PresentViewController(new UINavigationController(new QRCodeController(GenerateQRCodeXML(), QRCodeViewType.PublishedOffers, this.publishedOffer)), true, () => { });
               };

                barButtonItemList.Add(new UIBarButtonItem(scanBarcodeButton));
            }

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
                    var itemDetailsController = new ItemDetailsController(item);
                    itemDetailsController.RelatedPublishedOfferSelected += HandleRelatedPublishedOfferSelectedFromItemDetailsScreen;
                    Utils.UI.AddDismissSelfButtonToController(itemDetailsController, true);
                    this.PresentViewController(new UINavigationController(itemDetailsController), true, null);
                }
            }
        }

        public void SeeAllRelatedItems()
        {
            this.PresentViewController(new UINavigationController(new RelatedController(RelatedTableSource.RelatedType.item, this.relatedItems, null, (id) =>
            {
                ViewRelatedItem(id);
            })), true, null);
        }

        private void HandleRelatedPublishedOfferSelectedFromItemDetailsScreen(PublishedOffer pubOffer, Action<bool> dismissSelf)
        {
            UINavigationController nc = this.NavigationController;
            PublishedOfferDetailController publishedOfferDetailController = new PublishedOfferDetailController(pubOffer);
            nc.PopToRootViewController(false);
            nc.PushViewController(publishedOfferDetailController, false);

            dismissSelf(true);
        }

        private string GenerateQRCodeXML()
        {
            string xml = string.Format("<mobiledevice><contactid>{0}</contactid><accountid>{1}</accountid><cardid>{2}</cardid>",
                AppData.Device.UserLoggedOnToDevice.Id,
                AppData.Device.UserLoggedOnToDevice.Account.Id,
                AppData.Device.UserLoggedOnToDevice.Card.Id);

            xml += "<coupons>";
            if (this.publishedOffer.Code == OfferDiscountType.Coupon)
            {
                xml += string.Format("<cid>{0}</cid>", this.publishedOffer.Id);
            }
            xml += "</coupons>";

            xml += "<offers>";
            if (this.publishedOffer.Code != OfferDiscountType.Coupon)
            {
                xml += string.Format("<oid>{0}</oid>", this.publishedOffer.Id);
            }
            xml += "</offers>";
            xml += "</mobiledevice>";
            return xml;
        }

        private async void GetRelatedItems()
        {
            // This is supposed to be a transparent operation to the user so let's just use the network activity indicator
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

            List<LoyItem> items = await new Models.ItemModel().GetItemsByPublishedOfferId(this.publishedOffer.Id, 4);
            if (items != null)
            {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                this.relatedItems = items;

                this.rootView.UpdateData(this.publishedOffer, this.relatedItems);
            }
            else
            {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
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
    }
}
