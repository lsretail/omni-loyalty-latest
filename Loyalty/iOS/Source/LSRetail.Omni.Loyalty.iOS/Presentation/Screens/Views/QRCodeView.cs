using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class QRCodeView : BaseView
	{
		private UITableView QRCodeTableView;
		private QRCodeViewType type;

		private UIImageView qrCodeImageView;
		private UILabel qrCodeInstructions;

		public QRCodeView (QRCodeViewType type)
		{
			this.type = type;

			switch (this.type)
			{
			case QRCodeViewType.Account:
				this.BackgroundColor = UIColor.White;
				break;
			case QRCodeViewType.PublishedOffers:
				this.BackgroundColor = Utils.AppColors.BackgroundGray;
				break; 
			default:
				break;
			}
				
			//Settign up Account QRcode View
			this.qrCodeImageView = new UIImageView();
			this.qrCodeImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			this.qrCodeInstructions = new UILabel();
			this.qrCodeInstructions.UserInteractionEnabled = false;
			this.qrCodeInstructions.Text = LocalizationUtilities.LocalizedString("QRCode_Identify", "Show this QR code to identify yourself");
			this.qrCodeInstructions.TextColor = Utils.AppColors.PrimaryColor;
			this.qrCodeInstructions.Font = UIFont.SystemFontOfSize (16);
			this.qrCodeInstructions.TextAlignment = UITextAlignment.Center;

			this.AddSubview(this.qrCodeImageView);
			this.AddSubview(this.qrCodeInstructions);

			// Setting up general QRcode view
			this.QRCodeTableView = new UITableView();
			this.QRCodeTableView.BackgroundColor = UIColor.Clear;
			this.QRCodeTableView.AlwaysBounceVertical = true;
			this.QRCodeTableView.ShowsVerticalScrollIndicator = false;
			this.QRCodeTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.QRCodeTableView.TranslatesAutoresizingMaskIntoConstraints = false;

			this.AddSubview(this.QRCodeTableView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			switch (this.type)
			{
			case QRCodeViewType.Account:
				this.qrCodeImageView.Frame = new CGRect (0f, this.TopLayoutGuideLength, this.Bounds.Width, 260f);
				this.qrCodeInstructions.Frame = new CGRect (20f, qrCodeImageView.Frame.Bottom, this.Frame.Width - 2*20f, 20f);
				break;
			case QRCodeViewType.PublishedOffers:
				this.QRCodeTableView.Frame = new CGRect (0,this.TopLayoutGuideLength,this.Frame.Width, this.Frame.Height);
				break; 
			default:
				break;
			}
		}

		public void UpdateDataAccount (string qrCodeXML)
		{
			this.qrCodeImageView.Image = Utils.QRCode.QRCode.GenerateQRCode(qrCodeXML);	
		}

		public void UpdateDataPublishedOffers (string qrCodeXML, List<PublishedOffer> publishedOffers)
		{
			this.QRCodeTableView.Source = new QRCodeTableSource (qrCodeXML, publishedOffers);
		}
	}
}

