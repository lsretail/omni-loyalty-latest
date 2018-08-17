using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Domain.Menus;
using Presentation.Models;
using System.Timers;

namespace Presentation.Screens
{
	/// <summary>
	/// Modal view controller
	/// </summary>
	public class AddToBasketScreen2 : ModifiersScreen
	{
		public AddToBasketScreen2 (MenuItem menuItem, decimal initialBasketItemQuantity, bool onlyShowRequiredModifiers) : base(menuItem, onlyShowRequiredModifiers, true)
		{
			this.basketItemQuantity = initialBasketItemQuantity;
			base.OnAddToBasketPressed += () =>
			{
				AddItemToBasket();
			};
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

			this.View.BackgroundColor = UIColor.White;

			this.Title = NSBundle.MainBundle.LocalizedString("AddToBasket_AddToBasket", "Add to basket");

			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes (false);
			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;

			UIButton doneButton = new UIButton (UIButtonType.Custom);
			doneButton.SetImage (Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconShoppingBasketAdd.png"), UIColor.White), UIControlState.Normal);
			doneButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			doneButton.Frame = new CGRect (0, 0, 30, 30);
			doneButton.TouchUpInside += (sender, e) => 
			{
				AddItemToBasket();
			};

			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem (doneButton);


			UIButton cancelButton = new UIButton (UIButtonType.Custom);
			cancelButton.SetImage (Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconCancel.png"), UIColor.White), UIControlState.Normal);
			cancelButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			cancelButton.Frame = new CGRect (0, 0, 30, 30);
			cancelButton.TouchUpInside += (sender, e) => 
			{
				this.DismissViewController(true, null);
			};

			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (cancelButton);
		}

		private void AddItemToBasket()
		{
			new BasketModel().AddItemToBasket(this.MenuItem, this.basketItemQuantity);
			Utils.Util.AppDelegate.SlideoutBasket.Refresh();

			Utils.UI.bannerViewTimer.Start();
			this.DismissViewController(true, null);
			Utils.UI.ShowAddedToBasketBannerView (NSBundle.MainBundle.LocalizedString("/Branding/Standard/SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Utils.Image.FromFile("default_map_location_image.png"));
		}
	}
}

