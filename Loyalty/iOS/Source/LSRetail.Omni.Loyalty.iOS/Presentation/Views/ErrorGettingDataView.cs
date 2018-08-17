using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using UIKit;

namespace Presentation.Screens
{
	public class ErrorGettingDataView : UIView
	{
		public ErrorGettingDataView (CGRect frame, Action retryAction)
		{
			this.Frame = frame;
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			UILabel couldNotGetDataText = new UILabel();
			couldNotGetDataText.Frame = new CGRect(0, this.Frame.Height/2 - 60f, this.Frame.Width, 50f);
			couldNotGetDataText.Text = LocalizationUtilities.LocalizedString("General_GetDataErrorMessage", "Something went wrong...\r\nhit refresh to try and fix it");
			couldNotGetDataText.Lines = 2;
			couldNotGetDataText.TextColor = Utils.AppColors.PrimaryColor;
			couldNotGetDataText.TextAlignment = UITextAlignment.Center;
			couldNotGetDataText.Font = UIFont.SystemFontOfSize(14);
			this.AddSubview(couldNotGetDataText);

			UIButton btnRetry = new UIButton();
			btnRetry.Frame = new CGRect(this.Frame.Width/2 - 100/2, this.Frame.Height/2 + 10f, 100f, 40f);
			btnRetry.Layer.CornerRadius = 2;
			btnRetry.SetTitle(LocalizationUtilities.LocalizedString("General_Refresh", "Refresh"), UIControlState.Normal);
			btnRetry.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnRetry.SetTitleColor(UIColor.White, UIControlState.Normal);
			btnRetry.TouchUpInside += (object sender, EventArgs e) => {
				this.Hidden = true;
				retryAction();
			};
			this.AddSubview(btnRetry);
		}
	}
}

