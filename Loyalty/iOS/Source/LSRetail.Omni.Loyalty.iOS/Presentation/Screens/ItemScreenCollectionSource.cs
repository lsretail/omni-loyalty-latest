using System;
using Foundation;
using UIKit;
using Presentation.Utils;
using System.Linq;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
    public class ItemScreenCollectionSource : CardCollectionSource
	{
		private ItemScreen controller;

		public ItemScreenCollectionSource (ItemScreen controller)
		{
			this.controller = controller;

			BuildHeaderTemplates();
			BuildCellTemplates();
		}
			
		public override void BuildCellTemplates()
		{
			ItemScreenCellTemplate cellTemplate;

			int id = 1;

			if(controller.itemListType == ItemScreen.ItemListType.Category)
			{
				foreach(ItemCategory itemCategory in AppData.ItemCategories)
				{
					cellTemplate = new ItemScreenCellTemplate ();
					cellTemplate.Id = id;

					cellTemplate.Size = this.controller.CellSize;
                    cellTemplate.ImageId = (itemCategory.Images != null && itemCategory.Images.Count() > 0) ? itemCategory.Images[0].Id : string.Empty;
					cellTemplate.LocalImage = false;
                    cellTemplate.ImageColorHex = (itemCategory.Images != null && itemCategory.Images.Count() > 0) ? itemCategory.Images[0].AvgColor : string.Empty;
					cellTemplate.Title = itemCategory.Description;
					cellTemplate.FormattedPrice = "";
					cellTemplate.ObjectToDisplay = itemCategory;

					cellTemplate.OnSelected = (x) => {
						controller.CellSelected (x);
					}; 

					// Can't add to basket.
					cellTemplate.OnAddToBasketButtonPressed = (x) => {
						// Do nothing
					};

					this.cellTemplateList.Add (cellTemplate);
					id++;
				}
			}
			else if(controller.itemListType == ItemScreen.ItemListType.Group)
			{
				foreach(ProductGroup productGroup in controller.itemCategory.ProductGroups)
				{
					cellTemplate = new ItemScreenCellTemplate ();
					cellTemplate.Id = id;

					cellTemplate.Size = this.controller.CellSize;
                    cellTemplate.ImageId = productGroup.Images != null && productGroup.Images.Count > 0 ? productGroup.Images[0].Id : string.Empty;
					cellTemplate.LocalImage = false;
					cellTemplate.ImageColorHex = productGroup.Images != null && productGroup.Images.Count > 0 ? productGroup.Images[0].AvgColor : string.Empty;
					cellTemplate.Title = productGroup.Description;
					cellTemplate.FormattedPrice = "";
					cellTemplate.ObjectToDisplay = productGroup;

					cellTemplate.OnSelected = (x) => {
						controller.CellSelected (x);
					}; 

					// Can't add to basket.
					cellTemplate.OnAddToBasketButtonPressed = (x) => {
						// Do nothing
					};

					this.cellTemplateList.Add (cellTemplate);
					id++;
				}
			}
			else
			{
				foreach(LoyItem item in controller.productGroup.Items)
				{
					cellTemplate = new ItemScreenCellTemplate ();
					cellTemplate.Id = id;

					ImageView imgView = item.Images.FirstOrDefault();
					cellTemplate.Size = this.controller.CellSize;
					cellTemplate.ImageId = imgView != null ? imgView.Id : string.Empty;
					cellTemplate.LocalImage = false;
					cellTemplate.ImageColorHex = imgView != null ? imgView.AvgColor : string.Empty;
					cellTemplate.Title = item.Description;
					cellTemplate.FormattedPrice = item.Price;
					cellTemplate.ObjectToDisplay = item;

					cellTemplate.OnSelected = (x) => {
						controller.CellSelected (x);
					}; 

					// Can't add to basket.
					cellTemplate.OnAddToBasketButtonPressed = (x) => {
						// Do nothing
					};

					this.cellTemplateList.Add (cellTemplate);
					id++;
				}
			}
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			NSString cellKey;
			ItemScreenCellTemplate cellTemplate = cellTemplateList [indexPath.Row] as ItemScreenCellTemplate;

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

			var cell = collectionView.DequeueReusableCell (cellKey, indexPath) as ItemScreenCell;

			if (cellTemplate.ImageColorHex == null || cellTemplate.ImageColorHex == string.Empty)
				cellTemplate.ImageColorHex = "E0E0E0"; // Default to light gray
				
			cell.SetValues (
				cellTemplate.Id,
				cellTemplate.ObjectToDisplay,
				cellTemplate.OnSelected,
				cellTemplate.Size, 
				cellTemplate.Title, 
				cellTemplate.ImageColorHex, 
				cellTemplate.ImageId, 
				cellTemplate.LocalImage, 
				cellTemplate.FormattedPrice, 
				cellTemplate.OnAddToBasketButtonPressed
			);

			if ((indexPath.Row + 4) % 10 == 0 && this.controller.productGroup != null) 
			{
				System.Diagnostics.Debug.WriteLine ("this is the index path number:" + indexPath.Row);
				this.controller.LazyLoadItemsByProductGroup (indexPath.Row);
			}

			return cell;
		}



		public override void BuildHeaderTemplates()
		{
			// Don't do anything here if you don't want a header

			/*this.headerTemplateList = new List<HeaderTemplate> ();

			HeaderTemplate headerTemplate = new HeaderTemplate ();
			headerTemplate.Id = 1;
			headerTemplate.ShowTitle = true;
			headerTemplate.Title = "Offertext";
			headerTemplate.ImageFileName = "Vinafata.png";
			headerTemplate.onSelected =  (x) => {
				controller.HeaderSelected (x);
			};

			this.headerTemplateList.Add (headerTemplate);*/
		}
			
		public void RefreshCellTemplates()
		{
			this.cellTemplateList.Clear ();
			BuildCellTemplates ();
		}

		public void RefreshHeaderTemplates()
		{
			this.headerTemplateList.Clear ();
			BuildHeaderTemplates ();
		}





		private class ItemScreenCellTemplate : CellTemplate
		{
			public string FormattedPrice { get; set; }
			public Action<LoyItem> OnAddToBasketButtonPressed { get; set; }
		}
	}
}

