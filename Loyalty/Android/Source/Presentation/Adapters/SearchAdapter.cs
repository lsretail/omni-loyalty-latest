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

using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace Presentation.Adapters
{
    public class SearchAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private List<Object> items = new List<object>();

        private ImageModel imageModel;
        private ImageSize imageSize;

        public SearchAdapter(Context context, IItemClickListener listener)
        {
            this.listener = listener;

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetSearchResults(Context context, List<SearchType> availableTypes, SearchRs searchRs)
        {
            items.Clear();

            foreach (var availableType in availableTypes)
            {
                if (availableType == SearchType.Item)
                {
                    items.Add(new HeaderItem()
                        {
                            Title = string.Format(context.GetString(Resource.String.GeneralSearchViewItemsGroupHeader), searchRs.Items.Count)
                        });
                    items.AddRange(searchRs.Items);
                }
                else if (availableType == SearchType.Notification)
                {
                    items.Add(new HeaderItem()
                    {
                        Title = string.Format(context.GetString(Resource.String.GeneralSearchViewNotificationsGroupHeader), searchRs.Notifications.Count)
                    });
                    items.AddRange(searchRs.Notifications);
                }
                else if (availableType == SearchType.SalesEntry)
                {
                    items.Add(new HeaderItem()
                    {
                        Title = string.Format(context.GetString(Resource.String.GeneralSearchViewTransactionsGroupHeader), searchRs.SalesEntries.Count)
                    });
                    items.AddRange(searchRs.SalesEntries);
                }
                else if (availableType == SearchType.OneList)
                {
                    items.Add(new HeaderItem()
                    {
                        Title = string.Format(context.GetString(Resource.String.GeneralSearchViewWishListGroupHeader), searchRs.OneLists.Count)
                    });
                    items.AddRange(searchRs.OneLists);
                }
                else if (availableType == SearchType.Store)
                {
                    items.Add(new HeaderItem()
                    {
                        Title = string.Format(context.GetString(Resource.String.GeneralSearchViewStoresGroupHeader), searchRs.Stores.Count)
                    });
                    items.AddRange(searchRs.Stores);
                }
            }

            NotifyDataSetChanged();
        }

        private void SetImageSize(Context context)
        {
            var height = context.Resources.GetDimensionPixelSize(Resource.Dimension.ListImageHeight);
            imageSize = new ImageSize(height, height);
        }

        public override int ItemCount
        {
            get
            {
                if (items == null)
                    return 0;
                return items.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
            var item = items[position];
            
            if (item is HeaderItem)      
            {
                return 0;
            }
            else if (item is LoyItem)
            {
                return 1;
            }
            else if (item is PublishedOffer && (item as PublishedOffer).Code != OfferDiscountType.Coupon)
            {
                return 2;
            }
            else if (item is PublishedOffer && (item as PublishedOffer).Code == OfferDiscountType.Coupon)
            {
                return 3;
            }
            else if (item is Notification)
            {
                return 4;
            }
            else if (item is SalesEntry)
            {
                return 5;
            }
            else if (item is OneListItem)
            {
                return 6;
            }
            else if (item is Store)
            {
                return 7;
            }
            return -1;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ImageView imageView = null;
            View imageContainer = null;
            LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView image = null;

            if (holder is HeaderViewHolder)
            {
                var headerItem = items[position] as HeaderItem;
                var headerViewHolder = holder as HeaderViewHolder;

                if (headerItem == null)
                {
                    return;
                }

                headerViewHolder.Title.Text = headerItem.Title;
            }
            else if (holder is ItemAdapter.ItemViewHolder)
            {
                var item = items[position] as LoyItem;
                var itemViewHolder = holder as ItemAdapter.ItemViewHolder;

                if (item == null)
                {
                    return;
                }

                itemViewHolder.Title.Text = item.Description;

                imageView = itemViewHolder.Image;
                imageContainer = itemViewHolder.ImageContainer;

                if (item.Images != null && item.Images.Count > 0)
                {
                    image = item.Images[0];
                }
            }
            else if (holder is OfferAdapter.OfferViewHolder)
            {
                var offer = items[position] as PublishedOffer;
                var offerViewHolder = holder as OfferAdapter.OfferViewHolder;

                if (offer == null || offerViewHolder == null)
                {
                    return;
                }

                offerViewHolder.Title.Text = offer.Description;
                offerViewHolder.Subtitle.Text = offer.Details;

                imageView = offerViewHolder.Image;
                imageContainer = offerViewHolder.ImageContainer;

                if (offer.Images != null && offer.Images.Count > 0)
                {
                    image = offer.Images[0];
                }
            }
            else if (holder is CouponAdapter.CouponViewHolder)
            {
                var coupon = items[position] as PublishedOffer;
                var couponViewHolder = holder as CouponAdapter.CouponViewHolder;

                if (coupon == null || couponViewHolder == null)
                {
                    return;
                }

                couponViewHolder.Title.Text = coupon.Description;
                couponViewHolder.Subtitle.Text = coupon.Details;

                imageView = couponViewHolder.Image;
                imageContainer = couponViewHolder.ImageContainer;

                if (coupon.Images != null && coupon.Images.Count > 0)
                {
                    image = coupon.Images[0];
                }
            }
            else if (holder is NotificationAdapter.NotificationViewHolder)
            {
                var notification = items[position] as Notification;
                var notificationViewHolder = holder as NotificationAdapter.NotificationViewHolder;

                if (notification == null || notificationViewHolder == null)
                {
                    return;
                }

                notificationViewHolder.Title.Text = notification.Description;
                notificationViewHolder.Subtitle.Text = notification.Details;

                imageView = notificationViewHolder.Image;
                imageContainer = notificationViewHolder.ImageContainer;

                if (notification.Images != null && notification.Images.Count > 0)
                {
                    image = notification.Images[0];
                }
            }
            else if (holder is TransactionAdapter.TransactionViewHolder)
            {
                var transactionViewHolder = holder as TransactionAdapter.TransactionViewHolder;
                var transaction = items[position] as SalesEntry;

                if (transactionViewHolder == null || transaction == null)
                {
                    return;
                }

                transactionViewHolder.Title.Text = transaction.DocumentRegTime.ToString("f");

                transactionViewHolder.Subtitle.Text = transaction.StoreName;
                transactionViewHolder.Price.Text = transaction.TotalAmount.ToString("N02");
            }
            else if (holder is ShoppingListDetailAdapter.ShoppingListLineViewHolder)
            {
                var shoppingListLineViewHolder = holder as ShoppingListDetailAdapter.ShoppingListLineViewHolder;
                var shoppingListLine = items[position] as OneListItem;

                if (shoppingListLineViewHolder == null || shoppingListLine == null)
                {
                    return;
                }

                shoppingListLineViewHolder.Title.Text = shoppingListLine.Item.Description;

                if (shoppingListLine.VariantReg != null)
                {
                    shoppingListLineViewHolder.Variant.Visibility = ViewStates.Visible;
                    shoppingListLineViewHolder.Variant.Text = shoppingListLine.VariantReg.ToString();
                }
                else
                {
                    shoppingListLineViewHolder.Variant.Visibility = ViewStates.Gone;
                } 

                imageView = shoppingListLineViewHolder.Image;
                imageContainer = shoppingListLineViewHolder.ImageContainer;

                if (shoppingListLine.Image != null)
                {
                    image = shoppingListLine.Image;
                }
            }
            else if (holder is StoreLocatorHeaderAdapter.StoreViewHolder)
            {
                var store = items[position] as Store;
                var storeViewHolder = holder as StoreLocatorHeaderAdapter.StoreViewHolder;

                if (store == null || storeViewHolder == null)
                {
                    return;
                }

                storeViewHolder.Title.Text = store.Description;
                storeViewHolder.Subtitle.Text = store.Address.Address1;

                imageView = storeViewHolder.Image;
                imageContainer = storeViewHolder.ImageContainer;

                if (store.Images != null && store.Images.Count > 0)
                {
                    image = store.Images[0];
                }
            }

            LoadImage(imageView, imageContainer, image);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder vh = null;

            if (viewType == 0)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardDisabledSectionHeader, parent, false);
                vh = new HeaderViewHolder(view);
            }
            if (viewType == 1)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ItemsListItem, parent, false);
                vh = new ItemAdapter.ItemViewHolder(
                    view,
                    (type, position) =>
                    {
                        var item = items[position] as LoyItem;

                        if (item != null)
                        {
                            listener.ItemClicked((int)ItemClickType.Item, item.Id, "", view);
                        }
                    });
            }
            if (viewType == 2)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.OfferHeaderListItem, parent, false);
                vh = new OfferAdapter.OfferViewHolder(
                    view,
                    (type, position) =>
                    {
                        var offer = items[position] as PublishedOffer;

                        if (offer != null)
                        {
                            listener.ItemClicked((int)ItemClickType.Offer, offer.Id, "", view);
                        }
                    });

                (vh as OfferAdapter.OfferViewHolder).AddRemoveButton.Visibility = ViewStates.Gone;
            }
            if (viewType == 3)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CouponHeaderListItem, parent, false);
                vh = new CouponAdapter.CouponViewHolder(
                    view,
                    (type, position) =>
                    {
                        var coupon = items[position] as PublishedOffer;

                        if (coupon != null)
                        {
                            listener.ItemClicked((int)ItemClickType.Coupon, coupon.Id, "", view);
                        }
                    });

                (vh as CouponAdapter.CouponViewHolder).AddRemoveButton.Visibility = ViewStates.Gone;
            }
            if (viewType == 4)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.NotificationHeaderListItem, parent, false);
                vh = new NotificationAdapter.NotificationViewHolder(
                    view,
                    (type, position, clickedView) =>
                    {
                        var notification = items[position] as Notification;

                        if (notification != null)
                        {
                            listener.ItemClicked((int)ItemClickType.Notification, notification.Id, "", view);
                        }
                    });

                (vh as NotificationAdapter.NotificationViewHolder).OverFlow.Visibility = ViewStates.Gone;
            }
            if (viewType == 5)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionHeaderListItem, parent, false);
                vh = new TransactionAdapter.TransactionViewHolder(
                    view,
                    (type, position) =>
                    {
                        var item = items[position] as SalesEntry;

                        if (item != null)
                        {
                            listener.ItemClicked((int)ItemClickType.Transaction, item.Id, "", view);
                        }
                    });
            }
            if (viewType == 6)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.WishListListItem, parent, false);
                vh = new ShoppingListDetailAdapter.ShoppingListLineViewHolder(
                    view,
                    (type, position, clickedView) =>
                    {
                        var item = items[position] as OneListItem;

                        if (item != null)
                        {
                            listener.ItemClicked((int)ItemClickType.ShoppingListLine, item.Id, "", view);
                        }
                    });

                (vh as ShoppingListDetailAdapter.ShoppingListLineViewHolder).Overflow.Visibility = ViewStates.Gone;
            }
            if (viewType == 7)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.StoreLocatorHeaderListItem, parent, false);
                vh = new StoreLocatorHeaderAdapter.StoreViewHolder(
                    view,
                    (type, position) =>
                    {
                        var item = items[position] as Store;

                        if (item != null)
                        {
                            listener.ItemClicked((int)ItemClickType.Store, item.Id, "", view);
                        }
                    });
            }

            return vh;
        }

        private async void LoadImage(ImageView imageView, View imageContainer, LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView image)
        {
            if (imageView != null && imageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(imageView);

                imageView.Tag = null;

                if (image != null)
                {
                    var tag = image.Id;
                    imageView.Tag = tag;

                    var backgroundCircle = new ShapeDrawable(new OvalShape());
                    backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(image.GetAvgColor());

                    imageContainer.Background = backgroundCircle;

                    var loadedImage = await imageModel.ImageGetById(image.Id, imageSize);

                    if (loadedImage == null)
                    {
                        if (imageView?.Tag.ToString() == tag)
                        {
                            imageView.Tag = null;

                            imageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (imageView?.Tag != null && imageView.Tag.ToString() == tag)
                        {
                            imageView.Tag = null;

                            Utils.ImageUtils.CrossfadeImage(imageView, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(loadedImage.Image)), imageContainer, loadedImage.Crossfade);
                        }
                    }
                }
            }
        }

        private class HeaderItem
        {
            public string Title { get; set; }
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
    }
}