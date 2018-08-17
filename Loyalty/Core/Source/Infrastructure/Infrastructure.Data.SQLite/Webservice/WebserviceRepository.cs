using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;

namespace Infrastructure.Data.SQLite.Webservice
{
    public class WebserviceRepository
    {
        private object locker = new object();

        public WebserviceRepository()
        {
            DBHelper.OpenDBConnection();
        }

        public WebserviceData GetWebserviceData()
        {
            lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                return DBHelper.DBConnection.Table<WebserviceData>().FirstOrDefault();
            }
        }

        public void SaveWebserviceData(WebserviceData webserviceData)
        {
            lock (locker)
            {
                var deviceExist = DBHelper.DBConnection.Table<WebserviceData>().FirstOrDefault();
                if (deviceExist != null)
                {
                    webserviceData.ID = deviceExist.ID;
                    DBHelper.DBConnection.Update(webserviceData);
                }
                else
                {
                    DBHelper.DBConnection.Insert(webserviceData);
                }
            }
        }
    }
}
