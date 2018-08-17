using System;
using UIKit;
using Foundation;

namespace Presentation.Utils.Containers
{
	public class ContactUsTableCell : UITableViewCell
	{
		public ContactUsTableCell()
		{}

		public ContactUsTableCell(NSString cellId, UITableViewCellStyle cellStyle = UITableViewCellStyle.Subtitle) : base(cellStyle, cellId)
		{
			this.SelectionStyle = UITableViewCellSelectionStyle.Gray;

			this.TextLabel.TextAlignment = UITextAlignment.Left;
			this.TextLabel.LineBreakMode = UILineBreakMode.TailTruncation;

			if (this.DetailTextLabel != null)
			{
				this.DetailTextLabel.TextColor = UIColor.Blue;
			}

			BackgroundColor = Utils.AppColors.BackgroundGray;
		}

		public void SetValues(string key, string value)
		{
			this.TextLabel.Text = key;
			if (this.DetailTextLabel != null)
				this.DetailTextLabel.Text = value;
		}
	}
}

