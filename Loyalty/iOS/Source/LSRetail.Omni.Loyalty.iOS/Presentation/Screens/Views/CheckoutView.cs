using System;
using UIKit;
using CoreGraphics;

namespace Presentation
{
	public class CheckoutView : BaseView
	{
		private UIView sendOrderContainerView;
		private UITableView transactionOverviewTableView;

		public delegate void ProceedToShippingMethodsEventHandler ();
		public event ProceedToShippingMethodsEventHandler ProceedToShippingMethods;

		public CheckoutView ()
		{
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			this.sendOrderContainerView = new UIView();
			this.sendOrderContainerView.BackgroundColor = UIColor.Clear;
			this.sendOrderContainerView.TranslatesAutoresizingMaskIntoConstraints = false;

			this.transactionOverviewTableView = new UITableView();
			this.transactionOverviewTableView.BackgroundColor = UIColor.Clear;
			this.transactionOverviewTableView.AlwaysBounceVertical = true;
			this.transactionOverviewTableView.ShowsVerticalScrollIndicator = false;
			this.transactionOverviewTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.transactionOverviewTableView.Source = new CheckoutDetailsTableSource( () => {
				if (this.ProceedToShippingMethods != null)
				{
					this.ProceedToShippingMethods ();
				}	
			});
			this.transactionOverviewTableView.TranslatesAutoresizingMaskIntoConstraints = false;

			this.AddSubview(this.sendOrderContainerView);
			this.sendOrderContainerView.AddSubview(this.transactionOverviewTableView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.sendOrderContainerView.Frame = new CGRect (
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);

			this.transactionOverviewTableView.Frame = new CGRect (
				0,
				0,
				this.sendOrderContainerView.Frame.Width,
				this.sendOrderContainerView.Frame.Height
			);
		}
	}
}

