using System;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Infrastructure.Data.SQLite.Transactions
{
    class TransactionFactory
    {
        public LoyTransaction BuildEntity(TransactionData transactionData)
        {
            var entity = new LoyTransaction(transactionData.TransactionId)
            {
                Amount = transactionData.Amount,
                Date = transactionData.Date,
                DiscountAmount = transactionData.DiscountAmount,
                NetAmount = transactionData.NetAmount,
                Staff = transactionData.Staff,
                Terminal = transactionData.Terminal,
                VatAmount = transactionData.VatAmount,
                Store = new Store(transactionData.StoreId)
                            {
                                Description = transactionData.StoreDescription
                            }
            };

            return entity;
        }

        public TransactionData BuildEntity(LoyTransaction transaction)
        {
            var entity = new TransactionData()
            {
                TransactionId = transaction.Id,
                Amount = transaction.Amount,
                Date = transaction.Date,
                DiscountAmount = transaction.DiscountAmount,
                NetAmount = transaction.NetAmount,
                Staff = transaction.Staff,
                StoreDescription = transaction.Store.Description,
                StoreId = transaction.Store.Id,
                Terminal = transaction.Terminal,
                VatAmount = transaction.VatAmount,
            };

            return entity;
        }
    }
}
