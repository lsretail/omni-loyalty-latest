using System;
using CoreGraphics;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Tables
{
    public class BasketCell : UITableViewCell
    {
        public static string KEY = "BASKETCELL";
        protected int id;

        public const float INTER_CELL_SPACING = 10f;

        private UIView customContentView;
        private UIImageView imageView;
        private UILabel lblTitle;
        private UILabel lblExtraInfo;
        private UILabel lblPrice;
        private UIButton btnRemove;

        private const float titleLabelHeight = 20f;
        private const float priceLabelHeight = 20f;
        private const float margin = 5f;
        private const float buttonDimensions = 40f;

        public delegate void RemoveItemFromBasketDelegate(int cellId);
        public RemoveItemFromBasketDelegate RemoveItemFromBasket;

        public BasketCell() : base(UITableViewCellStyle.Default, KEY)
        {
            this.BackgroundColor = UIColor.Clear;
            this.SelectionStyle = UITableViewCellSelectionStyle.None;

            this.customContentView = new UIView();
            this.customContentView.BackgroundColor = UIColor.White;
            this.ContentView.AddSubview(customContentView);

            this.imageView = new UIImageView();
            imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            imageView.ClipsToBounds = true;
            customContentView.AddSubview(imageView);

            this.lblTitle = new UILabel();
            lblTitle.BackgroundColor = UIColor.Clear;
            //lblTitle.BackgroundColor = UIColor.Red;
            lblTitle.TextColor = Utils.AppColors.TextColor;
            customContentView.AddSubview(lblTitle);

            this.lblExtraInfo = new UILabel();
            lblExtraInfo.BackgroundColor = UIColor.Clear;
            //lblExtraInfo.BackgroundColor = UIColor.Blue;
            lblExtraInfo.TextColor = UIColor.Gray;
            lblExtraInfo.Font = UIFont.SystemFontOfSize(12f);
            customContentView.AddSubview(lblExtraInfo);

            this.lblPrice = new UILabel();
            lblPrice.BackgroundColor = UIColor.Clear;
            //lblPrice.BackgroundColor = UIColor.Brown;
            lblPrice.TextColor = Utils.AppColors.PrimaryColor;
            customContentView.AddSubview(lblPrice);

            this.btnRemove = new UIButton();
            btnRemove.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.Red), UIControlState.Normal);
            btnRemove.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnRemove.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            btnRemove.BackgroundColor = UIColor.Clear;
            btnRemove.TouchUpInside += (object sender, EventArgs e) =>
            {

                if (this.RemoveItemFromBasket != null)
                    this.RemoveItemFromBasket(this.id);

            };
            customContentView.AddSubview(btnRemove);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.customContentView.Frame = new CGRect(
                0f,
                INTER_CELL_SPACING,
                this.ContentView.Frame.Width,
                this.ContentView.Frame.Height - INTER_CELL_SPACING
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
                80f                         //arbitrary height
            );
            this.lblExtraInfo.SizeToFit();

            this.lblPrice.Frame = new CGRect(
                this.lblTitle.Frame.Left,
                this.lblExtraInfo.Frame.Bottom + 2 * margin,
                this.lblTitle.Frame.Width - buttonDimensions - margin,
                priceLabelHeight
            );

            this.btnRemove.Frame = new CGRect(
                this.lblPrice.Frame.Right + margin,
                this.lblPrice.Frame.Top - (buttonDimensions - priceLabelHeight) / 2,
                buttonDimensions,
                buttonDimensions
            );
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
            this.lblExtraInfo.LineBreakMode = UILineBreakMode.WordWrap;
            this.lblExtraInfo.Text = extraInfo;

            this.lblPrice.Text = formattedPrice;

            if (String.IsNullOrEmpty(imageAvgColorHex))
                imageAvgColorHex = "E0E0E0"; // Default to light gray
            this.imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageAvgColorHex);

            Utils.UI.LoadImageToImageView(imageId, false, imageView, new ImageSize(100, 100), this.id.ToString());

            this.SetNeedsLayout();
        }

        public static nfloat GetCellHeight(string extraInfoString)
        {
            nfloat minHeight = INTER_CELL_SPACING + 2 * margin + titleLabelHeight + 2 * margin + Math.Max(priceLabelHeight, buttonDimensions) + margin;
            return minHeight + Utils.UI.GetLabelHeight(extraInfoString, UIFont.SystemFontOfSize(12f));
        }
    }
}

