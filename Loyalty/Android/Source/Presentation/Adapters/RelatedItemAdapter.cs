using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
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
    public class RelatedItemAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private List<LoyItem> items;

        private ImageModel imageModel;
        private ImageSize imageSize;

        public RelatedItemAdapter(Context context, IItemClickListener listener)
        {
            this.listener = listener;

            var height = context.Resources.GetDimensionPixelSize(Resource.Dimension.CardImageHeight);
            imageSize = new ImageSize(height, height);

            imageModel = new ImageModel(context, null);
        }

        public void SetItems(List<LoyItem> items)
        {
            this.items = items;
            NotifyDataSetChanged();
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

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
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

                    itemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(item.Images[0].GetAvgColor()));

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
                        if (itemViewHolder?.Image != null && itemViewHolder.Image.Tag != null && itemViewHolder.Image.Tag.ToString() == tag)
                        {
                            itemViewHolder.Image.Tag = null;

                            Utils.ImageUtils.CrossfadeImage(itemViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), itemViewHolder.ImageContainer, image.Crossfade);
                        }
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ItemsCardItem,
                parent, false);

            view.LayoutParameters.Width =
                view.Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardImageHeight);

            var vh = new ItemViewHolder(view, (type, pos) =>
            {
                var store = items[pos];

                switch (type)
                {
                    case ItemType.Item:
                        listener.ItemClicked((int) ItemClickType.Item, store.Id, string.Empty, view);
                        break;
                }
            });

            return vh;
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