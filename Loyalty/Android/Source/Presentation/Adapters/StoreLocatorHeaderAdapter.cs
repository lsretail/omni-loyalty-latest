using System;
using System.Collections.Generic;

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
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation.Adapters
{
    public class StoreLocatorHeaderAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private List<Object> items = new List<object>();
        private ListItemSize itemSize;

        private ImageModel imageModel;
        private ImageSize imageSize;

        public StoreLocatorHeaderAdapter(Context context, IItemClickListener listener, ListItemSize itemSize)
        {
            this.listener = listener;
            this.itemSize = itemSize;

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetStores(List<Store> stores, string header = "")
        {
            this.items.Clear();
            
            if (!string.IsNullOrEmpty(header))
                this.items.Add(new HeaderItem() { Title = header });

            this.items.AddRange(stores);

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

        public override int ItemCount
        {
            get
            {
                if (items == null)
                    return 0;
                return items.Count;
            }
        }
        public override int GetColumnSpan(int position, int maxColumns)
        {
            var item = items[position];

            if (item is HeaderItem)
            {
                return maxColumns;
            }

            return base.GetColumnSpan(position, maxColumns);
        }

        public override int GetItemViewType(int position)
        {
            if (items[position] is HeaderItem)
                return 2;

            if (itemSize == ListItemSize.Normal)        //list
            {
                return 0;
            }
            else                                        //card
            {
                return 1;
            }
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
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
            var store = items[position] as Store;
            var storeViewHolder = holder as StoreViewHolder;

            if (store == null || storeViewHolder == null)
            {
                return;
            }

            storeViewHolder.Title.Text = store.Description;
            storeViewHolder.Subtitle.Text = store.Address.Address1;

            if (storeViewHolder.Image != null && storeViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(storeViewHolder.Image);

                storeViewHolder.Image.Tag = null;

                if (store.Images != null && store.Images.Count > 0)
                {
                    var tag = store.Images[0].Id;
                    storeViewHolder.Image.Tag = tag;

                    if (itemSize == ListItemSize.Normal)
                    {
                        var backgroundCircle = new ShapeDrawable(new OvalShape());
                        backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(store.Images[0].GetAvgColor());

                        storeViewHolder.ImageContainer.Background = backgroundCircle;
                    }
                    else if (itemSize == ListItemSize.SmallCard)
                    {
                        storeViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(store.Images[0].GetAvgColor()));
                    }

                    var image = await imageModel.ImageGetById(store.Images[0].Id, imageSize);

                    if (image == null)
                    {
                        if (storeViewHolder?.Image != null && storeViewHolder.Image.Tag.ToString() == tag)
                        {
                            storeViewHolder.Image.Tag = null;

                            storeViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (storeViewHolder?.Image != null && storeViewHolder.Image.Tag.ToString() == tag)
                        {
                            storeViewHolder.Image.Tag = null;

                            if (itemSize == ListItemSize.Normal)
                            {
                                Utils.ImageUtils.CrossfadeImage(storeViewHolder.Image, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(image.Image)), storeViewHolder.ImageContainer, image.Crossfade);
                            }
                            else
                            {
                                Utils.ImageUtils.CrossfadeImage(storeViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), storeViewHolder.ImageContainer, image.Crossfade);
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
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.StoreLocatorHeaderListItem, parent, false);
            }
            else if (viewType == 1)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.StoreLocatorHeaderCardItem, parent, false);
            }
            else if (viewType == 2)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardDisabledSectionHeader, parent, false);

                return new HeaderViewHolder(view);
            }

            var vh = new StoreViewHolder(view, (type, pos) =>
            {
                var store = items[pos] as Store;

                switch (type)
                {
                    case StoreType.Item:
                        listener.ItemClicked((int)ItemClickType.Store, store.Id, string.Empty, view);
                        break;
                }

                
            });

            return vh;
        }

        private class HeaderItem
        {
            public string Title { get; set; }
        }

        public enum StoreType
        {
            Item = 0,
        }

        public class HeaderViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }

            public HeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public HeaderViewHolder(View itemView)
                : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.CardDisabledSectionHeaderDescription);
            }
        }

        public class StoreViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<StoreType, int> itemClicked;

            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }

            public StoreViewHolder(View view, Action<StoreType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.StoreLocatorHeaderListItemStoreName);
                Subtitle = view.FindViewById<TextView>(Resource.Id.StoreLocatorHeaderListItemStoreAddress);
                ImageContainer = view.FindViewById<View>(Resource.Id.StoreLocatorHeaderListItemStoreImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.StoreLocatorHeaderListItemStoreImage);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    default:
                        itemClicked(StoreType.Item, AdapterPosition);
                        break;
                }
            }
        }
    }
}