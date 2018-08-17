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

namespace Presentation.Util
{
    public enum ItemType
    {
        None = 0,
        ItemCategory = 1,
        ItemGroup = 2,
        Item = 3,
        Offer = 4,
        Transaction = 5,
        Coupon = 6,
        Notification = 7,
        Store = 8,
        ShoppingList = 9
    }
    public interface IItemClickedListener
    {
        void ItemClicked(string itemId, ItemType itemType = ItemType.None);
    }
}