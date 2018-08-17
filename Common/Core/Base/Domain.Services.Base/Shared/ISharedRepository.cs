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

        ImageView ImageGetById(string id, ImageSize imageSize);

        List<PublishedOffer> GetPublishedOffers(string itemId, string cardId);

        OmniEnvironment GetEnvironment();

        string AppSettingsGetByKey(AppSettingsKey key, string languageCode);

        bool PushNotificationSave(PushNotificationRequest pushNotificationRequest);
        void PushNotificationDelete(string deviceId);

    }
}
