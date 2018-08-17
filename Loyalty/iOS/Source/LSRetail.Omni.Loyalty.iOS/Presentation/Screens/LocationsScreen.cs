using System;
using CoreGraphics;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using System.Linq;

namespace Presentation.Screens
{
    public class LocationsScreen : CardCollectionViewController
	{
		private ErrorGettingDataView errorGettingDataView;
		private LocationsScreenCollectionSource locationsScreenCollectionSource;
		private bool itemInStockLocations;
		public List<Store> Stores;

		public LocationsScreen (UICollectionViewFlowLayout layout, List<Store> stores, bool itemInStockLocations) : base (layout)
		{
			Title = LocalizationUtilities.LocalizedString("Location_Locations", "Locations");

            Stores = new List<Store>();
			locationsScreenCollectionSource = new LocationsScreenCollectionSource (this);
			cellSize = AppData.CellSize;
			Stores = stores;
			this.itemInStockLocations = itemInStockLocations;
		}
			
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar); 

			CellSize = AppData.CellSize;
		}

		public override void ViewDidLoad ()
		{
			this.CollectionView.DataSource = this.locationsScreenCollectionSource;
			this.CollectionView.BackgroundColor = Utils.AppColors.BackgroundGray;

			base.ViewDidLoad ();

            if (!HasData && !this.itemInStockLocations)
            {
                GetData();
            }

            // If the data has already been loaded (HasData == true), but the Stores property has not been set yet with the new data
            // set it, otherwise the locations won't be shown on the view
            // This may happen when the constructor is called before the data is loaded - eg: when creating the instance in the 'More' menu (RootTabBarController), then the data is loaded through the Home page Locations screen
            if (!this.itemInStockLocations && HasData && (Stores == null || !Stores.Any()))
            {
                Stores = AppData.Stores;
            }

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

			UIButton changeLayoutButton = new UIButton();
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

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		public override void HeaderSelected(object objectInCell)
		{
			System.Diagnostics.Debug.WriteLine ("header selected");
		}

		public override void CellSelected(object objectInCell)
		{
			if (!(objectInCell is Store))
			{
				System.Diagnostics.Debug.WriteLine ("ABORT CellSelection operation. Expected Store but got: " + objectInCell.GetType().ToString());
				return;
			}

			Store selectedLocation = objectInCell as Store;

			if (selectedLocation != null)	// TODO Handle nulls here differently? Now nothing happens when the user presses the cell and the item isn't found
			{
				LocationDetailController detailController = new LocationDetailController(selectedLocation, this.Stores);
				this.NavigationController.PushViewController (detailController, true);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("selectedLocation is null");
			}
		}

		public override CardCollectionCell.CellSizes CellSize {
			get {
				return this.cellSize;
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

		public void MapCellSelected()
		{
			MapController map = new MapController(this.Stores, false);
			this.NavigationController.PushViewController(map, true);
		}

		private void ShowErrorGettingDataView()
		{
			if (this.errorGettingDataView == null)
			{
				CGRect errorGettingDataViewFrame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);
				this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame, GetData);
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
			this.locationsScreenCollectionSource.RefreshCellTemplates ();
			this.locationsScreenCollectionSource.RefreshHeaderTemplates ();
			this.CollectionView.ReloadData ();	// TODO: Animate
		}

        private bool HasData => AppData.Stores != null && AppData.Stores.Any();

        public async void GetData()
        {
            System.Diagnostics.Debug.WriteLine("LocationsScreen.GetData running");

            Utils.UI.ShowLoadingIndicator();
            List<Store> storeList = await new Models.StoreModel().GetAllStores();
            if (storeList != null)
            {
                GetDataSuccess(storeList);
            }
            else
            {
                GetDataFailure();
            }
        }

		private void GetDataSuccess(List<Store> stores)
		{
			System.Diagnostics.Debug.WriteLine ("LocationsScreen.GetData success");

			this.Stores = stores;

			Utils.UI.HideLoadingIndicator();
			HideErrorGettingDataView();

			RefreshCollectionViewData ();
		}

		private void GetDataFailure()
		{
			System.Diagnostics.Debug.WriteLine ("LocationsScreen.GetData failure");

			Utils.UI.HideLoadingIndicator();
			ShowErrorGettingDataView();
		}
	}
}

