using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;

using IItemClickListener = Presentation.Util.IItemClickListener;
using ImageView = Android.Widget.ImageView;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Activities.Items
{
    public class ItemsFragment : CardListFragment, RecyclerViewOnScrollListener.IRecyclerViewOnScrollListener, View.IOnClickListener, IItemClickListener
    {
        private RecyclerView itemRecyclerView;
        private CollapsingToolbarLayout collapsingToolbar;
        private ImageView imageHeader;
        private View imageHeaderContainer;
        private Toolbar toolbar;

        private ImageModel imageModel;
        private ItemModel model;

        private ItemAdapter adapter;

        private bool isBigScreen;
        private bool showAsList;

        private int pageSize = 7;
        private int pageNumber = 1;
        private bool loadMoreItems;
        private bool isLoadingItems = false;

        private string categoryId;
        private string productGroupId;

        private List<LoyItem> Items;

        public static ItemsFragment NewInstance()
        {
            var itemFragment = new ItemsFragment() { Arguments = new Bundle() };
            return itemFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            categoryId = Arguments.GetString(BundleConstants.ItemCategoryId);
            productGroupId = Arguments.GetString(BundleConstants.ItemgroupId);

            imageModel = new ImageModel(Activity, null);
            model = new ItemModel(Activity);

            showAsList = Util.Utils.PreferenceUtils.GetBool(Activity, Util.Utils.PreferenceUtils.ShowListAsList);
            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            HasOptionsMenu = true;

            var view = base.OnCreateView(inflater, container, bundle);

            toolbar = view.FindViewById<Toolbar>(Resource.Id.ItemsScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            collapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.ItemsScreenCollapsingToolbar);
            imageHeader = view.FindViewById<ImageView>(Resource.Id.ItemsScreenHeader);
            imageHeaderContainer = view.FindViewById<View>(Resource.Id.ItemsImageContainer);

            adapter = new ItemAdapter(Activity, this, BaseRecyclerAdapter.ListItemSize.Normal);

            itemRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.ItemViewItemList);
            SetLayoutManager(showAsList);
            itemRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            itemRecyclerView.HasFixedSize = true;

            itemRecyclerView.SetAdapter(adapter);

            LoadProductGroup();

            return view;
        }

        public override View CreateView(LayoutInflater inflater)
        {
            return Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Items);
        }

        public override void OnResume()
        {
            base.OnResume();

            itemRecyclerView.AddOnScrollListener(new RecyclerViewOnScrollListener(this));
        }

        public override void OnPause()
        {
            itemRecyclerView.ClearOnScrollListeners();

            base.OnPause();
        }

        public void SetLayoutManager(bool showAsList)
        {
            if (showAsList)
            {
                itemRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.Normal);
            }
            else
            {
                var manager = new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.CardColumns), LinearLayoutManager.Vertical, false);
                manager.SetSpanSizeLookup(new BaseRecyclerAdapter.GridSpanSizeLookup(Activity, adapter));
                itemRecyclerView.SetLayoutManager(manager);
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.SmallCard);
            }
        }

        private void LoadProductGroup()
        {
            var group = GetCurrentProductGroup();
            SetHeader(group);

            if (group.Items == null || group.Items.Count == 0)
            {
                this.Items = new List<LoyItem>();
            }

            adapter.IsLoading = true;
            LoadNextItems();
        }

        private async void LoadNextItems()
        {
            if (isLoadingItems)
            {
                return;
            }

            isLoadingItems = true;

            var group = GetCurrentProductGroup();

            List<LoyItem> items = null;

            if (group.Items.Count % pageSize == 0)
            {
                pageNumber = (group.Items.Count / pageSize) + 1;

                items = await model.GetItemsByPage(pageSize, pageNumber, categoryId, productGroupId, string.Empty);

                if (active)
                {
                    if (items == null)
                    {
                        LoadPageError();
                    }
                }
            }
            if (Items == null || Items.Count == 0)
            {
                Items = group.Items;
            }
            
            LoadPageSuccess(items);
            

        }

        private void LoadPageSuccess(List<LoyItem> newItems)
        {
            if (newItems == null || newItems.Count == 0)
            {
                loadMoreItems = false;
            }
            else
            {
                
                if (Items == null || Items.Count == 0)
                {
                    Items = new List<LoyItem>();

                    GetCurrentProductGroup().Items = Items;
                }

                this.Items.AddRange(newItems);

                if (newItems.Count == pageSize)
                {
                    loadMoreItems = true;
                }
                else
                {
                    loadMoreItems = false;
                }
            }

            isLoadingItems = false;

            LoadItemsList(loadMoreItems);
        }

        private void LoadPageError()
        {
            isLoadingItems = false;
        }

        private void LoadItemsList(bool hasMoreItems = true)
        {
            adapter.SetItems(Items);
            adapter.IsLoading = hasMoreItems;
        }

		private async void SetHeader(ProductGroup group)
        {
            collapsingToolbar.SetTitle(group.Description);

            if (group.Images == null || group.Images.Count == 0 || imageHeader.Activated)
			{
				return;
			}

		    imageHeaderContainer.SetOnClickListener(this);

			imageHeaderContainer.SetBackgroundColor(Color.ParseColor(group.Images[0].GetAvgColor()));

            var image = await imageModel.ImageGetById(group.Images[0].Id, new ImageSize(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.WidthPixels));

            if (active && image != null)
            {
                imageHeader.SetImageBitmap(Utils.ImageUtils.DecodeImage(image.Image));
                imageHeader.Activated = true;
            }
		}

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var intent = new Intent();

            intent.PutExtra(BundleConstants.ItemId, id);

            intent.SetClass(Activity, typeof(ItemActivity));
            StartActivity(intent);
        }

        public void OnClick(View v)
        {
            
        }

        private ProductGroup GetCurrentProductGroup()
        {
            ProductGroup productGroup = null;

            var category = AppData.ItemCategories.FirstOrDefault(x => x.Id == categoryId);
            if (category != null)
            {
                productGroup = category.ProductGroups.FirstOrDefault(x => x.Id == productGroupId);
            }

            return productGroup;
        }

        public void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            //do nothing
        }

        public void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            if (loadMoreItems)
            {
                var lastVisibleItem = 0;

                if (recyclerView.GetLayoutManager() is LinearLayoutManager)
                {
                    lastVisibleItem = (recyclerView.GetLayoutManager() as LinearLayoutManager).FindLastVisibleItemPosition();
                }
                else if (recyclerView.GetLayoutManager() is GridLayoutManager)
                {
                    lastVisibleItem = (recyclerView.GetLayoutManager() as GridLayoutManager).FindLastVisibleItemPosition();
                }

                if (lastVisibleItem >= adapter.ItemCount - 3)
                {
                    LoadNextItems();
                }
            }
        }

        #region MENU

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.GenericSearchMenu, menu);

            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewSearch:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof (ItemSearchActivity));

                    StartActivity(intent);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}