using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Models;
using Presentation.Utils;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Adapters
{
    public class TransactionDetailAdapter : BaseRecyclerAdapter
    {
        private Context context;

        private MenuService menuService;

        private ImageModel imageModel;
        private BasketModel basketModel;
        private FavoriteModel favoriteModel;

        private readonly IItemClickListener listener;
        private Transaction transaction;
        private List<SaleLine> saleLines;
        private ImageSize imageSize;

        public TransactionDetailAdapter(Context context, IItemClickListener listener, int columns)
        {
            menuService = new MenuService();

            this.context = context;
            this.listener = listener;
            imageModel = new ImageModel(context);
            basketModel = new BasketModel(context);
            favoriteModel = new FavoriteModel(context);

            var dimension = context.Resources.GetDimensionPixelSize(Resource.Dimension.CardThumbnailImageWidth);
            imageSize = new ImageSize(dimension, dimension);
        }

        public void SetTransaction(Transaction transaction)
        {
            this.transaction = transaction;
            this.saleLines = transaction.SaleLines;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get { return saleLines.Count + 1; }
        }

        private int RealPosition(int position)
        {
            return position - 1;
        }

        public override int GetItemViewType(int pos)
        {
            if (pos == 0)
            {
                return 0;
            }

            return 1;
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var type = GetItemViewType(position);

            if (type == 0)
            {
                var favoriteTransactionDetailHeaderViewHolder = viewHolder as TransactionDetailHeaderViewHolder;

                favoriteTransactionDetailHeaderViewHolder.Title.Text = transaction.DateToShortFormat;
                favoriteTransactionDetailHeaderViewHolder.SubTitle.Text = string.Format(context.Resources.GetString(Resource.String.FavoriteTransactionItems), transaction.SaleLines.Count);
                favoriteTransactionDetailHeaderViewHolder.Price.Text = transaction.Amount;

                if (favoriteModel.IsFavorite(transaction))
                {
                    favoriteTransactionDetailHeaderViewHolder.Favorite.Text = context.Resources.GetString(Resource.String.RemoveFavorite);
                }
                else
                {
                    favoriteTransactionDetailHeaderViewHolder.Favorite.Text = context.Resources.GetString(Resource.String.Favorite);
                }
            }
            else 
            { 
                var favoriteItemViewHolder = viewHolder as TransactionDetailItemViewHolder;
                var saleLine = saleLines[RealPosition(position)];

                favoriteItemViewHolder.Title.Text = saleLine.Item.Description;
                favoriteItemViewHolder.Qty.Text = string.Format(favoriteItemViewHolder.Qty.Context.GetString(Resource.String.QtyN), saleLine.Quantity);

                var extraInfo = Utils.Utils.GenerateItemExtraInfo(context, menuService, AppData.MobileMenu, saleLine.Item);

                if (string.IsNullOrEmpty(extraInfo))
                {
                    favoriteItemViewHolder.SubTitle.Visibility = ViewStates.Gone;
                }
                else
                {
                    favoriteItemViewHolder.SubTitle.Visibility = ViewStates.Visible;
                    favoriteItemViewHolder.SubTitle.Text = extraInfo;
                }

                favoriteItemViewHolder.Price.Text = saleLine.Amount;

                Drawable favoriteDrawable;

                if (favoriteModel.IsFavorite(saleLine.Item))
                {
                    favoriteDrawable = ContextCompat.GetDrawable(context, Resource.Drawable.ic_favorite_black_24dp);
                }
                else
                {
                    favoriteDrawable = ContextCompat.GetDrawable(context, Resource.Drawable.ic_favorite_border_black_24dp);
                }

                var favoriteWrappedDrawable = DrawableCompat.Wrap(favoriteDrawable);
                DrawableCompat.SetTint(favoriteWrappedDrawable, new Color(ContextCompat.GetColor(favoriteItemViewHolder.Favorite.Context, Resource.Color.black87)));
                DrawableCompat.Unwrap(favoriteWrappedDrawable);

                //favoriteDrawable.SetColorFilter(Utils.Utils.GetColorFilter(context.Resources.GetColor(Resource.Color.black54)));
                favoriteItemViewHolder.Favorite.SetImageDrawable(favoriteDrawable);

                if (favoriteItemViewHolder.Image != null && favoriteItemViewHolder.ImageContainer != null)
                {
                    Utils.ImageUtils.ClearImageView(favoriteItemViewHolder.Image);

                    favoriteItemViewHolder.Image.Tag = null;

                    if (saleLine.Item.Images.Count > 0 && saleLine.Item.Images[0] != null)
                    {
                        var tag = saleLine.Item.Images[0].Id;
                        favoriteItemViewHolder.Image.Tag = tag;

                        favoriteItemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(saleLine.Item.Images[0].GetAvgColor()));

                        var image = await imageModel.ImageGetById(saleLine.Item.Images[0].Id, imageSize);

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
                View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionDetailHeader, parent);

                var newParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                newParams.FullSpan = true;
                view.LayoutParameters = newParams;

                vh = new TransactionDetailHeaderViewHolder(view, async (itemClickedType, pos) =>
                {
                    if (itemClickedType == ItemClickedType.AddToBasket)
                    {
                        basketModel.AddSaleLinesToBasket(transaction.SaleLines);
                    }
                    else if (itemClickedType == ItemClickedType.Favorite)
                    {
                        await favoriteModel.ToggleFavorite(transaction);
                        NotifyDataSetChanged();
                    }
                });
            }
            else
            {
                View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionDetailCardItem, parent);

                vh = new TransactionDetailItemViewHolder(view, async (itemClickedType, pos) =>
                {
                    var saleLine = saleLines[RealPosition(pos)];

                    if (itemClickedType == ItemClickedType.AddToBasket)
                    {
                        basketModel.AddItemToBasket(saleLine.Item, saleLine.Quantity);
                    }
                    else if (itemClickedType == ItemClickedType.Favorite)
                    {
                        await favoriteModel.ToggleFavorite(saleLine.Item, true);
                        NotifyItemChanged(pos);
                    }
                    else if (itemClickedType == ItemClickedType.Item)
                    {
                        listener.ItemClicked(ItemType.TransactionDetail, saleLine.Id, saleLine.Item.MenuId, 0, view, string.Empty);
                    }

                });
            }
            
            return vh;
        }

        private enum ItemClickedType
        {
            Item,
            AddToBasket,
            Favorite,
        }

        private class TransactionDetailHeaderViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }
            public TextView Price { get; set; }
            public Button Favorite { get; set; }
            public Button AddToBasket { get; set; }

            public TransactionDetailHeaderViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public TransactionDetailHeaderViewHolder(View view, Action<ItemClickedType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailHeaderTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.TransactionDetailHeaderSubTitle);
                Price = view.FindViewById<TextView>(Resource.Id.TransactionDetailHeaderPrice);
                Favorite = view.FindViewById<Button>(Resource.Id.TransactionDetailHeaderFavorite);
                AddToBasket = view.FindViewById<Button>(Resource.Id.TransactionDetailHeaderAddToBasket);

                Favorite.SetOnClickListener(this);
                AddToBasket.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.TransactionDetailHeaderFavorite:
                        itemClicked(ItemClickedType.Favorite, AdapterPosition);
                        break;

                    case Resource.Id.TransactionDetailHeaderAddToBasket:
                        itemClicked(ItemClickedType.AddToBasket, AdapterPosition);
                        break;
                }
            }
        }

        private class TransactionDetailItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public FrameLayout ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView Qty { get; set; }
            public TextView SubTitle { get; set; }
            public TextView Price { get; set; }
            public ImageButton Favorite { get; set; }
            public ImageButton AddToBasket { get; set; }

            public TransactionDetailItemViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public TransactionDetailItemViewHolder(View view, Action<ItemClickedType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer = view.FindViewById<FrameLayout>(Resource.Id.TransactionDetailListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.TransactionDetailListItemViewItemImage);
                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewTitle);
                Qty = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewQty);
                SubTitle = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewSubtitle);
                Price = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewPrice);
                Favorite = view.FindViewById<ImageButton>(Resource.Id.TransactionDetailListItemViewFavorite);
                AddToBasket = view.FindViewById<ImageButton>(Resource.Id.TransactionDetailListItemViewAddToBasket);

                Favorite.SetOnClickListener(this);
                AddToBasket.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.TransactionDetailListItemViewFavorite:
                        itemClicked(ItemClickedType.Favorite, AdapterPosition);
                        break;

                    case Resource.Id.TransactionDetailListItemViewAddToBasket:
                        itemClicked(ItemClickedType.AddToBasket, AdapterPosition);
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
    }
}