using System;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Image;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Base
{
    public class ImageRepository : BaseRepository, IImageRepository
    {
        public ImageView ImageGetById(string id, ImageSize imageSize)
        {
            string methodName = "ImageGetById";
            var jObject = new { id = id, imageSize = imageSize };
            return base.PostData<ImageView>(jObject, methodName);
        }
    }
}
