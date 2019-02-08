using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace LSRetail.Omni.Domain.Services.Loyalty.Transactions
{
    public class TransactionLocalService
    {
        private ITransactionLocalRepository iRepository;

        public TransactionLocalService(ITransactionLocalRepository iRep)
        {
            iRepository = iRep;
        }

        public List<SalesEntry> GetLocalTransactions()
        {
            return iRepository.GetLocalTransactions();
        }

        public void SaveTransactions(List<SalesEntry> transactions)
        {
            iRepository.SaveTransactions(transactions);
        }
    }
}
