using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;

namespace Presentation.Models
{
	public class StoreModel : BaseModel
	{
		private StoreRepository storeRepository;
		private StoreService storeService;

		public StoreModel ()
		{
			//TODO: setup security token
			//storeRepository = new StoreRepository(Util.AppDelegate.Device.SecurityToken);

			//MOCK:
			//storeRepository = new StoreRepository ();


			storeRepository = new StoreRepository();
			storeService = new StoreService(storeRepository);
		}

		public async Task<List<Store>> GetAllStores()
		{
			try
			{
				List<Store> stores = await storeService.GetStoresAsync();
				return stores;
			}
			catch (Exception ex)
			{
				base.HandleException(ex, "StoreModel.GetAllStores()", false);
				return null;
			}
			finally
			{
				Utils.UI.StopNetworkActivityIndicator();
			}

		}


	}
}

