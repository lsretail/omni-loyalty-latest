using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Orders;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Hospitality.Orders;
using Presentation.Utils;

namespace Presentation.Models
{
	public class OrderModel : BaseModel
	{
		private OrderService orderService;
		private OrderRepository orderRepository;

		public OrderModel()
		{
			orderRepository = new OrderRepository();
			orderService = new OrderService(orderRepository);
		}
			
		public async Task<string> OrderSave()
		{
			var basketQrCode = new BasketQrCode(AppData.MobileMenu);
			basketQrCode.Items = AppData.Basket.Items;

			if (AppData.Contact != null)
			{
				basketQrCode.Contact = AppData.Contact;
				basketQrCode.PublishedOffers = AppData.SelectedPublishedOffers;
			}

			var order = new OrderQueue(string.Empty)
			{
				DeviceId = "iOS",
				OrderXml = basketQrCode.Serialize(),
			};

			if (AppData.Contact != null)
			{
				order.ContactId = AppData.Contact.Id;
			}

			System.Diagnostics.Debug.WriteLine("orderxml to send:" + System.Environment.NewLine + order.OrderXml);

			try
			{
				OrderQueue queue = await orderService.OrderSaveAsync(order);
				return queue.Id;

			}
			catch(Exception exception) 
			{
				HandleException(exception, "OrderModel.OrderSave()", false);
				return null;
			}

		}
	}
}