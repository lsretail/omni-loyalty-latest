using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace Infrastructure.Data.SQLite.DB.DTO
{
    public class OfferData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } //each DTO class used in our SQLite class must have this ID. Never used by us, only internally

        public string OfferId { get; set; }
        public int OfferType { get; set; }
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Details { get; set; }
        public bool HasDetails { get; set; }
        public string Images { get; set; }
    }
}
