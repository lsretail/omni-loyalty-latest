using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Models;
using Presentation.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Globalization;
using Android;

namespace Presentation.Activities.Store
{
    [Activity(Label = "Map", Theme = "@style/BaseThemeNoActionBar")]
    public class StoreMapActivity : HospActivityNoStatusBar, GoogleMap.IOnInfoWindowClickListener, GoogleMap.IOnMarkerClickListener, IRefreshableActivity, ViewTreeObserver.IOnGlobalLayoutListener, View.IOnClickListener, GoogleApiClient.IConnectionCallbacks, IOnMapReadyCallback
    {
        private View storeView;
        private TextView storeDescription;
        private TextView storeAddress;
        private ImageView storeDirections;
        private FloatingActionButton currentLocationFab;

        private GoogleMap map;
        private Dictionary<string, string> markerIdDictionary = new Dictionary<string, string>();
        private string selectedMarker = "";
        private GoogleApiClient googleApiClient;
        private Location myLocation;
        private List<LSRetail.Omni.Domain.DataModel.Base.Setup.Store> stores;
        private bool storeViewIsShown = false;

        public enum MapAction
        {
            None = 0,
            Direction = 1,
            StoreInfo = 2
        }

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.StoreMapScreen);

            var toolbar = FindViewById<Toolbar>(Resource.Id.StoreMapScreenToolbar);
            SetSupportActionBar(toolbar);

            storeView = FindViewById(Resource.Id.StoreMapScreenStore);
            storeDescription = FindViewById<TextView>(Resource.Id.StoreMapScreenStoreDescription);
            storeAddress = FindViewById<TextView>(Resource.Id.StoreMapScreenStoreAddress);
            storeDirections = FindViewById<ImageView>(Resource.Id.StoreMapScreenStoreDirections);
            currentLocationFab = FindViewById<FloatingActionButton>(Resource.Id.StoreMapScreenCurrentLocation);

            storeView.SetOnClickListener(this);
            storeDirections.SetOnClickListener(this);
            currentLocationFab.SetOnClickListener(this);

