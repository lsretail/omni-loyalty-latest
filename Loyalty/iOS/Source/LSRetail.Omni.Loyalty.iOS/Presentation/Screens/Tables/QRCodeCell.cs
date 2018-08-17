using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
	public class QRCodeCell : UITableViewCell
	{
		public static string Key = "QRCodeCell";
		protected int id;

		private UIView customContentView;
		private UIImageView imageView;
		private UILabel lblTitle;
		private UILabel lblExtraInfo;

		private const float titleLabelHeight = 20f;
		private const float margin = 5f;
		private const float interCellSpacing = 10f;
		private const float imageDimension = 60f;

		public QRCodeCell() : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			this.customContentView = new UIView();
			customContentView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(customContentView);

			this.imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.Clear;
			customContentView.AddSubview(imageView);

			this.lblTitle = new UILabel();
			lblTitle.BackgroundColor = UIColor.Clear;
			lblTitle.TextColor = Utils.AppColors.PrimaryColor;
			customContentView.AddSubview(lblTitle);

			this.lblExtraInfo = new UILabel();
			lblExtraInfo.BackgroundColor = UIColor.Clear;
			lblExtraInfo.TextColor = Utils.AppColors.MediumGray;
			lblExtraInfo.Font = UIFont.SystemFontOfSize(14f);
			customContentView.AddSubview(lblExtraInfo);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.customContentView.Frame = new CGRect(
				0f, 
				interCellSpacing, 
				this.ContentView.Frame.Width, 
				this.ContentView.Frame.Height - interCellSpacing
			);

			this.imageView.Frame = new CGRect(
				2 * margin,
				2 * margin, 
				imageDimension,
				imageDimension
			);

			this.lblTitle.Frame = new CGRect(
				this.imageView.Frame.Right + 2 * margin,
				2 * margin, 
				this.customContentView.Frame.Width - this.imageView.Frame.Right - 4 * margin,
				this.customContentView.Frame.Height / 2 - 2 * margin
			);

			this.lblExtraInfo.Frame = new CGRect(
				this.lblTitle.Frame.Left,
				this.lblTitle.Frame.Bottom, 
				this.lblTitle.Frame.Width,
				this.lblTitle.Frame.Height
			);
		}

		public void SetValues(int id, string title, string extraInfo, string imageAvgColorHex, string imageId)
		{
			this.id = id;

			this.lblTitle.Text = title;

			this.lblExtraInfo.Text = extraInfo;

			if (String.IsNullOrEmpty(imageAvgColorHex))
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			this.imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageAvgColorHex);
			Utils.UI.LoadImageToImageView(imageId, false, imageView, new ImageSize(100, 100), this.id.ToString());

			this.SetNeedsLayout();
		}

		public static nfloat GetCellHeight(string extraInfoString)
		{
			return interCellSpacing + 8 * margin + 2 * titleLabelHeight;
		}
	}
}

