using System;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class BasketCell : UITableViewCell
	{
		public static string KEY = "BASKETCELL";
		protected int id;
		protected BasketView.BasketType basketType;

		public const float MinCellHeight = 75f;
		public const float INTER_CELL_SPACING = 10f;
		private const float titleLabelHeight = 20f;
		private const float priceLabelHeight = 20f;
		private const float margin = 5f;
		private const float buttonDimensions = 40f;

		public UIView customContentView;
		public UIImageView imageView;
		public UILabel lblTitle;
		public UILabel lblExtraInfo;
		public UILabel lblPrice;
		public UIButton btnRemove;
		public UIButton btnFavorite;

		private readonly BasketView.IBasketListeners listener;

		public BasketCell(BasketView.BasketType cellType, BasketView.IBasketListeners listener)
		{
			this.listener = listener;

			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;
			this.basketType = cellType;

			this.customContentView = new UIView();
			this.customContentView.BackgroundColor = UIColor.White;
			this.AddSubview(customContentView);

			this.imageView = new UIImageView();
			this.imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.imageView.ClipsToBounds = true;
			this.imageView.BackgroundColor = UIColor.Purple;
			this.customContentView.AddSubview(imageView);

			this.lblTitle = new UILabel();
			this.lblTitle.BackgroundColor = UIColor.Clear;
			this.lblTitle.TextColor = UIColor.Black;
			this.customContentView.AddSubview(lblTitle);

			this.lblExtraInfo = new UILabel();
			this.lblExtraInfo.BackgroundColor = UIColor.Clear;
			this.lblExtraInfo.TextColor = UIColor.Gray;
			this.lblExtraInfo.Font = UIFont.SystemFontOfSize(12f);
			this.customContentView.AddSubview(lblExtraInfo);

			this.lblPrice = new UILabel();
			this.lblPrice.BackgroundColor = UIColor.Clear;
			this.lblPrice.TextColor = AppColors.PrimaryColor;
			this.customContentView.AddSubview(lblPrice);

			this.btnRemove = new UIButton();
			this.btnRemove.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.Red), UIControlState.Normal);
			this.btnRemove.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.btnRemove.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			this.btnRemove.BackgroundColor = UIColor.Clear;
			this.btnRemove.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.RemoveItemFromBasket(this.id, this.basketType);
			};
			this.customContentView.AddSubview(btnRemove);

			this.btnFavorite = new UIButton();
			if (this.basketType == BasketView.BasketType.Item)
				SetFavoriteButtonIcon(this.listener.IsFavorite(this.id));
			this.btnFavorite.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.btnFavorite.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			this.btnFavorite.BackgroundColor = UIColor.Clear;
			this.btnFavorite.TouchUpInside += (object sender, EventArgs e) =>
			{
				bool isFavorite = this.listener.ToggleFavorite (this.id, basketType);
				SetFavoriteButtonIcon(isFavorite);
			};
			this.customContentView.AddSubview(this.btnFavorite);

			this.btnFavorite.TranslatesAutoresizingMaskIntoConstraints = false;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.ConstrainLayout(() =>
				customContentView.Frame.Top == this.Bounds.Top + INTER_CELL_SPACING &&
				customContentView.Frame.Left == this.Bounds.Left &&
				customContentView.Frame.Right == this.Bounds.Right &&
				customContentView.Frame.Bottom == this.Bounds.Bottom
			);

			customContentView.ConstrainLayout(() =>
				imageView.Frame.Top == customContentView.Frame.Top + 2 * margin &&
				imageView.Frame.Left == customContentView.Bounds.Left + 2 * margin &&
				imageView.Frame.Height == 60f &&
				imageView.Frame.Width == 60f &&

				lblTitle.Frame.Top == imageView.Frame.Top &&
				lblTitle.Frame.Left == imageView.Frame.Right + 2 * margin &&
				lblTitle.Frame.Right == customContentView.Frame.Right - 2 * margin &&
				lblTitle.Frame.Height == titleLabelHeight &&

				lblExtraInfo.Frame.Top == lblTitle.Frame.Bottom + margin &&
				lblExtraInfo.Frame.Left == lblTitle.Frame.Left &&
				lblExtraInfo.Frame.Right == lblTitle.Frame.Right &&

				lblPrice.Frame.GetCenterY() == btnRemove.Frame.GetCenterY() &&
				lblPrice.Frame.Left == lblExtraInfo.Frame.Left &&
				lblPrice.Frame.Right == btnRemove.Frame.Left - 5f &&
				lblPrice.Frame.Height == priceLabelHeight &&

				btnRemove.Frame.Top == lblExtraInfo.Frame.Bottom + margin &&
		        btnRemove.Frame.Right == customContentView.Frame.Right - margin &&
				btnRemove.Frame.Width == buttonDimensions &&
				btnRemove.Frame.Height == buttonDimensions &&

				btnFavorite.Frame.Top == lblExtraInfo.Frame.Bottom + margin &&
			    btnFavorite.Frame.Right == btnRemove.Frame.Right - 8  * margin &&
                btnFavorite.Frame.Width == buttonDimensions &&
			    btnFavorite.Frame.Height == buttonDimensions 
			);
		}

		public void UpdateData (int id, string title, decimal quantity, string formattedPrice, string extraInfo, string imageAvgColorHex, string imageId, BasketView.BasketType basketType)
		{
			this.id = id;

			int qty = Convert.ToInt32(Convert.ToDecimal(quantity));
			if (qty > 1)
			{
				title = qty.ToString() + LocalizationUtilities.LocalizedString("Basket_Multiplier", "x") + " " + title;
			}

			this.lblTitle.Text = title;
			this.lblExtraInfo.Lines = Utils.UI.GetStringLineCount(extraInfo);
			this.lblExtraInfo.Text = extraInfo;
			this.lblExtraInfo.SizeToFit();
			this.lblPrice.Text = formattedPrice;

			if (String.IsNullOrEmpty(imageAvgColorHex))
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			imageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(imageAvgColorHex);

			Utils.UI.LoadImageToImageView(imageId, false, this.imageView, new ImageSize(100, 100), this.id.ToString());

			this.basketType = basketType;

			if (basketType == BasketView.BasketType.Item)
				SetFavoriteButtonIcon(this.listener.IsFavorite(this.id));
			else if (basketType == BasketView.BasketType.Offer)
			{
				btnFavorite.Hidden = true;
			}
		}

		public static nfloat GetCellHeight(string extraInfoString)
		{
			nfloat minHeight = INTER_CELL_SPACING + 2 * margin + titleLabelHeight + 2 * margin + Math.Max(priceLabelHeight, buttonDimensions) + margin;
			return minHeight + Utils.UI.GetLabelHeight(extraInfoString, UIFont.SystemFontOfSize(12f));
		}

		private void SetFavoriteButtonIcon(bool isFavorite)
		{
			if (isFavorite)
				this.btnFavorite.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOnIcon"), UIColor.Red), UIControlState.Normal);
			else
				this.btnFavorite.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), UIColor.Red), UIControlState.Normal);
		}
	}
}