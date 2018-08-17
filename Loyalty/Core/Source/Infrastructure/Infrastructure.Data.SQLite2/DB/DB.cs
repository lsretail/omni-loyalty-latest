using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif
using Infrastructure.Data.SQLite2.Scripts;

namespace Infrastructure.Data.SQLite2
{
	public static class DB
	{
		public static string DatabaseFilePath { get { return SqliteHelper.DatabaseFilePath; } }

		public static void CreateTables()
		{
			/*if (SqliteHelper.DatabaseExists ()) 
			{
				ClearDB ();
			}*/

			using (var conn = new SqliteConnection(SqliteHelper.DatabaseConnectionString()))
			{
				conn.Open();

				string sqlText = Scripts.CreateTables.SqlStatements(); //"Create table Test_Table (COLA integer, COLB TEXT)";
				SqliteCommand cmd = new SqliteCommand(sqlText, conn);

				cmd.ExecuteNonQuery();
				cmd.Dispose();

				conn.Close();

				//cmd.CommandText = "INSERT INTO TEST_TABLE ( COLA, COLB ) VALUES (123,'ABC')";
				//cmd.ExecuteNonQuery();

				//cmd.CommandText = "SELECT COLA, COLB FROM TEST_TABLE";
				//SqliteDataReader reader = cmd.ExecuteReader();

				//Console.WriteLine("Read the data...");
				//while (reader.Read())
				//{

				//    int i = reader.GetInt32(reader.GetOrdinal("COLA"));
				//    Console.WriteLine("    COLA: {0}", i);

				//    string s =reader.GetString(reader.GetOrdinal("COLB"));

				//}

			}

		}

		private static void ClearDB()
		{
			using (var conn = new SqliteConnection(SqliteHelper.DatabaseConnectionString())) 
			{
				conn.Open ();

				string sqlText = Scripts.DropTables.SqlStatements ();
				SqliteCommand cmd = new SqliteCommand (sqlText, conn);

				cmd.ExecuteNonQuery ();
				cmd.Dispose ();

				conn.Close ();
			}
		}

		/*
		private static string GetStoreId()
		{
			string storeId = "";
			using (var conn = new SqliteConnection(SqliteHelper.DatabaseConnectionString()))
			{
				conn.Open();

				string sqlText = "select * from Terminal"; // storeId should always be the same for all terminals
				SqliteCommand cmd = new SqliteCommand(sqlText, conn);

				SqliteDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					storeId = reader.GetString(reader.GetOrdinal("STOREID"));
					break;
				}
				reader.Close ();
				cmd.Dispose();
				conn.Close();
			}

			return storeId;
		}*/

	}
}
