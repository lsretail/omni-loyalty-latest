using System;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class CheckoutOrderPlacedView : BaseView
	{
		private UIImageView qrCodeImageView;
		private UITextView qrCodeInstructions;
		private UITextView qrCodeInfo;
		private UIButton btnDone;

		private readonly ICheckoutOrderPlacedListener listener;

		public CheckoutOrderPlacedView(ICheckoutOrderPlacedListener listener)
		{
			this.listener = listener;

			this.BackgroundColor = UIColor.White;

			this.qrCodeImageView = new UIImageView();
			qrCodeImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			qrCodeImageView.Image = null;   // Initialize empty QR code, don't have an orderID to put in it yet
			qrCodeImageView.BackgroundColor = UIColor.Clear;
			this.AddSubview(qrCodeImageView);

			this.qrCodeInstructions = new UITextView();
			qrCodeInstructions.UserInteractionEnabled = true;
			qrCodeInstructions.Editable = false;
			qrCodeInstructions.Text = LocalizationUtilities.LocalizedString("Checkout_ScanQRCodeInstructions", "");
			qrCodeInstructions.TextColor = Utils.AppColors.PrimaryColor;
			qrCodeInstructions.Font = UIFont.SystemFontOfSize(16);
			qrCodeInstructions.TextAlignment = UITextAlignment.Center;
			this.AddSubview(qrCodeInstructions);

			this.qrCodeInfo = new UITextView();
			qrCodeInfo.UserInteractionEnabled = true;
			qrCodeInfo.Editable = false;
			qrCodeInfo.Text = LocalizationUtilities.LocalizedString("Checkout_QRCodeInfo", "");
			qrCodeInfo.TextColor = Utils.AppColors.PrimaryColor;
			qrCodeInfo.Font = UIFont.SystemFontOfSize(12);
			qrCodeInfo.TextAlignment = UITextAlignment.Center;
			this.AddSubview(qrCodeInfo);

			this.btnDone = new UIButton();
			btnDone.SetTitle(LocalizationUtilities.LocalizedString("General_Done", "Done"), UIControlState.Normal);
			btnDone.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnDone.Layer.CornerRadius = 2;
			btnDone.TouchUpInside += (object sender, EventArgs e) =>
			{

				this.listener.DonePressed();

			};
			this.AddSubview(btnDone);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.ConstrainLayout(() =>

				this.qrCodeImageView.Frame.Top == this.Bounds.Top &&
				this.qrCodeImageView.Frame.Left == this.Bounds.Left &&
				this.qrCodeImageView.Frame.Right == this.Bounds.Right &&
				this.qrCodeImageView.Frame.Height == 260f &&

				this.btnDone.Frame.Bottom == this.Bounds.Bottom - 20f &&
				this.btnDone.Frame.Left == this.Bounds.Left + 20f &&
				this.btnDone.Frame.Right == this.Bounds.Right - 20f &&
				this.btnDone.Frame.Height == 50f &&

				this.qrCodeInfo.Frame.Bottom == this.btnDone.Frame.Top - 20f &&
				this.qrCodeInfo.Frame.Left == this.Bounds.Left + 20f &&
				this.qrCodeInfo.Frame.Right == this.Bounds.Right - 20f &&
				this.qrCodeInfo.Frame.Height == 40f &&

				this.qrCodeInstructions.Frame.Top == this.qrCodeImageView.Frame.Bottom &&
				this.qrCodeInstructions.Frame.Left == this.Bounds.Left + 20f &&
				this.qrCodeInstructions.Frame.Right == this.Bounds.Right - 20f &&
				this.qrCodeInstructions.Frame.Bottom == this.qrCodeInfo.Frame.Top

		   );
		}

		public void SetQRCodeImage(UIImage QRCodeImage)
		{
			this.qrCodeImageView.Image = QRCodeImage;
		}

		public interface ICheckoutOrderPlacedListener
		{
			void DonePressed();
		}
	}
}

