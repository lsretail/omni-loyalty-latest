using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;

using UIKit;

namespace Presentation
{
    // TODO Select quantity?
    public class ItemDetailsView : BaseView
    {
        private const float HEADER_HEIGHT = 220f;

        private ImageCarouselView imageCarousel;
        private UIPageControl imageCarouselPageControl;
        private UIScrollView scrollView;
        private UIView imageWindowView;
        private UIView contentView;
        private UIView indicatorView;

        private ChangeVariantQtyPopUp selectVariantAndQtyPopup;

        private UILabel lblRelatedPublishedOffersTitle;
        private UIScrollView publishedOffersScrollView;
        private UIButton btnSeeRelatedPublishedOffers;

        private UILabel lblTitle;
        private UILabel lblPrice;
        private UIScrollView buttonContainerView;
        private UITextView tvDetails;
        //private UITableView tblPublishedOffers;

        private UIButton btnAddToBasket;
        private UIButton btnAddToWishList;
        private UIButton btnViewAvailability;
        private UIButton btnVariantAndQuantitySelection;

        private UILabel lblAddToBasket;
        private UILabel lblAddToWishList;
        private UILabel lblViewAvailability;

        private nfloat height = 140f;
        private nfloat width = 80f;
        private nfloat padding = 15.0f;

        private bool showVariantSelection;

        public delegate void AddToBasketEventHandler(Action onVariantNotSelected);
        public event AddToBasketEventHandler AddToBasket;

        public delegate void AddToWishlistEventHandler(Action onVariantNotSelected);
        public event AddToWishlistEventHandler AddToWishlist;

        public delegate void ViewAvailabilityEventHandler(Action onVariantNotSelected);
        public event ViewAvailabilityEventHandler ViewAvailability;

        public delegate void ImageSelectedEventHandler(List<ImageView> imageViews, nint selectedImageViewIndex);
        public event ImageSelectedEventHandler ImageSelected;

        public delegate void SelectVariantUomAndQuantityEventHandler(VariantRegistration variantRegistration, string uomId, decimal qty);
        public event SelectVariantUomAndQuantityEventHandler SelectVariantUomAndQuantity;

        public delegate void PublishedOfferSelectedEventHandler(string id);
        public event PublishedOfferSelectedEventHandler PublishedOfferSelected;

        public delegate void SeeAllRelatedOffersAndCouponsEventHandler();
        public event SeeAllRelatedOffersAndCouponsEventHandler SeeAllRelatedOffersAndCoupons;

