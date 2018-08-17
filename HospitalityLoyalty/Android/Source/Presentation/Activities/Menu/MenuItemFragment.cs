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
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Activities.Image;
using Presentation.Activities.SlideControl;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using Xamarin.ViewPagerIndicator;
using Math = System.Math;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Menu
{
    public class MenuItemFragment : BaseFragment, IRefreshableActivity, View.IOnClickListener, IBroadcastObserver, Toolbar.IOnMenuItemClickListener
    {
        private const int AddToBasketItemRequestCode = 101;

        private MenuService menuService;
        private MenuModel menuModel;
        private BasketModel basketModel;
        private FavoriteModel favoriteModel;

        private MenuItem item;

        private ViewPager imagePager;
        private Spinner addToBasketQty;
        private ViewSwitcher viewSwitcher;
        private View progressView;
        private View contentView;
        private FloatingActionButton favorite;
        private Toolbar addToBasketToolbar;
        private View scrollContentView;
        
        private string menuId;
        private string nodeId;
        private string itemId;
        private NodeLineType itemType;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            menuService = new MenuService();
            menuModel = new MenuModel(Activity, this);
            basketModel = new BasketModel(Activity);
            favoriteModel = new FavoriteModel(Activity);
            
            //HasOptionsMenu = true;

            var view = Inflate(inflater, Resource.Layout.MenuItemScreen, null);
            
            var toolbar = view.FindViewById<Toolbar>(Resource.Id.MenuItemScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            addToBasketToolbar = view.FindViewById<Toolbar>(Resource.Id.MenuItemScreenBasketToolbar);
            addToBasketToolbar.InflateMenu(Resource.Menu.AddToBasketMenu);
            addToBasketToolbar.SetOnMenuItemClickListener(this);

            scrollContentView = view.FindViewById(Resource.Id.MenuItemScreenScrollContent);
            scrollContentView.SetMinimumHeight(Utils.Utils.ViewUtils.GetContentViewHeight(Activity) - Resources.GetDimensionPixelSize(Resource.Dimension.ActionBarHeight));

            imagePager = view.FindViewById<ViewPager>(Resource.Id.HeaderImageViewPager);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                imagePager.SetPageTransformer(true, new ZoomOutPageTransformer());
            }

            //(view.FindViewById<NotifyingScrollView>(Resource.Id.MenuItemScreenScrollView)).OnScrollChangedListener = this;

            addToBasketQty = view.FindViewById<Spinner>(Resource.Id.MenuItemScreenAddToBasketQty);
            viewSwitcher = view.FindViewById<ViewSwitcher>(Resource.Id.MenuItemScreenViewSwitcher);
            progressView = view.FindViewById<View>(Resource.Id.MenuItemScreenLoadingSpinner);
            contentView = view.FindViewById<View>(Resource.Id.MenuItemScreenContent);

            favorite = view.FindViewById<FloatingActionButton>(Resource.Id.MenuItemDetailScreenFavorite);
            favorite.SetOnClickListener(this);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume(); 

            LoadItem();

            (Activity as HospActivity).AddObserver(this);
        }

        public override void OnPause()
        {
            (Activity as HospActivity).RemoveObserver(this);

            base.OnPause();
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.MenuUpdated)
            {
                LoadItem();
            }
            else if(action == BroadcastUtils.FavoritesUpdated || action == BroadcastUtils.FavoritesUpdatedInList)
            {
                SetFavorite();
            }
        }

        private void LoadItem()
        {
            if (AppData.MobileMenu == null || AppData.MobileMenu.MenuNodes.Count == 0)
            {
                menuModel.GetMenus();
                return;
            }
            else if(AppData.MenuNeedsUpdate)
            {
                menuModel.GetMenus();
            }

            if (Arguments.ContainsKey(BundleUtils.NodeId))
            {
                menuId = Arguments.GetString(BundleUtils.MenuId);
                nodeId = Arguments.GetString(BundleUtils.NodeId);
                itemType = (NodeLineType)Arguments.GetInt(BundleUtils.Type);

                item = menuService.GetMenuItem(AppData.MobileMenu, nodeId, itemType);
            }
            else if (Arguments.ContainsKey(BundleUtils.ItemId))
            {
                menuId = Arguments.GetString(BundleUtils.MenuId);
                itemId = Arguments.GetString(BundleUtils.ItemId);
                itemType = (NodeLineType)Arguments.GetInt(BundleUtils.Type);

                if (string.IsNullOrEmpty(menuId))
                {
                    foreach (var menu in AppData.MobileMenu.MenuNodes)
                    {
                        if (menuService.ContainsItem(menu, itemId, itemType))
                        {
                            menuId = menu.Id;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(menuId))
                {
                    item = menuService.GetMenuItem(AppData.MobileMenu, itemId, itemType);
                }
            }
            else if (Arguments.ContainsKey(BundleUtils.TransactionId))
            {
                var transactionId = Arguments.GetString(BundleUtils.TransactionId);
                var lineId = Arguments.GetString(BundleUtils.SaleLineId);
                var saleLine = AppData.Transactions.FirstOrDefault(x => x.Id == transactionId).SaleLines.FirstOrDefault(x => x.Id == lineId);

                menuId = saleLine.Item.MenuId;
                item = saleLine.Item;

            }

            if (item == null)
            {
                Activity.Finish();
                return;
                //todo add error message
            }

            LoadItem(item);
        }

        private void LoadItem(MenuItem item)
        {
            var animationImageId = string.Empty;
            if (Arguments.ContainsKey(BundleUtils.AnimationImageId))
            {
                animationImageId = Arguments.GetString(BundleUtils.AnimationImageId);
            }

            imagePager.Adapter = new ImagePagerAdapter(ChildFragmentManager, item.Images, Resources.DisplayMetrics.WidthPixels, Resources.GetDimensionPixelSize(Resource.Dimension.HeaderImageHeight), animationImageId);

            var indicator = View.FindViewById<LinePageIndicator>(Resource.Id.HeaderImageIndicator);
            if (item.Images != null && item.Images.Count > 1)
            {
                indicator.SetViewPager(imagePager);
            }
            else
            {
                indicator.Visibility = ViewStates.Gone;
            }

            View.FindViewById<TextView>(Resource.Id.MenuItemScreenItemName).Text = item.Description;
            View.FindViewById<TextView>(Resource.Id.MenuItemScreenItemPrice).Text = AppData.FormatCurrency(item.Price.Value);
            View.FindViewById<TextView>(Resource.Id.MenuItemScreenItemDescription).Text = item.Detail;

            var orderDetails = View.FindViewById<TextView>(Resource.Id.MenuItemScreenOrderDetails);

            if (item is MenuDeal)
            {
                var orderDetailsText = string.Empty;
                var dealItem = item as MenuDeal;

                foreach (var dealLine in dealItem.DealLines)
                {
                    var selectedItem = dealLine.DealLineItems.FirstOrDefault(x => x.ItemId == dealLine.SelectedId);

                    if (selectedItem != null)
                    {
                        if (selectedItem.Quantity > 1)
                        {
                            orderDetailsText += selectedItem.Quantity.ToString("0.##") + " ";
                        }

                        var menuItem = menuService.GetMenuItem(AppData.MobileMenu, selectedItem.ItemId, false);

                        orderDetailsText += menuItem.Description + System.Environment.NewLine;
                    }
                }

                orderDetails.Text = orderDetailsText.TrimEnd(System.Environment.NewLine.ToCharArray());
            }
            
            if(string.IsNullOrEmpty(orderDetails.Text))
            {
                orderDetails.Visibility = ViewStates.Gone;
            }

            // Create an ArrayAdapter using the string array and a default spinner layout
            var adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, Utils.Utils.GenerateQtyList(Activity));
            //var adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.QtyOptions, Android.Resource.Layout.SimpleSpinnerItem);
            // Specify the layout to use when the list of choices appears
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // Apply the adapter to the spinner
            addToBasketQty.Adapter = adapter;

            if (AppData.HasBasket)
            {
                if (menuService.HasAnyModifiers(AppData.MobileMenu, item) && menuService.HasAllRequiredModifiers(AppData.MobileMenu, item))
                {
                    //addToBasketQty.Visibility = ViewStates.Gone;
                    addToBasketToolbar.Menu.FindItem(Resource.Id.MenuViewEdit).SetVisible(false);
                }
                else if (menuService.HasAnyRequiredModifers(AppData.MobileMenu, item))
                {
                    //addToBasketQty.Visibility = ViewStates.Gone;
                }
                else if (menuService.HasAnyModifiers(AppData.MobileMenu, item))
                {
                    
                }
                else
                {
                    addToBasketToolbar.Menu.FindItem(Resource.Id.MenuViewEdit).SetVisible(false);
                }
            }
            else
            {
                addToBasketToolbar.Visibility = ViewStates.Gone;

                //fix scroll height
                scrollContentView.SetMinimumHeight(Utils.Utils.ViewUtils.GetContentViewHeight(Activity));
            }

            SetFavorite();

            //HasOptionsMenu = true;
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if(viewSwitcher.CurrentView != progressView)
                    viewSwitcher.ShowNext();
            }
            else
            {
                if (viewSwitcher.CurrentView != contentView)
                    viewSwitcher.ShowPrevious();
            }
        }

        public async void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.MenuItemDetailScreenFavorite:
                    await favoriteModel.ToggleFavorite(item);
                    break;
            }
        }

        public bool OnMenuItemClick(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.MenuViewAdd:
                    if (item != null)
                    {
                        if (menuService.HasAnyModifiers(AppData.MobileMenu, item) && menuService.HasAllRequiredModifiers(AppData.MobileMenu, item))
                        {
                            var intent = new Intent();
                            intent.SetClass(Activity, typeof(MenuItemModificationActivity));
                            intent.PutExtra(BundleUtils.ItemId, item.Id);
                            intent.PutExtra(BundleUtils.MenuId, menuId);
                            intent.PutExtra(BundleUtils.Qty, addToBasketQty.SelectedItemPosition + 1);
                            intent.PutExtra(BundleUtils.Type, (int)menuService.GetItemType(item));

                            StartActivityForResult(intent, AddToBasketItemRequestCode);
                            //ActivityUtils.StartActivityForResultWithAnimation(Activity, AddToBasketItemRequestCode, intent, addToBasketToolbar);
                        }
                        else if (menuService.HasAnyRequiredModifers(AppData.MobileMenu, item))
                        {
                            var intent = new Intent();
                            intent.SetClass(Activity, typeof(MenuItemModificationActivity));
                            intent.PutExtra(BundleUtils.ItemId, item.Id);
                            intent.PutExtra(BundleUtils.MenuId, menuId);
                            intent.PutExtra(BundleUtils.RequiredOnly, true);
                            intent.PutExtra(BundleUtils.Qty, addToBasketQty.SelectedItemPosition + 1);
                            intent.PutExtra(BundleUtils.Type, (int)menuService.GetItemType(item));

                            StartActivityForResult(intent, AddToBasketItemRequestCode);
                            //ActivityUtils.StartActivityForResultWithAnimation(Activity, AddToBasketItemRequestCode, intent, addToBasketToolbar);
                        }
                        else if (menuService.HasAnyModifiers(AppData.MobileMenu, item))
                        {
                            //basketModel.AddItemToBasket(item, addToBasketQty.SelectedItemPosition + 1);
                            basketModel.AddItemToBasket(item, addToBasketQty.SelectedItemPosition + 1, !EnabledItems.GoBackOnAddToBasket);

                            if (EnabledItems.GoBackOnAddToBasket)
                            {
                                Activity.SetResult(Result.Ok);
                                Activity.Finish();
                            }
                        }
                        else
                        {
                            //basketModel.AddItemToBasket(item, addToBasketQty.SelectedItemPosition + 1);
                            basketModel.AddItemToBasket(item, addToBasketQty.SelectedItemPosition + 1, !EnabledItems.GoBackOnAddToBasket);

                            if (EnabledItems.GoBackOnAddToBasket)
                            {
                                Activity.SetResult(Result.Ok);
                                Activity.Finish();
                            }
                        }
                    }
                    return true;

                case Resource.Id.MenuViewEdit:
                    if (item != null)
                    {
                        var intent = new Intent();
                        intent.SetClass(Activity, typeof(MenuItemModificationActivity));
                        intent.PutExtra(BundleUtils.ItemId, item.Id);
                        intent.PutExtra(BundleUtils.MenuId, menuId);
                        intent.PutExtra(BundleUtils.Qty, addToBasketQty.SelectedItemPosition + 1);
                        intent.PutExtra(BundleUtils.Type, (int)menuService.GetItemType(item));

                        StartActivityForResult(intent, AddToBasketItemRequestCode);
                        //ActivityUtils.StartActivityForResultWithAnimation(Activity, AddToBasketItemRequestCode, intent, addToBasketToolbar);
                    }
                    return true;
            }

            return false;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == AddToBasketItemRequestCode)
            {
                if (resultCode == (int) Result.Ok)
                {
                    //BaseModel.ShowToast(Activity.FindViewById(Resource.Id.BaseActivityScreenDrawerLayout), Resource.String.BasketItemAdded);
                    if (EnabledItems.GoBackOnAddToBasket)
                    {
                        Activity.SetResult(Result.Ok);
                        Activity.Finish();
                    }
                    else
                    {
                        BaseModel.ShowToast(View, Resource.String.BasketItemAdded);
                    }
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void SetFavorite()
        {

            if (favoriteModel.IsFavorite(item))
            {
                favorite.SetImageResource(Resource.Drawable.ic_favorite_black_24dp);
            }
            else
            {
                favorite.SetImageResource(Resource.Drawable.ic_favorite_border_black_24dp);
            }

            var favoriteDrawable = DrawableCompat.Wrap(favorite.Drawable);
            DrawableCompat.SetTint(favoriteDrawable, new Color(ContextCompat.GetColor(Activity, Resource.Color.white87)));
            DrawableCompat.Unwrap(favoriteDrawable);
        }
    }
}