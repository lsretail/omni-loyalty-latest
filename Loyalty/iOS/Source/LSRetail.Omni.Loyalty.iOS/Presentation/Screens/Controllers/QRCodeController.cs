using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public enum QRCodeViewType
	{
		Account,
		PublishedOffers
	};

	public class QRCodeController : UIViewController
	{
		private QRCodeView rootView;
		private QRCodeViewType type;
		private string qrCodeXML;
		private List<PublishedOffer> selectedPublishedOffers;

		public QRCodeController (string xml, QRCodeViewType type)
		{
			this.qrCodeXML = xml;
			this.selectedPublishedOffers = AppData.SelectedPublishedOffers;
			this.type = type;
			this.Title = LocalizationUtilities.LocalizedString("QRCode_QRCode", "QR code");
			this.rootView = new QRCodeView (type);
		}

		public QRCodeController (string xml, QRCodeViewType type, PublishedOffer selectedPublishedOffer)
		{
			this.qrCodeXML = xml;
			this.selectedPublishedOffers = new List<PublishedOffer>(){selectedPublishedOffer};
			this.type = type;
			this.Title = LocalizationUtilities.LocalizedString("QRCode_QRCode", "QR code");
			this.rootView = new QRCodeView (type);
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.BottomLayoutGuideLength = BottomLayoutGuide.Length;
			this.rootView.TopLayoutGuideLength = TopLayoutGuide.Length;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			UIBarButtonItem doneButton = new UIBarButtonItem ();
			doneButton.Title = LocalizationUtilities.LocalizedString("General_Done", "Done");
			doneButton.Clicked += (object sender, EventArgs e) => {
				this.DismissViewController(true, null);
			};
			this.NavigationItem.RightBarButtonItem = doneButton;	

			switch (this.type)
			{
			case QRCodeViewType.Account:
				this.View.BackgroundColor = UIColor.White;
				this.rootView.UpdateDataAccount (this.qrCodeXML);
				break;
			case QRCodeViewType.PublishedOffers:
				this.View.BackgroundColor = Utils.AppColors.BackgroundGray;
				this.rootView.UpdateDataPublishedOffers (this.qrCodeXML, this.selectedPublishedOffers);
				break; 
			default:
				break;
			}
				
			this.View = this.rootView;
		}
	}
}

