using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Presentation.Util;

namespace Presentation.Models
{
    public class StoreModel : BaseModel
    {
        private StoreService service;

        public StoreModel(Context context, IRefreshableActivity refreshableActivity) : base(context, refreshableActivity)
        {
        }

        public async Task GetAllStores()
        {
            ShowIndicator(true);

            BeginWsCall();

            try
            {
                AppData.Stores = await service.GetStoresAsync();
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public async Task<List<Store>> GetItemsInStock(string itemId, string variantId, double longitde, double latitude, double maxDistance)
        {
            List<Store> stores = null;

            ShowIndicator(true);

            BeginWsCall();

            try
            {
                stores = await service.GetItemsInStockAsync(itemId, variantId, longitde, latitude, maxDistance, Int32.MaxValue);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return stores;
        }

        protected override void CreateService()
        {
            service = new StoreService(new StoreRepository());
        }
    }
}