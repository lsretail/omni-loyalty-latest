using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif
using System.Data;
using Infrastructure.Data.SQLite2.DTO;

namespace Infrastructure.Data.SQLite2.Webservice
{
	public class WebserviceRepository
	{
		private static readonly string sqlGetWebserviceData = "Select * FROM WEBSERVICEDATA LIMIT 1";
		private static readonly string sqlInsertWebserviceData = @"INSERT OR REPLACE INTO WEBSERVICEDATA VALUES (@BASEURL, @RESOURCE)";
		private static readonly string sqlDeleteWebserviceData = @"DELETE FROM WEBSERVICEDATA";

		public WebserviceData GetWebserviceData()
		{
			WebserviceData wsData = new WebserviceData();

			try
			{
				var conn = SqliteHelper.DatabaseConnection();

				SqliteCommand cmd = new SqliteCommand(sqlGetWebserviceData, conn);

				SqliteDataReader reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					wsData.BaseUrl = reader["BASEURL"].ToString();
					//wsData.Resource = reader["RESOURCE"].ToString();
				}

				reader.Close();

				cmd.Dispose();

				SqliteHelper.CloseDBConnection(conn);

				return wsData;
			}
			catch (Exception x)
			{
				throw new SQLException(SQLStatusCode.GetWebserviceDataError, x);
			}
		}

		public void SaveWebserviceData(WebserviceData wsData)
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
							cmd.CommandText = sqlDeleteWebserviceData;
							cmd.ExecuteNonQuery ();
						}

						using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand()) 
						{
							cmd.CommandText = sqlInsertWebserviceData;

							SqliteParameter BASEURL = new SqliteParameter ("@BASEURL", System.Data.DbType.String);
							SqliteParameter RESOURCE = new SqliteParameter ("@RESOURCE", System.Data.DbType.String);

							cmd.Parameters.Add(BASEURL);
							cmd.Parameters.Add(RESOURCE);

							BASEURL.Value = wsData.BaseUrl;
							//RESOURCE.Value = wsData.Resource;

							cmd.ExecuteNonQuery ();
						}

						dbTrans.Commit ();
					}

					conn.Close ();
				}
			}
			catch (Exception ex)
			{
				throw new SQLException(SQLStatusCode.SaveWebserviceDataError, ex);
			}
		}
	}
}