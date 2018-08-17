using System;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using System.Collections.Generic;
using Presentation.Utils;
using Domain.Transactions;
using Domain.Menus;
using Presentation.Models;

namespace Presentation.Screens
{
	public class HistoryScreen2 : UIViewController
	{
		private UITableView transactionTableView;
		private UIView noDataView;
	
		public HistoryScreen2()
		{}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
			
		public override void ViewWillAppear (bool animated)
		{
			if (this.transactionTableView.Source == null)
				this.transactionTableView.Source = new TransactionHistoryTableSource(this);

			(this.transactionTableView.Source as TransactionHistoryTableSource).RefreshData();
			this.transactionTableView.ReloadData();

			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Navigationbar
			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes (false);
			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;

			RefreshNoDataView();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.Title = NSBundle.MainBundle.LocalizedString("History_History", "History");

			this.View.BackgroundColor = UIColor.White;

			this.transactionTableView = new UITableView();
			transactionTableView.Source = new TransactionHistoryTableSource(this);
			transactionTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			transactionTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			transactionTableView.Tag = 200;
			this.View.AddSubview(transactionTableView);

			this.View.ConstrainLayout(() =>
			
				transactionTableView.Frame.Top == this.View.Bounds.Top &&
				transactionTableView.Frame.Left == this.View.Bounds.Left &&
				transactionTableView.Frame.Right == this.View.Bounds.Right &&
				transactionTableView.Frame.Bottom == this.View.Bounds.Bottom

			);
				
			SetRightBarButtonItems();
		}
			
		public override void ViewDidDisappear (bool animated)
		{
			// Attempt to fix 'random' segfaulting error by nulling the tableview sources
			this.transactionTableView.Source = null;

			base.ViewDidDisappear (animated);
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			if (Utils.Util.AppDelegate.BasketEnabled)
				barButtonItemList.Add(Utils.UI.GetBasketBarButtonItem());

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}
			
		public void TransactionSelected(Transaction transaction)
		{
			TransactionDetailsScreen transactionDetailsScreen = new TransactionDetailsScreen(transaction);
			this.NavigationController.PushViewController (transactionDetailsScreen, true);
		}

		private void RefreshNoDataView()
		{
			if (this.transactionTableView.Source == null)
				return;

			if (!(this.transactionTableView.Source as TransactionHistoryTableSource).HasData)
				ShowNoDataView();
			else
				HideNoDataView();
		}

		private void ShowNoDataView()
		{
			if (this.noDataView == null)
			{
				UIView ndView = new UIView();
				ndView.Frame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);
				ndView.BackgroundColor = UIColor.Clear;

				UILabel noDataText = new UILabel();
				float labelHeight = 20;
				noDataText.Frame = new CGRect(0, ndView.Bounds.Height/2 - labelHeight/2, ndView.Bounds.Width, labelHeight);
				noDataText.Text = NSBundle.MainBundle.LocalizedString("History_NoData", "No previous transactions available");
				noDataText.TextColor = UIColor.Gray;
				noDataText.TextAlignment = UITextAlignment.Center;
				noDataText.Font = UIFont.SystemFontOfSize(14);
				ndView.AddSubview(noDataText);

				this.noDataView = ndView;
				this.View.AddSubview(this.noDataView);
			}
			else
			{
				this.noDataView.Hidden = false;
			}
		}

		private void HideNoDataView()
		{
			if (this.noDataView != null)
				this.noDataView.Hidden = true;
		}
	}

	public class TransactionHistoryTableSource : UITableViewSource
	{
		private HistoryScreen2 controller;
		private List<Transaction> transactions;

		public bool HasData { get { return this.transactions.Count > 0; } }

		public TransactionHistoryTableSource (HistoryScreen2 controller)
		{
			this.controller = controller;
			this.transactions = new List<Transaction>();
			RefreshData();
		}
			
		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.transactions.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			TransactionOverviewTableViewCell cell = tableView.DequeueReusableCell (TransactionOverviewTableViewCell.Key) as TransactionOverviewTableViewCell;
			if (cell == null)
				cell = new TransactionOverviewTableViewCell();
				
			Transaction transaction = this.transactions[indexPath.Row];

			string itemCountString = transaction.SaleLines.Count().ToString();
			if (transaction.SaleLines.Count() == 1)
				itemCountString += " " + NSBundle.MainBundle.LocalizedString("History_Item_Lowercase", "item");
			else
				itemCountString += " " + NSBundle.MainBundle.LocalizedString("History_Items_Lowercase", "items");

			cell.SetValues(indexPath.Row, transaction.DateToShortFormat, itemCountString, transaction.AmountForDisplay);

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var transactionInCell = this.transactions[indexPath.Row];
			this.controller.TransactionSelected(transactionInCell);

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 70f;
		}

		public void RefreshData()
		{
			this.transactions = AppData.Transactions.OrderByDescending(x => x.Date).ToList();
		}
	}
}