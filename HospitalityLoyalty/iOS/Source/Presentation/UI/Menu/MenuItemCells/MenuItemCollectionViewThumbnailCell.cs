using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.UI
{
	public class MenuItemCollectionViewThumbnailCell : MenuItemBaseCollectionCell
	{
		private float addToBasketButtonWidth = 40f;
		internal static string CellIdentifier = "MenuItemCollectionViewThumbnailCell";
		private UIImageView imageView;
		private UIView overlayView;
		private UILabel lblText;
		private UILabel lblPrice;
		private UIImageView addToBasketIcon;

		[Export("initWithFrame:")]
		public MenuItemCollectionViewThumbnailCell(CGRect frame) : base(frame)
		{
			// Image view
			imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(imageView);

			// Overlay view
			overlayView = new UIView();
			overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
			this.ContentView.AddSubview(overlayView);


			// Text label
			lblText = new UILabel();
			lblText.TextColor = UIColor.White;
			lblText.Font = UIFont.FromName("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview(lblText);

			// Price label
			lblPrice = new UILabel();
			lblPrice.TextColor = UIColor.White;
			lblPrice.Font = UIFont.FromName("Helvetica", 12);
			lblPrice.TextAlignment = UITextAlignment.Left;
			lblPrice.Tag = 400;
			lblPrice.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview(lblPrice);

			// Add to basket button
			// Let's use a view with a gesture recognizer, instead of a button, so we can increase the touch surface while keeping the icon small
			addToBasketView.BackgroundColor = UIColor.Clear;

			addToBasketIcon = new UIImageView();
			addToBasketIcon.Image = Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.White);
			addToBasketIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			addToBasketIcon.BackgroundColor = UIColor.Clear;
			addToBasketView.AddSubview(addToBasketIcon);
			addToBasketView.Tag = 500;
			if (!Utils.Util.AppDelegate.BasketEnabled)
				addToBasketView.Hidden = true;
			this.ContentView.AddSubview(addToBasketView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			// Image view
			imageView.Frame = this.ContentView.Frame;

			// Overlay view
			int overlayViewHeight = (int)Math.Floor(this.ContentView.Bounds.Height / 4);
			overlayView.Frame = new CGRect(this.ContentView.Bounds.X, this.ContentView.Bounds.Height - overlayViewHeight, this.ContentView.Bounds.Width, overlayViewHeight);

			float margin = 5f;

			// Text label
			lblText.Frame = new CGRect(overlayView.Frame.X + margin, overlayView.Frame.Y + margin, overlayView.Frame.Width - margin - this.addToBasketButtonWidth, overlayView.Frame.Height / 2 - margin);

			// Price label
			lblPrice.Frame = new CGRect(overlayView.Frame.X + margin, lblText.Frame.Bottom, overlayView.Frame.Width - margin - this.addToBasketButtonWidth, overlayView.Frame.Height / 2 - margin);

			// Add to basket button
			// Let's use a view with a gesture recognizer, instead of a button, so we can increase the touch surface while keeping the icon small
			addToBasketView.Frame = new CGRect(overlayView.Frame.Right - this.addToBasketButtonWidth, overlayView.Frame.Y, this.addToBasketButtonWidth, overlayView.Frame.Height);

			addToBasketIcon.Frame = new CGRect(addToBasketView.Bounds.Right - 34, 0, 24, addToBasketView.Bounds.Height);

		}

		public override void SetValue(MobileMenuNode menu)
		{
			base.SetValue(menu);

			imageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(menu.Image.AvgColor);
			imageView.Layer.RemoveAllAnimations();
			imageView.Image = null;

			lblText.Text = menu.Description;

			LoadImageToImageView(menu.Image.Id, false, imageView);

		}

		public override void SetPrice(string itemPrice)
		{
			lblPrice.Text = itemPrice;
		}

	}
}

