using System;
using System.Collections.Generic;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
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
    public class ItemCategoryAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private List<ItemCategory> itemCategories;
        private ListItemSize itemSize;

        private ImageModel imageModel;
        private ImageSize imageSize;

        public ItemCategoryAdapter(Context context, IItemClickListener listener, ListItemSize itemSize)
        {
            this.listener = listener;
            this.itemSize = itemSize;

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetItemCategories(List<ItemCategory> itemCategories)
        {
            this.itemCategories = itemCategories;
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
                if (itemCategories == null)
                    return 0;
                return itemCategories.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
            if (itemSize == ListItemSize.Normal)        //list
                return 0;
            
            return 1;   // card
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (itemCategories.Count <= position)
                return;

            var itemCategory = itemCategories[position];
            var itemCategoryViewHolder = holder as ItemCategoryViewHolder;

            if (itemCategory == null || itemCategoryViewHolder == null)
                return;

            itemCategoryViewHolder.Title.Text = itemCategory.Description;

            if (itemCategoryViewHolder.Image != null && itemCategoryViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(itemCategoryViewHolder.Image);

                itemCategoryViewHolder.Image.Tag = null;

                if (itemCategory.Images != null && itemCategory.Images.Count > 0)
                {
                    var tag = itemCategory.Images[0].Id;
                    itemCategoryViewHolder.Image.Tag = tag;

                    if (itemSize == ListItemSize.Normal)
                    {
                        var backgroundCircle = new ShapeDrawable(new OvalShape());
                        backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(itemCategory.Images[0].GetAvgColor());

                        itemCategoryViewHolder.ImageContainer.Background = backgroundCircle;
                    }
                    else if (itemSize == ListItemSize.SmallCard)
                    {
                        itemCategoryViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(itemCategory.Images[0].GetAvgColor()));
                    }

                    var image = await imageModel.ImageGetById(itemCategory.Images[0].Id, imageSize);

                    if (image == null)
                    {
                        if (itemCategoryViewHolder?.Image != null && itemCategoryViewHolder.Image.Tag.ToString() == tag)
                        {
                            itemCategoryViewHolder.Image.Tag = null;

                            itemCategoryViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (itemCategoryViewHolder?.Image != null && itemCategoryViewHolder.Image.Tag.ToString() == tag)
                        {
                            itemCategoryViewHolder.Image.Tag = null;

                            if (itemSize == ListItemSize.Normal)
                            {
                                Utils.ImageUtils.CrossfadeImage(itemCategoryViewHolder.Image, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(image.Image)), itemCategoryViewHolder.ImageContainer, image.Crossfade);
                            }
                            else
                            {
                                Utils.ImageUtils.CrossfadeImage(itemCategoryViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), itemCategoryViewHolder.ImageContainer, image.Crossfade);
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
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ItemCategoryListItem, parent, false);
            }
            else if (viewType == 1)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ItemCategoryCardItem, parent, false);
            }

            var vh = new ItemCategoryViewHolder(view, (pos) =>
            {
                var itemCategory = itemCategories[pos];

                listener.ItemClicked((int)ItemClickType.ItemCategory, itemCategory.Id, string.Empty, view);
            });

            return vh;
        }

        public class ItemCategoryViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public TextView Title { get; set; }
            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public View TextContainer { get; set; }

            public ItemCategoryViewHolder(View view, Action<int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.ItemCategoryListItemDescription);
                ImageContainer = view.FindViewById<View>(Resource.Id.ItemCategoryListItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ItemCategoryListItemImage);
                TextContainer = view.FindViewById<View>(Resource.Id.ItemCategoryListTextContainer);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    default:
                        itemClicked(AdapterPosition);
                        break;
                }
            }
        }
    }
}
