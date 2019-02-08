using System;
using System.Collections.Generic;
using Presentation.Utils;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace Presentation.Models
{
    class TransactionModel : BaseModel
    {
        private ITransactionRepository transactionRepository;
        private TransactionService transactionService;

        public TransactionModel()
        {
            this.transactionRepository = new TransactionRepository();
            this.transactionService = new TransactionService(this.transactionRepository);
        }

        public async Task<SalesEntry> GetTransaction(SalesEntry transaction)
        {
            try
            {
                return await this.transactionService.SalesEntryGetByIdAsync(transaction.Id, SourceType.Standard);
            }
            catch (Exception ex)
            {
                HandleException(ex, "TransactionModel.GetTransaction()", false);
                return null;
            }
        }

        public async Task<bool> GetTransactionHeaders(string contactId)
        {
            try
            {
                List<SalesEntry> listOfTransactions = await this.transactionService.GetSalesEntriesAsync(contactId, Int32.MaxValue);
                if (listOfTransactions != null)
                {
                    AppData.Device.UserLoggedOnToDevice.SalesEntries = listOfTransactions;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "TransactionModel.GetTransactionHeaders()", false);
                return false;
            }
        }
    }
}