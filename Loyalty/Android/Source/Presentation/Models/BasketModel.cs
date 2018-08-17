using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.Content;
using Android.Widget;

using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
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
            AppData.Device.UserLoggedOnToDevice.Basket.State = BasketState.Updating;
            SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);

            if (openBasket)
                SendBroadcast(Utils.BroadcastUtils.OpenBasket);

            OneList newList = AppData.Device.UserLoggedOnToDevice.Basket;
            newList.CardId = AppData.Device.UserLoggedOnToDevice.Card.Id;
            newList.ContactId = AppData.Device.UserLoggedOnToDevice.Id;
            newList.AddItem(item);

            try
            {
                AppData.Device.UserLoggedOnToDevice.Basket = await OneListSave(newList, true);
                SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public async Task GetBasketByContactId(string contactId)
        {
            ShowIndicator(true);

            try
            {
                List<OneList> list = await OneListGetByContactId(contactId, ListType.Basket, true);
                OneList basketList = list.FirstOrDefault();
                if (basketList != null)
                {
                    basketList.CalculateBasket();
                    AppData.Device.UserLoggedOnToDevice.Basket = basketList;
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
                success = await OneListDeleteById(AppData.Device.UserLoggedOnToDevice.Basket.Id, ListType.Basket);
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
            var newList = AppData.Device.UserLoggedOnToDevice.Basket;

            var existingItem = newList.Items.FirstOrDefault(x => x.Id == basketItemId);
            if (existingItem == null)
                return;

            existingItem.Quantity = newQty;
            if (newVariant != null)
                existingItem.VariantReg = newVariant;

            ShowIndicator(true);

            try
            {
                AppData.Device.UserLoggedOnToDevice.Basket = await OneListSave(newList, true);
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
            OneList newList = AppData.Device.UserLoggedOnToDevice.Basket;

            OneListItem existingItem = newList.ItemGetByIds(item.Item.Id, item.VariantReg?.Id, item.UnitOfMeasure?.Id);
            if (existingItem == null)
                return;

            var existinItemIndex = newList.Items.IndexOf(existingItem);
            newList.Items.Remove(existingItem);

            ShowIndicator(true);

            try
            {
                AppData.Device.UserLoggedOnToDevice.Basket = await OneListSave(newList, true);
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

        public async Task<bool> SendOrder(OneList basket, Device device, Address billingAddress, Address shippingAddress, PaymentType paymentType, string currencyCode, string cardNumber, string cardCVV, string cardName)
        {
            var success = false;

            ShowIndicator(true);

            AppData.Basket.State = BasketState.Calculating;

            BeginWsCall();

            try
            {
                await service.OrderCreateAsync(service.CreateOrderForSale(basket, "S0013", device, billingAddress, shippingAddress, paymentType, currencyCode, cardNumber, cardCVV, cardName));
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
