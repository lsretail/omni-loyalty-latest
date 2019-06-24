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
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime Date { get; set; }
    }
}
