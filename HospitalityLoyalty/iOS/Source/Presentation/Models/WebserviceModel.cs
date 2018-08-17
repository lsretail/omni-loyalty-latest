using System;
using UIKit;
using System.ComponentModel;
using System.Collections.Generic;
using Infrastructure.Data.SQLite2.Webservice;
using Infrastructure.Data.SQLite2.DTO;

namespace Presentation.Models
{
	public class WebserviceModel : BaseModel
	{
		private WebserviceRepository localRepo;

		public WebserviceModel ()
		{
			this.localRepo = new WebserviceRepository();
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



		/* PCL Core functionality - Not in use
		private static string BaseURLKey = "BaseUrl";

		public WebserviceModel ()
		{
		}

		public string GetBaseURL()
		{
			return NSUserDefaults.StandardUserDefaults.StringForKey(BaseURLKey);
		}

		public void SaveBaseURL(string url, Action onSuccess, Action onFailure)
		{
			try
			{
				NSUserDefaults.StandardUserDefaults.SetString(url, BaseURLKey);
			}
			catch (Exception ex)
			{
				HandleException(ex, "WebserviceModel.SaveWebserviceData()", true);
				onFailure();
			}
		}

		public void InitializeWebService()
		{
			string baseUrl = GetBaseURL();
			if(baseUrl == null)
			{
				SetDefaultBaseURL();
			}

			Infrastructure.Data.WS.Utils.Utils.InitWebService("", GetBaseURL());
		}

		private void SetDefaultBaseURL()
		{
			string defaultBaseURL = "http://lsretail.cloudapp.net/";
			string defaultResource = "LSOmniService/json.svc/";

			SaveBaseURL(defaultBaseURL + defaultResource, () => {}, () => {});
		}
		*/
	}
}

