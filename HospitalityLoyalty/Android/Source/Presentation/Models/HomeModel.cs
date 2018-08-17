using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Utils;

namespace Presentation.Models
{
    class HomeModel : BaseModel
    {
        private SharedService service;

        public HomeModel(Context context, IRefreshableActivity refreshableActivity = null)
            : base(context, refreshableActivity)
        {
        }

        protected override void CreateService()
        {
            service = new SharedService(new SharedRepository());
        }

        public async void GetAdvertisements()
        {
            Show(true);

            BeginWsCall();

            try
            {
                var ads = await service.AdvertisementsGetByIdAsync("HOSPLOY", "");
                AppData.Advertisements = ads;
                SendBroadcast(BroadcastUtils.AdsUpdated);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);
        }
    }
}