using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace Infrastructure.Data.SQLite.DB.DTO
{
    public class DeviceData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } //each DTO class used in our SQLite class must have this ID. Never used by us, only internally

        public string DeviceId { get; set; } //we had a API to call into device (WP7) to get this unique id.
        public string Make { get; set; }
        public string Model { get; set; }
        public bool UserLoggedOnToDevice { get; set; }
        public string UserName { get; set; }
        public string ContactId { get; set; }
        public string CardId { get; set; }
        public string SecurityToken { get; set; }

    }

}
