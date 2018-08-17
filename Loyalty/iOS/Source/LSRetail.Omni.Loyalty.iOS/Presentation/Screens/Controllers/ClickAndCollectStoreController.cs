using System;
using UIKit;
using System.Collections.Generic;
using CoreLocation;
using CoreGraphics;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation
{
    public class ClickAndCollectStoreController : UIViewController
    {
        private ClickAndCollectStoreView rootView;

        private CLLocationManager locationManager;
        public List<Store> Stores;
        private bool HasData { get; set; }

        public ClickAndCollectStoreController()
        {
            this.Title = LocalizationUtilities.LocalizedString("ClickCollect_Stores", "Stores");
            this.Stores = new List<Store>();

            this.HasData = false;
            this.locationManager = new CLLocationManager();

            if (!HasData)
            {
                GetData();
            }

            this.rootView = new ClickAndCollectStoreView();
            this.rootView.GetData += GetData;
            this.rootView.StoreInfoButtonPressed += StoreInfoButtonPressed;
            this.rootView.StoreSelected += StoreSelected;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
            SetRightBarButtonItems();
            this.rootView.UpdateData(new List<Store>());
            this.View = this.rootView;
        }

        public void GetData()
        {
            System.Diagnostics.Debug.WriteLine("LocationsScreen.GetData running");
            Utils.UI.ShowLoadingIndicator();

            if (Utils.Util.GetOSVersion().Major >= 8)
            {
                locationManager.RequestWhenInUseAuthorization();
            }

            locationManager.StartUpdatingLocation();

            locationManager.LocationsUpdated += async (sender, e) =>
            {
                // We can access user's location
                locationManager.StopUpdatingLocation();

                if (locationManager.Location != null)
                {
                    CLLocationCoordinate2D coord = locationManager.Location.Coordinate;

                    if (!HasData)
                    {
                        HasData = true;
                        List<Store> stores = await new Models.StoreModel().GetStoresByCoordinates(coord.Latitude, coord.Longitude, 100000, 100);
                        if (stores != null)
                        {
                            GetStoresSuccess(stores);
                        }
                        else
                        {
                            GetStoresFailure();
                        }
                    }
                }
            };

            locationManager.Failed += async (sender, e) =>
            {
                // We can't access user's locations - he probably didn't allow it
                locationManager.StopUpdatingLocation();

                if (!HasData)
                {
                    HasData = true;
                    List<Store> storeList = await new Models.StoreModel().GetAllStores();
                    if (storeList != null)
                    {
                        GetStoresSuccess(storeList);
                    }
                    else
                    {
                        GetStoresFailure();
                    }
                }
            };
        }

        private void GetStoresSuccess(List<Store> stores)
        {
            System.Diagnostics.Debug.WriteLine("LocationsScreen.GetData success");

            this.Stores = stores.FindAll(x => x.IsClickAndCollect == true);

            Utils.UI.HideLoadingIndicator();
            this.rootView.HideErrorGettingDataView();
            this.rootView.UpdateData(Stores);
        }

        private void GetStoresFailure()
        {
            System.Diagnostics.Debug.WriteLine("LocationsScreen.GetData failure");

            HasData = false;
            Utils.UI.HideLoadingIndicator();
            this.rootView.ShowErrorGettingDataView();
        }

        private void SetRightBarButtonItems()
        {
            UIButton mapButton = new UIButton();
            mapButton.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("MapIcon"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
            mapButton.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            mapButton.Frame = new CGRect(0, 0, 30, 30);
            mapButton.TouchUpInside += (sender, e) =>
            {
                MapController map = new MapController(this.Stores, true);
                this.NavigationController.PushViewController(map, true);
            };

            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(mapButton);
        }

        public void StoreSelected(Store store)
        {
            Console.WriteLine(store.Description + " selected");
            Utils.UI.ShowLoadingIndicator();

            new Models.ClickCollectModel().CheckAvailability(store.Id,
                async (newBasket, unavailableItems) =>
                {
                    //success
                    if (newBasket != null)
                    {
                        // not all items asked for are available

                        if (newBasket.Items.Count == 0)
                        {
                            Utils.UI.HideLoadingIndicator();

                            await AlertView.ShowAlert(
                                this,
                                LocalizationUtilities.LocalizedString("ClickCollect_NotAvailableTitle", "Not available in this store"),
                                LocalizationUtilities.LocalizedString("ClickCollect_NotAvailableMsg", "Item/s not available, please try another store"),
                                LocalizationUtilities.LocalizedString("General_OK", "OK")
                            );
                        }
                        else
                        {
                            // calculate the new basket
                            OneList calculatedBasket = await new Models.BasketModel().CalculateBasket(newBasket);
                            if (calculatedBasket != null)
                            {
                                //success
                                Utils.UI.HideLoadingIndicator();

                                ConfirmOrderController confirmOrderController = new ConfirmOrderController(store, calculatedBasket, unavailableItems, false);
                                this.NavigationController.PushViewController(confirmOrderController, true);
                            }
                            else
                            {
                                //failure
                                Utils.UI.HideLoadingIndicator();
                            }
                        }
                    }
                    else
                    {
                        Utils.UI.HideLoadingIndicator();

                        // all items are available - the basket doesn't change
                        ConfirmOrderController confirmOrderController = new ConfirmOrderController(store, AppData.Device.UserLoggedOnToDevice.Basket, null, true);
                        this.NavigationController.PushViewController(confirmOrderController, true);
                    }
                },
                () =>
                {
                    //failure
                    Utils.UI.HideLoadingIndicator();
                }
            );
        }

        public void StoreInfoButtonPressed(Store store)
        {
            LocationDetailController locationDetailsScreen = new LocationDetailController(store, this.Stores, true);
            this.NavigationController.PushViewController(locationDetailsScreen, true);
        }
    }
}
