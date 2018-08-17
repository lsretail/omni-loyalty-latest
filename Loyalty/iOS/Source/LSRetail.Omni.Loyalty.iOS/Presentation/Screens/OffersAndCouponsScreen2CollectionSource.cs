using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Foundation;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
    public class OffersAndCouponsScreen2CollectionSource : CardCollectionSource
    {
        private OffersAndCouponsScreen2 controller;
        private PossibleDisplayModes activeDisplayMode;
        private new List<OffersAndCouponsCellTemplate> cellTemplateList;
        public PossibleDisplayModes ActiveDisplayMode { get { return this.activeDisplayMode; } set { this.activeDisplayMode = value; } }

        public new bool HasData
        {
            get
            {
                if (this.headerTemplateList != null && this.headerTemplateList.Count > 0)
                    return true;
                else if (this.cellTemplateList != null && this.cellTemplateList.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public OffersAndCouponsScreen2CollectionSource(OffersAndCouponsScreen2 controller, PossibleDisplayModes displayMode)
        {
            this.controller = controller;
            this.activeDisplayMode = displayMode;
            this.cellTemplateList = new List<OffersAndCouponsCellTemplate>();

            BuildHeaderTemplates();
            BuildCellTemplates();
        }

        public override void BuildCellTemplates()
        {
            // TODO
            // We only use the coupons and offers that come with the contact
            // Have to take into account other offers and coupons, not linked with contact?
            if (!AppData.UserLoggedIn)
                return;

            OffersAndCouponsCellTemplate cellTemplate;
            int cellId = 1;

            if (this.activeDisplayMode == PossibleDisplayModes.Coupons)
            {
                foreach (PublishedOffer coupon in AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code == OfferDiscountType.Coupon))
                {
                    cellTemplate = new OffersAndCouponsCellTemplate();
                    cellTemplate.Id = cellId++;
                    cellTemplate.Size = this.controller.CellSize;
                    cellTemplate.Title = coupon.Description;

                    ImageView imgView = coupon.Images.FirstOrDefault();
                    cellTemplate.ImageId = (imgView != null ? imgView.Id : string.Empty);
                    cellTemplate.ImageColorHex = (imgView != null ? imgView.AvgColor : string.Empty);
                    cellTemplate.LocalImage = false;

                    cellTemplate.ObjectToDisplay = coupon;

                    cellTemplate.OnSelected = (x) =>
                    {
                        controller.CellSelected(x);
                    };

                    cellTemplate.OnAddRemoveCouponQRCodePressed = (x) =>
                    {
                        controller.AddRemoveCouponOfferQRCode(x);
                    };

                    cellTemplate.cellType = CellTypes.Coupon;

                    this.cellTemplateList.Add(cellTemplate);
                }
            }
            else if (this.activeDisplayMode == PossibleDisplayModes.Offers)
            {
                //TODO : For demo purposes we display all types of offers
                foreach (PublishedOffer offer in AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon))
                {
                    cellTemplate = new OffersAndCouponsCellTemplate();
                    cellTemplate.Id = cellId++;
                    cellTemplate.Size = this.controller.CellSize;
                    cellTemplate.Title = offer.Description;

                    ImageView imgView = offer.Images.FirstOrDefault();
                    cellTemplate.ImageId = (imgView != null ? imgView.Id : string.Empty);
                    cellTemplate.ImageColorHex = (imgView != null ? imgView.AvgColor : string.Empty);
                    cellTemplate.LocalImage = false;

                    cellTemplate.ObjectToDisplay = offer;

                    cellTemplate.OnSelected = (x) =>
                    {
                        controller.CellSelected(x);
                    };

                    cellTemplate.OnAddRemoveCouponQRCodePressed = (x) =>
                    {
                        controller.AddRemoveCouponOfferQRCode(x);
                    };

                    if (offer.Type == OfferType.General)
                        cellTemplate.cellType = CellTypes.GeneralOffer;
                    else if (offer.Type == OfferType.PointOffer)
                        cellTemplate.cellType = CellTypes.PointOffer;
                    else if (offer.Type == OfferType.SpecialMember)
                        cellTemplate.cellType = CellTypes.MemberOffer;
                    else if (offer.Type == OfferType.Club)
                        cellTemplate.cellType = CellTypes.ClubOffer;
                    else
                        cellTemplate.cellType = CellTypes.GeneralOffer; // Default to general offer

                    this.cellTemplateList.Add(cellTemplate);
                }
            }
            else
            {
                // Unknown display mode, do nothing
                System.Diagnostics.Debug.WriteLine("Unkown display mode in offers and coupons screen");
            }
        }

        public override void BuildHeaderTemplates()
        {
            // Do nothing
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            OffersAndCouponsCellTemplate cellTemplate = cellTemplateList.Where(x => x.cellType == MapDisplayModeAndSectionNumberToCellType(indexPath.Section)).ToList()[indexPath.Row];

            NSString cellKey;


            switch (cellTemplate.Size)
            {
                case (CardCollectionCell.CellSizes.ShortNarrow):
                    cellKey = CardCollectionCell.ShortNarrowCellKey;
                    break;
                case (CardCollectionCell.CellSizes.ShortWide):
                    cellKey = CardCollectionCell.ShortWideCellKey;
                    break;
                case (CardCollectionCell.CellSizes.TallNarrow):
                    cellKey = CardCollectionCell.TallNarrowCellKey;
                    break;
                case (CardCollectionCell.CellSizes.TallWide):
                    cellKey = CardCollectionCell.TallWideCellKey;
                    break;
                default:
                    cellKey = CardCollectionCell.TallWideCellKey;
                    break;
            }

            var cell = collectionView.DequeueReusableCell(cellKey, indexPath) as OffersAndCouponsScreen2Cell;

            if (cellTemplate.ImageColorHex == null || cellTemplate.ImageColorHex == string.Empty)
                cellTemplate.ImageColorHex = "E0E0E0"; // Default to light gray

            int cellId = cellTemplate.Id;
            object objectToDisplay = cellTemplate.ObjectToDisplay;
            Action<object> onSelected = cellTemplate.OnSelected;
            var size = cellTemplate.Size;
            string text = cellTemplate.Title;
            string imageColorHex = cellTemplate.ImageColorHex;
            string imageId = cellTemplate.ImageId;
            bool localImage = cellTemplate.LocalImage;
            Action<object> onAddToBasketButtonPressed = cellTemplate.OnAddRemoveCouponQRCodePressed;

            cell.SetValues(cellId, objectToDisplay, onSelected, size, text, imageColorHex, imageId, localImage, onAddToBasketButtonPressed);

            return cell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            // Section header views

            var supplementaryView = collectionView.DequeueReusableSupplementaryView(UICollectionElementKindSection.Header, OfferAndCouponsHeaderView.Key, indexPath) as OfferAndCouponsHeaderView;

            string title = MapCellTypeToHeaderTitle(MapDisplayModeAndSectionNumberToCellType(indexPath.Section));

            supplementaryView.SetValues(title);

            return supplementaryView;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            if (this.activeDisplayMode == PossibleDisplayModes.Coupons)
            {
                return 1;
            }
            else if (this.activeDisplayMode == PossibleDisplayModes.Offers)
            {
                return this.cellTemplateList.GroupBy(x => x.cellType).Select(group => group.First()).Count();
            }
            else
            {
                return 1;
            }
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.activeDisplayMode == PossibleDisplayModes.Coupons)
            {
                return (nint)this.cellTemplateList.Count;
            }
            else if (this.activeDisplayMode == PossibleDisplayModes.Offers)
            {
                return (nint)this.cellTemplateList.Where(x => x.cellType == MapDisplayModeAndSectionNumberToCellType(section)).Count();
            }
            else
            {
                return (nint)this.cellTemplateList.Count;
            }
        }

        public override bool IsCellWide(int section, int row)
        {
            return CardCollectionCell.IsCellSizeWide(this.cellTemplateList.Where(x => x.cellType == MapDisplayModeAndSectionNumberToCellType(section)).ToList()[row].Size);
        }

        public void RefreshCellTemplates()
        {
            this.cellTemplateList.Clear();
            BuildCellTemplates();
        }

        public void RefreshHeaderTemplates()
        {
            this.headerTemplateList.Clear();
            BuildHeaderTemplates();
        }

        public enum PossibleDisplayModes
        {
            Offers,
            Coupons
        }

        public enum CellTypes
        {
            Coupon,
            PointOffer,
            MemberOffer,
            ClubOffer,
            GeneralOffer
        }

        private string MapCellTypeToHeaderTitle(CellTypes cellType)
        {
            if (cellType == CellTypes.PointOffer)
                return LocalizationUtilities.LocalizedString("OffersAndCoupons_PointOffers", "Point offers").ToUpper();
            else if (cellType == CellTypes.MemberOffer)
                return LocalizationUtilities.LocalizedString("OffersAndCoupons_MemberOffers", "Member offers").ToUpper();
            else if (cellType == CellTypes.GeneralOffer)
                return LocalizationUtilities.LocalizedString("OffersAndCoupons_GeneralOffers", "General offers").ToUpper();
            else if (cellType == CellTypes.ClubOffer)
                return LocalizationUtilities.LocalizedString("OffersAndCoupons_ClubOffers", "Club offers").ToUpper();
            else
                return string.Empty;
        }

        private CellTypes MapDisplayModeAndSectionNumberToCellType(nint sectionNumber)
        {
            // TODO: Decide on a better section grouping ... PointOffers and MemberOffers e.g. in "My offers"

            if (this.activeDisplayMode == PossibleDisplayModes.Coupons)
            {
                return CellTypes.Coupon;
            }
            else if (this.activeDisplayMode == PossibleDisplayModes.Offers)
            {
                // Order sections according to the CellType enum ... PointOffers go to section 1, memberoffers go to section 2, general offers go to section 3
                return this.cellTemplateList.GroupBy(x => x.cellType).Select(group => group.First()).OrderBy(x => x.cellType).ToList()[(int)sectionNumber].cellType;
            }
            else
            {
                // Default to general offer celltype
                return CellTypes.GeneralOffer;
            }
        }

        private class OffersAndCouponsCellTemplate : CellTemplate
        {
            public CellTypes cellType;
            public Action<object> OnAddRemoveCouponQRCodePressed { get; set; }
        }
    }
}