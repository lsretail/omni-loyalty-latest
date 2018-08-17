using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Terminals
{
    public class TerminalRepository : BaseRepository
    {
        public enum AppType
        {
            Inventory,
            Pos
        }

        public string Registration(string terminalId, string uniqueDeviceId, AppType appType)
        {
            var appTypeName = string.Empty;

            if (appType == AppType.Inventory)
            {
                appTypeName = "INV";
            }
            else if (appType == AppType.Pos)
            {
                appTypeName = "POS";
            }

            var methodName = "Registration";
            var jObject = new {terminalId = terminalId, appid = appTypeName, deviceId = uniqueDeviceId};
            var licenseKey = base.PostData<string>(jObject, methodName);

            if (!string.IsNullOrWhiteSpace(licenseKey))
            {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(Licence.ServerLicence.LicenseSettingsKey, licenseKey);
            }
            else
            {
                Plugin.Settings.CrossSettings.Current.Remove(Licence.ServerLicence.LicenseSettingsKey);
            }

            return licenseKey;
        }
    }
}
