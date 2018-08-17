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
    public class Card<T>
    {
        public enum CardContentType
        {
            None = 0,
            MenuItem = 1,
            Store = 2,
            Map = 3,
            MenuGroup = 4,
            Stores = 5,
            Menu = 6,
            Transaction = 7,
            TransactionDetail = 8,
            Offer = 9,
            Coupon = 10,
            BasketItem = 11,
            HomeAd = 12,
            QuickAction = 13,
            FavoriteItem = 14,
            FavoriteTransaction = 15,
            FavoriteTransactionDetail = 16,
            Hint = 17,
            LogoPoints = 18,
        }

        public enum CardType
        {
            SmallExtraSmall = 0,
            Small = 1,
            MediumExtraSmall = 2,
            MediumSmall = 3,
            Medium = 4,
            MediumWrapped = 5,
            MediumSingle = 6,
            Single = 7,
            SmallSingle = 8,
        }

        public CardContentType ContentType { get; set; }
        public T Item;
    }

    public class CardHeader<T> : Card<T>
    {
        public string Description { get; set; }
    }

    public class CardItem<T> : Card<T>
    {
        //public T Item { get; set; }
    }
}