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
	internal class SqliteHelper
	{
		private const string dbName = "LSRetailLoyaltyDB.db3";
		private static string connectionString = string.Empty;
		private static string pathToDatabase = string.Empty;

		static SqliteHelper()
		{
			//for desktop the database file is simply in the \bin folder
			pathToDatabase = DatabaseFilePath;
			//connectionString = String.Format(@"Version=3,uri=file:{0}", DatabaseFilePath);
			//connectionString = String.Format(@"Version=3; Data Source={0}; Pooling=True; Max Pool Size=100;Cache Size=2000; Journal Mode=Off", DatabaseFilePath);
			#if USE_CSHARP_SQLITE
			connectionString = String.Format(@"Version=3,uri=file:{0}", DatabaseFilePath);
			#else
			connectionString = String.Format(@"Version=3; Data Source={0}; Cache Size=1000; Journal Mode=Off", DatabaseFilePath);
			#endif
		}

		public static string DatabaseFilePath
		{
			//returns the correct location of database file based on platform
			get
			{
				#if USE_CSHARP_SQLITE
				//C:\Users\jij\Documents\LSRetailMobilePos.db3 
				var dbPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				var path = Path.Combine(dbPath, dbName);

				#elif __ANDROID__
				//JIJ __ANDROID__ shall always be defined by the Android toolchain, without a need for special flags in project.
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				var path = System.IO.Path.Combine(libraryPath, dbName);
				//Environment.SpecialFolder.Personal -> /data/data/AndroidApplication1.AndroidApplication1/files/LSRetailMobilePos.db3
				//use adb.exe shell     or pull file to desktop machine
				//adb.exe pull /data/data/AndroidApplication1.AndroidApplication1/files/LSRetailMobilePos.db3 c:\temp 
				#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = System.IO.Path.Combine(documentsPath, "../Library/");

				var path = System.IO.Path.Combine(libraryPath, dbName);
				#endif
				return path;
			}
		}

		public static string PathToDatabase()
		{
			return pathToDatabase;
		}

		public static string DatabaseConnectionString()
		{
			return connectionString;
		}

		public static SqliteConnection DatabaseConnection()
		{
			var connection = new SqliteConnection (connectionString);
			connection.Open ();
			return connection;
		}

		public static void CloseDBConnection(SqliteConnection connection)
		{
			if (connection != null)
			{
				connection.Close();
				connection.Dispose();
				connection = null;
			}
		}

		/// <summary>
		/// Checks whether the database is already created on the device
		/// </summary>
		/// <returns>True if the database exists on the device. Otherwise returns false</returns>
		internal static bool DatabaseExists()
		{
			return File.Exists(pathToDatabase);
		}

		internal static bool ToBool(object value)
		{
			try
			{
				if (value.GetType() == typeof(Boolean))
				{
					return Convert.ToBoolean(value);
				}
				if (value == null || value == DBNull.Value)
					return false;
				if (value.ToString() == "1" || value.ToString() == "true")
					return true;
				else
					return false;
			}
			catch
			{
				return false;
			}
		}
	}
}