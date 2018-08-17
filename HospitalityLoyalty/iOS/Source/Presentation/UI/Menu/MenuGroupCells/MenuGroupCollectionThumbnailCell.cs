﻿using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.UI
{
	public class MenuGroupCollectionThumbnailCell : MenuBaseCollectionCell
	{
		internal static string CellIdentifier = "MenuGroupCollectionThumbnailCell";

		UIImageView imageView;
		UILabel lblText;
		UIView overlayView;

		[Export("initWithFrame:")]
		public MenuGroupCollectionThumbnailCell(CGRect frame) : base(frame)
		{
			// Image view
			imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;

			// Overlay view
			overlayView = new UIView();
			overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
			overlayView.Tag = 200;

			// Text label
			lblText = new UILabel();
			lblText.TextColor = UIColor.White;
			lblText.Font = UIFont.FromName("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.Tag = 300;
			lblText.BackgroundColor = UIColor.Clear;
			ContentView.AddSubviews(imageView, overlayView, lblText);
			//overlayView.AddSubview(lblText);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			SetLayoutImageWithOverlay();
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

		private void SetLayoutImageWithOverlay()
		{

			imageView.Frame = this.ContentView.Frame;

			int overlayViewHeight = (int)Math.Floor(this.ContentView.Bounds.Height / 4);
			overlayView.Frame = new CGRect(this.ContentView.Bounds.X, this.ContentView.Bounds.Height - overlayViewHeight, this.ContentView.Bounds.Width, overlayViewHeight);


			float margin = 5f;
			lblText.Frame = new CGRect(overlayView.Frame.X + margin, overlayView.Frame.Y, overlayView.Frame.Width - margin, overlayView.Frame.Height);
		}
	}
}