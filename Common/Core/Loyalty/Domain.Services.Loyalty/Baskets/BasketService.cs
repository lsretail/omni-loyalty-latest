using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace LSRetail.Omni.Domain.Services.Loyalty.Baskets
{
    public class BasketService
    {
        private IBasketRepository repository;

        public BasketService(IBasketRepository iRepo)
        {
            repository = iRepo;
        }

        public Order OrderCreate(Order request)
        {
            return repository.OrderCreate(request);
        }

        public List<OrderLineAvailability> OrderAvailabilityCheck(OrderAvailabilityRequest request)
        {
            return repository.OrderAvailabilityCheck(request);
        }

        public OrderAvailabilityResponse OrderCheckAvailability(OneList request)
        {
            return repository.OrderCheckAvailability(request);
        }

        public async Task<List<OrderLineAvailability>> OrderAvailabilityCheckAsync(OrderAvailabilityRequest request)
        {
            return await Task.Run(() => OrderAvailabilityCheck(request));
        }

        public async Task<OrderAvailabilityResponse> OrderCheckAvailabilityAsync(OneList request)
        {
            return await Task.Run(() => OrderCheckAvailability(request));
        }

        public async Task<Order> OrderCreateAsync(Order request)
        {
            return await Task.Run(() => OrderCreate(request));
        }

        public Order CreateOrderForCAC(OneList basket, string contactId, string cardId, string storeId, string email)
        {
            try
            {
                Order order = new Order()
                {
                    CardId = cardId,
                    SourceType = SourceType.LSOmni,
                    ClickAndCollectOrder = true,
                    AnonymousOrder = (string.IsNullOrWhiteSpace(contactId)),
                    StoreId = storeId,
                    ContactId = contactId,
                    Email = email
                };

                int cnt = 1;
                foreach (OneListItem basketItem in basket.Items)
                {
                    order.OrderLines.Add(new OrderLine()
                    {
                        Amount = basketItem.Amount,
                        ItemId = basketItem.Item.Id,
                        LineNumber = cnt,
                        NetAmount = basketItem.NetAmount,
                        NetPrice = basketItem.NetPrice,
                        OrderId = order.Id,
                        Price = basketItem.Price,
                        Quantity = basketItem.Quantity,
                        TaxAmount = basketItem.TaxAmount,
                        UomId = basketItem.UnitOfMeasureId,
                        VariantId = basketItem.VariantId,
                    });

                    foreach (OneListItemDiscount basketItemDiscountLine in basketItem.OnelistItemDiscounts)
                    {
                        order.OrderDiscountLines.Add(new OrderDiscountLine()
                        {
                            DiscountType = basketItemDiscountLine.DiscountType,
                            No = basketItemDiscountLine.No,
                            LineNumber = cnt,
                            OfferNumber = basketItemDiscountLine.OfferNumber,
                            Description = basketItemDiscountLine.Description,
                            DiscountAmount = basketItemDiscountLine.DiscountAmount,
                            DiscountPercent = basketItemDiscountLine.DiscountPercent,
                            PeriodicDiscGroup = basketItemDiscountLine.PeriodicDiscGroup,
                            PeriodicDiscType = basketItemDiscountLine.PeriodicDiscType,
                            OrderId = order.Id,
                        });
                    }
                    cnt++;
                }
                return order;
            }
            catch
            {
                return null;
            }
        }

        public Order CreateOrderForSale(OneList basket, string store, Device device, Address billingAddress, Address shippingAddress, PaymentType paymentType, string currencyCode, string cardNumber, string cardCVV, string cardName)
        {
            Order order = new Order()
            {
                ContactAddress = billingAddress,
                ContactId = device.UserLoggedOnToDevice.Id,
                StoreId = store,
                Email = device.UserLoggedOnToDevice.Email,
                ContactName = device.UserLoggedOnToDevice.Name,
                CardId = device.UserLoggedOnToDevice.Card.Id,
                PhoneNumber = device.UserLoggedOnToDevice.Phone,
                ShippingAgentServiceCode = "ISP",
                ShipToAddress = shippingAddress,
                ClickAndCollectOrder = false,
                AnonymousOrder = (string.IsNullOrWhiteSpace(device.UserLoggedOnToDevice.Id))
            };

            int cnt = 1;

            foreach (OneListItem basketItem in basket.Items)
            {
                order.OrderLines.Add(new OrderLine()
                {
                    Id = basketItem.Id,
                    ItemId = basketItem.Item.Id,
                    LineNumber = cnt,
                    Quantity = basketItem.Quantity,
                    UomId = basketItem.UnitOfMeasureId,
                    VariantId = basketItem.VariantId,
                    Amount = basketItem.Amount,
                    NetAmount = basketItem.NetAmount,
                    TaxAmount = basketItem.TaxAmount,
                    Price = basketItem.Price,
                    NetPrice = basketItem.NetPrice,
                });

                foreach (OneListItemDiscount basketItemDiscountLine in basketItem.OnelistItemDiscounts)
                {
                    order.OrderDiscountLines.Add(new OrderDiscountLine()
                    {
                        Id = basketItemDiscountLine.Id,
                        DiscountType = basketItemDiscountLine.DiscountType,
                        No = basketItemDiscountLine.No,
                        LineNumber = cnt,
                        OfferNumber = basketItemDiscountLine.OfferNumber,
                        Description = basketItemDiscountLine.Description,
                        DiscountAmount = basketItemDiscountLine.DiscountAmount,
                        DiscountPercent = basketItemDiscountLine.DiscountPercent,
                        PeriodicDiscGroup = basketItemDiscountLine.PeriodicDiscGroup,
                        PeriodicDiscType = basketItemDiscountLine.PeriodicDiscType,
                    });
                }
                cnt++;
            }

            if (paymentType == PaymentType.CreditCard)
            {
                order.OrderPayments.Add(new OrderPayment()
                {
                    PreApprovedAmount = basket.TotalAmount,
                    FinalizedAmount = basket.TotalAmount,
                    CardType = "VISA",
                    CurrencyCode = currencyCode,
                    AuthorisationCode = cardCVV,
                    CardNumber = cardNumber,
                    LineNumber = 1,
                    TenderType = ((int)TenderType.Card).ToString(),
                });
            }
            else if (paymentType == PaymentType.PayOnDelivery)
            {
                order.OrderPayments.Add(new OrderPayment()
                {
                    PreApprovedAmount = basket.TotalAmount,
                    FinalizedAmount = basket.TotalAmount,
                    CurrencyCode = currencyCode,
                    LineNumber = 1,
                    TenderType = ((int)TenderType.Cash).ToString(),
                });
            }
            return order;
        }
    }
}
