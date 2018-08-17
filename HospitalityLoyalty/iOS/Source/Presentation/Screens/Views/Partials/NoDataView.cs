using System;
using CoreGraphics;
using Presentation;
using UIKit;

namespace Presentation.Screens
{
	public class NoDataView : BaseView
	{
		private UILabel lblMessage;

		private string textToDisplay;

		public string TextToDisplay
		{
			get
			{
				return this.textToDisplay;
			}
			set
			{
				textToDisplay = value;
				lblMessage.Text = textToDisplay;
				lblMessage.Lines = Presentation.Utils.Util.GetStringLineCount(lblMessage.Text);
			}
		}

		public NoDataView()
		{
			BackgroundColor = Presentation.Utils.AppColors.BackgroundGray;

			lblMessage = new UILabel();
			lblMessage.BackgroundColor = UIColor.Clear;
			lblMessage.TextAlignment = UITextAlignment.Center;
			lblMessage.TextColor = UIColor.Gray;
			lblMessage.Font = UIFont.SystemFontOfSize(14f);
			AddSubview(lblMessage);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.lblMessage.Frame = new CGRect(
				0,
				TopLayoutGuideLength,
				Frame.Width,
				Frame.Height - TopLayoutGuideLength - BottomLayoutGuideLength
			);
		}
	}
}

