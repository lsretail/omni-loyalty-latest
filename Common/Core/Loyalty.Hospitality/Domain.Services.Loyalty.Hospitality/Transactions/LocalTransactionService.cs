using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Transactions
{
    public class LocalTransactionService
    {
        private ILocalTransactionRepository repository;

        public LocalTransactionService(ILocalTransactionRepository repository)
        {
            this.repository = repository;
        }

        public List<Transaction> GetTransactions()
        {
            return repository.GetTransactions();
        }

        public List<Transaction> SyncTransactions(List<Transaction> transactions)
        {
            var existingTransactions = GetTransactions();

            /*
            existingTransactions.ForEach(transaction =>
                {
                    if (transactions.FirstOrDefault(x => x.Id == transaction.Id) == null)
                    {
                        transactions.Add(transaction);
                    }
                });

             */

            foreach (var existingTransaction in existingTransactions)
            {
                if (transactions.FirstOrDefault(x => x.Id == existingTransaction.Id) == null)
                {
                    transactions.Add(existingTransaction);
                }
            }

            repository.SyncTransactions(transactions);

            return transactions;
        }

        #region windows

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            return await Task.Run(() => GetTransactions());
        }

        public async Task<List<Transaction>> SyncTransactionsAsync(List<Transaction> transactions)
        {
            return await Task.Run(() => SyncTransactions(transactions));
        }

        #endregion


    }
}