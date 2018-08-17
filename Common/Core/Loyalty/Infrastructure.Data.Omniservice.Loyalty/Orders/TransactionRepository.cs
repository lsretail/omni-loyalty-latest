using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Orders
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        public List<LoyTransaction> GetSalesEntries(string contactId, int maxNumberOfTransactions)
        {
            string methodName = "SalesEntriesGetByContactId";
            var jObject = new { contactId = contactId, maxNumberOfTransactions = maxNumberOfTransactions };
            return base.PostData<List<LoyTransaction>>(jObject, methodName);
        }

        public LoyTransaction GetTransactionByReceiptNo(string receiptNo)
        {
            string methodName = "TransactionGetByReceiptNo";
            var jObject = new { receiptNo = receiptNo };
            return base.PostData<LoyTransaction>(jObject, methodName);
        }

        public List<LoyTransaction> GetTransactionSearch(string contactId, string itemSearch, int maxNumberOfTransactions, bool includeLines)
        {
            string methodName = "TransactionsSearch";
            var jObject = new { contactId = contactId, itemSearch = itemSearch, maxNumberOfTransactions = maxNumberOfTransactions, includeLines = includeLines.ToString().ToLower() };
            return base.PostData<List<LoyTransaction>>(jObject, methodName);
        }

        public LoyTransaction SalesEntryGetById(string entryId)
        {
            string methodName = "SalesEntryGetById";
            var jObject = new { entryId = entryId };
            return base.PostData<LoyTransaction>(jObject, methodName);
        }

        public Order OrderGetById(string orderId, bool includeLines)
        {
            string methodName = "OrderGetById";
            var jObject = new { id = orderId, includeLines = includeLines };
            return base.PostData<Order>(jObject, methodName);
        }
    }
}
