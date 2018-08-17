using CoreGraphics;
using UIKit;
using CoreAnimation;
using System.Collections.Generic;
using Presentation.Utils;
using Presentation.Screens;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using System;

namespace Presentation
{
	public class FavouriteView : BaseView
	{
		#region UI elements

		private UITableView itemTableView;
		private UITableView transactionTableView;
		private UIView noDataView;
		private UIToolbar segmentContainer;
		private UISegmentedControl segmentedControl;

		#endregion

		#region Constants
		private float toolbarHeight = 48f;
		#endregion

		#region View Interface

		private readonly IFavouritesListeners listeners;

		public interface IFavouritesListeners
		{
			bool IsFavourite(IFavorite item);
			bool IsSaleLineFavourite(SaleLine item);
			void ToggleFavourite(IFavorite item);
			void OnToggleFavourite(int index, bool isItem);
			void ItemSelected(MenuItem item);
			void TransactionSelected(Transaction item);
			void AddFavoriteToBasket(int index, bool isItem);
			List<IFavorite> GetItems();
			IFavorite GetItem(int index);
			List<IFavorite> GetTransactions();
			IFavorite GetTransaction(int index);
			void RefreshItemData(); 
			void RefreshTransactionData();
		}

		#endregion

		#region Constructor
		public FavouriteView(IFavouritesListeners listeners)
		{
			this.listeners = listeners;

			this.itemTableView = new UITableView();
			this.transactionTableView = new UITableView();

			this.itemTableView.Source = new FavoriteItemsTableSource(listeners);
			this.transactionTableView.Source = new FavoriteTransactionsTableSource(listeners);

			this.segmentedControl = new UISegmentedControl();
			this.segmentedControl.InsertSegment(LocalizationUtilities.LocalizedString("Favorites_Items", "Items"), 0, true);
			this.segmentedControl.InsertSegment(LocalizationUtilities.LocalizedString("Favorites_Transactions", "Transactions"), 1, true);
			this.segmentedControl.TintColor = UIColor.White;
			this.segmentedControl.SelectedSegment = 0;
			this.segmentedControl.ValueChanged += ShowData;
		}
		#endregion

		#region LayoutSubviews
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.itemTableView.Frame = new CGRect(0f, toolbarHeight, this.Frame.Width, this.Frame.Height - toolbarHeight);
			this.transactionTableView.Frame = new CGRect(0f, toolbarHeight, this.Frame.Width, this.Frame.Height - toolbarHeight);

			if (this.segmentContainer == null)
			{
				float margin = 10f;

				this.segmentContainer = new UIToolbar();
				this.segmentContainer.Delegate = new CustomToolbarDelegate();
				this.segmentContainer.Translucent = false;
				this.segmentContainer.BarTintColor = AppColors.PrimaryColor;

				this.segmentContainer.Frame = new CGRect(0f, this.TopLayoutGuideLength, this.Frame.Width, toolbarHeight);
				this.segmentedControl.Frame = new CGRect(2 * margin, margin, this.segmentContainer.Frame.Width - 4 * margin, this.segmentContainer.Frame.Height - 2 * margin);

				this.AddSubview(this.segmentContainer);

				var barItem = new UIBarButtonItem(segmentedControl);
				var barObjects = new[] { barItem };
				this.segmentContainer.Items = barObjects;
			}

			this.itemTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			itemTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			itemTableView.Tag = 100;

			if (ViewIsNotAdded(itemTableView))
				this.AddSubview(itemTableView);

			this.transactionTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			transactionTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			transactionTableView.Tag = 200;

			if (ViewIsNotAdded(transactionTableView))
				this.AddSubview(transactionTableView);
				
			if (ViewIsNotAdded(noDataView))
			{
				UIView ndView = new UIView();
				ndView.Frame = new CGRect(0, this.TopLayoutGuideLength + this.toolbarHeight, this.Bounds.Width, this.Bounds.Height - this.TopLayoutGuideLength - this.toolbarHeight);
				ndView.BackgroundColor = UIColor.Clear;

				UILabel noDataText = new UILabel();
				float labelHeight = 20;
				noDataText.Frame = new CGRect(0, ndView.Bounds.Height / 2 - labelHeight / 2, ndView.Bounds.Width, labelHeight);
				noDataText.TextColor = UIColor.Gray;
				noDataText.TextAlignment = UITextAlignment.Center;
				noDataText.Font = UIFont.SystemFontOfSize(14);
				noDataText.Tag = 10;
				ndView.AddSubview(noDataText);

				this.noDataView = ndView;
				this.AddSubview(this.noDataView);
			}
		}

		private bool ViewIsNotAdded(UIView view) => Array.IndexOf(this.Subviews, view) == -1;
		#endregion

		#region Show Items && Transactions
		private void ShowData(object sender = null, EventArgs e = null)
		{
			var selectedSegmentId = this.segmentedControl.SelectedSegment;

			if (selectedSegmentId == 0)
				ShowItems();
			else
				ShowTransactions();
		}

