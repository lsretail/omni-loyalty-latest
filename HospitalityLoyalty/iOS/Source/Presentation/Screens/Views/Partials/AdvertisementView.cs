using System;
using UIKit;
using CoreGraphics;

namespace Presentation.Screens
{
	public class AdvertisementView : BaseView
	{
		private UILabel lblText;
		private UIView overlayView;
		private UIImageView adImageView;

		public UIImageView AdImageView { get { return this.adImageView; } }

		private float pageIndicatorHeight;

		public AdvertisementView(CGRect frame, float pgIndicatorHeight)
		{
			Frame = frame;
			pageIndicatorHeight = pgIndicatorHeight;
			Layer.MasksToBounds = true;
			SetLayout();
		}

		private void SetLayout()
		{
			// Image view
			adImageView = new UIImageView();
			adImageView.Frame = this.Bounds;
			adImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			// Overlay view
			float overlayViewHeight = (float)Math.Floor(this.Bounds.Height / 8) + this.pageIndicatorHeight;
			overlayView = new UIView();
			overlayView.Frame = new CGRect(0, this.Bounds.Bottom - overlayViewHeight, this.Bounds.Width, overlayViewHeight);
			overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;

			// Text label
			lblText = new UILabel();
			lblText.Frame = new CGRect(5f, 0f, overlayView.Bounds.Width - 2 * 5f, overlayView.Bounds.Height - this.pageIndicatorHeight);
			lblText.TextColor = UIColor.White;
			lblText.Font = UIFont.FromName("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.BackgroundColor = UIColor.Clear;

			AddSubviews(adImageView, overlayView);
			overlayView.AddSubview(lblText);
		}

		public void SetDescriptionText(string text)
		{
			lblText.Text = text;
		}

		public void ShowDescriptionText()
		{
			overlayView.Hidden = false;
		}

		public void HideDescriptionText()
		{
			overlayView.Hidden = true;
		}
	}
}

