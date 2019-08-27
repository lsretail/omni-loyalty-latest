using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Java.Lang;
using Presentation.Activities.Base;
using Presentation.Activities.Coupons;
using Presentation.Activities.History;
using Presentation.Activities.Items;
using Presentation.Activities.Notifications;
using Presentation.Activities.Offers;
using Presentation.Activities.StoreLocator;
using Presentation.Adapters;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Activities.Search
{
    public class GeneralSearchFragment : LoyaltyFragment, IItemClickListener, IRefreshableActivity, View.IOnClickListener, Android.Support.V7.Widget.SearchView.IOnQueryTextListener, Android.Support.V7.Widget.SearchView.IOnCloseListener
    {
        private RecyclerView searchRecyclerView;
        private Android.Support.V7.Widget.SearchView searchKey;
        private View loadingView;
        private ViewSwitcher switcher;

        private SearchAdapter adapter;
        private List<SearchType> availableTypes;
        private bool[] selectedTypes;
        private string[] typeNames;
        private GeneralSearchModel searchModel;

        private int lastSearchLength = 0;

        public SearchRs CurrentResults { get; private set; }


        public static GeneralSearchFragment NewInstance()
        {
            var searchFragment = new GeneralSearchFragment() { Arguments = new Bundle() };
            return searchFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;

            if (availableTypes == null)
            {
                availableTypes = new List<SearchType>();

                if (Arguments != null && Arguments.ContainsKey(BundleConstants.SearchType))
                {
                    availableTypes.Add((SearchType)Arguments.GetInt(BundleConstants.SearchType));
                }
                else
                {
                    if (EnabledItems.HasItemCatalog)
                    {
                        availableTypes.Add(SearchType.Item);
                    }

                    if (EnabledItems.HasOffers)
                    {
                        availableTypes.Add(SearchType.Offer);
                    }

                    if (EnabledItems.HasCoupons)
                    {
                        availableTypes.Add(SearchType.Coupon);
                    }

                    if (EnabledItems.HasNotifications)
                    {
                        availableTypes.Add(SearchType.Notification);
                    }

                    if (EnabledItems.HasHistory)
                    {
                        availableTypes.Add(SearchType.SalesEntry);
                    }

                    if (EnabledItems.HasWishLists)
                    {
                        availableTypes.Add(SearchType.OneList);
                    }

                    if (EnabledItems.HasStoreLocator)
                    {
                        availableTypes.Add(SearchType.Store);
                    }
                }
            }

            if (selectedTypes == null)
            {
                selectedTypes = new bool[availableTypes.Count];

                for (int i = 0; i < availableTypes.Count; i++)
                {
                    selectedTypes[i] = true;
                }
            }

            if (typeNames == null)
            {
                typeNames = new string[availableTypes.Count];

                for (int i = 0; i < availableTypes.Count; i++)
                {
                    if (availableTypes[i] == SearchType.Item)
                    {
                        typeNames[i] = GetString(Resource.String.GeneralSearchViewItem);
                    }
                    else if (availableTypes[i] == SearchType.Offer)
                    {
                        typeNames[i] = GetString(Resource.String.GeneralSearchViewOffer);
                    }
                    else if (availableTypes[i] == SearchType.Coupon)
                    {
                        typeNames[i] = GetString(Resource.String.GeneralSearchViewCoupon);
                    }
                    else if (availableTypes[i] == SearchType.Notification)
                    {
                        typeNames[i] = GetString(Resource.String.GeneralSearchViewNotification);
                    }
                    else if (availableTypes[i] == SearchType.SalesEntry)
                    {
                        typeNames[i] = GetString(Resource.String.GeneralSearchViewTransaction);
                    }
                    else if (availableTypes[i] == SearchType.OneList)
                    {
                        typeNames[i] = GetString(Resource.String.ShoppingListDetailViewWishlist);
                    }
                    else if (availableTypes[i] == SearchType.Store)
                    {
                        typeNames[i] = GetString(Resource.String.GeneralSearchViewStore);
                    }
                }
            }

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.GeneralSearch);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.GeneralSearchScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            searchModel = new GeneralSearchModel(Activity, this);

            searchRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.GeneralSearchViewItemList);
            searchRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            searchRecyclerView.HasFixedSize = true;
            searchRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));

            adapter = new SearchAdapter(Activity, this);
            searchRecyclerView.SetAdapter(adapter);

            searchKey = view.FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.GeneralSearchView);
            searchKey.SetOnQueryTextListener(this);
            searchKey.SetOnSearchClickListener(this);
            searchKey.SetOnCloseListener(this);

            searchKey.SetIconifiedByDefault(false);
            //searchKey.Iconified = false;

            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.GeneralSearchViewSwitcher);
            loadingView = view.FindViewById(Resource.Id.GeneralSearchViewLoadingSpinner);
            var filter = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.GeneralSearchViewFilter);

            filter.SetOnClickListener(this);
            if (availableTypes.Count == 1)
            {
                filter.Hide();
            }

            return view;
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var itemType = (ItemClickType) type;

            Intent intent = new Intent();

            if (itemType == ItemClickType.Item)
            {
                intent.PutExtra(BundleConstants.ItemId, id);
                intent.SetClass(Activity, typeof (ItemActivity));
            }
            else if (itemType == ItemClickType.Transaction)
            {
                intent.PutExtra(BundleConstants.TransactionId, id);
                intent.SetClass(Activity, typeof (TransactionDetailActivity));
            }
            else if (itemType == ItemClickType.Notification)
            {
                intent.PutExtra(BundleConstants.NotificationId, id);
                intent.SetClass(Activity, typeof (NotificationDetailActivity));
            }
            else if (itemType == ItemClickType.Offer)
            {
                intent.PutExtra(BundleConstants.OfferId, id);
                intent.SetClass(Activity, typeof (OfferDetailActivity));
            }
            else if (itemType == ItemClickType.Coupon)
            {
                intent.PutExtra(BundleConstants.CouponId, id);
                intent.SetClass(Activity, typeof (CouponDetailActivity));
            }
            else if (itemType == ItemClickType.ShoppingListLine)
            {
                var shoppingList = CurrentResults.OneLists.FirstOrDefault();
                var shoppingListLine = shoppingList.Items.FirstOrDefault(x => x.Id == id);

                if (shoppingListLine != null)
                {
                    intent.SetClass(Activity, typeof (ItemActivity));
                    intent.PutExtra(BundleConstants.ItemId, shoppingListLine.ItemId);
                    if (!string.IsNullOrEmpty(shoppingListLine.UnitOfMeasureId))
                        intent.PutExtra(BundleConstants.SelectedUomId, shoppingListLine.UnitOfMeasureId);
                    if (!string.IsNullOrEmpty(shoppingListLine.VariantId))
                        intent.PutExtra(BundleConstants.SelectedVariantId, shoppingListLine.VariantId);
                }
            }
            else if (itemType == ItemClickType.Store)
            {
                intent.PutExtra(BundleConstants.StoreId, id);
                intent.SetClass(Activity, typeof (StoreLocatorStoreDetailActivity));
            }

            StartActivity(intent);
        }

        private async void Search(bool resetSearch)
        {
            if (resetSearch)
                searchModel.ResetSearch();

            SearchType searchType = availableTypes.Where((t, i) => selectedTypes[i]).Aggregate<SearchType, SearchType>(0, (current, t) => current | t);

            var results = await searchModel.Search(searchKey.Query, searchType);

            if (results != null)
            {
                LoadResults(results);
            }
        }

        private void LoadResults(SearchRs results)
        {
            CurrentResults = results;
            adapter.SetSearchResults(Activity, availableTypes.Where((t, i) => selectedTypes[i]).ToList(), results);
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (actionId)
            {
                case ImeAction.Search:
                    if (searchKey.Query.Length < 3)
                    {
                        WarningDialog dialog = new WarningDialog(Activity, "");
                        dialog.Message = GetString(Resource.String.GeneralSearchViewSearchStringToShort);
                        dialog.Show();
                    }
                    else
                    {
                        Search(false);
                        ((InputMethodManager)Activity.GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(searchKey.WindowToken, 0);
                    }
                    break;
            }
            return false;
        }

        public void AfterTextChanged(IEditable s)
        {
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (searchKey.Query.Length > 2)
            {
                if (count > before)
                    Search(false);
                else
                    searchModel.ResetSearch();
            }
        }

        private void ShowContent()
        {
            if (switcher.CurrentView != searchRecyclerView)
                switcher.ShowPrevious();
        }

        private void ShowLoading()
        {
            if (switcher.CurrentView != loadingView)
                switcher.ShowNext();
        }

        public void ShowIndicator(bool show)
        {
            if (show)
                ShowLoading();
            else
                ShowContent();
        }

        public bool OnQueryTextChange(string newText)
        {
            if (searchKey.Query.Length > 2)
            {
                if (newText.Length > lastSearchLength)
                    Search(false);
                else
                    searchModel.ResetSearch();
            }

            lastSearchLength = newText.Length;

            return false;
        }

        public bool OnQueryTextSubmit(string query)
        {
            if (searchKey.Query.Length < 3)
            {
                WarningDialog dialog = new WarningDialog(Activity, "");
                dialog.Message = GetString(Resource.String.GeneralSearchViewSearchStringToShort);
                dialog.Show();
            }
            else
            {
                Search(false);
                ((InputMethodManager)Activity.GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(searchKey.WindowToken, 0);
            }

            return false;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.GeneralSearchViewFilter:
                    var filterDialog = new MultiChoiceAlertDialog(Activity, typeNames, selectedTypes, GetString(Resource.String.GeneralSearchViewChooseCategories));
                    filterDialog.SetPositiveButton(GetString(Resource.String.ApplicationOk),
                                                   () => Search(true));
                    filterDialog.Show();
                    break;

                case Resource.Id.MenuViewSearch:
                    break;
            }
        }

        public bool OnClose()
        {
            return false;
        }
    }
}