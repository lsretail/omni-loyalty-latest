using System;
using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Baskets
{
    public class BasketsRepository : BaseRepository, IBasketRepository
    {
		public List<OrderLineAvailability> OrderAvailabilityCheck(OrderAvailabilityRequest request)
		{
			string methodName = "OrderAvailabilityCheck";
			var jObject = new { request = request };
			return base.PostData<List<OrderLineAvailability>>(jObject, methodName);
		}

        public OrderAvailabilityResponse OrderCheckAvailability(OneList request)
        {
            string methodName = "OrderCheckAvailability";
            var jObject = new { request = request };
            return base.PostData<OrderAvailabilityResponse>(jObject, methodName);
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
