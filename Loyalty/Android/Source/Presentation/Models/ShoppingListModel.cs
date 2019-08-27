using System;
using System.Linq;
using System.Threading.Tasks;

using Android.Content;
using Presentation.Dialogs;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation.Models
{
    public class ShoppingListModel : OneListModel
    {
        public ShoppingListModel(Context context, IRefreshableActivity refreshableActivity = null) : base(context, refreshableActivity)
        {
        }

        public bool ItemIsInWishList(LoyItem item, VariantRegistration variant, UnitOfMeasure uom)
        {
            if (AppData.Device.UserLoggedOnToDevice == null)
                return false;

            var existingItem = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).ItemGetByIds(item.Id, variant?.Id, uom?.Id);

            return existingItem != null;
        }

        public async Task WishListSave(OneList wishList)
        {
            ShowIndicator(true);

            BeginWsCall();

            try
            {
                var list = await OneListSave(wishList, false);

                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Wish);

                SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public async Task AddItemToWishList(OneListItem line, int index = 0)
        {
            ShowIndicator(true);

            BeginWsCall();

            var newList = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Clone();
            newList.Items.Insert(index, line);

            try
            {
                var list = await OneListSave(newList, false);

                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Wish);

                SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public async Task GetShoppingListsByCardId(string cardId)
        {
            ShowIndicator(true);

            try
            {
                var list = await OneListGetByCardId(cardId, ListType.Wish, true);

                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list.FirstOrDefault(), ListType.Wish);

                SendBroadcast(Utils.BroadcastUtils.ShoppingListsUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
            finally
            {
                ShowIndicator(false);
            }
        }

        public async Task DeleteWishListLine(string wishListLineId)
        {
            ShowIndicator(true);

            BeginWsCall();
            
            var newList = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Clone();

            var deletedItem = newList.Items.FirstOrDefault(x => x.Id == wishListLineId);
            var deletedItemIndex = newList.Items.IndexOf(deletedItem);

            newList.Items.RemoveAll(x => x.Id == wishListLineId);

            try
            {
                var list = await OneListSave(newList, false);

                AppData.Device.UserLoggedOnToDevice.AddList(AppData.Device.CardId, list, ListType.Wish);

                SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated);

                if (deletedItem != null)
                {
                    ShowSnackbar(
                        AddSnackbarAction(
                            CreateSnackbar(
                                Context.GetString(Resource.String.ApplicatioItemDeleted)),
                            Context.GetString(Resource.String.ApplicationUndo), async view =>
                            {
                                await AddItemToWishList(deletedItem, deletedItemIndex);
                            }));
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
            finally
            {
                ShowIndicator(false);
            }
        }

        public void ClearWishListWithConfirmation()
        {
            var message = Context.GetString(Resource.String.ShoppingListViewClearConfirmation);

            var dialog = new WarningDialog(Context, Context.GetString(Resource.String.ShoppingListDetailViewWishlist))
                                .SetPositiveButton(Context.GetString(Resource.String.ApplicationYes),
                                                    () => DeleteWishList());
            dialog.Message = message;
            dialog.SetNegativeButton(Context.GetString(Resource.String.ApplicationNo), delegate() { });
            dialog.Show();
        }

        public async void DeleteWishList()
        {
            if (string.IsNullOrEmpty(AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Id))
            {
                AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Items.Clear();
                return;
            }

            ShowIndicator(true);

            BeginWsCall();

            try
            {
                var success = await this.OneListDeleteById(AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId).Id);

                if (success)
                {
                    SendBroadcast(Utils.BroadcastUtils.ShoppingListUpdated);
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }
    }
}