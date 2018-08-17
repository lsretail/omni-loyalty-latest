using System;
using CoreGraphics;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;
using CoreAnimation;
using Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Presentation.Models;
using Domain.Images;

namespace Presentation.Screens
{
	public class LocationDetailsScreen : UIViewController
	{
		public Store store;
		private List<Store> stores { get; set;}
		private UIScrollView scrollView;
		private UIView detailsView;
		private UIScrollView scrollImgView;
		private UIToolbar toolbar;
		private UIButton transparentView;
		private UIView transparentImgView;
		private CGPoint currentImgPoint;
		private CGPoint currentImgOffset;
		private CGPoint beginningPoint;
		private CGPoint newPoint;
		private UIPageControl pageControlImage;
		private List<UIImageView> imageViews;
		private List<UIImage> images;
		//private Dictionary<string, UIImageView> imageViewIdToUIImageViewMap;
		//private Task DownloadTask { get; set; }
		private float headerImageHeight = 220f;

		public LocationDetailsScreen (Store store, List<Store> stores)
		{
			this.Title = this.store.Description;
			this.store = store;
			this.stores = stores;	// TODO Don't keep all the store in here. Should get this from somewhere else.
			this.currentImgPoint = new CGPoint (0, 0);
			this.currentImgOffset = new CGPoint (0, 0);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = UIColor.White;

			#region Toolbar

			this.toolbar = new UIToolbar();
			this.toolbar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.toolbar.Translucent = false;
			this.toolbar.TintColor = UIColor.White;
			this.View.AddSubview(this.toolbar);
			float toolbarButtonDimensions = 30f;

			UIButton btnOrder = new UIButton (UIButtonType.Custom);
			btnOrder.SetImage (Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconShoppingBasket.png"), UIColor.White), UIControlState.Normal);
			btnOrder.Frame = new CGRect (0, 0, toolbarButtonDimensions, toolbarButtonDimensions);
			UIBarButtonItem btnBarOrder = new UIBarButtonItem (btnOrder);

			UIButton btnMap = new UIButton (UIButtonType.Custom);
			btnMap.SetImage (Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconMap.png"), UIColor.White), UIControlState.Normal);
			btnMap.Frame = new CGRect (0, 0, toolbarButtonDimensions, toolbarButtonDimensions);
			UIBarButtonItem btnBarMap = new UIBarButtonItem (btnMap);

			UIButton btnDirections = new UIButton (UIButtonType.Custom);
			btnDirections.SetImage (Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconDirections.png"), UIColor.White), UIControlState.Normal);
			btnDirections.Frame = new CGRect (0, 0, toolbarButtonDimensions, toolbarButtonDimensions);
			UIBarButtonItem btnBarDirections = new UIBarButtonItem (btnDirections);

			var flexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

			// TODO Re-add btnBarOrder
			// Hide the "order from this restaurant" button until we have some more functionality behind it
			//this.toolbarRestaurant.SetItems (new [] { btnBarOrder, flexibleSpace, btnBarMap, flexibleSpace, btnBarDirections }, true);
			this.toolbar.SetItems (new [] { btnBarMap, flexibleSpace, btnBarDirections }, true);

			btnOrder.TouchUpInside += (sender, e) => 
			{
				this.NavigationController.PopToRootViewController(true);
				//Utils.Util.AppDelegate.SlideoutMenu.MenuItemPressedMenu();
			};

			btnMap.TouchUpInside += (sender, e) => 
			{
				MapController map = new MapController (this.store, this.stores);
				this.NavigationController.PushViewController(map, true);

			};
			btnDirections.TouchUpInside += (sender, e) => 
			{
				RestaurantDirectionsDialogController directions = new RestaurantDirectionsDialogController(this.store);
				this.NavigationController.PushViewController(directions, true);
			};

			#endregion

			#region Details view

			nfloat scrollViewContentHeight = 0f;	// Determine height of details and scrollview dynamically to fit all the content they need to display
			nfloat detailsViewContentHeight = 0f;
			nfloat xMargin = 10f;
			nfloat yMargin = 10f;

			scrollView = new UIScrollView ();
			scrollView.Frame = this.View.Frame;
			scrollView.BackgroundColor = UIColor.Clear;
			scrollView.ShowsVerticalScrollIndicator = false;
			scrollView.ContentInset = new UIEdgeInsets(0f, 0f, yMargin, 0f);

			// Transparent view (to see imageview through)
			transparentImgView = new UIView ();
			transparentImgView.Frame = new CGRect (0f, 0f, scrollView.Frame.Width, this.headerImageHeight);
			transparentImgView.BackgroundColor = UIColor.Clear;
			transparentImgView.UserInteractionEnabled = true;

			//Gesture recognizer to swipe between images
			if(this.store.Images != null && this.store.Images.Count > 1)
			{
				UIPanGestureRecognizer swipe = new UIPanGestureRecognizer();
				swipe.AddTarget(() => { HandleDrag(swipe); });
				transparentImgView.AddGestureRecognizer(swipe);
			}

			//Gesture recognizer, click to view images larger
			UITapGestureRecognizer tap = new UITapGestureRecognizer();
			tap.AddTarget(() => { HandleTap(tap);} );
			transparentImgView.AddGestureRecognizer(tap);

			// The detailsview itself
			detailsView = new UIView ();
			detailsView.Frame = new CGRect (0f, transparentImgView.Frame.Bottom, scrollView.Frame.Width, 1); // Set arbitrary height
			detailsView.BackgroundColor = UIColor.Clear;

			// Title
			UILabel title = new UILabel ();
			title.Frame = new CGRect (xMargin, yMargin, detailsView.Frame.Width - 2 * xMargin, 20f);
			title.UserInteractionEnabled = false;
			title.Text = this.store.Description;
			title.TextColor = Utils.AppColors.PrimaryColor;
			title.Font = UIFont.BoldSystemFontOfSize (17);
			title.BackgroundColor = UIColor.Clear;
			title.TextAlignment = UITextAlignment.Left;
			title.Tag = 100;
			detailsView.AddSubview (title);
			detailsViewContentHeight += yMargin;
			detailsViewContentHeight += title.Frame.Height;

			// Address text
			UITextView txtAddress = new UITextView();
			txtAddress.Frame = new CGRect(xMargin, title.Frame.Bottom, detailsView.Frame.Width - 2 * xMargin, 1f); // Set arbitrary height
			txtAddress.Editable = false;
			txtAddress.ScrollEnabled = false;
			txtAddress.Text = this.store.FormatAddress;
			txtAddress.Font = UIFont.SystemFontOfSize(15f);
			txtAddress.BackgroundColor = UIColor.Clear;
			//TODO: size to fit not working as it should (like in ItemDetailsScreen)
			CGSize newSizeThatFits = txtAddress.SizeThatFits(txtAddress.Frame.Size);
			CGRect tempFrame = txtAddress.Frame;
			tempFrame.Size = new CGSize(tempFrame.Size.Width, newSizeThatFits.Height);	// Only adjust the height
			txtAddress.Frame = tempFrame;
			detailsView.AddSubview(txtAddress);	
			detailsViewContentHeight += txtAddress.Frame.Height;

			// Phone text
			UITextView txtPhone = new UITextView();
			txtPhone.Frame = new CGRect(xMargin, txtAddress.Frame.Bottom, detailsView.Frame.Width - 2 * xMargin, 1f); // Set arbitrary height
			txtPhone.Editable = false;
			txtPhone.ScrollEnabled = false;
			txtPhone.Text = NSBundle.MainBundle.LocalizedString("Location_Details_Phone", "Phone") + ": " + this.store.Phone;
			txtPhone.Font = UIFont.SystemFontOfSize(14f);
			txtPhone.BackgroundColor = UIColor.Clear;
			txtPhone.Selectable = true;
			txtPhone.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
			CGSize newSizeThatFitsPhone = txtPhone.SizeThatFits(txtPhone.Frame.Size);
			CGRect tempFramePhone = txtPhone.Frame;
			tempFramePhone.Size = new CGSize(tempFramePhone.Size.Width, newSizeThatFitsPhone.Height);	// Only adjust the height
			txtPhone.Frame = tempFramePhone;
			detailsView.AddSubview(txtPhone);	
			detailsViewContentHeight += txtPhone.Frame.Height;

			// Store services
			UIScrollView servicesView = new UIScrollView();
			servicesView.Frame = new CGRect(xMargin, txtPhone.Frame.Bottom + 5f, detailsView.Frame.Width - 2 * xMargin, 60f);
			if (!LoadStoreServicesImages(servicesView))
			{
				// No images added to servicesView, let's hide it
				servicesView.Hidden = true;
				servicesView.Frame = new CGRect(servicesView.Frame.X, servicesView.Frame.Y, servicesView.Frame.Width, 0f);
			}
			detailsView.AddSubview(servicesView);
			detailsViewContentHeight += servicesView.Frame.Height;

			// Opening hours

			// Opening hours label
			UILabel openingHours = new UILabel ();
			openingHours.Frame = new CGRect (xMargin, servicesView.Frame.Bottom + 10f, detailsView.Frame.Width - 2 * xMargin, 20f);
			openingHours.UserInteractionEnabled = false;
			openingHours.Text = NSBundle.MainBundle.LocalizedString("Location_Details_OpeningHours", "Opening hours");
			openingHours.TextColor = Utils.AppColors.PrimaryColor;
			openingHours.Font = UIFont.BoldSystemFontOfSize (15f);
			openingHours.BackgroundColor = UIColor.Clear;
			openingHours.TextAlignment = UITextAlignment.Left;
			openingHours.Tag = 200;
			if (this.store.StoreHours.Count == 0)
				openingHours.Hidden = true;
			detailsView.AddSubview (openingHours);
			detailsViewContentHeight += openingHours.Frame.Height;

			#region StoreHours textviews
			// We use two textviews side by side to get a column-look for the opening hours. One column containing the days, the other the opening hours.

			// StoreHourType and days
			UITextView txtStoreHourTypeAndDays = new UITextView();
			txtStoreHourTypeAndDays.Frame = new CGRect(xMargin, openingHours.Frame.Bottom + 5f, (detailsView.Frame.Width - 2 * xMargin) / 2, 1f); // Set arbitrary height
			txtStoreHourTypeAndDays.Editable = false;
			txtStoreHourTypeAndDays.ScrollEnabled = false;
			txtStoreHourTypeAndDays.Font = UIFont.SystemFontOfSize(14f);
			txtStoreHourTypeAndDays.BackgroundColor = UIColor.Clear;

			// Opening hours
			UITextView txtOpeningHours = new UITextView();
			txtOpeningHours.Frame = new CGRect(txtStoreHourTypeAndDays.Frame.Right, openingHours.Frame.Bottom + 5f, (detailsView.Frame.Width - 2 * xMargin) / 2, 1f); // Set arbitrary height
			txtOpeningHours.Editable = false;
			txtOpeningHours.ScrollEnabled = false;
			txtOpeningHours.Font = UIFont.SystemFontOfSize(14f);
			txtOpeningHours.BackgroundColor = UIColor.Clear;

			SetStoreHoursText(txtStoreHourTypeAndDays, txtOpeningHours);

			CGSize newSizeThatFitsStoreHourTypeAndDays = txtStoreHourTypeAndDays.SizeThatFits(txtStoreHourTypeAndDays.Frame.Size);
			CGRect tempFrameStoreHourTypeAndDays = txtStoreHourTypeAndDays.Frame;
			tempFrameStoreHourTypeAndDays.Size = new CGSize(tempFrameStoreHourTypeAndDays.Size.Width, newSizeThatFitsStoreHourTypeAndDays.Height);	// Only adjust the height
			txtStoreHourTypeAndDays.Frame = tempFrameStoreHourTypeAndDays;
			detailsView.AddSubview(txtStoreHourTypeAndDays);	

			CGSize newSizeThatFitsOpeningHours = txtOpeningHours.SizeThatFits(txtOpeningHours.Frame.Size);
			CGRect tempFrameOpeningHours = txtOpeningHours.Frame;
			tempFrameOpeningHours.Size = new CGSize(tempFrameOpeningHours.Size.Width, newSizeThatFitsOpeningHours.Height);	// Only adjust the height
			txtOpeningHours.Frame = tempFrameOpeningHours;
			detailsView.AddSubview(txtOpeningHours);	

			detailsViewContentHeight += (nfloat)System.Math.Max(txtStoreHourTypeAndDays.Frame.Height, txtOpeningHours.Frame.Height);	// Should be the same height though

			#endregion

			// We want the scrollview to be scrollable even though the entire content fits on the screen.
			// Let's do this by setting a minimum height for the detailsViewContentHeight
			nfloat minDetailsViewContentHeight = Utils.Util.AppDelegate.DeviceScreenHeight - detailsView.Frame.Y;
			if (detailsViewContentHeight < minDetailsViewContentHeight)
				detailsViewContentHeight = minDetailsViewContentHeight;

			// Resize views to fit content (i.e. adjust height)
			scrollViewContentHeight += transparentImgView.Frame.Height;
			scrollViewContentHeight += detailsViewContentHeight;
			// TODO Find a better solution than this hack, this seems to work, but I don't know why
			// NOTE: This is the only details screen that behaves this way. Probably because it is built with a xib and not the others.
			// HACK It seems the screensize matters in these calculations, hack the problem away by handling different screensizes differently
			// Maybe using toplayoutguide instead of our own StatusbarPlusNavbarHeight would fix it.
			// Moreover, I don't think StatusBarPlusNavBarHeight is the correct value to add to the scrollviewcontentheight, but it seems to work OK
			/*
			if (Utils.Util.AppDelegate.DeviceScreenHeight < Utils.Util.AppDelegate.ScreenHeight4Inch)
				scrollViewContentHeight += Utils.Util.AppDelegate.StatusbarPlusNavbarHeight;
			*/
			scrollViewContentHeight += Utils.Util.AppDelegate.StatusbarPlusNavbarHeight;
			scrollViewContentHeight += 44f; //height of the toolbar
			scrollView.ContentSize = new CGSize (scrollView.Frame.Width, scrollViewContentHeight);
			detailsView.Frame = new CGRect(detailsView.Frame.Location, new CGSize(detailsView.Frame.Size.Width, detailsViewContentHeight));

			SetupPagingImageViews();
			LoadImages();

			scrollView.AddSubview (this.transparentImgView);
			scrollView.AddSubview (this.detailsView);
			this.View.AddSubview (this.scrollView);

			// Add transparentwhite-to-white gradient to detailsview
			CoreAnimation.CAGradientLayer gradientLayer = new CoreAnimation.CAGradientLayer ();
			gradientLayer.Frame = detailsView.Bounds;
			CoreGraphics.CGColor[] colors = new CoreGraphics.CGColor[2];
			colors [0] = Utils.AppColors.TransparentWhite.CGColor;
			colors [1] = UIColor.White.CGColor;
			gradientLayer.Colors = colors;
			gradientLayer.EndPoint = new CGPoint (0.5f, 0.2f);
			detailsView.Layer.InsertSublayer (gradientLayer, 0);

			#endregion

			this.View.BringSubviewToFront (this.toolbar);

			SetRightBarButtonItems();
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			//TODO : Move all frame-handling-code in here

			this.toolbar.Frame = new CGRect(0f, this.View.Frame.Bottom - 44f, this.View.Frame.Width, 44f);
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private void SetupPagingImageViews()
		{
			int pageSize = (int)Utils.Util.AppDelegate.DeviceScreenWidth;
			float offset = 0;
			int pageCount = this.store.Images.Count;
			//this.imageViewIdToUIImageViewMap = new Dictionary<string, UIImageView>();

			this.imageViews = new List<UIImageView> ();
			this.scrollImgView = new UIScrollView (new CGRect (this.View.Frame.X, this.View.Frame.Y, this.View.Frame.Width, this.headerImageHeight));

			for (int i = 0; i < pageCount; i++)
			{
				if (i > 0)
				{
					offset += pageSize;
				}

				UIImageView imgView = new UIImageView ();
				imgView.Frame = new CGRect (this.View.Frame.X, this.View.Frame.Y, this.View.Frame.Width, this.headerImageHeight);
				imgView.ClipsToBounds = true;
				imgView.ContentMode = UIViewContentMode.ScaleAspectFill;
				imgView.BackgroundColor = Utils.UI.GetUIColorFromHexString (this.store.Images [i].AvgColor);
				this.imageViews.Add (imgView);

				var clickableImageView = new UIButton(new CGRect(offset, 0, this.View.Frame.Width, this.headerImageHeight));
				clickableImageView.AddSubview (imgView);
				this.scrollImgView.Add(clickableImageView);

				//create the transparent image as a button
				this.transparentView = new UIButton(new CGRect(offset, 0, this.View.Frame.Width, this.headerImageHeight));
				this.transparentView.Alpha = 0.4f;
				this.scrollImgView.Add(this.transparentView);

				this.scrollImgView.ContentSize = new CGSize(pageCount * pageSize + 130, this.headerImageHeight);
				this.scrollImgView.ContentInset = new UIEdgeInsets(0f, 0f, 0f, 0f);

				/* TODO Rethink this. If the image IDs are not unique we get non-unique keys, crash. Fix it like this for now.
				if (!this.imageViewIdToUIImageViewMap.ContainsKey(this.store.Images[i].Id))
					this.imageViewIdToUIImageViewMap.Add(this.store.Images[i].Id, imgView);
					*/
			}

			this.pageControlImage = new UIPageControl(new CGRect(0, 60, this.View.Frame.Width, this.headerImageHeight));
			pageControlImage.HidesForSinglePage = true;
			pageControlImage.Pages = this.store.Images.Count;

			this.View.AddSubview (this.scrollImgView);
			this.View.AddSubview (this.pageControlImage);
		}

		private void LoadImages()
		{
			this.images = new List<UIImage> ();

			ImageModel imageModel = new ImageModel();
			int imageCount = 0;

			foreach (var storeImage in this.store.Images)
			{
				imageModel.ImageGetById(storeImage.Id, new Domain.Images.ImageSize(700, 500), 
					(x, destinationId) => {

						UIImage image = Utils.Image.FromBase64(x.Image);

						this.images.Add(image);

						// Add image to its UIImageView
						//UIImageView imageView = null;
						//this.imageViewIdToUIImageViewMap.TryGetValue(storeImage.Id, out imageView);

						UIImageView imageView = this.imageViews[imageCount];

						if (imageView != null)
						{
							imageView.Image = image;

							CATransition transition = new CATransition ();
							transition.Duration = 0.5f;
							transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
							transition.Type = CATransition.TransitionFade;
							imageView.Layer.AddAnimation (transition, null);
						}

						imageCount++;
					},
					() => { /* Do nothing */ }
				);
			}
		}

		private void SetStoreHoursText(UITextView txtViewTypeAndDays, UITextView txtViewOpeningHours)
		{
			// Must make the two texts in the textboxes align, i.e. show the correct opening hours for monday in the same line as "Monday"

			string storeHourTypeAndDaysString = string.Empty;	// Goes into the left textbox, type and days
			string openingHoursString = string.Empty;			// Goes into the right textbox, actual opening hours

			List<StoreHourType> storeHourTypes = this.store.StoreHours.Select(x => x.StoreHourType).Distinct().ToList();
			List<StoreHours> storeHoursFilteredByType = new List<StoreHours>();

			bool newLineRequired = false;
			foreach (var storeHourType in storeHourTypes)
			{
				storeHourTypeAndDaysString += (newLineRequired ? "\r\n" : string.Empty) + GetStoreHourTypeName(storeHourType) + ":\r\n\r\n";
				openingHoursString += (newLineRequired ? "\r\n" : string.Empty) + "\r\n\r\n";
				newLineRequired = true;

				storeHoursFilteredByType = this.store.StoreHours.Where(x => x.StoreHourType == storeHourType).ToList();

				foreach (var sHour in storeHoursFilteredByType)
				{
					storeHourTypeAndDaysString += GetStoreHoursDayName(sHour) + "\r\n";
					openingHoursString += sHour.OpenFrom.ToShortTimeString() + " - " + sHour.OpenTo.ToShortTimeString() + "\r\n";
				}
			}

			txtViewTypeAndDays.Text = storeHourTypeAndDaysString;
			txtViewOpeningHours.Text = openingHoursString;
		}

		private string GetStoreHourTypeName(StoreHourType storeHourType)
		{
			switch (storeHourType)
			{
			case StoreHourType.DriveThruWindow:
				return NSBundle.MainBundle.LocalizedString ("Location_OpeningHoursType_DriveThru", "Drive thru");
			case StoreHourType.MainStore:
				return NSBundle.MainBundle.LocalizedString ("Location_OpeningHoursType_MainStore", "Main store");
			default:
				return string.Empty;
			}
		}

		private string GetStoreHoursDayName(StoreHours storeHours)
		{
			switch ((int)storeHours.DayOfWeek)
			{
			case 0:
				return NSBundle.MainBundle.LocalizedString ("Weekday_Sunday", "Sunday");
			case 1:
				return NSBundle.MainBundle.LocalizedString ("Weekday_Monday", "Monday");
			case 2:
				return NSBundle.MainBundle.LocalizedString ("Weekday_Tuesday", "Tuesday");
			case 3:
				return NSBundle.MainBundle.LocalizedString ("Weekday_Wednesday", "Wednesday");
			case 4:
				return NSBundle.MainBundle.LocalizedString ("Weekday_Thursday", "Thursday");
			case 5:
				return NSBundle.MainBundle.LocalizedString ("Weekday_Friday", "Friday");
			case 6:
				return NSBundle.MainBundle.LocalizedString ("Weekday_Saturday", "Saturday");
			default:
				return string.Empty;
			}
		}

		private bool LoadStoreServicesImages(UIScrollView scrollViewServices)
		{
			bool imageAdded = false;
			nfloat xCoord = 0f;

			foreach (var service in this.store.StoreServices)
			{
				UIImage serviceImage = GetStoreServiceImage(service.StoreServiceType);

				if (serviceImage != null)
				{
					UIImageView serviceImageView = new UIImageView();
					serviceImageView.Image = serviceImage;
					serviceImageView.TintColor = Utils.AppColors.PrimaryColor;
					serviceImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					serviceImageView.Frame = new CGRect(xCoord, 0, scrollViewServices.Frame.Height - 20f, scrollViewServices.Frame.Height - 20f);
					scrollViewServices.AddSubview(serviceImageView);
					imageAdded = true;

					UILabel lblStoreServiceTitle = new UILabel () 
					{
						Frame = new CGRect (xCoord - 10, serviceImageView.Frame.Bottom, scrollViewServices.Frame.Height, 20f),
						Text = GetStoreServiceTitle(service.StoreServiceType),
						Font = UIFont.BoldSystemFontOfSize(10),
						TextColor = Utils.AppColors.PrimaryColor,
						TextAlignment = UITextAlignment.Center
					};
					scrollViewServices.AddSubview (lblStoreServiceTitle);

					xCoord = serviceImageView.Frame.Right + 18f;
				}
			}

			return imageAdded;
		}

		private UIImage GetStoreServiceImage(StoreServiceType serviceType)
		{
			switch (serviceType)
			{
			case StoreServiceType.DriveThruWindow:
				return Utils.UI.GetColoredImage(Utils.Image.FromFile("/Other/store_service_drive_through.png"), Utils.AppColors.PrimaryColor);
			case StoreServiceType.FreeRefill:
				return Utils.UI.GetColoredImage(Utils.Image.FromFile("/Other/store_service_free_refill.png"), Utils.AppColors.PrimaryColor);
			case StoreServiceType.FreeWiFi:
				return Utils.UI.GetColoredImage(Utils.Image.FromFile("/Other/store_service_free_wifi.png"), Utils.AppColors.PrimaryColor);
			case StoreServiceType.Garden:
				return Utils.UI.GetColoredImage(Utils.Image.FromFile("/Other/store_service_garden.png"), Utils.AppColors.PrimaryColor);
			case StoreServiceType.GiftCard:
				return Utils.UI.GetColoredImage(Utils.Image.FromFile("/Other/store_service_gift_card.png"), Utils.AppColors.PrimaryColor);
			case StoreServiceType.PlayPlace:
				return Utils.UI.GetColoredImage(Utils.Image.FromFile("/Other/store_service_play_place.png"), Utils.AppColors.PrimaryColor);
			case StoreServiceType.None:
				return null;
			default:
				return null;
			}
		}

		private string GetStoreServiceTitle(StoreServiceType serviceType)
		{
			switch (serviceType)
			{
			case StoreServiceType.DriveThruWindow:
				return NSBundle.MainBundle.LocalizedString("StoreService_DriveThrough", "Drive through");
			case StoreServiceType.FreeRefill:
				return NSBundle.MainBundle.LocalizedString("StoreService_FreeRefill", "Free refill");
			case StoreServiceType.FreeWiFi:
				return NSBundle.MainBundle.LocalizedString("StoreService_FreeWiFi", "WiFi");
			case StoreServiceType.Garden:
				return NSBundle.MainBundle.LocalizedString("StoreService_Garden", "Garden");
			case StoreServiceType.GiftCard:
				return NSBundle.MainBundle.LocalizedString("StoreService_GiftCard", "Gift card");
			case StoreServiceType.PlayPlace:
				return NSBundle.MainBundle.LocalizedString("StoreService_PlayPlace", "Play place");
			case StoreServiceType.None:
				return string.Empty;
			default:
				return string.Empty;
			}
		}

		private void HandleDrag(UIPanGestureRecognizer recognizer)
		{
			if(recognizer.State == UIGestureRecognizerState.Began)
			{
				this.beginningPoint = recognizer.TranslationInView (this.transparentImgView);
				this.currentImgOffset.X = this.scrollImgView.ContentOffset.X;
			}

			if(recognizer.State != (UIGestureRecognizerState.Ended | UIGestureRecognizerState.Cancelled |UIGestureRecognizerState.Failed))
			{
				this.newPoint = recognizer.TranslationInView (this.transparentImgView);
				this.currentImgPoint.X = this.beginningPoint.X - this.newPoint.X + this.currentImgOffset.X;
				this.scrollImgView.SetContentOffset (this.currentImgPoint, false);
			}

			if(recognizer.State == UIGestureRecognizerState.Ended)
			{
				nfloat length = this.beginningPoint.X - this.newPoint.X;

				if(length >= 60f)
				{
					if (this.pageControlImage.Pages != (this.pageControlImage.CurrentPage + 1)) 
					{
						this.currentImgPoint.X = (this.pageControlImage.CurrentPage + 1) * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
						this.pageControlImage.CurrentPage = this.pageControlImage.CurrentPage + 1;
					}
					else
					{
						this.currentImgPoint.X = this.pageControlImage.CurrentPage * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else if(length <= -60f)
				{
					if (this.pageControlImage.CurrentPage != 0) 
					{
						this.currentImgPoint.X = (this.pageControlImage.CurrentPage - 1) * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
						this.pageControlImage.CurrentPage = this.pageControlImage.CurrentPage - 1;
					}
					else
					{
						this.currentImgPoint.X = this.pageControlImage.CurrentPage * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else
				{
					this.currentImgPoint.X = this.pageControlImage.CurrentPage * scrollImgView.Frame.Width;
					this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
				}
			}
		}

		private void HandleTap(UITapGestureRecognizer tap)
		{
			Console.WriteLine ("TAP");
			if (this.images.Count != 0) 
			{
				ImageSliderController imagesScreen = new ImageSliderController (this.images, this.pageControlImage.CurrentPage);
				this.NavigationController.PushViewController (imagesScreen, true);
			}
		}
	}
}

