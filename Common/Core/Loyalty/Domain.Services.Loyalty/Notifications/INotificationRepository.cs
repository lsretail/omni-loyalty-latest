using System;
using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Notifications
{
    public interface INotificationRepository
    {
        // Returns a list of notifications valid for a specific member contact
        List<Notification> NotificationsGetByCardId(string contactId, int numberOfNotifications = 10000);
        bool UpdateStatus(string contactId, List<string> notificationIds, NotificationStatus status);
        Notification NotificationGetById(string notificationId);
        NotificationUnread NotificationCountGetUnread(string contactId, DateTime lastChecked);
    }
}
 