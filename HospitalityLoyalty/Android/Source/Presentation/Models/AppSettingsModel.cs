using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Utils;

namespace Presentation.Models
{
    public class AppSettingsModel : BaseModel
    {
        private SharedService service;

        public AppSettingsModel(Context context, IRefreshableActivity refreshableActivity) : base(context, refreshableActivity)
        {
        }

        protected override void CreateService()
        {
            service = new SharedService(new SharedRepository());
        }

        public async Task<string> AppSettingsGetByKey(AppSettingsKey key, string languageCode)
        {
            var message = string.Empty;

            Show(true);

            BeginWsCall();

            try
            {
                message = await service.AppSettingsAsync(key, languageCode);
            }
            catch (Exception exception)
            {
                HandleUIException(exception);
            }

            Show(false);

            return message;
        }
    }
}