		private void ShowItems()
		{
			(this.itemTableView.Source as FavoriteItemsTableSource).RefreshData();
			this.itemTableView.ReloadData();

			this.itemTableView.Hidden = false;
			this.transactionTableView.Hidden = true;

			RefreshNoDataView();
		}

		private void ShowTransactions()
		{
			(this.transactionTableView.Source as FavoriteTransactionsTableSource).RefreshData();
			this.transactionTableView.ReloadData();

			this.itemTableView.Hidden = true;
			this.transactionTableView.Hidden = false;

			RefreshNoDataView();
		}
		#endregion

		#region Reload && Empty Tables

		public void ReloadTables()
		{
			if (this.itemTableView.Source == null)
				this.itemTableView.Source = new FavoriteItemsTableSource(listeners);
			if (this.transactionTableView.Source == null)
				this.transactionTableView.Source = new FavoriteTransactionsTableSource(listeners);

			ShowData();
		}

		public void EmptyTables()
		{
			this.itemTableView.Source = null;
			this.transactionTableView.Source = null;
		}

		#endregion

		public async void ToggleFavorite(object favorite)
		{
			// Let's make the user confirm that he wants to unfavorite an item
			// (because they disappear from the favorites list after being unfavorited)

			if (favorite is MenuItem && listeners.IsFavourite(favorite as IFavorite))
			{
				var alertResult = await AlertView.ShowAlert(
					null,
					LocalizationUtilities.LocalizedString("Favorites_RemoveFromFavorites", "Remove from favorites"),
					LocalizationUtilities.LocalizedString("Favorites_AreYouSureRemoveItem", "Are you sure you want to remove this item from favorites?"),
					LocalizationUtilities.LocalizedString("General_Yes", "Yes"),
					LocalizationUtilities.LocalizedString("General_No", "No")
				);

				if (alertResult == AlertView.AlertButtonResult.PositiveButton)
				{
					//new FavoriteModel().ToggleFavorite(favorite as IFavorite);
					listeners.ToggleFavourite(favorite as IFavorite);
					RefreshWithAnimation();
				}
			}
			else
			{
				listeners.ToggleFavourite(favorite as IFavorite);
				RefreshWithAnimation();
			}
		}

		#region RefreshWithAnimation
		public void RefreshWithAnimation()
		{
			if (this.itemTableView.Source == null || this.transactionTableView.Source == null)
				return;

			CATransition transition = new CATransition();
			transition.Duration = 0.3;
			transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			transition.Type = CATransition.TransitionPush;
			transition.Subtype = CATransition.TransitionFade;
			transition.FillMode = CAFillMode.Both;

			this.itemTableView.Layer.AddAnimation(transition, null);
			this.transactionTableView.Layer.AddAnimation(transition, null);

			(this.itemTableView.Source as FavoriteItemsTableSource).RefreshData();
			this.itemTableView.ReloadData();

			(this.transactionTableView.Source as FavoriteTransactionsTableSource).RefreshData();
			this.transactionTableView.ReloadData();

			if (this.segmentedControl.SelectedSegment == 0)
			{
				if ((this.itemTableView.Source as FavoriteItemsTableSource).HasData)
					HideNoDataView();
			}
			else if (this.segmentedControl.SelectedSegment == 1)
			{
				if ((this.transactionTableView.Source as FavoriteTransactionsTableSource).HasData)
					HideNoDataView();
			}
		}
		#endregion

		#region No data view

		public void RefreshNoDataView()
		{
			if (this.itemTableView.Source == null || this.transactionTableView.Source == null)
				return;

			if (this.segmentedControl.SelectedSegment == 0)
			{
				if (!(this.itemTableView.Source as FavoriteItemsTableSource).HasData)
					ShowNoDataView(LocalizationUtilities.LocalizedString("Favorites_NoItems", "No favorite items"));
				else
					HideNoDataView();
			}
			else if (this.segmentedControl.SelectedSegment == 1)
			{
				if (!(this.transactionTableView.Source as FavoriteTransactionsTableSource).HasData)
					ShowNoDataView(LocalizationUtilities.LocalizedString("Favorites_NoTransactions", "No favorite transactions"));
				else
					HideNoDataView();
			}
			else
			{
				HideNoDataView();
			}
		}

		private void ShowNoDataView(string displayText)
		{
			// If there's no data to be displayed when the view is loaded, this method is called, but the noDataView is not created yet and the app crashes
			// So check for null because this method will be called again and the noDataView will be properly displayed
			if (this.noDataView == null)
				return;
			
			UILabel noDataText = this.noDataView.ViewWithTag(10) as UILabel;
			noDataText.Text = displayText;
			this.noDataView.Hidden = false;
		}

		private void HideNoDataView()
		{
			if (this.noDataView != null)
				this.noDataView.Hidden = true;
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			this.segmentedControl.ValueChanged -= ShowData;

			base.Dispose(disposing);
		}
	}
}