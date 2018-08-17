using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Orders;

namespace Domain.Orders
{
    public interface IOrderRepository
    {
        OrderQueue OrderSave(OrderQueue orderQueue);
    }
}
