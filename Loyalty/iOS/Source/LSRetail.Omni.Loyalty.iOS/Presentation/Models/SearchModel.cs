using System;
using Presentation.Utils;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.Services.Loyalty.Search;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;

namespace Presentation.Models
{
    public class SearchModel : BaseModel
    {
        private ISearchRepository searchRepository;
        private SearchService searchService;
        private ISharedRepository sharedRepository;
        private SharedService sharedService;

        public SearchModel()
        {
            searchRepository = new SearchRepository();
            searchService = new SearchService(searchRepository);
            this.sharedRepository = new SharedRepository();
            sharedService = new SharedService(this.sharedRepository);
        }

        public async Task<SearchRs> GetSearch(string search)
        {
            //Always search at least for items, if item's are not enabled - search should not be enabled.
            try
            {
                SearchType searchType = SearchType.Item;

                if (EnabledItems.HasOffers)
                {
                    searchType = searchType | SearchType.Offer;
                }
                if (EnabledItems.HasCoupons)
                {
                    searchType = searchType | SearchType.Coupon;
                }
                if (EnabledItems.HasHistory)
                {
                    searchType = searchType | SearchType.Transaction;
                }
                if (EnabledItems.HasStoreLocator)
                {
                    searchType = searchType | SearchType.Store;
                }
                if (EnabledItems.HasNotifications)
                {
                    searchType = searchType | SearchType.Notification;
                }

                SearchRs searchRs = await this.searchService.SearchAsync(AppData.UserLoggedIn ? AppData.Device.UserLoggedOnToDevice.Id : string.Empty, search, 100, searchType);
                return searchRs;

            }
            catch (Exception ex)
            {
                HandleException(ex, "SearchModel.GetSearch()", false);
                return null;
            }

        }

        public async Task<string> GetAppSettings(ConfigKey key, string languageCode)
        {
            try
            {
                string settingsKey = await this.sharedService.AppSettingsAsync(key, languageCode);
                return settingsKey;
            }
            catch (Exception ex)
            {
                HandleException(ex, "SearchModel.GetAppSettings()", false);
                return "";
            }

        }
    }
}
