using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts;
using Newtonsoft.Json;
#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif

namespace Infrastructure.Data.SQLite2.MemberContacts
{
    public class ContactRepository : ILocalContactRepository
    {
        private static readonly string sqlGetContact = "Select * FROM CONTACT";
        private static readonly string sqlInsertContact = @"INSERT OR REPLACE INTO CONTACT VALUES (@ID, @CONTACTXML)";
        private static readonly string sqlDeleteContact = @"DELETE FROM CONTACT";

        public MemberContact GetContact()
        {
            MemberContact contact = null;

            try
            {
                var conn = SqliteHelper.DatabaseConnection();

                SqliteCommand cmd = new SqliteCommand(sqlGetContact, conn);

                SqliteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    contact = JsonConvert.DeserializeObject<MemberContact>(reader["CONTACTXML"].ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                }

                reader.Close();

                cmd.Dispose();

                SqliteHelper.CloseDBConnection(conn);

                return contact;
            }
            catch (Exception x)
            {
				throw new SQLException(SQLStatusCode.GetContactError, x);
            }
        }

        public void SaveContact(MemberContact contact)
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
                            cmd.CommandText = sqlDeleteContact;
                            cmd.ExecuteNonQuery();
                        }

                        using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = sqlInsertContact;

                            SqliteParameter ID = new SqliteParameter("@ID", System.Data.DbType.String);
                            SqliteParameter CONTACTXML = new SqliteParameter("@CONTACTXML", System.Data.DbType.String);

                            cmd.Parameters.Add(ID);
                            cmd.Parameters.Add(CONTACTXML);

                            var contactXml = JsonConvert.SerializeObject(contact, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                            ID.Value = contact.Id;
                            CONTACTXML.Value = contactXml;

                            cmd.ExecuteNonQuery();
                        }

                        dbTrans.Commit();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
				throw new SQLException(SQLStatusCode.SaveContactError, ex);
            }
        }

		public void ClearContact()
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
							cmd.CommandText = sqlDeleteContact;
							cmd.ExecuteNonQuery();
						}
							
						dbTrans.Commit();
					}

					conn.Close();
				}
			}
			catch (Exception ex)
			{
				throw new SQLException(SQLStatusCode.ClearContactError, ex);
			}
		}
    }
}
