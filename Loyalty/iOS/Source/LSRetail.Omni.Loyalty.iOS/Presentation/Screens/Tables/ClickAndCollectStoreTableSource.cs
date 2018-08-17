using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using Presentation.Utils;
using System.Linq;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class ClickAndCollectStoreTableSource : UITableViewSource
	{
		private UIView headerView;
		private List<Store> stores;
		public bool HasData { get { return this.stores.Count > 0; } }

		public delegate void StoreInfoButtonPressedEventHandler (Store store);
		public event StoreInfoButtonPressedEventHandler StoreInfoButtonPressed;

		public delegate void StoreSelectedEventHandler (Store store);
		public event StoreSelectedEventHandler StoreSelected;

		public ClickAndCollectStoreTableSource (List<Store> stores)
		{
			this.stores = stores;

			BuildHeaderView();
		}

		private void BuildHeaderView()
		{
			headerView = new UIView();
			headerView.BackgroundColor = Utils.AppColors.TransparentWhite;

			// Total
			UILabel lblSelect = new UILabel()
			{
				Text = LocalizationUtilities.LocalizedString("ClickCollect_SelectStore", "Select store to collect from" + ":"),
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
			return this.stores.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			ClickAndCollectStoreTableViewCell cell = tableView.DequeueReusableCell (ClickAndCollectStoreTableViewCell.Key) as ClickAndCollectStoreTableViewCell;
			if (cell == null)
				cell = new ClickAndCollectStoreTableViewCell();

			Store store = this.stores [indexPath.Row];

			string title = store.Description;
			string distance = decimal.Round((decimal)store.Distance).ToString() + " " + LocalizationUtilities.LocalizedString("ClickCollect_StoreDistance", "km. away from here");
			string extraInfo = store.Address + "\n" + "\n" + distance;


			// Image
			ImageView imageView = store.Images.FirstOrDefault();
			string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
			string imageId = (imageView != null ? imageView.Id : string.Empty);

			cell.SetValues(indexPath.Row, HandleInfoButtonPress, title, extraInfo, imageAvgColor, imageId);

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

		public void HandleInfoButtonPress(int cellIndexPathRow)
		{
			Store store = this.stores[cellIndexPathRow];
			if (this.StoreInfoButtonPressed != null)
			{
				this.StoreInfoButtonPressed (store);
			}

			//this.controller.StoreInfoButtonPressed (store);
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			Store store = this.stores[indexPath.Row];
			if (this.StoreSelected != null)
			{
				this.StoreSelected (store);
			}
			//this.controller.StoreSelected (store);

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			Store store = this.stores [indexPath.Row];

			return ClickAndCollectStoreTableViewCell.GetCellHeight (store.FormatAddress);
		}

		public void RefreshData (List<Store> stores)
		{
			this.stores = stores;
		}
	}
}

