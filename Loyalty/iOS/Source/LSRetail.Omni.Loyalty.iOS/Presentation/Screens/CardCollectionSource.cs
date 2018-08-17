using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using Presentation.Utils;

namespace Presentation.Screens
{
	public abstract class CardCollectionSource : UICollectionViewDataSource
	{
		protected List<HeaderTemplate> headerTemplateList;
		protected List<CellTemplate> cellTemplateList;

		public bool HasData { 
			get { 
				if (this.headerTemplateList != null && this.headerTemplateList.Count > 0)
					return true;
				else if (this.cellTemplateList != null && this.cellTemplateList.Count > 0)
					return true;
				else
					return false;
			}
		}

		public bool ContainsHeaders { 
			get { 
				if (this.headerTemplateList == null)
					return false;
				else if (this.headerTemplateList.Count == 0)
					return false;
				else
					return true;
			} 
		}

		public CardCollectionSource()
		{
			this.cellTemplateList = new List<CellTemplate> ();
			this.headerTemplateList = new List<HeaderTemplate> ();  
		}

		public abstract void BuildCellTemplates ();
		public abstract void BuildHeaderTemplates ();

		// NOTE: Have to override this in subclasses if we want to use another celltype
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			NSString cellKey;
			CellTemplate cellTemplate = cellTemplateList [indexPath.Row];

			switch (cellTemplate.Size)
			{
			case (CardCollectionCell.CellSizes.ShortNarrow):
					cellKey = CardCollectionCell.ShortNarrowCellKey;
					break;
			case (CardCollectionCell.CellSizes.ShortWide):
					cellKey = CardCollectionCell.ShortWideCellKey;
					break;
			case (CardCollectionCell.CellSizes.TallNarrow):
					cellKey = CardCollectionCell.TallNarrowCellKey;
					break;
			case (CardCollectionCell.CellSizes.TallWide):
					cellKey = CardCollectionCell.TallWideCellKey;
					break;
			default:
					cellKey = CardCollectionCell.TallWideCellKey;
					break;
			}

			var cell = collectionView.DequeueReusableCell (cellKey, indexPath) as CardCollectionCell;

			if (cellTemplate.ImageColorHex == null || cellTemplate.ImageColorHex == string.Empty)
				cellTemplate.ImageColorHex = "E0E0E0"; // Default to light gray
				
			cell.SetValues (cellTemplate.Id, cellTemplate.ObjectToDisplay, cellTemplate.OnSelected, cellTemplate.Size, cellTemplate.Title, cellTemplate.ImageColorHex, cellTemplate.ImageId, cellTemplate.LocalImage);

			return cell;
		}
			
		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return this.cellTemplateList.Count;
		}

		public virtual bool IsCellWide(int section, int row)
		{
			return CardCollectionCell.IsCellSizeWide(this.cellTemplateList[row].Size);
		}
	
		#region Cell & header templates

		// NOTE:
		// We want to put everything we need to display the cell in the template ...
		// ... because we store the templates in memory. We don't want to figure out 
		// the values that we need to display everytime we're loading a cell with new data.

		protected class CellTemplate
		{
			public int Id;
			public object ObjectToDisplay;
			public Action<object> OnSelected { get; set; }
			public CardCollectionCell.CellSizes Size { get; set; }
			public string Title { get; set; }
			public string ImageId { get; set; }
			public string ImageColorHex { get; set; }
			public bool LocalImage { get; set; }			// Should we get the image for this cell locally or through a WS call?
		}

		protected class HeaderTemplate
		{
			public int Id;
			public object ObjectToDisplay;
			public Action<object> OnSelected { get; set; }
			public bool ShowTitle { get; set; }
			public string Title { get; set; }
			public string ImageId { get; set; }
			public bool LocalImage { get; set; }			// Should we get the image for this cell locally or through a WS call?
		}

		#endregion
	}
}

