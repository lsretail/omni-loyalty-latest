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
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Adapters
{
    public class QRCodeAdapter : BaseRecyclerAdapter
    {
        private List<object> items = new List<object>();

        private ImageModel imageModel;
        private ImageSize imageSize;

        public QRCodeAdapter(Context context)
        {

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetItems(Context context)
        {
            this.items.Clear();

            if (EnabledItems.HasCoupons)
            {
                items.Add(new HeaderItem() {Title = context.GetString(Resource.String.ActionbarCoupons)});

                items.AddRange(AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Selected && x.Code == OfferDiscountType.Coupon).ToList());
            }

            if (EnabledItems.HasOffers)
            {
                items.Add(new HeaderItem() {Title = context.GetString(Resource.String.ActionbarOffers)});

                items.AddRange(AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Type == OfferType.PointOffer && x.Code != OfferDiscountType.Coupon && x.Selected).ToList());
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
            if (item is PublishedOffer)        //list
            {
                return 1;
            }
            return -1;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
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
            else if (holder is OfferAdapter.OfferViewHolder)
            {
                var offer = items[position] as PublishedOffer;
                var offerViewHolder = holder as OfferAdapter.OfferViewHolder;

                if (offer == null)
                {
                    return;
                }

                offerViewHolder.Title.Text = offer.Description;
                offerViewHolder.Subtitle.Text = offer.Details;

                if (offer.Images != null && offer.Images.Count > 0)
                {
                    LoadImage(offerViewHolder.Image, offerViewHolder.ImageContainer, offer.Images[0]);
                }
                else
                {
                    Utils.ImageUtils.ClearImageView(offerViewHolder.Image);
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == 0)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardDisabledSectionHeader, parent, false);
                
                return new HeaderViewHolder(view);
            }
            if (viewType == 1)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.OfferHeaderListItem, parent, false);

                var holder =  new OfferAdapter.OfferViewHolder(view, (type, pos) => { });

                holder.AddRemoveButton.Visibility = ViewStates.Gone; 

                return holder;
            }
            else if (viewType == 2)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CouponHeaderListItem, parent, false);

                var holder =  new CouponAdapter.CouponViewHolder(view, (type, pos) => { });

                holder.AddRemoveButton.Visibility = ViewStates.Gone; 

                return holder;
            }

            return null;
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
                        if (imageView != null && imageView.Tag.ToString() == tag)
                        {
                            imageView.Tag = null;

                            imageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (imageView != null && imageView.Tag.ToString() == tag)
                        {
                            if (imageView.Tag != null && imageView.Tag.ToString() == tag)
                            {
                                imageView.Tag = null;

                                Utils.ImageUtils.CrossfadeImage(imageView, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(loadedImage.Image)), imageContainer, loadedImage.Crossfade);
                            }
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