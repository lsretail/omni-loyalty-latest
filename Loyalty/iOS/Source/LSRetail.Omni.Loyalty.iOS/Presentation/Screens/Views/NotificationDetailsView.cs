using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation
{
    public class NotificationDetailsView : BaseView
	{
		private const float HEADER_HEIGHT = 220f;

		private ImageCarouselView imageCarousel;
		private UIPageControl imageCarouselPageControl;
		private UIScrollView scrollView;
		private UIView imageWindowView;
		private UIView contentView;

		private UILabel lblValidUntil;
		private UITextView tvDetails;

		public delegate void ImageSelectedEventHandler(List<ImageView> imageViews, nint selectedImageViewIndex);
		public event ImageSelectedEventHandler ImageSelected;

		public NotificationDetailsView ()
		{
			this.BackgroundColor = UIColor.White;

			this.imageCarousel = new ImageCarouselView(UIViewContentMode.ScaleAspectFit);
			this.imageCarousel.UseImageAverageColorAsBackgroundColor = false;

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
			this.currentImgPoint = new CGPoint (0, 0);
			this.currentImgOffset = new CGPoint (0, 0);

			this.contentView = new UIView();
			this.contentView.BackgroundColor = UIColor.Clear;

			this.lblValidUntil = new UILabel();
			this.lblValidUntil.BackgroundColor = UIColor.Clear;
			this.lblValidUntil.UserInteractionEnabled = false;
			this.lblValidUntil.TextColor = Utils.AppColors.PrimaryColor;
			this.lblValidUntil.Font = UIFont.SystemFontOfSize(14);
			this.lblValidUntil.TextAlignment = UITextAlignment.Left;

			this.tvDetails = new UITextView();
			this.tvDetails.BackgroundColor = UIColor.Clear;
			this.tvDetails.Editable = false;
			this.tvDetails.ScrollEnabled = false;					
			this.tvDetails.Font = UIFont.SystemFontOfSize(14);
			this.contentView.AddSubview(this.tvDetails);

			this.AddSubview (this.imageCarousel);
			this.AddSubview (this.imageCarouselPageControl);
			this.AddSubview (this.scrollView);
			this.scrollView.AddSubview (this.imageWindowView);
			this.scrollView.AddSubview (this.contentView);
			this.contentView.AddSubview (this.lblValidUntil);
			this.contentView.AddSubview (this.tvDetails);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

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
				);	// Hide the page control
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
			);	// Initialized as arbitrary height

			#region Contentview layout

			this.lblValidUntil.Frame = new CGRect(
				xMargin,
				yMargin,
				this.lblValidUntil.Superview.Frame.Width - 2 * xMargin,
				(string.IsNullOrEmpty(this.lblValidUntil.Text) ? 0f : 20f)	// Hide it if it contains no value
			);
				
			this.tvDetails.Frame = new CGRect(
				xMargin, 
				this.lblValidUntil.Frame.Bottom, 
				this.tvDetails.Superview.Frame.Width - 2 * xMargin, 
				0f	// Arbitrary height
			); 
			tvDetails.ResizeTextViewHeightToFitContent();	// Adjust details text view height

			#endregion
				
			// Adjust contentview height
			this.contentView.Frame = new CGRect(
				this.contentView.Frame.X, 
				this.contentView.Frame.Y, 
				this.contentView.Frame.Width, 
				this.tvDetails.Frame.Bottom 
			);	

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
		}
			
		public void UpdateView(Notification notification)
		{			
			this.imageCarousel.ImageViews = notification.Images;
			this.imageCarouselPageControl.Pages = this.imageCarousel.ImageViews.Count;

			this.lblValidUntil.Text = notification.ExpiryDate.HasValue ? LocalizationUtilities.LocalizedString("Coupon_Details_ValidUntil", "Valid until") + " " + notification.ExpiryDate.ToString() : string.Empty;

            string detailsText = notification.Description + System.Environment.NewLine + System.Environment.NewLine + notification.Details;

			// Let's use an attributed string to format the heading (aka primarytext) (better than using a label since we don't know how long the heading will be)
			NSMutableAttributedString attributedDetailsText = new NSMutableAttributedString(detailsText);
            NSRange headingRange = new NSRange(0, notification.Description.Length);
			var headingAttributes = new UIStringAttributes {
				ForegroundColor = Utils.AppColors.PrimaryColor,
				Font = UIFont.BoldSystemFontOfSize(17)
			};
			attributedDetailsText.SetAttributes(headingAttributes, headingRange);

			this.tvDetails.AttributedText = attributedDetailsText;

			this.LayoutSubviews ();
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
				this.beginningPoint = recognizer.TranslationInView (this.imageWindowView);
				this.currentImgOffset.X = this.imageCarousel.ContentOffset.X;
			}

			if (recognizer.State != (UIGestureRecognizerState.Ended | UIGestureRecognizerState.Cancelled | UIGestureRecognizerState.Failed))
			{
				this.newPoint = recognizer.TranslationInView (this.imageWindowView);
				this.currentImgPoint.X = this.beginningPoint.X - this.newPoint.X + this.currentImgOffset.X;
				this.imageCarousel.SetContentOffset (this.currentImgPoint, false);
			}

			if (recognizer.State == UIGestureRecognizerState.Ended)
			{
				nfloat length = this.beginningPoint.X - this.newPoint.X;

				if (length >= 60f)
				{
					if (this.imageCarouselPageControl.Pages != (this.imageCarouselPageControl.CurrentPage + 1)) 
					{
						this.currentImgPoint.X = (this.imageCarouselPageControl.CurrentPage + 1) * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
						this.imageCarouselPageControl.CurrentPage = this.imageCarouselPageControl.CurrentPage + 1;
					}
					else
					{
						this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else if (length <= -60f)
				{
					if (this.imageCarouselPageControl.CurrentPage != 0) 
					{
						this.currentImgPoint.X = (this.imageCarouselPageControl.CurrentPage - 1) * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
						this.imageCarouselPageControl.CurrentPage = this.imageCarouselPageControl.CurrentPage - 1;
					}
					else
					{
						this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else
				{
					this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
					this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
				}
			}
		}

		#endregion
	}
}

