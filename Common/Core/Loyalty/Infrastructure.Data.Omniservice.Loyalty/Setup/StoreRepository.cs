using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup
{
    public class StoreRepository : BaseRepository, IStoreRepository
    {
        public List<Store> GetStores()
        {
            string methodName = "StoresGetAll";
            var jObject = "";
            return base.PostData<List<Store>>(jObject, methodName);
        }

        public List<Store> GetStoresByCoordinates(double latitude, double longitude, double maxDistance, int maxNumberOfStores)
        {
            string methodName = "StoresGetByCoordinates";
            var jObject = new { latitude = latitude, longitude = longitude, maxDistance = maxDistance, maxNumberOfStores = maxNumberOfStores };
            return base.PostData<List<Store>>(jObject, methodName);
        }

        public List<Store> GetItemsInStock(string itemId, string variantId, double latitude, double longitude, double maxDistance, int maxNumberOfStores)
        {
            string methodName = "StoresGetbyItemInStock";
            var jObject = new { itemId = itemId, variantId = variantId, latitude = latitude, longitude = longitude, maxDistance = maxDistance, maxNumberOfStores = maxNumberOfStores };
            return base.PostData<List<Store>>(jObject, methodName);
        }

        public Store StoreGetById(string storeId)
        {
            string methodName = "StoreGetById";
            var jObject = new { storeId = storeId };
            return base.PostData<Store>(jObject, methodName);
        }
    }
}
