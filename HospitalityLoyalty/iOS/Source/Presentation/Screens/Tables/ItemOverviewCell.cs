using System;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using System.Collections.Generic;
using Presentation.Utils;
using Domain.Transactions;
using Presentation.Models;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
	public class ItemOverviewCell : UITableViewCell
	{
		public static string Key = "ItemOverviewTableViewCell";

		protected int id;

		protected UIView customContentView;
		protected UIImageView imageView;
		protected UILabel lblTitle;
		protected UILabel lblExtraInfo;
		protected UILabel lblPrice;
		protected UIButton btnFavorite;
		protected UIButton btnReorder;

		private const float titleLabelHeight = 20f;
		private const float priceLabelHeight = 20f;
		private const float margin = 5f;
		private const float buttonDimensions = 40f;
		private const float interCellSpacing = 10f;

		public ItemOverviewCell() : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			SetLayout();
		}

		public virtual void SetLayout()
		{
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

			this.lblPrice = new UILabel();
			this.lblPrice.BackgroundColor = UIColor.Clear;
			this.lblPrice.TextColor = Utils.AppColors.PrimaryColor;
			this.customContentView.AddSubview(lblPrice);

			this.btnFavorite = new UIButton();
			this.btnFavorite.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), UIColor.Red), UIControlState.Normal);
			this.btnFavorite.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.btnFavorite.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			this.btnFavorite.BackgroundColor = UIColor.Clear;
			this.btnFavorite.Tag = 300;
			this.customContentView.AddSubview(btnFavorite);

			this.btnReorder = new UIButton();
			this.btnReorder.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.Gray), UIControlState.Normal);
			this.btnReorder.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.btnReorder.ImageEdgeInsets = new UIEdgeInsets(9, 9, 9, 9);
			this.btnReorder.BackgroundColor = UIColor.Clear;

			if (!Util.AppDelegate.BasketEnabled)
				btnReorder.Hidden = true;
			customContentView.AddSubview(btnReorder);

			this.ContentView.ConstrainLayout(() =>

				this.customContentView.Frame.Top == this.ContentView.Bounds.Top + interCellSpacing &&
				this.customContentView.Frame.Left == this.ContentView.Bounds.Left &&
				this.customContentView.Frame.Right == this.ContentView.Bounds.Right &&
				this.customContentView.Frame.Bottom == this.ContentView.Bounds.Bottom
			);

			this.customContentView.ConstrainLayout(() =>

				this.imageView.Frame.Top == this.customContentView.Frame.Top + 2 * margin &&
				this.imageView.Frame.Left == this.customContentView.Bounds.Left + margin &&
				this.imageView.Frame.Height == 60f &&
				this.imageView.Frame.Width == 60f &&

				this.lblTitle.Frame.Top == this.imageView.Frame.Top &&
				this.lblTitle.Frame.Left == this.imageView.Frame.Right + 2 * margin &&
				this.lblTitle.Frame.Right == this.customContentView.Frame.Right - 2 * margin &&
				this.lblTitle.Frame.Height == titleLabelHeight &&

				this.lblExtraInfo.Frame.Top == this.lblTitle.Frame.Bottom + margin &&
				this.lblExtraInfo.Frame.Left == this.lblTitle.Frame.Left &&
				this.lblExtraInfo.Frame.Right == this.lblTitle.Frame.Right &&

				this.lblPrice.Frame.GetCenterY() == this.btnFavorite.Frame.GetCenterY() &&
				this.lblPrice.Frame.Left == this.lblExtraInfo.Frame.Left &&
				this.lblPrice.Frame.Right == this.btnFavorite.Frame.Left - 5f &&
				this.lblPrice.Frame.Height == priceLabelHeight &&

				this.btnFavorite.Frame.Top == this.lblExtraInfo.Frame.Bottom + margin &&
				this.btnFavorite.Frame.Right == this.btnReorder.Frame.Left &&
				this.btnFavorite.Frame.Width == buttonDimensions &&
				this.btnFavorite.Frame.Height == buttonDimensions &&

				this.btnReorder.Frame.Top == this.btnFavorite.Frame.Top &&
				this.btnReorder.Frame.Right == this.lblExtraInfo.Frame.Right &&
				this.btnReorder.Frame.Width == buttonDimensions &&
				this.btnReorder.Frame.Height == buttonDimensions

			);
		}

		public void SetValues(int id, string title, string extraInfo, string quantity, string formattedPrice, string imageAvgColorHex, string imageId, bool isFavorite)
		{
			this.id = id;

			float qty = float.Parse(quantity);//Convert.ToInt32(quantity);
			if (qty > 1)
			{
				title = qty.ToString() + LocalizationUtilities.LocalizedString("TransactionDetails_Multiplier", "x") + " " + title;
			}
			lblTitle.Text = title;

			lblExtraInfo.Lines = Util.GetStringLineCount(extraInfo);
			lblExtraInfo.Text = extraInfo;
			lblExtraInfo.SizeToFit();

			lblPrice.Text = formattedPrice;

			btnFavorite.SetImage(GetFavoriteButtonIcon(isFavorite), UIControlState.Normal);

			if (String.IsNullOrEmpty(imageAvgColorHex))
			{
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			}
			this.imageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(imageAvgColorHex);
			Utils.UI.LoadImageToImageView(imageId, false, this.imageView, new ImageSize(100, 100), this.id.ToString());
		}

		protected UIImage GetFavoriteButtonIcon(bool isFavorite)
		{
			if (isFavorite)
				return Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOnIcon"), UIColor.Red);
			else
				return Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), UIColor.Red);
		}


		protected static nfloat GetExtraInfoLabelHeight(string extraInfoString)
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
			nfloat minHeight = interCellSpacing + 2 * margin + titleLabelHeight + 4 * margin + Math.Max(priceLabelHeight, buttonDimensions) + margin;
			return minHeight + GetExtraInfoLabelHeight(extraInfoString);
		}
	}
}

