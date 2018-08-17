using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;
using Domain.Menus;
using System.Linq;
using System.Collections.Generic;
using Presentation.Models;
using Presentation.Utils;
using Domain.Images;

namespace Presentation.Screens
{
	/*
	public class ModifiersScreen : UIViewController
	{
		protected UITableView TableView;
		private UIView itemView;
		private UIView changeQtyView;
		private UIView noModifiersView;

		private MenuItem menuItemToModify;
		public MenuItem MenuItem { get { return this.menuItemToModify; } }

		protected decimal basketItemQuantity;

		public delegate void AddToBasketEventHandler();
		public event AddToBasketEventHandler OnAddToBasketPressed;

		public ModifiersScreen (MenuItem menuItem, bool onlyShowRequiredModifiers, bool showAddToBasketBtn)
		{
			this.menuItemToModify = menuItem;	// Note, this should be a deep clone of the menuitem that got us this far, we can manipulate it before adding it to basket
			this.TableView = new UITableView (new CGRect (), UITableViewStyle.Grouped);

			ModifiersScreenTableSource modifiersScreenTableSource = new ModifiersScreenTableSource (null, onlyShowRequiredModifiers, showAddToBasketBtn);

			if(showAddToBasketBtn)
			{
				modifiersScreenTableSource.OnAddToBasketPressed += () =>
				{
					if(this.OnAddToBasketPressed != null)
						this.OnAddToBasketPressed();
				};
			}

			this.TableView.Source = modifiersScreenTableSource;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Itemview

			this.itemView = new UIView();
			this.View.AddSubview(this.itemView);

			UIImageView itemViewImage = new UIImageView();
			itemViewImage.ClipsToBounds = true;
			itemViewImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			itemViewImage.BackgroundColor = UIColor.White;
			itemViewImage.Tag = 100;
			this.itemView.AddSubview(itemViewImage);
			Domain.Images.ImageView imgView = this.MenuItem.Images.FirstOrDefault();
			if (imgView != null)
			{
				itemViewImage.BackgroundColor = Utils.UI.GetUIColorFromHexString (imgView.AvgColor);
				//LoadImageToItemImageView(imgView.Id);
			}

			UIView overlayView = new UIView();
			overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
			overlayView.Tag = 200;
			this.itemView.AddSubview(overlayView);

			UILabel lblText = new UILabel ();
			lblText.TextColor = UIColor.White;
			lblText.Font = UIFont.FromName ("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.Text = this.MenuItem.Description;
			lblText.Tag = 300;
			this.itemView.AddSubview(lblText);

			UILabel lblPrice = new UILabel();
			lblPrice.TextColor = UIColor.White;
			lblPrice.Font = UIFont.FromName ("Helvetica", 14);
			lblPrice.TextAlignment = UITextAlignment.Right;
			lblPrice.Text = AppData.MobileMenu.Currency.FormatDecimal(this.MenuItem.FullPrice);
			lblPrice.Tag = 400;
			this.itemView.AddSubview(lblPrice);

			// Change quantity view

			this.changeQtyView = new UIView();
			this.View.AddSubview(this.changeQtyView);
			this.changeQtyView.BackgroundColor = Utils.AppColors.PrimaryColor;

			UILabel lblChangeQty = new UILabel();
			lblChangeQty.Text = NSBundle.MainBundle.LocalizedString("Modifiers_Quantity", "Quantity") + ":";
			lblChangeQty.TextColor = UIColor.White;
			lblChangeQty.Tag = 100;
			this.changeQtyView.AddSubview(lblChangeQty);

			UIButton btnPlusQty = new UIButton();
			btnPlusQty.SetTitle("+", UIControlState.Normal);
			btnPlusQty.SetTitleColor(UIColor.White, UIControlState.Normal);
			btnPlusQty.BackgroundColor = UIColor.Clear;
			btnPlusQty.Tag = 200;
			btnPlusQty.TouchUpInside += (object sender, EventArgs e) => { IncreaseBasketItemQuantity(); };
			this.changeQtyView.AddSubview(btnPlusQty);

			UILabel lblItemQuantity = new UILabel();
			lblItemQuantity.Text = this.basketItemQuantity.ToString();
			lblItemQuantity.TextColor = UIColor.White;
			lblItemQuantity.Font = UIFont.SystemFontOfSize(14);
			lblItemQuantity.TextAlignment = UITextAlignment.Center;
			lblItemQuantity.Tag = 400;
			this.changeQtyView.AddSubview(lblItemQuantity);

			UIButton btnMinusQty = new UIButton();
			btnMinusQty.SetTitle("-", UIControlState.Normal);
			btnMinusQty.SetTitleColor(UIColor.White, UIControlState.Normal);
			btnMinusQty.BackgroundColor = UIColor.Clear;
			btnMinusQty.Tag = 300;
			btnMinusQty.TouchUpInside += (object sender, EventArgs e) => { DecreaseBasketItemQuantity(); };
			this.changeQtyView.AddSubview(btnMinusQty);

			// Tableview

			this.TableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.View.AddSubview (this.TableView);

			// No modifiers view

			this.noModifiersView = new UIView();
			this.noModifiersView.BackgroundColor = UIColor.Clear;

			UILabel noModifiersText = new UILabel();
			noModifiersText.Text = NSBundle.MainBundle.LocalizedString("Modifiers_NoModifiersAvailable", "No modifications available");
			noModifiersText.TextColor = UIColor.Gray;
			noModifiersText.TextAlignment = UITextAlignment.Center;
			noModifiersText.Font = UIFont.SystemFontOfSize(14);
			noModifiersText.Tag = 100;
			this.noModifiersView.AddSubview(noModifiersText);

			this.View.AddSubview(this.noModifiersView);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			ToggleNoModifiersView();
		}
			
		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			// Itemview

			this.itemView.Frame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Frame.Width, 80f);

			UIView itemViewImage = this.itemView.ViewWithTag(100);
			itemViewImage.Frame = this.itemView.Frame;

			UIView overlayView = this.itemView.ViewWithTag(200);
			int overlayViewHeight = (int)Math.Floor (itemViewImage.Frame.Height / 3);
			overlayView.Frame = new CGRect (itemViewImage.Frame.X, itemViewImage.Frame.Height - overlayViewHeight, itemViewImage.Frame.Width, overlayViewHeight);

			float textLabelMargin = 5f;
			float priceLabelWidth = 105f;

			UIView lblText = this.itemView.ViewWithTag(300);
			lblText.Frame = new CGRect(overlayView.Frame.X + textLabelMargin, overlayView.Frame.Y, overlayView.Frame.Width - 2 * textLabelMargin - priceLabelWidth, overlayView.Frame.Height);

			UIView lblPrice = this.itemView.ViewWithTag(400);
			lblPrice.Frame = new CGRect(lblText.Frame.Right, overlayView.Frame.Y, priceLabelWidth, overlayView.Frame.Height);

			// Change quantity view

			float changeQtyViewHeight = 40f;
			this.changeQtyView.Frame = new CGRect(0, this.View.Bounds.Bottom - this.BottomLayoutGuide.Length - changeQtyViewHeight, this.View.Frame.Width, changeQtyViewHeight);

			float changeQtyViewMargin = 10f;
			float changeQtyViewButtonWidth = 50f;

			UIView lblChangeQuantity = this.changeQtyView.ViewWithTag(100);
			lblChangeQuantity.Frame = new CGRect(changeQtyViewMargin, 0f, 80f, this.changeQtyView.Frame.Height);

			UIView btnPlusQty = this.changeQtyView.ViewWithTag(200);
			btnPlusQty.Frame = new CGRect(this.changeQtyView.Frame.Width - changeQtyViewButtonWidth, 0, changeQtyViewButtonWidth, this.changeQtyView.Frame.Height);

			UIView lblItemQuantity = this.changeQtyView.ViewWithTag(400);
			lblItemQuantity.Frame = new CGRect(btnPlusQty.Frame.Left - 40f, 0, 40f, this.changeQtyView.Frame.Height);

			UIView btnMinusQty = this.changeQtyView.ViewWithTag(300);
			btnMinusQty.Frame = new CGRect(lblItemQuantity.Frame.Left - changeQtyViewButtonWidth, 0, changeQtyViewButtonWidth, this.changeQtyView.Frame.Height);

			// Tableview

			this.TableView.Frame = new CGRect(0, this.itemView.Frame.Bottom, this.View.Frame.Width, this.View.Bounds.Bottom - this.BottomLayoutGuide.Length - this.itemView.Frame.Bottom - this.changeQtyView.Frame.Height);
		
			// No modifiers view

			this.noModifiersView.Frame = this.TableView.Frame;
			UIView noModifiersText = this.noModifiersView.ViewWithTag(100);
			noModifiersText.Frame = new CGRect(0, this.noModifiersView.Frame.Height/2 - 20/2, this.noModifiersView.Frame.Width, 20);
		}

		private void RefreshPriceLabel()
		{
			UILabel lblPrice = (UILabel)this.itemView.ViewWithTag(400);
			lblPrice.Text = AppData.MobileMenu.Currency.FormatDecimal(this.MenuItem.FullPrice);
		}

		private void RefreshBasketItemQuantityLabel()
		{
			UILabel lblItemQuantity = (UILabel)this.changeQtyView.ViewWithTag(400);
			lblItemQuantity.Text = this.basketItemQuantity.ToString();
		}

		private void LoadImageToItemImageView(string imageId, Action<ImageView> onSuccess)
		{
			new ImageModel().ImageGetById(imageId, new ImageSize(700, 500), 
				(x, destinationId) => {
					onSuccess(x);
				},
			  	() => { }
			);
		}

		public void ChangeObjectQuantity (int newQuantity, object myObject, object myObjectGroup = null)
		{
			if (myObject is Modifier && myObjectGroup is ModifierGroup)
			{
				//ChangeModifierAndModifierGroupQuantity (myObject as Modifier, myObjectGroup as ModifierGroup, newQuantity);
				ChangeModifierQuantity (myObject as Modifier, myObjectGroup as ModifierGroup, newQuantity);
			}
			else if (myObject is Ingredient)
			{
				ChangeIngredientQuantity (myObject as Ingredient, newQuantity);
			} 
			else if (myObject is DealLineItem && myObjectGroup is DealLine)
			{
				SelectDealLineItem(myObject as DealLineItem, myObjectGroup as DealLine);
				RefreshTableView();
			}

			(this.TableView.Source as ModifiersScreenTableSource).RefreshCellDecorations ();
			RefreshPriceLabel();
		}

		private void SelectDealLineItem(DealLineItem item, DealLine dealLine)
		{
			// TODO ?
			// Note: There is no way to skip a deal line item (i.e. select no item) with the current implementation (or select more than one)
			// Should change the domain model, give deal line items a quantity and use that instead?

			dealLine.SelectedId = item.DealLineMenuItem.Id;
		}

		private void ChangeModifierQuantity(Modifier modifier, ModifierGroup group, decimal newQuantity)
		{
			if (modifier.GetModifierType(group) == Modifier.ModifierType.Radio)
			{
				// We have to mimic the behaviour of radio buttons
				// i.e. deselect all other modifiers in the group before selecting the specified item
			
				if (group is ProductModifierGroup && modifier is ProductModifier)
				{
					foreach (Modifier mod in (group as ProductModifierGroup).ProductModifiers.Where (x => x != (modifier as ProductModifier)).ToList ())
					{
						mod.Qty = mod.MinQty;
					}
				}
				else if (group is DealModifierGroup && modifier is DealModifier)
				{
					foreach (Modifier mod in (group as DealModifierGroup).DealModifiers.Where (x => x != (modifier as DealModifier)).ToList ())
					{
						mod.Qty = mod.MinQty;
					}
				}
			}

			decimal verifiedNewQuantity = group.NewQty(modifier, newQuantity);
			modifier.Qty = verifiedNewQuantity;
			System.Diagnostics.Debug.WriteLine ("New modifier quantity :" + modifier.Qty.ToString ());
		}

		private void ChangeIngredientQuantity (Ingredient ingredient, decimal newQuantity)
		{
			decimal verifiedNewQty = ingredient.NewQty(newQuantity);
			ingredient.Qty = verifiedNewQty;
			System.Diagnostics.Debug.WriteLine ("Ingredient quantity set: " + ingredient.Item.Description + " QTY " + ingredient.Qty.ToString ());
		}

		private void ToggleNoModifiersView()
		{
			if ((this.TableView.Source as ModifiersScreenTableSource).ContainsData)
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

		private void IncreaseBasketItemQuantity()
		{
			this.basketItemQuantity++;
			RefreshBasketItemQuantityLabel();
		}

		private void DecreaseBasketItemQuantity()
		{
			if (this.basketItemQuantity > 1)
				this.basketItemQuantity--;
			RefreshBasketItemQuantityLabel();
		}

		public void RefreshTableView()
		{
			(this.TableView.Source as ModifiersScreenTableSource).RefreshTableData();
			this.TableView.ReloadData();
		}
	}
	*/
}

