using System;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class CheckoutController : UIViewController
    {
        private CheckoutView rootView;

        public CheckoutController()
        {
            Title = LocalizationUtilities.LocalizedString("Checkout_Checkout", "Checkout");
            rootView = new CheckoutView();
            rootView.ProceedToShippingMethods += ProceedToShippingMethod;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

            UIBarButtonItem cancelBarButton = new UIBarButtonItem();
            cancelBarButton.Title = LocalizationUtilities.LocalizedString("General_Cancel", "Cancel");
            cancelBarButton.Clicked += (object sender, EventArgs e) =>
            {
                this.DismissViewController(true, () => { });
            };

            UIBarButtonItem proceedBarButton = new UIBarButtonItem();

            proceedBarButton = new UIBarButtonItem();
            proceedBarButton.Title = LocalizationUtilities.LocalizedString("General_Proceed", "Proceed");
            proceedBarButton.Clicked += (object sender, EventArgs e) =>
            {
                ProceedToShippingMethod();
            };

            this.NavigationItem.LeftBarButtonItem = cancelBarButton;
            this.NavigationItem.RightBarButtonItem = proceedBarButton;
            this.View = this.rootView;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            this.rootView.BottomLayoutGuideLength = BottomLayoutGuide.Length;
            this.rootView.TopLayoutGuideLength = TopLayoutGuide.Length;
        }

        public void ProceedToShippingMethod()
        {
            ShippingMethodController shippingMethodController = new ShippingMethodController();
            this.NavigationController.PushViewController(shippingMethodController, true);
        }
    }
}

