using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Domain.Orders;
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

        public OrderModel(Context context, IRefreshableActivity refreshableActivity) : base(context, refreshableActivity)
        {
        }

        protected override void CreateService()
        {
            orderService = new OrderService(new OrderRepository());
        }

        public async Task<string> OrderSave()
        {
            Show(true);

            BeginWsCall();

            var basketQrCode = new BasketQrCode(AppData.MobileMenu);
            basketQrCode.Items = AppData.Basket.Items;
            if (AppData.Contact != null)
            {
                basketQrCode.Contact = AppData.Contact;
                basketQrCode.PublishedOffers = AppData.Contact.PublishedOffers.Where(publishedOffer => AppData.Basket.PublishedOffers.FirstOrDefault(x => x.Id == publishedOffer.Id) != null).ToList();
            }

            var order = new OrderQueue("")
                {
                    DeviceId = "Android",
                    OrderXml = basketQrCode.Serialize(),
                };

            if (AppData.Contact != null)
            {
                order.ContactId = AppData.Contact.Id;
            }

            var queueId = string.Empty;

            try
            {
                var queue = await orderService.OrderSaveAsync(order);
                queueId = queue.Id;
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);

            return queueId;
        }
    }
}