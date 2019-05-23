using System;
using UIKit;
using CoreGraphics;
using Presentation.Models;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation
{
    public class BasketController : UIViewController
    {
        private BasketView rootView;

        public BasketController()
        {
            this.Title = LocalizationUtilities.LocalizedString("Basket_Basket", "Basket");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

            this.rootView = new BasketView();
            this.rootView.RemoveItemFromBasket += RemoveItemFromBasket;
            this.rootView.RefreshBasket += RefreshBasket;
            this.rootView.Checkout += Checkout;
            this.rootView.Update += UpdateItemInBasket;
            this.View = this.rootView;

            // Clear basket bar button
            UIButton btnClearBasket = new UIButton(UIButtonType.Custom);
            btnClearBasket.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("TrashIcon"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
            btnClearBasket.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnClearBasket.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            btnClearBasket.Frame = new CGRect(0, 0, 30, 30);
            btnClearBasket.TouchUpInside += (sender, e) =>
            {
                ClearBasket();
            };
            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(btnClearBasket);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
            this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (AppData.UserLoggedIn)
                this.NavigationItem.RightBarButtonItem.Enabled = true;
            else
                this.NavigationItem.RightBarButtonItem.Enabled = false;

            this.rootView.Refresh(GetFormattedTotalString());
        }

        private string GetFormattedTotalString()
        {
            decimal total = 0;
            string formattedTotal = total.ToString();

            if (AppData.UserLoggedIn)
            {
                if (AppData.Device.UserLoggedOnToDevice.Basket.TotalAmount == 0)
                    AppData.Device.UserLoggedOnToDevice.Basket.CalculateBasket();
                
                total = AppData.Device.UserLoggedOnToDevice.Basket.TotalAmount;
                formattedTotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(total);
            }

            return formattedTotal;
        }

        /*
		private void BasketItemPressed(int itemPosition, BasketItem item)
		{			
			ItemVariantUOMScreen editScreen = new ItemVariantUOMScreen(item);
			editScreen.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
			editScreen.EditingDone += (decimal quantity, string variantId, string uomId) => 
			{
				var basketItemClone = new BasketItem(item.Item, quantity, uomId, variantId);

				Utils.UI.ShowLoadingIndicator();
				new BasketModel().EditItemAtPosition(
					itemPosition,
					basketItemClone,
					() => 
					{	
						// Success
						Utils.UI.HideLoadingIndicator();
						this.rootView.Refresh(GetFormattedTotalString());
					},
					() => 
					{
						// Failure
						Utils.UI.HideLoadingIndicator();
						Utils.UI.ShowAlertView(
							LocalizationUtilities.LocalizedString("General_Error", "Error"),
							LocalizationUtilities.LocalizedString("EditBasketItem_EditItemErrorTryAgain", "Could not edit item, please try again."),
							null,
							null,
							false
						);
					}
				);

			};

			this.PresentViewController(new UINavigationController(editScreen), true, null);
		}
		*/

        private void RemoveItemFromBasket(int itemPosition)
        {
            Utils.UI.ShowLoadingIndicator();
            new Models.BasketModel().RemoveItemFromBasket(
                itemPosition,
                () =>
                {
                    // Success
                    Utils.UI.HideLoadingIndicator();
                    this.rootView.Refresh(GetFormattedTotalString());
                },
                async () =>
                {
                    // Failure
                    Utils.UI.HideLoadingIndicator();
                    await AlertView.ShowAlert(
                        this,
                        LocalizationUtilities.LocalizedString("General_Error", "Error"),
                        LocalizationUtilities.LocalizedString("EditBasketItem_DeleteItemErrorTryAgain", "Could not delete item from basket, please try again."),
                        LocalizationUtilities.LocalizedString("General_OK", "OK")
                    );
                }
            );
        }

        private void UpdateItemInBasket(int positionInBasketList, OneListItem basketItem, Action onSuccess, Action onFailure)
        {
            Utils.UI.ShowLoadingIndicator();
            new BasketModel().EditItemAtPosition(
                positionInBasketList,
                basketItem,
                () =>
                {
                    // Success
                    Utils.UI.HideLoadingIndicator();
                    onSuccess();
                },
                async () =>
                {
                    // Failure
                    Utils.UI.HideLoadingIndicator();
                    await AlertView.ShowAlert(
                        this,
                        LocalizationUtilities.LocalizedString("General_Error", "Error"),
                        LocalizationUtilities.LocalizedString("EditBasketItem_EditItemErrorTryAgain", "Could not edit item, please try again."),
                        LocalizationUtilities.LocalizedString("General_OK", "OK")
                    );

                    onFailure();
                }
            );
        }

        private async void ClearBasket()
        {
            var alertResult = await AlertView.ShowAlert(
                this,
                LocalizationUtilities.LocalizedString("Checkout_ClearBasket", "Clear basket"),
                LocalizationUtilities.LocalizedString("Checkout_AreYouSureClearBasket", "Are you sure you want to clear the basket?"),
                LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
                LocalizationUtilities.LocalizedString("General_No", "No")
            );

            if (alertResult == AlertView.AlertButtonResult.PositiveButton)
            {
                // OK pressed

                new BasketModel().ClearBasket(
                        () =>
                        {
                            // Success
                            Utils.UI.HideLoadingIndicator();
                            this.rootView.Refresh(GetFormattedTotalString());
                        },
                        async () =>
                        {       // Failure
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
        }

        private void RefreshBasket()
        {
            new BasketModel().Refresh(
                () =>
                {
                    this.rootView.Refresh(GetFormattedTotalString());
                },
                () =>
                {
                    this.rootView.Refresh(GetFormattedTotalString());
                },
                () =>
                {
                    this.rootView.StopRefreshingIndicator();
                }
            );
        }

        private async void Checkout()
        {
            // We don't want to calculate the basket if it doesn't contain any items.
            // This function should never be called in the first place if there aren't any basket items.
            if (AppData.Device.UserLoggedOnToDevice.Basket.Items.Count <= 0)
                return;

            Utils.UI.ShowLoadingIndicator();
            OneList calculatedBasket = await new Models.BasketModel().CalculateBasket(AppData.Device.UserLoggedOnToDevice.Basket);
            if (calculatedBasket != null)
            {
                Utils.UI.HideLoadingIndicator();

                // Set the calculated basket as the main basket
                AppData.Device.UserLoggedOnToDevice.Basket = calculatedBasket;
                AppData.Device.UserLoggedOnToDevice.Basket.State = BasketState.Normal;

                // Go to checkout screen
                CheckoutController checkoutController = new CheckoutController();
                this.PresentViewController(
                    new UINavigationController(checkoutController),
                    true,
                    () => { this.rootView.Refresh(GetFormattedTotalString()); }
                );
            }
            else
            {
                Utils.UI.HideLoadingIndicator();
            }

        }

        public static string GenerateItemExtraInfo(OneListItem item)
        {
            var extraInfo = string.Empty;

            if (item.VariantReg != null)
            {
                extraInfo += item.VariantReg.ToString();
            }

            return extraInfo.TrimEnd(System.Environment.NewLine.ToCharArray());
        }
    }
}

