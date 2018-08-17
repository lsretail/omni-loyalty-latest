using System;
using System.Threading.Tasks;
using Domain.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Orders;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Orders
{
    public class OrderService
    {
        private IOrderRepository repository;

        public OrderService(IOrderRepository repository)
        {
            this.repository = repository;
        }

        public OrderQueue OrderSave(OrderQueue orderQueue)
        {
            return repository.OrderSave(orderQueue);
        }

        #region windows

        public async Task<OrderQueue> OrderSaveAsync(OrderQueue orderQueue)
        {
            return await Task.Run(() => OrderSave(orderQueue));
        }

        #endregion

    }
}
