using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using UIKit;

namespace Presentation.Screens
{
	public class ErrorGettingDataView: UIView
	{
		UIButton btnRetry;

		public ErrorGettingDataView (CGRect frame)
		{
			this.Frame = frame;
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			UILabel couldNotGetDataText = new UILabel();
			couldNotGetDataText.Frame = new CGRect(0, this.Center.Y - 60f, this.Frame.Width, 50f);
			couldNotGetDataText.Text = LocalizationUtilities.LocalizedString("General_GetDataErrorMessage", "Something went wrong...\r\nhit refresh to try and fix it");
			couldNotGetDataText.Lines = 2;
			couldNotGetDataText.TextColor = Utils.AppColors.PrimaryColor;
			couldNotGetDataText.TextAlignment = UITextAlignment.Center;
			couldNotGetDataText.Font = UIFont.SystemFontOfSize(14);
			this.AddSubview(couldNotGetDataText);

			btnRetry = new UIButton();
			btnRetry.Frame = new CGRect(this.Center.X - 100/2, this.Center.Y + 10, 100, 40);
			btnRetry.Layer.CornerRadius = 2;
			btnRetry.SetTitle(LocalizationUtilities.LocalizedString("General_Refresh", "Refresh"), UIControlState.Normal);
			btnRetry.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnRetry.SetTitleColor(UIColor.White, UIControlState.Normal);
			btnRetry.TouchUpInside += btnPressed;

			this.AddSubview(btnRetry);
		}

		private void btnPressed(object sender, EventArgs e)
		{
			if (Retry != null)
			{
				Retry(sender, e);
			}
		}

		public EventHandler Retry { get; set; }
		protected override void Dispose(bool disposing)
		{
			btnRetry.TouchUpInside -= btnPressed;

		}
	}
}

