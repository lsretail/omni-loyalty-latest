using System;
using CoreGraphics;
using Foundation;
using UIKit;
using Presentation.Utils;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.Screens
{
	public class ModifiersTableCell : UITableViewCell
	{
		private static readonly NSString Key = new NSString ("ModifiersTableCell");

		public ModifiersTableCell (string titleText, object objectToDisplay) : base(UITableViewCellStyle.Subtitle, ModifiersTableCell.Key)
		{
			this.TextLabel.Text = titleText;
			this.TextLabel.Font = UIFont.SystemFontOfSize(14);

			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			SetDetailsText ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.BackgroundColor = UIColor.White;
		}

		private void SetDetailsText()
		{
			// TODO Do we ever want details text?
			this.DetailTextLabel.Text = string.Empty;
		}
	}

	public class ModifiersTableCellCheckbox : UITableViewCell
	{
		private static readonly NSString Key = new NSString ("ModifiersTableCellCheckbox");
		private object objectOnDisplay;
		private object objectOnDisplayGroup;
		ModifiersView.IModifiersListeners listener;

		public ModifiersTableCellCheckbox (ModifiersView.IModifiersListeners listener,string titleText, object objectToDisplay, object objectToDisplayGroup) : base(UITableViewCellStyle.Subtitle, ModifiersTableCellCheckbox.Key)
		{
			this.listener = listener;
			this.TextLabel.Text = titleText;
			this.TextLabel.Font = UIFont.SystemFontOfSize(14);

			this.objectOnDisplay = objectToDisplay;
			this.objectOnDisplayGroup = objectToDisplayGroup;

			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			SetPriceAndQtyText ();
			RefreshCheckmark ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.BackgroundColor = UIColor.White;
		}

		private void SetPriceAndQtyText()
		{
			string detailString = string.Empty;

			if (this.objectOnDisplay is Ingredient)
			{
				// Ingredients don't have a price
			}
			else if (this.objectOnDisplay is ProductModifier)
			{
				ProductModifier myProdMod = this.objectOnDisplay as ProductModifier;

				detailString += (myProdMod.Price > 0 ? AppData.MobileMenu.Currency.FormatDecimal(myProdMod.Price) + " " : string.Empty);
			}
			else if (this.objectOnDisplay is DealModifier)
			{
				DealModifier myDealMod = this.objectOnDisplay as DealModifier;

				detailString += (myDealMod.Price > 0 ? AppData.MobileMenu.Currency.FormatDecimal(myDealMod.Price) + " " : string.Empty);
			}

			if(!string.IsNullOrEmpty(detailString))
			{
				NSMutableAttributedString attributedString = new NSMutableAttributedString(this.TextLabel.Text + " +" + detailString);
				string priceString = " +" + detailString;

				int lengthOfText = this.TextLabel.Text.Length;
				int lengthOfPriceText = priceString.Length;

				var greyTextAttribute = new UIStringAttributes();
				greyTextAttribute.ForegroundColor = UIColor.DarkGray;
				greyTextAttribute.Font = UIFont.SystemFontOfSize(14f);

				attributedString.SetAttributes(greyTextAttribute, new NSRange(lengthOfText, lengthOfPriceText));

				this.TextLabel.AttributedText = attributedString;
			}
		}

		public void CellPressed()
		{
			this.listener.ChangeObjectQuantity ((this.Accessory == UITableViewCellAccessory.Checkmark ? 0 : 1), this.objectOnDisplay, this.objectOnDisplayGroup);
			RefreshCheckmark ();
		}

		public void RefreshCheckmark()
		{
			if (GetCurrentQuantity () == 0)
				this.Accessory = UITableViewCellAccessory.None;
			else
				this.Accessory = UITableViewCellAccessory.Checkmark;
		}

		private decimal GetCurrentQuantity()
		{
			if (this.objectOnDisplay is Ingredient) 
			{
				return (this.objectOnDisplay as Ingredient).Quantity;
			} 
			else if (this.objectOnDisplay is Modifier) 
			{
				return (this.objectOnDisplay as Modifier).Quantity;
			} 
			else 
			{
				return 0;
			}
		}
	}

	public class ModifiersTableCellStepper : UITableViewCell
	{
		private static readonly NSString Key = new NSString ("ModifiersTableCellStepper");
		private UIStepper stepper;
		private object objectOnDisplay;
		private object objectOnDisplayGroup;
		ModifiersView.IModifiersListeners listener;

		public ModifiersTableCellStepper (ModifiersView.IModifiersListeners listener,string titleText, object objectToDisplay, object objectToDisplayGroup) : base(UITableViewCellStyle.Subtitle, ModifiersTableCellStepper.Key)
		{
			this.listener = listener;
			this.TextLabel.Text = titleText;
			this.TextLabel.Font = UIFont.SystemFontOfSize(14);

			this.stepper = new UIStepper ();
			this.ContentView.AddSubview (this.stepper);

			this.objectOnDisplay = objectToDisplay;
			this.objectOnDisplayGroup = objectToDisplayGroup;

			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			SetPriceAndQtyText (titleText);
			RefreshStepperValues ();

			this.stepper.ValueChanged += (object sender, EventArgs e) => {
				this.listener.ChangeObjectQuantity ((int)this.stepper.Value, this.objectOnDisplay, this.objectOnDisplayGroup);
				SetPriceAndQtyText(titleText);
			};
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.stepper.Frame = new CGRect (this.ContentView.Bounds.Width - this.stepper.Frame.Width - 5f, (this.ContentView.Frame.Height - this.stepper.Frame.Height)/2, this.stepper.Frame.Width, this.stepper.Frame.Height);

			this.TextLabel.Frame = new CGRect(this.TextLabel.Frame.X, this.TextLabel.Frame.Y, this.stepper.Frame.X - this.TextLabel.Frame.X - 5f, this.TextLabel.Frame.Height);

			this.BackgroundColor = UIColor.White;
		}

		public void RefreshStepperValues()
		{
			if (this.objectOnDisplay is Modifier && this.objectOnDisplayGroup is ModifierGroup) 
			{
				Modifier myMod = this.objectOnDisplay as Modifier;
				ModifierGroup myModGroup = this.objectOnDisplayGroup as ModifierGroup;

				this.stepper.Value = (double)myMod.Quantity;
				this.stepper.MaximumValue = (double)myMod.MaximumSelection;
				this.stepper.MinimumValue = (double)myMod.MinimumSelection;

				// Check if the stepper values are limited by the modifier group selection restrictions
				if (myModGroup.HasSelectionRestriction)
				{
					if (myModGroup.Selected + 1 > myModGroup.MaximumSelection)
						this.stepper.MaximumValue = this.stepper.Value;
					if (myModGroup.Selected - 1 < myModGroup.MinimumSelection)
						this.stepper.MinimumValue = this.stepper.Value;
				}
			} 
			else if (this.objectOnDisplay is Ingredient) 
			{
				// No selection restrictions on ingredients other than the restrictions placed on the ingredients themselves
				this.stepper.Value = (double)(this.objectOnDisplay as Ingredient).Quantity;
				this.stepper.MaximumValue = (double)(this.objectOnDisplay as Ingredient).MaximumQuantity;
				this.stepper.MinimumValue = (double)(this.objectOnDisplay as Ingredient).MinimumQuantity;
			}
		}

		private void SetPriceAndQtyText(string titleText)
		{
			string detailString = string.Empty;

			if (this.objectOnDisplay is Ingredient)
			{
				Ingredient myIngredient = this.objectOnDisplay as Ingredient;

				// Ingredients don't have a price
				this.TextLabel.Text = Utils.Util.FormatQty(myIngredient.Quantity) + "x " + titleText;
			}
			else if (this.objectOnDisplay is ProductModifier)
			{
				ProductModifier myProdMod = this.objectOnDisplay as ProductModifier;

				this.TextLabel.Text = Utils.Util.FormatQty(myProdMod.Quantity) + "x " + titleText;
				detailString += (myProdMod.Price > 0 ? AppData.MobileMenu.Currency.FormatDecimal(myProdMod.Price) + " " : string.Empty);
			}
			else if (this.objectOnDisplay is DealModifier)
			{
				DealModifier myDealMod = this.objectOnDisplay as DealModifier;

				this.TextLabel.Text = Utils.Util.FormatQty(myDealMod.Quantity) + "x " + titleText;
				detailString += (myDealMod.Price > 0 ? AppData.MobileMenu.Currency.FormatDecimal(myDealMod.Price) + " " : string.Empty);
			}

			if(!string.IsNullOrEmpty(detailString))
			{
				NSMutableAttributedString attributedString = new NSMutableAttributedString(this.TextLabel.Text + " +" + detailString);
				string priceString = " +" + detailString;

				int lengthOfText = this.TextLabel.Text.Length;
				int lengthOfPriceText = priceString.Length;

				var greyTextAttribute = new UIStringAttributes();
				greyTextAttribute.ForegroundColor = UIColor.DarkGray;
				greyTextAttribute.Font = UIFont.SystemFontOfSize(14f);

				attributedString.SetAttributes(greyTextAttribute, new NSRange(lengthOfText, lengthOfPriceText));

				this.TextLabel.AttributedText = attributedString;
			}
		}
	}

	public class DealLineItemTableCell : UITableViewCell
	{
		private static readonly NSString Key = new NSString ("DealLineItemTableCell");
		private MenuDealLineItem itemOnDisplay;
		private MenuDealLine itemOnDisplayLine;
		ModifiersView.IModifiersListeners listener;

		public DealLineItemTableCell (ModifiersView.IModifiersListeners listener,string titleText, MenuDealLineItem itemToDisplay, MenuDealLine itemToDisplayLine) : base(UITableViewCellStyle.Subtitle, DealLineItemTableCell.Key)
		{
			this.listener = listener;
			this.TextLabel.Text = titleText;
			this.TextLabel.Font = UIFont.SystemFontOfSize(14);

			this.itemOnDisplay = itemToDisplay;
			this.itemOnDisplayLine = itemToDisplayLine;

			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			SetPriceAndQtyText ();
			RefreshCheckmark ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.BackgroundColor = UIColor.White;
		}

		private void SetPriceAndQtyText()
		{
			if(this.itemOnDisplay.Quantity > 1)
				this.TextLabel.Text = Utils.Util.FormatQty(this.itemOnDisplay.Quantity) + "x " + this.TextLabel.Text;

			string detailString = string.Empty;

			detailString += (this.itemOnDisplay.PriceAdjustment > 0 ? AppData.MobileMenu.Currency.FormatDecimal(this.itemOnDisplay.Quantity * this.itemOnDisplay.PriceAdjustment) + " " : string.Empty);

			if(!string.IsNullOrEmpty(detailString))
			{
				NSMutableAttributedString attributedString = new NSMutableAttributedString(this.TextLabel.Text + " +" + detailString);
				string priceString = " +" + detailString;

				int lengthOfText = this.TextLabel.Text.Length;
				int lengthOfPriceText = priceString.Length;

				var greyTextAttribute = new UIStringAttributes();
				greyTextAttribute.ForegroundColor = UIColor.DarkGray;
				greyTextAttribute.Font = UIFont.SystemFontOfSize(14f);

				attributedString.SetAttributes(greyTextAttribute, new NSRange(lengthOfText, lengthOfPriceText));

				this.TextLabel.AttributedText = attributedString;
			}
		}

		public void CellPressed()
		{
			this.listener.ChangeObjectQuantity ((this.Accessory == UITableViewCellAccessory.Checkmark ? 0 : 1), this.itemOnDisplay, this.itemOnDisplayLine);
			RefreshCheckmark ();
		}

		public void RefreshCheckmark()
		{
			if (GetCurrentQuantity () == 0)
				this.Accessory = UITableViewCellAccessory.None;
			else
				this.Accessory = UITableViewCellAccessory.Checkmark;
		}

		private int GetCurrentQuantity()
		{
			if (this.itemOnDisplay.MenuItem.Id == this.itemOnDisplayLine.SelectedId)
				return 1;
			else
				return 0;
		}
	}
}

