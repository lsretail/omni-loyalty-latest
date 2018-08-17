using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Presentation.Utils;
using Infrastructure.Data.SQLite2.Devices;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;

namespace Presentation.Models
{
    public class OfferModel : BaseModel
	{
		private SharedService offerService;

		public OfferModel()
		{
			this.offerService = new SharedService(new SharedRepository());
		}

        public async Task<bool> GetPublishedOffersByCardId(string cardId)
		{
            try
            {
                List<PublishedOffer> publishedOffers = await offerService.GetPublishedOffersByCardIdAsync(cardId);

                  AppData.Device.UserLoggedOnToDevice.PublishedOffers = publishedOffers;
                  new DeviceService(new DeviceRepository()).SaveDevice(AppData.Device);

                return true;
                
            }
			catch (Exception ex)
			{
				HandleException(ex, "OfferModel.GetPublishedOffersByCardId()", false);
                return false;
			}
			
		}

		public async Task<List<PublishedOffer>> GetPublishedOffersByItemId(string itemId, string cardId)
		{
            try
            {
                List<PublishedOffer> publishedOffers = await offerService.GetPublishedOffersByItemIdAsync(itemId, cardId);
                return publishedOffers;

             }
			catch (Exception ex)

			{
				HandleException(ex, "OfferModel.GetPublishedOffersByItemId()", false);
                return null;
			}
			
		}
	}
}