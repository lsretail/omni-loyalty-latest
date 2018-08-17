using System;
using Presentation.Utils;
using System.Collections.Generic;
using Infrastructure.Data.SQLite2.Devices;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.Services.Loyalty.Notifications;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Offers;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;

namespace Presentation.Models
{
    public class NotificationModel : BaseModel
	{
		private INotificationRepository notificationRepository;
		private NotificationService notificationService;

		public NotificationModel ()
		{
			this.notificationRepository = new NotificationRepository ();
			this.notificationService = new NotificationService (this.notificationRepository);
		}

		public async Task<bool> UpdateStatus(string contactId, List<string> notificationIds, NotificationStatus status)
		{
            try
            {
                bool success = await this.notificationService.UpdateStatusAsync(contactId, notificationIds, status);

                if (success)
                {
                    if (status == NotificationStatus.Closed)
                    {
                        //remove from local list
                        foreach (string notificationId in notificationIds)
                        {
                            AppData.Device.UserLoggedOnToDevice.Notifications.RemoveAll(notification => notification.Id == notificationId);
                        }
                    }
                    else
                    {
                        //update the status
                        foreach (string notificationId in notificationIds)
                        {
                            for (int i = 0; i < AppData.Device.UserLoggedOnToDevice.Notifications.Count; i++)
                            {
                                if (notificationId == AppData.Device.UserLoggedOnToDevice.Notifications[i].Id)
                                {
                                    AppData.Device.UserLoggedOnToDevice.Notifications[i].Status = status;
                                }
                            }
                        }
                    }

                    new DeviceService(new DeviceRepository()).SaveDevice(AppData.Device);
                    return true;
                }
                else
                {
                    return false;
                }
            }
				
			catch (Exception ex)
			{
				HandleException (ex, "NotificationModel.UpdateStatus()", true);
                return false;
			}
			
		}

		public async Task<bool> GetNotifications(string contactId, bool showErrorMessage)
		{
            try
            {
                List<Notification> listOfNotifications = await this.notificationService.GetNotificationsAsync(contactId, Int32.MaxValue);

                System.Diagnostics.Debug.WriteLine("Notifications successfully updated for contact: " + contactId);
                AppData.Device.UserLoggedOnToDevice.Notifications = listOfNotifications;
                return true;
                
            }
			catch (Exception ex)
			{
				HandleException (ex, "NotificationModel.UpdateStatus()", showErrorMessage);
                return false;
			}
			
		}
	}
}

