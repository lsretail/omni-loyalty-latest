using System;

using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using Presentation.Views;

namespace Presentation.Activities.Items
{
    public class ItemCategoriesFragment : CardListFragment, IRefreshableActivity, View.IOnClickListener, IItemClickListener
    {
        private ItemModel model;
        private ItemCategoryAdapter adapter;

        private View contentView;
        private View loadingView;
        private ViewSwitcher switcher;
        private RecyclerView itemCategoriesRecyclerView;
        private View EmptyView;

        public static ItemCategoriesFragment NewInstance()
        {
            var itemCategory = new ItemCategoriesFragment() { Arguments = new Bundle() };
            return itemCategory;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            var view = base.OnCreateView(inflater, container, bundle);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.ItemCategoryScreenToolbar);
            toolbar.SetTitle(Resource.String.ActionbarItems);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            model = new ItemModel(Activity, this);

            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.ItemCategoryViewSwitcher);
            contentView = view.FindViewById(Resource.Id.ItemCategoryViewContent);
            loadingView = view.FindViewById(Resource.Id.ItemCategoryViewLoadingSpinner);

            adapter = new ItemCategoryAdapter(Activity, this, BaseRecyclerAdapter.ListItemSize.Normal);

            itemCategoriesRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.ItemCategoryViewList);
            SetLayoutManager(ShowAsList);
            itemCategoriesRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            itemCategoriesRecyclerView.HasFixedSize = true;

            itemCategoriesRecyclerView.SetAdapter(adapter);

            //todo
            EmptyView = view.FindViewById(Resource.Id.ItemCategoryViewEmptyView);

            view.FindViewById<ColoredButton>(Resource.Id.ItemCategoryViewEmptyViewRetry).SetOnClickListener(this);

            if (AppData.ItemCategories == null)
            {
                LoadItemCategories();
            }
            else
            {
                LoadCategories();
            }
            
            return view;
        }

        public override View CreateView(LayoutInflater inflater)
        {
            return Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ItemCategories);
        }

        public void SetLayoutManager(bool showAsList)
        {
            if (showAsList)
            {
                itemCategoriesRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.Normal);
            }
            else
            {
                itemCategoriesRecyclerView.SetLayoutManager(new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.CardColumns), LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.SmallCard);
            }
        }

        private async void LoadItemCategories()
        {
            await model.GetItemCategories();

            LoadCategories();
        }

        private void LoadCategories()
        {
            //headers.Adapter = new SimpleArrayAdapter(Activity, Resource.Layout.SimpleListItem, itemcategories);
            //headers.Adapter = new ItemCategoryAdapter(Activity, AppData.ItemCategories);

            if (AppData.ItemCategories?.Count == 0)
            {
                EmptyView.Visibility = ViewStates.Visible;
                itemCategoriesRecyclerView.Visibility = ViewStates.Gone;
            }
            else
            {
                adapter.SetItemCategories(AppData.ItemCategories);
                EmptyView.Visibility = ViewStates.Gone;
                itemCategoriesRecyclerView.Visibility = ViewStates.Visible;
            }
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var itemGroupIntent = new Intent();

            itemGroupIntent.PutExtra(BundleConstants.ItemCategoryId, id);

            itemGroupIntent.SetClass(Activity, typeof(ItemGroupsActivity));
            StartActivity(itemGroupIntent);
        }

        private void ShowLoading()
        {
            if (switcher.CurrentView != loadingView)
                switcher.ShowNext();
        }

        private void ShowContent()
        {
            if (switcher.CurrentView != contentView)
                switcher.ShowPrevious();
        }

        public void ShowIndicator(bool show)
        {
            if (show)
                ShowLoading();
            else
                ShowContent();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ItemCategoryViewEmptyViewRetry:
                    LoadItemCategories();
                    break;
            }
        }

        #region MENU

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            inflater.Inflate(Resource.Menu.OpenSearchMenu, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewSearch:

                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(ItemSearchActivity));

                    StartActivity(intent);

                    return true;

            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}