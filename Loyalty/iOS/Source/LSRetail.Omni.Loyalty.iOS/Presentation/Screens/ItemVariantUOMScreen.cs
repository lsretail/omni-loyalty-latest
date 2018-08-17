using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Domain.Items;
using Domain.Basket;
using Foundation;
using Presentation.Utils;
using Presentation.Models;
using CoreGraphics;

namespace Presentation.Screens
{
	/*
	public class ItemVariantUOMScreen : UIViewController
	{		
		private Item item;
		private string selectedVariant;
		private string selectedUom;

		private UILabel lblUOM;
		private UILabel lblVariant;
		private UITableView UOMTableView;
		private UITableView VariantTableView;

		private UIView changeQtyView;
		private UILabel lblChangeQty;
		private UIButton btnPlusQty;
		private UILabel lblItemQuantity;
		private UIButton btnMinusQty;

		private decimal qty;

		public delegate void EditingDoneEventHandler(decimal quantity, string variantId, string uomId);
		public event EditingDoneEventHandler EditingDone;

		public ItemVariantUOMScreen(Item item, decimal qty)
		{			
			this.item = item;
			this.qty = qty;
			selectedVariant = string.Empty;
			selectedUom = string.Empty;

			this.Title = NSBundle.MainBundle.LocalizedString("Account_Change", "Change");
		}

		public ItemVariantUOMScreen(BasketItem basketItem)
		{			
			this.item = basketItem.Item;
			this.qty = basketItem.Quantity;		
			selectedVariant = string.Empty;
			selectedUom = string.Empty;

			SetSelectedVariant(basketItem.VariantId);
			SetSelectedUom(basketItem.UomId);

			this.Title = NSBundle.MainBundle.LocalizedString("Account_Change", "Change");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = UIColor.White;

			UIBarButtonItem cancelButton = new UIBarButtonItem ();
			cancelButton.Title = NSBundle.MainBundle.LocalizedString("General_Cancel", "Cancel");
			cancelButton.Clicked += (object sender, EventArgs e) => 
			{
				this.DismissViewController(true, null);
			};
			this.NavigationItem.LeftBarButtonItem = cancelButton;

			UIBarButtonItem doneButton = new UIBarButtonItem ();
			doneButton.Title = NSBundle.MainBundle.LocalizedString("General_Done", "Done");
			doneButton.Clicked += (object sender, EventArgs e) => 
			{				
				if (this.EditingDone != null)
					this.EditingDone(this.qty, this.selectedVariant, this.selectedUom);

				this.DismissViewController(true, null);
			};
			this.NavigationItem.RightBarButtonItem = doneButton;

			this.lblVariant = new UILabel()
			{
				Text = NSBundle.MainBundle.LocalizedString("Item_Variant", "Variant"),
				TextColor = Utils.AppColors.PrimaryColor,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize (18)
			};
			this.View.AddSubview (this.lblVariant);

			this.VariantTableView = new UITableView () 
			{
				ScrollEnabled = true,
				Bounces = true,
				AlwaysBounceVertical = true,
				ShowsVerticalScrollIndicator = true,
				ShowsHorizontalScrollIndicator = true
			};
			this.View.AddSubview(this.VariantTableView);

			this.lblUOM = new UILabel () 
			{
				Text = NSBundle.MainBundle.LocalizedString("Item_UOM", "UOM"),
				TextColor = Utils.AppColors.PrimaryColor,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize (18)
			};
			this.View.AddSubview (this.lblUOM);

			this.UOMTableView = new UITableView () 
			{
				ScrollEnabled = true,
				Bounces = true,
				AlwaysBounceVertical = true,
				ShowsVerticalScrollIndicator = true,
				ShowsHorizontalScrollIndicator = true
			};
			this.View.AddSubview (this.UOMTableView);

			// Change quantity bar

			this.changeQtyView = new UIView();
			this.View.AddSubview(this.changeQtyView);
			this.changeQtyView.BackgroundColor = Utils.AppColors.TransparentWhite3;

			this.lblChangeQty = new UILabel();
			lblChangeQty.Text = NSBundle.MainBundle.LocalizedString("ItemDetails_Quantity", "Quantity") + ":";
			lblChangeQty.TextColor = AppColors.PrimaryColor;
			lblChangeQty.Tag = 100;
			this.changeQtyView.AddSubview(lblChangeQty);

			this.btnPlusQty = new UIButton();
			btnPlusQty.SetTitle("+", UIControlState.Normal);
			btnPlusQty.SetTitleColor(AppColors.PrimaryColor, UIControlState.Normal);
			btnPlusQty.BackgroundColor = UIColor.Clear;
			btnPlusQty.Tag = 200;
			btnPlusQty.TouchUpInside += (object sender, EventArgs e) => { IncreaseQuantityToAddToBasket(); };
			this.changeQtyView.AddSubview(btnPlusQty);

			this.lblItemQuantity = new UILabel();
			lblItemQuantity.Text = this.qty.ToString();
			lblItemQuantity.TextColor = AppColors.PrimaryColor;
			lblItemQuantity.Font = UIFont.SystemFontOfSize(14);
			lblItemQuantity.TextAlignment = UITextAlignment.Center;
			lblItemQuantity.Tag = 400;
			this.changeQtyView.AddSubview(lblItemQuantity);

			this.btnMinusQty = new UIButton();
			btnMinusQty.SetTitle("-", UIControlState.Normal);
			btnMinusQty.SetTitleColor(AppColors.PrimaryColor, UIControlState.Normal);
			btnMinusQty.BackgroundColor = UIColor.Clear;
			btnMinusQty.Tag = 300;
			btnMinusQty.TouchUpInside += (object sender, EventArgs e) => { DecreaseQuantityToAddToBasket(); };
			this.changeQtyView.AddSubview(btnMinusQty);

			DisplayTables ();
		}

		public override void ViewDidLayoutSubviews ()
		{			
			base.ViewDidLayoutSubviews ();

			nfloat labelHeight = 40f;
			nfloat changeQtyViewHeight = 40f;
			nfloat tableHeight = (this.View.Frame.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length - 2 * labelHeight - changeQtyViewHeight) / 2;

			// Variants

			this.lblVariant.Frame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Frame.Width, labelHeight);
			this.VariantTableView.Frame = new CGRect(0, this.lblVariant.Frame.Bottom, this.View.Frame.Width, tableHeight);

			// UOMs

			this.lblUOM.Frame = new CGRect(0, this.VariantTableView.Frame.Bottom, this.View.Frame.Width, labelHeight);
			this.UOMTableView.Frame = new CGRect(0, this.lblUOM.Frame.Bottom, this.View.Frame.Width, tableHeight);

			// Change quantity view

			this.changeQtyView.Frame = new CGRect(0, this.View.Bounds.Bottom - this.BottomLayoutGuide.Length - changeQtyViewHeight, this.View.Frame.Width, changeQtyViewHeight);

			nfloat changeQtyViewMargin = 10f;
			nfloat changeQtyViewButtonWidth = 50f;

			UIView lblChangeQuantity = this.changeQtyView.ViewWithTag(100);
			lblChangeQuantity.Frame = new CGRect(changeQtyViewMargin, 0f, 80f, this.changeQtyView.Frame.Height);

			UIView btnPlusQty = this.changeQtyView.ViewWithTag(200);
			btnPlusQty.Frame = new CGRect(this.changeQtyView.Frame.Width - changeQtyViewButtonWidth, 0, changeQtyViewButtonWidth, this.changeQtyView.Frame.Height);

			UIView lblItemQuantity = this.changeQtyView.ViewWithTag(400);
			lblItemQuantity.Frame = new CGRect(btnPlusQty.Frame.Left - 40f, 0, 40f, this.changeQtyView.Frame.Height);

			UIView btnMinusQty = this.changeQtyView.ViewWithTag(300);
			btnMinusQty.Frame = new CGRect(lblItemQuantity.Frame.Left - changeQtyViewButtonWidth, 0, changeQtyViewButtonWidth, this.changeQtyView.Frame.Height);
		}

		private void DisplayTables()
		{
			// Units of measure

			if (item.UnitOfMeasures != null && item.UnitOfMeasures.Count > 0)
			{
				if (string.IsNullOrEmpty(selectedUom))
					selectedUom = item.SalesUomId;

				UOMTableViewSource uomTableSource = new UOMTableViewSource(item.UnitOfMeasures);
				uomTableSource.UomSelected += (string uomId) => SetSelectedUom(uomId);
				this.UOMTableView.Source = uomTableSource;

				this.UOMTableView.SelectRow(
					(this.UOMTableView.Source as UOMTableViewSource).GetIndexPathForUom(this.selectedUom),
					false,
					UITableViewScrollPosition.Top
				);			
			}
			else
			{
				this.lblUOM.Hidden = true;
				this.UOMTableView.Hidden = true;
			}
				
			// Variants

			if (item.Variants != null && item.Variants.Count > 0) 
			{
				if (string.IsNullOrEmpty(selectedVariant))
					selectedVariant = item.Variants[0].Id;
				
				VariantTableViewSource variantTableSource = new VariantTableViewSource(item.Variants);
				variantTableSource.VariantSelected += (string variantId) => SetSelectedVariant(variantId);
				this.VariantTableView.Source = variantTableSource;

				this.VariantTableView.SelectRow(
					(this.VariantTableView.Source as VariantTableViewSource).GetIndexPathForVariant(this.selectedVariant),
					false,
					UITableViewScrollPosition.Top
				);		
			}
			else
			{
				this.lblVariant.Hidden = true;
				this.VariantTableView.Hidden = true;
			}
		}
			
		public void SetSelectedVariant(string variant)
		{
			System.Diagnostics.Debug.WriteLine("ItemVariantUOMScreen.SetSelectedVariant() - Setting selected variant as: " + variant);
			this.selectedVariant = variant;
		}

		public void SetSelectedUom(string uom)
		{
			System.Diagnostics.Debug.WriteLine("ItemVariantUOMScreen.SetSelectedUom() - Setting selected UOM as: " + uom);
			this.selectedUom = uom;
		}
			
		private void RefreshQuantityToAddToBasketLabel()
		{
			UILabel lblQtyToAddToBasket = (UILabel)this.changeQtyView.ViewWithTag(400);
			lblQtyToAddToBasket.Text = this.qty.ToString();
		}

		private void IncreaseQuantityToAddToBasket()
		{
			this.qty++;
			RefreshQuantityToAddToBasketLabel();
		}

		private void DecreaseQuantityToAddToBasket()
		{
			if (this.qty > 1)
				this.qty--;
			RefreshQuantityToAddToBasketLabel();
		}

		#region Table view sources

		public class UOMTableViewSource : UITableViewSource
		{
			private const string CELL_IDENTIFIER = "UomCellID";
			private List<UnitOfMeasure> uoms;

			public delegate void UomSelectedEventHandler(string uomId);
			public event UomSelectedEventHandler UomSelected;

			public UOMTableViewSource (List<UnitOfMeasure> uoms) 
			{ 
				this.uoms = uoms; 
			}
				
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}
				
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return this.uoms.Count; 
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CELL_IDENTIFIER) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Value1, CELL_IDENTIFIER);
				
				cell.TextLabel.Text  = this.uoms[indexPath.Row].Description;
				cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (this.UomSelected != null)
					this.UomSelected(this.uoms[indexPath.Row].Id);				
			}

			public NSIndexPath GetIndexPathForUom(string uomId)
			{
				int row = 0;
				var uom = this.uoms.Where(x => x.Id == uomId).FirstOrDefault();
				if (uom != null)
					row = this.uoms.IndexOf(uom);

				return NSIndexPath.FromRowSection(row, 0);
			}
		}
			
		private class VariantTableViewSource : UITableViewSource
		{
			private const string CELL_IDENTIFIER = "VariantCellID";
			private List<Variant> variants;

			public delegate void VariantSelectedEventHandler(string variantId);
			public event VariantSelectedEventHandler VariantSelected;

			public VariantTableViewSource (List<Variant> variants) 
			{ 
				this.variants = variants; 
			}
				
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}
				
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return this.variants.Count; 
			}
				
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CELL_IDENTIFIER) as UITableViewCell;
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Value1, CELL_IDENTIFIER);

				cell.TextLabel.Text  = this.variants[indexPath.Row].Description;
				cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (this.VariantSelected != null)
					this.VariantSelected(this.variants[indexPath.Row].Id);				
			}

			public NSIndexPath GetIndexPathForVariant(string variantId)
			{
				int row = 0;
				var variant = this.variants.Where(x => x.Id == variantId).FirstOrDefault();
				if (variant != null)
					row = this.variants.IndexOf(variant);

				return NSIndexPath.FromRowSection(row, 0);
			}
		}

		#endregion
	}
	*/
}

