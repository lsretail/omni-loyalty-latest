using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Presentation.Utils;
using Foundation;
using System.Linq;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation.Screens
{
	public class PublishedOfferDetailView : BaseView
	{
		private const float HEADER_HEIGHT = 220f;

		private ImageCarouselView imageCarousel;
		private UIPageControl imageCarouselPageControl;
		private UIScrollView scrollView;
		private UIView imageWindowView;
		private UIView contentView;

		private UILabel lblRelatedItemsTitle;
		private UIScrollView relatedItemsScrollView;
		private UIButton btnSeeRelatedItems;

		private UIButton btnAddToBasket;
		private UILabel lblTitle;
		private UILabel lblValidUntil;
		private UITextView tvTextDetail;
		private bool shouldShowValidUntil = false;

		private nfloat height = 140f;
		private nfloat width = 80f;
		private nfloat padding = 15.0f;

		public delegate void ImageSelectedEventHandler(List<ImageView> imageViews, nint selectedImageViewIndex);
		public event ImageSelectedEventHandler ImageSelected;

		public delegate void RelatedItemSelectedEventHandler(string id);
		public event RelatedItemSelectedEventHandler RelatedItemSelected;

		public delegate void SeeAllRelatedItemsEventHandler();
		public event SeeAllRelatedItemsEventHandler SeeAllRelatedItems;

		public delegate void ToggleOfferInBasketEventHandler();
		public event ToggleOfferInBasketEventHandler ToggleOfferInBasket;

		public PublishedOfferDetailView()
		{
			this.BackgroundColor = UIColor.White;

			this.imageCarousel = new ImageCarouselView();
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

			// Title
			this.lblTitle = new UILabel();
			this.lblTitle.UserInteractionEnabled = false;
			this.lblTitle.TextColor = AppColors.PrimaryColor;
			this.lblTitle.Font = UIFont.BoldSystemFontOfSize(17);
			this.lblTitle.TextAlignment = UITextAlignment.Left;
			this.contentView.AddSubview(lblTitle);

			// Valid until
			this.lblValidUntil = new UILabel();
			this.lblValidUntil.UserInteractionEnabled = false;
			this.lblValidUntil.TextColor = AppColors.PrimaryColor;
			this.lblValidUntil.Font = UIFont.SystemFontOfSize(14);
			this.lblValidUntil.TextAlignment = UITextAlignment.Left;
			this.lblValidUntil.Tag = 200;
			this.contentView.AddSubview(lblValidUntil);

			// Detail text
			this.tvTextDetail = new UITextView();
			this.tvTextDetail.Editable = false;
			this.tvTextDetail.ScrollEnabled = false;
			this.tvTextDetail.Font = UIFont.SystemFontOfSize(14);
			this.tvTextDetail.BackgroundColor = UIColor.Clear;
			CGSize newSizeThatFits = this.tvTextDetail.SizeThatFits(this.tvTextDetail.Frame.Size);
			CGRect tempFrame = this.tvTextDetail.Frame;
			tempFrame.Size = new CGSize(tempFrame.Size.Width, newSizeThatFits.Height);  // Only adjust the height
			this.tvTextDetail.Frame = tempFrame;
			this.contentView.AddSubview(this.tvTextDetail);

			// Related items
			this.lblRelatedItemsTitle = new UILabel();
			this.lblRelatedItemsTitle.Text = LocalizationUtilities.LocalizedString("OffersAndCoupons_RelatedItems", "Related Items");
			this.lblRelatedItemsTitle.BackgroundColor = UIColor.Clear;
			this.lblRelatedItemsTitle.UserInteractionEnabled = false;
			this.lblRelatedItemsTitle.TextColor = AppColors.PrimaryColor;
			this.lblRelatedItemsTitle.Font = UIFont.BoldSystemFontOfSize(17);
			this.lblRelatedItemsTitle.TextAlignment = UITextAlignment.Left;

			this.btnSeeRelatedItems = new UIButton();
			this.btnSeeRelatedItems.SetTitleColor(UIColor.Gray, UIControlState.Normal);
			this.btnSeeRelatedItems.SetTitle("See all >", UIControlState.Normal);
			this.btnSeeRelatedItems.TitleLabel.AdjustsFontSizeToFitWidth = true;
			this.btnSeeRelatedItems.TouchUpInside += (sender, e) =>
			{
				if (SeeAllRelatedItems != null)
				{
					SeeAllRelatedItems();
				}
			};
			this.btnSeeRelatedItems.Hidden = true;

			this.relatedItemsScrollView = new UIScrollView();
			this.relatedItemsScrollView.BackgroundColor = UIColor.Clear;
			this.relatedItemsScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			this.relatedItemsScrollView.ShowsHorizontalScrollIndicator = false;

			this.btnAddToBasket = new UIButton();
			this.btnAddToBasket.BackgroundColor = AppColors.PrimaryColor;
			this.btnAddToBasket.SetTitleColor(UIColor.White, UIControlState.Normal);
			this.btnAddToBasket.Layer.CornerRadius = 2;
			this.btnAddToBasket.TouchDown += (object sender, EventArgs e) =>
			{
				this.btnAddToBasket.BackgroundColor = Utils.UI.GetLighterVersionOfColor(AppColors.PrimaryColor);
			};
			this.btnAddToBasket.TouchDragOutside += (object sender, EventArgs e) =>
			{
				this.btnAddToBasket.BackgroundColor = AppColors.PrimaryColor;
			};
			this.btnAddToBasket.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.btnAddToBasket.BackgroundColor = Utils.AppColors.PrimaryColor;
				if (ToggleOfferInBasket != null)
				{
					ToggleOfferInBasket();
				}
			};

			this.contentView.AddSubview(this.lblRelatedItemsTitle);
			this.contentView.AddSubview(this.btnSeeRelatedItems);
			this.contentView.AddSubview(this.relatedItemsScrollView);
			this.AddSubview(this.btnAddToBasket);
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

			this.lblValidUntil.Frame = new CGRect(
				xMargin,
				this.lblTitle.Frame.Bottom,
				this.lblTitle.Superview.Frame.Width - 2 * xMargin,
				20f // Set arbitrary height
			);

			if (shouldShowValidUntil)
			{

				this.tvTextDetail.Frame = new CGRect(
					xMargin,
					this.lblValidUntil.Frame.Bottom,
					this.lblTitle.Superview.Frame.Width - 2 * xMargin,
					1f // Set arbitrary height
				);
				this.lblValidUntil.Hidden = false;
			}
			else
			{
				this.tvTextDetail.Frame = new CGRect(
					xMargin,
					this.lblTitle.Frame.Bottom,
					this.lblTitle.Superview.Frame.Width - 2 * xMargin,
					1f // Set arbitrary height
				);

				this.lblValidUntil.Hidden = true;
			}

			Utils.UI.ResizeTextViewHeightToFitContent(this.tvTextDetail);

			this.lblRelatedItemsTitle.Frame = new CGRect(
				xMargin,
				this.tvTextDetail.Frame.Bottom + yMargin,
				this.Frame.Width - 2 * xMargin - 60f,
				20f
			);

			this.btnSeeRelatedItems.Frame = new CGRect(
				this.lblRelatedItemsTitle.Frame.Right,
				this.lblRelatedItemsTitle.Frame.Top,
				60f,
				20f
			);

			this.relatedItemsScrollView.Frame = new CGRect(
				0,
				this.lblRelatedItemsTitle.Frame.Bottom,
				Frame.Width,
				height + 2 * padding
			);

			this.contentView.Frame = new CGRect(
				this.contentView.Frame.X,
				this.contentView.Frame.Y,
				this.contentView.Frame.Width,
				this.relatedItemsScrollView.Frame.Bottom
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

			Utils.UI.AddColorGradientToView(contentView, AppColors.TransparentWhite2, UIColor.White);

			this.btnAddToBasket.Frame = new CGRect(
				0,
				this.Frame.Height - 50f - this.BottomLayoutGuideLength,
				this.Frame.Width,
				50f
			);
		}

		public void UpdateData(PublishedOffer publishedOffer, List<LoyItem> relatedItems)
		{
			this.lblTitle.Text = publishedOffer != null ? publishedOffer.Description : string.Empty;
			if (publishedOffer != null && publishedOffer.ExpirationDate.HasValue)
				shouldShowValidUntil = true;
			this.lblValidUntil.Text = shouldShowValidUntil ? LocalizationUtilities.LocalizedString("Coupon_Details_ValidUntil", "Valid until") + " " + publishedOffer.ExpirationDate.ToString() : string.Empty;
			this.tvTextDetail.Text = publishedOffer == null ? string.Empty : publishedOffer.Details;
			this.imageCarousel.ImageViews = publishedOffer.Images;
			this.imageCarouselPageControl.Pages = this.imageCarousel.ImageViews.Count;

			this.relatedItemsScrollView.ContentSize = new CGSize(
				(width + padding) * relatedItems.Count,
				height
			);

			if (relatedItems.Count() > 0)
			{
				for (int i = 0; i < relatedItems.Count; i++)
				{
					ScrollItemView scrollItemView = new ScrollItemView(relatedItems[i].Id);
					scrollItemView.viewClicked = (string id) =>
					{
						if (this.RelatedItemSelected != null)
						{
							this.RelatedItemSelected(id);
						}
					};
					scrollItemView.TextView.Text = relatedItems[i].Description;
					//UIImageView uiImageView = new UIImageView ();

					//TODO: This might be bug caused by refactor.
					ImageView imageView = relatedItems[i].DefaultImage;
					if (imageView != null)
					{
						scrollItemView.ImageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(imageView.AvgColor);
						Utils.UI.LoadImageToImageView(
							imageView.Id,
							false,
							scrollItemView.ImageView,
							new ImageSize(300, 300),
							imageView.Id
						);

						scrollItemView.SetFrame(padding * (i + 1) + (i * width), padding, height, width);

						this.relatedItemsScrollView.Add(scrollItemView);
					}
					else { }
				}

				this.lblRelatedItemsTitle.Hidden = false;
				this.relatedItemsScrollView.Hidden = false;
				this.lblRelatedItemsTitle.Hidden = false;
			}
			else
			{
				this.lblRelatedItemsTitle.Hidden = true;
				this.relatedItemsScrollView.Hidden = true;
				this.lblRelatedItemsTitle.Hidden = true;
			}

			this.LayoutSubviews();
		}

		public void BtnAddToBasketVisibility(bool hidden)
		{
			this.btnAddToBasket.Hidden = hidden;
		}

		public void SetBtnAddToBasketTitle(string title)
		{
			this.btnAddToBasket.SetTitle(title, UIControlState.Normal);
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

