using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.Linq;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class RelatedTableSource : UITableViewSource
	{
		public enum RelatedType {
			item = 0,
			offer = 1
		};
				
		private List<LoyItem> items;
		private List<PublishedOffer> offers; 

		public List<LoyItem> Items { set { this.items = value; } }
		public List<PublishedOffer> Offers  { set { this.offers = value; } }

		private RelatedType relatedType;

		private const int NUMBER_OF_TOP_ITEMS = 4;
		private const float HEADER_HEIGHT = 25f;

		public delegate void PressedEventHandler(string id);
		public event PressedEventHandler ItemPressed;
		public event PressedEventHandler OfferPressed;

		public RelatedTableSource (RelatedType relatedType)
		{
			this.relatedType = relatedType;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{	
			switch (this.relatedType) {
				case RelatedType.item:
					return items.Count;
				case RelatedType.offer:
					return offers.Count;
				default:
					return 1;
			}
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return RelatedCell.CellHeight;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			switch (this.relatedType) {
				case RelatedType.item: {
					var item = this.items [indexPath.Row];
					if (ItemPressed != null) {
						ItemPressed (item.Id);
					}
					break;
					}
				case RelatedType.offer: {
					var offer = this.offers [indexPath.Row];
					if (OfferPressed != null) {
						OfferPressed (offer.Id);
					}
					break;
					}
				default:
					break;
			}

			tableView.DeselectRow(indexPath, true);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			RelatedCell cell = tableView.DequeueReusableCell(RelatedCell.KEY) as RelatedCell;
			if (cell == null)
				cell = new RelatedCell();

			switch (this.relatedType) {
				case RelatedType.item: {
					var item = this.items [indexPath.Row];
					var imageView = item.Images.FirstOrDefault();
					if (imageView != null)
						cell.SetValues(item.Description, imageView.AvgColor, imageView.Id);
					else
						cell.SetValues(item.Description, string.Empty, string.Empty);
					break;
				}
				case RelatedType.offer: {
					var offer = this.offers [indexPath.Row];
					var imageView = offer.Images.FirstOrDefault();
					if (imageView != null)
						cell.SetValues(offer.Description, imageView.AvgColor, imageView.Id);
					else
						cell.SetValues(offer.Description, string.Empty, string.Empty);
					break;
				}
				default:
					break;
			}

			return cell;
		}
	}
}

