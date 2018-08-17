using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSRetail.Omni.Domain.Services.Loyalty.Offers
{
    public interface IOfferLocalRepository
    {
        List<PublishedOffer> GetLocalPublishedOffers();
        void SavePublishedOffers(List<PublishedOffer> publishedOffers);
    }
}
