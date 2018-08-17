using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace Presentation
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
				this.textToDisplay = value;
				this.lblMessage.Text = this.textToDisplay;
				this.lblMessage.Lines = Utils.Util.GetStringLineCount(this.lblMessage.Text);
			}
		}

		public NoDataView ()
		{			
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			this.lblMessage = new UILabel();
			this.lblMessage.BackgroundColor = UIColor.Clear;
			this.lblMessage.TextAlignment = UITextAlignment.Center;
			this.lblMessage.TextColor = Utils.AppColors.MediumGray;
			this.lblMessage.Font = UIFont.SystemFontOfSize(14f);
			this.AddSubview(this.lblMessage);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.lblMessage.Frame = new CGRect(
				0, 
				this.TopLayoutGuideLength,
				this.Frame.Width,
				this.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength
			);
		}
	}
}

