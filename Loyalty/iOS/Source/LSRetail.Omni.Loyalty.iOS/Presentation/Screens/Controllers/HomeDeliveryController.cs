using System;
using UIKit;
using Foundation;
using Presentation.Utils;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace Presentation
{
	public class HomeDeliveryController : UIViewController
	{
		private HomeDeliveryView rootView;

		public HomeDeliveryController ()
		{
			this.Title = LocalizationUtilities.LocalizedString("Checkout_HomeDelivery", "Home Delivery");
			this.rootView = new HomeDeliveryView();
			this.rootView.DoneButtonPressed = DonePressed;
			this.rootView.SendOrder = SendOrder;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View = this.rootView;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		private async void SendOrder (Action onSuccess) 
		{
			Utils.UI.ShowLoadingIndicator ();

           Order order = await  new BasketModel().SendOrder();
            if (order != null)
			    {
					//success
					Utils.UI.HideLoadingIndicator();

					AppData.ShouldRefreshPublishedOffers = true;
					AppData.ShouldRefreshPoints = true;

					onSuccess ();

					this.NavigationItem.HidesBackButton = true;
				}
            else
				{
					Utils.UI.HideLoadingIndicator();
				}
			
		}

		private void DonePressed()
		{
			Utils.UI.ShowLoadingIndicator();

            new BasketModel().ClearBasket(
                 () =>
                 {
                     // Success
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

		/*private bool ValidateData()
		{
			if (View != null)
			{
				//if (View.FindViewById<RadioButton>(Resource.Id.CheckoutViewHomeDelivery).Checked)
				{
					if (string.IsNullOrEmpty(shippingAddressName.Text) || string.IsNullOrEmpty(shippingAddressOne.Text) || string.IsNullOrEmpty(shippingCity.Text) || string.IsNullOrEmpty(shippingState.Text) || string.IsNullOrEmpty(shippingPostCode.Text) || string.IsNullOrEmpty(shippingCountry.Text) || string.IsNullOrEmpty(shippingAddressName.Text))
					{
						//Toast.MakeText(Activity, Resource.String.CheckoutViewAllRequiredFieldsMustBeFilled, ToastLength.Short).Show();
						return false;
					}
				}

				if (View.FindViewById<RadioButton>(Resource.Id.CheckoutViewPayCreditCard).Checked)
				{
					if (string.IsNullOrEmpty(paymentCardNumber.Text) || string.IsNullOrEmpty(paymentMM.Text) || string.IsNullOrEmpty(paymentYYYY.Text) || string.IsNullOrEmpty(paymentCVV.Text))
					{
						//Toast.MakeText(Activity, Resource.String.CheckoutViewMustEnterCreditCardInfo, ToastLength.Short).Show();
						return false;
					}

					if (!useShippingAddress.Checked)
					{
						if (string.IsNullOrEmpty(billingShippingAddressName.Text) || string.IsNullOrEmpty(billingShippingAddressOne.Text) || string.IsNullOrEmpty(billingShippingCity.Text) || string.IsNullOrEmpty(billingShippingPostCode.Text) || string.IsNullOrEmpty(billingShippingCountry.Text))
						{
							Toast.MakeText(Activity, Resource.String.CheckoutViewAllRequiredFieldsMustBeFilled, ToastLength.Short).Show();
							return false;
						}
					}
				}
			}

			return true;
		}*/
	}
}

