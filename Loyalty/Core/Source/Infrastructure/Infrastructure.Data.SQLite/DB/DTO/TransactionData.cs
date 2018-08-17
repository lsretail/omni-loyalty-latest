using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace Infrastructure.Data.SQLite.DB.DTO
{
    public class TransactionData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } //each DTO class used in our SQLite class must have this ID. Never used by us, only internally

        public string TransactionId { get; set; }
        public string StoreId { get; set; }
        public string StoreDescription { get; set; }
        public string Terminal { get; set; }
        public string Staff { get; set; }
        public string Amount { get; set; }
        public string NetAmount { get; set; }
        public string VatAmount { get; set; }
        public string DiscountAmount { get; set; }
        public DateTime? Date { get; set; }
    }
}
