using System.Collections.Generic;
using System.Linq;

using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.Services.Loyalty.Offers;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Infrastructure.Data.SQLite.Offers
{
    public class OfferRepository : IOfferLocalRepository
    {
        private object locker = new object();

        public OfferRepository()
        {
            DBHelper.OpenDBConnection();
        }

        public List<PublishedOffer> GetLocalPublishedOffers()
        {
            lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                var factory = new OfferFactory();
                var offers = new List<PublishedOffer>();

                DBHelper.DBConnection.Table<OfferData>().ToList().ForEach(x => offers.Add(factory.BuildEntity(x)));

                return offers;
            }
        }

        public void SavePublishedOffers(List<PublishedOffer> offers)
        {
            lock (locker)
            {
                var facotry = new OfferFactory();

                var offersData = new List<OfferData>();
                offers.ForEach(x => offersData.Add(facotry.BuildEntity(x)));

                DBHelper.DBConnection.DeleteAll<OfferData>();
                DBHelper.DBConnection.InsertAll(offersData);
            }
        }
    }
}
