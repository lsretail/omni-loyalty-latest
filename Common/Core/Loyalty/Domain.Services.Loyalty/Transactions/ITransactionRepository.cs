using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace LSRetail.Omni.Domain.Services.Loyalty.Transactions
{
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets a list of transaction headers
        /// </summary>
        /// <param name="contactId">The user the transactions belong to</param>
        /// <param name="numerOfTransactionsToReturn">The number of most recent transactions to return</param>
        /// <returns>
        /// A list of transaction headers.  To get the full details for a transaction a separate function must be called
        /// for that specific transaction.
        /// </returns>
        List<SalesEntry> GetSalesEntries(string contactId, int numerOfTransactionsToReturn);

        SalesEntry SalesEntryGetById(string entryId, SourceType type);
    }
}
