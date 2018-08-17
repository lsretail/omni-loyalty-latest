using System;
using CoreGraphics;
using Foundation;
using UIKit;
using Presentation.Utils;
using Domain.Offers;
using System.Linq;
using System.Collections.Generic;

namespace Presentation.Screens
{
	/*
	/// <summary>
	/// Modal view controller
	/// </summary>
	public class QRCodeScreen : UIViewController
	{
		private string qrCodeXML;
		private UITableView QRCodeTableView;
		private bool accountScreenQRCode;

		// for account screen only
		private UIImageView qrCodeImageView;
		private UILabel qrCodeInstructions;

		// when coming from Offer / Coupon details screen - when only show that deal, else show selected offers and coupons
		private List<PublishedOffer> selectedPublishedOffers;

		public QRCodeScreen(string xml, bool accountScreenQRCode)
		{
			this.qrCodeXML = xml;
			this.accountScreenQRCode = accountScreenQRCode;

			this.selectedPublishedOffers = AppData.SelectedPublishedOffers;

			this.Title = NSBundle.MainBundle.LocalizedString("QRCode_QRCode", "QR code");
		}

		public QRCodeScreen(string xml, bool accountScreenQRCode, PublishedOffer selectedPublishedOffer)
		{
			this.qrCodeXML = xml;
			this.accountScreenQRCode = accountScreenQRCode;

			this.selectedPublishedOffers = new List<PublishedOffer>(){selectedPublishedOffer};

			this.Title = NSBundle.MainBundle.LocalizedString("QRCode_QRCode", "QR code");
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			if (this.accountScreenQRCode)
			{
				// display simple view with on QR Code
				this.View.BackgroundColor = UIColor.White;

				UIBarButtonItem doneButton = new UIBarButtonItem ();
				doneButton.Title = NSBundle.MainBundle.LocalizedString("General_Done", "Done");
				doneButton.Clicked += (object sender, EventArgs e) => {

					this.DismissViewController(true, null);

				};
				this.NavigationItem.RightBarButtonItem = doneButton;

				this.qrCodeImageView = new UIImageView();
				qrCodeImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				qrCodeImageView.Image = Utils.QRCode.QRCode.GenerateQRCode(this.qrCodeXML);
				this.View.AddSubview(qrCodeImageView);

				this.qrCodeInstructions = new UILabel();
				qrCodeInstructions.UserInteractionEnabled = false;
				qrCodeInstructions.Text = NSBundle.MainBundle.LocalizedString("QRCode_Identify", "Show this QR code to identify yourself");


				qrCodeInstructions.TextColor = Utils.AppColors.PrimaryColor;
				qrCodeInstructions.Font = UIFont.SystemFontOfSize (16);
				qrCodeInstructions.TextAlignment = UITextAlignment.Center;
				this.View.AddSubview(qrCodeInstructions);
			}
			else
			{
				this.View.BackgroundColor = Utils.AppColors.BackgroundGray;

				UIBarButtonItem doneButton = new UIBarButtonItem ();
				doneButton.Title = NSBundle.MainBundle.LocalizedString("General_Done", "Done");
				doneButton.Clicked += (object sender, EventArgs e) => {

					this.DismissViewController(true, null);

				};
				this.NavigationItem.RightBarButtonItem = doneButton;

				// the table view
				this.QRCodeTableView = new UITableView();
				this.QRCodeTableView.BackgroundColor = UIColor.Clear;
				this.QRCodeTableView.AlwaysBounceVertical = true;
				this.QRCodeTableView.ShowsVerticalScrollIndicator = false;
				this.QRCodeTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
				this.QRCodeTableView.Source = new QRCodeTableSource(this.qrCodeXML, this.selectedPublishedOffers);
				this.QRCodeTableView.TranslatesAutoresizingMaskIntoConstraints = false;
				this.View.AddSubview(this.QRCodeTableView);

				this.View.ConstrainLayout(() => 

					this.QRCodeTableView.Frame.Top == this.View.Bounds.Top &&
					this.QRCodeTableView.Frame.Right == this.View.Bounds.Right &&
					this.QRCodeTableView.Frame.Left == this.View.Bounds.Left &&
					this.QRCodeTableView.Frame.Bottom == this.View.Bounds.Bottom

				);
			}				
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			if(this.accountScreenQRCode)
			{
				//TODO : Refactor - change to PopUpView?
				// NOTE: The QR code image has its own margins ("quiet zones") that are necessary for scanners when decoding the code
				this.qrCodeImageView.Frame = new CGRect(0f, this.TopLayoutGuide.Length, this.View.Bounds.Width, 260f);
				this.qrCodeInstructions.Frame = new CGRect(20f, qrCodeImageView.Frame.Bottom, this.View.Frame.Width - 2*20f, 20f);
			}
		}

		private class QRCodeTableSource : UITableViewSource
		{
			private UIView headerView;
			private UIView offerHeaderView;
			private UIView couponHeaderView;
			private string qrCodeXML;

			private List<PublishedOffer> selectedPublishedOffers;

			public QRCodeTableSource (string qrCodeXML, List<PublishedOffer> selectedPublishedOffers)
			{
				this.qrCodeXML = qrCodeXML;
				this.selectedPublishedOffers = selectedPublishedOffers;

				BuildHeaderView();
				BuildOfferHeaderView();
				BuildCouponHeaderView();
			}

			private void BuildHeaderView()
			{
				headerView = new UIView();
				headerView.BackgroundColor = UIColor.White;

				UIImageView qrCodeImageView = new UIImageView();
				qrCodeImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				qrCodeImageView.Image = Utils.QRCode.QRCode.GenerateQRCode(this.qrCodeXML);
				headerView.AddSubview(qrCodeImageView);

				UILabel qrCodeInstructions = new UILabel();

				qrCodeInstructions.UserInteractionEnabled = false;
				qrCodeInstructions.Text = NSBundle.MainBundle.LocalizedString("Coupon_Details_PleaseScan", "Please scan QR code");

				qrCodeInstructions.TextColor = Utils.AppColors.PrimaryColor;
				qrCodeInstructions.Font = UIFont.BoldSystemFontOfSize (16);
				qrCodeInstructions.TextAlignment = UITextAlignment.Center;
				headerView.AddSubview(qrCodeInstructions);

				const float margin = 20f;
				const float qrCodeHeight = 220f;

				headerView.ConstrainLayout(() =>

					qrCodeImageView.Frame.Top == headerView.Bounds.Top &&
					qrCodeImageView.Frame.Left == headerView.Bounds.Left &&
					qrCodeImageView.Frame.Right == headerView.Bounds.Right &&
					qrCodeImageView.Frame.Height == qrCodeHeight &&

					qrCodeInstructions.Frame.Top == qrCodeImageView.Frame.Bottom &&
					qrCodeInstructions.Frame.Left == headerView.Bounds.Left + margin &&
					qrCodeInstructions.Frame.Right == headerView.Bounds.Right - margin &&
					qrCodeInstructions.Frame.Height == margin
				);
			}

			private void BuildOfferHeaderView()
			{
				offerHeaderView = new UIView();
				offerHeaderView.BackgroundColor = AppColors.BackgroundGray;

				UIView containerView = new UIView();
				containerView.BackgroundColor = UIColor.White;
				offerHeaderView.AddSubview(containerView);

				UILabel offerLabel = new UILabel();
				offerLabel.UserInteractionEnabled = false;
				offerLabel.Text = NSBundle.MainBundle.LocalizedString("OffersAndCoupons_Offers", "Offers");
				offerLabel.TextColor = Utils.AppColors.PrimaryColor;
				offerLabel.Font = UIFont.BoldSystemFontOfSize (16);
				offerLabel.TextAlignment = UITextAlignment.Left;
				containerView.AddSubview(offerLabel);

				const float margin = 5f;

				offerHeaderView.ConstrainLayout(() =>

					containerView.Frame.Top == offerHeaderView.Bounds.Top + 2 * margin &&
					containerView.Frame.Left == offerHeaderView.Bounds.Left &&
					containerView.Frame.Right == offerHeaderView.Bounds.Right &&
					containerView.Frame.Height == offerHeaderView.Bounds.Height - 2 * margin
				);

				containerView.ConstrainLayout(() =>

					offerLabel.Frame.Top == containerView.Bounds.Top &&
					offerLabel.Frame.Left == containerView.Bounds.Left + 4 * margin &&
					offerLabel.Frame.Right == containerView.Bounds.Right &&
					offerLabel.Frame.Height == containerView.Bounds.Height
				);
			}

			private void BuildCouponHeaderView()
			{
				couponHeaderView = new UIView();
				couponHeaderView.BackgroundColor = AppColors.BackgroundGray;

				UIView containerView = new UIView();
				containerView.BackgroundColor = UIColor.White;
				couponHeaderView.AddSubview(containerView);

				UILabel couponLabel = new UILabel();
				couponLabel.UserInteractionEnabled = false;
				couponLabel.Text = NSBundle.MainBundle.LocalizedString("OffersAndCoupons_Coupons", "Coupons");
				couponLabel.TextColor = Utils.AppColors.PrimaryColor;
				couponLabel.Font = UIFont.BoldSystemFontOfSize (16);
				couponLabel.TextAlignment = UITextAlignment.Left;
				containerView.AddSubview(couponLabel);

				const float margin = 5f;

				couponHeaderView.ConstrainLayout(() =>

					containerView.Frame.Top == couponHeaderView.Bounds.Top + 2 * margin &&
					containerView.Frame.Left == couponHeaderView.Bounds.Left &&
					containerView.Frame.Right == couponHeaderView.Bounds.Right &&
					containerView.Frame.Height == couponHeaderView.Bounds.Height -2 * margin
				);

				containerView.ConstrainLayout(() =>

					couponLabel.Frame.Top == containerView.Bounds.Top &&
					couponLabel.Frame.Left == containerView.Bounds.Left + 4 * margin &&
					couponLabel.Frame.Right == containerView.Bounds.Right &&
					couponLabel.Frame.Height == containerView.Bounds.Height
				);
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 3;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				if (section == (int)Sections.Offers)
				{
					return this.selectedPublishedOffers.Where(x => x.OfferCode != OfferCode.Coupon).Count();
				}
				else if (section == (int)Sections.Coupons)
				{
					return this.selectedPublishedOffers.Where(x => x.OfferCode == OfferCode.Coupon).Count();
				}
				else
				{
					return 0;
				}
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				QRCodeCell cell = tableView.DequeueReusableCell (QRCodeCell.Key) as QRCodeCell;
				if (cell == null)
					cell = new QRCodeCell();

				// Set default values
				string description = string.Empty;
				string extraInfo = string.Empty;
				string imageAvgColor = string.Empty;
				string imageId = string.Empty;

				if (indexPath.Section == (int)Sections.Offers)
				{
					List<PublishedOffer> offers = this.selectedPublishedOffers.Where(x => x.OfferCode != OfferCode.Coupon).ToList();
					PublishedOffer offer = offers[indexPath.Row];

					description = offer.Description;
					extraInfo = offer.Details;
					Domain.Images.ImageView imageView = offer.Images.FirstOrDefault();
					imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					imageId = (imageView != null ? imageView.Id : string.Empty);
				}
				else if (indexPath.Section == (int)Sections.Coupons)
				{
					List<PublishedOffer> coupons = this.selectedPublishedOffers.Where(x => x.OfferCode == OfferCode.Coupon).ToList();
					PublishedOffer coupon = coupons[indexPath.Row];

					description = coupon.Description;
					extraInfo = coupon.Details;
					Domain.Images.ImageView imageView = coupon.Images.FirstOrDefault();
					imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
					imageId = (imageView != null ? imageView.Id : string.Empty);
				}

				cell.SetValues(indexPath.Row, description, extraInfo, imageAvgColor, imageId);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				tableView.DeselectRow(indexPath, true);
			}

			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if (indexPath.Section == (int)Sections.Offers)
				{
					List<PublishedOffer> offers = this.selectedPublishedOffers.Where(x => x.OfferCode != OfferCode.Coupon).ToList();
					PublishedOffer offer = offers[indexPath.Row];

					return QRCodeCell.GetCellHeight(offer.Details);
				}
				else if (indexPath.Section == (int)Sections.Coupons)
				{
					List<PublishedOffer> coupons = this.selectedPublishedOffers.Where(x => x.OfferCode == OfferCode.Coupon).ToList();
					PublishedOffer coupon = coupons[indexPath.Row];

					return QRCodeCell.GetCellHeight(coupon.Details);
				}
				else
				{
					return 0;
				}
			}

			public override UIView GetViewForHeader (UITableView tableView, nint section)
			{
				if (section == (int)Sections.DummySection)
				{
					return this.headerView;
				}
				else if (section == (int)Sections.Offers)
				{
					return this.offerHeaderView;
				}
				else if (section == (int)Sections.Coupons)
				{
					return this.couponHeaderView;
				}
				else
				{
					return null;
				}
			}

			public override nfloat GetHeightForHeader (UITableView tableView, nint section)
			{
				if (section == (int)Sections.DummySection)
				{
					return 255f;
				}
				else if(section == (int)Sections.Offers)
				{
					return 30f;
				}
				else if(section == (int)Sections.Coupons)
				{
					return 30f;
				}
				else
				{
					return 0f;
				}
			}

			private enum Sections
			{
				DummySection,
				Offers,
				Coupons
			}
		}

		public class QRCodeCell : UITableViewCell
		{
			public static string Key = "QRCodeCell";
			protected int id;

			private const float titleLabelHeight = 20f;
			private const float margin = 5f;
			private const float interCellSpacing = 10f;

			public QRCodeCell() : base(UITableViewCellStyle.Default, Key)
			{
				this.BackgroundColor = UIColor.Clear;
				this.SelectionStyle = UITableViewCellSelectionStyle.None;

				SetLayout();
			}

			public virtual void SetLayout()
			{
				UIView customContentView = new UIView();
				customContentView.BackgroundColor = UIColor.White;
				customContentView.Tag = 1;
				this.ContentView.AddSubview(customContentView);

				UIImageView imageView = new UIImageView();
				imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
				imageView.ClipsToBounds = true;
				imageView.BackgroundColor = UIColor.Purple;
				imageView.Tag = 100;
				customContentView.AddSubview(imageView);

				UILabel lblTitle = new UILabel();
				lblTitle.BackgroundColor = UIColor.Clear;
				lblTitle.TextColor = Utils.AppColors.PrimaryColor;
				lblTitle.Tag = 200;
				customContentView.AddSubview(lblTitle);

				UILabel lblExtraInfo = new UILabel();
				lblExtraInfo.BackgroundColor = UIColor.Clear;
				lblExtraInfo.TextColor = Utils.AppColors.MediumGray;
				lblExtraInfo.Font = UIFont.SystemFontOfSize(14f);
				lblExtraInfo.Tag = 300;
				customContentView.AddSubview(lblExtraInfo);

				this.ContentView.ConstrainLayout(() =>

					customContentView.Frame.Top ==  this.ContentView.Bounds.Top + interCellSpacing &&
					customContentView.Frame.Left == this.ContentView.Bounds.Left &&
					customContentView.Frame.Right == this.ContentView.Bounds.Right &&
					customContentView.Frame.Bottom == this.ContentView.Bounds.Bottom

				);

				customContentView.ConstrainLayout(() =>

					imageView.Frame.Top == customContentView.Frame.Top + 2 * margin &&
					imageView.Frame.Left == customContentView.Bounds.Left +  2 * margin &&
					imageView.Frame.Height == 50f &&
					imageView.Frame.Width == 50f &&

					lblTitle.Frame.Top == imageView.Frame.Top &&
					lblTitle.Frame.Left == imageView.Frame.Right + 2 * margin &&
					lblTitle.Frame.Right == customContentView.Frame.Right - 2 * margin &&
					lblTitle.Frame.Height == titleLabelHeight &&

					lblExtraInfo.Frame.Top == lblTitle.Frame.Bottom + margin &&
					lblExtraInfo.Frame.Left == lblTitle.Frame.Left &&
					lblExtraInfo.Frame.Right == customContentView.Frame.Right - 2 * margin
				);
			}

			public void SetValues(int id, string title, string extraInfo, string imageAvgColorHex, string imageId)
			{
				this.id = id;

				UILabel lblTitle = (UILabel)this.ContentView.ViewWithTag(200);
				lblTitle.Text = title;

				UILabel lblExtraInfo = (UILabel)this.ContentView.ViewWithTag(300);
				lblExtraInfo.Lines = Utils.Util.GetStringLineCount(extraInfo);
				lblExtraInfo.Text = extraInfo;
				lblExtraInfo.SizeToFit();

				UIImageView imageView = (UIImageView)this.ContentView.ViewWithTag(100);
				if (String.IsNullOrEmpty(imageAvgColorHex))
					imageAvgColorHex = "E0E0E0"; // Default to light gray
				imageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(imageAvgColorHex);

				Utils.UI.LoadImageToImageView(imageId, false, imageView, new Domain.Images.ImageSize(100, 100), this.id.ToString());
			}

			private static nfloat GetExtraInfoLabelHeight(string extraInfoString)
			{
				// Let's get the height of the extra info label by creating a templabel that is exactly like the one used in the
				// actual cell instance and then apply SizeToFit().

				UILabel tempLabel = new UILabel();
				tempLabel.Text = extraInfoString;
				tempLabel.Font = UIFont.SystemFontOfSize(12f);
				tempLabel.Lines = Utils.Util.GetStringLineCount(extraInfoString);
				tempLabel.SizeToFit();
				return tempLabel.Frame.Height;
			}

			public static nfloat GetCellHeight(string extraInfoString)
			{
				float minHeight = interCellSpacing + 7 * margin + titleLabelHeight;
				return minHeight + GetExtraInfoLabelHeight(extraInfoString);
			}
		}
	}
	*/
}

