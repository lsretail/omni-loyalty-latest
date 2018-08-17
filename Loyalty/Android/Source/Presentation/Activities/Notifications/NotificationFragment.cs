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
using Presentation.Activities.Search;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.Notifications
{
    public class NotificationFragment : CardListFragment, IRefreshableActivity, IItemClickListener, SwipeRefreshLayout.IOnRefreshListener, ViewTreeObserver.IOnGlobalLayoutListener, IBroadcastObserver, PopupMenu.IOnMenuItemClickListener
    {
        private SwipeRefreshLayout refreshLayout;
        private RecyclerView notificationRecyclerView;
        private ItemTouchHelper itemTouchHelper;

        private NotificationAdapter adapter;
        private NotificationModel model;

        private bool loadNotificationsFromService;

        private string popupNotificationId = string.Empty;

        public static NotificationFragment NewInstance()
        {
            var couponFragment = new NotificationFragment() { Arguments = new Bundle() };
            return couponFragment;
        }

        public override View CreateView(LayoutInflater inflater)
        {
            return Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Notifications);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstance)
        {
            if (container == null)
            {
                return null;
            }

            model = new NotificationModel(Activity, this);

            HasOptionsMenu = true;

            var view = base.OnCreateView(inflater, container, savedInstance);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.NotificationsScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            loadNotificationsFromService = Arguments.GetBoolean(BundleConstants.LoadNotificationsFromService, false);

            adapter = new NotificationAdapter(Activity, this, OnOverFlowClicked, BaseRecyclerAdapter.ListItemSize.Normal);
            adapter.SetNotifications(Activity, AppData.Device.UserLoggedOnToDevice.Notifications);

            notificationRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.NotificationViewNotificationList);
            SetLayoutManager(ShowAsList);
            notificationRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            notificationRecyclerView.HasFixedSize = true;

            notificationRecyclerView.SetAdapter(adapter);

            refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.NotificationsViewRefreshLayout);

            refreshLayout.SetColorSchemeResources(Resource.Color.notifications, Resource.Color.accent, Resource.Color.notifications, Resource.Color.accent);
            refreshLayout.SetOnRefreshListener(this);

            Util.Utils.ViewUtils.AddOnGlobalLayoutListener(view, this);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).AddObserver(this);
            }

            UpdateNotificationList();
        }

        public override void OnPause()
        {
            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.NotificationsUpdated:
                case Utils.BroadcastUtils.DomainModelUpdated:
                    UpdateNotificationList();
                    break;
            }
        }

        public void SetLayoutManager(bool showAsList)
        {
            itemTouchHelper = new ItemTouchHelper(new NotificationSwipeCallback(
                position =>
                {
                    var notification = adapter.GetNotification(position);

                    if (notification != null)
                    {
                        DeleteNotification(notification);
                    }
                },
                enabled =>
                {
                    refreshLayout.Enabled = enabled;
                }));

            if (showAsList)
            {

                notificationRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.Normal);

                itemTouchHelper.AttachToRecyclerView(notificationRecyclerView);
            }
            else
            {

                var manager = new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.CardColumns), LinearLayoutManager.Vertical, false);
                manager.SetSpanSizeLookup(new BaseRecyclerAdapter.GridSpanSizeLookup(Activity, adapter));
                notificationRecyclerView.SetLayoutManager(manager);
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.SmallCard);

                itemTouchHelper.AttachToRecyclerView(null);
            }
        }

        public void OnGlobalLayout()
        {
            if (loadNotificationsFromService)
            {
                UpdateNotificationList();
            }

            Util.Utils.ViewUtils.RemoveOnGlobalLayoutListener(View, this);
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var notification = AppData.Device.UserLoggedOnToDevice.Notifications.FirstOrDefault(x => x.Id == id);

            var intent = new Intent();
            intent.PutExtra(BundleConstants.NotificationId, notification.Id);
            intent.SetClass(Activity, typeof(NotificationDetailActivity));
            StartActivityForResult(intent, 0);
        }

        public override void OnDestroyView()
        {
            model.Stop();
            base.OnDestroyView();
        }

        public void UpdateNotificationList()
        {
            adapter.SetNotifications(Activity, AppData.Device.UserLoggedOnToDevice.Notifications);
        }

        private async void RefreshNotifications()
        {
            await model.GetNotificationsByContactId();
        }

        public void ShowIndicator(bool show)
        {
            refreshLayout.Refreshing = show;
        }

        private void OnOverFlowClicked(string notificationId, View view)
        {
            popupNotificationId = notificationId;

            var notification = AppData.Device.UserLoggedOnToDevice.Notifications.FirstOrDefault(x => x.Id == notificationId);

            var popup = new Android.Support.V7.Widget.PopupMenu(Activity, view);
            popup.MenuInflater.Inflate(Resource.Menu.NotificationCABMenu, popup.Menu);

            if (notification.Status == NotificationStatus.New)
            {
                popup.Menu.RemoveItem(Resource.Id.MenuViewMarkUnread);
            }

            if (notification.Status == NotificationStatus.Read)
            {
                popup.Menu.RemoveItem(Resource.Id.MenuViewMarkRead);
            }

            popup.SetOnMenuItemClickListener(this);
            popup.Show();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            var notification = AppData.Device.UserLoggedOnToDevice.Notifications.FirstOrDefault(x => x.Id == popupNotificationId);

            if (notification == null)
                return false;

            switch (item.ItemId)
            {
                case Resource.Id.MenuViewMarkRead:
                    UpdateNotificationStatus(notification, NotificationStatus.Read);
                    return true;

                case Resource.Id.MenuViewMarkUnread:
                    UpdateNotificationStatus(notification, NotificationStatus.New);
                    return true;

                case Resource.Id.MenuViewDelete:
                    DeleteNotification(notification);

                    return true;
            }

            return false;
        }

        private async void UpdateNotificationStatus(Notification notification, NotificationStatus status)
        {
            var success = await model.UpdateNotification(notification, status);

            if (success)
            {
                UpdateNotificationList();
            }
        }

        private void DeleteNotification(Notification notification)
        {
            UpdateNotificationStatus(notification, NotificationStatus.Closed);
        }

        #region MENU

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.GenericSearchMenu, menu);

            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewSearch:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(GeneralSearchActivity));
                    intent.PutExtra(BundleConstants.SearchType, (int)SearchType.Notification);

                    StartActivity(intent);

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        public void OnRefresh()
        {
            RefreshNotifications();
        }

        private class NotificationSwipeCallback : ItemTouchHelper.Callback
        {
            private readonly Action<int> deleteNotification;
            private readonly Action<bool> setRefreshEnabled;

            public NotificationSwipeCallback(Action<int> deleteNotification, Action<bool> setRefreshEnabled)
            {
                this.deleteNotification = deleteNotification;
                this.setRefreshEnabled = setRefreshEnabled;
            }

            public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                if (viewHolder is NotificationAdapter.NotificationViewHolder)
                {
                    return MakeMovementFlags(0, ItemTouchHelper.Start);
                }

                return MakeMovementFlags(0, 0);
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
                    deleteNotification(viewHolder.AdapterPosition);
                }
            }
        }
    }
}