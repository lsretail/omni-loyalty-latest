using System;
using CoreGraphics;
using Foundation;
using UIKit;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;

namespace Presentation.Screens
{
	/// <summary>
	/// Modal view controller
	/// </summary>
	public class EditBasketItemController : ModifiersController
	{
		private MenuItem originalMenuItem;
		private BasketItem basketItem;
		private Action OnDoneEditing;

		public EditBasketItemController (BasketItem basketItem, Action onDoneEditing) : base(basketItem.Item, false, false)
		{
			this.Title = LocalizationUtilities.LocalizedString("EditBasketItem_EditItem", "Edit item");

			this.originalMenuItem = basketItem.Item.Clone();
			this.basketItem = basketItem;
			this.OnDoneEditing = onDoneEditing;
			this.basketItemQuantity = basketItem.Quantity;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.View.BackgroundColor = UIColor.White;

			UIButton doneButton = new UIButton (UIButtonType.Custom);
			doneButton.SetImage (Utils.UI.GetColoredImage(UIImage.FromBundle("DoneIcon"), UIColor.White), UIControlState.Normal);
			doneButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			doneButton.Frame = new CGRect (0, 0, 30, 30);
			doneButton.TouchUpInside += (sender, e) => 
			{
				EditBasketItem();
				this.DismissViewController(true, null);
			};
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem (doneButton);

			UIButton cancelButton = new UIButton (UIButtonType.Custom);
			cancelButton.SetImage (Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.White), UIControlState.Normal);
			cancelButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			cancelButton.Frame = new CGRect (0, 0, 30, 30);
			cancelButton.TouchUpInside += (sender, e) => 
			{
				this.basketItem.Item = this.originalMenuItem;
				this.DismissViewController(true, null);
			};
			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (cancelButton);
		}
			
		private void EditBasketItem()
		{
			new Models.BasketModel().ChangeQty(this.basketItem, this.basketItemQuantity);
			this.OnDoneEditing();
		}
	}
}

