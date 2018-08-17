using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace Infrastructure.Data.SQLite.DB.DTO
{
    public class WebserviceData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } //each DTO class used in our SQLite class must have this ID. Never used by us, only internally

        public string BaseUrl { get; set; } 
        public string Resource { get; set; }
    }
}
