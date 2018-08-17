using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Data.SQLite2.Scripts
{
	public static class CreateTables
	{

		// SQLite has no boolean so tinyint is used to store boolean (0 == false)
		// Using  COLLATE NOCASE so text search is not case sensitive

		public static string SqlStatements()
		{
			string sql = @"

			CREATE TABLE IF NOT EXISTS WEBSERVICEDATA (
                    BASEURL       	nvarchar(300) NOT NULL COLLATE NOCASE,
                    RESOURCE        nvarchar(300) NOT NULL COLLATE NOCASE,
            CONSTRAINT PK_WEBSERVICEDATA PRIMARY KEY(BASEURL, RESOURCE));

			CREATE TABLE IF NOT EXISTS  DEVICEDATA (
                    ID   nvarchar (10) NOT NULL COLLATE NOCASE,
                    DEVICEDATAXML   nvarchar (10) NOT NULL COLLATE NOCASE,
            CONSTRAINT PK_DEVICEDATA PRIMARY KEY( ID));

			CREATE TABLE IF NOT EXISTS  BASKET (
                    ID   nvarchar (10) NOT NULL COLLATE NOCASE,
                    BASKETXML   nvarchar (10) NOT NULL COLLATE NOCASE,
            CONSTRAINT PK_BASKET PRIMARY KEY( ID));

            ";

			return sql;
		}
	}
}
