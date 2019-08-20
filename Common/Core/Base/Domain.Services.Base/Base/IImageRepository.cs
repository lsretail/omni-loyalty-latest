using System.Collections.Generic;

using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSRetail.Omni.Domain.Services.Base.Image
{
    public interface IImageRepository
    {
        ImageView ImageGetById(string id, ImageSize imageSize);
    }
}
