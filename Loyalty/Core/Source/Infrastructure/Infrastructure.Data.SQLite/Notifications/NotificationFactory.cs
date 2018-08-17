using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Infrastructure.Data.SQLite.Notifications
{
    public class NotificationFactory
    {
        public Notification BuildEntity(NotificationData notificationData)
        {
            var entity = new Notification(notificationData.NotificationId)
            {
                ContactId = notificationData.ContactId,
                ExpiryDate = notificationData.ExpiryDate,
                Description = notificationData.PrimaryText,
                Details = notificationData.SecondaryText,
                Status = (NotificationStatus)notificationData.Status,
            };

            XmlSerializer serializer = new XmlSerializer(typeof(List<ImageView>), new Type[] { });

            using (TextReader textReader = new StringReader(notificationData.Images))
            {
                Console.WriteLine(textReader.ToString());

                entity.Images = (List<ImageView>)serializer.Deserialize(textReader);
            }

            return entity;
        }

        public NotificationData BuildEntity(Notification notification)
        {
            var entity = new NotificationData()
            {
                NotificationId = notification.Id,
                ContactId = notification.ContactId,
                ExpiryDate = notification.ExpiryDate,
                PrimaryText = notification.Description,
                SecondaryText = notification.Details,
                Status = (int)notification.Status
            };

            XmlSerializer serializer = new XmlSerializer(typeof(List<ImageView>), new Type[] { });

            using (var textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, notification.Images);
                entity.Images = textWriter.ToString();
            }

            return entity;
        }
    }
}
