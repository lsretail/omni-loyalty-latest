using System;
using System.Threading.Tasks;
using Android.Content;
using Infrastructure.Data.SQLite2.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Transactions;
using Presentation.Utils;

namespace Presentation.Models
{
    class TransactionModel : BaseModel
    {
        private LocalTransactionService localService;
        private LocalBasketService basketService;

        public TransactionModel(Context context, IRefreshableActivity refreshableActivity = null)
            : base(context, refreshableActivity)
        {
            basketService = new LocalBasketService();
        }

        protected override void CreateService()
        {
            localService = new LocalTransactionService(new TransactionRepository());
        }

        public Transaction CreateTransaction()
        {
            var transaction = new Transaction(AppData.Basket.Id);
            
            AppData.Basket.Items.ForEach(basketItem => transaction.SaleLines.Add(new SaleLine()
                {
                    Amount = AppData.FormatCurrency(basketService.GetBasketItemFullPrice(AppData.MobileMenu, basketItem)),
                    DiscountAmount = "",
                    Item = basketItem.Item,
                    Quantity = basketItem.Quantity
                }));

            transaction.Amount = AppData.FormatCurrency(AppData.Basket.Amount);
            transaction.Date = DateTime.Now;
            transaction.DiscountAmount = "";
            transaction.NetAmount = "";
            transaction.Staff = "";
            transaction.Store = new Store("S001")
                {
                    Description = "Chronos store"
                };
            transaction.Terminal = "";
            transaction.VatAmount = "";

            return transaction;
        }

        public async void GetTransactions()
        {
            Show(true);

            BeginWsCall();

            try
            {
                var transactions = await localService.GetTransactionsAsync();

                AppData.Transactions = transactions;
                SendBroadcast(BroadcastUtils.TransactionsUpdated);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }


            Show(false);
        }

        public async Task<bool> SyncTransactions()
        {
            var success = false;

            Show(true);

            BeginWsCall();

            try
            {
                var transactions = await localService.SyncTransactionsAsync(AppData.Transactions);

                AppData.Transactions = transactions;
                SendBroadcast(BroadcastUtils.TransactionsUpdated);

                success = true;
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);

            return success;
        }
    }
}