﻿using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.UI
{
	public class MenuItemCollectionViewRowCell : MenuItemBaseCollectionCell
	{
		private float addToBasketButtonWidth = 40f;
		internal static string CellIdentifier = "MenuItemCollectionViewRowCell";
		private UIImageView imageView;
		private UILabel lblText;
		private UILabel lblPrice;
		private UIImageView addToBasketIcon;
		private UIView textContainerView;

		[Export("initWithFrame:")]
		public MenuItemCollectionViewRowCell(CGRect frame) : base(frame)
		{
			// Image view
			imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(imageView);

			// Text container view
			textContainerView = new UIView();
			textContainerView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(textContainerView);


			// Text label
			lblText = new UILabel();
			lblText.TextColor = Utils.AppColors.PrimaryColor;
			lblText.Font = UIFont.FromName("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview(lblText);

			// Price label
			lblPrice = new UILabel();
			lblPrice.TextColor = UIColor.Gray;
			lblPrice.Font = UIFont.FromName("Helvetica", 12);
			lblPrice.TextAlignment = UITextAlignment.Left;
			lblPrice.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview(lblPrice);

			// Add to basket button
			// Let's use a view with a gesture recognizer, instead of a button, so we can increase the touch surface while keeping the icon smalladdToBasketView = new UIView();
			addToBasketView.BackgroundColor = UIColor.Clear;
			//addToBasketView.AddGestureRecognizer(new UITapGestureRecognizer(() => { this.onAddToBasketButtonPressed((this.objectOnDisplay as MenuNode)); }));
			addToBasketIcon = new UIImageView();
			addToBasketIcon.Image = Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.LightGray);
			addToBasketIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			addToBasketIcon.BackgroundColor = UIColor.Clear;
			addToBasketView.AddSubview(addToBasketIcon);
			addToBasketView.Tag = 500;
			this.ContentView.AddSubview(addToBasketView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			float margin = 10f;

			imageView.Frame = new CGRect(0, 0, this.ContentView.Frame.Width / 4, this.ContentView.Frame.Height);
			textContainerView.Frame = new CGRect(imageView.Frame.Right, 0, this.ContentView.Bounds.Width - imageView.Frame.Right, this.ContentView.Frame.Height);
			lblText.Frame = new CGRect(textContainerView.Frame.X + margin, textContainerView.Frame.Y + 2 * 5f, textContainerView.Frame.Width - margin - this.addToBasketButtonWidth, textContainerView.Frame.Height / 2 - 2 * 5f);
			lblPrice.Frame = new CGRect(textContainerView.Frame.X + margin, lblText.Frame.Bottom, textContainerView.Frame.Width - margin - this.addToBasketButtonWidth, textContainerView.Frame.Height / 2 - 2 * 5f);
			addToBasketView.Frame = new CGRect(textContainerView.Frame.Right - this.addToBasketButtonWidth, textContainerView.Frame.Y, this.addToBasketButtonWidth, textContainerView.Frame.Height);
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
			ShowPriceLabelAndBasketButton();
		}

		private void HidePriceLabelAndBasketButton()
		{


			float smallMargin = 5f;

			lblText.Frame = new CGRect(textContainerView.Frame.X + smallMargin, textContainerView.Frame.Y, textContainerView.Frame.Width - 2 * smallMargin, textContainerView.Frame.Height);

			lblPrice.Hidden = true;

			addToBasketView.Hidden = true;
		}

		private void ShowPriceLabelAndBasketButton()
		{
			float smallMargin = 5f;

			lblText.Frame = new CGRect(textContainerView.Frame.X + smallMargin, textContainerView.Frame.Y + smallMargin, textContainerView.Frame.Width - smallMargin - this.addToBasketButtonWidth, textContainerView.Frame.Height / 2 - smallMargin);
			lblPrice.Hidden = false;
			if (Utils.Util.AppDelegate.BasketEnabled)
				addToBasketView.Hidden = false;
		}
	}
}

