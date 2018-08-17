using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Transactions
{
    public interface ILocalTransactionRepository
    {
        List<Transaction> GetTransactions();
        void SyncTransactions(List<Transaction> transactions);
    }
}