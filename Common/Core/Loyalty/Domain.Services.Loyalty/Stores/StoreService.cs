using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Stores
{
    public class StoreService
    {
        private IStoreRepository iStoreRepository;

        public StoreService(IStoreRepository iRepo)
        {
            iStoreRepository = iRepo;
        }

        public List<Store> GetStores()
        {
            return iStoreRepository.GetStores();
        }

		public List<Store> GetStoresByCoordinates(double latitude, double longitude, double maxDistance, int maxNumberOfStores)
		{
			return iStoreRepository.GetStoresByCoordinates(latitude, longitude, maxDistance, maxNumberOfStores);
		}
        
        public List<Store> GetItemsInStock(string itemId, string variantId, double latitude, double longitude, double maxDistance, int maxNumberOfStores)
        {   
            return iStoreRepository.GetItemsInStock(itemId, variantId, latitude, longitude,  maxDistance,  maxNumberOfStores);
        }

        public async Task<List<Store>> GetStoresAsync()
        {
            return await Task.Run(() => GetStores());
        }

        public async Task<List<Store>> GetStoresByCoordinatesAsync(double latitude, double longitude, double maxDistance, int maxNumberOfStores)
        {
            return await Task.Run(() => GetStoresByCoordinates(latitude, longitude, maxDistance, maxNumberOfStores));
        }

        public async Task<List<Store>> GetItemsInStockAsync(string itemId, string variantId, double latitude, double longitude, double maxDistance, int maxNumberOfStores)
        {
            return await Task.Run(() => GetItemsInStock(itemId, variantId, latitude, longitude, maxDistance, maxNumberOfStores));
        }
    }
}
