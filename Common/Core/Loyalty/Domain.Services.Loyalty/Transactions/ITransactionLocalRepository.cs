using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace LSRetail.Omni.Domain.Services.Loyalty.Transactions
{
    public interface ITransactionLocalRepository
    {
        List<SalesEntry> GetLocalTransactions();
        void SaveTransactions(List<SalesEntry> transactions);
    }
}
