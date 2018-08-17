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
using LSRetail.Omni.Hospitality.Loyalty.iOS;

namespace Presentation.Screens
{
	/*
	public class HistoryScreen : UIViewController
	{
		private UITableView transactionTableView;
		private UIView noDataView;
	
		public HistoryScreen ()
		{
			this.Title = NSBundle.MainBundle.LocalizedString("History_History", "History");
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
			
		public override void ViewWillAppear (bool animated)
		{
			if (this.transactionTableView.Source == null)
				this.transactionTableView.Source = new TransactionHistoryTableSource();

			(this.transactionTableView.Source as TransactionHistoryTableSource).RefreshData();
			this.transactionTableView.ReloadData();

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			RefreshNoDataView();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = UIColor.White;

			this.transactionTableView = new UITableView();
			transactionTableView.Source = new TransactionHistoryTableSource();
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
	*/
}