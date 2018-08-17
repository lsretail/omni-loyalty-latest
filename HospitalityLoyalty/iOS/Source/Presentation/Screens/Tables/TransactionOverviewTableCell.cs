using System;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
public class TransactionOverviewTableCell : UITableViewCell
	{
		public static string Key = "TransactionOverviewTableViewCell";

		public TransactionOverviewTableCell() : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			SetLayout();
		}

		private void SetLayout()
		{
			UIView customContentView = new UIView();
			customContentView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(customContentView);

			// Date label
			UILabel lblDate = new UILabel();
			lblDate.TextColor = AppColors.PrimaryColor;
			lblDate.Font = UIFont.FromName("Helvetica", 14);
			lblDate.TextAlignment = UITextAlignment.Left;
			lblDate.BackgroundColor = UIColor.Clear;
			lblDate.Tag = 100;
			customContentView.AddSubview(lblDate);

			// ItemCount label
			UILabel lblItemCount = new UILabel();
			lblItemCount.TextColor = UIColor.Gray;
			lblItemCount.Font = UIFont.FromName("Helvetica", 12);
			lblItemCount.TextAlignment = UITextAlignment.Left;
			lblItemCount.BackgroundColor = UIColor.Clear;
			lblItemCount.Tag = 200;
			customContentView.AddSubview(lblItemCount);

			// Price label
			UILabel lblPrice = new UILabel();
			lblPrice.TextColor = AppColors.PrimaryColor;
			lblPrice.Font = UIFont.FromName("Helvetica", 14);
			lblPrice.TextAlignment = UITextAlignment.Right;
			lblPrice.BackgroundColor = UIColor.Clear;
			lblPrice.Tag = 300;
			customContentView.AddSubview(lblPrice);

			const float margin = 10f;
			const float priceLabelWidth = 80f;
			const float interCellSpacing = 10f;

			this.ContentView.ConstrainLayout(() =>

				customContentView.Frame.Top == this.ContentView.Bounds.Top + interCellSpacing &&
				customContentView.Frame.Left == this.ContentView.Bounds.Left &&
				customContentView.Frame.Right == this.ContentView.Bounds.Right &&
				customContentView.Frame.Bottom == this.ContentView.Bounds.Bottom
			);

			customContentView.ConstrainLayout(() =>

				lblDate.Frame.Top == customContentView.Frame.Top + margin &&
				lblDate.Frame.Left == customContentView.Frame.Left + 2 * margin &&
				lblDate.Frame.Right == lblPrice.Frame.Left - margin &&
				lblDate.Frame.Bottom == customContentView.Frame.GetCenterY() &&

				lblItemCount.Frame.Top == lblDate.Frame.Bottom &&
				lblItemCount.Frame.Left == lblDate.Frame.Left &&
				lblItemCount.Frame.Right == lblDate.Frame.Right &&
				lblItemCount.Frame.Bottom == customContentView.Frame.Bottom - margin &&

				lblPrice.Frame.GetCenterY() == customContentView.Frame.GetCenterY() &&
				lblPrice.Frame.Right == customContentView.Frame.Right - 2 * margin &&
				lblPrice.Frame.Height == customContentView.Frame.Height - 2 * margin &&
				lblPrice.Frame.Width == priceLabelWidth

			);
		}

		public void SetValues(int id, string formattedDate, string itemCount, string formattedPrice)
		{

			UILabel lblDate = this.ContentView.ViewWithTag(100) as UILabel;
			lblDate.Text = formattedDate;

			UILabel lblItemCount = this.ContentView.ViewWithTag(200) as UILabel;
			lblItemCount.Text = itemCount;

			UILabel lblPrice = this.ContentView.ViewWithTag(300) as UILabel;
			lblPrice.Text = formattedPrice;
		}
	}
}

