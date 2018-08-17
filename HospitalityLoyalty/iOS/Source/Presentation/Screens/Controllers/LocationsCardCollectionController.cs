using System;
using CoreGraphics;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation.Screens
{
	public class LocationsCardCollectionController : CardCollectionController
	{
		private ErrorGettingDataView errorGettingDataView;
		private LocationsCollectionSource locationsCollectionSource;
		public List<Store> Stores;

		public LocationsCardCollectionController(UICollectionViewFlowLayout layout) : base(layout)
		{
			this.Title = LocalizationUtilities.LocalizedString("Locations_Locations", "Restaurants");
			this.Stores = new List<Store>();
			this.locationsCollectionSource = new LocationsCollectionSource(this);
			this.cellSize = CardCollectionCell.CellSizes.TallWide;
			this.Stores = AppData.Stores;

			if (!HasData)
			{
				GetData();
			}
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public override void ViewDidLoad()
		{
			this.CollectionView.DataSource = this.locationsCollectionSource;
			this.CollectionView.BackgroundColor = AppColors.BackgroundGray;

			base.ViewDidLoad();

			if (!HasData)
				GetData();

			SetRightBarButtonItems();

			RefreshCollectionViewData();
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			UIButton changeLayoutButton = new UIButton();
			changeLayoutButton.SetImage(
				Utils.UI.GetColoredImage(
					Utils.UI.MapCellSizeToIcon(CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize)), UIColor.White), UIControlState.Normal);
			changeLayoutButton.Frame = new CGRect(0, 0, 30, 30);
			changeLayoutButton.TouchUpInside += (object sender, EventArgs e) =>
			{

				CellSize = CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize);

				changeLayoutButton.SetImage(
					Utils.UI.GetColoredImage(
						Utils.UI.MapCellSizeToIcon(CardCollectionCell.GetNextCellSizeInCycle(this.AvailableCellSizes, this.cellSize)), UIColor.White), UIControlState.Normal);

			};
			barButtonItemList.Add(new UIBarButtonItem(changeLayoutButton));

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		public override void HeaderSelected(object objectInCell)
		{
			System.Diagnostics.Debug.WriteLine("header selected");
		}

		public override void CellSelected(object objectInCell)
		{
			if (!(objectInCell is Store))
			{
				System.Diagnostics.Debug.WriteLine("ABORT CellSelection operation. Expected Store but got: " + objectInCell.GetType().ToString());
				return;
			}

			Store selectedLocation = objectInCell as Store;

			if (selectedLocation != null)   // TODO Handle nulls here differently? Now nothing happens when the user presses the cell and the item isn't found
			{
				LocationDetailsController detailsController = new LocationDetailsController(selectedLocation, this.Stores); // TODO The details screen shouldn't have all stores
				this.NavigationController.PushViewController(detailsController, true);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("selectedLocation is null");
			}
		}

		public override CardCollectionCell.CellSizes CellSize
		{
			get
			{
				return this.cellSize;
			}
			set
			{
				this.cellSize = value;
				SetCellSize(value);
				RefreshCollectionViewData();
			}
		}

		public override List<CardCollectionCell.CellSizes> AvailableCellSizes
		{
			get
			{
				return new List<CardCollectionCell.CellSizes>(){
					CardCollectionCell.CellSizes.TallWide,
					CardCollectionCell.CellSizes.TallNarrow,
					CardCollectionCell.CellSizes.ShortWide
				};
			}
		}

		public void MapCellSelected()
		{
			MapController map = new MapController(this.Stores);
			this.NavigationController.PushViewController(map, true);
		}

		private void ShowErrorGettingDataView()
		{
			if (this.errorGettingDataView == null)
			{
				if (IsViewLoaded)
				{
					CGRect errorGettingDataViewFrame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);
					this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame);
					this.errorGettingDataView.Retry += Retry;
					this.View.AddSubview(this.errorGettingDataView);
				}
			}
			else
			{
				this.errorGettingDataView.Hidden = false;
			}
		}

		private void Retry(object sender, EventArgs e)
		{
			GetData();
		}

		private void HideErrorGettingDataView()
		{
			if (this.errorGettingDataView != null)
			{
				this.errorGettingDataView.Hidden = true;
			}
		}

		private void RefreshCollectionViewData()
		{
			this.locationsCollectionSource.RefreshCellTemplates();
			this.locationsCollectionSource.RefreshHeaderTemplates();
			this.CollectionView.ReloadData();   // TODO: Animate
		}

		private bool HasData
		{
			get
			{
				return (AppData.Stores != null && AppData.Stores.Count > 0);
			}
		}

		public async void GetData()
		{
			System.Diagnostics.Debug.WriteLine("LocationsScreen.GetData running");

			Utils.UI.ShowLoadingIndicator();
			List<Store> stores = await new Models.StoreModel().GetAllStores();

			if(stores != null)
			{
				GetDataSuccess(stores);
			}
			else
			{
				GetDataFailure();
			}

		}

		private void GetDataSuccess(List<Store> stores)
		{
			System.Diagnostics.Debug.WriteLine("LocationsScreen.GetData success");

			AppData.Stores = stores;
			this.Stores = AppData.Stores;

			Utils.UI.HideLoadingIndicator();
			HideErrorGettingDataView();

			RefreshCollectionViewData();
		}

		private void GetDataFailure()
		{
			System.Diagnostics.Debug.WriteLine("LocationsScreen.GetData failure");

			Utils.UI.HideLoadingIndicator();
			ShowErrorGettingDataView();
		}
	}
}

