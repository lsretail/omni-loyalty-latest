using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation
{
    public class LocationDetailView : BaseView
    {
        private const float HEADER_HEIGHT = 220f;

        private ImageCarouselView imageCarousel;
        private UIPageControl imageCarouselPageControl;
        private UIScrollView scrollView;
        private UIView imageWindowView;
        private UIView contentView;

        private UILabel lblTitle;
        private UITextView tvAddress;
        private UITextView tvPhone;
        private UITextView tvStoreHourTypeAndDays;
        private UITextView tvOpeningHours;
        private UILabel lblOpeningHours;

        public delegate void ImageSelectedEventHandler(List<ImageView> imageViews, nint selectedImageViewIndex);
        public event ImageSelectedEventHandler ImageSelected;

        public LocationDetailView()
        {
            this.BackgroundColor = UIColor.White;

            this.imageCarousel = new ImageCarouselView();

            this.imageCarouselPageControl = new UIPageControl();
            this.imageCarouselPageControl.HidesForSinglePage = true;
            this.imageCarouselPageControl.CurrentPageIndicatorTintColor = UIColor.DarkGray;
            this.imageCarouselPageControl.PageIndicatorTintColor = UIColor.LightGray;

            this.scrollView = new UIScrollView();
            this.scrollView.BackgroundColor = UIColor.Clear;

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
            this.currentImgPoint = new CGPoint(0, 0);
            this.currentImgOffset = new CGPoint(0, 0);

            this.contentView = new UIView();
            this.contentView.BackgroundColor = UIColor.Clear;

            this.lblTitle = new UILabel();
            this.lblTitle.BackgroundColor = UIColor.Clear;
            this.lblTitle.UserInteractionEnabled = false;
            this.lblTitle.TextColor = Utils.AppColors.PrimaryColor;
            this.lblTitle.Font = UIFont.BoldSystemFontOfSize(17);
            this.lblTitle.TextAlignment = UITextAlignment.Left;

            this.tvAddress = new UITextView();
            this.tvAddress.Editable = false;
            this.tvAddress.ScrollEnabled = false;
            this.tvAddress.Font = UIFont.SystemFontOfSize(14);
            this.tvAddress.BackgroundColor = UIColor.Clear;

            // Phone text
            this.tvPhone = new UITextView();
            this.tvPhone.Editable = false;
            this.tvPhone.ScrollEnabled = false;
            this.tvPhone.Font = UIFont.SystemFontOfSize(14);
            this.tvPhone.BackgroundColor = UIColor.Clear;
            this.tvPhone.Selectable = true;
            this.tvPhone.DataDetectorTypes = UIDataDetectorType.PhoneNumber;

            // Opening hours label
            this.lblOpeningHours = new UILabel();
            this.lblOpeningHours.UserInteractionEnabled = false;
            this.lblOpeningHours.Text = LocalizationUtilities.LocalizedString("Location_Details_OpeningHours", "Opening hours");
            this.lblOpeningHours.TextColor = Utils.AppColors.PrimaryColor;
            this.lblOpeningHours.Font = UIFont.BoldSystemFontOfSize(14);
            this.lblOpeningHours.BackgroundColor = UIColor.Clear;
            this.lblOpeningHours.TextAlignment = UITextAlignment.Left;
            this.lblOpeningHours.Hidden = true;


            // StoreHourType and days
            this.tvStoreHourTypeAndDays = new UITextView();
            this.tvStoreHourTypeAndDays.Editable = false;
            this.tvStoreHourTypeAndDays.ScrollEnabled = false;
            this.tvStoreHourTypeAndDays.Font = UIFont.SystemFontOfSize(14);
            this.tvStoreHourTypeAndDays.BackgroundColor = UIColor.Clear;

            // Opening hours
            this.tvOpeningHours = new UITextView();
            this.tvOpeningHours.Editable = false;
            this.tvOpeningHours.ScrollEnabled = false;
            this.tvOpeningHours.Font = UIFont.SystemFontOfSize(14);
            this.tvOpeningHours.BackgroundColor = UIColor.Clear;

            this.AddSubview(this.imageCarousel);
            this.AddSubview(this.imageCarouselPageControl);
            this.AddSubview(this.scrollView);
            this.scrollView.AddSubview(this.imageWindowView);
            this.scrollView.AddSubview(this.contentView);
            this.contentView.AddSubview(this.lblTitle);
            this.contentView.AddSubview(this.tvAddress);
            this.contentView.AddSubview(this.tvPhone);
            this.contentView.AddSubview(this.lblOpeningHours);
            this.contentView.AddSubview(this.tvStoreHourTypeAndDays);
            this.contentView.AddSubview(this.tvOpeningHours);
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
            {
                this.imageCarouselPageControl.Frame = new CGRect(
                    0,
                    this.imageCarousel.Frame.Bottom,
                    this.imageCarousel.Frame.Width,
                    0f
                );  // Hide the page control
            }

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
            );

            this.lblTitle.Frame = new CGRect(
                xMargin,
                yMargin,
                this.lblTitle.Superview.Frame.Width - 2 * xMargin,
                20f
            );

            this.tvAddress.Frame = new CGRect(
                xMargin,
                this.lblTitle.Frame.Bottom,
                this.lblTitle.Superview.Frame.Width - 2 * xMargin,
                1f // Set arbitrary height
            );

            tvAddress.ResizeTextViewHeightToFitContent();

            this.tvPhone.Frame = new CGRect(
                xMargin,
                this.tvAddress.Frame.Bottom,
                this.lblTitle.Superview.Frame.Width - 2 * xMargin,
                1f // Set arbitrary height
            );

            tvPhone.ResizeTextViewHeightToFitContent();

            this.lblOpeningHours.Frame = new CGRect(
                xMargin,
                this.tvPhone.Frame.Bottom,
                this.lblTitle.Superview.Frame.Width - 2 * xMargin,
                20f
            );

            this.tvStoreHourTypeAndDays.Frame = new CGRect(
                xMargin,
                this.lblOpeningHours.Frame.Bottom + 5f,
                (this.contentView.Frame.Width - 2 * xMargin) / 2,
                1f
            ); // Set arbitrary height

            tvStoreHourTypeAndDays.ResizeTextViewHeightToFitContent();

            this.tvOpeningHours.Frame = new CGRect(
                this.tvStoreHourTypeAndDays.Frame.Right,
                this.lblOpeningHours.Frame.Bottom + 5f,
                (this.contentView.Frame.Width - 2 * xMargin) / 2,
                1f
            ); // Set arbitrary height
            System.Diagnostics.Debug.WriteLine("this is the text" + this.tvOpeningHours.Text);
            tvOpeningHours.ResizeTextViewHeightToFitContent();

            this.contentView.Frame = new CGRect(
                this.contentView.Frame.X,
                this.contentView.Frame.Y,
                this.contentView.Frame.Width,
                (nfloat)System.Math.Max(tvStoreHourTypeAndDays.Frame.Bottom, tvOpeningHours.Frame.Bottom)
            );  // Adjust contentview height

            // Adjust the scrollview's content size
            // The content size should always be greater than the screen height, that way the scrollview will always be scrollable
            nfloat minScrollViewContentHeight = this.Frame.Height + 1f;
            nfloat requiredScrollViewContentHeight = this.contentView.Frame.Bottom;
            if (requiredScrollViewContentHeight < minScrollViewContentHeight)
                requiredScrollViewContentHeight = minScrollViewContentHeight;
            this.scrollView.ContentSize = new CGSize(this.scrollView.Frame.Width, requiredScrollViewContentHeight);
            this.scrollView.ContentOffset = new CGPoint(0, this.imageWindowView.Frame.Top - this.TopLayoutGuideLength);

            this.contentView.Frame = new CGRect(
                this.contentView.Frame.X,
                this.contentView.Frame.Y,
                this.contentView.Frame.Width,
                this.contentView.Frame.Height + this.imageWindowView.Frame.Bottom
            );

            Utils.UI.AddColorGradientToView(contentView, Utils.AppColors.TransparentWhite2, UIColor.White);
            //this.contentView.BackgroundColor = UIColor.Red;
        }

        public void UpdateView(Store store, string storeHourTypeAndDays, string openingHours)
        {
            this.lblTitle.Text = store.Description;
            this.tvAddress.Text = store.FormatAddress;
            this.tvPhone.Text = LocalizationUtilities.LocalizedString("Location_Details_Phone", "Phone") + ": " + store.Phone;
            this.tvStoreHourTypeAndDays.Text = storeHourTypeAndDays;
            this.tvOpeningHours.Text = openingHours;
            this.imageCarousel.ImageViews = store.Images;
            this.imageCarouselPageControl.Pages = this.imageCarousel.ImageViews.Count;

            if (string.IsNullOrEmpty(storeHourTypeAndDays) && string.IsNullOrEmpty(openingHours))
                this.lblOpeningHours.Hidden = true;
            else
                this.lblOpeningHours.Hidden = false;

            this.LayoutSubviews();
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
    }
}

