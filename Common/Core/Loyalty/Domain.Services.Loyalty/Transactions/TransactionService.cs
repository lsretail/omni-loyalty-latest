using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace LSRetail.Omni.Domain.Services.Loyalty.Transactions
{
    public class TransactionService
    {
        private ITransactionRepository iTransactionRepository;

        public TransactionService(ITransactionRepository iRepo)
        {
            iTransactionRepository = iRepo;
        }

        public List<LoyTransaction> GetSalesEntries(string contactId, int numerOfTransactionsToReturn)
        {
            return iTransactionRepository.GetSalesEntries(contactId, numerOfTransactionsToReturn);
        }

        public LoyTransaction GetTransactionByReceiptNo(string receiptNo)
        {
            return iTransactionRepository.GetTransactionByReceiptNo(receiptNo);
        }

        public List<LoyTransaction> GetTransactionSearch(string contactId, string itemSerach, int numerOfTransactionsToReturn, bool includeLines)
        {
            return iTransactionRepository.GetTransactionSearch(contactId, itemSerach, numerOfTransactionsToReturn, includeLines);
        }

        public LoyTransaction SalesEntryGetById(string entryId)
        {
            return iTransactionRepository.SalesEntryGetById(entryId);
        }

        public Order OrderGetById(string orderId)
        {
            return iTransactionRepository.OrderGetById(orderId, true);
        }

        public async Task<List<LoyTransaction>> GetSalesEntriesAsync(string contactId, int numerOfTransactionsToReturn)
        {
            return await Task.Run(() => GetSalesEntries(contactId, numerOfTransactionsToReturn));
        }

        public async Task<LoyTransaction> GetTransactionByReceiptNoAsync(string receiptNo)
        {
            return await Task.Run(() => GetTransactionByReceiptNo(receiptNo));
        }

        public async Task<List<LoyTransaction>> GetTransactionSearchAsync(string contactId, string itemSerach, int numerOfTransactionsToReturn, bool includeLines)
        {
            return await Task.Run(() => GetTransactionSearch(contactId, itemSerach, numerOfTransactionsToReturn, includeLines));
        }

        public async Task<LoyTransaction> SalesEntryGetByIdAsync(string entryId)
        {
            return await Task.Run(() => SalesEntryGetById(entryId));
        }

        public async Task<Order> OrderGetByIdAsync(string orderId)
        {
            return await Task.Run(() => OrderGetById(orderId));
        }
    }
}
