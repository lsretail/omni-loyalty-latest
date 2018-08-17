using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
	public class ShippingMethodTableViewCell : UITableViewCell
	{
		public static string Key = "ShippingMethodScreenTableViewCell";
		protected int id;

		private UIView customContentView;
		private UIImageView imageView;
		private UILabel lblTitle;
		private UILabel lblExtraInfo;

		public ShippingMethodTableViewCell() : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			this.customContentView = new UIView();
			customContentView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(customContentView);

			this.imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.Purple;
			customContentView.AddSubview(imageView);

			this.lblTitle = new UILabel();
			lblTitle.BackgroundColor = UIColor.Clear;
			lblTitle.TextColor = Utils.AppColors.PrimaryColor;
			customContentView.AddSubview(lblTitle);

			this.lblExtraInfo = new UILabel();
			lblExtraInfo.BackgroundColor = UIColor.Clear;
			lblExtraInfo.TextColor = UIColor.Gray;
			lblExtraInfo.Font = UIFont.SystemFontOfSize(12f);
			customContentView.AddSubview(lblExtraInfo);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			const float margin = 5f;
			const float interCellSpacing = 10f;
			const float imageDimension = 95f;

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
				4 * margin, 
				this.customContentView.Frame.Width - this.imageView.Frame.Right - 4 * margin,
				this.customContentView.Frame.Height / 2 - 4 * margin
			);

			this.lblExtraInfo.Frame = new CGRect(
				this.lblTitle.Frame.Left,
				this.lblTitle.Frame.Bottom, 
				this.lblTitle.Frame.Width,
				this.lblTitle.Frame.Height
			);
			lblExtraInfo.SizeToFit();
		}

		public void SetValues(int id, string title, string extraInfo, string imageId)
		{
			this.id = id;

			lblTitle.Text = title;

			lblExtraInfo.Lines = 0;
			lblExtraInfo.Text = extraInfo;
			lblExtraInfo.SizeToFit();

			imageView.BackgroundColor = UIColor.Clear;
			Utils.UI.LoadImageToImageView(imageId, true, imageView, new ImageSize(200, 200), this.id.ToString());
		}

		public static float GetCellHeight(string extraInfoString)
		{
			return 120f;
		}
	}
}

