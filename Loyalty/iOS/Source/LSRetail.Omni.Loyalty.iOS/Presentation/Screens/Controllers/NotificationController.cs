using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation
{
    // TODO Shouldn't display a spinner overlay when refreshing, that blocks all input for way too long if the update fails, should use network activity indicator instead
    public class NotificationsController : UIViewController
	{
		private NotificationsView rootView;

		public NotificationsController ()
		{
			this.Title = LocalizationUtilities.LocalizedString("Notifications_Notifications", "Notifications");

			rootView = new NotificationsView ();
			rootView.DeleteNotification += DeleteNotification;
			rootView.RefreshNotifications += UpdateNotifications;
			rootView.NotificationSelected += NotificationSelected;
		}

		#region Overwritten functions

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			this.View = rootView;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}
			
		public override void ViewWillAppear (bool animated)
		{
			if (AppData.UserLoggedIn)
			{				
				Utils.UI.ShowNetworkActivityIndicator();
				UpdateNotifications(
					()=>
					{ 
						this.rootView.RefreshData();			
						Utils.UI.HideNetworkActivityIndicator();

						// Let's reset the badge
						// We are now showing all notifications, so the notification counter on the badge should be reset
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					}, 
					()=>
					{
						this.rootView.RefreshData();
						Utils.UI.HideNetworkActivityIndicator();
					}
				);
			}
			else
			{
				this.rootView.RefreshData();
			}

			base.ViewWillAppear (animated);
		}
			
		#endregion

		#region Listeners

		public async void UpdateNotifications(Action onSuccess, Action onFailure)
		{
			if (AppData.UserLoggedIn)
			{
                bool success = await new Models.NotificationModel().GetNotifications(AppData.Device.UserLoggedOnToDevice.Id, true);
                if(success)
                {
                    onSuccess();
                }
                else
                {
                    onFailure();
                }
			}
			else
			{
				onFailure();
			}
		}

		public void NotificationSelected(Notification notification)
		{
			NotificationDetailsController detailsController = new NotificationDetailsController (notification);
			this.NavigationController.PushViewController (detailsController, true);
		}

		private async void DeleteNotification(string notificationId)
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
				List<string> allNotificationIds = new List<string>();
				allNotificationIds.Add(notificationId);

                bool success = await new Models.NotificationModel().UpdateStatus(
                    AppData.Device.UserLoggedOnToDevice.Id,
                    allNotificationIds,
                    NotificationStatus.Closed);

                if (success) 
					{
						// Success

						Utils.UI.HideLoadingIndicator();
						this.rootView.RefreshData ();
					}
					else
					{
						// Failure

						Utils.UI.HideLoadingIndicator();
					}	
			}
		}

		#endregion
	}
}