        public ItemDetailsView()
        {
            this.BackgroundColor = UIColor.White;

            this.imageCarousel = new ImageCarouselView();
            this.imageCarousel.UseImageAverageColorAsBackgroundColor = false;
            this.AddSubview(this.imageCarousel);

            this.imageCarouselPageControl = new UIPageControl();
            this.imageCarouselPageControl.HidesForSinglePage = true;
            this.imageCarouselPageControl.CurrentPageIndicatorTintColor = UIColor.DarkGray;
            this.imageCarouselPageControl.PageIndicatorTintColor = UIColor.LightGray;
            this.AddSubview(this.imageCarouselPageControl);

            this.scrollView = new UIScrollView();
            this.scrollView.BackgroundColor = UIColor.Clear;
            this.AddSubview(this.scrollView);

            this.imageWindowView = new UIView();
            this.imageWindowView.BackgroundColor = UIColor.Clear;
            this.imageWindowView.AddGestureRecognizer(
                new UITapGestureRecognizer(
                    (tapRecognizer) => HandleImageWindowTap(tapRecognizer)
                )
            );
            this.imageWindowView.AddGestureRecognizer(
                new UIPanGestureRecognizer(
                    (panRecognizer) => HandleImageWindowDrag(panRecognizer)
                )
            );
            this.scrollView.AddSubview(this.imageWindowView);
            this.currentImgPoint = new CGPoint(0, 0);
            this.currentImgOffset = new CGPoint(0, 0);

            this.contentView = new UIView();
            this.contentView.BackgroundColor = UIColor.Clear;
            this.scrollView.AddSubview(this.contentView);

            this.lblTitle = new UILabel();
            this.lblTitle.BackgroundColor = UIColor.Clear;
            this.lblTitle.UserInteractionEnabled = false;
            this.lblTitle.TextColor = Utils.AppColors.PrimaryColor;
            this.lblTitle.Font = UIFont.BoldSystemFontOfSize(17);
            this.lblTitle.TextAlignment = UITextAlignment.Left;
            this.contentView.AddSubview(this.lblTitle);

            this.lblPrice = new UILabel();
            this.lblPrice.BackgroundColor = UIColor.Clear;
            this.lblPrice.UserInteractionEnabled = false;
            this.lblPrice.TextColor = Utils.AppColors.PrimaryColor;
            this.lblPrice.Font = UIFont.SystemFontOfSize(14);
            this.lblPrice.TextAlignment = UITextAlignment.Left;
            this.contentView.AddSubview(this.lblPrice);

            this.indicatorView = new UIView();
            this.indicatorView.BackgroundColor = UIColor.Clear;

            this.buttonContainerView = new UIScrollView();
            this.buttonContainerView.BackgroundColor = UIColor.Clear;
            this.contentView.AddSubview(this.buttonContainerView);

            this.tvDetails = new UITextView();
            this.tvDetails.BackgroundColor = UIColor.Clear;
            this.tvDetails.Editable = false;
            this.tvDetails.ScrollEnabled = false;
            this.tvDetails.Font = UIFont.SystemFontOfSize(14);
            this.contentView.AddSubview(this.tvDetails);

            /*
			this.tblPublishedOffers = new UITableView();
			this.tblPublishedOffers.BackgroundColor = UIColor.Clear;
			this.tblPublishedOffers.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			//this.tblPublishedOffers.Source = new ItemDetailsPublishedOffersTableSource();
			//(this.tblPublishedOffers.Source as ItemDetailsPublishedOffersTableSource).PublishedOfferPressed += (PublishedOffer publishedOffer) => {
				if (this.PublishedOfferSelected != null)
					this.PublishedOfferSelected(publishedOffer);
			};
			*/
            //this.contentView.AddSubview(this.tblPublishedOffers);

            #region Button container view buttons

            // Add to basket

            this.btnAddToBasket = new UIButton();
            this.btnAddToBasket.SetBackgroundImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("iconShoppingBasketWhite3.png"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
            this.btnAddToBasket.SetBackgroundImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("iconShoppingBasketWhite3.png"), UIColor.LightGray), UIControlState.Highlighted);
            this.btnAddToBasket.TouchUpInside += (sender, e) =>
            {
                if (this.AddToBasket != null)
                    this.AddToBasket(
                        () =>
                        {
                            this.selectVariantAndQtyPopup.ShowWithAnimation();
                        }
                );
            };
            this.buttonContainerView.AddSubview(this.btnAddToBasket);

            this.lblAddToBasket = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("AddToBasket_AddToBasket", "Add to basket"),
                Font = UIFont.BoldSystemFontOfSize(8),
                TextColor = AppColors.PrimaryColor,
                TextAlignment = UITextAlignment.Center
            };
            this.buttonContainerView.AddSubview(this.lblAddToBasket);

            // Add to wish list

