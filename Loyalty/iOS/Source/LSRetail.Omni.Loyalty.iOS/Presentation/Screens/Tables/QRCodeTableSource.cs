using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using Presentation.Utils;
using System.Linq;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class QRCodeTableSource : UITableViewSource
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
			qrCodeInstructions.Text = LocalizationUtilities.LocalizedString("Coupon_Details_PleaseScan", "Please scan QR code");

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
			offerLabel.Text = LocalizationUtilities.LocalizedString("OffersAndCoupons_Offers", "Offers");
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
			couponLabel.Text = LocalizationUtilities.LocalizedString("OffersAndCoupons_Coupons", "Coupons");
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
				return this.selectedPublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).Count();
			}
			else if (section == (int)Sections.Coupons)
			{
				return this.selectedPublishedOffers.Where(x => x.Code == OfferDiscountType.Coupon).Count();
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
				List<PublishedOffer> offers = this.selectedPublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList();
				PublishedOffer offer = offers[indexPath.Row];

				description = offer.Description;
				extraInfo = offer.Details;
				ImageView imageView = offer.Images.FirstOrDefault();
				imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
				imageId = (imageView != null ? imageView.Id : string.Empty);
			}
			else if (indexPath.Section == (int)Sections.Coupons)
			{
				List<PublishedOffer> coupons = this.selectedPublishedOffers.Where(x => x.Code == OfferDiscountType.Coupon).ToList();
				PublishedOffer coupon = coupons[indexPath.Row];

				description = coupon.Description;
				extraInfo = coupon.Details;
				ImageView imageView = coupon.Images.FirstOrDefault();
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
				List<PublishedOffer> offers = this.selectedPublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList();
				PublishedOffer offer = offers[indexPath.Row];

				return QRCodeCell.GetCellHeight(offer.Details);
			}
			else if (indexPath.Section == (int)Sections.Coupons)
			{
				List<PublishedOffer> coupons = this.selectedPublishedOffers.Where(x => x.Code == OfferDiscountType.Coupon).ToList();
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
}

