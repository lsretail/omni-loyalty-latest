using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace Infrastructure.Data.SQLite.DB.DTO
{
    public class MemberContactData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } //each DTO class used in our SQLite class must have this ID. Never used by us, only internally

        public string ContactId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Initials { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string MemberAccountId { get; set; }
        public long PointBalance { get; set; }
        public string MemberSchemeDescription { get; set; }
        public string MemberSchemePerks { get; set; }
        public long MemberSchemePointsNeeded { get; set; }
        public string NextMemberSchemeDescription { get; set; }
        public string NextMemberSchemePerks { get; set; }
        public long NextMemberSchemePointsNeeded { get; set; }
        public string Version { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyPrefix { get; set; }
        public string CurrencyPostfix { get; set; }
        public string CurrencySymbol { get; set; }
        public int CurrencyDecimalPlaces { get; set; }
        public string CurrencyDecimalSeparator { get; set; }
        public string CurrencyThousandSeparator { get; set; }
    }
}
