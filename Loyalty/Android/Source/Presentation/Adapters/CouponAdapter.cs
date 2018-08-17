using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Presentation.Models;
using Presentation.Util;
using ImageView = Android.Widget.ImageView;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Adapters
{
    public class CouponAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private readonly Action<string> onAddRemoveClicked;
        private List<PublishedOffer> coupons;
        private ListItemSize itemSize;

        private ImageModel imageModel;
        private ImageSize imageSize;

        public CouponAdapter(Context context, IItemClickListener listener, Action<string> onAddRemoveClicked, ListItemSize itemSize)
        {
            this.listener = listener;
            this.itemSize = itemSize;
            this.onAddRemoveClicked = onAddRemoveClicked;

            SetImageSize(context);

            imageModel = new ImageModel(context, null);
        }

        public void SetCoupons(List<PublishedOffer> coupons)
        {
            this.coupons = coupons;
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
                if (coupons == null)
                    return 0;
                return coupons.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
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
            var coupon = coupons[position];
            var couponViewHolder = holder as CouponViewHolder;

            if (coupon == null || couponViewHolder == null)
            {
                return;
            }

            couponViewHolder.Title.Text = coupon.Description;
            couponViewHolder.Subtitle.Text = coupon.Details;

            SetAddRemoveImage(couponViewHolder.AddRemoveButton, coupon.Selected);

            if (couponViewHolder.Image != null && couponViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(couponViewHolder.Image);

                couponViewHolder.Image.Tag = null;

                if (coupon.Images != null && coupon.Images.Count > 0)
                {
                    var tag = coupon.Images[0].Id;
                    couponViewHolder.Image.Tag = tag;

                    if (itemSize == ListItemSize.Normal)
                    {
                        var backgroundCircle = new ShapeDrawable(new OvalShape());
                        backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(coupon.Images[0].GetAvgColor());

                        couponViewHolder.ImageContainer.Background = backgroundCircle;
                    }
                    else if (itemSize == ListItemSize.SmallCard)
                    {
                        couponViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(coupon.Images[0].GetAvgColor()));
                    }

                    var image = await imageModel.ImageGetById(coupon.Images[0].Id, imageSize);

                    if (image == null)
                    {
                        if (couponViewHolder?.Image != null && couponViewHolder.Image.Tag.ToString() == tag)
                        {
                            couponViewHolder.Image.Tag = null;

                            couponViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (couponViewHolder?.Image != null && couponViewHolder.Image.Tag.ToString() == tag)
                        {
                            couponViewHolder.Image.Tag = null;

                            if (itemSize == ListItemSize.Normal)
                            {
                                Utils.ImageUtils.CrossfadeImage(couponViewHolder.Image, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(image.Image)), couponViewHolder.ImageContainer, image.Crossfade);
                            }
                            else
                            {
                                Utils.ImageUtils.CrossfadeImage(couponViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), couponViewHolder.ImageContainer, image.Crossfade);
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
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CouponHeaderListItem, parent, false);
            }
            else if (viewType == 1)
            {
                view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CouponHeaderCardItem, parent, false);
            }

            var vh = new CouponViewHolder(view, (type, pos) =>
            {
                var coupon = coupons[pos];

                switch (type)
                {
                    case CouponType.AddRemove:
                        onAddRemoveClicked(coupon.Id);
                        break;

                    case CouponType.Item:
                        listener.ItemClicked((int)ItemClickType.Coupon, coupon.Id, string.Empty, view);
                        break;
                }

                
            });

            return vh;
        }

        public enum CouponType
        {
            Item = 0,
            AddRemove = 1,
        }

        public class CouponViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<CouponType, int> itemClicked;

            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public ImageButton AddRemoveButton { get; set; }

            public CouponViewHolder(View view, Action<CouponType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.CouponTitleText);
                Subtitle = view.FindViewById<TextView>(Resource.Id.CouponBodyText);
                ImageContainer = view.FindViewById<View>(Resource.Id.ItemsListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ItemsListItemViewItemImage);
                AddRemoveButton = view.FindViewById<ImageButton>(Resource.Id.CouponHeaderListItemAddRemove);

                AddRemoveButton.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.CouponHeaderListItemAddRemove:
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