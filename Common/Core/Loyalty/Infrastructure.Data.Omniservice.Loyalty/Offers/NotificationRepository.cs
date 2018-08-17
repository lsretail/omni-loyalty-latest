using System;
using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Notifications;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Offers
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        public List<Notification> NotificationsGetByContactId(string contactId, int numberOfNotifications)
        {
            string methodName = "NotificationsGetByContactId";
            var jObject = new { contactId = contactId, numberOfNotifications = numberOfNotifications };
            return base.PostData<List<Notification>>(jObject, methodName);
        }

        public bool UpdateStatus(string contactId, List<string> notificationId, NotificationStatus notificationStatus)
        {
            string methodName = "NotificationsUpdateStatus";
            var jObject = new { contactId = contactId, notificationIds = notificationId, notificationStatus = notificationStatus };
            return base.PostData<bool>((object)jObject, methodName);
        }

        public Notification NotificationGetById(string notificationId)
        {
            string methodName = "NotificationGetById";
            var jObject = new { notificationId = notificationId };
            return base.PostData<Notification>(jObject, methodName);
        }

        public NotificationUnread NotificationCountGetUnread(string contactId, DateTime lastChecked)
        {
            string methodName = "NotificationCountGetUnread";
            var jObject = new { contactId = contactId, lastChecked = lastChecked };
            return base.PostData<NotificationUnread>(jObject, methodName); 
        }
    }
}
