using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;
using UIKit;

namespace Presentation.Screens
{
    public class OffersAndCouponsScreen2Cell : CardCollectionCell
	{
		private CellLayouts layout;

		private Action<object> onAddRemoveCouponQRCodePressed;

		private float addCouponToQRCodeButtonWidth = 40f;

		[Export ("initWithFrame:")]
		public OffersAndCouponsScreen2Cell (CGRect frame) : base (frame)
		{}

		//Note : We save selected offers and coupons, but never sync it with the web service.
		//so when we refresh offers and coupons, the user has to reselect offers and coupons

		#region Layout

		protected override void SetLayout ()
		{
			ClearSubviews();

			if (this.layout == CellLayouts.ImageWithOverlay)
				SetLayoutImageWithOverlay();
			else if (this.layout == CellLayouts.ImageAndTextContainer)
				SetLayoutImageAndTextContainer();
			else
				SetLayoutImageWithOverlay();	// Default to this
		}

		private void SetLayoutImageWithOverlay ()
		{
			// Image view
			UIImageView imageView = new UIImageView ();
			imageView.Frame = this.ContentView.Frame;
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;
			this.ContentView.AddSubview (imageView);

			// Overlay view
			UIView overlayView = new UIView ();
			int overlayViewHeight = (int)Math.Floor (this.ContentView.Bounds.Height / 4);
			overlayView.Frame = new CGRect (this.ContentView.Bounds.X, this.ContentView.Bounds.Height - overlayViewHeight, this.ContentView.Bounds.Width, overlayViewHeight);
			overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
			overlayView.Tag = 200;
			this.ContentView.AddSubview (overlayView);

			float margin = 5f;

			// Text label
			UILabel lblText = new UILabel ();
			lblText.Frame = new CGRect(overlayView.Frame.X + margin, overlayView.Frame.Y, overlayView.Frame.Width - margin - this.addCouponToQRCodeButtonWidth, overlayView.Frame.Height);
			lblText.TextColor = UIColor.White;
			lblText.Font = UIFont.FromName ("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.Tag = 300;
			lblText.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview (lblText);

			// Add to basket button
			// Let's use a view with a gesture recognizer, instead of a button, so we can increase the touch surface while keeping the icon small
			UIView addRemoveView = new UIView();
			addRemoveView.Frame = new CGRect (overlayView.Frame.Right - this.addCouponToQRCodeButtonWidth, overlayView.Frame.Y, this.addCouponToQRCodeButtonWidth, overlayView.Frame.Height);
			addRemoveView.BackgroundColor = UIColor.Clear;
			addRemoveView.AddGestureRecognizer(new UITapGestureRecognizer(() => 
				{ 
					this.onAddRemoveCouponQRCodePressed((this.objectOnDisplay));
					GetAddRemoveIcon(this.objectOnDisplay);
				}
			));
			UIImageView addRemoveQRCodeIcon = new UIImageView();
			addRemoveQRCodeIcon.Frame = new CGRect(addRemoveView.Bounds.Right - 30, 0, 20, addRemoveView.Bounds.Height);
			addRemoveQRCodeIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			addRemoveQRCodeIcon.BackgroundColor = UIColor.Clear;
			addRemoveQRCodeIcon.Tag = 600;
			addRemoveView.AddSubview(addRemoveQRCodeIcon);
			addRemoveView.Tag = 500;

			this.ContentView.AddSubview(addRemoveView);
		}

		private void SetLayoutImageAndTextContainer()
		{
			// Image view
			UIImageView imageView = new UIImageView ();
			imageView.Frame = new CGRect(0, 0, this.ContentView.Frame.Width / 4, this.ContentView.Frame.Height);
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;
			this.ContentView.AddSubview (imageView);

			// Text container view
			UIView textContainerView = new UIView ();
			textContainerView.Frame = new CGRect (imageView.Frame.Right, 0, this.ContentView.Bounds.Width - imageView.Frame.Right, this.ContentView.Frame.Height);
			textContainerView.BackgroundColor = UIColor.White;
			textContainerView.Tag = 200;
			this.ContentView.AddSubview (textContainerView);

			float margin = 10f;

			// Text label
			UILabel lblText = new UILabel ();
			lblText.Frame = new CGRect(textContainerView.Frame.X + margin, textContainerView.Frame.Y, textContainerView.Frame.Width - margin - this.addCouponToQRCodeButtonWidth, textContainerView.Frame.Height);
			lblText.TextColor = Utils.AppColors.TextColor;
			lblText.Font = UIFont.FromName ("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.BackgroundColor = UIColor.Clear;
			lblText.Tag = 300;
			this.ContentView.AddSubview (lblText);

			// Add to basket button
			// Let's use a view with a gesture recognizer, instead of a button, so we can increase the touch surface while keeping the icon small
			UIView addRemoveQRCodeView = new UIView();
			addRemoveQRCodeView.Frame = new CGRect (textContainerView.Frame.Right - this.addCouponToQRCodeButtonWidth, textContainerView.Frame.Y, this.addCouponToQRCodeButtonWidth, textContainerView.Frame.Height);
			addRemoveQRCodeView.BackgroundColor = UIColor.Clear;
			addRemoveQRCodeView.AddGestureRecognizer(new UITapGestureRecognizer(() => 
				{ 
					this.onAddRemoveCouponQRCodePressed((this.objectOnDisplay as PublishedOffer));
					GetAddRemoveIcon(this.objectOnDisplay);
				}
			));
			UIImageView addRemoveQRCodeIcon = new UIImageView();
			addRemoveQRCodeIcon.Frame = new CGRect(addRemoveQRCodeView.Bounds.Right - 30, 0, 20, addRemoveQRCodeView.Bounds.Height);
			addRemoveQRCodeIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			addRemoveQRCodeIcon.BackgroundColor = UIColor.Clear;
			addRemoveQRCodeIcon.Tag = 600;
			addRemoveQRCodeView.AddSubview(addRemoveQRCodeIcon);
			addRemoveQRCodeView.Tag = 500;
			this.ContentView.AddSubview(addRemoveQRCodeView);
		}

		private void HideAddRemoveIcon()
		{
			UIView containerView = (UIView)this.ContentView.ViewWithTag (200);	// Either overlayview or textcontainerview ...

			UILabel lblText = (UILabel)this.ContentView.ViewWithTag (300);

			float smallMargin = 5f;
			float largeMargin = 10f;

			if (this.layout == CellLayouts.ImageWithOverlay)
				lblText.Frame = new CGRect(containerView.Frame.X + smallMargin, containerView.Frame.Y, containerView.Frame.Width - 2 * smallMargin, containerView.Frame.Height);
			else if (this.layout == CellLayouts.ImageAndTextContainer)
				lblText.Frame = new CGRect(containerView.Frame.X + largeMargin, containerView.Frame.Y, containerView.Frame.Width - 2 * largeMargin, containerView.Frame.Height);
		
			UIView addRemoveView = this.ContentView.ViewWithTag(500);
			addRemoveView.Hidden = true;
		}

		private void ShowAddRemoveIcon()
		{
			UIView containerView = (UIView)this.ContentView.ViewWithTag (200);

			UILabel lblText = (UILabel)this.ContentView.ViewWithTag (300);

			float smallMargin = 5f;
			float largeMargin = 10f;

			if (this.layout == CellLayouts.ImageWithOverlay)
				lblText.Frame = new CGRect(containerView.Frame.X + smallMargin, containerView.Frame.Y, containerView.Frame.Width - smallMargin - this.addCouponToQRCodeButtonWidth, containerView.Frame.Height);
			else if (this.layout == CellLayouts.ImageAndTextContainer)
				lblText.Frame = new CGRect(containerView.Frame.X + largeMargin, containerView.Frame.Y, containerView.Frame.Width - largeMargin - this.addCouponToQRCodeButtonWidth, containerView.Frame.Height);

			UIView addRemoveView = this.ContentView.ViewWithTag(500);
			addRemoveView.Hidden = false;
		}

		private void GetAddRemoveIcon(object objectToDisplay)
		{
			UIView addRemoveView = this.ContentView.ViewWithTag(500);

			UIImageView addRemoveIconView = (UIImageView)addRemoveView.ViewWithTag(600);

			if(objectToDisplay is PublishedOffer)
			{
				PublishedOffer publishedOffer = objectToDisplay as PublishedOffer;

				if(publishedOffer.Selected)
				{
					if (this.layout == CellLayouts.ImageAndTextContainer)
						addRemoveIconView.Image = ImageUtilities.GetColoredImage(ImageUtilities.FromFile ("MinusIcon.png"), UIColor.DarkGray);
					else
						addRemoveIconView.Image = ImageUtilities.GetColoredImage(ImageUtilities.FromFile ("MinusIcon.png"), UIColor.White);
				}
				else
				{
					if (this.layout == CellLayouts.ImageAndTextContainer)
						addRemoveIconView.Image = ImageUtilities.GetColoredImage(ImageUtilities.FromFile ("PlusIcon.png"), UIColor.DarkGray);
					else
						addRemoveIconView.Image = ImageUtilities.GetColoredImage(ImageUtilities.FromFile ("PlusIcon.png"), UIColor.White);
				}
			}
		}

		private CellLayouts MapCellSizeToLayout(CellSizes cellSize)
		{
			switch (cellSize)
			{
			case CellSizes.ShortNarrow:
				return CellLayouts.ImageAndTextContainer;
			case CellSizes.ShortWide:
				return CellLayouts.ImageAndTextContainer;
			case CellSizes.TallNarrow:
				return CellLayouts.ImageWithOverlay;
			case CellSizes.TallWide:
				return CellLayouts.ImageWithOverlay;
			default:
				return CellLayouts.ImageWithOverlay;
			}
		}

		public enum CellLayouts
		{
			ImageWithOverlay,
			ImageAndTextContainer
		}

		#endregion

		public void SetValues (int id, object objectToDisplay, Action<object> onSelected, CellSizes size, string text, string imageColorHex, string imageId, bool localImage, Action<object> onAddToBasketButtonPressed)
		{
			this.Id = id;
			this.objectOnDisplay = objectToDisplay;
			this.onSelected = onSelected;

			// TODO Set layout explicitly. Now we determine what layout to use depending on the cell size. 
			if (this.layout != MapCellSizeToLayout(size))
			{
				this.layout = MapCellSizeToLayout(size);
				SetLayout();
			}

			this.size = size;

			this.onAddRemoveCouponQRCodePressed = onAddToBasketButtonPressed;

			UIImageView imageView = (UIImageView)this.ContentView.ViewWithTag (100);
			imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString (imageColorHex);
			imageView.Layer.RemoveAllAnimations();
			imageView.Image = null;

			UILabel lblText = (UILabel)this.ContentView.ViewWithTag (300);
			lblText.Text = text;

			GetAddRemoveIcon(objectToDisplay);

			if(objectToDisplay is PublishedOffer)
			{
				PublishedOffer publishedOffer = objectToDisplay as PublishedOffer;

				if(publishedOffer.Type != OfferType.PointOffer)
				{
					HideAddRemoveIcon();
				}
				else
				{
					ShowAddRemoveIcon();
				}
			}
			else
			{
				ShowAddRemoveIcon();
			}

			LoadImageToImageView(imageId, localImage, imageView);
		}
	}
}



