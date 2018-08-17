using System;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class ModifiersView : BaseView
	{
		protected UITableView TableView;
		private UIView itemView;
		private UIView changeQtyView;
		private UIView noModifiersView;
		private UIView overlayView;

		private UIImageView itemViewImage;
		private UILabel lblText;
		private UILabel lblPrice;
		private UILabel lblChangeQty;
		private UIButton btnPlusQty;
		private UILabel lblItemQuantity;
		private UIButton btnMinusQty;
		private UILabel noModifiersText;

		private IModifiersListeners listeners;

		public ModifiersView(IModifiersListeners listeners)
		{
			this.listeners = listeners;
			this.TableView = new UITableView(new CGRect(), UITableViewStyle.Grouped);
			this.TableView.BackgroundColor = AppColors.BackgroundGray;

			this.itemView = new UIView();
			this.AddSubview(this.itemView);

			this.itemViewImage = new UIImageView();
			this.itemViewImage.ClipsToBounds = true;
			this.itemViewImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.itemViewImage.BackgroundColor = UIColor.White;
			this.itemView.AddSubview(itemViewImage);

			this.overlayView = new UIView();
			this.overlayView.BackgroundColor = AppColors.TransparentBlack;
			this.itemView.AddSubview(overlayView);

			this.lblText = new UILabel();
			this.lblText.TextColor = UIColor.White;
			this.lblText.Font = UIFont.FromName("Helvetica", 14);
			this.lblText.TextAlignment = UITextAlignment.Left;
			this.itemView.AddSubview(lblText);

			this.lblPrice = new UILabel();
			this.lblPrice.TextColor = UIColor.White;
			this.lblPrice.Font = UIFont.FromName("Helvetica", 14);
			this.lblPrice.TextAlignment = UITextAlignment.Right;
			this.itemView.AddSubview(lblPrice);

			// Change quantity view
			this.changeQtyView = new UIView();
			this.changeQtyView.BackgroundColor = AppColors.PrimaryColor;
			this.AddSubview(this.changeQtyView);

			this.lblChangeQty = new UILabel();
			this.lblChangeQty.Text = LocalizationUtilities.LocalizedString("Modifiers_Quantity", "Quantity") + ":";
			this.lblChangeQty.TextColor = UIColor.White;
			this.changeQtyView.AddSubview(lblChangeQty);

			this.btnPlusQty = new UIButton();
			this.btnPlusQty.SetTitle("+", UIControlState.Normal);
			this.btnPlusQty.SetTitleColor(UIColor.White, UIControlState.Normal);
			this.btnPlusQty.BackgroundColor = UIColor.Clear;
			this.btnPlusQty.TouchUpInside += (object sender, EventArgs e) => { this.listeners.IncreaseBasketItemQuantity(); };
			this.changeQtyView.AddSubview(btnPlusQty);

			this.lblItemQuantity = new UILabel();
			this.lblItemQuantity.TextColor = UIColor.White;
			this.lblItemQuantity.Font = UIFont.SystemFontOfSize(14);
			this.lblItemQuantity.TextAlignment = UITextAlignment.Center;
			this.changeQtyView.AddSubview(lblItemQuantity);

			this.btnMinusQty = new UIButton();
			this.btnMinusQty.SetTitle("-", UIControlState.Normal);
			this.btnMinusQty.SetTitleColor(UIColor.White, UIControlState.Normal);
			this.btnMinusQty.BackgroundColor = UIColor.Clear;
			this.btnMinusQty.TouchUpInside += (object sender, EventArgs e) => { this.listeners.DecreaseBasketItemQuantity(); };
			this.changeQtyView.AddSubview(btnMinusQty);

			// Tableview
			//this.TableView.BackgroundColor = AppColors.BackgroundGray;
			this.AddSubview(this.TableView);

			// No modifiers view
			this.noModifiersView = new UIView();
			this.noModifiersView.BackgroundColor = UIColor.Clear;

			this.noModifiersText = new UILabel();
		    this.noModifiersText.Text = LocalizationUtilities.LocalizedString("Modifiers_NoModifiersAvailable", "No modifications available");
			this.noModifiersText.TextColor = UIColor.Gray;
			this.noModifiersText.TextAlignment = UITextAlignment.Center;
			this.noModifiersText.Font = UIFont.SystemFontOfSize(14);
			this.noModifiersView.AddSubview(noModifiersText);
			this.AddSubview(this.noModifiersView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			float textLabelMargin = 5f;
			float priceLabelWidth = 105f;
			float changeQtyViewHeight = 40f;
			float changeQtyViewMargin = 10f;
			float changeQtyViewButtonWidth = 50f;

			this.itemView.Frame = new CGRect(
				0, 
				this.TopLayoutGuideLength,
				this.Frame.Width, 
				80f
			);

			this.itemViewImage.Frame = this.itemView.Frame;
			int overlayViewHeight = (int)Math.Floor(this.itemViewImage.Frame.Height / 3);

			this.overlayView.Frame = new CGRect(
				this.itemViewImage.Frame.X,
				this.itemViewImage.Frame.Height - overlayViewHeight,
				itemViewImage.Frame.Width,
				overlayViewHeight
			);
			this.lblText.Frame = new CGRect(
				overlayView.Frame.X + textLabelMargin, 
				overlayView.Frame.Y,
				overlayView.Frame.Width - 2 * textLabelMargin - priceLabelWidth, 
				overlayView.Frame.Height
			);
			this.lblPrice.Frame = new CGRect(
				lblText.Frame.Right,
				overlayView.Frame.Y, 
				priceLabelWidth, 
				overlayView.Frame.Height
			);

			// Change quantity view

			this.changeQtyView.Frame = new CGRect(
				0, 
				this.Frame.Height - changeQtyViewHeight,
				this.Frame.Width, 
				changeQtyViewHeight
			);

			this.lblChangeQty.Frame = new CGRect(
				changeQtyViewMargin, 
				0f,
				80f,
				this.changeQtyView.Frame.Height
			);

			this.btnPlusQty.Frame = new CGRect(
				this.changeQtyView.Frame.Width - changeQtyViewButtonWidth, 
				0, 
				changeQtyViewButtonWidth, 
				this.changeQtyView.Frame.Height
			);
			this.lblItemQuantity.Frame = new CGRect(
				btnPlusQty.Frame.Left - 40f,
				0, 
				40f, 
				this.changeQtyView.Frame.Height
			);
			this.btnMinusQty.Frame = new CGRect(
				lblItemQuantity.Frame.Left - changeQtyViewButtonWidth, 
				0, 
				changeQtyViewButtonWidth, 
				this.changeQtyView.Frame.Height
			);

			// Tableview
			this.TableView.Frame = new CGRect(
				0, 
				this.itemView.Frame.Bottom, 
				this.Frame.Width, 
				this.Bounds.Bottom - this.BottomLayoutGuideLength - this.itemView.Frame.Bottom - this.changeQtyView.Frame.Height
			);

			// No modifiers view
			this.noModifiersView.Frame = this.TableView.Frame;
			this.noModifiersText.Frame = new CGRect(
				0,
				this.noModifiersView.Frame.Height / 2 - 20 / 2,
				this.noModifiersView.Frame.Width, 
				20
			);
		}

		public void UpdateData (MenuItem menuItem, decimal basketItemQuantity,bool onlyShowRequiredModifiers, bool showAddToBasketBtn)
		{
			ModifiersTableSource modifiersTableSource = new ModifiersTableSource(this.listeners, menuItem, onlyShowRequiredModifiers, showAddToBasketBtn);

			this.TableView.Source = modifiersTableSource;

			ImageView imgView = menuItem.Images.FirstOrDefault();
			if (imgView != null)
			{
				this.itemViewImage.BackgroundColor = Utils.UI.GetUIColorFromHexString(imgView.AvgColor);
				this.listeners.LoadImageToItemImageView(imgView.Id, (imageView) => {
					UIImage image = Image.FromBase64(imageView.Image);
					this.itemViewImage.Image = image;

					CATransition transition = new CATransition();
					transition.Duration = 0.5f;
					transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
					transition.Type = CATransition.TransitionFade;
					itemViewImage.Layer.AddAnimation(transition, null);
				});
			}

			this.lblItemQuantity.Text = basketItemQuantity.ToString();
			this.lblText.Text = menuItem.Description;
			this.lblPrice.Text = AppData.MobileMenu.Currency.FormatDecimal(listeners.GetFullPrice(menuItem));
		}

		public void RefreshTableView()
		{
			(this.TableView.Source as ModifiersTableSource).RefreshTableData();
			this.TableView.ReloadData();
		}

		public void RefreshPriceLabel(string price)
		{
			(this.TableView.Source as ModifiersTableSource).RefreshCellDecorations();
			this.lblPrice.Text = price;
		}

		public void RefreshBasketItemQuantityLabel(string qty)
		{
			this.lblItemQuantity.Text = qty;
		}

		public void ToggleNoModifiersView()
		{
			if ((this.TableView.Source as ModifiersTableSource).ContainsData)
				HideNoModifiersView();
			else
				ShowNoModifiersView();
		}

		private void ShowNoModifiersView()
		{
			if (this.noModifiersView != null)
				this.noModifiersView.Hidden = false;
		}

		private void HideNoModifiersView()
		{
			if (this.noModifiersView != null)
				this.noModifiersView.Hidden = true;
		}

		public interface IModifiersListeners
		{
			void LoadImageToItemImageView(string imageId, Action<ImageView> onSuccess);
			void ChangeObjectQuantity(int newQuantity, object myObject, object myObjectGroup = null);
			void AddToBasketButtonClicked();
			void IncreaseBasketItemQuantity();
			void DecreaseBasketItemQuantity();
			decimal GetFullPrice(MenuItem item);
		}
	}
}

