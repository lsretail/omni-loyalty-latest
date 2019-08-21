using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Shared
{
    public class SharedRepository : BaseRepository, ISharedRepository
    {
        public List<Advertisement> AdvertisementsGetById(string id, string contactId)
        {
            string methodName = "AdvertisementsGetById";
            var jObject = new { id = id, contactId = contactId };
            return base.PostData<List<Advertisement>>(jObject, methodName);
        }

        public List<PublishedOffer> GetPublishedOffers(string cardId, string itemId)
        {
            //if itemId is empty then no filtering is done.
            string methodName = "PublishedOffersGetByCardId";
            var jObject = new { cardId = cardId, itemId = itemId };
            return base.PostData<List<PublishedOffer>>(jObject, methodName);
        }

        public OmniEnvironment GetEnvironment()
        {
            string methodName = "Environment";
            var jObject = "";
            return base.PostData<OmniEnvironment>(jObject, methodName);
        }

        public string AppSettingsGetByKey(ConfigKey key, string languageCode)
        {
            string methodName = "AppSettingsGetByKey";
            var jObject = new { key = (int)key, languageCode = languageCode };
            return base.PostData<string>(jObject, methodName);
        }

        #region PushNotification

        public bool PushNotificationSave(PushNotificationRequest pushNotificationRequest)
        {
            string methodName = "PushNotificationSave";
            var jObject = new { pushNotificationRequest = pushNotificationRequest };
            return base.PostData<bool>(jObject, methodName);
        }

        public void PushNotificationDelete(string deviceId)
        {
            string methodName = "PushNotificationDelete";
            var jObject = new { deviceId = deviceId };
            base.PostData<bool>(jObject, methodName);
        }

        #endregion PushNotification
    }
}
