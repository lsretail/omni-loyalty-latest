using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Notifications
{
    public interface INotificationLocalRepository
    {
        List<Notification> GetLocalNotifications();
        void SaveNotifications(List<Notification> notifications);
    }
}
