using System;
using System.Collections.Generic;
using Presentation.Utils;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;

namespace Presentation.Models
{
    public class ItemModel : BaseModel
	{
		private IItemRepository itemRepository;
		private ItemService itemService;

		public ItemModel ()
		{
			itemRepository = new LoyItemRepository();
			itemService = new ItemService(itemRepository);
		}

		public async Task<bool> GetItemCategories()
		{
            try
            {
                List<ItemCategory> itemCategories = await itemService.GetItemCategoriesAsync();
                if (itemCategories != null)
                {
                    AppData.ItemCategories = itemCategories;
                    return true;
                }
                else
                {
                    return false;
                }
            }
			catch (Exception ex)
			{
				HandleException(ex, "ItemModel.GetItemCategories()", false);
                return false;
			}
			
		}

		public async Task<List<LoyItem>> GetItemsByPage(int pageSize, int pageNumber, string itemCategoryId, string productGroupId, string search, bool includeDetails)
		{
            try
            {
                List<LoyItem> items = await this.itemService.GetItemsByPageAsync(pageSize, pageNumber, itemCategoryId, productGroupId, search, includeDetails);

                return items;

            }
			catch (Exception ex)
			{
				HandleException (ex, "ItemModel.GetItemsByPage()", false);
                return null;
			}
			
		}

        public async Task<List<LoyItem>> GetItemsByItemSearch(string search, bool includeDetails)
		{
            try
            {

                List<LoyItem> items = await this.itemService.GetItemsByItemSearchAsync(search, 100, includeDetails);
                return items;
            }
		
			catch (Exception ex)
			{
				HandleException (ex, "ItemModel.GetItemsByItemSearch()", false);
                return null;
			}
			
		}

        public async Task<LoyItem> GetItemByBarcode(string barcode)
		{
            try
            {
                LoyItem item = await this.itemService.GetItemByBarcodeAsync(barcode);
                if (item != null)
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }
			catch (Exception ex)
			{
				HandleException (ex, "ItemModel.GetItemByBarcode()", true);
                return null;
			}
			
		}

        public async Task<LoyItem> GetItem(string id)
		{
            try
            {
                LoyItem item = await this.itemService.GetItemAsync(id);

                System.Diagnostics.Debug.WriteLine("ItemModel.GetItem() - SUCCESS - item description: " + item.Description);
                return item;

            }
			catch (Exception ex)
			{
				HandleException (ex, "ItemModel.GetItem()", true);
                return null;
			}
			
		}

        public async Task<List<LoyItem>> GetItemsByPublishedOfferId(string publishedOfferId, int numberOfItems)
		{
            try
            {
                List<LoyItem> items = await this.itemService.GetItemsByPublishedOfferIdAsync(publishedOfferId, numberOfItems);
                return items;

            }
			catch (Exception ex)
			{
				HandleException (ex, "ItemModel.GetItemsByPublishedOfferId()", false);
                return null;
			}
		
		}
	}
}

