using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace Presentation.Screens
{
	public abstract class CardCollectionViewController : UICollectionViewController
	{
		//old hardcoded values
		//private int halfCellWidth = 145;
		//private int shortCellHeight = 60;
		//private int tallCellHeight = 160;

		private int halfCellWidth = (int)Utils.Util.AppDelegate.DeviceScreenWidth/2 - 15;
		private int shortCellHeight = (int)(Utils.Util.AppDelegate.DeviceScreenWidth/2 - 40) / 2;
		private int tallCellHeight = (int)Utils.Util.AppDelegate.DeviceScreenWidth/2;
		private int interCellSpace = 10;
		
	
		public CardCollectionViewController (UICollectionViewFlowLayout layout) : base (layout)
		{}

		protected CardCollectionCell.CellSizes cellSize;
		public abstract void HeaderSelected (object objectOnDisplay);
		public abstract void CellSelected (object objectOnDisplay);
		public abstract CardCollectionCell.CellSizes CellSize { get; set; }
		public abstract List<CardCollectionCell.CellSizes> AvailableCellSizes { get; }

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			// Must specify data source in subclass before this function is run

			base.ViewDidLoad ();

			// Register classes
			//CollectionView.RegisterClassForSupplementaryView (typeof(CardCollectionHeader), UICollectionElementKindSection.Header, CardCollectionHeader.Key);
			//Layout.RegisterClassForDecorationView (typeof(CardCollectionDecorationView), CardCollectionDecorationView.Key);
			RegisterCellClasses();

			CardCollectionSource myDataSource = this.CollectionView.DataSource as CardCollectionSource;
			this.CollectionView.Delegate = new CardCollectionViewDelegateFlowLayout (myDataSource);

			SetCellSize(this.CellSize);

			/*
			if (!myDataSource.ContainsHeaders)
				headerImageHeight = 0;
		
			SetHeaderSize (headerImageHeight);
			*/

			this.CollectionView.AlwaysBounceVertical = true;
		}
			
		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			// 24.01.14 - App locked in portrait mode at the moment, so this doesn't matter 
			this.CollectionView.CollectionViewLayout.InvalidateLayout ();
		}
			
		private void SetCellDimensions(int width, int height, float minInterItemSpace)
		{
			CardCollectionViewDelegateFlowLayout myDelegate = this.CollectionView.Delegate as CardCollectionViewDelegateFlowLayout;
			myDelegate.CellWidth = width;
			myDelegate.CellHeight = height;
			myDelegate.MinInterItemSpace = minInterItemSpace;

			System.Diagnostics.Debug.WriteLine("Cell dimensions set to: " + width + "x" + height);
		}

		public void SetCellSize(CardCollectionCell.CellSizes sizeToUse)
		{
			if (sizeToUse == CardCollectionCell.CellSizes.TallNarrow)
				SetCellDimensions (halfCellWidth, tallCellHeight, interCellSpace);
			else if (sizeToUse == CardCollectionCell.CellSizes.TallWide)
				SetCellDimensions (halfCellWidth, tallCellHeight, interCellSpace);
			else if (sizeToUse == CardCollectionCell.CellSizes.ShortNarrow)
				SetCellDimensions (halfCellWidth, shortCellHeight, interCellSpace);
			else if (sizeToUse == CardCollectionCell.CellSizes.ShortWide)
				SetCellDimensions (halfCellWidth, shortCellHeight, interCellSpace);
			else
				SetCellDimensions (halfCellWidth, tallCellHeight, interCellSpace);	// Default to tall wide
		}

		/*
		public void SetHeaderSize(int height)
		{
			UICollectionViewFlowLayout myLayout = this.Layout as UICollectionViewFlowLayout;
			myLayout.HeaderReferenceSize = new SizeF (this.CollectionView.Bounds.Width, height);
		}
		*/

		public virtual void RegisterCellClasses()
		{
			CollectionView.RegisterClassForCell (typeof(CardCollectionCell), CardCollectionCell.ShortNarrowCellKey);
			CollectionView.RegisterClassForCell (typeof(CardCollectionCell), CardCollectionCell.ShortWideCellKey);
			CollectionView.RegisterClassForCell (typeof(CardCollectionCell), CardCollectionCell.TallNarrowCellKey);
			CollectionView.RegisterClassForCell (typeof(CardCollectionCell), CardCollectionCell.TallWideCellKey);
		}
	}

	class CardCollectionViewDelegateFlowLayout: UICollectionViewDelegateFlowLayout
	{
		public int CellWidth { get; set; }
		public int CellHeight { get; set; }
		public float MinInterItemSpace { get; set; }
		private int wideCellWidth { get { return 2 * this.CellWidth + (int)this.MinInterItemSpace; } }	// TODO Allow more widths?
		private CardCollectionSource dataSource;

		public CardCollectionViewDelegateFlowLayout(CardCollectionSource dataSource)
		{
			this.dataSource = dataSource;
		}

		public override UIEdgeInsets GetInsetForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			int leftRightInsets = CalculateLeftRightEdgeInsets();
			return new UIEdgeInsets (10, leftRightInsets, 10, leftRightInsets);
		}

		public override CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			if (!dataSource.IsCellWide(indexPath.Section, indexPath.Row))
				return new CGSize (this.CellWidth, this.CellHeight);
			else
				return new CGSize (this.wideCellWidth, this.CellHeight);
		}

		public override nfloat GetMinimumInteritemSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return this.MinInterItemSpace;
		}
			
		/// <summary>
		/// We want the cells to be grouped together, not stuck to the edges of the screen with whitespace in between,
		/// as is the default behaviour of the flowlayout. We calculate the appropriate left and right edge insets to fix this.
		/// </summary>
		/// <returns>The left right edge insets.</returns>
		private int CalculateLeftRightEdgeInsets()
		{
			// The layout is like this:
			// edge - cell - interitemspace - cell - ... - edge (if two narrow cells in line)
			// edge - cell - edge (if one wide cell in line)

			// These calculations disregard wide cells, only take into account narrow cells
			// However, this works if the wide cells are exactly two narrow cells + intersitemspace in width

			nfloat horizontalSpace;
			if (Utils.Util.AppDelegate.DeviceOrientation != UIInterfaceOrientation.Portrait)
				horizontalSpace = Utils.Util.AppDelegate.DeviceScreenHeight;
			else
				horizontalSpace = Utils.Util.AppDelegate.DeviceScreenWidth;

			int minEdgeInsets = 5;	// Edge space must be at least this

			int maxNumberOfCellsInRow = (int)Math.Floor(horizontalSpace / this.CellWidth);	// Not accounting for interitemspace
			int numberOfInterItemSpaces = maxNumberOfCellsInRow - 1;
			int cellsPlusInter = maxNumberOfCellsInRow * this.CellWidth + numberOfInterItemSpaces * (int)this.MinInterItemSpace;

			// Will this number of cells fit into a row? If not, assume one cell less
			if (2 * minEdgeInsets + cellsPlusInter > horizontalSpace)
			{
				maxNumberOfCellsInRow--;
				numberOfInterItemSpaces = maxNumberOfCellsInRow - 1;
				cellsPlusInter = maxNumberOfCellsInRow * this.CellWidth + numberOfInterItemSpaces * (int)this.MinInterItemSpace;
			}

			int edgeInsets = (int)Math.Floor((horizontalSpace - cellsPlusInter) / 2);

			return edgeInsets;
		}
	}

	/* NOT IN USE
	// Decoration view - NOT ACTUALLY IN USE AT THE MOMENT
	public class CardCollectionDecorationView  : UICollectionReusableView
	{
		public static readonly NSString Key = new NSString ("CardCollectionDecorationView");

		[Export("initWithFrame:")]
		public CardCollectionDecorationView(System.Drawing.RectangleF frame) : base(frame)
		{
			UIView myView = new UIView ();
			myView.Frame = frame;
			myView.BackgroundColor = UIColor.Yellow;
			this.AddSubview (myView);
		}
	}
	*/

	/* NOT IN USE
	// TODO Put in its own .cs file
	// Supplementary view
	public class CardCollectionHeader : UICollectionReusableView
	{
		public static readonly NSString Key = new NSString ("CardCollectionHeader");
		private UIImageView headerImageView;
		private UIView overlayView;
		private UILabel textLabel;
		public object objectOnDisplay;
		public Action<object> onSelected;
		public int Id;

		[Export("initWithFrame:")]
		public CardCollectionHeader(System.Drawing.RectangleF frame) : base(frame)
		{
			this.headerImageView = new UIImageView ();
			this.headerImageView.Frame = frame;
			this.headerImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.headerImageView.Tag = 100;

			int overlayViewHeight = (int)Math.Floor (this.headerImageView.Frame.Height / 4);
			this.overlayView = new UIView ();
			this.overlayView.Frame = new RectangleF (this.headerImageView.Frame.X, this.headerImageView.Frame.Height - overlayViewHeight, this.headerImageView.Frame.Width, overlayViewHeight);
			this.overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
			this.overlayView.Tag = 200;

			this.textLabel = new UILabel ();
			float textLabelMargins = 5;
			this.textLabel.Frame = new RectangleF (0f + textLabelMargins, 0f, this.overlayView.Frame.Width - 2 * textLabelMargins, this.overlayView.Frame.Height);
			this.textLabel.TextColor = UIColor.White;
			this.textLabel.Font = UIFont.FromName ("Helvetica", 14);
			this.textLabel.Tag = 300;

			this.overlayView.AddSubview (this.textLabel);
			this.headerImageView.AddSubview (overlayView);

			AddSubview(this.headerImageView);
		}

		public void SetValues(int id, object objectToDisplay, Action<object> onSelected, bool showText, string text, string imageFileName)
		{
			this.Id = id;
			this.objectOnDisplay = objectToDisplay;
			this.onSelected = onSelected;

			this.headerImageView.Image = Utils.Image.FromFile (imageFileName);
			this.textLabel.Text = text;

			if (!showText)
			{
				this.overlayView.RemoveFromSuperview();
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			this.onSelected(this.objectOnDisplay);
		}
	}
	*/
}

