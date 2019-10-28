using System;
using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Requests;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets
{
    public class BasketsRepository : BaseRepository, IBasketRepository
    {
        public OrderAvailabilityResponse OrderCheckAvailability(OneList request)
        {
            string methodName = "OrderCheckAvailability";
            var jObject = new { request = request };
            return base.PostData<OrderAvailabilityResponse>(jObject, methodName);
        }

        public List<InventoryResponse> ItemsInStoreGet(List<InventoryRequest> items, string storeId)
        {
            string methodName = "ItemsInStoreGet";
            var jObject = new { items = items, storeId = storeId };
            return base.PostData<List<InventoryResponse>>(jObject, methodName);
        }

        public Order OrderCreate(Order request)
		{
			string methodName = "OrderCreate";
			var jObject = new { request = request };
			Order order = base.PostData<Order>(jObject, methodName);
            return order;
		}
	}
}
