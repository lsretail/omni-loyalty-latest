using System;
using System.Collections.Generic;
using Presentation.Utils;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation.Models
{
    class TransactionModel : BaseModel
	{
		private ITransactionRepository transactionRepository;
		private TransactionService transactionService;

		public TransactionModel()
		{
			this.transactionRepository = new TransactionRepository ();
			this.transactionService = new TransactionService (this.transactionRepository);
		}


		public async Task<LoyTransaction> GetTransaction(LoyTransaction transaction)
		{
            try
            {
                LoyTransaction trans = await this.transactionService.GetTransactionByReceiptNoAsync(transaction.ReceiptNumber);
                return trans;

            }
			catch (Exception ex)
			{
				HandleException (ex, "TransactionModel.GetTransaction()", false);
                return null;
			}
			
		}

		public async Task<bool> GetTransactionHeaders(string contactId)
		{
            try
            {
                List<LoyTransaction> listOfTransactions = await this.transactionService.GetSalesEntriesAsync(contactId, Int32.MaxValue);
                if (listOfTransactions != null)
                {
                    AppData.Device.UserLoggedOnToDevice.Transactions = listOfTransactions;
                   
                }

                return true;
            }

			catch (Exception ex)
			{
				HandleException (ex, "TransactionModel.GetTransactionHeaders()", false);
                return false;
			}
			
		}
	}
}