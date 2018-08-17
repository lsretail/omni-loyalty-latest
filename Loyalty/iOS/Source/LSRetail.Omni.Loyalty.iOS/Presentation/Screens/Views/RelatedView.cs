using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class RelatedView : UIView
	{
		UITableView tbl;

		public delegate void SelectedEventHandler (string id);
		public event SelectedEventHandler PublishedOfferSelected;
		public event SelectedEventHandler ItemSelected;

		public RelatedView ()
		{
			this.BackgroundColor = UIColor.White;

			this.tbl = new UITableView ();
			this.tbl.BackgroundColor = UIColor.Clear;
			this.tbl.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;

			this.AddSubview (this.tbl);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.tbl.Frame = new CGRect (
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);
		}

		public void UpdateData (List<LoyItem> items) 
		{
			this.tbl.Source = new RelatedTableSource (RelatedTableSource.RelatedType.item);
			(this.tbl.Source as RelatedTableSource).ItemPressed += (string id) => {
				if (this.ItemSelected != null)
					this.ItemSelected (id);
			};
			(this.tbl.Source as RelatedTableSource).Items = items;
			this.tbl.ReloadData ();
		}

		public void UpdateData (List<PublishedOffer> offers) 
		{
			this.tbl.Source = new RelatedTableSource (RelatedTableSource.RelatedType.offer);
			(this.tbl.Source as RelatedTableSource).OfferPressed += (string id) => {
				if (this.PublishedOfferSelected != null)
					this.PublishedOfferSelected (id);
			};
			(this.tbl.Source as RelatedTableSource).Offers = offers;
			this.tbl.ReloadData ();
		}
	}
}

