using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Models;
using Presentation.Utils;
using ImageView = Android.Widget.ImageView;

namespace Presentation.Adapters
{
    public class MenuNodeAdapter : BaseRecyclerAdapter
    {
        private MenuService menuService;

        private IItemClickListener itemClickListener;
        private bool showAsList;

        private string menuId;
        private List<MobileMenuNode> menuNodes;
        private ImageModel imageModel;
        private Context context;
        private ImageSize imageSize;

        public MenuNodeAdapter(Context context, IItemClickListener itemClickListener)
        {
            menuService = new MenuService();

            this.itemClickListener = itemClickListener;

            this.context = context;
            this.imageModel = new ImageModel(context, null);
        }

        public void SetMode(bool showAsList, int columns)
        {
            this.showAsList = showAsList;

            if (showAsList)
            {
                var dimension = context.Resources.GetDimensionPixelSize(Resource.Dimension.ListImageHeight);
                imageSize = new ImageSize(dimension, dimension);
            }
            else
            {
                var dimension = context.Resources.DisplayMetrics.WidthPixels / columns;
                imageSize = new ImageSize(dimension, dimension);
            }
        }

        public void SetItems(string menuId, List<MobileMenuNode> menuNodes)
        {
            this.menuId = menuId;
            this.menuNodes = menuNodes;

            NotifyDataSetChanged();
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var viewType = GetItemViewType(position);

            var menuNodeViewHolder = viewHolder as MenuNodeViewHolder;
            var menuNode = menuNodes[position];

            if (menuNodeViewHolder == null || menuNode == null)
            {
                return;
            }

            menuNodeViewHolder.Title.Text = menuNode.Description;

            if (viewType == 1)
            {
                var item = menuService.GetMenuItem(AppData.MobileMenu, menuNode.Id, menuNode.NodeLineType);

                menuNodeViewHolder.SubTitle.Text = AppData.FormatCurrency(menuService.GetItemFullPrice(AppData.MobileMenu, item));
            }

            if (menuNodeViewHolder.Image != null && menuNodeViewHolder.ImageContainer != null)
            {
                Utils.ImageUtils.ClearImageView(menuNodeViewHolder.Image);

                menuNodeViewHolder.Image.Tag = null;

                if (menuNode.Image != null)
                {
                    var tag = showAsList + "-" + menuNode.Image.Id;
                    menuNodeViewHolder.Image.Tag = tag;

                    if (showAsList)
                    {
                        var backgroundCircle = new ShapeDrawable(new OvalShape());
                        backgroundCircle.Paint.Color = Android.Graphics.Color.ParseColor(menuNode.Image.GetAvgColor());

                        menuNodeViewHolder.ImageContainer.Background = backgroundCircle;
                    }
                    else
                    {
                        menuNodeViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.ParseColor(menuNode.Image.GetAvgColor()));
                    }

                    var image = await  imageModel.ImageGetById(menuNode.Image.Id, imageSize);

                    if (image == null)
                    {
                        if (menuNodeViewHolder?.Image != null && menuNodeViewHolder.Image.Tag.ToString() == tag)
                        {
                            menuNodeViewHolder.Image.Tag = null;

                            menuNodeViewHolder.ImageContainer.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }
                    else
                    {
                        if (menuNodeViewHolder?.Image?.Tag != null && menuNodeViewHolder.Image.Tag.ToString() == tag)
                        {
                            menuNodeViewHolder.Image.Tag = null;

                            if (showAsList)
                            {
                                Utils.ImageUtils.CrossfadeImage(menuNodeViewHolder.Image, new ImageUtils.CircleDrawable(ImageUtils.DecodeImage(image.Image)), menuNodeViewHolder.ImageContainer, image.Crossfade);
                            }
                            else
                            {
                                Utils.ImageUtils.CrossfadeImage(menuNodeViewHolder.Image, ImageUtils.DecodeImage(image.Image), menuNodeViewHolder.ImageContainer, image.Crossfade);
                            }
                        }
                    }
                }
            }
        }

        public override int GetItemViewType(int pos)
        {
            if (menuNodes[pos].NodeType == MobileMenuNodeType.Item)
            {
                return 1;
            }
            return 0;
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder vh = null;

            if (viewType == 0)
            {
                View view;
                if (showAsList)
                {
                    view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.MenuNodeGroupListItem, parent);
                }
                else
                {
                    view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.MenuNodeGroupCardItem, parent);
                }

                vh = new MenuNodeViewHolder(view, (type, pos) =>
                {
                    var itemType = ItemType.MenuGroup;
                    var menuNode = menuNodes[pos];

                    itemClickListener.ItemClicked(itemType, menuNode.Id, "", 0, view, string.Empty);
                    //itemClickListener.ItemClicked(itemType, menuNode.Id, string.Empty, (vh as MenuNodeViewHolder).Image);
                });
            }
            else if (viewType == 1)
            {
                View view;
                if (showAsList)
                {
                    view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.MenuNodeListItem, parent);
                }
                else
                {
                    view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.MenuNodeCardItem, parent);
                }

                vh = new MenuNodeViewHolder(view, (type, pos) =>
                {
                    var itemType = ItemType.AddToBasket;
                    var menuNode = menuNodes[pos];

                    string key = string.Empty;

                    if (type == ItemClickedType.Item)
                    {
                        itemType = ItemType.Item;
                    }

                    itemClickListener.ItemClicked(itemType, menuNode.Id, key, (int)menuNode.NodeLineType, view);
                    //itemClickListener.ItemClicked(itemType, menuNode.Id, string.Empty, view);
                });
            }

            return vh;
        }

        public override int ItemCount
        {
            get
            {
                if (menuNodes == null)
                {
                    return 0;
                }

                return menuNodes.Count;
            }
        }

        private enum ItemClickedType
        {
            Item,
            AddToBasket
        }

        private class MenuNodeViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public View ImageContainer { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }
            public ImageButton AddToBasket { get; set; }

            public MenuNodeViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public MenuNodeViewHolder(View view, Action<ItemClickedType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                ImageContainer = view.FindViewById<View>(Resource.Id.MenuNodeListItemViewItemImageContainer);
                Image = view.FindViewById<ImageView>(Resource.Id.MenuNodeListItemViewItemImage);
                Title = view.FindViewById<TextView>(Resource.Id.MenuNodeListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.MenuNodeListItemViewSubtitle);
                AddToBasket = view.FindViewById<ImageButton>(Resource.Id.MenuNodeListItemViewAddToBasket);

                if (AddToBasket != null)
                {
                    AddToBasket.SetOnClickListener(this);
                }

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                if (v.Id == Resource.Id.MenuNodeListItemViewAddToBasket)
                {
                    itemClicked(ItemClickedType.AddToBasket, AdapterPosition);
                }
                else
                {
                    itemClicked(ItemClickedType.Item, AdapterPosition);
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