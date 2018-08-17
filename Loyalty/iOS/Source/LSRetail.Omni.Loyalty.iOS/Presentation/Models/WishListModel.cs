using System;
using Presentation.Utils;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation.Models
{
    public class WishListModel : OneListModel
    {
        public WishListModel()
        {
        }

        public async void AddItemToWishList(decimal quantity, LoyItem item, string itemVariant, string itemUOM, Action onSuccess, Action onFailure)
        {
            var oneList = AppData.Device.UserLoggedOnToDevice.WishList;
            var itemToAdd = new OneListItem(item, quantity, itemUOM, itemVariant);
            oneList.AddItem(itemToAdd);

            try
            {
                oneList.CalculateBasket();
                OneList returnedList = await OneListSave(oneList, false);
                if (returnedList != null)
                {
                    // Successfully synced with BO
                    AppData.Device.UserLoggedOnToDevice.WishList = returnedList;
                    if (onSuccess != null)
                        onSuccess();
                }
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "WishListModel.AddItemToWishList()", false);
                if (onFailure != null)
                    onFailure();
            }
        }

        public async Task<bool> RemoveItemFromWishList(int position)
        {
            var oneList = AppData.Device.UserLoggedOnToDevice.WishList;
            oneList.RemoveItemAtPosition(position);

            try
            {
                OneList returnedList = await OneListSave(oneList, false);
                if (returnedList != null)
                {
                    // Successfully synced with BO
                    AppData.Device.UserLoggedOnToDevice.WishList = returnedList;
                }
                return true;
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "WishListModel.RemoveItemFromWishList()", false);
                return false;
            }
        }

        public async void ClearWishList(Action onSuccess, Action onFailure)
        {
            var oneList = AppData.Device.UserLoggedOnToDevice.WishList;
            try
            {
                bool success = await OneListDeleteById(oneList.Id, ListType.Wish);

                // Successfully synced with BO
                if (success)
                    AppData.Device.UserLoggedOnToDevice.WishList.Clear();

                if (onSuccess != null)
                    onSuccess();
            }
            catch (Exception ex)
            {
                // Failed to sync with BO
                HandleException(ex, "WishListModel.ClearWishList()", false);

                if (onFailure != null)
                    onFailure();
            }
        }
    }
}

