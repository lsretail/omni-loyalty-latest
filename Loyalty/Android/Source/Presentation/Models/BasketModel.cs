using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.Content;
using Android.Widget;

using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets;

namespace Presentation.Models
{
    public class BasketModel : OneListModel
    {
        private BasketService service;

        public BasketModel(Context context, IRefreshableActivity refreshActivity) : base(context, refreshActivity)
        {
        }

        public async Task AddItemToBasket(OneListItem item, bool openBasket = false, int index = 0)
        {
            ShowIndicator(true);
            AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId).State = BasketState.Updating;
            SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);

            if (openBasket)
                SendBroadcast(Utils.BroadcastUtils.OpenBasket);

            OneList newList = AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId);
            newList.CardId = AppData.Device.CardId;
            newList.AddItem(item);

            try
            {
                var list = await OneListSave(newList, true);
                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Basket);
                SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public async Task GetBasketByCardId(string cardId)
        {
            ShowIndicator(true);

            try
            {
                List<OneList> list = await OneListGetByCardId(cardId, ListType.Basket, true);
                OneList basketList = list.FirstOrDefault();
                if (basketList != null)
                {
                    basketList.CalculateBasket();
                    AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, basketList, ListType.Basket);
                }
                SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
        }

        public async Task<bool> ClearBasket()
        {
            bool success = false;

            ShowIndicator(true);

            BeginWsCall();

            try
            {
                success = await OneListDeleteById(AppData.Basket.Id);
                AppData.Basket.Clear();
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
            return success;
        }

        public async Task EditItem(string basketItemId, decimal newQty, VariantRegistration newVariant)
        {
            var newList = AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId);

            var existingItem = newList.Items.FirstOrDefault(x => x.Id == basketItemId);
            if (existingItem == null)
                return;

            existingItem.Quantity = newQty;
            if (newVariant != null)
                existingItem.VariantId = newVariant.Id;

            ShowIndicator(true);

            try
            {
                var list = await OneListSave(newList, true);
                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Basket);
                SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public async Task DeleteItem(string basketItemId)
        {
            await DeleteItem(AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId));
        }

        public async Task DeleteItem(OneListItem item)
        {
            OneList newList = AppData.Device.UserLoggedOnToDevice.GetBasket(AppData.Device.CardId);

            OneListItem existingItem = newList.ItemGetByIds(item.ItemId, item.VariantId, item.UnitOfMeasureId);
            if (existingItem == null)
                return;

            var existinItemIndex = newList.Items.IndexOf(existingItem);
            newList.Items.Remove(existingItem);
            if (newList.Items.Count == 0)
                newList.StoreId = string.Empty;     // remove store in case it invalid from previous operations with basket

            ShowIndicator(true);

            try
            {
                var list = await OneListSave(newList, true);
                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Basket);
                SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);

                ShowSnackbar(AddSnackbarAction(CreateSnackbar(Context.GetString(Resource.String.ApplicatioItemDeleted)),
                        Context.GetString(Resource.String.ApplicationUndo), async view =>
                        {
                            await AddItemToBasket(existingItem, index: existinItemIndex);
                        }));
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
            finally
            {
                ShowIndicator(false);
            }
        }

        public async Task<bool> SendOrder(Order basket)
        {
            var success = false;

            ShowIndicator(true);

            AppData.Basket.State = BasketState.Calculating;

            BeginWsCall();

            try
            {
                await service.OrderCreateAsync(basket);
                success = true;

                await ClearBasket();
                ShowToast(Resource.String.CheckoutViewOrderSuccess, ToastLength.Long);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return success;
        }

        protected override void CreateService()
        {
            base.CreateService();

            service = new BasketService(new BasketsRepository());
        }
    }
}
