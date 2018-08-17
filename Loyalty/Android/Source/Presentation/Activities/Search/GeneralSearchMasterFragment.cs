using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Java.Lang;
using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using SearchView = Android.Support.V7.Widget.SearchView;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Activities.Search
{
    public class GeneralSearchMasterFragment : LoyaltyFragment, IRefreshableActivity, SearchView.IOnQueryTextListener, View.IOnClickListener, SearchView.IOnCloseListener
    {
        private RecyclerView searchRecyclerView;
        private SearchView searchKey;
        private View loadingView;
        private ViewSwitcher switcher;

        private SearchAdapter adapter;
        private List<SearchType> availableTypes;
        private bool[] selectedTypes;
        private string[] typeNames;
        private GeneralSearchModel searchModel;

        private int lastSearchLength = 0;

        public IItemClickListener ItemClickListener { get; set; }
        public SearchRs CurrentResults { get; private set; }

        public static GeneralSearchMasterFragment NewInstance()
        {
            var searchFragment = new GeneralSearchMasterFragment() { Arguments = new Bundle() };
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
                    availableTypes.Add((SearchType) Arguments.GetInt(BundleConstants.SearchType));
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
                        availableTypes.Add(SearchType.Transaction);
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
                    else if (availableTypes[i] == SearchType.Transaction)
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

            searchModel = new GeneralSearchModel(Activity, this);

            searchRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.GeneralSearchViewItemList);
            searchRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            searchRecyclerView.HasFixedSize = true;
            searchRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));

            adapter = new SearchAdapter(Activity, ItemClickListener);
            searchRecyclerView.SetAdapter(adapter);

            searchKey = view.FindViewById<SearchView>(Resource.Id.GeneralSearchView);
            searchKey.SetOnQueryTextListener(this);
            searchKey.SetOnSearchClickListener(this);
            searchKey.SetOnCloseListener(this);

            searchKey.SetIconifiedByDefault(false);
            //searchKey.Iconified = false;

            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.GeneralSearchViewSwitcher);
            loadingView = view.FindViewById(Resource.Id.GeneralSearchViewLoadingSpinner);
            var filter = view.FindViewById<FloatingActionButton>(Resource.Id.GeneralSearchViewFilter);

            filter.SetOnClickListener(this);
            if (availableTypes.Count == 1)
            {
                filter.Visibility = ViewStates.Gone;
            }

            return view;
        }

        private async void Search(bool resetSearch)
        {
            if(resetSearch)
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
            if(show)
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