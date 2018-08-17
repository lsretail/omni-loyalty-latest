using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

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
        List<LoyTransaction> GetSalesEntries(string contactId, int numerOfTransactionsToReturn);

        /// <summary>
        /// Gets a transaction
        /// </summary>
        /// <param name="receiptNo">Receipt Number</param>
        /// <returns>
        /// A transaction header with the sale and tender lines. 
        /// </returns>
        LoyTransaction GetTransactionByReceiptNo(string receiptNo);

        /// <summary>
        /// Gets a list of transaction headers
        /// </summary>
        /// <param name="contactId">The user the transactions belong to</param>
        /// <param name="itemSerach">The search string used as criteria for item description</param>
        /// <param name="numerOfTransactionsToReturn">The number of most recent transactions to return</param>
        /// <param name="includeLines">Include transaction detail lines</param>
        /// <returns>
        /// A list of transaction headers.  To get the full details for a transaction a separate function must be called
        /// for that specific transaction.
        /// </returns>
        List<LoyTransaction> GetTransactionSearch(string contactId, string itemSerach, int numerOfTransactionsToReturn, bool includeLines);

        LoyTransaction SalesEntryGetById(string entryId);

        Order OrderGetById(string orderId, bool includeLines);
    }
}
