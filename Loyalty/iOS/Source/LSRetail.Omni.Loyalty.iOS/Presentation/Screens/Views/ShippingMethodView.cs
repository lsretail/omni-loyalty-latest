using System;
using UIKit;
using System.Collections.Generic;

namespace Presentation
{
	public class ShippingMethodView : UIView
	{
		private UITableView shippingMethodTableView;

		public delegate void ShippingMethodSelectedEventHandler (ShippingMethodController.ShippingMethod shippingMethod);
		public ShippingMethodSelectedEventHandler ShippingMethodSelected;

		public ShippingMethodView ()
		{
			this.BackgroundColor = UIColor.White;
			this.shippingMethodTableView = new UITableView ();
			this.shippingMethodTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.shippingMethodTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.AddSubview (this.shippingMethodTableView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.shippingMethodTableView.Frame = new CoreGraphics.CGRect (
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);
		}

		public void UpdateData (List<ShippingMethodController.ShippingMethod> shippingMethods)
		{
			this.shippingMethodTableView.Source = new ShippingMethodTableSource (shippingMethods);
			(this.shippingMethodTableView.Source as ShippingMethodTableSource).ShippingMethodSelected += (ShippingMethodController.ShippingMethod shippingMethod) => {
				if (this.ShippingMethodSelected != null)
				{
					this.ShippingMethodSelected (shippingMethod);
				}
			};
		}
	}
}

