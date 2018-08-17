using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;

using Presentation.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;

namespace Presentation.Models
{
    public class ItemModel : BaseModel
    {
        private ItemService service;
        private string lastSearchKey;
        private IRefreshableActivity refreshableActivity;

        public ItemModel(Context context, IRefreshableActivity refreshableActivity = null)
            : base(context)
        {
            this.refreshableActivity = refreshableActivity;
        }

        public async Task<List<LoyItem>> GetItemsByPage(int pageSize, int pageNumber, string categoryId, string productGroupId, string search)
        {
            List<LoyItem> items = null;

            lastSearchKey = search;

            BeginWsCall();

            try
            {
                items = await service.GetItemsByPageAsync(pageSize, pageNumber, categoryId, productGroupId, search, true);

                if (search != lastSearchKey)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            return items;
        }

        public async Task GetItemCategories()
        {
            ShowRefreshableIndicator(true);

            BeginWsCall();

            try
            {
                var categories = await service.GetItemCategoriesAsync();

                AppData.ItemCategories = categories;
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowRefreshableIndicator(false);
        }

        public async Task<List<LoyItem>> ItemsGetByPublishedOfferIdAsync(string publishedOfferId)
        {
            List<LoyItem> items = null;

            ShowRefreshableIndicator(true);

            BeginWsCall();

            try
            {
                items = await service.GetItemsByPublishedOfferIdAsync(publishedOfferId, Int32.MaxValue);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowRefreshableIndicator(false);

            return items;
        }

        public async Task<LoyItem> GetItemById(string itemId)
        {
            LoyItem item = null;

            ShowRefreshableIndicator(true);

            BeginWsCall();

            try
            {
                item = await service.GetItemAsync(itemId);

                if (item == null)
                    throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    ShowToast(Resource.String.ItemModelItemNotFound);
                }
                else
                {
                    await HandleUIExceptionAsync(ex, showAsToast:true);
                }
            }

            ShowRefreshableIndicator(false);

            return item;
        }

        public async Task<LoyItem> GetItemByBarcode(string barcode)
        {
            LoyItem item = null;

            ShowRefreshableIndicator(true);

            BeginWsCall();

            try
            {
                item = await service.GetItemByBarcodeAsync(barcode);

                if (item == null)
                    throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    ShowToast(Resource.String.ItemModelItemNotFound);
                }
                else
                {
                    await HandleUIExceptionAsync(ex, showAsToast: true);
                }
            }

            ShowRefreshableIndicator(false);

            return item;
        }

        private void ShowRefreshableIndicator(bool show)
        {
            if(refreshableActivity != null)
                refreshableActivity.ShowIndicator(show);
        }

        protected override void CreateService()
        {
            service = new ItemService(new LoyItemRepository());
        }
    }
}