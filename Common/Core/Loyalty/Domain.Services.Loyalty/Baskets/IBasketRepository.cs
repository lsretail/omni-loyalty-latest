using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace LSRetail.Omni.Domain.Services.Loyalty.Baskets
{
    public interface IBasketRepository
    {
        Order OrderCreate(Order request);
        List<OrderLineAvailability> OrderAvailabilityCheck(OrderAvailabilityRequest request);
        OrderAvailabilityResponse OrderCheckAvailability(OneList request);
    }
}
