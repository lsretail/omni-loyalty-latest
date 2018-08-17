using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace LSRetail.Omni.Domain.Services.Loyalty.Transactions
{
    public interface ITransactionLocalRepository
    {
        List<LoyTransaction> GetLocalTransactions();
        void SaveTransactions(List<LoyTransaction> transactions);
    }
}
