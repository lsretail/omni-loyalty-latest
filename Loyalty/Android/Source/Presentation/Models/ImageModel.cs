using System;
using System.Threading.Tasks;
using Android.Content;

using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Image;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace Presentation.Models
{
    public class ImageModel : BaseModel
    {
        private readonly IRefreshableActivity refreshableActivity;
        private ImageService service;

        public ImageModel(Context context, IRefreshableActivity refreshableActivity) : base(context)
        {
            this.refreshableActivity = refreshableActivity;
        }

        public async Task<ImageView> ImageGetById(string id, ImageSize imageSize)
        {
            ImageView imageView = null;

            if (imageSize.Height == 0)
            {
                imageSize.Height = Int32.MaxValue;
            }

            if (imageSize.Width == 0)
            {
                imageSize.Width = Int32.MaxValue;
            }
            
            BeginWsCall();

            try
            {
                imageView = await service.ImageGetByIdAsync(id, imageSize);
            }
            catch (Exception)
            {
                //supress
            }

            return imageView;
        }

        protected override void CreateService()
        {
            service = new ImageService(new ImageRepository());
        }
    }
}
