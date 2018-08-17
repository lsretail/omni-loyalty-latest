using System;
using UIKit;
using Presentation.Utils;
using Foundation;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
	public class ConfirmOrderItemOverviewCell : UITableViewCell
	{
		public static string Key = "ConfirmOrderItemOverviewCell";

		public enum CellType
		{
			Item,
			Coupon,
			Offer
		}

		protected int id;

		private UIView customContentView;
		private UIImageView imageView;
		private UILabel lblTitle;
		private UILabel lblExtraInfo;
		private UILabel lblDiscount;
		private UILabel lblPrice;

		private const float titleLabelHeight = 20f;
		private const float priceLabelHeight = 20f;
		private const float margin = 5f;
		private const float buttonDimensions = 40f;
		private const float interCellSpacing = 10f;
		private const float lblPriceWidth = 110f;

		public ConfirmOrderItemOverviewCell() : base(UITableViewCellStyle.Default, Key)
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
			lblExtraInfo.TextColor = Utils.AppColors.MediumGray;
			lblExtraInfo.Font = UIFont.SystemFontOfSize(14f);
			customContentView.AddSubview(lblExtraInfo);

			this.lblDiscount = new UILabel();
			this.lblDiscount.Font = UIFont.SystemFontOfSize(14f);
			this.lblDiscount.TextColor = UIColor.Red;
			this.lblDiscount.TextAlignment = UITextAlignment.Left;
			this.lblDiscount.BackgroundColor = UIColor.Clear;
			customContentView.AddSubview (lblDiscount);

			this.lblPrice = new UILabel();
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.TextColor = Utils.AppColors.MediumGray;
			lblPrice.Font = UIFont.SystemFontOfSize (16f);
			lblPrice.TextAlignment = UITextAlignment.Right;
			customContentView.AddSubview(lblPrice);
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
				60f,
				60f
			);

			this.lblTitle.Frame = new CGRect(
				this.imageView.Frame.Right + 2 * margin,
				2 * margin, 
				this.customContentView.Frame.Width - this.imageView.Frame.Right - 4 * margin,
				titleLabelHeight
			);

			this.lblExtraInfo.Frame = new CGRect(
				this.lblTitle.Frame.Left,
				this.lblTitle.Frame.Bottom + margin, 
				this.lblTitle.Frame.Width,
				80f							//arbitrary height
			);
			this.lblExtraInfo.SizeToFit();

			this.lblDiscount.Frame = new CGRect(
				this.lblTitle.Frame.Left,
				this.lblExtraInfo.Frame.Bottom + 2 * margin,
				this.lblTitle.Frame.Width / 2,
				priceLabelHeight
			);

			this.lblPrice.Frame = new CGRect(
				this.lblDiscount.Frame.Right,
				this.lblExtraInfo.Frame.Bottom + 2 * margin, 
				this.lblTitle.Frame.Width / 2,
				priceLabelHeight
			);
		}

		public void SetValues(int id, string title, string extraInfo, string quantity, string discount, string formattedPrice, string imageAvgColorHex, string imageId)
		{
			this.id = id;

			decimal qty = Convert.ToDecimal(quantity);
            if (qty > 1)
            {
                title = qty.ToString() + LocalizationUtilities.LocalizedString("TransactionDetails_Multiplier", "x") + " " + title;
            }
			this.lblTitle.Text = title;

			this.lblExtraInfo.Lines = Utils.Util.GetStringLineCount(extraInfo);
			this.lblExtraInfo.Text = extraInfo;
			this.lblExtraInfo.SizeToFit();

			this.lblDiscount.Text = discount;

			this.lblPrice.Text = formattedPrice;

			if (String.IsNullOrEmpty(imageAvgColorHex))
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			this.imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageAvgColorHex);

			Utils.UI.LoadImageToImageView(imageId, false, imageView, new ImageSize(100, 100), this.id.ToString());
		}

		private static nfloat GetExtraInfoLabelHeight(string extraInfoString)
		{
			// Let's get the height of the extra info label by creating a templabel that is exactly like the one used in the
			// actual cell instance and then apply SizeToFit().

			UILabel tempLabel = new UILabel();
			tempLabel.Text = extraInfoString;
			tempLabel.Font = UIFont.SystemFontOfSize(12f);
			tempLabel.Lines = Utils.Util.GetStringLineCount(extraInfoString);
			tempLabel.SizeToFit();
			return tempLabel.Frame.Height;
		}

		public static nfloat GetCellHeight(string extraInfoString)
		{
			nfloat minHeight = interCellSpacing + 2 * margin + titleLabelHeight + 2 * margin + Math.Max(priceLabelHeight, buttonDimensions) + margin;
			return minHeight + GetExtraInfoLabelHeight(extraInfoString);
		}
	}
}

