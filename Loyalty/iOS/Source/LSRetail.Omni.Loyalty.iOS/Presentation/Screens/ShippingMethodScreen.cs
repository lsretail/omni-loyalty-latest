using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;

namespace Presentation.Screens
{
	/*
	public class ShippingMethodScreen : UIViewController
	{
		private UITableView shippingMethodTableView;
		private ShippingMethodTableSource shippingMethodTableViewSource;
		protected List<string> shippingMethods;

		public ShippingMethodScreen ()
		{
			//TODO : Check with NAV if Click & Collect is available or not - then display accordingly

			this.Title = NSBundle.MainBundle.LocalizedString("ClickCollect_Shipping", "Shipping");

			this.shippingMethodTableView = new UITableView ();
			this.shippingMethodTableViewSource = pnew ShippingMethodTableSource(this);
			this.shippingMethodTableView.Source = this.shippingMethodTableViewSource;
		}
			
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (this.shippingMethodTableView.Source == null)
				this.shippingMethodTableView.Source = new ShippingMethodTableSource(this);
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = UIColor.White;

			this.shippingMethods = new List<string>() ; //TODO : Create "ShippingMethods enum in Domain project, stop using this
			this.shippingMethods.Add(NSBundle.MainBundle.LocalizedString("Checkout_HomeDelivery", "Home Delivery"));
			if (Utils.Util.AppDelegate.ClickAndCollect)
			{
				this.shippingMethods.Add(NSBundle.MainBundle.LocalizedString("ClickCollect_ClickCollect", "Click & Collect"));
			}
				
			this.shippingMethodTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			shippingMethodTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			shippingMethodTableView.Tag = 100;
			shippingMethodTableView.Hidden = false;
			this.View.AddSubview(shippingMethodTableView);

			this.View.ConstrainLayout(() =>

				shippingMethodTableView.Frame.Top == this.View.Bounds.Top &&
				shippingMethodTableView.Frame.Left == this.View.Bounds.Left &&
				shippingMethodTableView.Frame.Right == this.View.Bounds.Right &&
				shippingMethodTableView.Frame.Bottom == this.View.Bounds.Bottom
			);
		}

		public void ShippingMethodSelected(string shippingMethod)
		{
			if(shippingMethod == NSBundle.MainBundle.LocalizedString("ClickCollect_ClickCollect", "Click & Collect")) //TODO : Use enum
			{
				ClickAndCollectStoreScreen CCStoreScreen = new ClickAndCollectStoreScreen();
				this.NavigationController.PushViewController(CCStoreScreen, true);
			}
			else
			{
				HomeDeliveryScreen homeDeliveryScreen = new HomeDeliveryScreen();
				this.NavigationController.PushViewController(homeDeliveryScreen, true);
			}
		}
	}
	*/
}

