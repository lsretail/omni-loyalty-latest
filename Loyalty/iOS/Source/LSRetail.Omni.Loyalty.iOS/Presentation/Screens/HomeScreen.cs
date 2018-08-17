using System;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public class HomeScreen : CardCollectionViewController
	{
		public HomeScreen (UICollectionViewFlowLayout layout) : base (layout)
		{
			Title = LocalizationUtilities.LocalizedString("Home_HomeScreen", "HomeScreen");
		}
			
		public override void ViewDidLoad ()
		{
			this.CollectionView.DataSource = new HomeScreenCollectionSource(this);

			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.CollectionView.BackgroundColor = UIColor.White; //Utils.AppColors.BackgroundGray;
		}

		public override void HeaderSelected(object objectInCell)
		{
			System.Diagnostics.Debug.WriteLine ("header selected");
		}
	
		public override void CellSelected(object objectInCell)
		{
			// TODO Figure out what kind of cell this is, and handle it accordingly

			System.Diagnostics.Debug.WriteLine ("cell selected");

			/*ItemDetailsScreen detailsScreen = new ItemDetailsScreen();

			this.NavigationController.PopToRootViewController (false);
			this.NavigationController.PushViewController (detailsScreen, true);*/
		}
			
		public override CardCollectionCell.CellSizes CellSize {
			get {
				return CardCollectionCell.CellSizes.TallWide;
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override List<CardCollectionCell.CellSizes> AvailableCellSizes {
			get {
				return new List<CardCollectionCell.CellSizes>(){ CardCollectionCell.CellSizes.TallWide };
			}
		}
	}
}

