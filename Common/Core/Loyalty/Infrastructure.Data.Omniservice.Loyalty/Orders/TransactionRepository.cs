using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Orders
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        public List<SalesEntry> GetSalesEntries(string cardId, int maxNumberOfTransactions)
        {
            string methodName = "SalesEntriesGetByCardId";
            var jObject = new { cardId = cardId, maxNumberOfTransactions = maxNumberOfTransactions };
            return base.PostData<List<SalesEntry>>(jObject, methodName);
        }

        public SalesEntry SalesEntryGetById(string entryId, DocumentIdType type)
        {
            string methodName = "SalesEntryGet";
            var jObject = new { entryId = entryId, type = type };
            return base.PostData<SalesEntry>(jObject, methodName);
        }
    }
}
