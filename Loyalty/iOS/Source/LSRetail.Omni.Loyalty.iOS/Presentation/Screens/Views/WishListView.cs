using System;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation
{
    public class WishListView : BaseView
	{
		private UITableView itemTableView;
		private UIView footerContainerView;
		private UIButton btnAddWishListToBasket;
		private UIView noDataView;
		private UILabel noDataText;

		private const float footerContainerViewHeight = 50f;
		private const float labelHeight = 20;

		public delegate Task AddItemToBasketEventHandler(OneListItem itemToAdd);
		public AddItemToBasketEventHandler AddItemToBasket;

		public delegate void ItemSelectedEventHandler(OneListItem item);
		public ItemSelectedEventHandler ItemSelected;

		public delegate void RemoveItemFromWishListEventHandler(int itemPosition);
		public RemoveItemFromWishListEventHandler RemoveItemFromWishList;

		public delegate void OnAddWishListToBasketEventHandle ();
		public OnAddWishListToBasketEventHandle OnAddWishListToBasket;

		public WishListView ()
		{
			itemTableView = new UITableView ();
			itemTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			itemTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			itemTableView.Tag = 100;

			footerContainerView = new UIView();
			footerContainerView.BackgroundColor = Utils.AppColors.TransparentWhite;
			footerContainerView.Tag = 300;

			btnAddWishListToBasket = new UIButton();
			btnAddWishListToBasket.SetTitle(LocalizationUtilities.LocalizedString("WishList_AddToBasket", "Add wish list to basket"), UIControlState.Normal);
			btnAddWishListToBasket.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			btnAddWishListToBasket.BackgroundColor = UIColor.Clear;
			btnAddWishListToBasket.TouchUpInside += (object sender, EventArgs e) => {
				this.btnAddWishListToBasket.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
				if (this.OnAddWishListToBasket != null) 
				{
					this.OnAddWishListToBasket ();
				}
			};

			noDataView = new UIView ();
			noDataView.BackgroundColor = UIColor.Clear;
			noDataView.Hidden = true;

			noDataText = new UILabel();
			noDataText.TextColor = UIColor.Gray;
			noDataText.TextAlignment = UITextAlignment.Center;
			noDataText.Font = UIFont.SystemFontOfSize(14);
		
			AddSubview(this.itemTableView);
			AddSubview(this.footerContainerView);
			footerContainerView.AddSubview(this.btnAddWishListToBasket);
			AddSubview(this.noDataView);
			noDataView.AddSubview (this.noDataText);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.itemTableView.Frame = new CGRect (
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);

			this.footerContainerView.Frame = new CGRect (
				this.itemTableView.Frame.X,
				this.itemTableView.Frame.Bottom - footerContainerViewHeight,
				this.itemTableView.Frame.Width,
				footerContainerViewHeight
			);

			this.btnAddWishListToBasket.Frame = new CGRect (
				this.footerContainerView.Frame.X,
				this.footerContainerView.Frame.Y,
				this.footerContainerView.Frame.Width,
				this.footerContainerView.Frame.Height
			);

			this.noDataView.Frame = new CGRect (
				0,
				0,
				this.Frame.Width,
				this.Frame.Height
			);

			this.noDataText.Frame = new CGRect (
				0, 
				this.noDataView.Frame.Height/2 - labelHeight/2,
				this.noDataView.Frame.Width,
				labelHeight
			);
		}

		public void UpdateData ()
		{
			this.itemTableView.Source = new WishListTableSource ();
            (this.itemTableView.Source as WishListTableSource).AddItemToBasket += async wishListItem =>
            {
                if (this.AddItemToBasket != null)
                {
                    await this.AddItemToBasket(wishListItem);
                }
            };
			(this.itemTableView.Source as WishListTableSource).ItemSelected += (item) => {
				if(this.ItemSelected != null)
				{
					this.ItemSelected (item);
				}
			};
			(this.itemTableView.Source as WishListTableSource).RemoveItemFromWishList += (itemPosition) => {
				if (this.RemoveItemFromWishList != null)
				{
					this.RemoveItemFromWishList (itemPosition);
				}
			};
			if (AppData.UserLoggedIn)
				this.itemTableView.ReloadData ();

			RefreshWithAnimation ();
		}

		public void RefreshWithAnimation ()
		{
			if (this.itemTableView.Source == null)
				return;

			CATransition transition = new CATransition ();
			transition.Duration = 0.3;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			transition.Type = CATransition.TransitionPush;
			transition.Subtype = CATransition.TransitionFade;
			transition.FillMode = CAFillMode.Both;

			this.itemTableView.Layer.AddAnimation (transition, null);

			this.itemTableView.ReloadData();

			RefreshNoDataView();
		}

		#region No data view

		private void RefreshNoDataView()
		{
			if (!AppData.UserLoggedIn)
				ShowNoDataView(LocalizationUtilities.LocalizedString("WishList_Login", "Please log in to add items to your wish list."));
			else if (!(this.itemTableView.Source as WishListTableSource).HasData)
				ShowNoDataView(LocalizationUtilities.LocalizedString("WishList_NoItems", "No items in the wish list."));
			else
				HideNoDataView();
		}

		private void ShowNoDataView(string displayText)
		{
			this.noDataText.Text = displayText;
			this.noDataView.Hidden = false;
			this.itemTableView.Hidden = true;
			this.footerContainerView.Hidden = true;
		}

		private void HideNoDataView ()
		{
			this.noDataView.Hidden = true;
			if (EnabledItems.HasBasket)
				this.footerContainerView.Hidden = false;
			
			this.itemTableView.Hidden = false;
		}
		#endregion
	}
}

