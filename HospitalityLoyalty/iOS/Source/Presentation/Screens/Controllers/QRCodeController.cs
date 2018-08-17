using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class QRCodeController : UIViewController
	{
		private QRCodeView rootView;
		private string qrCodeXML;

		public QRCodeController(string xml)
		{
			this.Title = LocalizationUtilities.LocalizedString("QRCode_QRCode", "QR code");
			this.qrCodeXML = xml;
			this.rootView = new QRCodeView();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UIButton btnClose = new UIButton(UIButtonType.Custom);
			btnClose.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.White), UIControlState.Normal);
			btnClose.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			btnClose.Frame = new CGRect(0, 0, 30, 30);
			btnClose.TouchUpInside += (sender, e) =>
			{
				this.DismissViewController(true, null);
			};

			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(btnClose);

			this.rootView.UpdateData(this.qrCodeXML);
			this.View = this.rootView;
		}
	}
}

