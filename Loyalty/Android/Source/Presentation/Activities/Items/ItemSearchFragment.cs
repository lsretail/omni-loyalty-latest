using System;
using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using Presentation.Views;
using SearchView = Android.Support.V7.Widget.SearchView;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation.Activities.Items
{
    public class ItemSearchFragment : LoyaltyFragment, IItemClickListener, SearchView.IOnQueryTextListener, RecyclerViewOnScrollListener.IRecyclerViewOnScrollListener
    {
        private LoyaltyRecyclerView itemRecyclerView;
        private SearchView searchView;
        private View emptyView;

        private RecyclerViewOnScrollListener onScrollListener;
        private ItemModel model;

        private ItemAdapter adapter;

        private int lastSearchLength = 0;

        private int pageSize = 7;
        private int pageNumber = 1;
        private bool loadMoreItems;

        private string searchKey;

        public List<LoyItem> Items { get; private set; }

        public static ItemsFragment NewInstance()
        {
            var itemFragment = new ItemsFragment() { Arguments = new Bundle() };
            return itemFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            model = new ItemModel(Activity);

            HasOptionsMenu = true;

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ItemSearchScreen);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.ItemSearchScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            adapter = new ItemAdapter(Activity, this, BaseRecyclerAdapter.ListItemSize.Normal);

            itemRecyclerView = view.FindViewById<LoyaltyRecyclerView>(Resource.Id.ItemSearchScreenItems);
            itemRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
            adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.Normal);

            itemRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            itemRecyclerView.HasFixedSize = true;

            itemRecyclerView.SetAdapter(adapter);

            searchView = view.FindViewById<SearchView>(Resource.Id.ItemSearchScreenSearch);
            searchView.SetOnQueryTextListener(this);

            searchView.SetIconifiedByDefault(false);

            emptyView = view.FindViewById<TextView>(Resource.Id.ItemSearchScreenEmptyView);

            onScrollListener = new RecyclerViewOnScrollListener(this);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            itemRecyclerView.AddOnScrollListener(onScrollListener);
        }

        public override void OnPause()
        {
            itemRecyclerView.RemoveOnScrollListener(onScrollListener);

            base.OnPause();
        }

        private void LoadItemsList(bool hasMoreItems = true)
        {
            adapter.SetItems(Items);
            adapter.IsLoading = hasMoreItems;
        }

        private void LoadItems()
        {
            loadMoreItems = Items.Count % pageSize == 0;

            LoadItemsList(loadMoreItems);

            itemRecyclerView.SetEmptyView(emptyView);
        }

        public void LoadSearch(string searchKey)
        {
            if (this.searchKey == searchKey)
                return;

            this.searchKey = searchKey;

            if (View != null)
                LoadSearch();
        }

        private async void LoadSearch()
        {
            if (Items == null)
            {
                Items = new List<LoyItem>();
                adapter.SetItems(new List<LoyItem>());
                adapter.IsLoading = false;
                //headers.Adapter = null;
            }
            else
            {
                Items.Clear();
            }

            LoadItemsList();

            pageNumber = 1;

            var items = await model.GetItemsByPage(pageSize, pageNumber, string.Empty, string.Empty, searchKey);

            if (items != null)
            {
                LoadPageSuccess(items);
            }
        }

        private void LoadPageSuccess(List<LoyItem> items)
        {
            if (this.Items?.Count == 0)
            {
                itemRecyclerView.SetEmptyView(emptyView);
            }

            this.Items.AddRange(items);

            if (items.Count == pageSize)
                loadMoreItems = true;

            LoadItemsList(loadMoreItems);
        }

        public void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            //do nothing
        }

        public async void OnScrolled(RecyclerView recyclerView, int dx, int dy)
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
                    loadMoreItems = false;

                    var items = await model.GetItemsByPage(pageSize, ++pageNumber, string.Empty, string.Empty, searchKey);

                    if (items != null)
                    {
                        LoadPageSuccess(items);
                    }
                }
            }
        }

        public bool OnQueryTextChange(string newText)
        {
            if (searchView.Query.Length > 2)
            {
                if (newText.Length > lastSearchLength)
                    LoadSearch(searchView.Query);
            }

            lastSearchLength = newText.Length;

            return false;
        }

        public bool OnQueryTextSubmit(string query)
        {
            ((InputMethodManager)Activity.GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(searchView.WindowToken, 0);

            if (searchView.Query.Length < 3)
            {
                var dialog = new WarningDialog(Activity, "")
                {
                    Message = GetString(Resource.String.ItemSearchViewSearchToShortError)
                };
                dialog.Show();
            }
            else
            {
                LoadSearch(searchView.Query);
            }

            return true;
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var intent = new Intent();

            intent.PutExtra(BundleConstants.ItemId, id);

            intent.SetClass(Activity, typeof(ItemActivity));
            StartActivity(intent);
        }
    }
}