using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class QRCodeView : UIView
	{
		private UIImageView qrCodeImageView;
		private UITextView qrCodeInstructions;

		public QRCodeView()
		{
			this.BackgroundColor = UIColor.White;

			this.qrCodeImageView = new UIImageView();
			// NOTE: The QR code image has its own margins ("quiet zones") that are necessary for scanners when decoding the code
			this.qrCodeImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.AddSubview(qrCodeImageView);

			this.qrCodeInstructions = new UITextView();
			this.qrCodeInstructions.UserInteractionEnabled = true;
			this.qrCodeInstructions.Editable = false;
			this.qrCodeInstructions.Text = LocalizationUtilities.LocalizedString("Coupon_Details_PleaseScan", "Please scan QR code");
			this.qrCodeInstructions.TextColor = AppColors.PrimaryColor;
			this.qrCodeInstructions.Font = UIFont.SystemFontOfSize(16);
			this.qrCodeInstructions.TextAlignment = UITextAlignment.Center;
			this.AddSubview(qrCodeInstructions);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.qrCodeImageView.Frame = new CGRect(
				0, 
				0, 
				this.Bounds.Width, 
				260f
			);

			this.qrCodeInstructions.Frame = new CGRect(
				20f, 
				qrCodeImageView.Frame.Bottom, 
				this.Frame.Width - 2 * 20f, 
				this.Frame.Height - qrCodeImageView.Frame.Bottom - 20f
			);
		}

		public void UpdateData(string qrCodeXML)
		{
			this.qrCodeImageView.Image = Presentation.Utils.QRCode.QRCode.GenerateQRCode(qrCodeXML);
		}
	}
}

