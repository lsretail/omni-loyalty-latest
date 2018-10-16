using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Content;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets;
using Presentation.Util;

namespace Presentation.Models
{
    public class ClickCollectModel : BaseModel
    {
        private BasketService service;
        private BasketModel basketModel;

        public ClickCollectModel(Context context, IRefreshableActivity refreshableActivity) : base(context, refreshableActivity)
        {
            basketModel = new BasketModel(context, refreshableActivity);
        }

        public async Task<List<OrderLineAvailability>> OrderAvailabilityCheck(string storeId)
        {
            OrderAvailabilityResponse orderLineAvailabilities = null;

            BeginWsCall();

            ShowIndicator(true);

            OneList basket = AppData.Device.UserLoggedOnToDevice.Basket;
            basket.StoreId = storeId;
            basket.CardId = AppData.Device.UserLoggedOnToDevice.Card.Id;

            try
            {
                orderLineAvailabilities = await service.OrderCheckAvailabilityAsync(basket);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            List<OrderLineAvailability> list = new List<OrderLineAvailability>();
            foreach (OrderLineAvailabilityResponse line in orderLineAvailabilities.Lines)
            {
                if(orderLineAvailabilities.PreferredSourcingLocation == orderLineAvailabilities.Lines[orderLineAvailabilities.Lines.IndexOf(line)].LocationCode)
                {
                    list.Add(new OrderLineAvailability()
                    {
                        ItemId = line.ItemId,
                        UomId = line.UnitOfMeasureId,
                        VariantId = line.VariantId,
                        Quantity = line.Quantity
                    });
                }
            }
            return list;
        }

        public async Task<bool> ClickCollectOrderCreate(OneList basket, string contactId, string cardId, string storeId, string email)
        {
            bool success = false;
            Order result = null;

            BeginWsCall();

            ShowIndicator(true);

            try
            {
                //success = await service.OrderCreateAsync(service.CreateOrderForCAC(basket, contactId, cardId, storeId, email));
                result = await service.OrderCreateAsync(service.CreateOrderForCAC(basket, contactId, cardId, storeId, email));

                //if (success)
                if (result != null)
                {
                    await basketModel.ClearBasket();
                    ShowToast(Resource.String.CheckoutViewOrderSuccess, ToastLength.Long);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return success;
        }

        public List<OneListItem> CreateBasketItems(List<OrderLineAvailability> orderLineAvailabilities)
        {
            var basketItems = new List<OneListItem>();
            var unavailableItems = new List<string>();

            foreach (var orderLineAvailability in orderLineAvailabilities)
            {
                OneListItem basketItem = AppData.Basket.ItemGetByIds(orderLineAvailability.ItemId, orderLineAvailability.VariantId, orderLineAvailability.UomId);
                if (basketItem == null)
                    continue;

                if (orderLineAvailability.Quantity > 0 && basketItem != null)
                {
                    var availableBasketItem = new OneListItem()
                    {
                        Id = basketItem.Id,
                        Item = basketItem.Item,
                        Amount = basketItem.Amount,
                        NetAmount = basketItem.NetAmount,
                        NetPrice = basketItem.NetPrice,
                        Price = basketItem.Price,
                        TaxAmount = basketItem.TaxAmount,
                        Quantity = basketItem.Quantity,
                        UnitOfMeasure = basketItem.UnitOfMeasure,
                        VariantReg = basketItem.VariantReg,
                    };

                    if (basketItem.Quantity > orderLineAvailability.Quantity)
                    {
                        unavailableItems.Add("-" + (basketItem.Quantity - orderLineAvailability.Quantity) + " " + basketItem.Item.Description);
                        availableBasketItem.Quantity = orderLineAvailability.Quantity;
                    }

                    basketItems.Add(availableBasketItem);
                }
                else
                {
                    if (basketItem != null)
                        unavailableItems.Add("-" + (basketItem.Quantity) + " " + basketItem.Item.Description);
                }
            }

            return basketItems;
        }

        protected override void CreateService()
        {
            service = new BasketService(new BasketsRepository());
        }
    }
}
