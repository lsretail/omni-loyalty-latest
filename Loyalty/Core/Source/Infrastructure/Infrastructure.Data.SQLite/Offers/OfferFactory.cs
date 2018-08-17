using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Infrastructure.Data.SQLite.Offers
{
    internal class OfferFactory
    {
        public PublishedOffer BuildEntity(OfferData offerData)
        {
            var entity = new PublishedOffer(offerData.OfferId)
                             {
                                 Details = offerData.Details,
                                 ExpirationDate = offerData.ExpiryDate,
                                 Type = (OfferType)offerData.OfferType,
                                 Description = offerData.PrimaryText,
                             };

            XmlSerializer serializer = new XmlSerializer(typeof(List<ImageView>), new Type[] { });

            using (TextReader textReader = new StringReader(offerData.Images))
            {
                //Console.WriteLine(textReader.ToString());

                entity.Images = (List<ImageView>)serializer.Deserialize(textReader);
            }

            return entity;
        }

        public OfferData BuildEntity(PublishedOffer offer)
        {
            var entity = new OfferData()
            {
                OfferId = offer.Id,
                Details = offer.Details,
                ExpiryDate = offer.ExpirationDate,
                OfferType = (int)offer.Type,
                PrimaryText = offer.Description,
            };

            XmlSerializer serializer = new XmlSerializer(typeof(List<ImageView>), new Type[] { });

            using (var textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, offer.Images);
                entity.Images = textWriter.ToString();
            }

            return entity;
        }
    }
}
