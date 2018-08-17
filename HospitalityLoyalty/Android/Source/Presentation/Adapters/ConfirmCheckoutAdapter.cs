using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Domain.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Menu;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Adapters
{
    public class ConfirmCheckoutAdapter : BaseRecyclerAdapter
    {
        private Context context;

        private MenuService menuService;
        private LocalBasketService basketService;

        private ImageModel imageModel;
        private BasketModel basketModel;
        private FavoriteModel favoriteModel;

        private readonly IItemClickListener listener;
        private List<IBasketItem> basketItems = new List<IBasketItem>();
        private ImageSize imageSize;

        private string price = string.Empty;

        public ConfirmCheckoutAdapter(Context context, IItemClickListener listener, int columns)
        {
            menuService = new MenuService();
            basketService = new LocalBasketService();

            this.context = context;
            this.listener = listener;
            imageModel = new ImageModel(context);
            basketModel = new BasketModel(context);
            favoriteModel = new FavoriteModel(context);

            var dimension = context.Resources.GetDimensionPixelSize(Resource.Dimension.CardThumbnailImageWidth);
            imageSize = new ImageSize(dimension, dimension);
        }

        public void SetBasket(Basket basket)
        {
            basketItems.Clear();

            if (basket.Items.Count > 0)
            {
                basketItems.Add(new BasketHeaderitem()
                {
                    Description = context.Resources.GetString(Resource.String.Items)
                });

                foreach (var basketItem in basket.Items)
                {
                    basketItems.Add(new BasketMenuItem()
                    {
                        Item = basketItem
                    });
                }
            }

            if (AppData.Contact != null && AppData.Contact.PublishedOffers != null && AppData.Contact.PublishedOffers.Count > 0)
            {
                basketItems.Add(new BasketHeaderitem()
                {
                    Description = context.Resources.GetString(Resource.String.Offers)
                });

                foreach (var offer in AppData.Contact.PublishedOffers)
                {
                    basketItems.Add(new BasketOfferItem()
                    {
                        Offer = offer
                    });
                }
            }

            NotifyDataSetChanged();
        }

        public void UpdatePrice(string amount)
        {
            price = amount;
            NotifyItemChanged(0);
        }

        public override int ItemCount
        {
            get { return basketItems.Count + 1; }
        }

        private int RealPosition(int position)
        {
            if (position == 0)
                return 0;

            return position - 1;
        }

        public override int GetItemViewType(int pos)
        {
            if (pos == 0)
            {
                return 0;
            }

            var item = basketItems[RealPosition(pos)];
            if (item is BasketHeaderitem)
            {
                return 1;
            }
            if (item is BasketMenuItem)
            {
                return 2;
            }
            return 4;
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var type = GetItemViewType(position);
            var item = basketItems[RealPosition(position)];

            if (type == 0)
            {
                var favoriteTransactionDetailHeaderViewHolder = viewHolder as ConfirmCheckoutHeaderViewHolder;

                favoriteTransactionDetailHeaderViewHolder.Price.Text = price;
            }
            else if (type == 1)
            {
                var storeViewHolder = viewHolder as ConfirmCheckoutItemHeaderViewHolder;
                var headerItem = item as BasketHeaderitem;
                
                storeViewHolder.Description.Text = headerItem.Description;
            }
            else if(type == 2)
            {
                var favoriteItemViewHolder = viewHolder as ConfirmCheckoutItemViewHolder;
                var headerItem = (item as BasketMenuItem).Item;

                favoriteItemViewHolder.Title.Text = headerItem.Item.Description;

                var extraInfo = Utils.Utils.GenerateItemExtraInfo(context, menuService, AppData.MobileMenu, headerItem.Item);

                if (string.IsNullOrEmpty(extraInfo))
                {
                    favoriteItemViewHolder.SubTitle.Visibility = ViewStates.Gone;
                }
                else
                {
                    favoriteItemViewHolder.SubTitle.Visibility = ViewStates.Visible;
                    favoriteItemViewHolder.SubTitle.Text = extraInfo;
                }

                favoriteItemViewHolder.Qty.Text = string.Format(context.Resources.GetString(Resource.String.QtyN), headerItem.Quantity);
                favoriteItemViewHolder.Price.Text = AppData.FormatCurrency(basketService.GetBasketItemFullPrice(AppData.MobileMenu, headerItem));

                if (menuService.HasAnyModifiers(AppData.MobileMenu, headerItem.Item))
                {
                    favoriteItemViewHolder.Edit.Visibility = ViewStates.Visible;

                    favoriteItemViewHolder.Increase.Visibility = ViewStates.Gone;
                    favoriteItemViewHolder.Decrease.Visibility = ViewStates.Gone;

                    (favoriteItemViewHolder.Favorite.LayoutParameters as RelativeLayout.LayoutParams).AddRule(LayoutRules.AlignParentRight);
                }
                else
                {
                    favoriteItemViewHolder.Edit.Visibility = ViewStates.Gone;

                    favoriteItemViewHolder.Increase.Visibility = ViewStates.Visible;
                    favoriteItemViewHolder.Decrease.Visibility = ViewStates.Visible;

                    (favoriteItemViewHolder.Favorite.LayoutParameters as RelativeLayout.LayoutParams).AddRule(LayoutRules.AlignParentRight, 0);
                    //(favoriteItemViewHolder.Favorite.LayoutParameters as RelativeLayout.LayoutParams).RemoveRule(LayoutRules.AlignParentRight);
                }

                if (favoriteModel.IsFavorite(headerItem.Item))
                {
                    favoriteItemViewHolder.Favorite.SetImageResource(Resource.Drawable.ic_favorite_black_24dp);
                }
                else
                {
                    favoriteItemViewHolder.Favorite.SetImageResource(Resource.Drawable.ic_favorite_border_black_24dp);
                }

                var favoriteDrawable = DrawableCompat.Wrap(favoriteItemViewHolder.Favorite.Drawable);
                DrawableCompat.SetTint(favoriteDrawable, new Color(ContextCompat.GetColor(favoriteItemViewHolder.Favorite.Context, Resource.Color.black87)));
                DrawableCompat.Unwrap(favoriteDrawable);

                if (favoriteItemViewHolder.Image != null && favoriteItemViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(favoriteItemViewHolder.Image);

                    favoriteItemViewHolder.Image.Tag = null;

                    if (headerItem.Item.Images.Count > 0 && headerItem.Item.Images[0] != null)
                    {
                        var tag = headerItem.Item.Images[0].Id;
                        favoriteItemViewHolder.Image.Tag = tag;

                        favoriteItemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(headerItem.Item.Images[0].GetAvgColor()));

                        var image = await imageModel.ImageGetById(headerItem.Item.Images[0].Id, imageSize);

                        if (image == null)
                        {
                            if (favoriteItemViewHolder?.Image != null && favoriteItemViewHolder.Image.Tag.ToString() == tag)
                            {
                                favoriteItemViewHolder.Image.Tag = null;

                                favoriteItemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                            }
                        }
                        else
                        {
                            if (favoriteItemViewHolder?.Image?.Tag != null && favoriteItemViewHolder.Image.Tag.ToString() == tag)
                            {
                                favoriteItemViewHolder.Image.Tag = null;

                                ImageUtils.CrossfadeImage(favoriteItemViewHolder.Image, ImageUtils.DecodeImage(image.Image), favoriteItemViewHolder.ImageContainer, image.Crossfade);
                            }
                        }
                    }
                }
            }
            else if (type == 4)
            {
                var favoriteItemViewHolder = viewHolder as ConfirmCheckoutCouponViewHolder;
                var offer = (item as BasketOfferItem).Offer;

                favoriteItemViewHolder.Title.Text = offer.Description;
                favoriteItemViewHolder.SubTitle.Text = offer.Details;

                if (offer.Type == OfferType.PointOffer)
                {
                    Drawable addRemoveDrawable;

                    if (basketModel.IsSelected(offer))
                    {
                        addRemoveDrawable = ContextCompat.GetDrawable(context, Resource.Drawable.ic_remove_black_24dp);
                    }
                    else
                    {
                        addRemoveDrawable = ContextCompat.GetDrawable(context, Resource.Drawable.ic_add_black_24dp);
                    }

                    favoriteItemViewHolder.AddRemove.SetImageDrawable(addRemoveDrawable);

                    favoriteItemViewHolder.AddRemove.Visibility = ViewStates.Visible;
                    favoriteItemViewHolder.Divider.Visibility = ViewStates.Visible;
                }
                else
                {
                    favoriteItemViewHolder.AddRemove.Visibility = ViewStates.Gone;
                    favoriteItemViewHolder.Divider.Visibility = ViewStates.Gone;
                }

                

                if (favoriteItemViewHolder.Image != null && favoriteItemViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(favoriteItemViewHolder.Image);

                    favoriteItemViewHolder.Image.Tag = null;

                    if (offer.Images.Count > 0 && offer.Images[0] != null)
                    {
                        var tag = offer.Images[0].Id;
                        favoriteItemViewHolder.Image.Tag = tag;

                        favoriteItemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(offer.Images[0].GetAvgColor()));

                        var image = await imageModel.ImageGetById(offer.Images[0].Id, imageSize);

                        if (image == null)
                        {
                            if (favoriteItemViewHolder?.Image != null && favoriteItemViewHolder.Image.Tag.ToString() == tag)
                            {
                                favoriteItemViewHolder.Image.Tag = null;

                                favoriteItemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                            }
                        }
                        else
                        {
                            if (favoriteItemViewHolder?.Image?.Tag != null && favoriteItemViewHolder.Image.Tag.ToString() == tag)
                            {
                                favoriteItemViewHolder.Image.Tag = null;

                                Utils.ImageUtils.CrossfadeImage(favoriteItemViewHolder.Image, ImageUtils.DecodeImage(image.Image), favoriteItemViewHolder.ImageContainer, image.Crossfade);
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
                View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ConfirmCheckoutTotalCard, parent);

                var newParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                newParams.FullSpan = true;
                view.LayoutParameters = newParams;

                vh = new ConfirmCheckoutHeaderViewHolder(view, (pos) =>
                {
                    listener.ItemClicked(ItemType.Checkout, "", "", 0, view, "");
                });
            }
            else if (viewType == 1)
            {
                var view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ListViewSectionHeader, parent);

                var newParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                newParams.FullSpan = true;
                view.LayoutParameters = newParams;

                vh = new ConfirmCheckoutItemHeaderViewHolder(view);
            }
            else if(viewType == 2)
            {
                View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ConfirmCheckoutItemCardItem, parent);

                vh = new ConfirmCheckoutItemViewHolder(view, async (itemClickedType, pos) =>
                {
                    var basketItem = (basketItems[RealPosition(pos)] as BasketMenuItem).Item;

                    if (itemClickedType == ItemClickedType.Delete)
                    {
                        basketModel.DeleteItem(basketItem.Id, false);
                        basketItems.RemoveAt(RealPosition(pos));

                        NotifyItemRemoved(pos);
                    }
                    else if (itemClickedType == ItemClickedType.Edit)
                    {
                        var intent = new Intent();
                        intent.SetClass(context, typeof(MenuItemModificationActivity));
                        intent.PutExtra(BundleUtils.BasketItemId, basketItem.Id);

                        ActivityUtils.StartActivityWithAnimation(context, intent, view);
                    }
                    else if (itemClickedType == ItemClickedType.Favorite)
                    {
                        await favoriteModel.ToggleFavorite(basketItem.Item);

                        NotifyDataSetChanged();
                    }
                    else if (itemClickedType == ItemClickedType.Increase)
                    {
                        basketModel.ChangeQty(basketItem, Math.Min(basketItem.Quantity + 1, AppData.MaxItems));
                    }
                    else if (itemClickedType == ItemClickedType.Decrease)
                    {
                        basketModel.ChangeQty(basketItem, Math.Max(basketItem.Quantity - 1, 1));
                    }
                    else if (itemClickedType == ItemClickedType.Item)
                    {
                        listener.ItemClicked(ItemType.Item, basketItem.Item.Id, basketItem.Item.MenuId, (int)menuService.GetItemType(basketItem.Item), view, string.Empty);
                    }
                });
            }
            else if (viewType == 4)
            {
                View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.ConfirmCheckoutCouponCardItem, parent);

                vh = new ConfirmCheckoutCouponViewHolder(view, (itemClickedType, pos) =>
                {
                    var offer = (basketItems[RealPosition(pos)] as BasketOfferItem).Offer;

                    if (itemClickedType == ItemClickedType.AddRemove)
                    {
                        basketModel.ToggleOffer(offer);
                    }
                    else if (itemClickedType == ItemClickedType.Item)
                    {
                        listener.ItemClicked(ItemType.Offer, offer.Id, "", 0, view, string.Empty);
                    }
                });
            }
            
            return vh;
        }

        private enum ItemClickedType
        {
            Checkout,
            Item,
            Delete,
            Edit,
            Favorite,
            Increase,
            Decrease,
            AddRemove
        }

        private class ConfirmCheckoutHeaderViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public TextView Price { get; set; }
            public ColoredButton PlaceOrder { get; set; }

            public ConfirmCheckoutHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public ConfirmCheckoutHeaderViewHolder(View view, Action<int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Price = view.FindViewById<TextView>(Resource.Id.ConfirmCheckoutTotal);
                PlaceOrder = view.FindViewById<ColoredButton>(Resource.Id.ConfirmCheckoutCheckout);

                PlaceOrder.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.ConfirmCheckoutCheckout:
                        itemClicked(AdapterPosition);
                        break;
                }
            }
        }

        private class ConfirmCheckoutItemHeaderViewHolder : RecyclerView.ViewHolder
        {
            public TextView Description { get; set; }

            public ConfirmCheckoutItemHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public ConfirmCheckoutItemHeaderViewHolder(View view)
                : base(view)
            {
                Description = view.FindViewById<TextView>(Resource.Id.ListViewSectionHeaderDescription);
            }
        }

        private class ConfirmCheckoutItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public FrameLayout ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }
            public TextView Qty { get; set; }
            public TextView Price { get; set; }
            public ImageButton Delete { get; set; }
            public ImageButton Edit { get; set; }
            public ImageButton Favorite { get; set; }
            public ImageButton Increase { get; set; }
            public ImageButton Decrease { get; set; }

            public ConfirmCheckoutItemViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public ConfirmCheckoutItemViewHolder(View view, Action<ItemClickedType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer =
                    view.FindViewById<FrameLayout>(Resource.Id.ConfirmCheckoutListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ConfirmCheckoutListItemViewItemImage);
                Title = view.FindViewById<TextView>(Resource.Id.ConfirmCheckoutListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.ConfirmCheckoutListItemViewSubtitle);
                Qty = view.FindViewById<TextView>(Resource.Id.ConfirmCheckoutListItemViewQty);
                Price = view.FindViewById<TextView>(Resource.Id.ConfirmCheckoutListItemViewPrice);
                Delete = view.FindViewById<ImageButton>(Resource.Id.ConfirmCheckoutListItemDelete);
                Edit = view.FindViewById<ImageButton>(Resource.Id.ConfirmCheckoutListItemEdit);
                Favorite = view.FindViewById<ImageButton>(Resource.Id.ConfirmCheckoutListItemFavorite);
                Increase = view.FindViewById<ImageButton>(Resource.Id.ConfirmCheckoutListItemDecrease);
                Decrease = view.FindViewById<ImageButton>(Resource.Id.ConfirmCheckoutListItemIncrease);

                Delete.SetOnClickListener(this);
                Edit.SetOnClickListener(this);
                Favorite.SetOnClickListener(this);
                Increase.SetOnClickListener(this);
                Decrease.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.ConfirmCheckoutListItemDelete:
                        itemClicked(ItemClickedType.Delete, AdapterPosition);
                        break;

                    case Resource.Id.ConfirmCheckoutListItemEdit:
                        itemClicked(ItemClickedType.Edit, AdapterPosition);
                        break;

                    case Resource.Id.ConfirmCheckoutListItemFavorite:
                        itemClicked(ItemClickedType.Favorite, AdapterPosition);
                        break;

                    case Resource.Id.ConfirmCheckoutListItemDecrease:
                        itemClicked(ItemClickedType.Decrease, AdapterPosition);
                        break;

                    case Resource.Id.ConfirmCheckoutListItemIncrease:
                        itemClicked(ItemClickedType.Increase, AdapterPosition);
                        break;

                    default:
                        itemClicked(ItemClickedType.Item, AdapterPosition);
                        break;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (Image != null)
                    ImageUtils.ClearImageView(Image);

                base.Dispose(disposing);
            }
        }

        private class ConfirmCheckoutCouponViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public FrameLayout ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }
            public ImageButton AddRemove { get; set; }
            public View Divider { get; set; }

            public ConfirmCheckoutCouponViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public ConfirmCheckoutCouponViewHolder(View view, Action<ItemClickedType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer = view.FindViewById<FrameLayout>(Resource.Id.ConfirmCheckoutListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.ConfirmCheckoutListItemViewItemImage);
                Title = view.FindViewById<TextView>(Resource.Id.ConfirmCheckoutListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.ConfirmCheckoutListItemViewSubtitle);
                AddRemove = view.FindViewById<ImageButton>(Resource.Id.ConfirmCheckoutListItemViewAddRemove);
                Divider = view.FindViewById(Resource.Id.ConfirmCheckoutListItemViewDivider);

                AddRemove.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.ConfirmCheckoutListItemViewAddRemove:
                        itemClicked(ItemClickedType.AddRemove, AdapterPosition);
                        break;

                    default:
                        itemClicked(ItemClickedType.Item, AdapterPosition);
                        break;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (Image != null)
                    ImageUtils.ClearImageView(Image);

                base.Dispose(disposing);
            }
        }

        private interface IBasketItem
        {
             
        }

        private class BasketHeaderitem : IBasketItem
        {
            public string Description { get; set; }
        }

        private class BasketMenuItem : IBasketItem
        {
            public BasketItem Item { get; set; }
        }

        private class BasketOfferItem : IBasketItem
        {
            public PublishedOffer Offer { get; set; }
        }
    }
}