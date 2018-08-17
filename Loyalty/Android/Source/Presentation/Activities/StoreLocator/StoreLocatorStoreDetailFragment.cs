using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.Image;
using Presentation.Models;
using Presentation.Util;
using ImageView = Android.Widget.ImageView;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Utils = Presentation.Util.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using System.Globalization;

namespace Presentation.Activities.StoreLocator
{
    public class StoreLocatorStoreDetailFragment : LoyaltyFragment, IRefreshableActivity, View.IOnClickListener, GoogleApiClient.IConnectionCallbacks
//, IConnectionListener, Util.ILocationListener
    {
        private string storeId;
        private StoreModel storeModel;
        private Store store;
        
        private View progressIndicator;
        private View content;

        private Location lastKnownLocation;
        private Android.Support.Design.Widget.FloatingActionButton showDirectionsButton;
        private TextView storeName;
        private TextView storeAddressLineOne;
        private TextView storeAddressPhone;
        private TextView opening;
        private View addressContainer;
        private View phoneContainer;
        private TextView storeAddressHeader;
        private GoogleApiClient googleApiClient;

        public static StoreLocatorStoreDetailFragment NewInstance()
        {
            var storeDetail = new StoreLocatorStoreDetailFragment() {Arguments = new Bundle()};
            return storeDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            HasOptionsMenu = true;

            //locationManager = LocationManager.NewInstance(Activity, this, this);

            Bundle data = Arguments;
            storeId = data.GetString(BundleConstants.StoreId);

            storeModel = new StoreModel(Activity, this);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.StoreLocatorStoreDetail);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.StoreLocatorStoreDetailScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);
            
            storeName = view.FindViewById<TextView>(Resource.Id.StoreLocatorStoreDetailStoreName);
            storeAddressHeader = view.FindViewById<TextView>(Resource.Id.StoreLocatorStoreDetailStoreAddressHeader);
            storeAddressLineOne = view.FindViewById<TextView>(Resource.Id.StoreLocatorStoreDetailStoreAddress);
            storeAddressPhone = view.FindViewById<TextView>(Resource.Id.StoreLocatorStoreDetailStorePhone);
            opening = view.FindViewById<TextView>(Resource.Id.StoreLocatorStoreDetailOpeningHours);
            addressContainer = view.FindViewById(Resource.Id.StoreLocatorStoreDetailLocationContainer);
            phoneContainer = view.FindViewById(Resource.Id.StoreLocatorStoreDetailPhoneContainer);

