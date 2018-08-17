using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using CoreGraphics;
using System.Linq;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class LocationDetailController : UIViewController
    {
        private LocationDetailView rootView;
        private List<Store> stores;
        public Store store;
        private bool clickAndCollect;


        public LocationDetailController(Store store, List<Store> stores, bool clickAncCollect = false)
        {
            this.store = store;
            this.stores = stores;
            this.rootView = new LocationDetailView();
            this.rootView.ImageSelected += ViewImages;
            this.View = this.rootView;
            this.clickAndCollect = clickAncCollect;
            LoadDataForView();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
            this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;

            SetRightBarButtonItems();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        private void LoadDataForView()
        {
            string storeHourTypeAndDaysString = string.Empty;
            string openingHoursString = string.Empty;
            bool newLineRequired = false;

            foreach (var storeHour in this.store.StoreHours)
            {
                storeHourTypeAndDaysString += (newLineRequired ? "\r\n" : string.Empty) + GetStoreHoursDayName(storeHour) + ":";
                openingHoursString += (newLineRequired ? "\r\n" : string.Empty) + storeHour.OpenFrom.ToShortTimeString() + " - " + storeHour.OpenTo.ToShortTimeString();
                newLineRequired = true;
            }

            this.rootView.UpdateView(this.store, storeHourTypeAndDaysString, openingHoursString);
        }

        private string GetStoreHoursDayName(StoreHours storeHours)
        {
            switch ((int)storeHours.DayOfWeek)
            {
                case 0:
                    return LocalizationUtilities.LocalizedString("Weekday_Sunday", "Sunday");
                case 1:
                    return LocalizationUtilities.LocalizedString("Weekday_Monday", "Monday");
                case 2:
                    return LocalizationUtilities.LocalizedString("Weekday_Tuesday", "Tuesday");
                case 3:
                    return LocalizationUtilities.LocalizedString("Weekday_Wednesday", "Wednesday");
                case 4:
                    return LocalizationUtilities.LocalizedString("Weekday_Thursday", "Thursday");
                case 5:
                    return LocalizationUtilities.LocalizedString("Weekday_Friday", "Friday");
                case 6:
                    return LocalizationUtilities.LocalizedString("Weekday_Saturday", "Saturday");
                default:
                    return string.Empty;
            }
        }

        private void CollectFromThisStore()
        {
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
                                LocalizationUtilities.LocalizedString("ClickCollect_NotAvailableMsg", "Item/s not available, please try another store"),
                                string.Empty,
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

                                ConfirmOrderController confirmOrderScreen = new ConfirmOrderController(store, calculatedBasket, unavailableItems, false);
                                this.NavigationController.PushViewController(confirmOrderScreen, true);
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
                        ConfirmOrderController confirmOrderScreen = new ConfirmOrderController(store, AppData.Device.UserLoggedOnToDevice.Basket, null, true);
                        this.NavigationController.PushViewController(confirmOrderScreen, true);
                    }
                },
                () =>
                {
                    //failure
                    Utils.UI.HideLoadingIndicator();
                }
            );
        }

        public void SetRightBarButtonItems()
        {
            List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();
            nfloat rightBarButtonDimension = 30f;

            UIButton btnMap = new UIButton(UIButtonType.Custom);
            btnMap.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("pinDrop"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
            btnMap.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            btnMap.Frame = new CGRect(0, 0, rightBarButtonDimension, rightBarButtonDimension);
            btnMap.TouchUpInside += (sender, e) =>
            {
                MapController map = new MapController(this.store, this.stores, this.clickAndCollect);
                this.NavigationController.PushViewController(map, true);
            };

            UIButton btnDirections = new UIButton(UIButtonType.Custom);
            btnDirections.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("Directions"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
            btnDirections.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            btnDirections.Frame = new CGRect(0, 0, rightBarButtonDimension, rightBarButtonDimension);
            btnDirections.TouchUpInside += (sender, e) =>
            {
                LocationDirectionsController directionsController = new LocationDirectionsController(this.store);
                this.NavigationController.PushViewController(directionsController, true);
            };

            if (this.clickAndCollect)
            {
                UIButton btnCollect = new UIButton(UIButtonType.Custom);
                btnCollect.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("StoreIcon1.png"), Utils.AppColors.PrimaryColor), UIControlState.Normal);
                btnCollect.Frame = new CGRect(0, 0, rightBarButtonDimension, rightBarButtonDimension);
                btnCollect.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
                btnCollect.TouchUpInside += (sender, e) =>
                {
                    CollectFromThisStore();
                };
                barButtonItemList.Add(new UIBarButtonItem(btnCollect));
            }

            barButtonItemList.Add(new UIBarButtonItem(btnMap));
            barButtonItemList.Add(new UIBarButtonItem(btnDirections));

            this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
        }

        private void ViewImages(List<ImageView> imageViews, nint selectedImageViewIndex)
        {
            if (selectedImageViewIndex > imageViews.Count() - 1)
                return;

            ImageView imageViewToShow = imageViews[(int)selectedImageViewIndex];

            if (imageViewToShow != null)
            {
                var imageZoomController = new ImageZoomController(imageViewToShow);
                this.NavigationController.PushViewController(imageZoomController, true);
            }
        }
    }
}
