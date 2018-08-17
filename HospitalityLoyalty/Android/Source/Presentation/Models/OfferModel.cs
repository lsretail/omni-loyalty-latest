using System;
using Android.Content;

using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Utils;

namespace Presentation.Models
{
    public class OfferModel : BaseModel
    {
        private SharedService offerService;

        public OfferModel(Context context, IRefreshableActivity refreshableActivity) : base(context, refreshableActivity)
        {
        }

        protected override void CreateService()
        {
            offerService = new SharedService(new SharedRepository());
        }

        public async void PublishedOffersGetByCardId()
        {
            Show(true);

            BeginWsCall();

            try
            {
                var offers = await offerService.GetPublishedOffersByCardIdAsync(AppData.Contact.Card.Id);

                AppData.Contact.PublishedOffers = offers;
                SendBroadcast(BroadcastUtils.OffersUpdated);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);
        }
    }
}