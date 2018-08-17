using System;
using Infrastructure.Data.SQLite2.Webservice;
using Infrastructure.Data.SQLite2.DTO;

namespace Presentation.Models
{
    public class WebserviceModel : BaseModel
	{
		private WebserviceRepository localRepo;
		//private DebugService debugService;

		public WebserviceModel ()
		{
			this.localRepo = new WebserviceRepository();
			//this.debugService = new DebugService (new DebugRepository());
		}

		public WebserviceData GetWebserviceData()
		{
			return this.localRepo.GetWebserviceData();
		}

		public void SaveWebserviceData(WebserviceData wsData, Action onSuccess, Action onFailure)
		{
			bool success = false;

			try
			{
				this.localRepo.SaveWebserviceData(wsData);
				success = true;
			}
			catch (Exception ex)
			{
				success = false;
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				this.HandleException(ex, "WebserviceModel.SaveWebserviceData()", true);
				onFailure();
			}

			if (success)
				onSuccess();	
		}

		public void ping(string url)
		{
            var utils = new LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils();
			try
			{
               string message = utils.PingServer();
                if (message != null) 
					{
						//Alert was never shown ?
						/*UIAlertView alert = new UIAlertView (
							"Ping",
							message,
							null,
							"ok",   //LocalizationUtilities.LocalizedString("General_OK", "OK"),
							null);*/
					}
					
			}
			catch(Exception) 
			{
				//Alert was never shown ?
				/*UIAlertView alert = new UIAlertView (
	                "Ping",
					ex.Message,
	                null,
					"ok",//LocalizationUtilities.LocalizedString("General_OK", "OK"),
	                null);*/
			}
		}
	}
}

