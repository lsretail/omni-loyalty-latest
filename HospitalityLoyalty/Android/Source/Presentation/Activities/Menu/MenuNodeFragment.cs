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
using Android.Support.V4.Util;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Activities.Image;
using Presentation.Activities.SlideControl;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using ActionBar = Android.Support.V7.App.ActionBar;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using ImageView = Android.Widget.ImageView;
using Math = System.Math;

namespace Presentation.Activities.Menu
{
    public class MenuNodeFragment : BaseFragment, IItemClickListener, IRefreshableActivity, View.IOnClickListener, SwipeRefreshLayout.IOnRefreshListener, IBroadcastObserver, Android.Support.V7.App.ActionBar.IOnNavigationListener
    {
        private const int AddToBasketItemRequestCode = 101;

        private MenuService menuService;
        private MenuModel menuModel;
        private BasketModel basketModel;
        private ImageModel imageModel;
        private MenuNode node;
        private LSRetail.Omni.Domain.DataModel.Base.Menu.Menu selectedMenu;

        private RecyclerView.LayoutManager layoutManager;
        private MenuNodeAdapter adapter;
        private RecyclerView.ItemDecoration itemDecoration;

        private CollapsingToolbarLayout collapsingToolbar;
        private SwipeRefreshLayout headersRefreshLayout;
        private ImageView imageHeader;
        private View imageHeaderContainer;
        private RecyclerView menuNodeRecyclerView;
        private View progressView;

        private string nodeId = string.Empty;
        private string menuId = string.Empty;
        private string animationImageId = string.Empty;
        private bool showAsList;

        private int columns;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            menuService = new MenuService();

            var view = Inflate(inflater, Resource.Layout.MenuNodeScreen, null);

            if (Arguments != null)
            {
                menuId = Arguments.GetString(BundleUtils.MenuId);
                nodeId = Arguments.GetString(BundleUtils.NodeId);
                animationImageId = Arguments.GetString(BundleUtils.AnimationImageId);
            }

            columns = Resources.GetInteger(Resource.Integer.GridColumnCount);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.MenuNodeScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            collapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.MenuNodeScreenCollapsingToolbar);

            itemDecoration = new Utils.DividerItemDecoration(Activity);