            this.btnAddToWishList = new UIButton();
            this.btnAddToWishList.SetBackgroundImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("WishList28.png"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
            this.btnAddToWishList.SetBackgroundImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("WishList28.png"), UIColor.LightGray), UIControlState.Highlighted);
            this.btnAddToWishList.TouchUpInside += (sender, e) =>
            {
                if (this.AddToWishlist != null)
                    this.AddToWishlist(
                        () =>
                        {
                            this.selectVariantAndQtyPopup.ShowWithAnimation();
                        }
                    );
            };
            this.buttonContainerView.AddSubview(this.btnAddToWishList);

            this.lblAddToWishList = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("WishList_AddToWishList", "Add to Wish list"),
                Font = UIFont.BoldSystemFontOfSize(8),
                TextColor = AppColors.PrimaryColor,
                TextAlignment = UITextAlignment.Center
            };
            this.buttonContainerView.AddSubview(this.lblAddToWishList);

            // View availability

            this.btnViewAvailability = new UIButton();
            this.btnViewAvailability.SetBackgroundImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("Availability.png"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
            this.btnViewAvailability.SetBackgroundImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile("Availability.png"), UIColor.LightGray), UIControlState.Highlighted);
            this.btnViewAvailability.TouchUpInside += (sender, e) =>
            {
                if (this.ViewAvailability != null)
                    this.ViewAvailability(
                        () =>
                        {
                            this.selectVariantAndQtyPopup.ShowWithAnimation();
                        }
                    );
            };
            this.buttonContainerView.AddSubview(this.btnViewAvailability);

            this.lblViewAvailability = new UILabel()
            {
                Text = LocalizationUtilities.LocalizedString("ItemDetails_Availability", "See availability"),
                Font = UIFont.BoldSystemFontOfSize(8),
                TextColor = AppColors.PrimaryColor,
                TextAlignment = UITextAlignment.Center
            };
            this.buttonContainerView.AddSubview(this.lblViewAvailability);

            // Enable/disable features

            if (ShouldShowAddToBasketButton)
            {
                this.btnAddToBasket.Hidden = false;
                this.lblAddToBasket.Hidden = false;
            }
            else
            {
                this.btnAddToBasket.Hidden = true;
                this.lblAddToBasket.Hidden = true;
            }

            if (ShouldShowAddToWishListButton)
            {
                this.btnAddToWishList.Hidden = false;
                this.lblAddToWishList.Hidden = false;
            }
            else
            {
                this.btnAddToWishList.Hidden = true;
                this.lblAddToWishList.Hidden = true;
            }

            if (ShouldShowViewAvailabilityButton)
            {
                this.btnViewAvailability.Hidden = false;
                this.lblViewAvailability.Hidden = false;
            }
            else
            {
                this.btnViewAvailability.Hidden = true;
                this.lblViewAvailability.Hidden = true;
            }

            #endregion

            this.btnVariantAndQuantitySelection = new UIButton();
            this.btnVariantAndQuantitySelection.BackgroundColor = Utils.AppColors.PrimaryColor;
            //this.btnVariantAndQuantitySelection.SetTitle (LocalizationUtilities.LocalizedString("ItemDetails_SelectVariant", "Select variant"), UIControlState.Normal);
            this.btnVariantAndQuantitySelection.SetTitleColor(UIColor.White, UIControlState.Normal);
            this.btnVariantAndQuantitySelection.Layer.CornerRadius = 3f;
            this.btnVariantAndQuantitySelection.TouchUpInside += (object sender, EventArgs e) =>
            {
                this.selectVariantAndQtyPopup.ShowWithAnimation();
            };
            this.contentView.AddSubview(this.btnVariantAndQuantitySelection);
            this.contentView.AddSubview(this.indicatorView);
            // Change quantity and variant popup


            // Related items
            this.lblRelatedPublishedOffersTitle = new UILabel();
            this.lblRelatedPublishedOffersTitle.Text = LocalizationUtilities.LocalizedString("ItemDetails_RelatedOffersAndCoupons", "Related offers and coupons");
            this.lblRelatedPublishedOffersTitle.BackgroundColor = UIColor.Clear;
            this.lblRelatedPublishedOffersTitle.UserInteractionEnabled = false;
            this.lblRelatedPublishedOffersTitle.TextColor = Utils.AppColors.PrimaryColor;
            this.lblRelatedPublishedOffersTitle.Font = UIFont.BoldSystemFontOfSize(17);
            this.lblRelatedPublishedOffersTitle.TextAlignment = UITextAlignment.Left;

            this.btnSeeRelatedPublishedOffers = new UIButton();
            this.btnSeeRelatedPublishedOffers.SetTitleColor(UIColor.Gray, UIControlState.Normal);
            this.btnSeeRelatedPublishedOffers.SetTitle("See all >", UIControlState.Normal);
            this.btnSeeRelatedPublishedOffers.TitleLabel.AdjustsFontSizeToFitWidth = true;
            this.btnSeeRelatedPublishedOffers.TouchUpInside += (sender, e) =>
            {
                if (SeeAllRelatedOffersAndCoupons != null)
                {
                    SeeAllRelatedOffersAndCoupons();
                }
            };

            this.publishedOffersScrollView = new UIScrollView();
            this.publishedOffersScrollView.BackgroundColor = UIColor.Clear;
            this.publishedOffersScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            this.publishedOffersScrollView.ShowsHorizontalScrollIndicator = false;

            this.contentView.AddSubview(this.lblRelatedPublishedOffersTitle);
            this.contentView.AddSubview(this.btnSeeRelatedPublishedOffers);
            this.contentView.AddSubview(this.publishedOffersScrollView);

            this.selectVariantAndQtyPopup = new ChangeVariantQtyPopUp();
            this.selectVariantAndQtyPopup.EditingDone = (decimal quantity, VariantRegistration variantRegistration) =>
            {
                if (this.SelectVariantUomAndQuantity != null)
                    this.SelectVariantUomAndQuantity(variantRegistration, "", quantity);

                this.selectVariantAndQtyPopup.HideWithAnimation();
            };
            Utils.Util.AppDelegate.Window.AddSubview(this.selectVariantAndQtyPopup);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float xMargin = 10f;
            float yMargin = 10f;
            float pageControlHeight = 15f;

            this.imageCarousel.Frame = new CGRect(
                0,
                this.TopLayoutGuideLength,
                this.Frame.Width,
                HEADER_HEIGHT
            );
            this.imageCarouselPageControl.Frame = new CGRect(
                0,
                this.imageCarousel.Frame.Bottom,
                this.imageCarousel.Frame.Width,
                pageControlHeight
            );
            if (this.imageCarouselPageControl.Pages <= 1)
                this.imageCarouselPageControl.Frame = new CGRect(
                    0,
                    this.imageCarousel.Frame.Bottom,
                    this.imageCarousel.Frame.Width,
                    0f
                );  // Hide the page control

            this.scrollView.Frame = this.Frame;
            this.scrollView.ContentInset = new UIEdgeInsets(
                this.TopLayoutGuideLength,
                0,
                this.BottomLayoutGuideLength,
                0
            );
            this.imageWindowView.Frame = new CGRect(
                0,
                0,
                this.scrollView.Frame.Width,
                HEADER_HEIGHT + this.imageCarouselPageControl.Frame.Height
            );
            this.contentView.Frame = new CGRect(
                0,
                this.imageWindowView.Frame.Bottom,
                this.scrollView.Frame.Width,
                0f
            );  // Arbitrary height

            // Content view

            this.lblTitle.Frame = new CGRect(
                xMargin,
                yMargin,
                this.lblTitle.Superview.Frame.Width - 2 * xMargin,
                20f
            );

            this.lblPrice.Frame = new CGRect(
                xMargin,
                this.lblTitle.Frame.Bottom,
                this.lblPrice.Superview.Frame.Width - 2 * xMargin,
                20f
            );

            this.btnVariantAndQuantitySelection.Frame = new CGRect(
                xMargin,
                this.lblPrice.Frame.Bottom + 10f,
                this.btnVariantAndQuantitySelection.Superview.Frame.Width - 2 * xMargin,
                44f
            );
            this.btnVariantAndQuantitySelection.Hidden = false;

            if (!this.showVariantSelection)
            {
                this.btnVariantAndQuantitySelection.Frame = new CGRect(
                    this.btnVariantAndQuantitySelection.Frame.X,
                    this.btnVariantAndQuantitySelection.Frame.Y,
                    this.btnVariantAndQuantitySelection.Frame.Width,
                    0f
                );
                this.btnVariantAndQuantitySelection.Hidden = true;
            }

            this.indicatorView.Frame = new CGRect(
                this.btnVariantAndQuantitySelection.Frame.X,
                this.btnVariantAndQuantitySelection.Frame.Y,
                this.btnVariantAndQuantitySelection.Frame.Width,
                this.btnVariantAndQuantitySelection.Frame.Height
            );

            SetProcessIndicator();

            this.buttonContainerView.Frame = new CGRect(
                xMargin,
                this.btnVariantAndQuantitySelection.Frame.Bottom + 10f,
                this.buttonContainerView.Superview.Frame.Width - 2 * xMargin,
                70f
            );
            this.buttonContainerView.Hidden = false;
            if (!HasActiveButtons)
            {
                this.buttonContainerView.Frame = new CGRect(
                    this.buttonContainerView.Frame.X,
                    this.buttonContainerView.Frame.Y,
                    this.buttonContainerView.Frame.Width,
                    0f
                );  // Hide the button container view
                this.buttonContainerView.Hidden = true;
            }

            this.tvDetails.Frame = new CGRect(
                xMargin,
                this.buttonContainerView.Frame.Bottom,
                this.tvDetails.Superview.Frame.Width - 2 * xMargin,
                0f
            ); // Arbitrary height
            tvDetails.ResizeTextViewHeightToFitContent();   // Adjust details text view height


            this.lblRelatedPublishedOffersTitle.Frame = new CGRect(
                xMargin,
                this.tvDetails.Frame.Bottom + yMargin,
                this.Frame.Width - 2 * xMargin - 60f,
                20f
            );

            this.btnSeeRelatedPublishedOffers.Frame = new CGRect(
                this.lblRelatedPublishedOffersTitle.Frame.Right,
                this.lblRelatedPublishedOffersTitle.Frame.Top,
                60f,
                20f
            );


            this.publishedOffersScrollView.Frame = new CGRect(
                0,
                this.lblRelatedPublishedOffersTitle.Frame.Bottom,
                Frame.Width,
                height + 2 * padding
            );

            /*
			this.tblPublishedOffers.Frame = new CGRect(
				xMargin,
				this.tvDetails.Frame.Bottom + yMargin,
				this.tblPublishedOffers.Superview.Frame.Width - 2 * xMargin,
				(this.tblPublishedOffers.Source as ItemDetailsPublishedOffersTableSource).GetRequiredTableViewHeight(this.tblPublishedOffers)
			);
			*/

            this.contentView.Frame = new CGRect(
                this.contentView.Frame.X,
                this.contentView.Frame.Y,
                this.contentView.Frame.Width,
                this.publishedOffersScrollView.Frame.Bottom
            );  // Adjust contentview height

            // Adjust the scrollview's content size
            // The content size should always be greater than the screen height, that way the scrollview will always be scrollable
            nfloat minScrollViewContentHeight = this.Frame.Height + 1f;
            nfloat requiredScrollViewContentHeight = this.contentView.Frame.Bottom;
            if (requiredScrollViewContentHeight < minScrollViewContentHeight)
                requiredScrollViewContentHeight = minScrollViewContentHeight;
            this.scrollView.ContentSize = new CGSize(this.scrollView.Frame.Width, requiredScrollViewContentHeight);
            this.scrollView.ContentOffset = new CGPoint(0, this.imageWindowView.Frame.Top - this.TopLayoutGuideLength);

            // Let's now make the content view longer (taller) so we will never see the image under it when we scroll it all the way to the top
            var contentViewHeight = this.tvDetails.Frame.Bottom < this.Frame.Height ? this.Frame.Height : this.tvDetails.Frame.Bottom;
            this.contentView.Frame = new CGRect(
                this.contentView.Frame.X,
                this.contentView.Frame.Y,
                this.contentView.Frame.Width,
                contentViewHeight
            );

            Utils.UI.AddColorGradientToView(contentView, Utils.AppColors.TransparentWhite2, UIColor.White, 0.5f, 1.0f);

            #region Button container view buttons

            float xCoordIncrementer = 0f;
            float buttonDimensions = 44f;
            float labelHeight = 20f;
            float buttonAndLabelHeight = buttonDimensions + labelHeight;
            float buttonAndLabelWidth = buttonDimensions + 25f;

            if (ShouldShowAddToBasketButton)
            {
                this.btnAddToBasket.Frame = new CGRect(
                    xCoordIncrementer + buttonAndLabelWidth / 2 - buttonDimensions / 2,
                    (this.buttonContainerView.Frame.Height - buttonAndLabelHeight) / 2,
                    buttonDimensions,
                    buttonDimensions
                );
                this.lblAddToBasket.Frame = new CGRect(
                    xCoordIncrementer,
                    this.btnAddToBasket.Frame.Bottom,
                    buttonAndLabelWidth,
                    labelHeight
                );
                xCoordIncrementer += buttonAndLabelWidth;
            }

            if (ShouldShowAddToWishListButton)
            {
                this.btnAddToWishList.Frame = new CGRect(
                    xCoordIncrementer + buttonAndLabelWidth / 2 - buttonDimensions / 2,
                    (this.buttonContainerView.Frame.Height - buttonAndLabelHeight) / 2,
                    buttonDimensions,
                    buttonDimensions
                );
                this.lblAddToWishList.Frame = new CGRect(
                    xCoordIncrementer,
                    this.btnAddToWishList.Frame.Bottom,
                    buttonAndLabelWidth,
                    labelHeight
                );
                xCoordIncrementer += buttonAndLabelWidth;
            }

            if (ShouldShowViewAvailabilityButton)
            {
                this.btnViewAvailability.Frame = new CGRect(
                    xCoordIncrementer + buttonAndLabelWidth / 2 - buttonDimensions / 2,
                    (this.buttonContainerView.Frame.Height - buttonAndLabelHeight) / 2,
                    buttonDimensions,
                    buttonDimensions
                );
                this.lblViewAvailability.Frame = new CGRect(
                    xCoordIncrementer,
                    this.btnViewAvailability.Frame.Bottom,
                    buttonAndLabelWidth,
                    labelHeight
                );
                xCoordIncrementer += buttonAndLabelWidth;
            }

            #endregion

            // Change variant and quantity popup

            this.selectVariantAndQtyPopup.TopLayoutGuideLength = this.TopLayoutGuideLength;
            this.selectVariantAndQtyPopup.BottomLayoutGuideLength = this.BottomLayoutGuideLength;

            nfloat popupMargin = Utils.Util.AppDelegate.Window.Frame.Width / 14;

            this.selectVariantAndQtyPopup.SetFrame(
                new CGRect(
                    popupMargin,
                    Utils.Util.AppDelegate.Window.Frame.GetMidY() - this.selectVariantAndQtyPopup.GetRequiredHeight() / 2,
                    Utils.Util.AppDelegate.Window.Frame.Width - 2 * popupMargin,
                    this.selectVariantAndQtyPopup.GetRequiredHeight()
                )
            );
        }

        private bool HasActiveButtons
        {
            get
            {
                if (ShouldShowAddToBasketButton)
                    return true;

                if (ShouldShowAddToWishListButton)
                    return true;

                if (ShouldShowViewAvailabilityButton)
                    return true;

                return false;
            }
        }

        private bool ShouldShowAddToBasketButton
        {
            get
            {
                if (EnabledItems.HasBasket && AppData.UserLoggedIn)
                    return true;
                else
                    return false;
            }
        }

        private bool ShouldShowAddToWishListButton
        {
            get
            {
                if (EnabledItems.HasWishLists && AppData.UserLoggedIn)
                    return true;
                else
                    return false;
            }
        }

        private bool ShouldShowViewAvailabilityButton
        {
            get
            {
                if (EnabledItems.HasStoreLocator)
                    return true;
                else
                    return false;
            }
        }

        public UIImageView GetSelectedUIImageView()
        {
            return this.imageCarousel.GetUIImageViewAtIndex((int)this.imageCarouselPageControl.CurrentPage);
        }

        #region Image window gesture stuff

        // We must forward gestures made on the image window to the image carousel below it.

        private CGPoint currentImgPoint;
        private CGPoint currentImgOffset;
        private CGPoint beginningPoint;
        private CGPoint newPoint;

        private void HandleImageWindowTap(UITapGestureRecognizer tap)
        {
            if (this.ImageSelected != null)
                this.ImageSelected(this.imageCarousel.ImageViews, this.imageCarouselPageControl.CurrentPage);
        }

        private void HandleImageWindowDrag(UIPanGestureRecognizer recognizer)
        {
            if (recognizer.State == UIGestureRecognizerState.Began)
            {
                this.beginningPoint = recognizer.TranslationInView(this.imageWindowView);
                this.currentImgOffset.X = this.imageCarousel.ContentOffset.X;
            }

            if (recognizer.State != (UIGestureRecognizerState.Ended | UIGestureRecognizerState.Cancelled | UIGestureRecognizerState.Failed))
            {
                this.newPoint = recognizer.TranslationInView(this.imageWindowView);
                this.currentImgPoint.X = this.beginningPoint.X - this.newPoint.X + this.currentImgOffset.X;
                this.imageCarousel.SetContentOffset(this.currentImgPoint, false);
            }

            if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                nfloat length = this.beginningPoint.X - this.newPoint.X;

                if (length >= 60f)
                {
                    if (this.imageCarouselPageControl.Pages != (this.imageCarouselPageControl.CurrentPage + 1))
                    {
                        this.currentImgPoint.X = (this.imageCarouselPageControl.CurrentPage + 1) * imageCarousel.Frame.Width;
                        this.imageCarousel.SetContentOffset(this.currentImgPoint, true);
                        this.imageCarouselPageControl.CurrentPage = this.imageCarouselPageControl.CurrentPage + 1;
                    }
                    else
                    {
                        this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
                        this.imageCarousel.SetContentOffset(this.currentImgPoint, true);
                    }
                }
                else if (length <= -60f)
                {
                    if (this.imageCarouselPageControl.CurrentPage != 0)
                    {
                        this.currentImgPoint.X = (this.imageCarouselPageControl.CurrentPage - 1) * imageCarousel.Frame.Width;
                        this.imageCarousel.SetContentOffset(this.currentImgPoint, true);
                        this.imageCarouselPageControl.CurrentPage = this.imageCarouselPageControl.CurrentPage - 1;
                    }
                    else
                    {
                        this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
                        this.imageCarousel.SetContentOffset(this.currentImgPoint, true);
                    }
                }
                else
                {
                    this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
                    this.imageCarousel.SetContentOffset(this.currentImgPoint, true);
                }
            }
        }

        #endregion

        public void UpdateView(LoyItem item, decimal selectedQuantity, VariantRegistration selectedVariant, UnitOfMeasure selectedUom, List<PublishedOffer> relatedPublishedOffers)
        {
            this.lblTitle.Text = item.Description;
            this.lblPrice.Text = item.PriceFromVariantsAndUOM(selectedVariant, selectedUom);

            if (selectedUom != null)
                this.lblPrice.Text += " " + LocalizationUtilities.LocalizedString("ItemDetails_Per", "per") + " " + selectedUom.Id.ToLower();

            this.tvDetails.Text = item.Details;
            if (selectedVariant != null && selectedVariant.Images.Count > 0)
            {
                this.imageCarousel.ImageViews = selectedVariant.Images;
            }
            else
            {
                this.imageCarousel.ImageViews = item.Images;
            }
            this.imageCarouselPageControl.Pages = this.imageCarousel.ImageViews.Count;

            bool shouldShowVariantSelection;
            if (item.VariantsRegistration.Count > 1)
                shouldShowVariantSelection = true;
            else
                shouldShowVariantSelection = false;

            this.showVariantSelection = shouldShowVariantSelection;
            if (this.showVariantSelection && selectedVariant != null)
            {
                if (selectedVariant.Id == "ForceSelectVariant")
                {
                    this.btnVariantAndQuantitySelection.SetTitle(LocalizationUtilities.LocalizedString("ChangeVariantQtyPopUp_SelectVariant", "Please select variant"), UIControlState.Normal);
                }
                else
                {
                    this.btnVariantAndQuantitySelection.SetTitle(selectedVariant.ToString(), UIControlState.Normal);
                }
                this.selectVariantAndQtyPopup.UpdateValues(item.VariantsExt, item.VariantsRegistration, item.Description, selectedQuantity, selectedVariant);
            }

            this.publishedOffersScrollView.ContentSize = new CGSize((width + padding) * relatedPublishedOffers.Count, height);
            if (relatedPublishedOffers.Count() > 0)
            {
                for (int i = 0; i < relatedPublishedOffers.Count; i++)
                {
                    ScrollItemView scrollItemView = new ScrollItemView(relatedPublishedOffers[i].Id);
                    scrollItemView.viewClicked = (string id) =>
                    {
                        if (this.PublishedOfferSelected != null)
                        {
                            this.PublishedOfferSelected(id);
                        }
                    };

                    scrollItemView.TextView.Text = relatedPublishedOffers[i].Description;

                    ImageView imageView = relatedPublishedOffers[i].Images.FirstOrDefault();
                    if (imageView != null)
                    {
                        scrollItemView.ImageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageView.AvgColor);
                        Utils.UI.LoadImageToImageView(
                            imageView.Id,
                            false,
                            scrollItemView.ImageView,
                            new ImageSize(300, 300),
                            imageView.Id
                        );

                        scrollItemView.SetFrame(padding * (i + 1) + (i * width), padding, height, width);

                        this.publishedOffersScrollView.Add(scrollItemView);
                    }
                }
                this.lblRelatedPublishedOffersTitle.Hidden = false;
                this.publishedOffersScrollView.Hidden = false;
                this.btnSeeRelatedPublishedOffers.Hidden = false;
            }
            else
            {
                this.lblRelatedPublishedOffersTitle.Hidden = true;
                this.publishedOffersScrollView.Hidden = true;
                this.btnSeeRelatedPublishedOffers.Hidden = true;
            }
            this.LayoutSubviews();
        }

        public void SetProcessIndicator()
        {
            UI.SetLoadingIndicator(this.indicatorView, true);
        }

        public void HideProcessIndicator()
        {
            this.indicatorView.Hidden = true;
        }
    }
}

