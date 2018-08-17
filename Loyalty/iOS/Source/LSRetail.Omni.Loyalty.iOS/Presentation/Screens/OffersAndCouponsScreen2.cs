﻿
using System;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;
using Foundation;
using UIKit;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
    public class OffersAndCouponsScreen2 : CardCollectionViewController
	{
		private UIView noDataView;
		private UIRefreshControl refreshControl;

		private UIToolbar segmentContainer;
		private UISegmentedControl segmentedControl;

		private float toolbarHeight = 48f;
		private UIImageView navBarHairLine;

		public OffersAndCouponsScreen2 (UICollectionViewFlowLayout layout) : base (layout)
		{
			this.Title = LocalizationUtilities.LocalizedString("OffersAndCoupons_OffersAndCoupons", "Offers & coupons");

			this.cellSize = AppData.CellSize;

			this.segmentedControl = new UISegmentedControl();
			this.segmentedControl.InsertSegment(LocalizationUtilities.LocalizedString("OffersAndCoupons_Offers", "Offers"), 0, true);
			this.segmentedControl.InsertSegment(LocalizationUtilities.LocalizedString("OffersAndCoupons_Coupons", "Coupons"), 1, true);
			this.segmentedControl.TintColor = AppColors.PrimaryColor;
			this.segmentedControl.SelectedSegment = 0;
			this.segmentedControl.ValueChanged += (sender, e) => 
			{
				var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;

				if(selectedSegmentId == 0)
					OnShowOffersButtonPressed();
				else
					OnShowCouponsButtonPressed();
			};
		}
			
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			this.navBarHairLine.Hidden = true;

			// NOTE:
			// With the current implementation we only want to refresh coupons/offers if we have a user logged in ... since coupons/offers are linked to the user account
			// and we need the contact ID to refresh them. This might change in the future?
			if (AppData.ShouldRefreshPublishedOffers && AppData.UserLoggedIn) 
				RefreshPublishedOffers();

			if(AppData.UserLoggedIn)
				this.NavigationItem.RightBarButtonItems[1].Enabled = true;
			else
				this.NavigationItem.RightBarButtonItems[1].Enabled = false;
				
			RefreshCollectionViewData();
			 
			CellSize = AppData.CellSize;
		}
			
		public override void ViewDidLoad ()
		{			
			this.CollectionView.DataSource = new OffersAndCouponsScreen2CollectionSource(this, OffersAndCouponsScreen2CollectionSource.PossibleDisplayModes.Offers);
			ToggleHeaderViews(true);

			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.CollectionView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.navBarHairLine = GetNavBarHairLineImageView(this.NavigationController.NavigationBar);

			this.segmentContainer = new UIToolbar();
			this.segmentContainer.Delegate = new CustomToolbarDelegate();
			this.View.AddSubview(this.segmentContainer);

            var barItem = new UIBarButtonItem(segmentedControl);
            var barObjects = new[] { barItem };
            this.segmentContainer.Items = barObjects;

			this.CollectionView.ContentInset = new UIEdgeInsets(
				this.CollectionView.ContentInset.Top + toolbarHeight,
				this.CollectionView.ContentInset.Left,
				this.CollectionView.ContentInset.Bottom,
				this.CollectionView.ContentInset.Right
			);

			this.CollectionView.RegisterClassForSupplementaryView (typeof(OfferAndCouponsHeaderView), UICollectionElementKindSection.Header, OfferAndCouponsHeaderView.Key);

			this.refreshControl = new UIRefreshControl();
			this.refreshControl.ValueChanged += (object sender, EventArgs e) => 
			{
				RefreshPublishedOffers();
			};
			this.CollectionView.AddSubview(refreshControl);

			//SetLeftBarButtonItem();
			SetRightBarButtonItems();

			RefreshCollectionViewData();
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			this.segmentContainer.Frame = new CGRect(0f, this.TopLayoutGuide.Length, this.View.Frame.Width, toolbarHeight);

			float margin = 10f;
			this.segmentedControl.Frame = new CGRect(2*margin, margin, this.segmentContainer.Frame.Width - 4*margin, this.segmentContainer.Frame.Height - 2*margin);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			this.navBarHairLine.Hidden = false;
		}
			
		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			UIButton changeLayoutButton = new UIButton();
			changeLayoutButton.SetImage(
				ImageUtilities.GetColoredImage(
					Utils.UI.MapCellSizeToIcon(CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize)), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
			changeLayoutButton.Frame = new CGRect (0, 0, 30, 30);
			changeLayoutButton.TouchUpInside += (object sender, EventArgs e) => {

				CellSize = CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize);

				changeLayoutButton.SetImage(
					ImageUtilities.GetColoredImage(
						Utils.UI.MapCellSizeToIcon(CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize)), Utils.UI.NavigationBarContentColor), UIControlState.Normal);

			};
			barButtonItemList.Add(new UIBarButtonItem(changeLayoutButton));


			// Generate QR code button
			UIButton generateQRCodeButton = new UIButton ();
			generateQRCodeButton.Frame = new CGRect (0, 0, 30, 30);
			generateQRCodeButton.SetImage (ImageUtilities.GetColoredImage(ImageUtilities.FromFile ("iconQRCodeWhite.png"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
			generateQRCodeButton.TouchUpInside += (sender, e) => 
			{
				this.PresentViewController(new UINavigationController (
					new QRCodeController(GenerateQRCodeXML(), QRCodeViewType.PublishedOffers)), true, () => {}
				);
			};				
			barButtonItemList.Add(new UIBarButtonItem(generateQRCodeButton));


			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private string GenerateQRCodeXML()
		{
			string xml = string.Format("<mobiledevice><contactid>{0}</contactid><accountid>{1}</accountid><cardid>{2}</cardid>",
				AppData.Device.UserLoggedOnToDevice.Id,
				AppData.Device.UserLoggedOnToDevice.Account.Id,
				AppData.Device.UserLoggedOnToDevice.Card.Id);

			xml += "<coupons>";

			foreach(PublishedOffer selectedCoupon in AppData.SelectedPublishedOffers.Where(x => x.Code == OfferDiscountType.Coupon).ToList())
			{
				string couponId = selectedCoupon.Id;
				xml += string.Format("<cid>{0}</cid>", couponId);
			}

			xml += "</coupons>";
			xml += "<offers>";

			foreach(PublishedOffer offer in AppData.SelectedPublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList())
			{
				string offerId = offer.Id;
				xml += string.Format("<oid>{0}</oid>", offerId);
			}

			xml += "</offers>";
			xml += "</mobiledevice>";

			return xml;
		}

		public void AddRemoveCouponOfferQRCode(object objectInCell)
		{
			if(objectInCell is PublishedOffer)
			{
				PublishedOffer publishedOffer = objectInCell as PublishedOffer;

				if(publishedOffer.Selected)
				{
					publishedOffer.Selected = false;
				}
				else
				{
					publishedOffer.Selected = true;
				}
			}
		}

		public override void HeaderSelected(object objectInCell)
		{
			System.Diagnostics.Debug.WriteLine ("header selected");
		}

		public override void CellSelected(object objectInCell)
		{
			if(objectInCell is PublishedOffer)
			{
				PublishedOffer selectedPublishedOffer = objectInCell as PublishedOffer;

				if(selectedPublishedOffer != null)
				{
					PublishedOfferDetailController detailsController = new PublishedOfferDetailController(selectedPublishedOffer);
					this.NavigationController.PushViewController(detailsController, true);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("selectedPublishedOffer is null");
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Unkown object selected: " + objectInCell.GetType().ToString());
			}
		}
			
		public override CardCollectionCell.CellSizes CellSize {
			get {
				return this.cellSize;
			}
			set {
				this.cellSize = value;
				AppData.CellSize = value;
				SetCellSize(value);
				RefreshCollectionViewData();
			}
		}

		public override List<CardCollectionCell.CellSizes> AvailableCellSizes {
			get {
				return new List<CardCollectionCell.CellSizes>(){
					CardCollectionCell.CellSizes.TallWide,
					CardCollectionCell.CellSizes.TallNarrow,
					CardCollectionCell.CellSizes.ShortWide
				};
			}
		}

		public override void RegisterCellClasses ()
		{
			CollectionView.RegisterClassForCell (typeof(OffersAndCouponsScreen2Cell), CardCollectionCell.ShortNarrowCellKey);
			CollectionView.RegisterClassForCell (typeof(OffersAndCouponsScreen2Cell), CardCollectionCell.ShortWideCellKey);
			CollectionView.RegisterClassForCell (typeof(OffersAndCouponsScreen2Cell), CardCollectionCell.TallNarrowCellKey);
			CollectionView.RegisterClassForCell (typeof(OffersAndCouponsScreen2Cell), CardCollectionCell.TallWideCellKey);
		}

		public void RefreshCollectionViewData()
		{
			(this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).RefreshCellTemplates ();
			(this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).RefreshHeaderTemplates ();
			this.CollectionView.ReloadData ();	// TODO: Animate

			RefreshNoDataView();
		}

		#region No data view

		private void RefreshNoDataView()
		{
			if (!(this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).HasData)
				ShowNoDataView();
			else
				HideNoDataView();
		}

		private void ShowNoDataView()
		{
			if (this.noDataView == null)
			{
				UIView ndView = new UIView();
				ndView.Frame = new CGRect(0, this.TopLayoutGuide.Length + this.toolbarHeight, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length - this.toolbarHeight);
				ndView.BackgroundColor = this.CollectionView.BackgroundColor;

				UILabel noDataText = new UILabel();
				float labelHeight = 20;
				noDataText.Frame = new CGRect(0, ndView.Bounds.Height/2 - labelHeight/2 - 20, ndView.Bounds.Width, labelHeight);	// NOTE: Subtracting 20 because this.View is at Y=20 (xib junk).
				noDataText.Text = GetNoDataViewText();
				noDataText.TextColor = UIColor.Gray;
				noDataText.TextAlignment = UITextAlignment.Center;
				noDataText.Font = UIFont.SystemFontOfSize(14);
				noDataText.Tag = 400;
				ndView.AddSubview(noDataText);

				this.noDataView = ndView;
				this.View.AddSubview(this.noDataView);
			}
			else
			{
				this.noDataView.Hidden = false;
				UILabel noDataText = this.noDataView.ViewWithTag(400) as UILabel;
				noDataText.Text = GetNoDataViewText();
			}
		}

		private void HideNoDataView()
		{
			if (this.noDataView != null)
				this.noDataView.Hidden = true;
		}
			
		private string GetNoDataViewText()
		{
			if ((this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).ActiveDisplayMode == OffersAndCouponsScreen2CollectionSource.PossibleDisplayModes.Offers)
			{
				if (AppData.UserLoggedIn)
					return LocalizationUtilities.LocalizedString("OffersAndCoupons_NoOffers", "There are no offers available");
				else
					return LocalizationUtilities.LocalizedString("OffersAndCoupons_NoOffersPleaseLogIn", "No offers, try logging in.");
			}
			else if ((this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).ActiveDisplayMode == OffersAndCouponsScreen2CollectionSource.PossibleDisplayModes.Coupons)
			{
				if (AppData.UserLoggedIn)
					return LocalizationUtilities.LocalizedString("OffersAndCoupons_NoCoupons", "There are no coupons available");
				else
					return LocalizationUtilities.LocalizedString("OffersAndCoupons_NoCouponsPleaseLogIn", "No coupons, try logging in.");
			}
			else
			{
				return LocalizationUtilities.LocalizedString("OffersAndCoupons_NoOffersAndCoupons", "There are no offers or coupons available");
			}
		}

		#endregion

		private void ToggleHeaderViews(bool showHeaders)
		{
			UICollectionViewFlowLayout myLayout = this.Layout as UICollectionViewFlowLayout;

			if (showHeaders)
				myLayout.HeaderReferenceSize = new CGSize (this.CollectionView.Bounds.Width, 40f);
			else
				myLayout.HeaderReferenceSize = new CGSize (this.CollectionView.Bounds.Width, 0f);
		}

		/// <summary>
		/// Refresh offers with webservice
		/// </summary>
		private async void RefreshPublishedOffers()
		{
			System.Diagnostics.Debug.WriteLine("Refreshing published offers");

            bool success = await new Models.OfferModel().GetPublishedOffersByCardId(AppData.Device.CardId);
            if (success)
			{
				AppData.ShouldRefreshPublishedOffers = false;
				this.refreshControl.EndRefreshing();
				RefreshCollectionViewData();
			}
            else
			{
				this.refreshControl.EndRefreshing();
			}
			
		}

		private void OnShowOffersButtonPressed()
		{
			if ((this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).ActiveDisplayMode != OffersAndCouponsScreen2CollectionSource.PossibleDisplayModes.Offers)
			{
				System.Diagnostics.Debug.WriteLine("showing offers ...");

				ToggleHeaderViews(true);

				(this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).ActiveDisplayMode = OffersAndCouponsScreen2CollectionSource.PossibleDisplayModes.Offers;
				RefreshCollectionViewData();

				RefreshNoDataView();
			}
		}

		private void OnShowCouponsButtonPressed()
		{
			if ((this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).ActiveDisplayMode != OffersAndCouponsScreen2CollectionSource.PossibleDisplayModes.Coupons)
			{
				System.Diagnostics.Debug.WriteLine("showing coupons ...");

				ToggleHeaderViews(false);

				(this.CollectionView.DataSource as OffersAndCouponsScreen2CollectionSource).ActiveDisplayMode = OffersAndCouponsScreen2CollectionSource.PossibleDisplayModes.Coupons;
				RefreshCollectionViewData();

				RefreshNoDataView();
			}
		}

		// Finds the small grey seperator view at the bottom of the NavBar so we can hide it
		private UIImageView GetNavBarHairLineImageView(UIView view)
		{
			if(view is UIImageView && (view as UIImageView).Bounds.Size.Height <= 1f)
			{
				return view as UIImageView;
			}

			foreach (UIView subView in view.Subviews)
			{
				UIImageView imageView = GetNavBarHairLineImageView(subView);

				if(imageView != null)
				{
					return imageView;
				}
			}

			return null;
		}
	}

	// Header view
	public class OfferAndCouponsHeaderView : UICollectionReusableView
	{
		public static readonly NSString Key = new NSString ("OfferAndCouponsHeaderView");
		private UILabel titleLabel;

		[Export("initWithFrame:")]
		public OfferAndCouponsHeaderView(CGRect frame) : base(frame)
		{
			this.titleLabel = new UILabel();
			this.titleLabel.TextColor = UIColor.Gray;
			this.titleLabel.Font = UIFont.SystemFontOfSize(14);
			this.titleLabel.Frame = new CGRect(15f, frame.Height/2, frame.Width, frame.Height/2);
			AddSubview(this.titleLabel);
		}

		public void SetValues(string headerTitle)
		{
			this.titleLabel.Text = headerTitle;
		}
	}

	// SegmentedControl Toolbar delegate
	public class CustomToolbarDelegate : UIToolbarDelegate
	{
		public override UIBarPosition GetPositionForBar (IUIBarPositioning barPositioning)
		{
			return UIBarPosition.TopAttached;
		}
	}
}