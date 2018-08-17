using Android.Views;

namespace Presentation.Util
{
    public enum ItemClickType
    {
        ShoppingListLine = 0,
        Transaction = 1,
        TransactionDetail = 2,
        Coupon = 3,
        Offer = 4,
        Notification = 5,
        Store = 6,
        ItemCategory = 7,
        ProductGroup = 8,
        Item = 9,
        BasketItem = 10,
        ShoppingList = 11
    }

    public interface IItemClickListener
    {
        void ItemClicked(int type, string id, string id2, View view);
    }
}