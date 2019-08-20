using System.Collections.Generic;
using System.Threading.Tasks;

using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSRetail.Omni.Domain.Services.Base.Image
{
    public class ImageService
    {
        private static ImageCache imageCache;
        private IImageRepository repository;

        public ImageService(IImageRepository iRepo)
        {
            repository = iRepo;
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

        #region Async Functions

        public async Task<ImageView> ImageGetByIdAsync(string id, ImageSize imageSize)
        {
            return await Task.Run(() => ImageGetById(id, imageSize));
        }

        #endregion
    }
}
