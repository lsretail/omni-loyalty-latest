
using System;
using System.Threading.Tasks;
using Android.Content;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Util;

namespace Presentation.Models
{
    public class AdvertisementModel : BaseModel
    {
        private readonly IRefreshableActivity refreshActivity;
        private SharedService service;

        public AdvertisementModel(Context context, IRefreshableActivity refreshActivity)
            : base(context)
        {
            this.refreshActivity = refreshActivity;
        }

        public async Task AdvertisementsGetById(string contactId)
        {
            if (string.IsNullOrWhiteSpace(contactId))
            {
                contactId = string.Empty;
            }

            refreshActivity.ShowIndicator(true);

            BeginWsCall();

            try
            {
                var advertisements = await service.AdvertisementsGetByIdAsync("LOY", contactId);

                AppData.Advertisements = advertisements;

                SendBroadcast(Utils.BroadcastUtils.AdvertisementsUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            refreshActivity.ShowIndicator(false);
        }

        protected override void CreateService()
        {
            service = new SharedService(new SharedRepository());
        }
    }
}