using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace Presentation
{
    public class ConfirmOrderController : UIViewController
    {
        private ConfirmOrderView rootView;

        private Store Store;
        private OneList Basket;
        private List<OneListItem> unavailableItems;
        private bool isFinalizedBasket;

        public ConfirmOrderController(Store store, OneList basket, List<OneListItem> unavailableItems, bool isFinalizedBasket)
        {
            this.Title = LocalizationUtilities.LocalizedString("Checkout_ConfirmOrder", "Confirm order");

            this.Store = store;
            this.Basket = basket;
            this.unavailableItems = unavailableItems;
            this.isFinalizedBasket = isFinalizedBasket;

            this.rootView = new ConfirmOrderView();
            this.rootView.PlaceOrderPressed += PlaceOrderPressed;
            this.rootView.ProceedToPlaceOrder += ProceedToPlaceOrder;
            this.rootView.DoneButtonPressed += DonePressed;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

            UIBarButtonItem proceedBarButton;

            // Bar buttons
            if (!this.isFinalizedBasket)
            {
                proceedBarButton = new UIBarButtonItem();
                proceedBarButton.Title = LocalizationUtilities.LocalizedString("General_Proceed", "Proceed");
                proceedBarButton.Clicked += (object sender, EventArgs e) =>
                {
                    ProceedToPlaceOrder();
                };
                this.NavigationItem.RightBarButtonItem = proceedBarButton;
            }

            this.View = this.rootView;
            this.rootView.UpdateData(this.Store, this.Basket, this.unavailableItems, this.isFinalizedBasket);

        }

        public void ProceedToPlaceOrder()
        {
            //Refresh the header view and hide the proceed button
            this.rootView.ReloadData(true);
            this.NavigationItem.RightBarButtonItem = null;
        }

        private void DonePressed()
        {
            Utils.UI.ShowLoadingIndicator();

            new BasketModel().ClearBasket(
                () =>
                {
                    // Success
                    this.Basket.Clear();
                    Utils.UI.HideLoadingIndicator();

                    this.DismissViewController(true, null);
                },
                async () =>

                {
                    // Failure
                    Utils.UI.HideLoadingIndicator();

                    await AlertView.ShowAlert(
                        this,
                        LocalizationUtilities.LocalizedString("General_Error", "Error"),
                        LocalizationUtilities.LocalizedString("Checkout_ClearBasketErrorTryAgain", "Could not clear basket, please try again."),
                        LocalizationUtilities.LocalizedString("General_OK", "OK")
                    );
                }
            );
        }

        private async void PlaceOrderPressed(string email)
        {
            try
            {
                Utils.UI.ShowLoadingIndicator();

                Order order = await new Models.ClickCollectModel().CreateOrder(this.Store.Id, email, this.Basket);
                if (order != null)
                {
                    //success
                    Utils.UI.HideLoadingIndicator();
                    this.rootView.OrderCreated();
                    this.NavigationItem.HidesBackButton = true;
                }
                else
                {
                    Utils.UI.HideLoadingIndicator();
                    //failure
                }
            }

            catch (Exception ex)
            {
                string exeption = ex.Message;
            }

        }
    }
}
