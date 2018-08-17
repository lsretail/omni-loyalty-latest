using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Stores
{
    public interface IStoreRepository
    {
        // Returns a list all stores
        List<Store> GetStores();
		List<Store> GetStoresByCoordinates(double latitude, double longitude, double maxDistance, int maxNumberOfStores);
        
		// Returns a list of store that have items in stock
        List<Store> GetItemsInStock(string itemId, string variantId, double latitude, double longitude, double maxDistance, int maxNumberOfStores);
        Store StoreGetById(string storeId);
 
    }
}
 