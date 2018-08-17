using System;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;

namespace Presentation.Models
{
	public class AppSettingsModel : BaseModel
	{
		private SharedService appSettingsService;
		private SharedRepository appSettingsRepository;

		public AppSettingsModel ()
		{
			appSettingsRepository = new SharedRepository();
			appSettingsService = new SharedService(appSettingsRepository);
		}

		public async Task<string> AppSettingsGetByKey(AppSettingsKey appSettingsKey, string languageCode)
		{
			try
			{
				string appsettings = await this.appSettingsService.AppSettingsAsync(appSettingsKey, languageCode);
				return appsettings;
			}
			catch(Exception ex)
			{
				HandleException(ex, "AppSettingsModel.AppSettingsGetByKey()", false);
				return null;
			}

		}
	}
}

