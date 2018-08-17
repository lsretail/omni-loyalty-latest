using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;

namespace Presentation.Models
{
	public class OfferModel : BaseModel
	{
		private SharedService offerService;

		public OfferModel()
		{
			offerService = new SharedService(new SharedRepository());
		}

		public async Task<List<PublishedOffer>> GetPublishedOffersByCardId(string cardId)
		{
			try
			{
				return await offerService.GetPublishedOffersByCardIdAsync(cardId);
			}
			catch (Exception exception)
			{
				HandleException(exception, "OfferModel.GetPublishedOffersByCardId()", false);
				return null;
			}
		}

		public async Task<List<PublishedOffer>> GetPublishedOffersByItemId(string itemId, string cardId)
		{
			try
			{
				return await offerService.GetPublishedOffersByItemIdAsync(itemId, cardId);
			}
			catch (Exception exception)
			{
				HandleException(exception, "OfferModel.GetPublishedOffersByItemId()", false);
				return null;
			}
		}
	}
}