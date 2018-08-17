using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
	public class RelatedCell : UITableViewCell
	{
		public static string KEY = "OFFERSANDCOUPONSRELATEDITEMSCELL";

		public const float INTER_CELL_SPACING = 0f;

		private const float TITLE_LABEL_HEIGHT = 20f;
		private const float MARGIN = 5f;
		private const float IMAGE_DIMENSIONS = 60f;

		private UIView customContentView;
		private UIImageView imageView;
		private UILabel lblTitle;

		public RelatedCell () : base(UITableViewCellStyle.Default, KEY)
		{
			this.BackgroundColor = AppColors.BackgroundGray;
			this.SelectionStyle = UITableViewCellSelectionStyle.Default;

			this.customContentView = new UIView();
			this.customContentView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(this.customContentView);

			this.imageView = new UIImageView();
			this.imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.imageView.ClipsToBounds = true;
			this.imageView.BackgroundColor = UIColor.Purple;
			this.customContentView.AddSubview(this.imageView);

			this.lblTitle = new UILabel();
			this.lblTitle.BackgroundColor = UIColor.Clear;
			this.lblTitle.TextColor = Utils.AppColors.TextColor;
			this.lblTitle.Font = UIFont.SystemFontOfSize(14);
			this.customContentView.AddSubview(this.lblTitle);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.customContentView.Frame = new CGRect(
				0,
				INTER_CELL_SPACING,
				this.customContentView.Superview.Frame.Width,
				this.customContentView.Superview.Frame.Height - INTER_CELL_SPACING
			);

			float imageWidth = 60f;
			this.imageView.Frame = new CGRect(
				2 * MARGIN,
				2 * MARGIN,
				imageWidth,
				imageWidth
			);

			this.lblTitle.Frame = new CGRect(
				this.imageView.Frame.Right + 4 * MARGIN,
				this.lblTitle.Superview.Frame.Height/2 - TITLE_LABEL_HEIGHT/2,
				this.lblTitle.Superview.Frame.Width - (this.imageView.Frame.Right + 4 * MARGIN) - 4 * MARGIN,
				TITLE_LABEL_HEIGHT
			);
		}

		public void SetValues(string title, string imageAvgColorHex, string imageId)
		{
			this.lblTitle.Text = title;

			if (string.IsNullOrEmpty(imageAvgColorHex))
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			this.imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageAvgColorHex);
			Utils.UI.LoadImageToImageView(imageId, false, this.imageView, new ImageSize(100, 100), imageId);
		}

		public static float CellHeight
		{
			get 
			{
				return INTER_CELL_SPACING + 2 * MARGIN + IMAGE_DIMENSIONS + 2 * MARGIN;
			}
		}
	}
}

