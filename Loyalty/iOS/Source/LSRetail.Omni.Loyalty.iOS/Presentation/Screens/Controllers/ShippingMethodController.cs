using UIKit;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation
{
    public class ShippingMethodController : UIViewController
	{
		private List<ShippingMethod> shippingMethods;
		private ShippingMethodView rootView;

		public ShippingMethodController ()
		{
			this.Title = LocalizationUtilities.LocalizedString("ClickCollect_Shipping", "Shipping");

			this.shippingMethods = new List<ShippingMethod>();
			this.shippingMethods.Add(ShippingMethod.HomeDelivery);
			if (EnabledItems.HasClickAndCollect)
			{
				this.shippingMethods.Add(ShippingMethod.ClickAndCollect);
			}

			this.rootView = new ShippingMethodView ();
			this.rootView.ShippingMethodSelected += ShippingMethodSelected;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			this.rootView.UpdateData (this.shippingMethods);
			this.View = this.rootView;
		}

		public void ShippingMethodSelected(ShippingMethod shippingMethod)
		{
			if(shippingMethod == ShippingMethod.ClickAndCollect)
			{
				ClickAndCollectStoreController CCStoreController = new ClickAndCollectStoreController ();
				this.NavigationController.PushViewController(CCStoreController, true);
			}
			else
			{
				HomeDeliveryController homeDeliveryController = new HomeDeliveryController ();
				this.NavigationController.PushViewController(homeDeliveryController, true);
			}
		}


		/// <summary>
		/// Shipping methods. 
		/// Please note that the the Home delivery shipping method in this solution is only for demo purposes as is.
		/// If you want to support more shipping methods and extend them, then it is recommended to move this enum to the Domain project.
		/// </summary>
		public enum ShippingMethod
		{
			HomeDelivery,
			ClickAndCollect
		}
	}
}

