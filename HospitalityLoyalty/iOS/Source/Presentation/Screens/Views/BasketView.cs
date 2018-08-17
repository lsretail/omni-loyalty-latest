using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using UIKit;

namespace Presentation.Screens
{
	public class BasketView : BaseView
	{
		public UITableView tblBasket;
		private UIRefreshControl refreshControlBasket;
		private BasketFooterView footerView;
		private NoDataView noDataView;

		private readonly IBasketListeners listener;

		public enum BasketType
		{
			Item,
			Offer
		}

		public delegate void RefreshBasketEventHandler();
		public event RefreshBasketEventHandler refreshBasket;

		public BasketView(IBasketListeners listener)
		{
			this.listener = listener;
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			this.tblBasket = new UITableView();
			this.tblBasket.BackgroundColor = Presentation.Utils.AppColors.BackgroundGray;
			this.tblBasket.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.tblBasket.Source = new BasketTableSource(this.listener);
			this.AddSubview(this.tblBasket);

			this.refreshControlBasket = new UIRefreshControl();
			this.refreshControlBasket.ValueChanged += (object sender, EventArgs e) =>
			{
				if (this.refreshBasket != null)
					this.refreshBasket();
			};
			this.tblBasket.AddSubview(this.refreshControlBasket);

			this.footerView = new BasketFooterView(this.listener);
			this.AddSubview(this.footerView);

			this.noDataView = new NoDataView();
			this.noDataView.TextToDisplay = LocalizationUtilities.LocalizedString("Basket_BasketEmpty", "Your basket is empty");
			this.AddSubview(this.noDataView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.footerView.Frame = new CGRect(
				0,
				this.Frame.Height - this.BottomLayoutGuideLength - BasketFooterView.HEIGHT,
				this.Frame.Width,
				BasketFooterView.HEIGHT
			);

			this.tblBasket.Frame = new CGRect(
				0,
				0,
				this.Frame.Width,
				this.Frame.Height - this.BottomLayoutGuideLength - this.footerView.Frame.Height
			);

			this.noDataView.TopLayoutGuideLength = this.TopLayoutGuideLength;
			this.noDataView.BottomLayoutGuideLength = this.BottomLayoutGuideLength;
			this.noDataView.Frame = new CGRect(
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);
		}

		public void Refresh(string formattedTotalString)
		{
			this.tblBasket.ReloadData();
			this.footerView.FormattedTotalString = formattedTotalString;

			if ((this.tblBasket.Source as BasketTableSource).HasData)
				this.noDataView.Hidden = true;
			else
				this.noDataView.Hidden = false;

			if (!this.refreshControlBasket.Hidden)
				this.refreshControlBasket.EndRefreshing();
		}

		public interface IBasketListeners
		{
			void RemoveItemFromBasket(int index, BasketType basketType);
			bool ToggleFavorite(int index, BasketType type);
			bool IsFavorite(int index);
			void ItemClicked(int index);
			void ChekoutButtonClicked();
		}

		#region FooterView
		private class BasketFooterView : BaseView
		{
			private UIView separatorView;
			private UILabel lblTotal;
			private UIButton btnCheckout;
			//private IBasketListeners listeners;

			public const float HEIGHT = 120f;

			public string FormattedTotalString
			{
				set
				{
					this.lblTotal.Text = LocalizationUtilities.LocalizedString("Basket_Total", "Total") + ": " + value;
				}
			}

			public BasketFooterView(IBasketListeners listeners)
			{
				//this.listeners = listeners;
				this.BackgroundColor = UIColor.White;

				this.separatorView = new UIView();
				this.separatorView.BackgroundColor = Presentation.Utils.AppColors.PrimaryColor;
				this.AddSubview(this.separatorView);

				this.lblTotal = new UILabel();
				this.lblTotal.BackgroundColor = UIColor.Clear;
				this.lblTotal.TextAlignment = UITextAlignment.Right;
				this.AddSubview(this.lblTotal);

				this.btnCheckout = new UIButton();
				this.btnCheckout.BackgroundColor = Presentation.Utils.AppColors.PrimaryColor;
				this.btnCheckout.SetTitle(LocalizationUtilities.LocalizedString("Basket_Checkout", "Checkout"), UIControlState.Normal);
				this.btnCheckout.SetTitleColor(UIColor.White, UIControlState.Normal);
				this.btnCheckout.Layer.CornerRadius = 3f;
				this.btnCheckout.TouchUpInside += (object sender, EventArgs e) => { listeners.ChekoutButtonClicked(); };
				this.AddSubview(this.btnCheckout);
			}

			public override void LayoutSubviews()
			{
				base.LayoutSubviews();

				this.separatorView.Frame = new CGRect(0, 0, this.Frame.Width, 3f);
				this.lblTotal.Frame = new CGRect(10f, this.separatorView.Frame.Bottom, this.Frame.Width - 2 * 10f, 40f);
				this.btnCheckout.Frame = new CGRect(44f, this.lblTotal.Frame.Bottom + 10f, this.Frame.Width - 2 * 44f, 44f);
			}
		}
		#endregion
	}
}