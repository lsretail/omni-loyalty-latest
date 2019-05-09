using System.Collections.Generic;
using System.Linq;

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using System.Globalization;

namespace Presentation.Activities.StoreLocator
{
    [Activity(Label = "@string/ActionbarMap")]
    public class StoreLocatorMapActivity : LoyaltyFragmentActivity, IRefreshableActivity, GoogleMap.IOnInfoWindowClickListener, GoogleMap.IOnMarkerClickListener, View.IOnClickListener, ViewTreeObserver.IOnGlobalLayoutListener, GoogleApiClient.IConnectionCallbacks, IOnMapReadyCallback
    {
        private Android.Support.Design.Widget.FloatingActionButton ShowPositionFab;
        private Android.Support.Design.Widget.FloatingActionButton ShowDirectionsFab;

        private GoogleMap map;
        private Dictionary<string, string> markerIdDictionary = new Dictionary<string, string>(); 
        private string selectedMarker = "";
        private string storeId;
        private string[] storeIds;
        private bool returnStoreId;

        private GoogleApiClient googleApiClient;
        private Location myLocation;

        private bool showPositionButton = false;

        private bool askedForLocationPermission = false;
        private bool useLocation;

        //private LocationManager locationManager;

        private List<Store> Stores
        {
            get
            {
                if (AppData.Stores == null || AppData.Stores.Count == 0)
                {
                    return null;
                }

                List<Store> stores;

                if (string.IsNullOrEmpty(storeId))
                    stores = storeIds.Select(id => AppData.Stores.FirstOrDefault(x => x.Id == id)).ToList();
                else
                    stores = AppData.Stores.Where(x => x.Id == storeId).ToList();

                return stores;
            }
        } 

        public enum MapAction
        {
            None = 0,
            Direction = 1,
            StoreInfo = 2
        }

        protected override void OnCreate(Bundle bundle)
        {
            HasLeftDrawer = false;
            HasRightDrawer = false;

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.StoreLocatorMap);

            var toolbar = FindViewById<Toolbar>(Resource.Id.StoreLocatorMapScreenToolbar);
            SetSupportActionBar(toolbar);

