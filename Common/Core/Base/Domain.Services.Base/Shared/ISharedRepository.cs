using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base;

namespace LSRetail.Omni.Domain.Services.Base.Loyalty
{
    public interface ISharedRepository
    {
        List<Advertisement> AdvertisementsGetById(string id, string contactId);

        List<PublishedOffer> GetPublishedOffers(string itemId, string cardId);

        string AppSettingsGetByKey(ConfigKey key, string languageCode);

        bool PushNotificationSave(PushNotificationRequest pushNotificationRequest);
        void PushNotificationDelete(string deviceId);

    }
}