            storeDirections.SetColorFilter(Utils.Utils.GetColorFilter(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.accent))));
            
            googleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(OnConnectionFailed)
                .AddApi(LocationServices.API)
                .Build();

            //storeId = Intent.Extras.GetString(BundleConstants.StoreId);
            if (Intent.Extras != null && Intent.Extras.ContainsKey(BundleUtils.StoreIds))
            {
                var storeIds = Intent.Extras.GetStringArray(BundleUtils.StoreIds);
                stores = storeIds.Select(id => AppData.Stores.FirstOrDefault(x => x.Id == id)).ToList();
            }

            InitMap();

            Utils.Utils.ViewUtils.AddOnGlobalLayoutListener(storeView, this);
        }

        public void OnGlobalLayout()
        {
            Utils.Utils.ViewUtils.RemoveOnGlobalLayoutListener(storeView, this);

            var fragment = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
            if (fragment != null)
            {
                var bottom = fragment.View.Bottom;

                storeView.TranslationY = bottom;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            googleApiClient.Connect();
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

        /// <summary>
        ///   Add markers to the map.
        /// </summary>
        private void AddMarkersToMap()
        {
            if (AppData.Stores == null || AppData.Stores.Count == 0)
            {
                var model = new StoreModel(this, this);
                model.GetStores(AddStoreMarkersToMap);
            }
            else
            {
                AddStoreMarkersToMap();
            }
        }

        private void AddStoreMarkersToMap()
        {
            if (stores == null)
                stores = AppData.Stores;

            foreach (LSRetail.Omni.Domain.DataModel.Base.Setup.Store store in stores)
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

            PanToLocation();
        }

        private void PanToUserLocation()
        {
            if (myLocation != null)
            {
                NumberFormatInfo format = new NumberFormatInfo();
                format.NumberGroupSeparator = ",";
                format.NumberDecimalSeparator = ".";

                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(double.Parse(myLocation.Latitude.ToString(CultureInfo.InvariantCulture)), double.Parse(myLocation.Longitude.ToString(CultureInfo.InvariantCulture))), 14));
            }
        }

        private void PanToLocation()
        {
            if(stores == null)
                return;

            if (stores.Count != 1)
            {
                PanToUserLocation();
            }
            else
            {
                var store = stores[0];
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(double.Parse(store.Latitude.ToString(CultureInfo.InvariantCulture)), double.Parse(store.Longitude.ToString(CultureInfo.InvariantCulture))), 14));
            }
        }

        /// <summary>
        ///   All we do here is add a SupportMapFragment to the Activity.
        /// </summary>
        private void InitMap()
        {
            var mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeNormal)
                .InvokeZoomControlsEnabled(false)
                .InvokeCompassEnabled(false);

            var fragTx = SupportFragmentManager.BeginTransaction();
            var mapFragment = SupportMapFragment.NewInstance(mapOptions);
            fragTx.Add(Resource.Id.StoreMapScreenMap, mapFragment, "map");
            fragTx.Commit();
        }

        public void OnMapReady(GoogleMap xmap)
        {
            if (map == null)
                //GetMapAsync raises OnMapReady
                //was getting called 2x
                if (map != null)
                    return;

            map = xmap;

            map.MyLocationEnabled = true;
            PanToLocation();

            //newer API needs to ask and set the permissions
            string[] PermissionsLocation =
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            const int RequestLocationId = 0;
            const string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) == Android.Content.PM.Permission.Denied)
            {
                this.RequestPermissions(PermissionsLocation, RequestLocationId);
            }
            //this.RequestPermissions(PermissionsLocation, RequestLocationId);


            // Enable the my-location layer.
            map.MyLocationEnabled = true;

            AddMarkersToMap();

            // Setup a handler for when the user clicks on a marker.
            map.SetOnMarkerClickListener(this);
            map.SetOnInfoWindowClickListener(this);
            map.SetPadding(0, Resources.GetDimensionPixelSize(Resource.Dimension.abc_action_bar_default_height_material), 0, 0);

        }

        private void ShowStoreInfo(string id)
        {
            var upIntent = new Intent();
            upIntent.SetClass(this, typeof(StoreDetailActivity));
            upIntent.AddFlags(ActivityFlags.ClearTop);
            upIntent.AddFlags(ActivityFlags.SingleTop);

            upIntent.PutExtra(BundleUtils.Id, id);

            StartActivity(upIntent);

            Finish();
        }

        private void ShowDirections(string storeId)
        {
            MapUtils.ShowDirections(this, storeId);

            Finish();
        }

        #region MENU

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);

            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.StoreMapMenu, menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewNormalMap:
                    if (map != null)
                    {
                        map.MapType = GoogleMap.MapTypeNormal;
                    }
                    item.SetChecked(true);
                    return true;

                case Resource.Id.MenuViewSatelliteMap:
                    if (map != null)
                    {
                        map.MapType = GoogleMap.MapTypeSatellite;
                    }
                    item.SetChecked(true);
                    return true;

                case Resource.Id.MenuViewHybridMap:
                    if (map != null)
                    {
                        map.MapType = GoogleMap.MapTypeHybrid;
                    }
                    item.SetChecked(true);
                    return true;

                case Android.Resource.Id.Home:
                    if (stores.Count == 1)
                    {
                        OnBackPressed();
                    }
                    else
                    {
                        var upIntent = new Intent();
                        upIntent.SetClass(this, typeof(HomeActivity));
                        upIntent.AddFlags(ActivityFlags.ClearTop);
                        upIntent.AddFlags(ActivityFlags.SingleTop);
                        upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Locations);

                        StartActivity(upIntent);

                        Finish();
                    }
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.StoreMapScreenStore:
                    ShowStoreInfo(markerIdDictionary[selectedMarker]);
                    break;

                case Resource.Id.StoreMapScreenStoreDirections:
                    ShowDirections(markerIdDictionary[selectedMarker]);
                    break;

                case Resource.Id.StoreMapScreenCurrentLocation:
                    PanToUserLocation();
                    break;
            }
        }

        public bool OnMarkerClick(Marker marker)
        {
            //marker.ShowInfoWindow();

            selectedMarker = marker.Id;

            SupportInvalidateOptionsMenu();

            var storeId = markerIdDictionary[marker.Id];
            var store = stores.FirstOrDefault(x => x.Id == storeId);

            if (store == null)
                return true;

            storeDescription.Text = store.Description;
            storeAddress.Text = store.Address.Address1;

            if (!storeViewIsShown)
            {
                var fragment = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
                if (fragment != null)
                {
                    var bottom = fragment.View.Bottom;

                    storeView.SetY(bottom);

                    storeView.Visibility = ViewStates.Visible;

                    var animator = ObjectAnimator.OfFloat(storeView, "translationY", bottom - storeView.Height);
                    animator.Start();

                    var fabAnimator = ObjectAnimator.OfFloat(currentLocationFab, "translationY", -storeView.Height);
                    fabAnimator.Start();

                    storeViewIsShown = true;
                }
            }

            return true;
        }

        public void OnInfoWindowClick(Marker marker)
        {
            if (markerIdDictionary.ContainsKey(selectedMarker))
            {
                ShowStoreInfo(markerIdDictionary[selectedMarker]);
            }
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
                    LogUtils.Log(e.Message);
                    LogUtils.Log(e.StackTrace);
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

            if (map == null)
            {
                var fragment = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
                if (fragment != null)
                {
                    fragment.GetMapAsync(this); //raisees onmapready
                }
            }
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        public override void ShowIndicator(bool show)
        {
            if (show)
            {
                SetSupportProgressBarIndeterminate(true);
                SetSupportProgressBarIndeterminateVisibility(true);
            }
            else
            {
                SetSupportProgressBarIndeterminateVisibility(false);
                SetSupportProgressBarIndeterminate(false);
            }
        }
    }
}