using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Presentation.Utils
{
    public class BroadcastUtils
    {
        //public static readonly string ItemAddedToBasket = "presentation.utils.itemaddedtobasket";
        public static readonly string ItemPriceChanged = "presentation.utils.itempricechanged";
        public static readonly string ContactUpdated = "presentation.utils.contactupdated";
        public static readonly string ContactPointsUpdated = "presentation.utils.contactPointsUpdated";
        public static readonly string CouponsUpdated = "presentation.utils.couponsUpdated";
        public static readonly string OffersUpdated = "presentation.utils.offersUpdated";
        public static readonly string AdsUpdated = "presentation.utils.adsUpdated";
        public static readonly string TransactionsUpdated = "presentation.utils.transactionsUpdated";
        public static readonly string MenuUpdated = "presentation.utils.menuUpdated";
        public static readonly string DrawerOpened = "presentation.utils.drawerOpened";
        public static readonly string DrawerClosed = "presentation.utils.drawerClosed";

        public static readonly string BasketUpdated = "presentation.utils.basketUpdated";
        public static readonly string BasketItemInserted = "presentation.utils.basketItemInserted";
        public static readonly string BasketItemDeleted = "presentation.utils.basketItemDeleted";
        public static readonly string BasketItemChanged = "presentation.utils.basketItemChanged";
        public static readonly string BasketPriceUpdated = "presentation.utils.basketPriceUpdated";

        public static readonly string FavoritesUpdated = "presentation.utils.favoritesUpdated";
        public static readonly string FavoritesUpdatedInList = "presentation.utils.favoritesUpdatedInList";

        public static readonly string[] BroadcastActions =
            {
                ItemPriceChanged, 
                ContactUpdated, 
                ContactPointsUpdated,
                CouponsUpdated, 
                OffersUpdated,
                AdsUpdated, 
                TransactionsUpdated, 
                MenuUpdated, 
                DrawerOpened,
                DrawerClosed,
                BasketUpdated, 
                BasketItemInserted, 
                BasketItemDeleted, 
                BasketItemChanged, 
                BasketPriceUpdated,
                FavoritesUpdated,
                FavoritesUpdatedInList
            };

        public static void SendBroadcast(Context context, string action)
        {
            var intent = new Intent();
            intent.SetAction(action);

            context.SendBroadcast(intent);
        }
    }
}