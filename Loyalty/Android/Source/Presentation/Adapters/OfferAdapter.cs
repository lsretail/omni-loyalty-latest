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
using System.Linq;
using ImageView = Android.Widget.ImageView;
using Object = Java.Lang.Object;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Adapters
{
    public class OfferAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private readonly Action<string> onAddRemoveClicked;
        private List<object> offers = new List<object>();
        private ListItemSize itemSize;

        private ImageModel imageModel;
        private ImageSize imageSize;

        public OfferAdapter(Context context, IItemClickListener listener, Action<string> onAddRemoveClicked, ListItemSize itemSize)
        {
            this.listener = listener;
            this.itemSize = itemSize;
            this.onAddRemoveClicked = onAddRemoveClicked;

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetOffers(Context context, List<PublishedOffer> offers)
        {
            this.offers.Clear();

            var pointOffers = offers.Where(x => x.Type == OfferType.PointOffer).ToList();
            var clubOffers = offers.Where(x => x.Type == OfferType.Club).ToList();
            var memberOffers = offers.Where(x => x.Type == OfferType.SpecialMember).ToList();
            var generalOffers = offers.Where(x => x.Type == OfferType.General).ToList();

            if (pointOffers != null && pointOffers.Count > 0)
            {
                this.offers.Add(new HeaderItem()
                {
                    Title = context.GetString(Resource.String.PointOffers)
                });

                this.offers.AddRange(pointOffers);
            }

            if (clubOffers != null && clubOffers.Count > 0)
            {
                this.offers.Add(new HeaderItem()
                {
                    Title = context.GetString(Resource.String.ClubOffers)
                });

                this.offers.AddRange(clubOffers);
            } 
            
            if (memberOffers != null && memberOffers.Count > 0)
            {
                this.offers.Add(new HeaderItem()
                {
                    Title = context.GetString(Resource.String.MemberOffers)
                });

                this.offers.AddRange(memberOffers);
            }

            if (generalOffers != null && generalOffers.Count > 0)
            {
                this.offers.Add(new HeaderItem()
                {
                    Title = context.GetString(Resource.String.GeneralOffers)
                });

                this.offers.AddRange(generalOffers);
            }

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
                if (offers == null)
                    return 0;
                return offers.Count;
            }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            var item = offers[position];

            if (item is HeaderItem)
            {
                return maxColumns;
            }

            return base.GetColumnSpan(position, maxColumns);
        }

        public override int GetItemViewType(int position)
        {
            if (offers[position] is HeaderItem)
            {
                return 0;
            }
            if (itemSize == ListItemSize.Normal)        //list
            {
                return 1;
            }
            else                                        //card
            {
                return 2;
            }
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var type = GetItemViewType(position);

            if (type == 0)
            {
                var headerItem = offers[position] as HeaderItem;
                var headerViewHolder = holder as HeaderViewHolder;

                if (headerItem == null || headerViewHolder == null)
                {
                    return;
                }

                headerViewHolder.Title.Text = headerItem.Title;
            }
            else if (type == 1 || type == 2)
            {
                var offer = offers[position] as PublishedOffer;
                var offerViewHolder = holder as OfferViewHolder;

                if (offer == null || offerViewHolder == null)
                {
                    return;
                }

                offerViewHolder.Title.Text = offer.Description;
                offerViewHolder.Subtitle.Text = offer.Details;

                if (offer.Type == OfferType.PointOffer)
                {
                    SetAddRemoveImage(offerViewHolder.AddRemoveButton, offer.Selected);
                    offerViewHolder.AddRemoveButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    offerViewHolder.AddRemoveButton.Visibility = ViewStates.Gone;
                }

                if (offerViewHolder.Image != null && offerViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(offerViewHolder.Image);

                    offerViewHolder.Image.Tag = null;

                    if (offer.Images != null && offer.Images.Count > 0)
                    {
                        var tag = offer.Images[0].Id;
                        offerViewHolder.Image.Tag = tag;

                        if (itemSize == ListItemSize.Normal)
                        {
                            var backgroundCircle = new ShapeDrawable(new OvalShape());
                            backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(offer.Images[0].GetAvgColor());

                            offerViewHolder.ImageContainer.Background = backgroundCircle;
                        }
                        else if (itemSize == ListItemSize.SmallCard)
                        {
                            offerViewHolder.ImageContainer.SetBackgroundColor(
                                Android.Graphics.Color.ParseColor(offer.Images[0].GetAvgColor()));
                        }

                        var image = await imageModel.ImageGetById(offer.Images[0].Id, imageSize);

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

                                if (itemSize == ListItemSize.Normal)
                                {
                                    Utils.ImageUtils.CrossfadeImage(offerViewHolder.Image, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(image.Image)), offerViewHolder.ImageContainer, image.Crossfade);
                                }
                                else
                                {
                                    Utils.ImageUtils.CrossfadeImage(offerViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), offerViewHolder.ImageContainer, image.Crossfade);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetAddRemoveImage(ImageButton addRemoveButton, bool isSelected)
        {
            if (isSelected)
                addRemoveButton.SetImageResource(Resource.Drawable.ic_action_remove);
            else
                addRemoveButton.SetImageResource(Resource.Drawable.ic_action_new);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = null;

            if (viewType == 0)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardDisabledSectionHeader, parent, false);
                
                return new HeaderViewHolder(view);
            }
            if (viewType == 1)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.OfferHeaderListItem, parent, false);
            }
            else if (viewType == 2)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.OfferHeaderCardItem, parent, false);//todo
            }

            return new OfferViewHolder(view, (type, pos) =>
            {
                var offer = offers[pos];

                switch (type)
                {
                    case CouponType.AddRemove:
                        onAddRemoveClicked((offer as PublishedOffer).Id);
                        break;

                    case CouponType.Item:
                        listener.ItemClicked((int)ItemClickType.Offer, (offer as PublishedOffer).Id, string.Empty, view);
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