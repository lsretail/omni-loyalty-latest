using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Activities.Search;
using Presentation.Adapters;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using IItemClickListener = Presentation.Util.IItemClickListener;
using Utils = Presentation.Util.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using System.Globalization;

namespace Presentation.Activities.StoreLocator
{
    public class StoreLocatorMasterFragment : LoyaltyFragment, IRefreshableActivity, ViewTreeObserver.IOnGlobalLayoutListener, View.IOnClickListener//, Util.ILocationListener, IConnectionListener
    {
        public IItemClickListener ItemClickListener { get; set; }

        private StoreModel storeModel;

        private RecyclerView storeRecyclerView;
        private View progressIndicator;
        private Android.Support.Design.Widget.FloatingActionButton showOnMapFAB;

        private StoreLocatorHeaderAdapter adapter;
        //private LocationManager locationManager;
        private Action googleApiClientPendingAction;

        private string itemId;
        private string variantId;
        private string itemDescription;
        public List<Store> Stores { get; private set; }
        
        private bool showAsList;
        private bool isBigScreen;

        private string focusedItemId = string.Empty;

        public static StoreLocatorMasterFragment NewInstance()
        {
            var storeLocatorDetail = new StoreLocatorMasterFragment() { Arguments = new Bundle() };
            return storeLocatorDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //locationManager = LocationManager.NewInstance(Activity, this, this);

            showAsList = Util.Utils.PreferenceUtils.GetBool(Activity, Util.Utils.PreferenceUtils.ShowListAsList);
            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);
            
            HasOptionsMenu = true;

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.StoreLocator);


            if (Arguments != null)
            {
                itemId = Arguments.GetString(BundleConstants.ItemId);
                variantId = Arguments.GetString(BundleConstants.VariantId);
                itemDescription = Arguments.GetString(BundleConstants.ItemDescription);
            }

            storeModel = new StoreModel(Activity, this);
            
            progressIndicator = view.FindViewById<View>(Resource.Id.StoreLocatorViewLoadingSpinner);
            showOnMapFAB = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.StoreLocatorViewShowOnMap);

            adapter = new StoreLocatorHeaderAdapter(Activity, ItemClickListener, BaseRecyclerAdapter.ListItemSize.Normal);

            storeRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.StoreLocatorViewStoreList);
            SetLayoutManager(showAsList);
            storeRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            storeRecyclerView.HasFixedSize = true;

            storeRecyclerView.SetAdapter(adapter);

            showOnMapFAB.SetOnClickListener(this);

            if (savedInstanceState != null && savedInstanceState.ContainsKey(BundleConstants.SelectedItemId))
            {
                focusedItemId = savedInstanceState.GetString(BundleConstants.SelectedItemId);
            }

            Util.Utils.ViewUtils.AddOnGlobalLayoutListener(view, this);

            return view;
        }

        public override void OnSaveInstanceState(Bundle bundle)
        {
            base.OnSaveInstanceState(bundle);
        }

        public void SetLayoutManager(bool showAsList)
        {
            if (showAsList)
            {
                storeRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.Normal);
            }
            else
            {
                var manager = new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.CardColumns), LinearLayoutManager.Vertical, false);
                manager.SetSpanSizeLookup(new BaseRecyclerAdapter.GridSpanSizeLookup(Activity, adapter));
                storeRecyclerView.SetLayoutManager(manager);
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.SmallCard);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            //locationManager.Connect();
        }

        public override void OnPause()
        {
            //locationManager.Disconnect();

            base.OnPause();
        }

        public void OnGlobalLayout()
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                LoadItemStockStores();
            }
            else
            {
                Stores = AppData.Stores;

                if (Stores == null || Stores.Count == 0)
                {
                    LoadStoresFromServer();
                }
                else
                {
                    LoadStores();
                }
            }

            Util.Utils.ViewUtils.RemoveOnGlobalLayoutListener(View, this);
        }

        private async void LoadItemStockStores()
        {
            var stores = await storeModel.GetItemsInStock(itemId, variantId, 0, 0, 0);

            LoadStores(stores);
        }

        private async void LoadStoresFromServer()
        {
            await storeModel.GetAllStores();

            LoadStores();
        }

        private void LoadStores()
        {
            LoadStores(AppData.Stores);
        }

        private void LoadStores(List<Store> stores)
        {
            if (!string.IsNullOrEmpty(itemId) && stores.Count == 0)
            {
                var dialog = new WarningDialog(Activity, string.Empty);
                dialog.Message = GetString(Resource.String.StorelocatorViewNotInStock);
                dialog.SetPositiveButton(GetString(Resource.String.ApplicationOk), () => Activity.Finish());
                dialog.Show();
            }
            else
            {
                this.Stores = stores;

                if (string.IsNullOrEmpty(itemDescription))
                {
                    adapter.SetStores(stores);
                }
                else
                {
                    adapter.SetStores(stores, string.Format(GetString(Resource.String.StorelocatorViewInStockStores), itemDescription));
                }
            }
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                storeRecyclerView.Visibility = ViewStates.Gone;
                progressIndicator.Visibility = ViewStates.Visible;
            }
            else
            {
                storeRecyclerView.Visibility = ViewStates.Visible;
                progressIndicator.Visibility = ViewStates.Gone;
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
                    intent.SetClass(Activity, typeof(GeneralSearchActivity));
                    intent.PutExtra(BundleConstants.SearchType, (int)SearchType.Store);

                    StartActivity(intent);

                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        private void ShowStoresOnMap()
        {
            if (Utils.CheckForPlayServices(Activity))
            {
                var stores = Stores;

                string[] storeIds = new string[stores.Count];

                for (int i = 0; i < stores.Count; i++)
                {
                    storeIds[i] = stores[i].Id;
                }

                var intent = new Intent();
                intent.SetClass(Activity, typeof(StoreLocatorMapActivity));

                intent.PutExtra(BundleConstants.StoreIds, storeIds);

                StartActivityForResult(intent, 0);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == (int)Result.Ok)
            {
                var action = data.GetIntExtra(BundleConstants.Action, (int)StoreLocatorMapActivity.MapAction.None);
                if ((StoreLocatorMapActivity.MapAction)action != StoreLocatorMapActivity.MapAction.None)
                {
                    var value = data.GetStringExtra(BundleConstants.Value);

                    switch ((StoreLocatorMapActivity.MapAction)action)
                    {
                        case StoreLocatorMapActivity.MapAction.StoreInfo:
                            ItemClickListener.ItemClicked((int)ItemClickType.Store, value, string.Empty, null);
                            //storeToLoad = value;
                            break;

                        case StoreLocatorMapActivity.MapAction.Direction:
                            ShowDirections(Stores.FirstOrDefault(x => x.Id == value));
                            break;
                    }
                }
            }
        }

        private void ShowDirections(Store store)
        {
            var intent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?saddr=&daddr=" + store.Latitude.ToString(CultureInfo.InvariantCulture) + "," + store.Longitude.ToString(CultureInfo.InvariantCulture)));
            StartActivity(intent);
        }

        public void OnConnected(Bundle connectionHint)
        {
            if (googleApiClientPendingAction != null)
            {
                googleApiClientPendingAction();
                googleApiClientPendingAction = null;
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.StoreLocatorViewShowOnMap:
                    ShowStoresOnMap();
                    break;
            }
        }
    }
}