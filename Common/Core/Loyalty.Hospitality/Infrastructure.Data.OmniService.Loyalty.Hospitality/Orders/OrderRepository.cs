using Domain.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Orders;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Hospitality.Orders
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderQueue OrderSave(OrderQueue orderQueue)
        {
            //remove all CRLF from xml.  important in Restsharp..keeping here
            orderQueue.OrderXml = orderQueue.OrderXml.Replace("\r\n", "");
            orderQueue.OrderXml = orderQueue.OrderXml.Replace("\n", "");
            orderQueue.OrderXml = orderQueue.OrderXml.Replace("\"", "'");

            string methodName = "OrderQueueSave";
            var jObject = new { order = orderQueue };
            return base.PostData<OrderQueue>(jObject, methodName);
        }
    }
}
