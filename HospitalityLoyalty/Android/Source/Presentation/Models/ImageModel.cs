using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Utils;
using ImageView = LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView;

namespace Presentation.Models
{
    public class ImageModel : BaseModel
    {
        private SharedService service;

        public ImageModel(Context context, IRefreshableActivity refreshableActivity = null)
            : base(context, refreshableActivity)
        {

        }

        protected override void CreateService()
        {
            service = new SharedService(new SharedRepository());
        }

        public async Task<ImageView> ImageGetById(string id, ImageSize imageSize)
        {
            if (imageSize == null)
            {
                imageSize = new ImageSize();
            }

            if (imageSize.Height == 0)
            {
                imageSize.Height = Int32.MaxValue;
            }

            if (imageSize.Width == 0)
            {
                imageSize.Width = Int32.MaxValue;
            }

            BeginWsCall();

            ImageView image = null;

            try
            {
                image = await service.ImageGetByIdAsync(id, imageSize);
            }

            catch (Exception)
            {
                //supress
            }

            return image;
        }
    }
}