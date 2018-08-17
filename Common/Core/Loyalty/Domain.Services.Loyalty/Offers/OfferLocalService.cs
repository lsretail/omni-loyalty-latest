using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSRetail.Omni.Domain.Services.Loyalty.Offers
{
    public class OfferLocalService
    {
        private IOfferLocalRepository iRepository;

        public OfferLocalService(IOfferLocalRepository iRepo)
        {
            iRepository = iRepo;
        }

        public List<PublishedOffer> GetLocalPublishedOffers()
        {
            return iRepository.GetLocalPublishedOffers();
        }

        public void SavePublishedOffers(List<PublishedOffer> publishedOffers)
        {
            iRepository.SavePublishedOffers(publishedOffers);
        }
    }
}
