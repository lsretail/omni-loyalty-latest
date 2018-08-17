using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.Screens
{
	public class MenuCollectionCell : CardCollectionCell
	{
		private CellLayouts layout;

		private Action<MenuNode> onAddToBasketButtonPressed;

		private float addToBasketButtonWidth = 40f;

		[Export("initWithFrame:")]
		public MenuCollectionCell(CGRect frame) : base(frame)
		{ }

		#region Layout

		protected override void SetLayout()
		{
			ClearSubviews();

			if (this.layout == CellLayouts.ImageWithOverlay)
				SetLayoutImageWithOverlay();
			else if (this.layout == CellLayouts.ImageAndTextContainer)
				SetLayoutImageAndTextContainer();
			else
				SetLayoutImageWithOverlay();    // Default to this
		}

		private void SetLayoutImageWithOverlay()
		{
			// Image view
			UIImageView imageView = new UIImageView();
			imageView.Frame = this.ContentView.Frame;
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;
			this.ContentView.AddSubview(imageView);

			// Overlay view
			UIView overlayView = new UIView();
			int overlayViewHeight = (int)Math.Floor(this.ContentView.Bounds.Height / 4);
			overlayView.Frame = new CGRect(this.ContentView.Bounds.X, this.ContentView.Bounds.Height - overlayViewHeight, this.ContentView.Bounds.Width, overlayViewHeight);
			overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
			overlayView.Tag = 200;
			this.ContentView.AddSubview(overlayView);

			float margin = 5f;

			// Text label
			UILabel lblText = new UILabel();
			lblText.Frame = new CGRect(overlayView.Frame.X + margin, overlayView.Frame.Y + margin, overlayView.Frame.Width - margin - this.addToBasketButtonWidth, overlayView.Frame.Height / 2 - margin);
			lblText.TextColor = UIColor.White;
			lblText.Font = UIFont.FromName("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.Tag = 300;
			lblText.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview(lblText);

			// Price label
			UILabel lblPrice = new UILabel();
			lblPrice.Frame = new CGRect(overlayView.Frame.X + margin, lblText.Frame.Bottom, overlayView.Frame.Width - margin - this.addToBasketButtonWidth, overlayView.Frame.Height / 2 - margin);
			lblPrice.TextColor = UIColor.White;
			lblPrice.Font = UIFont.FromName("Helvetica", 12);
			lblPrice.TextAlignment = UITextAlignment.Left;
			lblPrice.Tag = 400;
			lblPrice.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview(lblPrice);

			// Add to basket button
			// Let's use a view with a gesture recognizer, instead of a button, so we can increase the touch surface while keeping the icon small
			UIView addToBasketView = new UIView();
			addToBasketView.Frame = new CGRect(overlayView.Frame.Right - this.addToBasketButtonWidth, overlayView.Frame.Y, this.addToBasketButtonWidth, overlayView.Frame.Height);
			addToBasketView.BackgroundColor = UIColor.Clear;
			addToBasketView.AddGestureRecognizer(new UITapGestureRecognizer(() => { this.onAddToBasketButtonPressed((this.objectOnDisplay as MenuNode)); }));
			UIImageView addToBasketIcon = new UIImageView();
			addToBasketIcon.Frame = new CGRect(addToBasketView.Bounds.Right - 34, 0, 24, addToBasketView.Bounds.Height);
			addToBasketIcon.Image = Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.White);
			addToBasketIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			addToBasketIcon.BackgroundColor = UIColor.Clear;
			addToBasketView.AddSubview(addToBasketIcon);
			addToBasketView.Tag = 500;
			if (!Utils.Util.AppDelegate.BasketEnabled)
				addToBasketView.Hidden = true;
			this.ContentView.AddSubview(addToBasketView);
		}

		private void SetLayoutImageAndTextContainer()
		{
			// Image view
			UIImageView imageView = new UIImageView();
			imageView.Frame = new CGRect(0, 0, this.ContentView.Frame.Width / 4, this.ContentView.Frame.Height);
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;
			this.ContentView.AddSubview(imageView);

			// Text container view
			UIView textContainerView = new UIView();
			textContainerView.Frame = new CGRect(imageView.Frame.Right, 0, this.ContentView.Bounds.Width - imageView.Frame.Right, this.ContentView.Frame.Height);
			textContainerView.BackgroundColor = UIColor.White;
			textContainerView.Tag = 200;
			this.ContentView.AddSubview(textContainerView);

			float margin = 10f;

			// Text label
			UILabel lblText = new UILabel();
			lblText.Frame = new CGRect(textContainerView.Frame.X + margin, textContainerView.Frame.Y + 2 * 5f, textContainerView.Frame.Width - margin - this.addToBasketButtonWidth, textContainerView.Frame.Height / 2 - 2 * 5f);
			lblText.TextColor = Utils.AppColors.PrimaryColor;
			lblText.Font = UIFont.FromName("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.BackgroundColor = UIColor.Clear;
			lblText.Tag = 300;
			this.ContentView.AddSubview(lblText);

			// Price label
			UILabel lblPrice = new UILabel();
			lblPrice.Frame = new CGRect(textContainerView.Frame.X + margin, lblText.Frame.Bottom, textContainerView.Frame.Width - margin - this.addToBasketButtonWidth, textContainerView.Frame.Height / 2 - 2 * 5f);
			lblPrice.TextColor = UIColor.Gray;
			lblPrice.Font = UIFont.FromName("Helvetica", 12);
			lblPrice.TextAlignment = UITextAlignment.Left;
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.Tag = 400;
			this.ContentView.AddSubview(lblPrice);

			// Add to basket button
			// Let's use a view with a gesture recognizer, instead of a button, so we can increase the touch surface while keeping the icon small
			UIView addToBasketView = new UIView();
			addToBasketView.Frame = new CGRect(textContainerView.Frame.Right - this.addToBasketButtonWidth, textContainerView.Frame.Y, this.addToBasketButtonWidth, textContainerView.Frame.Height);
			addToBasketView.BackgroundColor = UIColor.Clear;
			addToBasketView.AddGestureRecognizer(new UITapGestureRecognizer(() => { this.onAddToBasketButtonPressed((this.objectOnDisplay as MenuNode)); }));
			UIImageView addToBasketIcon = new UIImageView();
			addToBasketIcon.Frame = new CGRect(addToBasketView.Bounds.Right - 34, 0, 24, addToBasketView.Bounds.Height);
			addToBasketIcon.Image = Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.LightGray);
			addToBasketIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			addToBasketIcon.BackgroundColor = UIColor.Clear;
			addToBasketView.AddSubview(addToBasketIcon);
			addToBasketView.Tag = 500;
			this.ContentView.AddSubview(addToBasketView);
		}

		private void HidePriceLabelAndBasketButton()
		{
			UIView containerView = (UIView)this.ContentView.ViewWithTag(200);   // Either overlayview or textcontainerview ...

			UILabel lblText = (UILabel)this.ContentView.ViewWithTag(300);

			float smallMargin = 5f;
			float largeMargin = 10f;

			if (this.layout == CellLayouts.ImageWithOverlay)
				lblText.Frame = new CGRect(containerView.Frame.X + smallMargin, containerView.Frame.Y, containerView.Frame.Width - 2 * smallMargin, containerView.Frame.Height);
			else if (this.layout == CellLayouts.ImageAndTextContainer)
				lblText.Frame = new CGRect(containerView.Frame.X + largeMargin, containerView.Frame.Y, containerView.Frame.Width - 2 * largeMargin, containerView.Frame.Height);

			UILabel lblPrice = (UILabel)this.ContentView.ViewWithTag(400);
			lblPrice.Hidden = true;

			UIView addToBasketView = this.ContentView.ViewWithTag(500);
			addToBasketView.Hidden = true;
		}

		private void ShowPriceLabelAndBasketButton()
		{
			UIView containerView = (UIView)this.ContentView.ViewWithTag(200);

			UILabel lblText = (UILabel)this.ContentView.ViewWithTag(300);

			float smallMargin = 5f;
			float largeMargin = 10f;

			if (this.layout == CellLayouts.ImageWithOverlay)
				lblText.Frame = new CGRect(containerView.Frame.X + smallMargin, containerView.Frame.Y + smallMargin, containerView.Frame.Width - smallMargin - this.addToBasketButtonWidth, containerView.Frame.Height / 2 - smallMargin);
			else if (this.layout == CellLayouts.ImageAndTextContainer)
				lblText.Frame = new CGRect(containerView.Frame.X + largeMargin, containerView.Frame.Y + 2 * 5f, containerView.Frame.Width - largeMargin - this.addToBasketButtonWidth, containerView.Frame.Height / 2 - 2 * 5f);

			UILabel lblPrice = (UILabel)this.ContentView.ViewWithTag(400);
			lblPrice.Hidden = false;

			UIView addToBasketView = this.ContentView.ViewWithTag(500);
			if (Utils.Util.AppDelegate.BasketEnabled)
				addToBasketView.Hidden = false;
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

		public void SetValues(int id, object objectToDisplay, Action<object> onSelected, CellSizes size, string text, string imageColorHex, string imageId, bool localImage, string formattedPriceString, Action<MenuNode> onAddToBasketButtonPressed)
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

			this.onAddToBasketButtonPressed = onAddToBasketButtonPressed;

			UIImageView imageView = (UIImageView)this.ContentView.ViewWithTag(100);
			imageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(imageColorHex);
			imageView.Layer.RemoveAllAnimations();
			imageView.Image = null;

			UILabel lblText = (UILabel)this.ContentView.ViewWithTag(300);
			lblText.Text = text;

			UILabel lblPrice = (UILabel)this.ContentView.ViewWithTag(400);
			lblPrice.Text = formattedPriceString;

			// Show price label and add-to-basket button if we have a price string
			// TODO Should be able to have either a price string or add-to-basket button, or both, or none
			if (!String.IsNullOrEmpty(lblPrice.Text))
				ShowPriceLabelAndBasketButton();
			else
				HidePriceLabelAndBasketButton();

			LoadImageToImageView(imageId, localImage, imageView);
		}
	}
}

