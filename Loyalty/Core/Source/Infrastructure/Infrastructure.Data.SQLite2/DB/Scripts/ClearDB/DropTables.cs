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
				DROP TABLE IF EXISTS DEVICEDATA;
				DROP TABLE IF EXISTS BASKET;
				";

			return sql;
		}
	}
}
