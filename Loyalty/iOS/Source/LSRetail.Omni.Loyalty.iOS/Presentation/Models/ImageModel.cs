using System;
using UIKit;
using System.Threading.Tasks;
using Presentation.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Models
{
    public class ImageModel : BaseModel
	{
        
        private SharedService sharedService;
        private ISharedRepository iRepo;
       

		public ImageModel ()
		{
            this.iRepo = new SharedRepository();
            this.sharedService = new SharedService(this.iRepo);
		}

		public async void ImageGetById(string id, ImageSize imageSize, Action<ImageView, string> onSuccess, Action onFailure, string destinationId = null)
		{
            // Start with cache, then WS

                 ImageView imageViewFromWS = await ImageGetByIdFromWS(id, imageSize);
                 if (imageViewFromWS != null) 
                 {
				     if (imageViewFromWS != null)
					        onSuccess(imageViewFromWS, destinationId);
				     else
					        onFailure();
				 }
						
	    }
	
		

        private async Task<ImageView> ImageGetByIdFromWS(string id, ImageSize imageSize)
		{
            try
            {
				System.Diagnostics.Debug.WriteLine("IMAGE TO DOWNLOAD: " + id + " THREADID " + System.Threading.Thread.CurrentThread.ManagedThreadId);

                ImageView imageView = await sharedService.ImageGetByIdAsync(id, imageSize);

				return imageView;
            }
			catch (Exception ex)
			{
                base.HandleException(ex, "ImageModel.ImageGetByIdFromWS()", false);
                return null;
				
			}

		}



        public UIImage GetImageByIdFromFile(string id)
		{
            try
            {
                return ImageUtilities.FromFile(id);
            }

			catch (Exception ex)
			{
				base.HandleException(ex, "ImageModel.GetImageByIdFromFile()");
                return null;
			}
			finally
			{
				Utils.UI.StopNetworkActivityIndicator();
			}

			
		}



	}
}

