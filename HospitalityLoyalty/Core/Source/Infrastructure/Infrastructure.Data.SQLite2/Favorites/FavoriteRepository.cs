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
using LSRetail.Omni.Domain.DataModel.Base.Favorites;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Favorites;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using Newtonsoft.Json;

namespace Infrastructure.Data.SQLite2.Favorites
{
    public class FavoriteRepository : ILocalFavoriteRepository
    {
        private enum FavoriteType
        {
            None = 0,
            MenuItem = 1,
            Transaction = 2
        }

        private static readonly string sqlGetFavorites = "Select * FROM FAVORITE";
        private static readonly string sqlInsertFavorites = @"INSERT OR REPLACE INTO FAVORITE VALUES (@ID, @TYPE, @FAVORITEXML)";
        private static readonly string sqlDeleteFavorites = @"DELETE FROM FAVORITE";

        public List<IFavorite> GetFavorites()
        {
            var favorites = new List<IFavorite>();

            try
            {
                var conn = SqliteHelper.DatabaseConnection();

                SqliteCommand cmd = new SqliteCommand(sqlGetFavorites, conn);

                SqliteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var json = reader["FAVORITEXML"].ToString();

                    favorites.Add(JsonConvert.DeserializeObject<IFavorite>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
                }

                reader.Close();

                cmd.Dispose();

                SqliteHelper.CloseDBConnection(conn);

                return favorites;
            }
            catch (Exception x)
            {
				throw new SQLException(SQLStatusCode.GetFavoritesError, x);
            }
        }

        public void SaveFavorites(List<IFavorite> favorites)
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
                            cmd.CommandText = sqlDeleteFavorites;
                            cmd.ExecuteNonQuery();
                        }

                        using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = sqlInsertFavorites;

                            SqliteParameter ID = new SqliteParameter("@ID", System.Data.DbType.String);
                            SqliteParameter TYPE = new SqliteParameter("@TYPE", System.Data.DbType.Int16);
                            SqliteParameter FAVORITE = new SqliteParameter("@FAVORITEXML", System.Data.DbType.String);

                            cmd.Parameters.Add(ID);
                            cmd.Parameters.Add(TYPE);
                            cmd.Parameters.Add(FAVORITE);

                            foreach (var favorite in favorites)
                            {
                                var favoriteXml = JsonConvert.SerializeObject(favorite, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                                ID.Value = favorite.Id;

                                if (favorite is Transaction)
                                {
                                    TYPE.Value = (int) FavoriteType.Transaction;
                                }
                                else if (favorite is MenuItem)
                                {
                                    TYPE.Value = (int) FavoriteType.MenuItem;
                                }
                                else
                                {
                                    TYPE.Value = (int) FavoriteType.None;
                                }

                                FAVORITE.Value = favoriteXml;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        dbTrans.Commit();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
				throw new SQLException(SQLStatusCode.SaveFavoritesError, ex);
            }
        }
    }
}
