using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Domain.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Transactions;
using Newtonsoft.Json;
#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif

namespace Infrastructure.Data.SQLite2.Transactions
{
    public class TransactionRepository : ILocalTransactionRepository
    {
        private static readonly string sqlGetTransactions = "Select * FROM TRANS";
        private static readonly string sqlInsertTransaction = @"INSERT OR REPLACE INTO TRANS VALUES (@ID, @TRANSACTIONXML)";
        private static readonly string sqlDeleteTransactions = @"DELETE FROM TRANS";

        public List<Transaction> GetTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                var conn = SqliteHelper.DatabaseConnection();

                SqliteCommand cmd = new SqliteCommand(sqlGetTransactions, conn);

                SqliteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    transactions.Add(JsonConvert.DeserializeObject<Transaction>(reader["TRANSACTIONXML"].ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
                }

                reader.Close();

                cmd.Dispose();

                SqliteHelper.CloseDBConnection(conn);

                return transactions;
            }
            catch (Exception x)
            {
				throw new SQLException(SQLStatusCode.GetTransactionsError, x);
            }
        }

        public void SyncTransactions(List<Transaction> transactions)
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
                            cmd.CommandText = sqlDeleteTransactions;
                            cmd.ExecuteNonQuery();
                        }

                        using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = sqlInsertTransaction;

                            SqliteParameter ID = new SqliteParameter("@ID", System.Data.DbType.String);
                            SqliteParameter TRANSACTION = new SqliteParameter("@TRANSACTIONXML", System.Data.DbType.String);

                            cmd.Parameters.Add(ID);
                            cmd.Parameters.Add(TRANSACTION);

                            foreach (var transaction in transactions)
                            {
                                var transactionXml = JsonConvert.SerializeObject(transaction, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                                ID.Value = transaction.Id;
                                TRANSACTION.Value = transactionXml;

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
				throw new SQLException(SQLStatusCode.SaveTransactionsError, ex);
            }
        }
    }
}
