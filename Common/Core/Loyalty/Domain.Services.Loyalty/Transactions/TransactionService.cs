using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace LSRetail.Omni.Domain.Services.Loyalty.Transactions
{
    public class TransactionService
    {
        private ITransactionRepository iTransactionRepository;

        public TransactionService(ITransactionRepository iRepo)
        {
            iTransactionRepository = iRepo;
        }

        public List<SalesEntry> GetSalesEntries(string cardId, int numerOfTransactionsToReturn)
        {
            return iTransactionRepository.GetSalesEntries(cardId, numerOfTransactionsToReturn);
        }

        public SalesEntry SalesEntryGetById(string entryId, DocumentIdType type)
        {
            return iTransactionRepository.SalesEntryGetById(entryId, type);
        }

        public async Task<List<SalesEntry>> GetSalesEntriesAsync(string cardId, int numerOfTransactionsToReturn)
        {
            return await Task.Run(() => GetSalesEntries(cardId, numerOfTransactionsToReturn));
        }

        public async Task<SalesEntry> SalesEntryGetByIdAsync(string entryId, DocumentIdType type)
        {
            return await Task.Run(() => SalesEntryGetById(entryId, type));
        }
    }
}
