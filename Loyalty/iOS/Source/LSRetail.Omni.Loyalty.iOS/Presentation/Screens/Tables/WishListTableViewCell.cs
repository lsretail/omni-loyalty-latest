using System;
using UIKit;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class WishListTableViewCell : UITableViewCell
	{
		public static string Key = "WishListTableViewCell";

		protected int id;

		private UIView customContentView;
		private UIImageView imageView;
		private UILabel lblTitle;
		private UILabel lblExtraInfo;
		private UILabel lblPrice;
		private UIButton btnRemove;
		private UIButton btnAddToBasket;

		private const float titleLabelHeight = 20f;
		private const float priceLabelHeight = 20f;
		private const float margin = 5f;
		private const float buttonDimensions = 40f;
		private const float interCellSpacing = 10f;
		private const float imageDimension = 60f;

		public delegate void AddToBasketDelegate(int cellId);
		public AddToBasketDelegate AddToBasket;

		public delegate void RemoveItemFromWishListDelegate(int cellId);
		public RemoveItemFromWishListDelegate RemoveItemFromWishList;

		public WishListTableViewCell() : base(UITableViewCellStyle.Default, Key)
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
			lblTitle.TextColor = Utils.AppColors.TextColor;
			customContentView.AddSubview(lblTitle);

			this.lblExtraInfo = new UILabel();
			lblExtraInfo.BackgroundColor = UIColor.Clear;
			lblExtraInfo.TextColor = UIColor.Gray;
			lblExtraInfo.Font = UIFont.SystemFontOfSize(12f);
			customContentView.AddSubview(lblExtraInfo);

			this.lblPrice = new UILabel();
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.TextColor = Utils.AppColors.PrimaryColor;
			customContentView.AddSubview(lblPrice);

			this.btnRemove = new UIButton();
            btnRemove.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.Red), UIControlState.Normal);
			btnRemove.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnRemove.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			btnRemove.BackgroundColor = UIColor.Clear;
			btnRemove.TouchUpInside += (object sender, EventArgs e) => { 

				if (this.RemoveItemFromWishList != null)
					this.RemoveItemFromWishList(this.id);				

			};
			btnRemove.BackgroundColor = UIColor.Clear;
			customContentView.AddSubview(btnRemove);

			this.btnAddToBasket = new UIButton();
			btnAddToBasket.SetImage(ImageUtilities.GetColoredImage ( ImageUtilities.FromFile ("IconsForTabBar/Fullsize/ShoppingBasketAdd.png"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
			btnAddToBasket.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnAddToBasket.ImageEdgeInsets = new UIEdgeInsets(8, 8, 8, 8);
			btnAddToBasket.BackgroundColor = UIColor.Clear;
			btnAddToBasket.TouchUpInside += (object sender, EventArgs e) => { 

				if (this.AddToBasket != null)
					this.AddToBasket(this.id);				

			};
			btnAddToBasket.BackgroundColor = UIColor.Clear;

			if (!EnabledItems.HasBasket)
				btnAddToBasket.Hidden = true;

			customContentView.AddSubview(btnAddToBasket);
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
				titleLabelHeight
			);

			this.lblExtraInfo.Frame = new CGRect(
				this.lblTitle.Frame.Left,
				this.lblTitle.Frame.Bottom + margin, 
				this.lblTitle.Frame.Width,
				titleLabelHeight
			);
			this.lblExtraInfo.SizeToFit();

			this.lblPrice.Frame = new CGRect(
				this.lblExtraInfo.Frame.Left,
				this.lblExtraInfo.Frame.Bottom + margin, 
				this.lblTitle.Frame.Width - 2 * buttonDimensions,
				titleLabelHeight
			);

			if(EnabledItems.HasBasket)
			{
				this.btnRemove.Frame = new CGRect(
					this.lblPrice.Frame.Right,
					this.lblPrice.Frame.Top - (buttonDimensions - priceLabelHeight)/2, 
					buttonDimensions,
					buttonDimensions
				);

				this.btnAddToBasket.Frame = new CGRect(
					this.btnRemove.Frame.Right,
					this.lblPrice.Frame.Top - (buttonDimensions - priceLabelHeight)/2, 
					buttonDimensions,
					buttonDimensions
				);
			}
			else
			{
				this.btnAddToBasket.Frame = new CGRect(
					this.lblPrice.Frame.Right,
					this.lblPrice.Frame.Top - (buttonDimensions - priceLabelHeight)/2, 
					buttonDimensions,
					buttonDimensions
				);

				this.btnRemove.Frame = new CGRect(
					this.btnAddToBasket.Frame.Right,
					this.lblPrice.Frame.Top - (buttonDimensions - priceLabelHeight)/2, 
					buttonDimensions,
					buttonDimensions
				);
			}
		}

		public void SetValues(int id, string title, string extraInfo, string quantity, string formattedPrice, string imageAvgColorHex, string imageId)
		{
			this.id = id;

			int qty = Convert.ToInt32(Convert.ToDecimal(quantity));
            if (qty > 1)
            {
                title = qty.ToString() + LocalizationUtilities.LocalizedString("TransactionDetails_Multiplier", "x") + " " + title;
            }
			this.lblTitle.Text = title;

			this.lblExtraInfo.Lines = Utils.Util.GetStringLineCount(extraInfo);
			this.lblExtraInfo.Text = extraInfo;

			this.lblPrice.Text = formattedPrice;

			if (String.IsNullOrEmpty(imageAvgColorHex))
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			this.imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageAvgColorHex);
			Utils.UI.LoadImageToImageView(imageId, false, this.imageView, new ImageSize(100, 100), this.id.ToString());

			this.SetNeedsLayout();
		}

		public static nfloat GetCellHeight(string extraInfoString)
		{			
			nfloat minHeight = interCellSpacing + 2 * margin + titleLabelHeight + 2 * margin + Math.Max(priceLabelHeight, buttonDimensions) + margin;
			return minHeight + Utils.UI.GetLabelHeight(extraInfoString, UIFont.SystemFontOfSize(12f));
		}
	}
}

