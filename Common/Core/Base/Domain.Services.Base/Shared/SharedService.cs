using System.Collections.Generic;
using System.Threading.Tasks;

using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base;

namespace LSRetail.Omni.Domain.Services.Base.Loyalty
{
    public class SharedService
    {
        private static ImageCache imageCache;
        private ISharedRepository repository;

        public SharedService(ISharedRepository iRepo)
        {
            repository = iRepo;
        }

        private static void AddImageToCache(string itemId, ImageSize imageSize, List<ImageView> itemImages)
        {
            if (imageCache == null)
            {
                imageCache = new ImageCache();
            }

            imageCache.AddImageToCache(new ImageCache.ImageCacheItem() { ImageSize = imageSize, VariantId = string.Empty, ItemId = itemId, Images = itemImages });
        }

        private static void AddImageToCache(string itemId, string variantId, ImageSize imageSize, ImageView itemImage)
        {
            if (imageCache == null)
            {
                imageCache = new ImageCache();
            }

            imageCache.AddImageToCache(new ImageCache.ImageCacheItem() { ImageSize = imageSize, VariantId = variantId, ItemId = itemId, Images = new List<ImageView>() { itemImage } });
        }

        private static List<ImageView> GetItemFromCache(string itemId, string variantId, ImageSize imageSize)
        {
            if (imageCache == null)
            {
                imageCache = new ImageCache();
            }

            return imageCache.GetItemFromCache(itemId, variantId, imageSize);
        }

        public static void ClearImageCache()
        {
            imageCache?.Clear();
        }


        public List<PublishedOffer> GetPublishedOffersByCardId(string cardId)
        {
            return repository.GetPublishedOffers(cardId, string.Empty);
        }

        public List<PublishedOffer> GetPublishedOffersByItemId(string itemId, string cardId)
        {
            return repository.GetPublishedOffers(cardId, itemId);
        }


        public List<Advertisement> AdvertisementsGetById(string id, string contactId)
        {
            return repository.AdvertisementsGetById(id, contactId);
        }

        public ImageView ImageGetById(string id, ImageSize imageSize)
        {
            var cachedImages = GetItemFromCache(id, string.Empty, imageSize);

            if (cachedImages?.Count > 0)
            {
                cachedImages[0].Crossfade = false;
                return cachedImages[0];
            }
            else
            {
                var images = repository.ImageGetById(id, imageSize);
                AddImageToCache(id, string.Empty, imageSize, images);
                images.Crossfade = true;
                return images;
            }
        }

        public OmniEnvironment GetEnvironment()
        {
            return repository.GetEnvironment();
        }

        public string AppSettings(ConfigKey key, string languageCode)
        {
            return repository.AppSettingsGetByKey(key, languageCode);
        }

        public bool PushNotificationSave(PushNotificationRequest pushNotificationRequest)
        {
            return repository.PushNotificationSave(pushNotificationRequest);
        }

        public void PushNotificationDelete(string deviceId)
        {
            repository.PushNotificationDelete(deviceId);
        }

        #region Async Functions

        public async Task<List<Advertisement>> AdvertisementsGetByIdAsync(string id, string contactId)
        {
            return await Task.Run(() => AdvertisementsGetById(id, contactId));
        }

        public async Task<ImageView> ImageGetByIdAsync(string id, ImageSize imageSize)
        {
            return await Task.Run(() => ImageGetById(id, imageSize));
        }

        public async Task<List<PublishedOffer>> GetPublishedOffersByCardIdAsync(string cardId)
        {
            return await Task.Run(() => GetPublishedOffersByCardId(cardId));
        }

        public async Task<List<PublishedOffer>> GetPublishedOffersByItemIdAsync(string itemId, string cardId)
        {
            return await Task.Run(() => GetPublishedOffersByItemId(itemId, cardId));
        }

        public async Task<OmniEnvironment> GetEnvironmentAsync()
        {
            return await Task.Run(() => GetEnvironment());
        }

        public async Task<string> AppSettingsAsync(ConfigKey key, string languageCode)
        {
            return await Task.Run(() => AppSettings(key, languageCode));
        }

        public async Task<bool> PushNotificationSaveAsync(PushNotificationRequest pushNotificationRequest)
        {
            return await Task.Run(() => PushNotificationSave(pushNotificationRequest));
        }

        public async Task PushNotificationDeleteAsync(string deviceId)
        {
            await Task.Run(() => PushNotificationDelete(deviceId));
        }

        #endregion
    }
}
