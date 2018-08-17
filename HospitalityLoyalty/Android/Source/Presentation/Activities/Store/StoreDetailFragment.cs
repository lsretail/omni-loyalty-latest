using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using Presentation.Activities.Base;
using Presentation.Activities.Image;
using Presentation.Dialog;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using Xamarin.ViewPagerIndicator;
using Math = System.Math;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Store
{
    public class StoreDetailFragment : BaseFragment, View.IOnClickListener
    {
        private LSRetail.Omni.Domain.DataModel.Base.Setup.Store store;

        private ViewPager imagePager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.StoreDetailScreen, null);

            if (AppData.Stores == null)
            {
                Activity.Finish();
                return view;
            }

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.StoreDetailScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            imagePager = view.FindViewById<ViewPager>(Resource.Id.HeaderImageViewPager);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                imagePager.SetPageTransformer(true, new ZoomOutPageTransformer());
            }

            var storeId = Arguments.GetString(BundleUtils.Id);

            store = AppData.Stores.FirstOrDefault(x => x.Id == storeId);

            var contentView = view.FindViewById(Resource.Id.StoreDetailScreenContent);
            contentView.SetMinimumHeight(Utils.Utils.ViewUtils.GetContentViewHeight(Activity));

            var storeName = view.FindViewById<TextView>(Resource.Id.StoreDetailScreenStoreName);
            var storeAddress = view.FindViewById<TextView>(Resource.Id.StoreDetailScreenStoreAddress);
            var storePhone = view.FindViewById<TextView>(Resource.Id.StoreDetailScreenStorePhone);

            var openingHoursContainer = view.FindViewById<LinearLayout>(Resource.Id.StoreDetailScreenOpeningHours);
            var storeServices = view.FindViewById<LinearLayout>(Resource.Id.StoreDetailScreenServices);
            var storeServicesContainer = view.FindViewById(Resource.Id.StoreDetailScreenServicesContainer);

            storeName.Text = store.Description;
            storeAddress.Text = store.Address.FormatAddress;
            storePhone.Text = store.Phone;

            var addressContainer = view.FindViewById(Resource.Id.StoreDetailScreenLocationContainer);
            var phoneContainer = view.FindViewById(Resource.Id.StoreDetailScreenPhoneContainer);

            addressContainer.SetOnClickListener(this);
            phoneContainer.SetOnClickListener(this);

            var directionsFab = view.FindViewById<FloatingActionButton>(Resource.Id.StoreDetailScreenDirections);
            directionsFab.SetOnClickListener(this);

            var groups = store.StoreHours.Select(x => x.StoreHourtype).Distinct().ToList();

            foreach (var storeHourType in groups)
            {
                var hours = store.StoreHours.Where(s => s.StoreHourtype == storeHourType).ToList();

                var openingHours = Inflate(inflater, Resource.Layout.StoreOpeningHours, null);
                var description = openingHours.FindViewById<TextView>(Resource.Id.StoreOpeningHoursDescripion);
                var openingHourDescription = openingHours.FindViewById<TextView>(Resource.Id.StoreOpeningHours);

                description.Text = GetStoreOpeningHourDescription(storeHourType);

                List<StoreHours> sameHours = new List<StoreHours>();

                while (hours.Count > 0)
                {
                    if (sameHours.Count == 0)
                    {
                        sameHours.Add(hours[0]);
                    }
                    else
                    {
                        if (sameHours[0].OpenFrom != hours[0].OpenFrom || sameHours[0].OpenTo != hours[0].OpenTo)
                        {
                            openingHourDescription.Text += FormatHours(sameHours) + System.Environment.NewLine;
                            sameHours.Clear();
                        }

                        sameHours.Add(hours[0]);
                    }

                    hours.RemoveAt(0);
                }

                openingHourDescription.Text += FormatHours(sameHours);

                openingHoursContainer.AddView(openingHours);
            }

            if (store.StoreServices.Count == 0)
            {
                storeServicesContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                store.StoreServices.ForEach(service =>
                {
                    var serviceImageView =
                        Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.StoreService, storeServices) as ImageView;

                    switch (service.StoreServiceType)
                    {
                        case StoreServiceType.DriveThruWindow:
                            serviceImageView.SetImageResource(Resource.Drawable.store_service_drive_through);
                            break;

                        case StoreServiceType.FreeRefill:
                            serviceImageView.SetImageResource(Resource.Drawable.store_service_free_refill);
                            break;

                        case StoreServiceType.FreeWiFi:
                            serviceImageView.SetImageResource(Resource.Drawable.store_service_free_wifi);
                            break;

                        case StoreServiceType.GiftCard:
                            serviceImageView.SetImageResource(Resource.Drawable.store_service_gift_card);
                            break;

                        case StoreServiceType.PlayPlace:
                            serviceImageView.SetImageResource(Resource.Drawable.store_service_play_place);
                            break;

                        case StoreServiceType.Garden:
                            serviceImageView.SetImageResource(Resource.Drawable.store_service_drive_garden);
                            break;
                    }

                    serviceImageView.Tag = (int)service.StoreServiceType;
                    serviceImageView.SetOnClickListener(this);

                    storeServices.AddView(serviceImageView);
                });
            }

            var animationImageId = string.Empty;
            if (Arguments.ContainsKey(BundleUtils.AnimationImageId))
            {
                animationImageId = Arguments.GetString(BundleUtils.AnimationImageId);
            }

            imagePager.Adapter = new ImagePagerAdapter(ChildFragmentManager, store.Images, Resources.DisplayMetrics.WidthPixels, Resources.GetDimensionPixelSize(Resource.Dimension.HeaderImageHeight), animationImageId);

            var indicator = view.FindViewById<LinePageIndicator>(Resource.Id.HeaderImageIndicator);
            if (store.Images != null && store.Images.Count > 1)
            {
                indicator.SetViewPager(imagePager);
            }
            else
            {
                indicator.Visibility = ViewStates.Gone;
            }

            return view;
        }

        private string FormatHours(List<StoreHours> sameHours)
        {
            if (sameHours.Count == 1)
            {
                return string.Format(Resources.GetString(Resource.String.StoreOpeningOneDay), GetDayDescription(sameHours[0].DayOfWeek), sameHours[0].OpenFrom.ToString("t"), sameHours[0].OpenTo.ToString("t"));
            }
            else if (sameHours.Count == 2)
            {
                return string.Format(Resources.GetString(Resource.String.StoreOpeningTwoDays), GetDayDescription(sameHours[0].DayOfWeek), GetDayDescription(sameHours[1].DayOfWeek), sameHours[0].OpenFrom.ToString("t"), sameHours[0].OpenTo.ToString("t"));
            }
            else
            {
                return string.Format(Resources.GetString(Resource.String.StoreOpeningManyDays), GetDayDescription(sameHours[0].DayOfWeek), GetDayDescription(sameHours[sameHours.Count - 1].DayOfWeek), sameHours[0].OpenFrom.ToString("t"), sameHours[0].OpenTo.ToString("t"));
            }
        }

        private string GetDayDescription(int day)
        {
            switch (day)
            {
                case 0:
                    return Resources.GetString(Resource.String.StoreSunday);

                case 1:
                    return Resources.GetString(Resource.String.StoreMonday);

                case 2:
                    return Resources.GetString(Resource.String.StoreTuesday);

                case 3:
                    return Resources.GetString(Resource.String.StoreWednesday);

                case 4:
                    return Resources.GetString(Resource.String.StoreThursday);

                case 5:
                    return Resources.GetString(Resource.String.StoreFriday);

                case 6:
                    return Resources.GetString(Resource.String.StoreSaturday);
            }

            return string.Empty;
        }

        private string GetServiceText(StoreServiceType tag)
        {
            switch (tag)
            {
                case StoreServiceType.DriveThruWindow:
                    return Resources.GetString(Resource.String.StoreServiceDriveThrough);

                case StoreServiceType.FreeRefill:
                    return Resources.GetString(Resource.String.StoreServiceFreeRefill);

                case StoreServiceType.FreeWiFi:
                    return Resources.GetString(Resource.String.StoreServiceFreeWifi);

                case StoreServiceType.GiftCard:
                    return Resources.GetString(Resource.String.StoreServiceGiftCard);

                case StoreServiceType.PlayPlace:
                    return Resources.GetString(Resource.String.StoreServicePlayPlace);

                case StoreServiceType.Garden:
                    return Resources.GetString(Resource.String.StoreServiceGarden);
            }

            return string.Empty;
        }

        private string GetStoreOpeningHourDescription(StoreHourType hourType)
        {
            switch (hourType)
            {
                case StoreHourType.MainStore:
                    return Resources.GetString(Resource.String.StoreMainOpeningHours);

                case StoreHourType.DriveThruWindow:
                    return Resources.GetString(Resource.String.StoreDriveThrough);
            }

            return string.Empty;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.StoreServiceIcon:
                    CheatSheet.ShowCheatSheet(v, GetServiceText((StoreServiceType)(int)v.Tag));
                    break;

                case Resource.Id.StoreDetailScreenDirections:
                    MapUtils.ShowDirections(Activity, store.Id);
                    break;

                case Resource.Id.StoreDetailScreenPhoneContainer:
                    Intent callIntent = new Intent(Intent.ActionDial);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + store.Phone));
                    Activity.StartActivity(callIntent);
                    break;

                case Resource.Id.StoreDetailScreenLocationContainer:
                    if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                    {
                        OpenMap();
                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation }, 0);
                    }
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            foreach (var grantResult in grantResults)
            {
                if (grantResult != Permission.Granted)
                {
                    ShowPermissionError();
                    return;
                }
            }

            OpenMap();
        }

        private void ShowPermissionError()
        {
            var dialog = new WarningDialog(Activity, "");
            dialog.Message = Resources.GetString(Resource.String.StoreAppNeedsLocationPermission);
            dialog.SetPositiveButton(Resources.GetString(Android.Resource.String.Ok), () => { });
            dialog.SetNegativeButton(Resources.GetString(Resource.String.OpenSettings), () =>
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                var uri = Android.Net.Uri.FromParts("package", Activity.PackageName, null);
                intent.SetData(uri);
                StartActivityForResult(intent, 1);
            });
            dialog.Show();
        }

        private void OpenMap()
        {
            var intent = new Intent(Activity, typeof(StoreMapActivity));
            intent.PutExtra(BundleUtils.StoreIds, new[] { store.Id });
            StartActivity(intent);
        }
    }
}