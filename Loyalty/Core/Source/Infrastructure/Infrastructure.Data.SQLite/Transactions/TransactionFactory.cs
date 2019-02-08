using System;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace Infrastructure.Data.SQLite.Transactions
{
    class TransactionFactory
    {
        public SalesEntry BuildEntity(TransactionData transactionData)
        {
            var entity = new SalesEntry(transactionData.TransactionId)
            {
                TotalAmount = transactionData.Amount,
                DocumentRegTime = transactionData.Date,
                TotalDiscount = transactionData.DiscountAmount,
                TotalNetAmount = transactionData.NetAmount,
                StoreId = transactionData.StoreId,
                StoreName = transactionData.StoreDescription,
                TerminalId = transactionData.Terminal
            };
            return entity;
        }

        public TransactionData BuildEntity(SalesEntry transaction)
        {
            var entity = new TransactionData()
            {
                TransactionId = transaction.Id,
                Amount = transaction.TotalAmount,
                Date = transaction.DocumentRegTime,
                DiscountAmount = transaction.TotalDiscount,
                NetAmount = transaction.TotalNetAmount,
                StoreDescription = transaction.StoreName,
                StoreId = transaction.StoreId,
                Terminal = transaction.TerminalId
            };
            return entity;
        }
    }
}
