using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation.Adapters
{
    public class BasketAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private ObservableCollection<OneListItem> basketItems;

        public BasketAdapter(IItemClickListener listener)
        {
            this.listener = listener;
        }

        public void SetBasketItems(ObservableCollection<OneListItem> basketItems)
        {
            this.basketItems = basketItems;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get
            {
                if (basketItems == null)
                    return 0;
                return basketItems.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var basketItem = basketItems[position];
            var basketViewHolder = holder as BasketViewHolder;

            if (basketItem == null || basketViewHolder == null)
            {
                return;
            }

            basketViewHolder.Title.Text = basketItem.ItemDescription;

            if (string.IsNullOrEmpty(basketItem.VariantId) == false)
            {
                basketViewHolder.Subtitle.Text = basketItem.VariantDescription;
                basketViewHolder.Subtitle.Visibility = ViewStates.Visible;
            }
            else
            {
                basketViewHolder.Subtitle.Visibility = ViewStates.Gone;
            }

            if (string.IsNullOrEmpty(basketItem.UnitOfMeasureId) == false)
            {
                basketViewHolder.Qty.Text = string.Format(basketViewHolder.Qty.Context.GetString(Resource.String.ApplicationQtyN), basketItem.Quantity.ToString() + " " + basketItem.UnitOfMeasureId);
            }
            else
            {
                basketViewHolder.Qty.Text = string.Format(basketViewHolder.Qty.Context.GetString(Resource.String.ApplicationQtyN), basketItem.Quantity.ToString("N0"));
            }

            if (AppData.Basket.State != BasketState.Calculating)
            {
                basketViewHolder.Price.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketItem.Amount);

                basketViewHolder.PriceProgressBar.Visibility = ViewStates.Gone;
                basketViewHolder.Price.Visibility = ViewStates.Visible;
            }
            else
            {
                basketViewHolder.Price.Visibility = ViewStates.Gone;
                basketViewHolder.PriceProgressBar.Visibility = ViewStates.Visible;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.BasketItem, parent, false);

            var vh = new BasketViewHolder(view, (pos) =>
            {
                var basketItem = basketItems[pos];
                listener.ItemClicked((int)ItemClickType.BasketItem, basketItem.Id, string.Empty, view);
            });

            return vh;
        }

        public class BasketViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public TextView Qty { get; set; }
            public TextView Price { get; set; }
            public ProgressBar PriceProgressBar { get; set; }

            public BasketViewHolder(View view, Action<int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.BasketItemDescription);
                Subtitle = view.FindViewById<TextView>(Resource.Id.BasketItemVariants);
                Qty = view.FindViewById<TextView>(Resource.Id.BasketItemQuantity);
                Price = view.FindViewById<TextView>(Resource.Id.BasketItemPrice);
                PriceProgressBar = view.FindViewById<ProgressBar>(Resource.Id.BasketItemPriceProgressBar);

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