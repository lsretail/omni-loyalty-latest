using System;
using System.Linq;

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Activities.Items;
using Presentation.Activities.Search;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Presentation.Views;

namespace Presentation.Activities.ShoppingLists
{
    public class WishListFragment : LoyaltyFragment, IBroadcastObserver, IRefreshableActivity, IItemClickListener, SwipeRefreshLayout.IOnRefreshListener, PopupMenu.IOnMenuItemClickListener
    {
        private ShoppingListModel model;
        private BasketModel basketModel;

        private LoyaltyRecyclerView shoppingListRecyclerView;
        private ShoppingListDetailAdapter shoppingListAdapter;
        private SwipeRefreshLayout shoppingLeftRefreshLayout;
        private ItemTouchHelper itemTouchHelper;

        private string popupMenuItemId;

        public OneList WishList { get; private set; }

        public static WishListFragment NewInstance()
        {
            var shoppingListDetail = new WishListFragment() { Arguments = new Bundle() };
            return shoppingListDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            model = new ShoppingListModel(Activity, this);
            basketModel = new BasketModel(Activity, null);

            LoadShoppingList();

            HasOptionsMenu = true;

            var isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.WishList);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.WishListScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            shoppingListRecyclerView = view.FindViewById<LoyaltyRecyclerView>(Resource.Id.WishListScreenList);
            shoppingListRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            shoppingListRecyclerView.HasFixedSize = true;

