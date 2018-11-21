using System;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Utils
{
    public class Utils
    {
        public enum AppType
        {
            POS,
            Inventory,
            Loyalty,
            HospitalityLoyalty
        };

		public const string DefaultUrl = @"http://mobiledemo.lsretail.com/LSOmniService/appjson.svc";
        public const string DefaultUrlLoyalty = @"http://mobiledemo.lsretail.com/LSOmniService/loyjson.svc";


		public static void InitWebService(string deviceId, AppType appType, string url = "", int timeOut = 0, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(url))
            {
                ConfigStatic.Url = (appType == AppType.POS || appType == AppType.Inventory) ? DefaultUrl : DefaultUrlLoyalty;
            }
            else
            {
                ConfigStatic.Url = url;
            }
            ConfigStatic.Timeout = timeOut;
            ConfigStatic.LanguageCode = languageCode;
            ConfigStatic.UniqueDeviceId = deviceId;
        }

        public string PingServer()
        {
            try
            {
                var repository = new UtilityRepository();
                return repository.Ping(ConfigStatic.Timeout);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
