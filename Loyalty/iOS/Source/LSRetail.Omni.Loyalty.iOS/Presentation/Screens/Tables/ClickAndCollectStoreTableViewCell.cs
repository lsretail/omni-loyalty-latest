using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
	public class ClickAndCollectStoreTableViewCell : UITableViewCell
	{
		public static string Key = "ClickAndCollectStoreScreenTableViewCell";

		private UIView customContentView;
		private UIImageView imageView;
		private UILabel lblTitle;
		private UILabel lblExtraInfo;
		private UIButton btnStoreInfo;

		protected int id;
		private Action<int> onInfoButtonPressed;

		private const float titleLabelHeight = 20f;
		private const float priceLabelHeight = 20f;
		private const float margin = 5f;
		private const float buttonDimensions = 40f;
		private const float interCellSpacing = 10f;
		private const float imageDimensions = 95f;

		public ClickAndCollectStoreTableViewCell() : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			this.customContentView = new UIView();
			this.customContentView.BackgroundColor = UIColor.White;
			this.customContentView.Tag = 1;
			this.ContentView.AddSubview(customContentView);

			this.imageView = new UIImageView();
			this.imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.imageView.ClipsToBounds = true;
			this.imageView.BackgroundColor = UIColor.Purple;
			this.customContentView.AddSubview(imageView);

			this.lblTitle = new UILabel();
			this.lblTitle.BackgroundColor = UIColor.Clear;
			this.lblTitle.TextColor = Utils.AppColors.PrimaryColor;
			this.customContentView.AddSubview(lblTitle);

			this.lblExtraInfo = new UILabel();
			this.lblExtraInfo.BackgroundColor = UIColor.Clear;
			this.lblExtraInfo.TextColor = UIColor.Gray;
			this.lblExtraInfo.Font = UIFont.SystemFontOfSize(12f);
			this.customContentView.AddSubview(lblExtraInfo);

			this.btnStoreInfo = new UIButton();
			this.btnStoreInfo.SetImage(ImageUtilities.GetColoredImage(ImageUtilities.FromFile ("InfoIcon.png"), UIColor.Red), UIControlState.Normal);
			this.btnStoreInfo.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.btnStoreInfo.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			this.btnStoreInfo.BackgroundColor = UIColor.Clear;
			this.btnStoreInfo.TouchUpInside += (object sender, EventArgs e) => { 
				this.onInfoButtonPressed(this.id);
				this.btnStoreInfo.SetImage(GetInfoButtonIcon(), UIControlState.Normal);
			};
			customContentView.AddSubview(btnStoreInfo);
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
				imageDimensions,
				imageDimensions
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

			this.btnStoreInfo.Frame = new CGRect(
				this.lblTitle.Frame.Right - buttonDimensions,
				this.customContentView.Frame.Bottom - 4 * margin - buttonDimensions, 
				buttonDimensions,
				buttonDimensions
			);
		}

		private UIImage GetInfoButtonIcon()
		{
			return ImageUtilities.GetColoredImage (ImageUtilities.FromFile ("InfoIcon.png"), UIColor.Gray);
		}

		public void SetValues(int id, Action<int> onInfoButtonPressed, string title, string extraInfo, string imageAvgColorHex, string imageId)
		{
			this.id = id;
			this.onInfoButtonPressed = onInfoButtonPressed;

			this.lblTitle.Text = title;

			this.lblExtraInfo.Lines = Utils.Util.GetStringLineCount(extraInfo);
			this.lblExtraInfo.Text = extraInfo;
			this.btnStoreInfo.SetImage(GetInfoButtonIcon(), UIControlState.Normal);

			if (String.IsNullOrEmpty(imageAvgColorHex))
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			this.imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageAvgColorHex);

			Utils.UI.LoadImageToImageView(imageId, false, this.imageView, new ImageSize(200, 200), this.id.ToString());
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
			nfloat minHeight = interCellSpacing + 2 * margin + titleLabelHeight + 2 * margin + Math.Max (priceLabelHeight, buttonDimensions) + margin;
			return minHeight + GetExtraInfoLabelHeight(extraInfoString);
		}
	}
}