            progressView = view.FindViewById(Resource.Id.MenuNodeProgress);
            menuNodeRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.MenuNodeList);
            menuNodeRecyclerView.AddItemDecoration(itemDecoration);
            menuNodeRecyclerView.HasFixedSize = true;
            
            menuNodeRecyclerView.SetItemAnimator(new DefaultItemAnimator());

            headersRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.MenuNodeListRefreshContainer);
            headersRefreshLayout.SetColorSchemeResources(Resource.Color.accent);
            headersRefreshLayout.SetOnRefreshListener(this);

            imageHeader = view.FindViewById<ImageView>(Resource.Id.MenuNodeScreenHeader);
            imageHeaderContainer = view.FindViewById<View>(Resource.Id.HeaderImageContainer);

            if (string.IsNullOrEmpty(nodeId))
            {
                imageHeaderContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                headersRefreshLayout.Enabled = false;

                imageHeaderContainer.SetOnClickListener(this);

                /*if (!string.IsNullOrEmpty(animationImageId))
                {
                    var cachedImage = AppData.ImageCache.Get(animationImageId);
                    if (cachedImage != null)
                    {
                        imageHeader.SetImageBitmap(ImageUtils.DecodeImage(cachedImage.Image));
                    }
                }*/
            }

            imageModel = new ImageModel(Activity);
            menuModel = new MenuModel(Activity, this);
            basketModel = new BasketModel(Activity);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            (Activity as HospActivity).AddObserver(this);

            var newShowAsList = PreferenceUtils.GetBool(Activity, PreferenceUtils.ShowListAsList);

            if (menuNodeRecyclerView.GetAdapter() == null || newShowAsList != showAsList)
            {
                showAsList = newShowAsList;

                if (menuNodeRecyclerView.GetAdapter() == null)
                {
                    HasOptionsMenu = true;
                }
                else
                {
                    Activity.SupportInvalidateOptionsMenu();
                }

                adapter = new MenuNodeAdapter(Activity, this);
                adapter.SetMode(showAsList, columns);

                if (showAsList)
                {
                    layoutManager = new LinearLayoutManager(Activity);
                }
                else
                {
                    layoutManager = new GridLayoutManager(Activity, columns);//, StaggeredGridLayoutManager.Vertical);
                    (layoutManager as GridLayoutManager).SetSpanSizeLookup(new GridSpanSizeLookup(adapter, columns));
                }

                menuNodeRecyclerView.SetLayoutManager(layoutManager);

                menuNodeRecyclerView.SetAdapter(adapter);

                if (AppData.MobileMenu == null || AppData.MobileMenu.MenuNodes.Count == 0)
                {
                    menuModel.GetMenus();
                }
                else
                {
                    LoadMenu();
                }
            }
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
                LoadMenu();
            }
        }

        private void LoadMenu()
        {
            if (AppData.MobileMenu == null || AppData.MobileMenu.MenuNodes.Count == 0)
            {
                menuModel.GetMenus();
                return;
            }
            if (AppData.MenuNeedsUpdate)
            {
                menuModel.GetMenus();
            }

            if (string.IsNullOrEmpty(nodeId))
            {
                if (AppData.MobileMenu.MenuNodes.Count > 1)
                {
                    (Activity as HospActivity).SupportActionBar.SetDisplayShowTitleEnabled(false);

                    (Activity as HospActivity).SupportActionBar.NavigationMode = (int)ActionBarNavigationMode.List;

                    (Activity as HospActivity).SupportActionBar.SetListNavigationCallbacks(new MenuSpinnerAdapter(Activity), this);

                    if (string.IsNullOrEmpty(menuId))
                    {
                        (Activity as HospActivity).SupportActionBar.SetSelectedNavigationItem(MenuUtils.DefaultMenu);
                    }
                    else
                    {
                        (Activity as HospActivity).SupportActionBar.SetSelectedNavigationItem(MenuUtils.GetMenuPositionById(menuId));
                    }
                }
                else if(AppData.MobileMenu.MenuNodes.Count == 1)
                {
                    LoadMenu(0);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(menuId))
                {
                    foreach (var menu in AppData.MobileMenu.MenuNodes)
                    {
                        if (menu.MenuNodes.FirstOrDefault(x => x.Id == nodeId) != null)
                        {
                            menuId = menu.Id;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(menuId))
                {
                    selectedMenu = AppData.MobileMenu.MenuNodes.FirstOrDefault(x => x.Id == menuId);
                }
                else if(AppData.MobileMenu.MenuNodes != null && AppData.MobileMenu.MenuNodes.Count > 0)
                {
                    selectedMenu = AppData.MobileMenu.MenuNodes[0];
                }

                if (selectedMenu == null)
                {
                    Activity.Finish();
                    //todo add error message
                    return;
                }

                node = selectedMenu.MenuNodes.FirstOrDefault(x => x.Id == nodeId);

                if (node == null)
                {
                    Activity.Finish();
                    //todo add error message
                    return;
                }

                var mobileMenuNodes = menuService.GetMobileMenuNodes(AppData.MobileMenu, node);

                adapter.SetItems(menuId, mobileMenuNodes);

                imageHeaderContainer.SetBackgroundColor(Color.ParseColor(node.Image.GetAvgColor()));

                LoadImage();

                collapsingToolbar.SetTitle(node.Description);
            }
        }

        private async void LoadImage()
        {
            var imageView = await imageModel.ImageGetById(node.Image?.Id, new ImageSize(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.WidthPixels));

            if (imageView != null && imageHeader != null)
            {
                ImageUtils.CrossfadeImage(imageHeader, ImageUtils.DecodeImage(imageView.Image), imageHeader);
            }
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            if (type == ItemType.Item)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(MenuItemActivity));
                intent.PutExtra(BundleUtils.NodeId, id);
                intent.PutExtra(BundleUtils.MenuId, selectedMenu.Id);
                intent.PutExtra(BundleUtils.Type, itemType);
                intent.PutExtra(BundleUtils.ViewType, (int)SlideControlFragment.ViewTypes.Menu);

                if (node != null)
                {
                    intent.PutExtra(BundleUtils.ParentNodeId, node.Id);
                }

                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.MenuNodeListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                //ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
                ActivityUtils.StartActivityForResultWithAnimation(Activity, HospActivity.OpenMenuItemRequestCode, intent, view);
            }
            else if (type == ItemType.MenuGroup)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(MenuNodeActivity));
                intent.PutExtra(BundleUtils.NodeId, id);
                intent.PutExtra(BundleUtils.MenuId, selectedMenu.Id);
                intent.PutExtra(BundleUtils.ViewType, (int)SlideControlFragment.ViewTypes.Menu);

                if (node != null)
                {
                    intent.PutExtra(BundleUtils.ParentNodeId, node.Id);
                }
                
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                if (!string.IsNullOrEmpty(nodeId))
                {
                    intent.PutExtra(BundleUtils.ParentNodeId, nodeId);
                }

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.MenuNodeListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
            }
            else if (type == ItemType.AddToBasket)
            {
                var itemNode = GetItemNode(id);
                var item = menuService.GetMenuItem(AppData.MobileMenu, itemNode.Id, itemNode.NodeLineType);

                if (menuService.HasAnyRequiredModifers(AppData.MobileMenu, item))
                {
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(MenuItemModificationActivity));
                    intent.PutExtra(BundleUtils.ItemId, item.Id);
                    intent.PutExtra(BundleUtils.MenuId, item.MenuId);
                    intent.PutExtra(BundleUtils.RequiredOnly, true);
                    intent.PutExtra(BundleUtils.Type, (int)itemNode.NodeLineType);

                    StartActivityForResult(intent, AddToBasketItemRequestCode);
                }
                else
                {
                    basketModel.AddItemToBasket(item, 1);
                }
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == AddToBasketItemRequestCode)
            {
                if (resultCode == (int)Result.Ok)
                {
                    BaseModel.ShowToast(View, Resource.String.BasketItemAdded);
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private MenuNode GetNode(string id)
        {
            if (node != null)
            {
                return node.MenuGroupNodes.FirstOrDefault(x => x.Id == id);
            }
            else
            {
                return selectedMenu.MenuNodes.FirstOrDefault(x => x.Id == id);
            }
        }

        private MenuNodeLine GetItemNode(string id)
        {
            if (node != null)
            {
                return node.MenuNodeLines.FirstOrDefault(x => x.Id == id);
            }

            return null;
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if (AppData.MobileMenu == null)
                {
                    menuNodeRecyclerView.Visibility = ViewStates.Gone;
                    progressView.Visibility = ViewStates.Visible;
                }
                else
                {
                    headersRefreshLayout.Refreshing = show;

                    if (Activity is HospActivity)
                    {
                        (Activity as HospActivity).ShowLoadingMessage(Resources.GetString(Resource.String.MenuRefreshingMenu));
                    }
                }
            }
            else
            {
                progressView.Visibility = ViewStates.Gone;
                menuNodeRecyclerView.Visibility = ViewStates.Visible;

                headersRefreshLayout.Refreshing = show;

                if (Activity is HospActivity)
                {
                    (Activity as HospActivity).RemoveLoadingMessage();
                }
            }
        }
        
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.HeaderImageContainer:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(FullScreenImageActivity));
                    intent.PutExtra(BundleUtils.ImageId, node.Image.Id);

                    ActivityUtils.StartActivityWithAnimation(Activity, intent, v);
                    break;
            }
        }

        public bool OnNavigationItemSelected(int position, long p1)
        {
            LoadMenu(position);

            return true;
        }

        private void LoadMenu(int position)
        {
            selectedMenu = AppData.MobileMenu.MenuNodes[position];

            var mobileMenuNodes = menuService.GetMobileMenuNodes(selectedMenu);

            adapter.SetItems(selectedMenu.Id, mobileMenuNodes);

            //(Utils.Utils.GetBaseAdapter(headers.Adapter) as MenuNodeAdapter).SetItems(selectedMenu.MenuNodes, selectedMenu.Id);
            //(Utils.Utils.GetBaseAdapter(headers.Adapter) as MenuNodeAdapter).NotifyDataSetChanged();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            inflater.Inflate(Resource.Menu.ShowListAsMenu, menu);

            var menuItem = menu.FindItem(Resource.Id.MenuViewShowListAs);

            if (showAsList)
            {
                menuItem.SetIcon(Resource.Drawable.ic_view_module_white_24dp);
                menuItem.SetTitle(Resource.String.MenuShowAsGrid);
            }
            else
            {
                menuItem.SetIcon(Resource.Drawable.ic_view_list_white_24dp);
                menuItem.SetTitle(Resource.String.MenuShowAsList);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewShowListAs:
                    showAsList = !showAsList;

                    PreferenceUtils.SetBool(Activity, PreferenceUtils.ShowListAsList, showAsList);

                    if (showAsList)
                    {
                        layoutManager = new LinearLayoutManager(Activity);
                        menuNodeRecyclerView.AddItemDecoration(itemDecoration);
                    }
                    else
                    {
                        layoutManager = new GridLayoutManager(Activity, columns);//, StaggeredGridLayoutManager.Vertical);
                        (layoutManager as GridLayoutManager).SetSpanSizeLookup(new GridSpanSizeLookup(adapter, columns));
                        menuNodeRecyclerView.RemoveItemDecoration(itemDecoration);
                    }

                    adapter.SetMode(showAsList, columns);
                    menuNodeRecyclerView.SetLayoutManager(layoutManager);
                    menuNodeRecyclerView.GetRecycledViewPool().Clear();

                    //var oldAdapter = (Utils.Utils.GetBaseAdapter(headers.Adapter) as MenuNodeAdapter);

                    //headers.Adapter = new MenuNodeAdapter(this, Activity, showAsList);
                    //(Utils.Utils.GetBaseAdapter(headers.Adapter) as MenuNodeAdapter).SetItems(oldAdapter.Items, oldAdapter.MenuId);

                    Activity.SupportInvalidateOptionsMenu();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void OnRefresh()
        {
            menuModel.GetMenus();
        }
    }
}