            if (isBigScreen)
            {
                shoppingListRecyclerView.SetLayoutManager(new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.CardColumns)));
            }
            else
            {
                shoppingListRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
            }

            shoppingListAdapter = new ShoppingListDetailAdapter(Activity, this, CreatePopUpMenu);
            shoppingListAdapter.SetWishListLines(WishList.Items);

            shoppingListRecyclerView.SetAdapter(shoppingListAdapter);
            shoppingListRecyclerView.SetEmptyView(view.FindViewById<View>(Resource.Id.WishListScreenEmptyView));

            shoppingLeftRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.WishListScreenListRefreshContainer);
            shoppingLeftRefreshLayout.SetColorSchemeResources(Resource.Color.accent);
            shoppingLeftRefreshLayout.SetOnRefreshListener(this);

            itemTouchHelper = new ItemTouchHelper(new WishListItemSwipeCallback(
                position =>
                {
                    DeleteShoppingListItem(WishList.Items[position]);
                },
                (position) =>
                {
                    AddItemToBasket(WishList.Items[position], true);
                },
                (from, to) =>
                {
                    var item = WishList.Items[from];

                    WishList.Items.Remove(item);

                    WishList.Items.Insert(to, item);

                    shoppingListAdapter.NotifyItemMoved(from, to);
                }, async () =>
                {
                    await model.WishListSave(WishList);
                },
                enabled =>
                {
                    shoppingLeftRefreshLayout.Enabled = enabled;
                }));

            itemTouchHelper.AttachToRecyclerView(shoppingListRecyclerView);

            return view;
        }

        private void LoadShoppingList()
        {
            WishList = AppData.Device.UserLoggedOnToDevice.GetWishList(AppData.Device.CardId);
        }

        public override void OnResume()
        {
            base.OnResume();

            UpdateShoppingList();

            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).AddObserver(this);
            }
        }

        public override void OnPause()
        {
            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public override void OnDestroyView()
        {
            model.Stop();
            base.OnDestroyView();
        }

        public void UpdateShoppingList()
        {
            LoadShoppingList();
            shoppingListAdapter.SetWishListLines(WishList.Items);
        }

        public async void OnRefresh()
        {
            await model.GetShoppingListsByCardId(AppData.Device.CardId);
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var shoppingListLine = WishList.Items.FirstOrDefault(x => x.Id == id);

            GoToItemDetail(shoppingListLine);
        }

        private void GoToItemDetail(OneListItem line)
        {
            if (!EnabledItems.HasItemCatalog)
            {
                return;
            }

            var intent = new Intent();
            intent.SetClass(Activity, typeof(ItemActivity));
            intent.PutExtra(BundleConstants.ItemId, line.ItemId);
            if (string.IsNullOrEmpty(line.UnitOfMeasureId) == false)
                intent.PutExtra(BundleConstants.SelectedUomId, line.UnitOfMeasureId);
            if (string.IsNullOrEmpty(line.VariantId) == false)
                intent.PutExtra(BundleConstants.SelectedVariantId, line.VariantId);

            if (AppData.IsDualScreen)
                intent.PutExtra(BundleConstants.LoadContainer, true);

            StartActivity(intent);
        }

        #region context menu

        public void CreatePopUpMenu(string itemId, View anchorView)
        {
            popupMenuItemId = itemId;

            var popup = new Android.Support.V7.Widget.PopupMenu(Activity, anchorView);
            popup.MenuInflater.Inflate(Resource.Menu.ShoppingListDetailCABMenu, popup.Menu);

            popup.Menu.RemoveItem(Resource.Id.MenuViewItemInformation);
            popup.Menu.RemoveItem(Resource.Id.MenuViewChangeQty);
            popup.Menu.RemoveItem(Resource.Id.MenuViewAddToAnotherList);

            if (!EnabledItems.HasItemCatalog)
            {
                popup.Menu.RemoveItem(Resource.Id.MenuViewItemInformation);
            }

            if (!EnabledItems.HasBasket)
            {
                popup.Menu.RemoveItem(Resource.Id.MenuViewAddToBasket);
            }

            popup.SetOnMenuItemClickListener(this);
            popup.Show();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            var shoppingListLine = WishList.Items.FirstOrDefault(x => x.Id == popupMenuItemId);

            switch (item.ItemId)
            {
                case Resource.Id.MenuViewItemInformation:
                    GoToItemDetail(shoppingListLine);
                    break;

                case Resource.Id.MenuViewDeleteItemFromList:
                    DeleteShoppingListItem(shoppingListLine);
                    break;

                case Resource.Id.MenuViewAddToBasket:
                    AddItemToBasket(shoppingListLine);
                    break;
            }

            return false;
        }

        #endregion

        private async void AddItemToBasket(OneListItem wishListItem, bool refreshList = false)
        {
            // Get the last data for the selected item, including its price
            var item = await new Models.ItemModel(Activity).GetItemById(wishListItem.ItemId);
            wishListItem.Price = item.AmtFromVariantsAndUOM(wishListItem.VariantId, wishListItem.UnitOfMeasureId);

            await basketModel.AddItemToBasket(wishListItem);

            if (refreshList)
            {
                shoppingListAdapter.NotifyDataSetChanged();
            }
        }

        private async void DeleteShoppingListItem(OneListItem line)
        {
            await model.DeleteWishListLine(line.Id);
        }

        public void ShowIndicator(bool show)
        {
            shoppingLeftRefreshLayout.Refreshing = show;
        }

        #region menu

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.GenericSearchMenu, menu);
            inflater.Inflate(Resource.Menu.ShoppingListDetailMenu, menu);

            var addItem = menu.FindItem(Resource.Id.MenuViewAddToShoppingList);
            var deleteItem = menu.FindItem(Resource.Id.MenuViewDelete);

            menu.RemoveItem(addItem.ItemId);

            deleteItem.SetTitle(Resource.String.MenuViewClearTitle);

            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewDelete:
                    model.DeleteWishList();
                    return true;

                case Resource.Id.MenuViewSearch:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(GeneralSearchActivity));
                    intent.PutExtra(BundleConstants.SearchType, (int)SearchType.OneList);

                    StartActivity(intent);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        private class WishListItemSwipeCallback : ItemTouchHelper.Callback
        {
            private readonly Action<int> deleteWishListItem;
            private readonly Action<int> addWishListItemToBasket;
            private readonly Action<int, int> moveWishListItem;
            private readonly Action saveWishList;
            private readonly Action<bool> setRefreshEnabled;

            private bool itemMoved = false;

            public WishListItemSwipeCallback(Action<int> deleteWishListItem, Action<int> addWishListItemToBasket, Action<int, int> moveWishListItem, Action saveWishList, Action<bool> setRefreshEnabled)
            {
                this.deleteWishListItem = deleteWishListItem;
                this.addWishListItemToBasket = addWishListItemToBasket;
                this.moveWishListItem = moveWishListItem;
                this.setRefreshEnabled = setRefreshEnabled;
                this.saveWishList = saveWishList;
            }

            public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                var swipeDirection = ItemTouchHelper.Start;

                if (EnabledItems.HasBasket)
                {
                    swipeDirection |= ItemTouchHelper.End;
                }

                return MakeMovementFlags(ItemTouchHelper.Up | ItemTouchHelper.Down, swipeDirection);
            }

            public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewholder, RecyclerView.ViewHolder target)
            {
                moveWishListItem(viewholder.AdapterPosition, target.AdapterPosition);

                itemMoved = true;

                return true;
            }

            public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
            {
                base.OnSelectedChanged(viewHolder, actionState);

                var dragging = actionState == ItemTouchHelper.ActionStateDrag;
                var swiping = actionState == ItemTouchHelper.ActionStateSwipe;
                setRefreshEnabled(!dragging && !swiping);

                if (!dragging && itemMoved)
                {
                    itemMoved = false;

                    saveWishList();
                }
            }

            public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
            {
                if (actionState == ItemTouchHelper.ActionStateSwipe)
                {
                    // Get RecyclerView item from the ViewHolder
                    View itemView = viewHolder.ItemView;

                    Paint p = new Paint();
                    if (dX > 0)
                    {
                        /* Set your color for positive displacement */
                        p.Color = new Color(ContextCompat.GetColor(viewHolder.ItemView.Context, Resource.Color.positive));

                        // Draw Rect with varying right side, equal to displacement dX
                        c.DrawRect((float)itemView.Left, (float)itemView.Top, dX,
                                (float)itemView.Bottom, p);

                        var bitmap = BitmapFactory.DecodeResource(viewHolder.ItemView.Resources, Resource.Drawable.ic_add_shopping_cart_white_24dp);

                        float height = (itemView.Height / 2) - (bitmap.Height / 2);
                        float bitmapWidth = bitmap.Width;

                        c.DrawBitmap(bitmap, (float)itemView.Left + viewHolder.ItemView.Resources.GetDimensionPixelSize(Resource.Dimension.BasePadding), (float)itemView.Top + height, null);
                    }
                    else
                    {
                        /* Set your color for negative displacement */
                        p.Color = new Color(ContextCompat.GetColor(viewHolder.ItemView.Context, Resource.Color.negative));

                        // Draw Rect with varying left side, equal to the item's right side plus negative displacement dX
                        c.DrawRect((float)itemView.Right + dX, (float)itemView.Top,
                                (float)itemView.Right, (float)itemView.Bottom, p);

                        var bitmap = BitmapFactory.DecodeResource(viewHolder.ItemView.Resources, Resource.Drawable.ic_delete_white_24dp);

                        float height = (itemView.Height / 2) - (bitmap.Height / 2);
                        float bitmapWidth = bitmap.Width;

                        c.DrawBitmap(bitmap, (float)itemView.Right - viewHolder.ItemView.Resources.GetDimensionPixelSize(Resource.Dimension.BasePadding) - bitmapWidth, (float)itemView.Top + height, null);
                    }
                }

                base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
            }

            public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int swipeDirection)
            {
                if (swipeDirection == ItemTouchHelper.Start)
                {
                    deleteWishListItem(viewHolder.AdapterPosition);
                }
                else if (swipeDirection == ItemTouchHelper.End)
                {
                    addWishListItemToBasket(viewHolder.AdapterPosition);
                }
            }
        }

        public void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.DomainModelUpdated:
                case Utils.BroadcastUtils.ShoppingListUpdated:
                case Utils.BroadcastUtils.ShoppingListsUpdated:
                case Utils.BroadcastUtils.ShoppingListDeleted:
                    UpdateShoppingList();
                    break;
            }
        }
    }
}