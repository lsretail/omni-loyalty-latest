using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Models;
using Presentation.Util;

using ImageView = Android.Widget.ImageView;
using Object = Java.Lang.Object;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Adapters
{
    public class NotificationAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private readonly Action<string, View> onOverFlowClicked;
        private List<object> notifications = new List<object>();
        private ListItemSize itemSize;

        private ImageModel imageModel;
        private ImageSize imageSize;

        public NotificationAdapter(Context context, IItemClickListener listener, Action<string, View> onOverFlowClicked, ListItemSize itemSize)
        {
            this.listener = listener;
            this.itemSize = itemSize;
            this.onOverFlowClicked = onOverFlowClicked;

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetNotifications(Context context, List<Notification> notifications)
        {
            this.notifications.Clear();

            var unread = notifications.Where(x => x.Status == NotificationStatus.New).ToList();
            var read = notifications.Where(x => x.Status == NotificationStatus.Read).ToList();

            if (unread != null && unread.Count > 0)
            {
                this.notifications.Add(new HeaderItem()
                {
                    Title = context.GetString(Resource.String.NotificationViewUnread)
                });

                this.notifications.AddRange(unread);
            }

            if (read != null && read.Count > 0)
            {
                this.notifications.Add(new HeaderItem()
                {
                    Title = context.GetString(Resource.String.NotificationViewRead)
                });

                this.notifications.AddRange(read);
            }

            NotifyDataSetChanged();
        }

        public void SetCardMode(Context context, ListItemSize itemSize)
        {
            this.itemSize = itemSize;
            SetImageSize(context);
            NotifyDataSetChanged();
        }

        private void SetImageSize(Context context)
        {
            if (itemSize == ListItemSize.Normal)
            {
                var height = context.Resources.GetDimensionPixelSize(Resource.Dimension.ListImageHeight);
                imageSize = new ImageSize(height, height);
            }
            else if (itemSize == ListItemSize.SmallCard)
            {
                var height = context.Resources.GetDimensionPixelSize(Resource.Dimension.CardImageHeight);
                imageSize = new ImageSize(height, height);
            }
        }

        public Notification GetNotification(int position)
        {
            return notifications[position] as Notification;
        }

        public override int ItemCount
        {
            get
            {
                if (notifications == null)
                    return 0;
                return notifications.Count;
            }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            var item = notifications[position];

            if (item is HeaderItem)
            {
                return maxColumns;
            }

            return base.GetColumnSpan(position, maxColumns);
        }

        public override int GetItemViewType(int position)
        {
            if (notifications[position] is HeaderItem)
            {
                return 0;
            }
            if (itemSize == ListItemSize.Normal)        //list
            {
                return 1;
            }
            else                                        //card
            {
                return 2;
            }
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var type = GetItemViewType(position);

            if (type == 0)
            {
                var headerItem = notifications[position] as HeaderItem;
                var headerViewHolder = holder as HeaderViewHolder;

                if (headerItem == null || headerViewHolder == null)
                {
                    return;
                }

                headerViewHolder.Title.Text = headerItem.Title;
            }
            else if (type == 1 || type == 2)
            {
                var offer = notifications[position] as Notification;
                var offerViewHolder = holder as NotificationViewHolder;

                if (offer == null || offerViewHolder == null)
                {
                    return;
                }

                offerViewHolder.Title.Text = offer.Description;
                offerViewHolder.Subtitle.Text = offer.Details;

                if (offerViewHolder.Image != null && offerViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(offerViewHolder.Image);

                    offerViewHolder.Image.Tag = null;

                    if (offer.Images != null && offer.Images.Count > 0)
                    {
                        var tag = offer.Images[0].Id;
                        offerViewHolder.Image.Tag = tag;

                        if (itemSize == ListItemSize.Normal)
                        {
                            var backgroundCircle = new ShapeDrawable(new OvalShape());
                            backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(offer.Images[0].GetAvgColor());

                            offerViewHolder.ImageContainer.Background = backgroundCircle;
                        }
                        else if (itemSize == ListItemSize.SmallCard)
                        {
                            offerViewHolder.ImageContainer.SetBackgroundColor(
                                Android.Graphics.Color.ParseColor(offer.Images[0].GetAvgColor()));
                        }

                        var image = await imageModel.ImageGetById(offer.Images[0].Id, imageSize);

                        if (image == null)
                        {
                            if (offerViewHolder?.Image != null && offerViewHolder.Image.Tag.ToString() == tag)
                            {
                                offerViewHolder.Image.Tag = null;

                                offerViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                            }
                        }
                        else
                        {
                            if (offerViewHolder?.Image != null && offerViewHolder.Image.Tag.ToString() == tag)
                            {
                                offerViewHolder.Image.Tag = null;

                                if (itemSize == ListItemSize.Normal)
                                {
                                    Utils.ImageUtils.CrossfadeImage(offerViewHolder.Image, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(image.Image)), offerViewHolder.ImageContainer, image.Crossfade);
                                }
                                else
                                {
                                    Utils.ImageUtils.CrossfadeImage(offerViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), offerViewHolder.ImageContainer, image.Crossfade);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = null;

            if (viewType == 0)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardDisabledSectionHeader, parent, false);
                
                return new HeaderViewHolder(view);
            }
            if (viewType == 1)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.NotificationHeaderListItem, parent, false);
            }
            else if (viewType == 2)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.NotificationHeaderCardItem, parent, false);//todo
            }

            return new NotificationViewHolder(view, (type, pos, clickedView) =>
            {
                var offer = notifications[pos];

                switch (type)
                {
                    case NotificationType.OverFlow:
                        onOverFlowClicked((offer as Notification).Id, clickedView);
                        break;

                    case NotificationType.Item:
                        listener.ItemClicked((int)ItemClickType.Notification, (offer as Notification).Id, string.Empty, clickedView);
                        break;
                }

                
            });
        }

        private class HeaderItem
        {
            public string Title { get; set; }
        }

        public enum NotificationType
        {
            Item = 0,
            OverFlow = 1,
        }

        public class HeaderViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }

            public HeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public HeaderViewHolder(View itemView) : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.CardDisabledSectionHeaderDescription);
            }
        }

        public class NotificationViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<NotificationType, int, View> itemClicked;

            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public ImageButton OverFlow { get; set; }

            public NotificationViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public NotificationViewHolder(View view, Action<NotificationType, int, View> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.NotificationViewPrimaryText);
                Subtitle = view.FindViewById<TextView>(Resource.Id.NotificationViewSecondaryText);
                ImageContainer = view.FindViewById<View>(Resource.Id.ItemsListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ItemsListItemViewItemImage);
                OverFlow = view.FindViewById<ImageButton>(Resource.Id.NotificationHeaderListItemOverflow);

                OverFlow.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.NotificationHeaderListItemOverflow:
                        itemClicked(NotificationType.OverFlow, AdapterPosition, v);
                        break;

                    default:
                        itemClicked(NotificationType.Item, AdapterPosition, v);
                        break;
                }
            }
        }
    }
}