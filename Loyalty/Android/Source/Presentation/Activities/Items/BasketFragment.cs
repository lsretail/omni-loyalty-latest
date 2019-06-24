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
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.Checkout;
using Presentation.Activities.Home;
using Presentation.Adapters;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using Presentation.Views;
using ColoredButton = Presentation.Views.ColoredButton;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Utils = Presentation.Util.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation.Activities.Items
{
    public class BasketFragment : LoyaltyFragment, IBroadcastObserver, View.IOnClickListener, Toolbar.IOnMenuItemClickListener, IItemClickListener, SwipeRefreshLayout.IOnRefreshListener, IRefreshableActivity
    {
        private LoyaltyRecyclerView basketRecyclerView;
        private BasketAdapter adapter;
        private SwipeRefreshLayout basketRefreshLayout;
        private TextView total;
        private ProgressBar totalProgress;
        private BasketModel basketModel;
        private Toolbar toolbar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.BasketScreen);

            basketModel = new BasketModel(Activity, this);

            HasOptionsMenu = true;

            basketRecyclerView = view.FindViewById<LoyaltyRecyclerView>(Resource.Id.BasketScreenList);
            basketRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            basketRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            basketRecyclerView.HasFixedSize = true;

            adapter = new BasketAdapter(this);
            basketRecyclerView.SetAdapter(adapter);

            basketRecyclerView.SetEmptyView(view.FindViewById<View>(Resource.Id.BasketScreenEmptyView));

            basketRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.BasketScreenListRefreshContainer);
            basketRefreshLayout.SetColorSchemeResources(Resource.Color.accent);
            basketRefreshLayout.SetOnRefreshListener(this);

            view.FindViewById<ColoredButton>(Resource.Id.BasketScreenStartOrdering).SetOnClickListener(this);
            view.FindViewById<ColoredButton>(Resource.Id.BasketScreenCheckout).SetOnClickListener(this);
            toolbar = view.FindViewById<Toolbar>(Resource.Id.BasketScreenToolbar);
            toolbar.Title = GetString(Resource.String.ApplicationBasket);
            toolbar.InflateMenu(Resource.Menu.ClearBasketMenu);
            toolbar.SetOnMenuItemClickListener(this);

            var itemTouchHelper = new ItemTouchHelper(new BasketItemSwipeCallback(
                position =>
                {
                    DeleteItem(position);
                },
                enabled =>
                {
                    basketRefreshLayout.Enabled = enabled;
                }));

            itemTouchHelper.AttachToRecyclerView(basketRecyclerView);

            total = view.FindViewById<TextView>(Resource.Id.BasketScreenTotal);
            totalProgress = view.FindViewById<ProgressBar>(Resource.Id.BasketScreenTotalProgressBar);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
                (Activity as LoyaltyFragmentActivity).AddObserver(this);

            adapter.SetBasketItems(AppData.Basket.Items);

            UpdateTotal();

            UpdateMenu();
        }

        public override void OnPause()
        {
            if (Activity is LoyaltyFragmentActivity)
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);

            base.OnPause();
        }

        public void BroadcastReceived(string action)
        {
            if (action == Utils.BroadcastUtils.BasketStateUpdated || action == Utils.BroadcastUtils.DomainModelUpdated)
            {
                adapter.SetBasketItems(AppData.Basket.Items);

                if (AppData.Basket.State == BasketState.Updating)
                {
                    ShowIndicator(true);
                }
                else
                {
                    ShowIndicator(false);
                }

                UpdateTotal();

                UpdateMenu();
            }
            else if (action == Utils.BroadcastUtils.OpenBasket)
            {
                if (Activity is LoyaltyFragmentActivity && (Activity as LoyaltyFragmentActivity).HasRightDrawer)
                    (Activity as LoyaltyFragmentActivity).OpenDrawer((int)GravityFlags.End);
            }
        }

        private void UpdateTotal()
        {
            if (AppData.Basket.State == BasketState.Calculating)
            {
                total.Visibility = ViewStates.Gone;
                totalProgress.Visibility = ViewStates.Visible;
            }
            else
            {
                totalProgress.Visibility = ViewStates.Gone;
                total.Visibility = ViewStates.Visible;

                if (AppData.Basket.State == BasketState.Dirty)
                {
                    total.Text = "~" + AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.TotalAmount); 
                }
                else
                {
                    total.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.TotalAmount);
                }
            }
        }

        private void UpdateMenu()
        {
            toolbar.Menu.FindItem(Resource.Id.MenuViewClearBasket).SetVisible(!AppData.Basket.IsEmpty);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            inflater.Inflate(Resource.Menu.BasketMenu, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewBasket:
                    if (Activity is LoyaltyFragmentActivity && (Activity as LoyaltyFragmentActivity).HasRightDrawer)
                    {
                        if ((Activity as LoyaltyFragmentActivity).IsOpen((int)GravityFlags.End))
                        {
                            (Activity as LoyaltyFragmentActivity).CloseDrawer((int)GravityFlags.End);
                        }
                        else
                        {
                            (Activity as LoyaltyFragmentActivity).OpenDrawer((int)GravityFlags.End);
                        }
                    }

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.BasketScreenStartOrdering:
                    if (Activity is HomeActivity)
                    {
                        (Activity as HomeActivity).SelectItem(LoyaltyFragmentActivity.ActivityTypes.Items);
                        (Activity as HomeActivity).CloseDrawer((int)GravityFlags.End);
                    }
                    else
                    {
                        var upIntent = new Intent();
                        upIntent.SetClass(Activity, typeof(HomeActivity));
                        upIntent.AddFlags(ActivityFlags.ClearTop);
                        upIntent.AddFlags(ActivityFlags.SingleTop);
                        upIntent.PutExtra(BundleConstants.ChosenMenuBundleName, LoyaltyFragmentActivity.ActivityTypes.Items);

                        StartActivity(upIntent);

                        Activity.Finish();
                    }
                    break;

                case Resource.Id.BasketScreenCheckout:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(CheckoutActivity));

                    StartActivity(intent);
                    break;
            }
        }

        public async void OnRefresh()
        {
            await basketModel.GetBasketByCardId(AppData.Device.CardId);
        }

        public void ShowIndicator(bool show)
        {
            basketRefreshLayout.Refreshing = show;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewClearBasket:
                    var dialog = new WarningDialog(Activity, string.Empty);
                    dialog.Message = GetString(Resource.String.BasketViewClearBasketPrompt);
                    dialog.SetPositiveButton(GetString(Resource.String.ApplicationYes), async () => { await basketModel.ClearBasket(); adapter.SetBasketItems(AppData.Basket.Items);});
                    dialog.SetNegativeButton(GetString(Resource.String.ApplicationNo), () => { });
                    dialog.Show();
                    return true;
            }

            return false;
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var item = AppData.Basket.Items.FirstOrDefault(x => x.Id == id);

            var editDialog = new VariantDialog(Activity, item, (variant, arg2) => { });
            editDialog.Show();
        }

        private async void DeleteItem(int index)
        {
            var item = AppData.Basket.Items[index];

            await basketModel.DeleteItem(item);

            AppData.Basket.CalculateBasket();
        }

        private class BasketItemSwipeCallback : ItemTouchHelper.Callback
        {
            private readonly Action<int> deleteWishListItem;
            private readonly Action<bool> setRefreshEnabled;

            public BasketItemSwipeCallback(Action<int> deleteWishListItem, Action<bool> setRefreshEnabled)
            {
                this.deleteWishListItem = deleteWishListItem;
                this.setRefreshEnabled = setRefreshEnabled;
            }

            public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                return MakeMovementFlags(0, ItemTouchHelper.Start);
            }

            public override bool OnMove(RecyclerView p0, RecyclerView.ViewHolder p1, RecyclerView.ViewHolder p2)
            {
                return false;
            }

            public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
            {
                base.OnSelectedChanged(viewHolder, actionState);

                var dragging = actionState == ItemTouchHelper.ActionStateDrag;
                var swiping = actionState == ItemTouchHelper.ActionStateSwipe;
                setRefreshEnabled(!dragging && !swiping);
            }

            public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
            {
                if (actionState == ItemTouchHelper.ActionStateSwipe)
                {
                    // Get RecyclerView item from the ViewHolder
                    View itemView = viewHolder.ItemView;

                    Paint p = new Paint();
                    if (dX < 0)
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
            }
        }
    }
}