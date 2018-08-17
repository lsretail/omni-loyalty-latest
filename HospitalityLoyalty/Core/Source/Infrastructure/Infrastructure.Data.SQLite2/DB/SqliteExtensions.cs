using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif
using System.IO;
using System.Data;
 
namespace Infrastructure.Data.SQLite2
{
    internal static class SqlDataReaderExtensions
    {
        //handle all DBNull issues to avoid exceptions when reading values 
        public static int SafeGetInt32(this SqliteDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
                return reader.GetInt32(ordinal);
            else
                return 0;
        }
        public static string SafeGetString(this SqliteDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
                return reader.GetString(ordinal);
            else
                return string.Empty;
        }
        public static Decimal SafeDecimal(this SqliteDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
                return reader.GetDecimal(ordinal);
            else
                return 0.0M;
        }
        public static Double SafeDouble(this SqliteDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
                return reader.GetDouble(ordinal);
            else
                return 0.0;
        }
        public static DateTime SafeGetDateTime(this SqliteDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
                return reader.GetDateTime(ordinal);
            else
                return DateTime.MinValue;
        }
        //public static DateTime? SafeGetDateTime(this SqliteDataReader reader, string columnName)
        //{
        //    int ordinal = reader.GetOrdinal(columnName);
        //    if (!reader.IsDBNull(ordinal))
        //        return reader.GetDateTime(ordinal);
        //    else
        //        return null;
        //}

    }
}
