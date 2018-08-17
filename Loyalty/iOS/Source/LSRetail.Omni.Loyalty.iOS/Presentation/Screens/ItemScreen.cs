using System;
using CoreGraphics;
using UIKit;
using System.Linq;
using System.Collections.Generic;
using Presentation.Utils;
using ZXing.Mobile;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Screens
{
    public class ItemScreen : CardCollectionViewController
	{
		private ItemScreenCollectionSource itemScreenCollectionSource;
		private UIView noDataView;
		private ErrorGettingDataView errorGettingDataView;
		private UIRefreshControl refreshControl;
		private bool allowPullToRefresh = false;
		private MobileBarcodeScanner barcodeScanner;
		private UIButton changeLayoutButton;
		private int lastPageLoaded;
		private bool allItemsLoaded;

		public ItemListType itemListType;
		public ItemCategory itemCategory;
		public ProductGroup productGroup;


		public ItemScreen (UICollectionViewFlowLayout layout, ItemListType itemListType, string title, ItemCategory itemCategory, ProductGroup productGroup, CardCollectionCell.CellSizes cellSize) : base (layout)
		{
			this.itemListType = itemListType;
			this.Title = title;
			this.cellSize = AppData.CellSize;

			this.refreshControl = new UIRefreshControl();

			if (productGroup != null)
			{
				this.itemCategory = itemCategory;
				this.productGroup = productGroup;
				this.itemScreenCollectionSource = new ItemScreenCollectionSource(this);
				this.lastPageLoaded = 0;	
				this.allItemsLoaded = false;
				GetItemsByProductGroup ();
			}
			else if (itemCategory != null)
			{
				this.itemCategory = itemCategory;
				this.itemScreenCollectionSource = new ItemScreenCollectionSource(this);
			}
			else if (!HasData)
			{
				this.itemScreenCollectionSource = new ItemScreenCollectionSource(this);
				GetItemCategories ();
			}
		}
			
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			CellSize = AppData.CellSize;

			if (changeLayoutButton != null) 
			{
				changeLayoutButton.SetImage (
					ImageUtilities.GetColoredImage (
						Utils.UI.MapCellSizeToIcon (CardCollectionCell.GetNextCellSizeInCycle (this.AvailableCellSizes, this.cellSize)), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
			}				
		}
			
		public  override void ViewDidLoad ()
		{											
			this.CollectionView.DataSource = this.itemScreenCollectionSource;
			this.CollectionView.BackgroundColor = Utils.AppColors.BackgroundGray;

			this.refreshControl.ValueChanged += async(object sender, EventArgs e) => {

				System.Diagnostics.Debug.WriteLine ("Refreshing items ...");
                bool success = await new Models.ItemModel ().GetItemCategories ();
                if (success)
                {
                    RefreshDataSuccess();
                }
                else
                {
                    RefreshDataFailure();
                }

			};
			if (this.allowPullToRefresh)
				this.CollectionView.AddSubview(refreshControl);

			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			SetRightBarButtonItems();

			RefreshCollectionViewData ();
		}
			
		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();
			/*
			if (EnabledItems.HasBasket)
				barButtonItemList.Add(Utils.UI.GetBasketBarButtonItem());
			*/

			changeLayoutButton = new UIButton();
			changeLayoutButton.SetImage(
				ImageUtilities.GetColoredImage(
					Utils.UI.MapCellSizeToIcon(CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize)), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
			changeLayoutButton.Frame = new CGRect (0, 0, 30, 30);
			changeLayoutButton.TouchUpInside += (object sender, EventArgs e) => {

				CellSize = CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize);

				changeLayoutButton.SetImage(
					ImageUtilities.GetColoredImage(
						Utils.UI.MapCellSizeToIcon(CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize)), Utils.UI.NavigationBarContentColor), UIControlState.Normal);

			};
			barButtonItemList.Add(new UIBarButtonItem(changeLayoutButton));

			if (EnabledItems.HasBarcodeScanner)
			{
				this.barcodeScanner = new ZXing.Mobile.MobileBarcodeScanner(this);

				UIButton scanBarcodeButton = new UIButton ();
				scanBarcodeButton.Frame = new CGRect (0, 0, 30, 30);
				scanBarcodeButton.SetImage (ImageUtilities.GetColoredImage(ImageUtilities.FromFile ("barcode.png"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
				scanBarcodeButton.TouchUpInside += async (sender, e) => 
				{
					try
					{
						var resault = await this.barcodeScanner.Scan(true);

						if(resault != null)
							BarcodeScanned(resault.Text);
					}
					catch
					{
						await AlertView.ShowAlert(
						    this,
						    LocalizationUtilities.LocalizedString("ScanBarcode_Error", "Failed to start scanner"),
							string.Empty,
							LocalizationUtilities.LocalizedString("General_OK", "OK")
						);
					}
				};

				barButtonItemList.Add(new UIBarButtonItem(scanBarcodeButton));
			}

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private async void BarcodeScanned(string barcode)
		{
           LoyItem item = await new Models.ItemModel().GetItemByBarcode(barcode);
            if(item != null)
            {
                BarcodeScannedSuccess(item);
            }
            else
            {
                BarcodeScannedFailure();
            }

		}

		private async void BarcodeScannedSuccess(LoyItem item)
		{
			try
			{
				if(item != null)
				{
					UINavigationController nc = this.NavigationController;

					ItemDetailsController itemDetailsController = new ItemDetailsController(item);
					nc.PopToRootViewController(false);
					nc.PushViewController (itemDetailsController, false);
				}
				else
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Item_NotFoundTitle", "Item not found"),
						LocalizationUtilities.LocalizedString("Item_NotFoundMsg", "This item is not in our database"),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}
			}
			catch
			{
			}
		}

		private async void BarcodeScannedFailure()
		{
			await AlertView.ShowAlert(
			    this,
				LocalizationUtilities.LocalizedString("ScanBarcode_Error", "Scanning failed"),
				string.Empty,
				LocalizationUtilities.LocalizedString("General_OK", "OK")
			);
		}

		public override void HeaderSelected(object objectInCell)
		{
			System.Diagnostics.Debug.WriteLine ("header selected");
		}

		public override void CellSelected(object objectInCell)
		{
			System.Diagnostics.Debug.WriteLine ("CELL SELECTED: " + objectInCell.GetType ().ToString ());


			if(objectInCell is ItemCategory)
			{
				ItemCategory itemCategory = objectInCell as ItemCategory;
				ItemScreen nextItemScreen = new ItemScreen (new UICollectionViewFlowLayout(), ItemScreen.ItemListType.Group, itemCategory.Description, itemCategory, null, CellSize);

				this.NavigationController.PushViewController (nextItemScreen, true);
			}
			else if(objectInCell is ProductGroup)
			{
				ProductGroup productGroup = objectInCell as ProductGroup;
				ItemScreen nextItemScreen = new ItemScreen (new UICollectionViewFlowLayout(), ItemScreen.ItemListType.Item, productGroup.Description, this.itemCategory, productGroup, CellSize);

				this.NavigationController.PushViewController (nextItemScreen, true);
			}
			else if(objectInCell is LoyItem)
			{
				LoyItem item = objectInCell as LoyItem;
				ItemDetailsController itemDetailsController = new ItemDetailsController(item);

				this.NavigationController.PushViewController (itemDetailsController, true);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Unknown object selected");
			}
		}

		public override CardCollectionCell.CellSizes CellSize {
			get {
				return AppData.CellSize;
			}
			set {
				
				this.cellSize = value;
				AppData.CellSize = value;
				SetCellSize(value);
				RefreshCollectionViewData();
			}
		}

		public override List<CardCollectionCell.CellSizes> AvailableCellSizes {
			get {
				return new List<CardCollectionCell.CellSizes>(){
					CardCollectionCell.CellSizes.TallWide,
					CardCollectionCell.CellSizes.TallNarrow,
					CardCollectionCell.CellSizes.ShortWide
				};
			}
		}

		public override void RegisterCellClasses ()
		{
			CollectionView.RegisterClassForCell (typeof(ItemScreenCell), CardCollectionCell.ShortNarrowCellKey);
			CollectionView.RegisterClassForCell (typeof(ItemScreenCell), CardCollectionCell.ShortWideCellKey);
			CollectionView.RegisterClassForCell (typeof(ItemScreenCell), CardCollectionCell.TallNarrowCellKey);
			CollectionView.RegisterClassForCell (typeof(ItemScreenCell), CardCollectionCell.TallWideCellKey);
		}

		private void ShowErrorGettingDataView()
		{
			if (this.errorGettingDataView == null)
			{
                CGRect errorGettingDataViewFrame = new CGRect(0, (this.TopLayoutGuide == null) ? 10 : this.TopLayoutGuide.Length, this.View.Frame.Width, this.View.Frame.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);
				this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame, GetItemCategories);
				this.View.AddSubview(this.errorGettingDataView);
			}
			else
			{
				this.errorGettingDataView.Hidden = false;
			}
		}

		private void HideErrorGettingDataView()
		{
			if (this.errorGettingDataView != null)
				this.errorGettingDataView.Hidden = true;
		}

		private void RefreshCollectionViewData()
		{
			this.itemScreenCollectionSource.RefreshCellTemplates ();
			this.CollectionView.ReloadData ();	// TODO: Animate
		}

		private bool HasData
		{
			get
			{
				if (AppData.ItemCategories != null && AppData.ItemCategories.Count != 0)
					return true;
				else
					return false;
			}
		}
			
		private async void GetItemCategories ()
		{
			System.Diagnostics.Debug.WriteLine ("ItemScreen.GetItemCategories() running");
			Utils.UI.ShowLoadingIndicator();
            bool success = await new Models.ItemModel ().GetItemCategories ();
            if(success)
            {
                RefreshDataSuccess();
            }
            else
            {
                RefreshDataFailure();
            }

           
		}

		public async void LazyLoadItemsByProductGroup(int tableIndex)
		{
			// if all items are loaded then return
			if (allItemsLoaded)
				return;

			if (tableIndex + 4 == lastPageLoaded * 10) {
				Utils.UI.ShowLoadingIndicator ();
				bool includeDetails = Utils.Util.AppDelegate.GetItemDetailsAtItemListScreen;

				System.Diagnostics.Debug.WriteLine ("Getting Items " + (tableIndex + 4) + "to" + (lastPageLoaded + 10));
                List<LoyItem> items = await new Models.ItemModel().GetItemsByPage(
                    10,
                    lastPageLoaded + 1,
                    this.itemCategory.Id,
                    this.productGroup.Id,
                    "",
                    includeDetails);
                if(items != null)
                {
                    GetItemsByProductGroupSuccess(items);
                }
                else
                {
                    RefreshDataFailure();
                }
				
			}
		}

		private async void GetItemsByProductGroup()
		{
			System.Diagnostics.Debug.WriteLine ("ItemScreen.GetItemsByProductGroup() running");
			Utils.UI.ShowLoadingIndicator();
			bool includeDetails = Utils.Util.AppDelegate.GetItemDetailsAtItemListScreen;

			System.Diagnostics.Debug.WriteLine ("Getting Items" + "0" + "to" + (lastPageLoaded + 10));

            List <LoyItem> items = await new Models.ItemModel ().GetItemsByPage (
				10, 
				lastPageLoaded + 1, 
				this.itemCategory.Id, 
				this.productGroup.Id,
				"",
                includeDetails);

			if (items != null)
			{
				GetItemsByProductGroupSuccess(items);
			}
			else
			{
				RefreshDataFailure();
			}
			
			
		}

		private void GetItemsByProductGroupSuccess(List<LoyItem> items)
		{
			System.Diagnostics.Debug.WriteLine ("ItemScreen.GetItemsByProductGroup success");

			lastPageLoaded++;

			foreach (var item in items) {

				if (this.productGroup.Items.FindIndex(x => x.Id == item.Id) == -1)
				{
					this.productGroup.Items.Add (item);
				}
			}

			if (items.Count < 10)
				allItemsLoaded = true;
			
			this.refreshControl.EndRefreshing();
			Utils.UI.HideLoadingIndicator();
			HideErrorGettingDataView();

			if(items.Count == 0 && this.productGroup.Items.Count == 0)
			{
				ShowNoDataView ();
			}
			else
			{
				RefreshCollectionViewData ();
			}
		}

		private void RefreshDataSuccess()
		{
			System.Diagnostics.Debug.WriteLine ("ItemScreen.RefreshData success");
			this.refreshControl.EndRefreshing();
			Utils.UI.HideLoadingIndicator();
			HideErrorGettingDataView();
			RefreshCollectionViewData ();
		}

		private void RefreshDataFailure()
		{
			System.Diagnostics.Debug.WriteLine ("ItemScreen.RefreshData failure");

			this.refreshControl.EndRefreshing();
			Utils.UI.HideLoadingIndicator();
			ShowErrorGettingDataView();
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
				noDataText.Frame =  new CGRect(0, ndView.Bounds.Height/2 - labelHeight/2, ndView.Bounds.Width, labelHeight);
				noDataText.Text = LocalizationUtilities.LocalizedString("ItemDetails_NoItems", "No items to display.");
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
			
		/*
		public void AddItemToBasket(MenuItem item)
		{
			// Have to check if there are any required modifiers.
			// If there are, show the modifiersscreen but only display the required modifiers.
			// If there aren't, add the item straight to basket and bypass the modifiersscreen.

			bool hasRequiredModifiers = false;

			if (item is Product)
				hasRequiredModifiers = (item as Product).AnyRequired;
			else if (item is Recipe)
				hasRequiredModifiers = (item as Recipe).AnyRequired;
			else if (item is Deal)
				hasRequiredModifiers = (item as Deal).AnyRequired;

			if (hasRequiredModifiers)
			{
				// Go to modifiers screen, but show only required modifiers
				AddToBasketScreen2 addToBasketScreen = new AddToBasketScreen2(item.Clone(), 1, true);
				this.PresentViewController(new UINavigationController(addToBasketScreen), true, null);
			}
			else
			{
				// Add item straight to basket (no modifiers screen)
				new Models.BasketModel().AddItemToBasket(item.Clone(), 1);
				Utils.Util.AppDelegate.SlideoutBasket.Refresh();
				Utils.Util.AppDelegate.SlideoutNavCtrl.ShowMenuRight();
			}
		}
		*/

		public enum ItemListType
		{
			None = 0,
			Category = 1,
			Group = 2,
			Item = 3,
		}
	}
}

