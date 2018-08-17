using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Domain.Transactions;
using Infrastructure.Data.SQLite2.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Utils;

namespace Presentation.Models
{
    public class BasketModel : BaseModel
    {
        private LocalBasketService basketService;
        private ILocalBasketRepository basketRepository;

        public BasketModel(Context context, IRefreshableActivity refreshableActivity = null) : base(context, refreshableActivity)
        {
            basketService = new LocalBasketService();
            basketRepository = new BasketRepository();
        }

        public void AddItemToBasket(MenuItem item, decimal qty, bool showToast = true)
        {
            AppData.Basket.AddItemToBasket(new BasketItem()
            {
                Item = item,
                Quantity = qty
            });

            CalculateBasket(false);

            SendBroadcast(BroadcastUtils.BasketItemInserted);

            if (showToast)
            {
                ShowToast(Resource.String.BasketItemAdded);
            }
        }

        public void AddSaleLinesToBasket(List<SaleLine> saleLines)
        {
            saleLines.ForEach(saleLine =>
                {
                    AppData.Basket.AddItemToBasket(new BasketItem()
                    {
                        Item = saleLine.Item,
                        Quantity = saleLine.Quantity
                    });
                });

            CalculateBasket(false);

            SendBroadcast(BroadcastUtils.BasketItemInserted);

            ShowToast(Resource.String.BasketItemAdded);
        }

        public void UpdateBasketItem(string basketItemId, MenuItem item, decimal qty)
        {
            var basketItem = AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId);

            basketItem.Item = item;
            basketItem.Quantity = qty;

            CalculateBasket(false);

            SendBroadcast(BroadcastUtils.BasketItemChanged);

            //ShowToast(Resource.String.BasketItemUpdated);
        }

        public void DeleteItem(string basketItemId, bool sendBroadcast = true)
        {
            DeleteItem(AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId), sendBroadcast);
        }

        public bool IsSelected(PublishedOffer publishedOffer)
        {
            return AppData.Basket.PublishedOffers.FirstOrDefault(x => x.Id == publishedOffer.Id) != null;
        }

        public void ToggleCoupon(string id)
        {
            ToggleCoupon(AppData.Contact.PublishedOffers.FirstOrDefault(x => x.Id == id));
        }

        public void ToggleCoupon(PublishedOffer publishedOffer)
        {
            if (AppData.HasBasket)
            {
                var exists = AppData.Basket.PublishedOffers.FirstOrDefault(x => x.Id == publishedOffer.Id);

                if (exists == null)
                {
                    AppData.Basket.AddPublishedOfferToBasket(publishedOffer);
                    ShowToast(Resource.String.BasketCouponAdded);
                }
                else
                {
                    AppData.Basket.RemovePublishedOfferFromBasket(exists);
                    ShowToast(Resource.String.BasketCouponRemoved);
                }
            }
            else
            {
                publishedOffer.Selected = !publishedOffer.Selected;
            }

            SendBroadcast(BroadcastUtils.BasketUpdated);
            SaveBasket(true);
        }

        public void ToggleOffer(PublishedOffer offer)
        {
            if (AppData.HasBasket)
            {
                var exists = AppData.Basket.PublishedOffers.FirstOrDefault(x => x.Id == offer.Id);

                if (exists == null)
                {
                    AppData.Basket.AddPublishedOfferToBasket(offer);
                    ShowToast(Resource.String.BasketOfferAdded);
                }
                else
                {
                    AppData.Basket.AddPublishedOfferToBasket(exists);
                    ShowToast(Resource.String.BasketOfferRemoved);
                }
            }
            else
            {
                offer.Selected = !offer.Selected;
            }

            SendBroadcast(BroadcastUtils.BasketUpdated);
            SaveBasket(true);
        }

        public void ChangeQty(string basketItemId, decimal qty)
        {
            ChangeQty(AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId), qty);
        }

        public void ChangeQty(BasketItem item, decimal qty)
        {
            item.Quantity = qty;

            CalculateBasket(false);

            SendBroadcast(BroadcastUtils.BasketItemChanged);
        }

        public void DeleteItem(BasketItem item, bool sendBroadcast = true)
        {
            AppData.Basket.Items.Remove(item);

            CalculateBasket(false);

            if (sendBroadcast)
            {
                SendBroadcast(BroadcastUtils.BasketItemDeleted);
            }
            else
            {
                SendBroadcast(BroadcastUtils.BasketPriceUpdated);
            }

            ShowToast(Resource.String.BasketItemRemoved);
        }

        public void CalculateBasket(bool sendBroadcast)
        {
            basketService.CalculateBasket(AppData.MobileMenu, AppData.Basket);

            if (sendBroadcast)
            {
                SendBroadcast(BroadcastUtils.BasketUpdated);
            }

            SaveBasket(sendBroadcast);
        }

        public void ClearBasket(bool sendBroadcast)
        {
            AppData.Basket.Clear();

            if (sendBroadcast)
            {
                SendBroadcast(BroadcastUtils.BasketUpdated);
            }

            SaveBasket(sendBroadcast);
        }

        private async void SaveBasket(bool sendBroadcast)
        {
            try
            {
                var basket = await basketService.SyncBasketAsync(basketRepository, AppData.Basket);

                AppData.Basket = basket;
                if (sendBroadcast)
                {
                    SendBroadcast(BroadcastUtils.BasketUpdated);
                }
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }
        }

        protected override void CreateService()
        {
            
        }
    }
}