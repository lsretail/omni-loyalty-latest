using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Presentation.Models;
using System.Timers;
using LSRetail.Omni.Hospitality.Loyalty.iOS;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.Screens
{
	/// <summary>
	/// Modal view controller
	/// </summary>
	public class AddToBasketController : ModifiersController
	{
		public AddToBasketController(MenuItem menuItem, decimal initialBasketItemQuantity, bool onlyShowRequiredModifiers) : base(menuItem, onlyShowRequiredModifiers, true)
		{
			Title = LocalizationUtilities.LocalizedString("AddToBasket_AddToBasket", "Add to basket");
			this.basketItemQuantity = initialBasketItemQuantity;
			base.OnAddToBasketPressed += () =>
			{
				AddItemToBasket();
			};
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.View.BackgroundColor = UIColor.White;

			UIButton doneButton = new UIButton(UIButtonType.Custom);
			doneButton.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.White), UIControlState.Normal);
			doneButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			doneButton.Frame = new CGRect(0, 0, 30, 30);
			doneButton.TouchUpInside += (sender, e) =>
			{
				AddItemToBasket();
			};

			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(doneButton);

			UIButton cancelButton = new UIButton(UIButtonType.Custom);
			cancelButton.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.White), UIControlState.Normal);
			cancelButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			cancelButton.Frame = new CGRect(0, 0, 30, 30);
			cancelButton.TouchUpInside += (sender, e) =>
			{
				this.DismissViewController(true, null);
			};

			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(cancelButton);
		}

		private void AddItemToBasket()
		{
			new BasketModel().AddItemToBasket(this.MenuItem, this.basketItemQuantity);
			Utils.UI.bannerViewTimer.Start();
			this.DismissViewController(true, null);
			Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Utils.Image.FromFile("/Branding/Standard/default_map_location_image.png"));
		}
	}
}

