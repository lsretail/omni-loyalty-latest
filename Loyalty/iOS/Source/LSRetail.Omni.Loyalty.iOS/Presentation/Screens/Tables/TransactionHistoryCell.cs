using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;

namespace Presentation
{
	public class TransactionHistoryCell : UITableViewCell
	{
		public static string Key = "HistoryScreenCell";

		private UIView customContentView;
		private UILabel lblDate;
		private UILabel lblLocation;
		private UILabel lblPrice;

		public TransactionHistoryCell () : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			this.customContentView = new UIView();
			customContentView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(customContentView);

			this.lblDate = new UILabel();
			lblDate.TextColor = Utils.AppColors.PrimaryColor; 
			lblDate.Font = UIFont.FromName ("Helvetica", 14);
			lblDate.TextAlignment = UITextAlignment.Left;
			lblDate.BackgroundColor = UIColor.Clear;
			customContentView.AddSubview (lblDate);

			this.lblLocation = new UILabel ();
			lblLocation.TextColor = UIColor.Gray;
			lblLocation.Font = UIFont.FromName ("Helvetica", 12);
			lblLocation.TextAlignment = UITextAlignment.Left;
			lblLocation.BackgroundColor = UIColor.Clear;
			customContentView.AddSubview (lblLocation);

			this.lblPrice = new UILabel();
			lblPrice.TextColor = Utils.AppColors.PrimaryColor; 
			lblPrice.Font = UIFont.FromName ("Helvetica", 14);
			lblPrice.TextAlignment = UITextAlignment.Right;
			lblPrice.BackgroundColor = UIColor.Clear;
			customContentView.AddSubview (lblPrice);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			const float margin = 10f;
			const float priceLabelWidth = 80f;
			const float interCellSpacing = 10f;

			this.customContentView.Frame = new CGRect(
				0f, 
				interCellSpacing, 
				this.ContentView.Frame.Width, 
				this.ContentView.Frame.Height - interCellSpacing
			);

			this.lblPrice.Frame = new CGRect(
				this.customContentView.Frame.Right - 2 * margin - priceLabelWidth, 
				margin, 
				priceLabelWidth,
				this.customContentView.Frame.Height - 2 * margin
			);

			this.lblDate.Frame = new CGRect(
				2 * margin, 
				margin, 
				this.customContentView.Frame.Width - 2 * margin - priceLabelWidth - margin, 
				this.customContentView.Frame.Height / 2 - margin
			);

			this.lblLocation.Frame = new CGRect(
				2 * margin,
				lblDate.Frame.Bottom,
				this.customContentView.Frame.Width - 2 * margin - priceLabelWidth - margin,
				this.customContentView.Frame.Height / 2 - margin
			);
		}

		public void SetValues(int id, string formattedDate, string location, string formattedPrice)
		{
			

			lblDate.Text = formattedDate;
			lblLocation.Text = location;
			lblPrice.Text = formattedPrice;
		}
	}
}

