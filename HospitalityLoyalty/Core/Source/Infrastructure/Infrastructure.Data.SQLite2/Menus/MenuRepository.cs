using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif
using System.Xml.Serialization;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Newtonsoft.Json;

namespace Infrastructure.Data.SQLite2.Menus
{
    public class MenuRepository : ILocalMenuRepository
    {
        private static readonly string sqlGetMenu = "Select * FROM MENU";
        private static readonly string sqlInsertMenu = @"INSERT OR REPLACE INTO MENU VALUES (@ID, @MENUXML)";
        private static readonly string sqlDeleteMenu = @"DELETE FROM MENU";

        public MobileMenu GetMobileMenu()
        {
            MobileMenu mobileMenu = null;

            try
            {
                var conn = SqliteHelper.DatabaseConnection();

                SqliteCommand cmd = new SqliteCommand(sqlGetMenu, conn);

                SqliteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    mobileMenu = JsonConvert.DeserializeObject<MobileMenu>(reader["MENUXML"].ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                }

                reader.Close();

                cmd.Dispose();

                SqliteHelper.CloseDBConnection(conn);

                return mobileMenu;
            }
            catch (Exception x)
            {
				throw new SQLException(SQLStatusCode.GetMobileMenuError, x);
            }
        }

        public void SaveMobileMenu(MobileMenu mobileMenu)
        {
            try
            {
                using (var conn = new SqliteConnection(SqliteHelper.DatabaseConnectionString()))
                {
                    conn.Open();

                    using (SqliteTransaction dbTrans = (SqliteTransaction)conn.BeginTransaction())
                    {
                        using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = sqlDeleteMenu;
                            cmd.ExecuteNonQuery();
                        }

                        using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = sqlInsertMenu;

                            SqliteParameter ID = new SqliteParameter("@ID", System.Data.DbType.String);
                            SqliteParameter MENUXML = new SqliteParameter("@MENUXML", System.Data.DbType.String);

                            cmd.Parameters.Add(ID);
                            cmd.Parameters.Add(MENUXML);

                            var contactXml = JsonConvert.SerializeObject(mobileMenu, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                            ID.Value = mobileMenu.Id;
                            MENUXML.Value = contactXml;

                            cmd.ExecuteNonQuery();
                        }

                        dbTrans.Commit();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
				throw new SQLException(SQLStatusCode.SaveMobileMenuError, ex);
            }
        }
    }
}
