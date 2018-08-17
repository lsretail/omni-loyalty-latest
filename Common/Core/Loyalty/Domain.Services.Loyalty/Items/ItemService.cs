using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace LSRetail.Omni.Domain.Services.Loyalty.Items
{
    public class ItemService
    {
        private IItemRepository iItemRepository;

        public ItemService(IItemRepository iRepo)
        {
            iItemRepository = iRepo;
        }

        public List<ItemCategory> GetItemCategories()
        {
            return iItemRepository.GetItemCategories();
        }

        public ItemCategory GetItemCategoriesById(string itemCategoryId)
        {
            return iItemRepository.ItemCategoriesGetById(itemCategoryId);
        }

        public LoyItem GetItem(string itemId)
        {
            return iItemRepository.GetItem(itemId, string.Empty);
        }

        public LoyItem GetItemByBarcode(string barcode)
        {
            return iItemRepository.GetItemByBarcode(barcode, string.Empty);
        }

        public ProductGroup GetProductGroup(string productGroupId, bool includeDetails)
        {
            return iItemRepository.GetProductGroup(productGroupId, includeDetails);
        }

        public List<LoyItem> GetItemsByItemSearch(string searchString, int maxNumberOfResults, bool includeDetails)
        {
            return iItemRepository.GetItemsByItemSearch(searchString, maxNumberOfResults, includeDetails);
        }

        public List<LoyItem> GetItemsByPage(int pageSize, int pageNumber, string itemCategoryId, string productGroupId, string search, bool includeDetails)
        {
            return iItemRepository.GetItemsByPage(pageSize, pageNumber, itemCategoryId, productGroupId, search, includeDetails);
        }

        public List<LoyItem> GetItemsByPublishedOfferId(string publishedOfferId, int numberOfItems)
        {
            return iItemRepository.GetItemsByPublishedOfferId(publishedOfferId, numberOfItems);
        }

        public bool RecommendedActive()
        {
            return iItemRepository.RecommendedActive();
        }

        public List<RecommendedItem> RecommendedItemsGetByUserId(string userId, List<LoyItem> items, int maxNumberOfItems)
        {
            return iItemRepository.RecommendedItemsGetByUserId(userId, items, maxNumberOfItems);
        }

        public List<RecommendedItem> RecommendedItemsGet(string userId, string storeId, string items)
        {
            return iItemRepository.RecommendedItemsGet(userId, storeId, items);
        }

        public async Task<List<ItemCategory>> GetItemCategoriesAsync()
        {
            return await Task.Run(() => GetItemCategories());
        }

        public async Task<ItemCategory> GetItemCategoriesByIdAsync(string itemCategoryId)
        {
            return await Task.Run(() => GetItemCategoriesById(itemCategoryId));
        }

        public async Task<LoyItem> GetItemAsync(string itemId)
        {
            return await Task.Run(() => GetItem(itemId));
        }

        public async Task<LoyItem> GetItemByBarcodeAsync(string barcode)
        {
            return await Task.Run(() => GetItemByBarcode(barcode));
        }

        public async Task<ProductGroup> GetProductGroupAsync(string productGroupId, bool includeDetails)
        {
            return await Task.Run(() => GetProductGroup(productGroupId, includeDetails));
        }

        public async Task<List<LoyItem>> GetItemsByItemSearchAsync(string searchString, int maxNumberOfResults, bool includeDetails)
        {
            return await Task.Run(() => GetItemsByItemSearch(searchString, maxNumberOfResults, includeDetails));
        }

        public async Task<List<LoyItem>> GetItemsByPageAsync(int pageSize, int pageNumber, string itemCategoryId, string productGroupId, string search, bool includeDetails)
        {
            return await Task.Run(() => GetItemsByPage(pageSize, pageNumber, itemCategoryId, productGroupId, search, includeDetails));
        }

        public async Task<List<LoyItem>> GetItemsByPublishedOfferIdAsync(string publishedOfferId, int numberOfItems)
        {
            return await Task.Run(() => GetItemsByPublishedOfferId(publishedOfferId, numberOfItems));
        }

        public async Task<bool> RecommendedActiveAsync()
        {
            return await Task.Run(() => RecommendedActive());
        }

        public async Task<List<RecommendedItem>> RecommendedItemsGetByUserIdAsync(string userId, List<LoyItem> items, int maxNumberOfItems)
        {
            return await Task.Run(() => RecommendedItemsGetByUserId(userId, items, maxNumberOfItems));
        }

        public async Task<List<RecommendedItem>> RecommendedItemsGetAsync(string userId, string storeId, string items)
        {
            return await Task.Run(() => RecommendedItemsGet(userId, storeId, items));
        }

    }
}

