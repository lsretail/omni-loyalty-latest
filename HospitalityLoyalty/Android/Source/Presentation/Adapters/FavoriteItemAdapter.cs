using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Dialog;
using Presentation.Models;
using Presentation.Utils;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Adapters
{
    class FavoriteItemAdapter : BaseRecyclerAdapter
    {
        private Context context;

        private MenuService menuService;

        private ImageModel imageModel;
        private BasketModel basketModel;
        private FavoriteModel favoriteModel;

        private readonly IItemClickListener listener;
        private List<MenuItem> menuItems;
        private ImageSize imageSize;

        public FavoriteItemAdapter(Context context, IItemClickListener listener, int columns)
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

        public void SetMenuItems(List<MenuItem> menuItems)
        {
            this.menuItems = menuItems;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get { return menuItems.Count; }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var favoriteItemViewHolder = viewHolder as FavoriteItemViewHolder;
            var menuItem = menuItems[position];

            favoriteItemViewHolder.Title.Text = menuItem.Name;

            var extraInfo = Utils.Utils.GenerateItemExtraInfo(context, menuService, AppData.MobileMenu, menuItem);

            if (string.IsNullOrEmpty(extraInfo))
            {
                favoriteItemViewHolder.SubTitle.Visibility = ViewStates.Gone;
            }
            else
            {
                favoriteItemViewHolder.SubTitle.Visibility = ViewStates.Visible;
                favoriteItemViewHolder.SubTitle.Text = extraInfo;
            }

            favoriteItemViewHolder.Price.Text = AppData.FormatCurrency(menuService.GetItemFullPrice(AppData.MobileMenu, menuItem));

            if (favoriteItemViewHolder.Image != null && favoriteItemViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(favoriteItemViewHolder.Image);

                favoriteItemViewHolder.Image.Tag = null;

                if (menuItem.Images.Count > 0 && menuItem.Images[0] != null)
                {
                    var tag = menuItem.Images[0].Id;
                    favoriteItemViewHolder.Image.Tag = tag;

                    favoriteItemViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(menuItem.Images[0].GetAvgColor()));

                    var image = await imageModel.ImageGetById(menuItem.Images[0].Id, imageSize);

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

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.FavoriteItemCardItem, parent);

            var vh = new FavoriteItemViewHolder(view, async (itemClickedType, pos) =>
            {
                var menuItem = menuItems[pos];

                if (itemClickedType == ItemClickedType.AddToBasket)
                {
                    basketModel.AddItemToBasket(menuItem, 1);
                }
                else if (itemClickedType == ItemClickedType.Delete)
                {
                    await favoriteModel.ToggleFavorite(menuItem, true);
                    menuItems.RemoveAt(pos);
                    NotifyItemRemoved(pos);
                }
                else if (itemClickedType == ItemClickedType.Edit)
                {
                    var dialog = new EditTextDialog(context, context.GetString(Resource.String.FavoriteChangeName));
                    dialog.EditText.Hint = context.GetString(Resource.String.FavoriteNewName);
                    dialog.SetPositiveButton(context.GetString(Resource.String.Ok), async () =>
                    {
                        await favoriteModel.RenameFavorites(menuItem, dialog.EditText.Text);
                        NotifyItemChanged(pos);
                    });
                    dialog.SetNegativeButton(context.GetString(Resource.String.Cancel), () => { });
                    dialog.Show();
                }
                else if (itemClickedType == ItemClickedType.Item)
                {
                    listener.ItemClicked(ItemType.Item, menuItem.Id, menuItem.MenuId, (int)menuService.GetItemType(menuItem), view, string.Empty);
                }
                
            });

            return vh;
        }

        private enum ItemClickedType
        {
            Item,
            AddToBasket,
            Delete,
            Edit
        }

        private class FavoriteItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public FrameLayout ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }
            public TextView Price { get; set; }
            public ImageButton Edit { get; set; }
            public ImageButton Delete { get; set; }
            public ImageButton AddToBasket { get; set; }

            public FavoriteItemViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public FavoriteItemViewHolder(View view, Action<ItemClickedType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer = view.FindViewById<FrameLayout>(Resource.Id.FavoriteItemListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.FavoriteItemListItemViewItemImage);
                Title = view.FindViewById<TextView>(Resource.Id.FavoriteItemListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.FavoriteItemListItemViewSubtitle);
                Price = view.FindViewById<TextView>(Resource.Id.FavoriteItemListItemViewPrice);
                Edit = view.FindViewById<ImageButton>(Resource.Id.FavoriteItemListItemViewEdit);
                Delete = view.FindViewById<ImageButton>(Resource.Id.FavoriteItemListItemViewDelete);
                AddToBasket = view.FindViewById<ImageButton>(Resource.Id.FavoriteItemListItemViewAddToBasket);

                Edit.SetOnClickListener(this);
                Delete.SetOnClickListener(this);
                AddToBasket.SetOnClickListener(this);
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.FavoriteItemListItemViewEdit:
                        itemClicked(ItemClickedType.Edit, AdapterPosition);
                        break;
                    case Resource.Id.FavoriteItemListItemViewDelete:
                        itemClicked(ItemClickedType.Delete, AdapterPosition);
                        break;

                    case Resource.Id.FavoriteItemListItemViewAddToBasket:
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