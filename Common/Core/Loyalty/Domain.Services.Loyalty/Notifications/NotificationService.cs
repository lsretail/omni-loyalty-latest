using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Notifications
{
    public class NotificationService
    {
        private INotificationRepository iNotificationRepository;

        public NotificationService(INotificationRepository iRepo)
        {
            iNotificationRepository = iRepo;
        }

        public List<Notification> GetNotifications(string cardId, int numberOfNotifications = Int32.MaxValue)
        {
            return iNotificationRepository.NotificationsGetByCardId(cardId, numberOfNotifications);
        }

        public bool UpdateStatus(string contactId, List<string> notificationIds, NotificationStatus status)
        {
            return iNotificationRepository.UpdateStatus(contactId, notificationIds, status);
        }

        public NotificationUnread NotificationCountGetUnread(string contactId, DateTime lastChecked)
        {
            return iNotificationRepository.NotificationCountGetUnread(contactId, lastChecked);
        }

        public async Task<List<Notification>> GetNotificationsAsync(string cardId, int numberOfNotifications)
        {
            return await Task.Run(() => GetNotifications(cardId, numberOfNotifications));
        }

        public async Task<bool> UpdateStatusAsync(string contactId, List<string> notificationIds, NotificationStatus status)
        {
            return await Task.Run(() => UpdateStatus(contactId, notificationIds, status));
        }

        public async Task<NotificationUnread> NotificationCountGetUnreadAsync(string contactId, DateTime lastChecked)
        {
            return await Task.Run(() => NotificationCountGetUnread(contactId, lastChecked));
        }
    }
}
