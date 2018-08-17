using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace LSRetail.Omni.Domain.Services.Loyalty.Items
{
    public interface IItemRepository
    {
        /// <summary>
        /// Get all available item categories
        /// </summary>
        /// <returns>Returns a list of ItemCategory objects</returns>
        List<ItemCategory> GetItemCategories();

        /// <summary>
        /// Returns an item by item id
        /// </summary>
        /// <param name="itemId">The item id of the item to be returned</param>
        /// <param name="storeId">Store Id to get price for, if empty prices for all stores returned</param>
        /// <returns>
        /// If an item with the provided id is not found the function returns an instance of the UnknownItem class.
        /// Otherwise the function returns the item.
        /// </returns>
        LoyItem GetItem(string itemId, string storeId);

        /// <summary>
        /// Returns an item by barcode
        /// </summary>
        /// <param name="barcode">The barcode of the item to be returned</param>
        /// <param name="storeId">Store Id to get price for, if empty prices for all stores returned</param>
        /// <returns>
        /// If an item with the provided barcode is not found the function returns an instance of the UnknownItem class.
        /// Otherwise the function returns the item.
        /// </returns>
        LoyItem GetItemByBarcode(string barcode, string storeId);

        /// <summary>
        /// Get a product group by id
        /// </summary>
        /// <param name="productGroupId">The id of the product group to be returned</param>
        /// <returns>
        /// If a product group with the provided id is not found the function returns an instance of the UnknownProductGroup class.
        /// Otherwise the function returns the product group.
        /// </returns>
        ProductGroup GetProductGroup(string productGroupId, bool includeDetails);

        /// <summary>
        /// Get a list of items according to a provided search string
        /// </summary>
        /// <param name="searchString">The search criteria</param>
        /// /// <param name="maxNumberOfResults">The maximum number of search results to return</param>
        /// <returns>A list of Item objects</returns>
        List<LoyItem> GetItemsByItemSearch(string searchString, int maxNumberOfResults, bool includeDetails);

        ItemCategory ItemCategoriesGetById(string itemCategoryId);
		List<LoyItem> GetItemsByPage(int pageSize, int pageNumber, string itemCategoryId, string productGroupId, string search, bool includeDetails);
		List<LoyItem> GetItemsByPublishedOfferId(string publishedOfferId, int numberOfItems);

        bool RecommendedActive();

        List<RecommendedItem> RecommendedItemsGetByUserId(string userId, List<LoyItem> items, int maxNumberOfItems);

        List<RecommendedItem> RecommendedItemsGet(string userId, string storeId, string items);
    }
}
