using System;
using System.Collections.Generic;
using System.Linq;
using Presentation.Utils;
using System.Threading.Tasks;

using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace Presentation.Models
{
    public class BasketModel : OneListModel
    {
        private BasketService basketService;
        private IBasketRepository basketRepository;

        public BasketModel()
        {
            this.basketRepository = new BasketsRepository();
            this.basketService = new BasketService(this.basketRepository);
        }

        public async void AddItemToBasket(decimal quantity, LoyItem item, string itemVariant, string itemUOM, Action onSuccess, Action onFailure)
        {
            AppData.Device.UserLoggedOnToDevice.Basket.AddItem(new OneListItem(item, quantity, itemUOM, itemVariant));

            try
            {
                OneList returnedList = await OneListSave(AppData.Device.UserLoggedOnToDevice.Basket, true);
                if (returnedList != null)
                {
                    // Successfully synced with BO
                    AppData.Device.UserLoggedOnToDevice.Basket = returnedList;
                    if (onSuccess != null)
                        onSuccess();
                }
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "BasketModel.AddItemToBasket()", false);
                if (onFailure != null)
                    onFailure();
            }
        }

        internal Task<bool> SendOrder(OneList basket, Device device, object billingAddress, object shippingAddress, PaymentType paymentType, string id1, string id2, object cardCVV, string name)
        {
            throw new NotImplementedException();
        }

        public async void AddWishListToBasket(Action onSuccess, Action onFailure)
        {
            var itemsToAdd = new List<OneListItem>();

            foreach (var wishListItem in AppData.Device.UserLoggedOnToDevice.WishList.Items)
                itemsToAdd.Add(
                    new OneListItem(
                        wishListItem.Item,
                        wishListItem.Quantity,
                        wishListItem.UnitOfMeasure != null ? wishListItem.UnitOfMeasureId : string.Empty,
                        wishListItem.VariantReg != null ? wishListItem.VariantReg.Id : string.Empty
                    )
                );

            await AddItemsToBasket(itemsToAdd, onSuccess, onFailure);
        }

        public async Task AddItemsToBasket(List<OneListItem> items, Action onSuccess, Action onFailure)
        {
            foreach (OneListItem item in items)
                AppData.Device.UserLoggedOnToDevice.Basket.AddItem(item);

            try
            {
                OneList returnedList = await OneListSave(AppData.Device.UserLoggedOnToDevice.Basket, false);
                if (returnedList != null)
                {
                    // Successfully synced with BO
                    AppData.Device.UserLoggedOnToDevice.Basket = returnedList;
                    if (onSuccess != null)
                        onSuccess();
                }
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "BasketModel.AddItemsToBasket()", false);
                if (onFailure != null)
                    onFailure();
            }
        }

        public async void RemoveItemFromBasket(int position, Action onSuccess, Action onFailure)
        {
            AppData.Device.UserLoggedOnToDevice.Basket.RemoveItemAtPosition(position);

            try
            {
                OneList returnedList = await OneListSave(AppData.Device.UserLoggedOnToDevice.Basket, true);
                if (returnedList != null)
                {
                    // Successfully synced with BO
                    AppData.Device.UserLoggedOnToDevice.Basket = returnedList;
                    if (onSuccess != null)
                        onSuccess();
                }
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "BasketModel.RemoveItemFromBasket()", false);
                if (onFailure != null)
                    onFailure();
            }
        }

        public async Task<OneList> CalculateBasket(OneList basket)
        {
            try
            {
                basket.StoreId = "S0013";
                Order response = await OneListCalculate(basket);
                if (response != null)
                {
                    List<OneListItem> basketItems = new List<OneListItem>();

                    foreach (OrderLine basketLineCalcResponse in response.OrderLines)
                    {
                        OneListItem item = basket.ItemGetByIds(basketLineCalcResponse.ItemId, basketLineCalcResponse.VariantId, basketLineCalcResponse.UomId);
                        OneListItem basketItem = new OneListItem()
                        {
                            Item = item.Item,
                            Quantity = basketLineCalcResponse.Quantity,
                            UnitOfMeasure = new UnitOfMeasure(basketLineCalcResponse.UomId, basketLineCalcResponse.ItemId),
                            VariantReg = item.VariantReg,
                            Amount = basketLineCalcResponse.Amount,
                            NetAmount = basketLineCalcResponse.NetAmount,
                            NetPrice = basketLineCalcResponse.NetPrice,
                            TaxAmount = basketLineCalcResponse.TaxAmount,
                            Price = basketLineCalcResponse.Price,
                            DiscountAmount = basketLineCalcResponse.DiscountAmount,
                            DiscountPercent = basketLineCalcResponse.DiscountPercent
                        };

                        basketItems.Add(basketItem);
                    }

                    foreach (OrderDiscountLine basketDiscLine in response.OrderDiscountLines)
                    {
                        OneListItem item = basketItems.Find(i => i.DisplayOrderId == basketDiscLine.LineNumber / 10000);
                        if (item == null)
                            continue;
                            
                        item.OnelistItemDiscounts.Add(new OneListItemDiscount()
                        {
                            DiscountType = basketDiscLine.DiscountType,
                            No = basketDiscLine.No,
                            OfferNumber = basketDiscLine.OfferNumber,
                            Description = basketDiscLine.Description,
                            DiscountAmount = basketDiscLine.DiscountAmount,
                            DiscountPercent = basketDiscLine.DiscountPercent,
                            PeriodicDiscGroup = basketDiscLine.PeriodicDiscGroup,
                            PeriodicDiscType = basketDiscLine.PeriodicDiscType
                        });
                    }

                    OneList calculatedBasket = new OneList(basket.Id, basketItems, false);   // Don't want to re-calculate the basket locally, we just calculated in the backend
                    calculatedBasket.TotalNetAmount = response.TotalNetAmount;
                    calculatedBasket.TotalAmount = response.TotalAmount;
                    calculatedBasket.TotalTaxAmount = calculatedBasket.TotalAmount - calculatedBasket.TotalNetAmount;
                    calculatedBasket.TotalDiscAmount = response.TotalDiscount;
                    return calculatedBasket;
                }
                return null;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Save and Calculate Basket", true);
                return null;
            }
        }

        public async Task<Order> SendOrder()
        {
            var contact = AppData.Device.UserLoggedOnToDevice;
            var address = new Address()
            {
                Type = AddressType.Residential,
                Address1 = "Hagasmari 3",
                Address2 = string.Empty,
                City = "Kopavogur",
                PostCode = "201",
                StateProvinceRegion = string.Empty,
                Country = "Iceland"
            };

            try
            {
                Order order = basketService.CreateOrderForSale(AppData.Device.UserLoggedOnToDevice.Basket, "S0013", AppData.Device, address, address, PaymentType.CreditCard, contact.Environment.Currency.Id, "EFTCardNumber", "EFTAuthCode", contact.Name);
                return await this.basketService.OrderCreateAsync(order);
            }
            catch (Exception ex)
            {
                HandleException(ex, "BasketModel.SendOrder()", true);
                return null;
            }
        }

        public async void Refresh(Action onSuccessNotEmpty, Action onSuccessEmpty, Action onFailure)
        {
            try
            {
                List<OneList> returnedLists = await OneListGetByCardId(AppData.Device.CardId, ListType.Basket, true);
                if (returnedLists != null)
                {
                    System.Diagnostics.Debug.WriteLine("BasketModel.Refresh() - Success, number of OneLists returned: " + returnedLists.Count.ToString());

                    // Only taking the first onelist that is returned
                    if (returnedLists.Count > 0)
                    {
                        AppData.Device.UserLoggedOnToDevice.Basket = returnedLists[0];
                        onSuccessNotEmpty();
                    }
                    else
                    {
                        AppData.Device.UserLoggedOnToDevice.Basket.Items.Clear();
                        onSuccessEmpty();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "BasketModel.Refresh()", true);
                onFailure();
            }
        }

        public async void Save(Action onSuccess, Action onFailure)
        {
            try
            {
                OneList returnedList = await OneListSave(AppData.Device.UserLoggedOnToDevice.Basket, false);
                if (returnedList != null)
                {
                    AppData.Device.UserLoggedOnToDevice.Basket = returnedList;
                    if (onSuccess != null)
                        onSuccess();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "BasketModel.Save()", true);

                if (onFailure != null)
                    onFailure();
            }
        }

        public async void EditItemAtPosition(int position, OneListItem editedItem, Action onSuccess, Action onFailure)
        {
            AppData.Device.UserLoggedOnToDevice.Basket.RemoveItemAtPosition(position);
            AppData.Device.UserLoggedOnToDevice.Basket.AddItemAtPosition(position, editedItem);

            try
            {
                OneList returnedList = await OneListSave(AppData.Device.UserLoggedOnToDevice.Basket, false);
                if (returnedList != null)
                {
                    // Successfully synced with BO
                    AppData.Device.UserLoggedOnToDevice.Basket = returnedList;
                    if (onSuccess != null)
                        onSuccess();
                }
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "BasketModel.EditItemAtPosition()", false);
                if (onFailure != null)
                    onFailure();
            }
        }

        public async void ClearBasket(Action onSuccess, Action onFailure)
        {
            try
            {
                bool success = await OneListDeleteById(AppData.Device.UserLoggedOnToDevice.Basket.Id, ListType.Basket);
                if (success)
                {
                    // Successfully synced with BO
                    if (success)
                        AppData.Device.UserLoggedOnToDevice.Basket.Clear();

                    if (onSuccess != null)
                        onSuccess();
                }
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "BasketModel.ClearBasket()", false);
                if (onFailure != null)
                    onFailure();
            }
        }
    }
}
