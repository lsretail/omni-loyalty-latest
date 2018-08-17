using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Presentation.Models;
using Presentation.Utils;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Adapters
{
    public class CouponAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private ImageModel imageModel;
        private List<PublishedOffer> publishedOffers;
        private ImageSize imageSize;

        public CouponAdapter(Context context, IItemClickListener listener, int columns)
        {
            this.listener = listener;
            imageModel = new ImageModel(context);

            var dimension = context.Resources.DisplayMetrics.WidthPixels / columns;
            imageSize = new ImageSize(dimension, dimension);
        }

        public void SetCoupons(List<PublishedOffer> publishedOffers)
        {
            this.publishedOffers = publishedOffers;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get { return publishedOffers.Count; }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var storeViewHolder = viewHolder as CouponViewHolder;
            var publishedOffer = publishedOffers[position];

            if (storeViewHolder == null || publishedOffer == null)
            {
                return;
            }

            storeViewHolder.Title.Text = publishedOffer.Description;
            storeViewHolder.SubTitle.Text = publishedOffer.Details;

            if (storeViewHolder.Image != null && storeViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(storeViewHolder.Image);

                storeViewHolder.Image.Tag = null;

                if (publishedOffer.Images.Count > 0 && publishedOffer.Images[0] != null)
                {
                    var tag = publishedOffer.Images[0].Id;
                    storeViewHolder.Image.Tag = tag;

                    storeViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(publishedOffer.Images[0].GetAvgColor()));

                    var image = await imageModel.ImageGetById(publishedOffer.Images[0].Id, imageSize);

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

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CouponCardItem, parent);

            var vh = new CouponViewHolder(view, (pos) =>
            {
                var coupon = publishedOffers[pos];

                listener.ItemClicked(ItemType.Coupon, coupon.Id, "", 0, view, string.Empty);
            });

            return vh;
        }

        private class CouponViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }

            public CouponViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public CouponViewHolder(View view, Action<int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer = view.FindViewById<View>(Resource.Id.CouponListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.CouponListItemViewItemImage);
                Title = view.FindViewById<TextView>(Resource.Id.CouponListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.CouponListItemViewSubtitle);

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