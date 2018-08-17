using System;
//using Infrastructure.Data.WS.Stores;
//using Domain.MemberContacts.Exceptions;
using Presentation.Utils;
using System.Collections.Generic;
//using Infrastructure.Data.Mock.Stores;
using System.Threading.Tasks;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation.Models
{
    public class StoreModel : BaseModel
	{
		private IStoreRepository storeRepository;
		private StoreService storeService;

		public StoreModel ()
		{
			storeRepository = new StoreRepository();
			storeService = new StoreService(storeRepository);
		}

		public async Task<List<Store>> GetAllStores()
		{
            try
            {
                List<Store> stores = await this.storeService.GetStoresAsync();
                if (stores != null)
                {
                    AppData.Stores = stores;

                }
                return stores;
            }

			catch (Exception ex)
			{
					HandleException (ex, "StoreModel.GetAllStores()", true);
                return null;
			}
			
		}

		public async Task<List<Store>> GetStoresByCoordinates(double latitude, double longitude, double maxDistance, int maxNumberOfStores)
		{
            try
            {
                List<Store> stores = await this.storeService.GetStoresByCoordinatesAsync(latitude, longitude, maxDistance, maxNumberOfStores);
                return stores;
             }
			catch (Exception ex)
			{
				HandleException (ex, "StoreModel.GetStoresByCoordinates()", true);
                return null;
			}
			
		}

		public async Task<List<Store>> GetItemsInStock(string itemId, string variantId, double latitude, double longitude, double maxDistance, int maxNumberOfStores)
		{
            try
            {
                List<Store> stores = await this.storeService.GetItemsInStockAsync(itemId, variantId, latitude, longitude, maxDistance, maxNumberOfStores);
                return stores;
            }
			catch (Exception ex)
			{
				HandleException (ex, "StoreModel.GetItemsInStock()", true);
                return null;
			}
			
		}
	}
}

