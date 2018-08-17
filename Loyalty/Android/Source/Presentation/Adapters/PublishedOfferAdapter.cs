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
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Adapters
{
    public class PublishedOfferAdapter : RecyclerView.Adapter
    {
        private readonly IItemClickListener listener;
        private List<PublishedOffer> publishedOffers = new List<PublishedOffer>();

        private ImageModel imageModel;
        private ImageSize imageSize;

        public PublishedOfferAdapter(Context context, IItemClickListener listener)
        {
            this.listener = listener;

            var height = context.Resources.GetDimensionPixelSize(Resource.Dimension.CardImageHeight);
            imageSize = new ImageSize(height, height);

            imageModel = new ImageModel(context, null);
        }

        public void SetOffers(Context context, List<PublishedOffer> publishedOffers)
        {
            this.publishedOffers = publishedOffers;

            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get
            {
                if (publishedOffers == null)
                    return 0;
                return publishedOffers.Count;
            }
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var publishedOffer = publishedOffers[position];
            var offerViewHolder = holder as OfferViewHolder;

            if (publishedOffer == null || offerViewHolder == null)
            {
                return;
            }

            offerViewHolder.Title.Text = publishedOffer.Description;
            offerViewHolder.Subtitle.Text = publishedOffer.Details;

            offerViewHolder.AddRemoveButton.Visibility = ViewStates.Gone;

            if (offerViewHolder.Image != null && offerViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(offerViewHolder.Image);

                offerViewHolder.Image.Tag = null;

                if (publishedOffer.Images != null && publishedOffer.Images.Count > 0)
                {
                    var tag = publishedOffer.Images[0].Id;
                    offerViewHolder.Image.Tag = tag;

                    offerViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(publishedOffer.Images[0].GetAvgColor()));

                    var image = await imageModel.ImageGetById(publishedOffer.Images[0].Id, imageSize);

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

                            Utils.ImageUtils.CrossfadeImage(offerViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), offerViewHolder.ImageContainer, image.Crossfade);
                        }
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.OfferHeaderCardItem, parent, false);//todo

            view.LayoutParameters.Width = view.Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardImageHeight);

            return new OfferViewHolder(view, (type, pos) =>
            {
                var publishedOffer = publishedOffers[pos];

                switch (type)
                {
                    case CouponType.Item:
                        listener.ItemClicked((int)ItemClickType.Offer, publishedOffer.Id, string.Empty, view);
                        break;
                }
            });
        }

        private class HeaderItem
        {
            public string Title { get; set; }
        }

        public enum CouponType
        {
            Item = 0,
            AddRemove = 1,
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

        public class OfferViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<CouponType, int> itemClicked;

            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public ImageButton AddRemoveButton { get; set; }

            public OfferViewHolder(View view, Action<CouponType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.OfferTitleText);
                Subtitle = view.FindViewById<TextView>(Resource.Id.OfferBodyText);
                ImageContainer = view.FindViewById<View>(Resource.Id.ItemsListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ItemsListItemViewItemImage);
                AddRemoveButton = view.FindViewById<ImageButton>(Resource.Id.OfferHeaderListItemAddRemove);

                AddRemoveButton.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.OfferHeaderListItemAddRemove:
                        itemClicked(CouponType.AddRemove, AdapterPosition);
                        break;

                    default:
                        itemClicked(CouponType.Item, AdapterPosition);
                        break;
                }
            }
        }
    }
}