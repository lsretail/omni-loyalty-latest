using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Newtonsoft.Json;

namespace Infrastructure.Data.SQLite2.Baskets
{
    public class BasketRepository : ILocalBasketRepository
    {
        private static readonly string sqlGetBasket = "Select * FROM BASKET";
        private static readonly string sqlInsertBasket = @"INSERT OR REPLACE INTO BASKET VALUES (@ID, @BASKETXML)";
        private static readonly string sqlDeleteBasket = @"DELETE FROM BASKET";

        public Basket GetBasket()
        {
            Basket basket = null;

			try
			{
                var conn = SqliteHelper.DatabaseConnection();

                SqliteCommand cmd = new SqliteCommand(sqlGetBasket, conn);

				SqliteDataReader reader = cmd.ExecuteReader();

				if (reader.Read())
				{
				    basket = JsonConvert.DeserializeObject<Basket>(reader["BASKETXML"].ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
				}

				reader.Close();

				cmd.Dispose();

				SqliteHelper.CloseDBConnection(conn);

			    return basket;
			}
			catch (Exception x)
			{
				throw new SQLException(SQLStatusCode.GetBasketError, x);
			}
        }

        public void SaveBasket(Basket basket)
        {
            try
            {
                using (var conn = new SqliteConnection(SqliteHelper.DatabaseConnectionString()))
                {
                    conn.Open();
                    
                    string json = JsonConvert.SerializeObject(basket, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                    using (SqliteTransaction dbTrans = (SqliteTransaction)conn.BeginTransaction())
                    {
                        using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = sqlDeleteBasket;
                            cmd.ExecuteNonQuery();
                        }

                        using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = sqlInsertBasket;

                            SqliteParameter ID = new SqliteParameter("@ID", System.Data.DbType.String);
                            SqliteParameter BASKETXML = new SqliteParameter("@BASKETXML", System.Data.DbType.String);

                            cmd.Parameters.Add(ID);
                            cmd.Parameters.Add(BASKETXML);

                            var basketXml = json;

                            ID.Value = basket.Id;
                            BASKETXML.Value = basketXml;

                            cmd.ExecuteNonQuery();
                        }

                        dbTrans.Commit();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
				throw new SQLException(SQLStatusCode.SaveBasketError, ex);
            }
        }
    }
}
