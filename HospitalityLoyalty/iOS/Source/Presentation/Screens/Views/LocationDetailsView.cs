using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation
{
	public class LocationDetailsView : BaseView
	{
		private float headerImageHeight = 220f;
		private nfloat xMargin = 10f;
		private nfloat yMargin = 10f;
		private CGColor gradient = UIColor.FromRGBA(255, 255, 255, 0.6f).CGColor;
		private int ratioOfGradient = 5;


		private UIScrollView imgContainer;
		private UIPageControl pageControlImage;
		private UIScrollView contentContainer;
		private UIScrollView servicesView;
		private GradientView content;
		private UIView detailsView;
		private UILabel lblTitle;
		private UITextView txtAddress;
		private UITextView txtPhone;
		private UILabel lblOpeningHours;
		//private UIScrollView servicesView;
		private UITextView txtStoreHourTypeAndDays;
		private UITextView txtOpeningHours;

		private List<UIImageView> imageViews;
		private Store store;

		private ILocationDetailsListeners listeners;

		#region interface
		public interface ILocationDetailsListeners
		{
			void HandleTap(UITapGestureRecognizer tap, nint displayImageIndex);
		}
		#endregion

		public LocationDetailsView(Store store, ILocationDetailsListeners listeners)
		{
			this.store = store;
			this.listeners = listeners;

			#region Image
			this.BackgroundColor = UIColor.White;
			this.imageViews = new List<UIImageView>();

			UITapGestureRecognizer tap = new UITapGestureRecognizer();
			tap.AddTarget(() =>
			{
				this.listeners.HandleTap(tap, this.pageControlImage.CurrentPage);
			});

			this.imgContainer = new UIScrollView();
			this.imgContainer.ShowsVerticalScrollIndicator = false;
			this.imgContainer.ShowsHorizontalScrollIndicator = false;
			this.imgContainer.PagingEnabled = true;
			this.imgContainer.AlwaysBounceVertical = false;
			this.imgContainer.AddGestureRecognizer(tap);
			this.imgContainer.Scrolled += OnSwipe;

			#endregion

			#region Details

			this.detailsView = new UIView();
			this.detailsView.BackgroundColor = UIColor.Clear;

			// Title
			this.lblTitle = new UILabel();
			this.lblTitle.UserInteractionEnabled = false;
			this.lblTitle.TextColor = AppColors.PrimaryColor;
			this.lblTitle.Font = UIFont.BoldSystemFontOfSize(17);
			this.lblTitle.BackgroundColor = UIColor.Clear;
			this.lblTitle.TextAlignment = UITextAlignment.Left;
			this.lblTitle.Text = store.Description;
			this.detailsView.AddSubview(this.lblTitle);

			// Address text
			this.txtAddress = new UITextView();
			this.txtAddress.Editable = false;
			this.txtAddress.ScrollEnabled = false;
			this.txtAddress.Font = UIFont.SystemFontOfSize(15f);
			this.txtAddress.BackgroundColor = UIColor.Clear;
			this.txtAddress.Text = store.Address.FormatAddress;
			this.detailsView.AddSubview(txtAddress);

			// Phone text
			this.txtPhone = new UITextView();
			this.txtPhone.Editable = false;
			this.txtPhone.ScrollEnabled = false;
			this.txtPhone.Text = LocalizationUtilities.LocalizedString("Location_Details_Phone", "Phone") + ": " + this.store.Phone;
			this.txtPhone.Font = UIFont.SystemFontOfSize(14f);
			this.txtPhone.BackgroundColor = UIColor.Clear;
			this.txtPhone.Selectable = true;
			this.txtPhone.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
			this.detailsView.AddSubview(txtPhone);

			// Store services
			this.servicesView = new UIScrollView();
			this.detailsView.AddSubview(servicesView);

			// Opening hours label
			this.lblOpeningHours = new UILabel();
			this.lblOpeningHours.UserInteractionEnabled = false;
			this.lblOpeningHours.Text = LocalizationUtilities.LocalizedString("Location_Details_OpeningHours", "Opening hours");
			this.lblOpeningHours.TextColor = AppColors.PrimaryColor;
			this.lblOpeningHours.Font = UIFont.BoldSystemFontOfSize(15f);
			this.lblOpeningHours.BackgroundColor = UIColor.Clear;
			this.lblOpeningHours.TextAlignment = UITextAlignment.Left;
			if (this.store.StoreHours.Count == 0)
				this.lblOpeningHours.Hidden = true;
			this.detailsView.AddSubview(lblOpeningHours);

			#endregion

			#region StoreHours textviews
			// We use two textviews side by side to get a column-look for the opening hours. One column containing the days, the other the opening hours.

			// StoreHourType and days
			this.txtStoreHourTypeAndDays = new UITextView();
			this.txtStoreHourTypeAndDays.Editable = false;
			this.txtStoreHourTypeAndDays.ScrollEnabled = false;
			this.txtStoreHourTypeAndDays.Font = UIFont.SystemFontOfSize(14f);
			this.txtStoreHourTypeAndDays.BackgroundColor = UIColor.Clear;

			// Opening hours
			this.txtOpeningHours = new UITextView();
			this.txtOpeningHours.Editable = false;
			this.txtOpeningHours.ScrollEnabled = false;
			this.txtOpeningHours.Font = UIFont.SystemFontOfSize(14f);
			this.txtOpeningHours.BackgroundColor = UIColor.Clear;

			SetStoreHoursText();

			this.detailsView.AddSubview(txtStoreHourTypeAndDays);
			this.detailsView.AddSubview(txtOpeningHours);

			this.content = new GradientView(UIColor.White.CGColor, gradient, ratioOfGradient);
			this.content.AddSubview(this.detailsView);
			this.contentContainer = new UIScrollView();
			this.contentContainer.AddSubview(content);

			#endregion

			this.pageControlImage = new UIPageControl();
			this.pageControlImage.HidesForSinglePage = true;
			//this.pageControlImage.Pages = this.item.Images.Count;
			this.pageControlImage.UserInteractionEnabled = false;

			AddSubviews(imgContainer, pageControlImage, contentContainer);
		}

		#region Overwritten Functions

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			nfloat detailsViewContentHeight = 0f;  // Determine height of details and scrollview dynamically to fit all the content they need to display

			this.imgContainer.Frame = new CGRect(0, 0, this.Frame.Width, this.headerImageHeight);
			// The contentSize height is 1f to disable vertical scroll
			this.imgContainer.ContentSize = new CGSize(this.Frame.Width * this.store.Images.Count, 1f);
			this.contentContainer.Frame = new CGRect(0, 0, this.Frame.Width, this.Frame.Height);
			this.contentContainer.ContentSize = new CGSize(this.Frame.Width, this.Frame.Height + headerImageHeight / 2);
			this.content.Frame = new CGRect(this.Frame.X, this.imgContainer.Frame.Bottom, this.Frame.Width, this.Frame.Height - headerImageHeight / 2);
			this.detailsView.Frame = new CGRect(0f, 0f, this.content.Frame.Width, this.content.Frame.Height);
			this.lblTitle.Frame = new CGRect(xMargin, yMargin, detailsView.Frame.Width - 2 * xMargin, 20f);
			this.txtAddress.Frame = new CGRect(xMargin, this.lblTitle.Frame.Bottom, detailsView.Frame.Width - 2 * xMargin, 1f); // Set arbitrary height
			TxtSizeFit(this.txtAddress);
			this.txtPhone.Frame = new CGRect(xMargin, txtAddress.Frame.Bottom, detailsView.Frame.Width - 2 * xMargin, 1f); // Set arbitrary height
			TxtSizeFit(this.txtPhone);
			this.servicesView.Frame = new CGRect(xMargin, txtPhone.Frame.Bottom + 5f, detailsView.Frame.Width - 2 * xMargin, 60f);

			if (!LoadStoreServicesImages(servicesView))
			{
				// No images added to servicesView, let's hide it
				this.servicesView.Hidden = true;
				this.servicesView.Frame = new CGRect(this.servicesView.Frame.X, this.servicesView.Frame.Y, this.servicesView.Frame.Width, 0f);
			}

			this.lblOpeningHours.Frame = new CGRect(xMargin, servicesView.Frame.Bottom + 10f, detailsView.Frame.Width - 2 * xMargin, 20f);
			this.txtStoreHourTypeAndDays.Frame = new CGRect(xMargin, lblOpeningHours.Frame.Bottom + 5f, (detailsView.Frame.Width - 2 * xMargin) / 2, 1f); // Set arbitrary height
			this.txtOpeningHours.Frame = new CGRect(txtStoreHourTypeAndDays.Frame.Right, lblOpeningHours.Frame.Bottom + 5f, (detailsView.Frame.Width - 2 * xMargin) / 2, 1f); // Set arbitrary height

			TxtSizeFit(this.txtStoreHourTypeAndDays);
			TxtSizeFit(this.txtOpeningHours);

			detailsViewContentHeight += yMargin;
			detailsViewContentHeight += this.lblTitle.Frame.Height;
			detailsViewContentHeight += this.servicesView.Frame.Height;
			detailsViewContentHeight += this.lblOpeningHours.Frame.Height;
			detailsViewContentHeight += (nfloat)Math.Max(this.txtStoreHourTypeAndDays.Frame.Height, this.txtOpeningHours.Frame.Height);    // Should be the same height though
			detailsViewContentHeight += this.imgContainer.Frame.Height;
			detailsViewContentHeight += Util.AppDelegate.RootTabBarController.TabBar.Frame.Height;

			if (detailsViewContentHeight > content.Frame.Height)
			{
				this.content.Frame = new CGRect(Frame.X, this.imgContainer.Frame.Bottom, Frame.Width, detailsViewContentHeight);
				this.contentContainer.ContentSize = new CGSize(Frame.Width, detailsViewContentHeight + headerImageHeight / 2);
			}

			Console.WriteLine("---- DetailsViewContentHeight: " + detailsViewContentHeight);

			this.pageControlImage.Frame = new CGRect(0, 60, this.Frame.Width, this.headerImageHeight);
		}

		public override UIView HitTest(CGPoint point, UIEvent uievent)
		{
			var hitView = base.HitTest(point, uievent);
			CGPoint pointInImg = imgContainer.ConvertPointFromView(point, this);
			CGPoint scrollPos = contentContainer.ContentOffset;
			if (scrollPos.Y > 0)
				pointInImg.Y = pointInImg.Y + scrollPos.Y;
			if (imgContainer.PointInside(pointInImg, uievent))
			{
				return imgContainer;
			}

			return hitView;
		}

		#endregion

		#region Private Functions

		private void TxtSizeFit(UITextView textView)
		{
			CGSize newSizeThatFits = textView.SizeThatFits(txtStoreHourTypeAndDays.Frame.Size);
			CGRect tempFrame = textView.Frame;
			tempFrame.Size = new CGSize(tempFrame.Size.Width, newSizeThatFits.Height);  // Only adjust the height
			textView.Frame = tempFrame;
		}

		private void OnSwipe(object sender, EventArgs e)
		{
			Console.WriteLine("Running onSwipe ------------------------");
			if (imgContainer.Frame.Width > 0)
			{
				int page = Convert.ToInt32(Math.Floor(imgContainer.ContentOffset.X / imgContainer.Frame.Width));
				if (page > -1 && page < imageViews.Count)
					pageControlImage.CurrentPage = page;
			}
		}

		private void SetupSlide(int index)
		{
			nfloat y = 0,
			x = this.Frame.Width * index,
			width = this.imgContainer.Frame.Width,
			height = this.imgContainer.Frame.Height;

			UIImageView img = this.imageViews[index];
			img.Frame = new CGRect(x, y, this.imgContainer.Frame.Width, this.imgContainer.Frame.Height);
			//img.ContentMode = UIViewContentMode.ScaleAspectFill;

			imgContainer.AddSubview(img);
		}

		private void SetStoreHoursText()
		{
			// Must make the two texts in the textboxes align, i.e. show the correct opening hours for monday in the same line as "Monday"

			string storeHourTypeAndDaysString = string.Empty;   // Goes into the left textbox, type and days
			string openingHoursString = string.Empty;           // Goes into the right textbox, actual opening hours

			List<StoreHourType> storeHourTypes = this.store.StoreHours.Select(x => x.StoreHourtype).Distinct().ToList();
			List<StoreHours> storeHoursFilteredByType = new List<StoreHours>();

			bool newLineRequired = false;
			foreach (var storeHourType in storeHourTypes)
			{
				storeHourTypeAndDaysString += (newLineRequired ? "\r\n" : string.Empty) + GetStoreHourTypeName(storeHourType) + ":\r\n\r\n";
				openingHoursString += (newLineRequired ? "\r\n" : string.Empty) + "\r\n\r\n";
				newLineRequired = true;

				storeHoursFilteredByType = this.store.StoreHours.Where(x => x.StoreHourtype == storeHourType).ToList();

				foreach (var sHour in storeHoursFilteredByType)
				{
					storeHourTypeAndDaysString += GetStoreHoursDayName(sHour) + "\r\n";
					openingHoursString += sHour.OpenFrom.ToShortTimeString() + " - " + sHour.OpenTo.ToShortTimeString() + "\r\n";
				}
			}

			this.txtStoreHourTypeAndDays.Text = storeHourTypeAndDaysString;
			this.txtOpeningHours.Text = openingHoursString;
		}

		private string GetStoreHourTypeName(StoreHourType storeHourType)
		{
			switch (storeHourType)
			{
				case StoreHourType.DriveThruWindow:
					return LocalizationUtilities.LocalizedString("Location_OpeningHoursType_DriveThru", "Drive thru");
				case StoreHourType.MainStore:
					return LocalizationUtilities.LocalizedString("Location_OpeningHoursType_MainStore", "Main store");
				default:
					return string.Empty;
			}
		}

		private string GetStoreHoursDayName(StoreHours storeHours)
		{
			switch ((int)storeHours.DayOfWeek)
			{
				case 0:
					return LocalizationUtilities.LocalizedString("Weekday_Sunday", "Sunday");
				case 1:
					return LocalizationUtilities.LocalizedString("Weekday_Monday", "Monday");
				case 2:
					return LocalizationUtilities.LocalizedString("Weekday_Tuesday", "Tuesday");
				case 3:
					return LocalizationUtilities.LocalizedString("Weekday_Wednesday", "Wednesday");
				case 4:
					return LocalizationUtilities.LocalizedString("Weekday_Thursday", "Thursday");
				case 5:
					return LocalizationUtilities.LocalizedString("Weekday_Friday", "Friday");
				case 6:
					return LocalizationUtilities.LocalizedString("Weekday_Saturday", "Saturday");
				default:
					return string.Empty;
			}
		}

		private UIImage GetStoreServiceImage(StoreServiceType serviceType)
		{
			switch (serviceType)
			{
				case StoreServiceType.DriveThruWindow:
					return Utils.UI.GetColoredImage(Image.FromFile("/Other/store_service_drive_through.png"), AppColors.PrimaryColor);
				case StoreServiceType.FreeRefill:
					return Utils.UI.GetColoredImage(Image.FromFile("/Other/store_service_free_refill.png"), AppColors.PrimaryColor);
				case StoreServiceType.FreeWiFi:
					return Utils.UI.GetColoredImage(Image.FromFile("/Other/store_service_free_wifi.png"), AppColors.PrimaryColor);
				case StoreServiceType.Garden:
					return Utils.UI.GetColoredImage(Image.FromFile("/Other/store_service_garden.png"), AppColors.PrimaryColor);
				case StoreServiceType.GiftCard:
					return Utils.UI.GetColoredImage(Image.FromFile("/Other/store_service_gift_card.png"), AppColors.PrimaryColor);
				case StoreServiceType.PlayPlace:
					return Utils.UI.GetColoredImage(Image.FromFile("/Other/store_service_play_place.png"), AppColors.PrimaryColor);
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
					return LocalizationUtilities.LocalizedString("StoreService_DriveThrough", "Drive through");
				case StoreServiceType.FreeRefill:
					return LocalizationUtilities.LocalizedString("StoreService_FreeRefill", "Free refill");
				case StoreServiceType.FreeWiFi:
					return LocalizationUtilities.LocalizedString("StoreService_FreeWiFi", "WiFi");
				case StoreServiceType.Garden:
					return LocalizationUtilities.LocalizedString("StoreService_Garden", "Garden");
				case StoreServiceType.GiftCard:
					return LocalizationUtilities.LocalizedString("StoreService_GiftCard", "Gift card");
				case StoreServiceType.PlayPlace:
					return LocalizationUtilities.LocalizedString("StoreService_PlayPlace", "Play place");
				case StoreServiceType.None:
					return string.Empty;
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
					serviceImageView.TintColor = AppColors.PrimaryColor;
					serviceImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					serviceImageView.Frame = new CGRect(xCoord, 0, scrollViewServices.Frame.Height - 20f, scrollViewServices.Frame.Height - 20f);
					scrollViewServices.AddSubview(serviceImageView);
					imageAdded = true;

					UILabel lblStoreServiceTitle = new UILabel();
					lblStoreServiceTitle.Frame = new CGRect(xCoord - 10, serviceImageView.Frame.Bottom, scrollViewServices.Frame.Height, 20f);
					lblStoreServiceTitle.Text = GetStoreServiceTitle(service.StoreServiceType);
					lblStoreServiceTitle.Font = UIFont.BoldSystemFontOfSize(10);
					lblStoreServiceTitle.TextColor = AppColors.PrimaryColor;
					lblStoreServiceTitle.TextAlignment = UITextAlignment.Center;
					scrollViewServices.AddSubview(lblStoreServiceTitle);

					xCoord = serviceImageView.Frame.Right + 18f;
				}
			}

			return imageAdded;
		}

		#endregion

		#region Public Functions

		public void LoadImage(UIImage image)
		{
			UIImageView imageView = new UIImageView();

			imageView.Image = image;

			CATransition transition = new CATransition();
			transition.Duration = 0.5f;
			transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			transition.Type = CATransition.TransitionFade;
			imageView.Layer.AddAnimation(transition, null);

			this.imageViews.Add(imageView);
			SetupSlide(this.imageViews.Count - 1);
		}

		#endregion
	}
}

