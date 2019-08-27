using System;
using System.Collections.Generic;

using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Models;
using Presentation.Util;
using IItemClickListener = Presentation.Util.IItemClickListener;
using ImageView = Android.Widget.ImageView;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation.Adapters
{
    public class CheckoutAdapter : BaseRecyclerAdapter
    {
        private List<object> items = new List<object>();
        
        private ImageModel imageModel;
        private ImageSize imageSize;
        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                NotifyDataSetChanged();
            }
        }

        public CheckoutAdapter(Context context)
        {
            imageModel = new ImageModel(context, null);
            var imageHeight = context.Resources.GetDimensionPixelSize(Resource.Dimension.StackedListImageHeight);
            imageSize = new ImageSize(imageHeight, imageHeight);
        }

        public void SetItems(Context context, List<OneListItem> basketItems, View header = null, View footer = null)
        {
            this.items.Clear();

            if (header != null)
            {
                items.Add(header);
            }

            items.Add(new HeaderItem() { Title = context.GetString(Resource.String.ActionbarItems) });

            items.AddRange(basketItems);

            if (footer != null)
            {
                items.Add(footer);
            }

            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get
            {
                if (items == null)
                    return 0;
                if (isLoading)
                    return items.Count + 1;
                return items.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
            if (isLoading && position == items.Count)
            {
                return 4;
            }

            var item = items[position];

            if (position == 0 && item is View)
            {
                return 0;
            }

            if (item is HeaderItem)
            {
                return 1;
            }

            if (item is OneListItem)
            {
                return 2;
            }

            if (position == items.Count - 1 && item is View)
            {
                return 3;
            }

            return -1;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
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
            else if (holder is CheckoutViewHolder)
            {
                var basketItem = items[position] as OneListItem;
                var checkoutViewHolder = holder as CheckoutViewHolder;

                if (basketItem == null)
                {
                    return;
                }

                checkoutViewHolder.PrimaryText.Text = basketItem.ItemDescription;

                if (string.IsNullOrEmpty(basketItem.VariantId))
                {
                    checkoutViewHolder.SecondaryText.Visibility = ViewStates.Gone;
                }
                else
                {
                    checkoutViewHolder.SecondaryText.Visibility = ViewStates.Visible;
                    checkoutViewHolder.SecondaryText.Text = basketItem.VariantDescription;
                }

                if (basketItem.OnelistItemDiscounts.Count > 0)
                {
                    
                    checkoutViewHolder.DiscountText.Text = checkoutViewHolder.DiscountText.Context.GetString(Resource.String.CheckoutViewDiscount) + " " + AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketItem.GetDiscount());
                    checkoutViewHolder.DiscountText.Visibility = ViewStates.Visible;
                }
                else
                {
                    checkoutViewHolder.DiscountText.Visibility = ViewStates.Gone;
                }

                if (string.IsNullOrEmpty(basketItem.UnitOfMeasureId))
                {
                    checkoutViewHolder.Quantity.Text = checkoutViewHolder.Quantity.Context.GetString(Resource.String.ApplicationQty) + ": " + basketItem.Quantity.ToString() + " " + basketItem.UnitOfMeasureId.ToLower();
                }
                else
                {
                    checkoutViewHolder.Quantity.Text = checkoutViewHolder.Quantity.Context.GetString(Resource.String.ApplicationQty) + ": " + basketItem.Quantity.ToString("N0");
                }

                if (AppData.Basket.State == BasketState.Calculating)
                {
                    checkoutViewHolder.Amount.Text = string.Empty;
                }
                else
                {
                    checkoutViewHolder.Amount.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketItem.Amount);
                }

                if (checkoutViewHolder.Image != null && checkoutViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(checkoutViewHolder.Image);

                    checkoutViewHolder.Image.Tag = null;

                    if (basketItem.Image != null)
                    {
                        var tag = basketItem.Image.Id;
                        checkoutViewHolder.Image.Tag = tag;

                        checkoutViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(basketItem.Image.GetAvgColor()));

                        var image = await imageModel.ImageGetById(basketItem.Image.Id, imageSize);

                        if (image == null)
                        {
                            if (checkoutViewHolder?.Image != null && checkoutViewHolder.Image.Tag.ToString() == tag)
                            {
                                checkoutViewHolder.Image.Tag = null;

                                checkoutViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                            }
                        }
                        else
                        {
                            if (checkoutViewHolder?.Image != null && checkoutViewHolder.Image.Tag != null && checkoutViewHolder.Image.Tag.ToString() == tag)
                            {
                                checkoutViewHolder.Image.Tag = null;

                                Utils.ImageUtils.CrossfadeImage(checkoutViewHolder.Image, Utils.ImageUtils.DecodeImage(image.Image), checkoutViewHolder.ImageContainer, image.Crossfade);
                            }
                        }
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == 0)
            {
                var view = items[0] as View;

                var newParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                newParams.FullSpan = true;
                view.LayoutParameters = newParams;

                return new HeaderFooterViewHolder(view);
            }
            else if (viewType == 1)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardDisabledSectionHeader, parent, false);

                var newParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                newParams.FullSpan = true;
                view.LayoutParameters = newParams;

                return new HeaderViewHolder(view);
            }
            else if (viewType == 2)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CheckoutCardItem, parent, false);
                return new CheckoutViewHolder(view);
            }
            else if (viewType == 3)
            {
                var view = items[items.Count - 1] as View;

                var newParams = new StaggeredGridLayoutManager.LayoutParams(StaggeredGridLayoutManager.LayoutParams.MatchParent, StaggeredGridLayoutManager.LayoutParams.WrapContent);
                newParams.FullSpan = true;
                view.LayoutParameters = newParams;

                return new HeaderFooterViewHolder(view);
            }
            else if (viewType == 4)
            {
                var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.CardLoading, parent, false);

                var newParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                newParams.FullSpan = true;
                view.LayoutParameters = newParams;

                return new ProgressViewHolder(view);
            }

            return null;
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

        public class ProgressViewHolder : RecyclerView.ViewHolder
        {
            public ProgressViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public ProgressViewHolder(View itemView)
                : base(itemView)
            {
            }
        }

        public class HeaderFooterViewHolder : RecyclerView.ViewHolder
        {
            public HeaderFooterViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public HeaderFooterViewHolder(View itemView) : base(itemView)
            {
            }
        }

        public class CheckoutViewHolder : RecyclerView.ViewHolder
        {
            public FrameLayout ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView PrimaryText { get; set; }
            public TextView DiscountText { get; set; }
            public TextView SecondaryText { get; set; }
            public TextView Quantity { get; set; }
            public TextView Amount { get; set; }

            public CheckoutViewHolder(View view)
                : base(view)
            {
                Image = view.FindViewById<ImageView>(Resource.Id.CardBasketItemImage);
                ImageContainer = view.FindViewById<FrameLayout>(Resource.Id.CardBasketItemImageContainer);
                PrimaryText = view.FindViewById<TextView>(Resource.Id.CardBasketItemDescription);
                DiscountText = view.FindViewById<TextView>(Resource.Id.CardBasketItemDiscount);
                SecondaryText = view.FindViewById<TextView>(Resource.Id.CardBasketItemVariant);
                Quantity = view.FindViewById<TextView>(Resource.Id.CardBasketItemQuantity);
                Amount = view.FindViewById<TextView>(Resource.Id.CardBasketItemAmount);
            }
        }
    }
}