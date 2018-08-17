using System.Collections.Generic;
using System.Linq;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.Services.Loyalty.Items;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup
{
    public class LoyItemRepository : BaseRepository, IItemRepository
    {
        public List<ItemCategory> GetItemCategories()
        {
            string methodName = "ItemCategoriesGetAll";
            var jObject = "";
            return base.PostData<List<ItemCategory>>(jObject, methodName);
        }

        public ProductGroup GetProductGroup(string productGroupId, bool includeDetails)
        {
            string methodName = "ProductGroupGetById";
            var jObject = new { productGroupId = productGroupId, includeDetails = includeDetails };
            return base.PostData<ProductGroup>(jObject, methodName);
        }

        public List<LoyItem> GetItemsByItemSearch(string search, int maxNumberOfItems, bool includeDetails)
        {
            string methodName = "ItemsSearch";
            var jObject = new { search = search, maxNumberOfItems = maxNumberOfItems, includeDetails = includeDetails };
            return base.PostData<List<LoyItem>>(jObject, methodName);
        }

        public List<LoyItem> GetItemsByPage(int pageSize, int pageNumber, string itemCategoryId, string productGroupId, string search, bool includeDetails)
        {
            string methodName = "ItemsPage";
            var jObject = new { pageSize = pageSize, pageNumber = pageNumber, itemCategoryId = itemCategoryId, productGroupId = productGroupId, search = search, includeDetails = includeDetails };
            var res = base.PostData<List<LoyItem>>(jObject, methodName);
            res = res.Where(x => x.AllowedToSell).ToList();
            return res;
        }

        public ItemCategory ItemCategoriesGetById(string itemCategoryId)
        {
            string methodName = "ItemCategoriesGetById";
            var jObject = new { itemCategoryId = itemCategoryId };
            return base.PostData<ItemCategory>(jObject, methodName);
        }

        public LoyItem GetItem(string itemId, string storeId)
        {
            string methodName = "ItemGetById";
            var jObject = new { itemId = itemId, storeId = storeId };
            return base.PostData<LoyItem>(jObject, methodName);
        }

        public LoyItem GetItemByBarcode(string barcode, string storeId)
        {
            string methodName = "ItemGetByBarcode";
            var jObject = new { barcode = barcode, storeId = storeId };
            return base.PostData<LoyItem>(jObject, methodName);
        }

        public List<LoyItem> GetItemsByPublishedOfferId(string pubOfferId, int numberOfItems)
        {
            string methodName = "ItemsGetByPublishedOfferId";
            var jObject = new { pubOfferId = pubOfferId, numberOfItems = numberOfItems };
            return base.PostData<List<LoyItem>>(jObject, methodName);
        }

        #region LS Recommends

        public bool RecommendedActive()
        {
            string methodName = "RecommendedActive";
            var jObject = "";
            return base.PostData<bool>(jObject, methodName);
        }

        public List<RecommendedItem> RecommendedItemsGetByUserId(string userId, List<LoyItem> items, int maxNumberOfItems)
        {
            string methodName = "RecommendedItemsGetByUserId";
            var jObject = new { userId = userId, items = items, maxNumberOfItems = maxNumberOfItems };
            return base.PostData<List<RecommendedItem>>(jObject, methodName);
        }

        public List<RecommendedItem> RecommendedItemsGet(string userId, string storeId, string items)
        {
            string methodName = "RecommendedItemsGet";
            var jObject = new { userId = userId, storeId = storeId, items = items };
            return base.PostData<List<RecommendedItem>>(jObject, methodName);
        }

        #endregion LS Recommends

    }
}
