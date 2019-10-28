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

using ImageView = Android.Widget.ImageView;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Adapters
{
    public class ShoppingListDetailAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private readonly Action<string, View> onOverFlowClicked;
        private ImageModel imageModel;
        private List<OneListItem> wishListLines;
        private ImageSize imageSize;

        public ShoppingListDetailAdapter(Context context, IItemClickListener listener, Action<string, View> onOverFlowClicked)
        {
            this.listener = listener;
            this.onOverFlowClicked = onOverFlowClicked;

            imageModel = new ImageModel(context, null);

            var imgHeight = context.Resources.GetDimensionPixelSize(Resource.Dimension.ListImageHeight);
            imageSize = new ImageSize(imgHeight, imgHeight);
        }

        public void SetWishListLines(List<OneListItem> wishListLines)
        {
            this.wishListLines = wishListLines;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get
            {
                if (wishListLines == null)
                    return 0;
                return wishListLines.Count;
            }
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var shoppingListLineViewHolder = viewHolder as ShoppingListLineViewHolder;
            var wishListLine = wishListLines[position];

            if (shoppingListLineViewHolder == null || wishListLine == null)
            {
                return;
            }

            shoppingListLineViewHolder.Title.Text = wishListLine.ItemDescription;

            if (string.IsNullOrEmpty(wishListLine.VariantId) == false)
            {
                shoppingListLineViewHolder.Variant.Visibility = ViewStates.Visible;
                shoppingListLineViewHolder.Variant.Text = wishListLine.VariantDescription;
            }
            else
            {
                shoppingListLineViewHolder.Variant.Visibility = ViewStates.Gone;
            }

            if (!EnabledItems.HasBasket && !EnabledItems.HasItemCatalog)
            {
                shoppingListLineViewHolder.Overflow.Visibility = ViewStates.Invisible;
            }

            if (shoppingListLineViewHolder.Image != null && shoppingListLineViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(shoppingListLineViewHolder.Image);

                shoppingListLineViewHolder.Image.Tag = null;

                if (wishListLine.Image != null)
                {
                    var tag = wishListLine.Image.Id;
                    shoppingListLineViewHolder.Image.Tag = tag;

                    var backgroundCircle = new ShapeDrawable(new OvalShape());
                    backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(wishListLine.Image.GetAvgColor());

                    shoppingListLineViewHolder.ImageContainer.Background = backgroundCircle;

                    var image = await imageModel.ImageGetById(wishListLine.Image.Id, imageSize);

                    if (image == null)
                    {
                        if (shoppingListLineViewHolder?.Image != null && shoppingListLineViewHolder.Image.Tag.ToString() == tag)
                        {
                            shoppingListLineViewHolder.Image.Tag = null;

                            shoppingListLineViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (shoppingListLineViewHolder?.Image != null && shoppingListLineViewHolder.Image.Tag != null && shoppingListLineViewHolder.Image.Tag.ToString() == tag)
                        {
                            shoppingListLineViewHolder.Image.Tag = null;

                            Utils.ImageUtils.CrossfadeImage(shoppingListLineViewHolder.Image, new Util.Utils.ImageUtils.CircleDrawable(Utils.ImageUtils.DecodeImage(image.Image)), shoppingListLineViewHolder.ImageContainer, image.Crossfade);
                        }
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.WishListListItem, parent, false);

            var vh = new ShoppingListLineViewHolder(view, (type, pos, clickedView) =>
            {
                var shoppingListLine = wishListLines[pos];

                if (type == (int) ShoppingListLineItemType.Overflow)
                {
                    onOverFlowClicked(shoppingListLine.Id, clickedView);
                }
                else
                {
                    listener.ItemClicked((int)ItemClickType.ShoppingListLine, shoppingListLine.Id, string.Empty, view);
                }
            });

            return vh;
        }

        public enum ShoppingListLineItemType
        {
            Item = 0,
            Overflow = 1
        }

        public class ShoppingListLineViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int, int, View> itemClicked;

            public FrameLayout ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView Variant { get; set; }
            public ImageButton Overflow { get; set; }

            public ShoppingListLineViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public ShoppingListLineViewHolder(View view, Action<int, int, View> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer = view.FindViewById<FrameLayout>(Resource.Id.ShoppingListDetailListItemViewImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ShoppingListDetailListItemViewImage);
                Title = view.FindViewById<TextView>(Resource.Id.ShoppingListDetailListItemViewItemName);
                Variant = view.FindViewById<TextView>(Resource.Id.ShoppingListDetailListItemViewVariantName);
                Overflow = view.FindViewById<ImageButton>(Resource.Id.ShoppingListDetailListItemViewItemOverflow);

                Overflow.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.ShoppingListDetailListItemViewItemOverflow:
                        itemClicked((int)ShoppingListLineItemType.Overflow, AdapterPosition, v);
                        break;

                    default:
                        itemClicked((int) ShoppingListLineItemType.Item, AdapterPosition, v);
                        break;
                }
            }
        }
    }
}