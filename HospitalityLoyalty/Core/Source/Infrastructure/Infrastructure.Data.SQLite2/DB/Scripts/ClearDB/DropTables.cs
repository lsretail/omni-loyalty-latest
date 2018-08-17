using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Data.SQLite2.Scripts
{
	public static class DropTables
    {
        public static string SqlStatements()
        {
            string sql = @"

        		DROP TABLE IF EXISTS WEBSERVICEDATA;
        		DROP TABLE IF EXISTS TRANS;
                DROP TABLE IF EXISTS FAVORITE;
                DROP TABLE IF EXISTS BASKET;
                DROP TABLE IF EXISTS CONTACT;
                DROP TABLE IF EXISTS MENU;
				";

            return sql;
        }
    }
}
