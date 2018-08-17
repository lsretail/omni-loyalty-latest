using System;
using Presentation.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets;

namespace Presentation.Models
{
    public class ClickCollectModel : BaseModel
    {
        private BasketService clickCollectService;
        private IBasketRepository clickCollectRepository;

        public ClickCollectModel()
        {
            this.clickCollectRepository = new BasketsRepository();
            this.clickCollectService = new BasketService(this.clickCollectRepository);
        }

        public async void CheckAvailability(string storeId, Action<OneList, List<OneListItem>> onSuccess, Action onFailure)
        {
            string guid = Guid.NewGuid().ToString();


            OneList basket = AppData.Device.UserLoggedOnToDevice.Basket;
            basket.StoreId = storeId;
            basket.CardId = AppData.Device.UserLoggedOnToDevice.Card.Id;

            try
            {
                OrderAvailabilityResponse orderAvailability = await this.clickCollectService.OrderCheckAvailabilityAsync(basket);
                if (orderAvailability != null)
                {
                    bool allItemsAvailable = true;

                    //if not all items are available, we will return new basket - with updated quantity
                    OneList newBasket = new OneList();
                    newBasket.State = BasketState.Dirty;
                    newBasket.CardId = AppData.Device.UserLoggedOnToDevice.Card.Id;

                    List<OneListItem> unavailableItems = new List<OneListItem>();
                    foreach (var orderLine in orderAvailability.Lines)
                    {
                        OneListItem basketItem = AppData.Device.UserLoggedOnToDevice.Basket.ItemGetByIds(orderLine.ItemId, orderLine.VariantId, orderLine.UnitOfMeasureId);
                        if (basketItem == null)
                            continue;

                        if (orderLine.Quantity > 0)
                        {
                            basketItem.Quantity = basketItem.Quantity;
                            newBasket.Items.Add(basketItem);
                            newBasket.TotalAmount += basketItem.Amount;
                            newBasket.TotalNetAmount += basketItem.NetAmount;
                            newBasket.TotalTaxAmount += basketItem.TaxAmount;
                            newBasket.TotalDiscAmount += basketItem.DiscountAmount;

                            if (basketItem.Quantity > orderLine.Quantity)
                            {
                                allItemsAvailable = false;
                                unavailableItems.Add(new OneListItem(basketItem.Id)
                                {
                                    Item = basketItem.Item,
                                    Quantity = basketItem.Quantity - orderLine.Quantity,
                                    UnitOfMeasure = basketItem.UnitOfMeasure,
                                    VariantReg = basketItem.VariantReg,

                                    Amount = basketItem.Amount,
                                    NetAmount = basketItem.NetAmount,
                                    TaxAmount = basketItem.TaxAmount,
                                    Price = basketItem.Price
                                });
                            }
                        }
                        else
                        {
                            allItemsAvailable = false;
                            unavailableItems.Add(new OneListItem(basketItem.Id)
                            {
                                Item = basketItem.Item,
                                Quantity = basketItem.Quantity - orderLine.Quantity,
                                UnitOfMeasure = basketItem.UnitOfMeasure,
                                VariantReg = basketItem.VariantReg,

                                Amount = basketItem.Amount,
                                NetAmount = basketItem.NetAmount,
                                TaxAmount = basketItem.TaxAmount,
                                Price = basketItem.Price
                            });
                        }
                    }

                    // if all items are available, we don't return any basket, the user just needs to confirm the original basket
                    if (allItemsAvailable)
                    {
                        onSuccess(null, null);
                    }
                    else
                    {
                        //remove all items that are not available at all
                        newBasket.Items.RemoveAll(x => x.Quantity <= 0);
                        onSuccess(newBasket, unavailableItems);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ClickCollectModel.CheckAvailability()", true);
            }
        }

        public async Task<Order> CreateOrder(string storeId, string email, OneList basket)
        {
            string guid = Guid.NewGuid().ToString();
            try
            {
                Order order = await this.clickCollectService.OrderCreateAsync(this.clickCollectService.CreateOrderForCAC(basket, AppData.Device.UserLoggedOnToDevice.Id, AppData.Device.UserLoggedOnToDevice.Card.Id, storeId, email));
                return order;
            }
            catch (Exception ex)
            {
                HandleException(ex, "ClickCollectModel.CreateOrder()", true);
                return null;
            }
        }
    }
}
