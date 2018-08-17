using System.Collections.Generic;
using System.Linq;

using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;

namespace Infrastructure.Data.SQLite.Transactions
{
    public class TransactionRepository : ITransactionLocalRepository
    {
        private object locker = new object();

        public TransactionRepository()
        {
            DBHelper.OpenDBConnection();
        }

        public List<LoyTransaction> GetLocalTransactions()
        {
            lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                var factory = new TransactionFactory();
                var transactions = new List<LoyTransaction>();

                DBHelper.DBConnection.Table<TransactionData>().ToList().ForEach(x => transactions.Add(factory.BuildEntity(x)));

                return transactions;
            }
        }

        public void SaveTransactions(List<LoyTransaction> transactions)
        {
            lock (locker)
            {
                var factory = new TransactionFactory();

                var transactionData = new List<TransactionData>();
                foreach (var transaction in transactions)
                {
                    transactionData.Add(factory.BuildEntity(transaction));
                }

                DBHelper.DBConnection.DeleteAll<TransactionData>();
                DBHelper.DBConnection.InsertAll(transactionData);
            }
        }
    }
}
