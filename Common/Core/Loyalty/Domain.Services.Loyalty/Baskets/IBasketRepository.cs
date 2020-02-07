using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Requests;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace LSRetail.Omni.Domain.Services.Loyalty.Baskets
{
    public interface IBasketRepository
    {
        SalesEntry OrderCreate(Order request);
        OrderAvailabilityResponse OrderCheckAvailability(OneList request);
        List<InventoryResponse> ItemsInStoreGet(List<InventoryRequest> items, string storeId);
    }
}
