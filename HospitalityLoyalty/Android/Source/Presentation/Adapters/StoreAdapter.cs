using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using Presentation.Models;
using Presentation.Utils;
using ImageView = Android.Widget.ImageView;
using Object = Java.Lang.Object;

namespace Presentation.Adapters
{
    public class StoreAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private ImageModel imageModel;
        private List<Store> stores;
        private ImageSize imageSize;

        public StoreAdapter(Context context, IItemClickListener listener, int columns)
        {
            this.listener = listener;
            imageModel = new ImageModel(context);

            var dimension = context.Resources.DisplayMetrics.WidthPixels / columns;
            imageSize = new ImageSize(dimension, dimension);
        }

        public void SetStores(List<Store> stores)
        {
            this.stores = stores;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get
            {
                if (stores == null || stores.Count == 0)
                {
                    return 0;
                }
                return stores.Count;
            }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override int GetItemViewType(int pos)
        {
            return 0;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            if (viewHolder is StoreViewHolder)
            {
                var storeViewHolder = viewHolder as StoreViewHolder;
                var store = stores[position];

                if (store == null)
                {
                    return;
                }

                storeViewHolder.Title.Text = store.Description;
                storeViewHolder.SubTitle.Text = store.Address.Address1;

                if (storeViewHolder.Image != null && storeViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(storeViewHolder.Image);

                    storeViewHolder.Image.Tag = null;

                    if (store.Images.Count > 0 && store.Images[0] != null)
                    {
                        var tag = store.Images[0].Id;
                        storeViewHolder.Image.Tag = tag;

                        storeViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(store.Images[0].GetAvgColor()));

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
                            if (storeViewHolder?.Image?.Tag != null && storeViewHolder.Image.Tag.ToString() == tag)
                            {
                                storeViewHolder.Image.Tag = null;

                                Utils.ImageUtils.CrossfadeImage(storeViewHolder.Image, ImageUtils.DecodeImage(image.Image), storeViewHolder.ImageContainer, image.Crossfade);
                            }
                        }
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder vh = null;

            if (viewType == 0)
            {
                View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.StoreCardItem, parent);


                vh = new StoreViewHolder(view, (pos) =>
                {
                    var store = stores[pos];

                    listener.ItemClicked(ItemType.Store, store.Id, "", 0, view, string.Empty);
                });
            }

            return vh;
        }

        private class StoreViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }

            public StoreViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public StoreViewHolder(View view, Action<int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer = view.FindViewById<View>(Resource.Id.StoreListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.StoreListItemViewItemImage);
                Title = view.FindViewById<TextView>(Resource.Id.StoreListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.StoreListItemViewSubtitle);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                itemClicked(AdapterPosition);
            }

            protected override void Dispose(bool disposing)
            {
                if (Image != null)
                    ImageUtils.ClearImageView(Image);
                
                base.Dispose(disposing);
            }
        }
    }
}