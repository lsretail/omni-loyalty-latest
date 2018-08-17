using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V7.Widget;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Menu;
using Presentation.Activities.Offer;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Adapters
{
    public class BasketAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private readonly Action<int> scrollToBottom;

        private MenuService menuService;
        private LocalBasketService basketService;

        private BasketModel basketModel;
        private FavoriteModel favoriteModel;
        private List<IBasketItem> basketItems = new List<IBasketItem>();

        private Context context;

        public BasketAdapter(Context context, IItemClickListener listener, Action<int> scrollToBottom)
        {
            menuService = new MenuService();
            basketService = new LocalBasketService();

            this.context = context;
            this.listener = listener;
            this.scrollToBottom = scrollToBottom;

            basketModel = new BasketModel(context);
            favoriteModel = new FavoriteModel(context);

            CreateItems();
        }
        
        public override int GetItemViewType(int position)
        {
            var item = basketItems[position];

            if(item is ListHeaderItem)
            {
                return 0;
            }

            if (item is BasketLineItem)
            {
                return 1;
            }

            return 2;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            int viewType = GetItemViewType(position);

            if (viewType == 0)
            {
                var headerItem = (basketItems[position] as ListHeaderItem);
                var listHeaderViewHolder = viewHolder as ListHeaderViewHolder;

                listHeaderViewHolder.Description.Text = headerItem.Description.ToUpper();
            }
            else if (viewType == 1)
            {
                var basketItem = (basketItems[position] as BasketLineItem).BasketItem;
                var basketViewHolder = viewHolder as BasketLineItemViewHolder;

                basketViewHolder.Description.Text = basketItem.Item.Description;

                if (menuService.HasAnyModifiers(AppData.MobileMenu, basketItem.Item))
                {
                    basketViewHolder.Edit.Visibility = ViewStates.Visible;

                    basketViewHolder.Increase.Visibility = ViewStates.Gone;
                    basketViewHolder.Decrease.Visibility = ViewStates.Gone;
                }
                else
                {
                    basketViewHolder.Edit.Visibility = ViewStates.Gone;

                    basketViewHolder.Increase.Visibility = ViewStates.Visible;
                    basketViewHolder.Decrease.Visibility = ViewStates.Visible;
                }

                if (favoriteModel.IsFavorite(basketItem.Item))
                {
                    basketViewHolder.Favorite.SetImageResource(Resource.Drawable.ic_favorite_black_24dp);
                }
                else
                {
                    basketViewHolder.Favorite.SetImageResource(Resource.Drawable.ic_favorite_border_black_24dp);
                }

                var favoriteDrawable = DrawableCompat.Wrap(basketViewHolder.Favorite.Drawable);
                DrawableCompat.SetTint(favoriteDrawable, new Color(ContextCompat.GetColor(basketViewHolder.Favorite.Context, Resource.Color.black87)));
                DrawableCompat.Unwrap(favoriteDrawable);

                //basketViewHolder.Favorite.SetColorFilter(Utils.Utils.GetColorFilter(context.Resources.GetColor(Resource.Color.secondarytextcolor)));

                var extraInfo = Utils.Utils.GenerateItemExtraInfo(context, menuService, AppData.MobileMenu, basketItem.Item);

                basketViewHolder.SecondaryText.Text = extraInfo;

                if (string.IsNullOrEmpty(extraInfo))
                {
                    basketViewHolder.SecondaryText.Visibility = ViewStates.Gone;
                }
                else
                {
                    basketViewHolder.SecondaryText.Visibility = ViewStates.Visible;
                }

                basketViewHolder.Price.Text = AppData.FormatCurrency(basketService.GetBasketItemFullPrice(AppData.MobileMenu, basketItem));
                basketViewHolder.Qty.Text = string.Format(context.Resources.GetString(Resource.String.BasketQty), basketItem.Quantity.ToString());

                if (basketItem.Selected)
                {
                    basketViewHolder.Menu.Visibility = ViewStates.Visible;
                }
                else
                {
                    basketViewHolder.Menu.Visibility = ViewStates.Gone;
                }
            }
            else if (viewType == 2)
            {
                var offerCouponViewHolder = viewHolder as OfferCouponViewHolder;

                if (basketItems[position] is OfferLineItem)
                {
                    var offer = (basketItems[position] as OfferLineItem).Offer;

                    offerCouponViewHolder.Description.Text = offer.Description;
                    offerCouponViewHolder.Details.Text = offer.Details;
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder vh = null;

            if (viewType == 0)
            {
                var view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ListViewSectionHeader, parent);
                
                vh = new ListHeaderViewHolder(view);
            }
            else if (viewType == 1)
            {
                var view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.BasketItem, parent);
                
                vh = new BasketLineItemViewHolder(view, async (type, pos) =>
                {
                    var basketItem = basketItems[pos] as BasketLineItem;

                    if (type == ItemClickedType.Item)
                    {
                        //itemType = ItemType.MenuGroup;

                        var selectedVal = basketItem.BasketItem.Selected;
                        int previousPos = -1;

                        for (int i = 0; i < AppData.Basket.Items.Count; i++)
                        {
                            if (AppData.Basket.Items[i].Selected)
                            {
                                AppData.Basket.Items[i].Selected = false;
                                previousPos = i;
                                break;
                            }
                        }

                        AppData.Basket.Items.ForEach(x => x.Selected = false);

                        basketItem.BasketItem.Selected = !selectedVal;

                        CreateItems();

                        NotifyItemRangeChanged(0, basketItems.Count);

                        //if (pos == ItemCount - 1)
                        {
                            scrollToBottom(pos);
                        }
                    }
                    else if (type == ItemClickedType.Delete)
                    {
                        basketModel.DeleteItem(basketItem.BasketItem);
                        CreateItems();
                        NotifyItemRemoved(pos);
                    }
                    else if (type == ItemClickedType.Edit)
                    {
                        basketItem.BasketItem.Selected = false;

                        listener.ItemClicked(ItemType.UpdateBasket, basketItem.BasketItem.Id, string.Empty, 0, view);
                    }
                    else if (type == ItemClickedType.Favorite)
                    {
                        await favoriteModel.ToggleFavorite(basketItem.BasketItem.Item);
                        NotifyDataSetChanged();
                    }
                    else if (type == ItemClickedType.Increase)
                    {
                        basketModel.ChangeQty(basketItem.BasketItem, Math.Min(basketItem.BasketItem.Quantity + 1, AppData.MaxItems));
                        NotifyItemChanged(pos);
                    }
                    else if (type == ItemClickedType.Decrease)
                    {
                        basketModel.ChangeQty(basketItem.BasketItem, Math.Max(basketItem.BasketItem.Quantity - 1, 1));
                        NotifyItemChanged(pos);
                    }
                });
            }
            else if (viewType == 2)
            {
                var view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.BasketCoupon, parent);
                
                vh = new OfferCouponViewHolder(view);
            }

            return vh;
        }

        public override int ItemCount
        {
            get { return basketItems.Count; }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public void CreateItems()
        {
            basketItems.Clear();

            if (AppData.Basket.Items != null && AppData.Basket.Items.Count > 0)
            {
                basketItems.Add(new ListHeaderItem() { Description = context.Resources.GetString(Resource.String.Items) });

                foreach (var basketItem in AppData.Basket.Items)
                {
                    basketItems.Add(new BasketLineItem() { BasketItem = basketItem });
                }
            }

            if (AppData.Basket.PublishedOffers != null && AppData.Basket.PublishedOffers.Count > 0)
            {
                basketItems.Add(new ListHeaderItem() { Description = context.Resources.GetString(Resource.String.Offers) });

                foreach (var offer in AppData.Basket.PublishedOffers)
                {
                    basketItems.Add(new OfferLineItem() { Offer = offer });
                }
            }
        }

        public interface IBasketItem
        {
            
        }

        public class ListHeaderItem : IBasketItem
        {
            public string Description { get; set; }

            public ListHeaderItem()
            {
                Description = string.Empty;
            }
        }

        public class BasketLineItem : IBasketItem
        {
            public BasketItem BasketItem { get; set; }
        }

        public class OfferLineItem : IBasketItem
        {
            public PublishedOffer Offer { get; set; }
        }

        private enum ItemClickedType
        {
            Item,
            Delete,
            Edit,
            Favorite,
            Increase,
            Decrease
        }

        private class ListHeaderViewHolder : RecyclerView.ViewHolder
        {
            public TextView Description { get; set; }

            public ListHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }
            
            public ListHeaderViewHolder(View view) : base(view)
            {
                Description = view.FindViewById<TextView>(Resource.Id.ListViewSectionHeaderDescription);
            }
        }

        private class BasketLineItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public TextView Description { get; set; }
            public TextView SecondaryText { get; set; }
            public TextView Price { get; set; }
            public TextView Qty { get; set; }

            public View Menu { get; set; }
            
            public Button Delete { get; set; }
            public View Edit { get; set; }
            public ImageButton Favorite { get; set; }
            public ImageButton Increase { get; set; }
            public ImageButton Decrease { get; set; }

            public BasketLineItemViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public BasketLineItemViewHolder(View view, Action<ItemClickedType, int> itemClicked) : base(view)
            {
                this.itemClicked = itemClicked;

                Description = view.FindViewById<TextView>(Resource.Id.BasketItemDescription);
                SecondaryText = view.FindViewById<TextView>(Resource.Id.BasketItemVariants);
                Price = view.FindViewById<TextView>(Resource.Id.BasketItemPrice);
                Qty = view.FindViewById<TextView>(Resource.Id.BasketItemQuantity);

                Menu = view.FindViewById<View>(Resource.Id.BasketItemMenu);

                Delete = view.FindViewById<Button>(Resource.Id.BasketItemDelete);
                Edit = view.FindViewById<View>(Resource.Id.BasketItemEdit);
                Favorite = view.FindViewById<ImageButton>(Resource.Id.BasketItemFavorite);
                Increase = view.FindViewById<ImageButton>(Resource.Id.BasketItemIncreaseQty);
                Decrease = view.FindViewById<ImageButton>(Resource.Id.BasketItemDecreaseQty);

                Delete.SetOnClickListener(this);
                Edit.SetOnClickListener(this);
                Favorite.SetOnClickListener(this);
                Increase.SetOnClickListener(this);
                Decrease.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                if (v.Id == Resource.Id.BasketItemDelete)
                {
                    itemClicked(ItemClickedType.Delete, AdapterPosition);
                }
                else if (v.Id == Resource.Id.BasketItemEdit)
                {
                    itemClicked(ItemClickedType.Edit, AdapterPosition);
                }
                else if (v.Id == Resource.Id.BasketItemFavorite)
                {
                    itemClicked(ItemClickedType.Favorite, AdapterPosition);
                }
                else if (v.Id == Resource.Id.BasketItemIncreaseQty)
                {
                    itemClicked(ItemClickedType.Increase, AdapterPosition);
                }
                else if (v.Id == Resource.Id.BasketItemDecreaseQty)
                {
                    itemClicked(ItemClickedType.Decrease, AdapterPosition);
                }
                else
                {
                    itemClicked(ItemClickedType.Item, AdapterPosition);
                }
            }
        }

        private class OfferCouponViewHolder : RecyclerView.ViewHolder
        {
            public TextView Description { get; set; }
            public TextView Details { get; set; }

            public OfferCouponViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public OfferCouponViewHolder(View view) : base(view)
            {
                Description = view.FindViewById<TextView>(Resource.Id.BasketCouponDescription);
                Details = view.FindViewById<TextView>(Resource.Id.BasketCouponDetails);
            }
        }
    }
}