using System;
using UIKit;
using Presentation.Utils;
using Foundation;
using CoreGraphics;
using System.Linq;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation
{
	//Sets up a section to display 
	public class ItemDetailsContent : UIView
	{
		//Data to display
		private MenuItem item;

		#region private UI Elements
		private UILabel title;
		private UILabel price;
		private UITextView textDetails;
		#endregion

		#region Interactive UI Elements
		public IconButton AddToBasket;
		public IconButton EditItem;
		public InlineQtyModifier qtyModifier;
		#endregion

		private nfloat buttonWidth;
		private nfloat margin = 10f;

		public ItemDetailsContent(MenuItem itm)
		{
			item = itm;

			title = new UILabel
			{
				UserInteractionEnabled = false,
				Text = item != null ? item.Description : string.Empty,
				TextColor = Utils.AppColors.PrimaryColor,
				Font = UIFont.BoldSystemFontOfSize(17),
				TextAlignment = UITextAlignment.Left,
				Tag = 100
			};

			price = new UILabel
			{
				UserInteractionEnabled = false,
				Text = item != null ? AppData.MobileMenu.Currency.FormatDecimal(item.Price.Value) : string.Empty,
				TextColor = Utils.AppColors.PrimaryColor,
				Font = UIFont.SystemFontOfSize(14),
				TextAlignment = UITextAlignment.Left,
				Tag = 200
			};

			textDetails = new UITextView
			{
				Editable = false,
				ScrollEnabled = false,
				Text = item == null ? string.Empty : item.Detail,
				Font = UIFont.SystemFontOfSize(16f),
				BackgroundColor = UIColor.Clear
			};

			//If modifiers, don't show qtyModifier since it's 
			//displayed on the screen after addToBasket click
			qtyModifier = new InlineQtyModifier(1);

			EditItem = new IconButton(LocalizationUtilities.LocalizedString("EditBasketItem_EditItem", "Modify item"), Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile("/Icons/IconEdit.png"), UIColor.White))
			{
				Tag = 400,
			};


			AddToBasket = new IconButton(LocalizationUtilities.LocalizedString("ItemDetails_AddToBasket", "Add to basket"), Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.White)) { 
				Tag = 300,
				HorizontalAlignment = EditItem == null ? UIControlContentHorizontalAlignment.Center : UIControlContentHorizontalAlignment.Left,
				ContentEdgeInsets = EditItem == null ? new UIEdgeInsets(0f, 0f, 0f, 0f) : new UIEdgeInsets(0f, 48f, 0f, 0f)
			};

			if (EditItem != null)
				AddSubviews(title, price, EditItem, AddToBasket, textDetails);
			else if (qtyModifier != null)
				AddSubviews(title, price, qtyModifier, AddToBasket, textDetails);
			else
				AddSubviews(title, price, AddToBasket, textDetails);
		}

		public void ShowOrHideQtyModifier(bool hide)
		{
			//qtyModifier = hasRequiredModifiers() ? null : new InlineQtyModifier(1);
			qtyModifier.Hidden = hide;
			buttonWidth = hide ? Frame.Width - 2 * margin : (Frame.Width - 2 * margin) / 2 - margin / 2;
		}

		public void ShowOrHideEdit(bool hide)
		{
			//EditItem = (item is Product || !item.AnyModifiers) ?
			EditItem.Hidden = hide;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			nfloat buttonHeight = 40f;
			nfloat contentHeaderHeight = 20f;
			nfloat contentHeaderWidth = Frame.Width - 2 * margin;
			nfloat qtyModifierHeight = 50f;

			title.Frame = new CGRect(margin, margin, contentHeaderWidth, contentHeaderHeight);
			price.Frame = new CGRect(margin, title.Frame.Bottom, contentHeaderWidth, contentHeaderHeight);


			if (qtyModifier != null)
				qtyModifier.Frame = new CGRect(0, price.Frame.Bottom, Frame.Width, qtyModifierHeight);

			if (!EditItem.Hidden)
			{
				EditItem.Frame = qtyModifier != null ? new CGRect(margin, qtyModifier.Frame.Bottom + margin, buttonWidth, buttonHeight) : new CGRect(margin, price.Frame.Bottom + margin, buttonWidth, buttonHeight);
			}

			AddToBasket.Frame = new CGRect(!EditItem.Hidden ? EditItem.Frame.Right + margin : margin, qtyModifier != null ? qtyModifier.Frame.Bottom + margin : price.Frame.Bottom + margin, buttonWidth, buttonHeight);

			textDetails.Frame = new CGRect(margin, AddToBasket.Frame.Bottom + margin / 2, Frame.Width - 2 * margin, textDetails.SizeThatFits(textDetails.Frame.Size).Height);
		}
	}
}

