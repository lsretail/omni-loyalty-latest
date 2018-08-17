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
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Adapters
{
    public class ItemAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private List<LoyItem> items;
        private ListItemSize itemSize;
        private bool isLoading;

        private ImageModel imageModel;
        private ImageSize imageSize;

        public ItemAdapter(Context context, IItemClickListener listener, ListItemSize itemSize)
        {
            this.listener = listener;
            this.itemSize = itemSize;

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetItems(List<LoyItem> items)
        {
            this.items = items;
            NotifyDataSetChanged();
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                NotifyDataSetChanged();
            }
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

                if (isLoading)
                {
                    return items.Count + 1;
                }

                return items.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
            if (position == items.Count)
            {
                return 2;   //progress spinner
            }

            if (itemSize == ListItemSize.Normal)        //list
            {
                return 0;
            }
            else                                        //card
            {
                return 1;
            }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            if (GetItemViewType(position) == 2)
                return maxColumns;
            return 1;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if(GetItemViewType(position) == 2)
                return;

            var item = items[position];
            var itemViewHolder = holder as ItemViewHolder;

            if (item == null || itemViewHolder == null)
            {
                return;
            }

            itemViewHolder.Title.Text = item.Description;

            if (itemViewHolder.Image != null && itemViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(itemViewHolder.Image);

                itemViewHolder.Image.Tag = null;

                if (item.Images != null && item.Images.Count > 0)
                {
                    var tag = item.Images[0].Id;
                    itemViewHolder.Image.Tag = tag;

                    if (itemSize == ListItemSize.Normal)
                    {
                        var backgroundCircle = new ShapeDrawable(new OvalShape());
                        backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(item.Images[0].GetAvgColor());

                        itemViewHolder.ImageContainer.Background = backgroundCircle;
                    }
                    else if (itemSize == ListItemSize.SmallCard)
                    {
                        itemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(item.Images[0].GetAvgColor()));
                    }

                    var image = await imageModel.ImageGetById(item.Images[0].Id, imageSize);

                    if (image == null)
                    {
                        if (itemViewHolder?.Image != null && itemViewHolder.Image.Tag.ToString() == tag)
                        {
                            itemViewHolder.Image.Tag = null;

                            itemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (itemViewHolder?.Image?.Tag != null && itemViewHolder.Image.Tag.ToString() == tag)
                        {
                            itemViewHolder.Image.Tag = null;

                            if (itemSize == ListItemSize.Normal)
                            {
                                Utils.ImageUtils.CrossfadeImage(itemViewHolder.Image, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(image.Image)), itemViewHolder.ImageContainer, image.Crossfade);
                            }
                            else
                            {
                                Utils.ImageUtils.CrossfadeImage(itemViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), itemViewHolder.ImageContainer, image.Crossfade);
                            }
                        }
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == 2)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardLoading, parent, false);

                return new ProgressViewHolder(view);
            }
            else
            {
                View view = null;

                if (viewType == 0)
                {
                    view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ItemsListItem, parent, false);
                }
                else if (viewType == 1)
                {
                    view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ItemsCardItem, parent, false);
                }

                var vh = new ItemViewHolder(view, (type, pos) =>
                {
                    var store = items[pos];

                    switch (type)
                    {
                        case ItemType.Item:
                            listener.ItemClicked((int)ItemClickType.Item, store.Id, string.Empty, view);
                            break;
                    }


                });

                return vh;
            }
        }

        public enum ItemType
        {
            Item = 0,
        }

        public class ItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemType, int> itemClicked;

            public TextView Title { get; set; }
            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }

            public ItemViewHolder(View view, Action<ItemType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.ItemsListItemViewItemTitle);
                ImageContainer = view.FindViewById<View>(Resource.Id.ItemsListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ItemsListItemViewItemImage);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    default:
                        itemClicked(ItemType.Item, AdapterPosition);
                        break;
                }
            }

            public void LoadItem(LoyItem item)
            {
                
            }
        }

        public class ProgressViewHolder : RecyclerView.ViewHolder
        {
            public ProgressViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public ProgressViewHolder(View itemView) : base(itemView)
            {
            }
        }
    }
}