            ShowPositionFab = FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.StoreLocatorMapViewShowCurrent);
            ShowDirectionsFab = FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.StoreLocatorMapViewShowDirections);

            ShowPositionFab.SetOnClickListener(this);
            ShowDirectionsFab.SetOnClickListener(this);

            if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
            {
                ShowPositionFab.Visibility = ViewStates.Gone;
                showPositionButton = true;
            }

            ShowPositionFab.SetColorFilter(Utils.ImageUtils.GetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.fab_accent))));

            //locationManager = LocationManager.NewInstance(this, this, this);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            storeId = Intent.Extras.GetString(BundleConstants.StoreId);
            storeIds = Intent.Extras.GetStringArray(BundleConstants.StoreIds);

            if (Intent.Extras.ContainsKey(BundleConstants.ClickReturnId))
            {
                returnStoreId = Intent.Extras.GetBoolean(BundleConstants.ClickReturnId);
            }

            googleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(OnConnectionFailed)
                .AddApi(LocationServices.API)
                .Build();

            InitMap();

            var content = FindViewById(ContentId);
            Util.Utils.ViewUtils.AddOnGlobalLayoutListener(content, this);
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                useLocation = true;
                ResumeMap();
            }
            else
            {
                if (askedForLocationPermission)
                {
                    var dialog = new WarningDialog(this, "");
                    dialog.Message = GetString(Resource.String.StoreMapCannotAccessLocation);
                    dialog.SetPositiveButton(GetString(Android.Resource.String.Ok), () => { });
                    dialog.SetNegativeButton(GetString(Resource.String.ApplicationOpenSettings), () =>
                    {
                        Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                        var uri = Android.Net.Uri.FromParts("package", PackageName, null);
                        intent.SetData(uri);
                        StartActivityForResult(intent, 1);
                    });
                    dialog.Show();

                    useLocation = false;
                    ResumeMap();
                }
                else
                {
                    askedForLocationPermission = true;

                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                    {
                        RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation }, 0);
                    }
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Pause the GPS - we won't have to worry about showing the 
            // location.
            if (map != null)
                map.MyLocationEnabled = false;

            googleApiClient.Disconnect();
            //locationClient.Connect();
        }

        private void ResumeMap()
        {
            if (useLocation)
            {
                googleApiClient.Connect();
            }

            SetupMap();
        }

        public void OnGlobalLayout()
        {
            if(string.IsNullOrEmpty(storeId))
                HideDirectionsButton();

            var content = FindViewById(ContentId);
            Util.Utils.ViewUtils.RemoveOnGlobalLayoutListener(content, this);
        }

        /// <summary>
        ///   Add markers to the map.
        /// </summary>
        private void AddMarkersToMap()
        {
            if (AppData.Stores == null || AppData.Stores.Count == 0)
            {
                LoadStoresFromServer();
            }
            else
            {
                AddStoreMarkersToMap();
            }

            PanToLocation();
        }

        private async void LoadStoresFromServer()
        {
            var model = new StoreModel(this, this);
            await model.GetAllStores();

            AddStoreMarkersToMap();
        }

        private void AddStoreMarkersToMap()
        {
            foreach (Store store in Stores)
            {
                if (store != null)
                {
                    var mapOption = new MarkerOptions()
                        .SetPosition(new LatLng(double.Parse(store.Latitude.ToString(CultureInfo.InvariantCulture)), double.Parse(store.Longitude.ToString(CultureInfo.InvariantCulture))))
                        .SetTitle(store.Description)
                        .SetSnippet(store.Address.Address1 + '\n' + store.Address.City)
                        .Visible(true);
                    var marker = map.AddMarker(mapOption);
                    markerIdDictionary.Add(marker.Id, store.Id);
                }
            }
        }

        /// <summary>
        ///   All we do here is add a SupportMapFragment to the Activity.
        /// </summary>
        private void InitMap()
        {
            var mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeNormal)
                .InvokeCompassEnabled(false)
                .InvokeZoomControlsEnabled(false);
            
            var fragTx = SupportFragmentManager.BeginTransaction();
            var mapFragment = SupportMapFragment.NewInstance(mapOptions);

            fragTx.Add(Resource.Id.StoreLocatorMapViewMap, mapFragment, "map");
            fragTx.Commit();
        }

        private void SetupMap()
        {
            if (map == null)
            {
                var fragment = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
                if (fragment != null)
                {
                    fragment.GetMapAsync(this);
                }
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.UiSettings.MyLocationButtonEnabled = showPositionButton;

            if (useLocation)
            {
                // Enable the my-location layer.
                map.MyLocationEnabled = true;
            }

            AddMarkersToMap();

            map.SetOnMarkerClickListener(this);
            map.SetOnInfoWindowClickListener(this);
            map.SetPadding(0, Resources.GetDimensionPixelSize(Resource.Dimension.abc_action_bar_default_height_material), 0, 0);
        }

        private void ShowStoreInfo()
        {
            string value;
            if (string.IsNullOrEmpty(storeId) && markerIdDictionary.TryGetValue(selectedMarker, out value))
            {
                var intent = new Intent();

                intent.PutExtra(BundleConstants.StoreId, value);

                intent.SetClass(this, typeof(StoreLocatorStoreDetailActivity));
                StartActivity(intent);
            }

            Finish();
        }

        private void ShowDirections()
        {
            string value;
            if (!markerIdDictionary.TryGetValue(selectedMarker, out value))
                if (!string.IsNullOrEmpty(storeId))
                    value = storeId;

            var store = Stores.FirstOrDefault(x => x.Id == value);

            ShowDirections(store);

            Finish();
        }

        private void ShowDirections(Store store)
        {
            var intent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?saddr=&daddr=" + store.Latitude.ToString(CultureInfo.InvariantCulture) + "," + store.Longitude.ToString(CultureInfo.InvariantCulture)));
            StartActivity(intent);
        }

        #region MENU

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);

            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.StoreLocatorMapMenu, menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                default:
                    OnBackPressed();
                    break;
            }
            return true;
        }

        #endregion

        public void ShowIndicator(bool show)
        {
            
        }

        public void PanToLocation()
        {
            if (Stores == null)
                return;

            if (Stores.Count != 1)
            {
                PanToUserLocation();
            }
            else
            {
                var store = Stores[0];
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(double.Parse(store.Latitude.ToString(CultureInfo.InvariantCulture)), double.Parse(store.Longitude.ToString(CultureInfo.InvariantCulture))), 14));
            }
        }

        private void PanToUserLocation()
        {
            if (myLocation != null)
            {
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(double.Parse(myLocation.Latitude.ToString(CultureInfo.InvariantCulture)), double.Parse(myLocation.Longitude.ToString(CultureInfo.InvariantCulture))), 14));
            }
        }

        public void OnInfoWindowClick(Marker marker)
        {
            if (returnStoreId)
            {
                Intent returnIntent = new Intent();

                string value;
                if (markerIdDictionary.TryGetValue(selectedMarker, out value))
                    returnIntent.PutExtra(BundleConstants.StoreId, value);

                SetResult(Result.Ok, returnIntent);
                Finish();
            }
            else
            {
                selectedMarker = marker.Id;

                ShowStoreInfo();
            }
        }

        public bool OnMarkerClick(Marker marker)
        {
            marker.ShowInfoWindow();

            selectedMarker = marker.Id;

            ShowDirectionsButton();

            InvalidateOptionsMenu();

            return true;
        }

        private void ShowDirectionsButton()
        {
            //ShowDirectionsFab.show(true);
            //ShowPositionFab.show(true);
            ShowDirectionsFab.Visibility = ViewStates.Visible;
        }

        private void HideDirectionsButton()
        {
            //ShowDirectionsFab.hide(false);
            //ShowPositionFab.hide(false);
            ShowDirectionsFab.Visibility = ViewStates.Gone;
        }

        private void OnConnectionFailed(ConnectionResult connectionResult)
        {
            /*
             * Google Play services can resolve some errors it detects.
             * If the error has a resolution, try sending an Intent to
             * start a Google Play services activity that can resolve
             * error.
             */
            if (connectionResult.HasResolution)
            {
                try
                {
                    // Start an Activity that tries to resolve the error
                    connectionResult.StartResolutionForResult(this, 9000);
                    /*
                     * Thrown if Google Play services canceled the original
                     * PendingIntent
                     */
                }
                catch (IntentSender.SendIntentException e)
                {
                    // Log the error
                    Utils.LogUtils.Log(e.Message);
                    Utils.LogUtils.Log(e.StackTrace);
                }
            }
            else
            {
                /*
                 * If no resolution is available, display a dialog to the
                 * user with the error.
                 */
                //showErrorDialog(connectionResult.getErrorCode());
            }
        }

        public void OnConnected(Bundle p0)
        {
            myLocation = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);

            PanToLocation();
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.StoreLocatorMapViewShowCurrent:
                    PanToUserLocation();
                    break;

                case Resource.Id.StoreLocatorMapViewShowDirections:
                    ShowDirections();
                    break;
            }
        }
    }
}