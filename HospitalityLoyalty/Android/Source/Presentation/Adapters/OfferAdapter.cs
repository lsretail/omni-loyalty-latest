using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    public class OfferAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private ImageModel imageModel;
        private List<IOfferItem> offers = new List<IOfferItem>();
        private ImageSize imageSize;

        public OfferAdapter(Context context, IItemClickListener listener, int columns)
        {
            this.listener = listener;
            imageModel = new ImageModel(context);

            var dimension = context.Resources.DisplayMetrics.WidthPixels / columns;
            imageSize = new ImageSize(dimension, dimension);
        }

        public void SetOffers(List<PublishedOffer> offers)
        {
            this.offers.Clear();

            var pointOffers = new List<PublishedOffer>();
            var clubOffers = new List<PublishedOffer>();
            var memberOffers = new List<PublishedOffer>();
            var generalOffers = new List<PublishedOffer>();

            if (AppData.Contact != null && AppData.Contact.PublishedOffers != null)
            {
                foreach (var offer in AppData.Contact.PublishedOffers)
                {
                    if (offer.Type == OfferType.General)
                    {
                        generalOffers.Add(offer);
                    }
                    else if (offer.Type == OfferType.Club)
                    {
                        clubOffers.Add(offer);
                    }
                    else if (offer.Type == OfferType.PointOffer)
                    {

                        pointOffers.Add(offer);
                    }
                    else if (offer.Type == OfferType.SpecialMember)
                    {

                        memberOffers.Add(offer);
                    }
                }
            }

            if (pointOffers.Count > 0)
            {
                this.offers.Add(new OfferHeader(){Description = "Point offers"});
                pointOffers.ForEach(pointOffer => this.offers.Add(new OfferItem(){Offer = pointOffer}));
            }

            if (clubOffers.Count > 0)
            {
                this.offers.Add(new OfferHeader() { Description = "Club offers" });
                clubOffers.ForEach(clubOffer => this.offers.Add(new OfferItem() { Offer = clubOffer }));
            }

            if (memberOffers.Count > 0)
            {
                this.offers.Add(new OfferHeader() { Description = "Member offers" });
                memberOffers.ForEach(memberOffer => this.offers.Add(new OfferItem() { Offer = memberOffer }));
            }

            if (generalOffers.Count > 0)
            {
                this.offers.Add(new OfferHeader() { Description = "General offers" });
                generalOffers.ForEach(generalOffer => this.offers.Add(new OfferItem() { Offer = generalOffer }));
            }

            NotifyDataSetChanged();
        }

        public List<string> GetIds()
        {
            var offerIds = new List<string>();

            foreach (var offer in offers)
            {
                if (offer is OfferItem)
                {
                    var offerItem = offer as OfferItem;
                    offerIds.Add(offerItem.Offer.Id);
                }
            }

            return offerIds;
        } 

        public override int ItemCount
        {
            get { return offers.Count; }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            var item = offers[position];

            if (item is OfferHeader)
            {
                return maxColumns;
            }

            return 1;
        }

        public override int GetItemViewType(int position)
        {
            var item = offers[position];
            
            if (item is OfferHeader)
            {
                return 0;
            }

            return 1;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var viewType = GetItemViewType(position);
            var offerItem = offers[position];

            if (viewType == 0)
            {
                var storeViewHolder = viewHolder as OfferHeaderViewHolder;
                var offerHeader = (offerItem as OfferHeader);

                storeViewHolder.Description.Text = offerHeader.Description;
            }
            else if(viewType == 1)
            {
                var storeViewHolder = viewHolder as OfferViewHolder;
                var offer = (offerItem as OfferItem).Offer;

                storeViewHolder.Title.Text = offer.Description;
                storeViewHolder.SubTitle.Text = offer.Details;

                if (storeViewHolder.Image != null && storeViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(storeViewHolder.Image);

                    storeViewHolder.Image.Tag = null;

                    if (offer.Images.Count > 0 && offer.Images[0] != null)
                    {
                        var tag = offer.Images[0].Id;
                        storeViewHolder.Image.Tag = tag;

                        storeViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(offer.Images[0].GetAvgColor()));

                        var image = await imageModel.ImageGetById(offer.Images[0].Id, imageSize);

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
                var view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ListViewSectionHeader, parent);

                vh = new OfferHeaderViewHolder(view);
            }
            else if(viewType == 1)
            {
                View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CouponCardItem, parent);

                vh = new OfferViewHolder(view, (pos) =>
                {
                    var offer = (offers[pos] as OfferItem).Offer;

                    listener.ItemClicked(ItemType.Offer, offer.Id, "", 0, view, string.Empty);
                });
            }
            
            return vh;
        }

        private interface IOfferItem{}

        private class OfferItem : IOfferItem
        {
            public PublishedOffer Offer { get; set; }
        }

        private class OfferHeader : IOfferItem
        {
            public string Description { get; set; }
        }

        private class OfferHeaderViewHolder : RecyclerView.ViewHolder
        {
            public TextView Description { get; set; }

            public OfferHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public OfferHeaderViewHolder(View view)
                : base(view)
            {
                Description = view.FindViewById<TextView>(Resource.Id.ListViewSectionHeaderDescription);
            }
        }

        private class OfferViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }

            public OfferViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public OfferViewHolder(View view, Action<int> itemClicked)
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