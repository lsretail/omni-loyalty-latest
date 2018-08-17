using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Android.Content;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.Services.Loyalty.Offers;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Util;

namespace Presentation.Models
{
    public class OfferModel : BaseModel
    {
        private SharedService service;
        private OfferLocalService localService;

        public OfferModel(Context context, IRefreshableActivity refreshActivity = null)
            : base(context, refreshActivity)
        {
            localService = new OfferLocalService(new Infrastructure.Data.SQLite.Offers.OfferRepository());
        }

        public async Task GetOffersByCardId(string cardId)
        {
            ShowIndicator(true);

            BeginWsCall();

            try
            {
                var offers = await service.GetPublishedOffersByCardIdAsync(cardId);

                AppData.Device.UserLoggedOnToDevice.PublishedOffers = offers;

                SendBroadcast(Utils.BroadcastUtils.CouponsUpdated);
                SendBroadcast(Utils.BroadcastUtils.OffersUpdated);

                SaveLocalOffers(offers);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public async Task<List<PublishedOffer>> GetPublishedOffersByItemId(string itemId, string cardId)
        {
            List<PublishedOffer> offers = null;

            ShowIndicator(true);

            BeginWsCall();

            try
            {
                offers = await service.GetPublishedOffersByItemIdAsync(itemId, cardId);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return offers;
        }

        public void ToggleOffer(PublishedOffer offer)
        {
            offer.Selected = !offer.Selected;

            SendBroadcast(Utils.BroadcastUtils.CouponsUpdated);
            SendBroadcast(Utils.BroadcastUtils.OffersUpdated);

            if (offer.Selected)
            {
                ShowToast(Resource.String.OfferViewAddedToQRCode, ToastLength.Short);
            }
        }

        private void SaveLocalOffers(List<PublishedOffer> offers)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += (sender, args) =>
            {
                localService.SavePublishedOffers(offers);
            };

            worker.RunWorkerAsync();
        }

        protected override void CreateService()
        {
            service = new SharedService(new SharedRepository());
        }
    }
}