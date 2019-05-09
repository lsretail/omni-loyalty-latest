using System;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.Services.Loyalty.Search;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;

namespace Presentation.Models
{
    public class GeneralSearchModel : BaseModel
    {
        private SearchService service;
        private SharedService sharedService;
        private string lastSearchKey = string.Empty;

        public GeneralSearchModel(Context context, IRefreshableActivity refreshableActivity) : base(context, refreshableActivity)
        {
        }

        public void ResetSearch()
        {
            lastSearchKey = string.Empty;
        }

        public async Task<SearchRs> Search(string searchKey, SearchType searchType)
        {
            SearchRs searchRs = null;

            if (searchKey == lastSearchKey)
                return null;

            lastSearchKey = searchKey;
            ShowIndicator(true);

            BeginWsCall();

            var userId = string.Empty;

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {
                userId = AppData.Device.UserLoggedOnToDevice.Id;
            }

            try
            {
                searchRs = await service.SearchAsync(userId, searchKey, Int32.MaxValue, searchType);

                if (lastSearchKey != searchKey)
                    return null;
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            if (lastSearchKey == searchKey)
                ShowIndicator(false);

            return searchRs;
        }

        public async Task<string> AppSettings(ConfigKey key)
        {
            string result = string.Empty;

            ShowIndicator(true);

            BeginWsCall();

            try
            {
                result = await sharedService.AppSettingsAsync(key, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return result;
        }

        protected override void CreateService()
        {
            service = new SearchService(new SearchRepository());
            sharedService = new SharedService(new SharedRepository());
        }
    }
}