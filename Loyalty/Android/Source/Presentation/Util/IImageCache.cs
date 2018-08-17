
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Util
{
    interface IImageCache
    {
        void AddImageToCache(ImageView imageView);
        ImageView GetItemFromCache(string id, ImageSize imageSize);
    }
}