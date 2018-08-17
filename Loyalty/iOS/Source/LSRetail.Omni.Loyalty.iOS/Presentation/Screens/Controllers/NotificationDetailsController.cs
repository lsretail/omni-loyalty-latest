using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class NotificationDetailsController : UIViewController
	{
		private NotificationDetailsView rootView;
		private Notification notification;

		public NotificationDetailsController (Notification notification)
		{			
			this.notification = notification;

			this.rootView = new NotificationDetailsView();		
			this.rootView.ImageSelected += ViewImages;
			this.rootView.UpdateView(notification);

			this.Title = LocalizationUtilities.LocalizedString("Notification_Notification", "Notification");
		}	

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.View = this.rootView;
			SetRightBarButtonItems();
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}	
			
		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			if (this.NavigationItem.RightBarButtonItems != null)
			{
				foreach (var currentRightBarButtonItem in this.NavigationItem.RightBarButtonItems)
					barButtonItemList.Add(currentRightBarButtonItem);
			}

			UIButton btnDelete = new UIButton (UIButtonType.Custom);
			btnDelete.SetImage (ImageUtilities.GetColoredImage(UIImage.FromBundle("TrashIcon"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
			btnDelete.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			btnDelete.Frame = new CGRect (0, 0, 30, 30);
			btnDelete.TouchUpInside += (sender, e) => 
			{
				DeleteNotification ();
			};
			barButtonItemList.Add(new UIBarButtonItem (btnDelete));

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private void ViewImages(List<ImageView> imageViews, nint selectedImageViewIndex)
		{												
			if (selectedImageViewIndex > imageViews.Count - 1)
				return;

			ImageView imageViewToShow = imageViews[(int)selectedImageViewIndex];

			if (imageViewToShow != null)
			{
				var imageZoomController = new ImageZoomController(imageViewToShow);
				this.NavigationController.PushViewController(imageZoomController, true);
			}
		}

		private async void DeleteNotification()
		{
			var alertResult = await AlertView.ShowAlert(
				this,
				LocalizationUtilities.LocalizedString("General_Confirmation", "Confirmation"),
				LocalizationUtilities.LocalizedString("Notification_AreYouSureRemoveNotification", "Are you sure you want to delete this notification?"),
				LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
				LocalizationUtilities.LocalizedString("General_No", "No")
			);

			if (alertResult == AlertView.AlertButtonResult.PositiveButton)
			{
				Utils.UI.ShowLoadingIndicator();
                bool success = await new Models.NotificationModel().UpdateStatus(
                        AppData.Device.UserLoggedOnToDevice.Id,
                        new List<string>() { this.notification.Id },
                        NotificationStatus.Closed);
                if (success)
				{
					// Success

					Utils.UI.HideLoadingIndicator();
					CloseScreen();
				}
                else
				{
					// Failure

					Utils.UI.HideLoadingIndicator();
				}
				
			}
		}

		private void CloseScreen()
		{
			if (Utils.Util.IsModalController(this))
				this.DismissViewController(true, null);
			else
				this.NavigationController.PopViewController(true);
		}
	}
}

