using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace Infrastructure.Data.SQLite.DB.DTO
{
    public class NotificationData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } //each DTO class used in our SQLite class must have this ID. Never used by us, only internally

        public string NotificationId { get; set; }
        public string ContactId { get; set; }
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Status { get; set; }
        public string Images { get; set; }
    }
}
