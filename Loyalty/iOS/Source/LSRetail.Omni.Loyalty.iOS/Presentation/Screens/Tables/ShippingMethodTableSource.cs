using System;
using UIKit;
using Foundation;
using Presentation.Utils;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation
{
    public class ShippingMethodTableSource : UITableViewSource
	{
		private List<ShippingMethodController.ShippingMethod> shippingMethods;
		private UIView headerView;

		public delegate void ShippingMethodSelectedEventHandler (ShippingMethodController.ShippingMethod shippingMethod);
		public ShippingMethodSelectedEventHandler ShippingMethodSelected;

		public ShippingMethodTableSource (List<ShippingMethodController.ShippingMethod> shippingMethods)
		{
			this.shippingMethods = shippingMethods;
			BuildHeaderView();
		}

		private void BuildHeaderView()
		{
			headerView = new UIView();
			headerView.BackgroundColor = Utils.AppColors.TransparentWhite;

			// Total
			UILabel lblSelect = new UILabel()
			{
				Text = LocalizationUtilities.LocalizedString("Checkout_ShippingMethod", "Select shipping method") + ":",
				TextColor = AppColors.PrimaryColor,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize(16)
			};
			headerView.AddSubview (lblSelect);

			headerView.ConstrainLayout(() =>

				lblSelect.Frame.GetCenterY() == headerView.Frame.GetCenterY() &&
				lblSelect.Frame.Left == headerView.Frame.Left &&
				lblSelect.Frame.Right == headerView.Frame.Right &&
				lblSelect.Frame.Width == headerView.Frame.Width
			);
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (EnabledItems.HasClickAndCollect)
			{
				return 2;
			}
			else
			{
				return 1;
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			ShippingMethodTableViewCell cell = tableView.DequeueReusableCell (ShippingMethodTableViewCell.Key) as ShippingMethodTableViewCell;
			if (cell == null)
				cell = new ShippingMethodTableViewCell();


			ShippingMethodController.ShippingMethod shippingMethod = this.shippingMethods [indexPath.Row];

			if(shippingMethod == ShippingMethodController.ShippingMethod.HomeDelivery)
			{
				string title = LocalizationUtilities.LocalizedString("Checkout_HomeDelivery", "Home Delivery");
				string extraInfo = LocalizationUtilities.LocalizedString("Checkout_HomeDeliveryDescription", "Let us send you the order");
				string imageName = "TruckIcon.png";

				cell.SetValues(indexPath.Row, title, extraInfo, imageName);
			}
			else if(shippingMethod == ShippingMethodController.ShippingMethod.ClickAndCollect)
			{
				string title = LocalizationUtilities.LocalizedString("ClickCollect_ClickCollect", "Click & Collect");
				string extraInfo = LocalizationUtilities.LocalizedString("Checkout_ClickCollectDescription", "Pick up your order at a chosen store at your own time");
				string imageName = "StoreIcon.png";

				cell.SetValues(indexPath.Row, title, extraInfo, imageName);
			}

			return cell;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			if (section == 0)
				return this.headerView;
			else
				return null;
		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			if (section == 0)
				return 44f;
			else
				return 0f;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ShippingMethodController.ShippingMethod shippingMethod = this.shippingMethods[indexPath.Row];

			if (this.ShippingMethodSelected != null)
			{
				this.ShippingMethodSelected (shippingMethod);
			}

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 120f;
		}
	}
}

