using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.SQLite2.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Transactions;
using Presentation.Utils;

namespace Presentation.Models
{
	class TransactionModel : BaseModel
	{
		private LocalTransactionService transactionService;
		private TransactionRepository transactionRepository;

		public TransactionModel()
		{
			transactionRepository = new TransactionRepository();
			transactionService = new LocalTransactionService(transactionRepository);
		}

		public Transaction CreateTransaction()
		{
			var transaction = new Transaction(AppData.Basket.Id);

			AppData.Basket.Items.ForEach(basketItem => transaction.SaleLines.Add(
				new SaleLine()
				{
					Amount = AppData.MobileMenu.Currency.FormatDecimal(basketItem.Item.Price.Value),
					DiscountAmount = "",
					Item = basketItem.Item,
					Quantity = basketItem.Quantity
				})
			);

			if (AppData.MobileMenu != null)
				transaction.Amount = AppData.MobileMenu.Currency.FormatDecimal(AppData.Basket.Amount);
			else
				transaction.Amount = AppData.Basket.Amount.ToString();

			transaction.Date = DateTime.Now;

			// TODO Remove hardcoding
			transaction.DiscountAmount = "";
			transaction.NetAmount = "";
			transaction.Staff = "";
			transaction.Store = new Store("S001")
			{
				Description = "Cronus Store"
			};
			transaction.Terminal = "";
			transaction.VatAmount = "";

			return transaction;
		}

		public async void LoadLocalTransactions()
		{
			try
			{
				List<Transaction> transactionList = await this.transactionService.GetTransactionsAsync();

				AppData.Transactions = transactionList;
				System.Diagnostics.Debug.WriteLine("Local transactions loaded successfully, count: " + transactionList.Count);
			}
			catch(Exception ex) 
			{
				HandleException(ex, "TransactionModel.LoadLocalTransactions()", false);
			}

		}

		public async Task<bool> SyncTransactionsLocally()
		{
			try
			{
				List<Transaction> transactionList = await this.transactionService.SyncTransactionsAsync(AppData.Transactions);

				AppData.Transactions = transactionList;
				return true;
			}
				
			catch(Exception ex)
			{
				HandleException(ex, "TransactionModel.SyncTransactionsLocally()", false);
				return false;
			}

		}

	}
}