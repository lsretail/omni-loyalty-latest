using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Presentation.UI
{
	class MenuCollectionViewDelegateFlowLayout : UICollectionViewDelegateFlowLayout
	{
		public CGRect ParentFrame { get; set; }
		public float CellWidth { get; set; }
		public float CellHeight { get; set; }
		public float MinInterItemSpace = 10;//{ get; set; }

		private int currentStyle;
		public MenuCollectionCellLayouts Style
		{
			get { return (MenuCollectionCellLayouts)currentStyle; }
			set
			{
				currentStyle = (int)value;
				SetStyle();
			}
		}

		private WeakReference<IItemSelectedListener> itemSelectedListener;

		private IItemSelectedListener ItemSelectedListener
		{
			get
			{
				IItemSelectedListener listener = null;
				itemSelectedListener.TryGetTarget(out listener);

				return listener;
			}
		}

		public MenuCollectionViewDelegateFlowLayout(IItemSelectedListener listener)
		{
			this.itemSelectedListener = new WeakReference<IItemSelectedListener>(listener);
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			int leftRightInsets = CalculateLeftRightEdgeInsets();
			return new UIEdgeInsets(MinInterItemSpace, leftRightInsets, MinInterItemSpace, leftRightInsets);
		}

		public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return new CGSize(CellWidth, CellHeight);
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return this.MinInterItemSpace;
		}
		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			ItemSelectedListener.ItemSelected(indexPath.Row);
			//base.ItemSelected(collectionView, indexPath);
		}

		private int CalculateLeftRightEdgeInsets()
		{
			nfloat horizontalSpace;
			if (Utils.Util.AppDelegate.DeviceOrientation != UIInterfaceOrientation.Portrait)
				horizontalSpace = Utils.Util.AppDelegate.DeviceScreenHeight;
			else
				horizontalSpace = Utils.Util.AppDelegate.DeviceScreenWidth;

			int minEdgeInsets = 5;  // Edge space must be at least this

			int maxNumberOfCellsInRow = (int)Math.Floor(horizontalSpace / this.CellWidth);  // Not accounting for interitemspace
			int numberOfInterItemSpaces = maxNumberOfCellsInRow - 1;
			int cellsPlusInter = (int)(maxNumberOfCellsInRow * this.CellWidth + numberOfInterItemSpaces * (int)this.MinInterItemSpace);

			// Will this number of cells fit into a row? If not, assume one cell less
			if (2 * minEdgeInsets + cellsPlusInter > horizontalSpace)
			{
				maxNumberOfCellsInRow--;
				numberOfInterItemSpaces = maxNumberOfCellsInRow - 1;
				cellsPlusInter = (int)(maxNumberOfCellsInRow * this.CellWidth + numberOfInterItemSpaces * (int)this.MinInterItemSpace);
			}

			int edgeInsets = (int)Math.Floor((horizontalSpace - cellsPlusInter) / 2);

			return edgeInsets;
		}

		internal void SetStyle()
		{
			switch (Style)
			{
				case MenuCollectionCellLayouts.ThumbnailLarge:
					CellWidth = (float)ParentFrame.Width - MinInterItemSpace * 2;
					CellHeight = (float)ParentFrame.Width / 2 - MinInterItemSpace;
					break;
				case MenuCollectionCellLayouts.ThumbnailSmall:
					CellWidth = CellHeight = (float)ParentFrame.Width / 2 - MinInterItemSpace * 1.5f;
					break;
				case MenuCollectionCellLayouts.Row:
					CellWidth = (float)ParentFrame.Width - MinInterItemSpace * 2;
					CellHeight = (float)ParentFrame.Width / 6;
					break;
			}
		}
	}
}