using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Requests;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

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

        public OrderAvailabilityResponse OrderCheckAvailability(OneList request)
        {
            return repository.OrderCheckAvailability(request);
        }

        public List<InventoryResponse> ItemsInStoreGet(List<InventoryRequest> items, string storeId)
        {
            return repository.ItemsInStoreGet(items, storeId);
        }

        public async Task<OrderAvailabilityResponse> OrderCheckAvailabilityAsync(OneList request)
        {
            return await Task.Run(() => OrderCheckAvailability(request));
        }

        public async Task<Order> OrderCreateAsync(Order request)
        {
            return await Task.Run(() => OrderCreate(request));
        }

        public async Task<List<InventoryResponse>> ItemsInStoreGetAsync(List<InventoryRequest> items, string storeId)
        {
            return await Task.Run(() => ItemsInStoreGet(items, storeId));
        }
    }
}
