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
    public enum ItemType
    {
        Item = 0,
        Store = 1,
        Map = 2,
        MenuGroup = 3,
        Menu = 4,
        Stores = 5,
        HomeItem = 6,
        Transaction = 7,
        Transactions = 8,
        TransactionDetail = 9,
        Coupon = 10,
        AddToBasket = 11,
        UpdateBasket = 12,
        AddAllToBasket = 13,
        Offer = 14,
        Delete = 15,
        Checkout = 16
    }
}