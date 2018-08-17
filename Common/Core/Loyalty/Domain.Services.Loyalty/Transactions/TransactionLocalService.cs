using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace LSRetail.Omni.Domain.Services.Loyalty.Transactions
{
    public class TransactionLocalService
    {
        private ITransactionLocalRepository iRepository;

        public TransactionLocalService(ITransactionLocalRepository iRep)
        {
            iRepository = iRep;
        }

        public List<LoyTransaction> GetLocalTransactions()
        {
            return iRepository.GetLocalTransactions();
        }

        public void SaveTransactions(List<LoyTransaction> transactions)
        {
            iRepository.SaveTransactions(transactions);
        }
    }
}
