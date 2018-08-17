using System;
using CoreAnimation;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class CheckoutOrderOverView : BaseView
	{
		private UITableView orderOverviewTableView;
		private CheckoutOrderOverViewTableSource orderOverviewTableSource;

		private readonly ICheckoutOrderOverViewListener listener;

		public CheckoutOrderOverView(ICheckoutOrderOverViewListener listener)
		{
			this.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.listener = listener;

			this.orderOverviewTableView = new UITableView();
			this.orderOverviewTableView.BackgroundColor = UIColor.Clear;
			this.orderOverviewTableView.AlwaysBounceVertical = true;
			this.orderOverviewTableView.ShowsVerticalScrollIndicator = false;
			this.orderOverviewTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.orderOverviewTableSource = new CheckoutOrderOverViewTableSource(this.listener);
			this.orderOverviewTableView.Source = this.orderOverviewTableSource;
			this.orderOverviewTableView.TranslatesAutoresizingMaskIntoConstraints = false;
			this.AddSubview(this.orderOverviewTableView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.orderOverviewTableView.Frame = new CGRect(
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);

		}

		public void RefreshData(bool withAnimation = false)
		{
			if(withAnimation)
			{
				CATransition transition = new CATransition ();
				transition.Duration = 0.3;
				transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				transition.Type = CATransition.TransitionPush;
				transition.Subtype = CATransition.TransitionFade;
				transition.FillMode = CAFillMode.Both;

				this.orderOverviewTableView.Layer.AddAnimation (transition, null);
			}

			this.orderOverviewTableSource.RefreshTotalLabel();
			this.orderOverviewTableView.ReloadData();
		}

		public interface ICheckoutOrderOverViewListener
		{
			void SendOrder();
			void RemoveBasketItemPressed(CheckoutOrderOverViewCell.CellType cellType, int id);
			void BasketItemPressed(BasketItem item);
			void BasketItemToggleFavorite(int index);
			bool BasketItemCheckIfFavorite(int index);
		}
	}
}

