using System.Collections.Generic;
using System.Linq;

using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Notifications;

namespace Infrastructure.Data.SQLite.Notifications
{
    public class NotificationRepository : INotificationLocalRepository
    {
        private object locker = new object();

        public NotificationRepository()
        {
            DBHelper.OpenDBConnection();
        }

        public List<Notification> GetLocalNotifications()
        {
            lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                var factory = new NotificationFactory();
                var notifications = new List<Notification>();

                foreach (var notification in DBHelper.DBConnection.Table<NotificationData>().ToList())
                {
                    notifications.Add(factory.BuildEntity(notification));
                }

                return notifications;
            }
        }

        public void SaveNotifications(List<Notification> notifications)
        {
            lock (locker)
            {
                var facotry = new NotificationFactory();

                var notificationsData = new List<NotificationData>();
                notifications.ForEach(x => notificationsData.Add(facotry.BuildEntity(x)));

                DBHelper.DBConnection.DeleteAll<NotificationData>();
                DBHelper.DBConnection.InsertAll(notificationsData);
            }
        }
    }
}