            progressIndicator = view.FindViewById<View>(Resource.Id.StoreLocatorStoreDetailViewLoadingSpinner);
            showDirectionsButton = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.StoreLocatorStoreDetailShowDirections);

            content = view.FindViewById(Resource.Id.StoreLocatorStoreDetailScreenContent);
            
            if (AppData.Stores != null && AppData.Stores.Count > 0)
            {
                LoadStore(view);
            }
            else
            {
                LoadStoresFromServer();
            }

            googleApiClient = new GoogleApiClient.Builder(Activity)
                .AddConnectionCallbacks(this)
                .AddApi(LocationServices.API)
                .Build();

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            googleApiClient.Connect();

            //locationManager.Connect();
        }

        public override void OnPause()
        {
            //locationManager.Disconnect();

            googleApiClient.Disconnect();
            base.OnPause();
        }

        private async void LoadStoresFromServer()
        {
            await storeModel.GetAllStores();
            
            LoadStore();
        }

        private void LoadStore(View view = null)
        {
            if(view == null)
                view = View;

            store = AppData.Stores.FirstOrDefault(x => x.Id == storeId);

            if(store == null)
            {
                return;
            }

            Activity.SupportInvalidateOptionsMenu();

            storeName.Text = store.Description;

            storeAddressLineOne.Text = store.Address.FormatAddress;

            storeAddressPhone.Text = string.Format(GetString(Resource.String.StorelocatorDetailViewPhone), store.Phone);

            foreach (var storeHour in store.StoreHours)
            {
                opening.Text += string.Format(GetString(Resource.String.StorelocatorDetailViewOpeningHourDay), storeHour.OpenFrom.ToString("t"), storeHour.OpenTo.ToString("t"), GetStoreHourDayName(storeHour.DayOfWeek)) + System.Environment.NewLine;
            }

            opening.Text = opening.Text.TrimEnd(System.Environment.NewLine.ToCharArray());

            addressContainer.SetOnClickListener(this);
            phoneContainer.SetOnClickListener(this);
            showDirectionsButton.SetOnClickListener(this);

            new DetailImagePager(view, ChildFragmentManager, store.Images);
        }

        private string GetStoreHourDayName(int dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case 0:   //Sunday
                    return GetString(Resource.String.StorelocatorDetailViewSundays);
                case 1:   //Monday
                    return GetString(Resource.String.StorelocatorDetailViewMonday);
                case 2:   //Tuesday
                    return GetString(Resource.String.StorelocatorDetailViewTuesday);
                case 3:   //Wednesday
                    return GetString(Resource.String.StorelocatorDetailViewWednesday);
                case 4:   //Thursday
                    return GetString(Resource.String.StorelocatorDetailViewThursday);
                case 5:   //Friday
                    return GetString(Resource.String.StorelocatorDetailViewFriday);
                case 6:   //Saturday
                    return GetString(Resource.String.StorelocatorDetailViewSaturday);
                default:
                    return dayOfWeek.ToString();

            }
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                content.Visibility = ViewStates.Gone;
                progressIndicator.Visibility = ViewStates.Visible;
            }
            else
            {
                progressIndicator.Visibility = ViewStates.Gone;
                content.Visibility = ViewStates.Visible;
            }
        }

        private void ShowStoreOnMap()
        {
            if (Utils.CheckForPlayServices(Activity))
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(StoreLocatorMapActivity));

                intent.PutExtra(BundleConstants.StoreId, storeId);

                StartActivityForResult(intent, 0);
            }
        }

        private void ShowDirections()
        {
            var intent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?saddr=&daddr=" + store.Latitude.ToString(CultureInfo.InvariantCulture) + "," + store.Longitude.ToString(CultureInfo.InvariantCulture)));
            StartActivity(intent);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 0)
            {
                if (resultCode == (int)Result.Ok)
                {
                    var action = data.GetIntExtra(BundleConstants.Action, (int)StoreLocatorMapActivity.MapAction.None);

                    if ((StoreLocatorMapActivity.MapAction)action != StoreLocatorMapActivity.MapAction.None)
                    {
                        if ((StoreLocatorMapActivity.MapAction)action == StoreLocatorMapActivity.MapAction.Direction)
                        {
                            ShowDirections();
                        }
                    }
                }
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            var location = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);
            SetDistance(location);
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        private void SetDistance(Location location)
        {
            lastKnownLocation = location;
            if (store == null || location == null)
            {
                return;
            }
            
            var distance = new float[3];
            Location.DistanceBetween((double)store.Latitude, (double)store.Longitude, location.Latitude, location.Longitude, distance);

            storeAddressHeader.Text = string.Format(GetString(Resource.String.StorelocatorDetailViewKilometersAway), (distance[0] / 1000).ToString("N1"));

            storeAddressHeader.Visibility = ViewStates.Visible;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.StoreLocatorStoreDetailShowDirections:
                    ShowDirections();
                    break;

                case Resource.Id.StoreLocatorStoreDetailLocationContainer:
                    ShowStoreOnMap();
                    break;

                case Resource.Id.StoreLocatorStoreDetailPhoneContainer:
                    Intent callIntent = new Intent(Intent.ActionDial);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + store.Phone));
                    Activity.StartActivity(callIntent);
                    break;
            }
        }
    }
}