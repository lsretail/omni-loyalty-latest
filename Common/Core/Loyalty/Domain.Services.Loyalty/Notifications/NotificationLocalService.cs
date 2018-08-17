using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Notifications
{
    public class NotificationLocalService
    {
        private INotificationLocalRepository iRepository;

        public NotificationLocalService(INotificationLocalRepository iRepo)
        {
            iRepository = iRepo;
        }

        public List<Notification> GetLocalNotifications()
        {
            return iRepository.GetLocalNotifications();
        }

        public void SaveNotifications(List<Notification> notifications)
        {
            iRepository.SaveNotifications(notifications);
        }
    }
}
