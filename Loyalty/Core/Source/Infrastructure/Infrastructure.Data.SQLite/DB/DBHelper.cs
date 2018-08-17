
#if NET4
#define SILVERLIGHT
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Data.SQLite;
using Infrastructure.Data.SQLite.DB.DTO;
using SQLite;


namespace Infrastructure.Data.SQLite.DB
{
    public static class DBHelper
    {
        private static string dbName = "LSRetailLoyaltyDB.db3";
        private static SQLiteConnection mySQLiteDBConnection = null;

        public static SQLiteConnection DBConnection
        {
            get
            {
                if (mySQLiteDBConnection == null)
                    OpenDBConnection();
                return mySQLiteDBConnection;
            }
        }

        public static string DatabaseFilePath
        {
            get
            {
#if SILVERLIGHT || WP
                var path = dbName;
#else

                //JIJ __ANDROID__ shall always be defined by the Android toolchain, without a need for special flags in project.
#if __ANDROID__
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //Environment.SpecialFolder.Personal -> /data/data/AndroidApplication1.AndroidApplication1/files/LsRetailDB.db3
                //use adb.exe shell     or pull file to desktop machine
                //adb.exe pull /data/data/AndroidApplication1.AndroidApplication1/files/LsRetailDB.db3 c:\temp 
                 
#else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = System.IO.Path.Combine(documentsPath, "../Library/");
#endif

                var path = System.IO.Path.Combine(libraryPath, dbName);
#endif
                return path;
            }
        }

        public static void OpenDBConnection()
        {
  
            if (mySQLiteDBConnection == null)
            {
                mySQLiteDBConnection = new SQLiteConnection(DatabaseFilePath); //opens connection in construtor
                mySQLiteDBConnection.CreateTable<DeviceData>(); //creates the table if needed
                mySQLiteDBConnection.CreateTable<WebserviceData>(); //creates the table if needed
				mySQLiteDBConnection.CreateTable<OfferData>(); //creates the table if needed
                mySQLiteDBConnection.CreateTable<MemberContactData>(); //creates the table if needed
                mySQLiteDBConnection.CreateTable<CouponData>(); //creates the table if needed
                mySQLiteDBConnection.CreateTable<NotificationData>(); //creates the table if needed                              
                mySQLiteDBConnection.CreateTable<TransactionData>(); //creates the table if needed 
            }
        }

        public static void CloseDBConnection()
        {
            if (mySQLiteDBConnection != null)
            {
                mySQLiteDBConnection.Dispose();
                mySQLiteDBConnection = null;
            }
        }

    }
}

