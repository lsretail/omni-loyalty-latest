// NOT IN USE

//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using Domain.Menus;
//
//namespace Presentation.Screens
//{
//	public partial class EditBasketItemScreen : UIViewController
//	{
//		private BasketItem BasketItem;
//
//		public EditBasketItemScreen (BasketItem basketItem) : base ("EditBasketItemScreen", null)
//		{
//			this.BasketItem = basketItem;
//		}
//
//		public override void DidReceiveMemoryWarning ()
//		{
//			// Releases the view if it doesn't have a superview.
//			base.DidReceiveMemoryWarning ();
//			
//			// Release any cached data, images, etc that aren't in use.
//		}
//
//		public override void ViewDidLoad ()
//		{
//			base.ViewDidLoad ();
//			
//			this.View.BackgroundColor = Utils.AppColors.KFCRed;
//
//			// Navigationbar
//			this.CustomNavBar.SetTitleTextAttributes (Utils.UI.TitleTextAttributes (false));
//			this.CustomNavBar.BarTintColor = Utils.AppColors.KFCRed;
//			this.CustomNavBar.Translucent = false;
//			this.CustomNavBar.TintColor = UIColor.White;
//
//			UINavigationItem navItem = new UINavigationItem ();
//			navItem.Title = "Edit item";
//
//			UIBarButtonItem cancelButton = new UIBarButtonItem ();
//			cancelButton.Title = "Cancel";
//			cancelButton.Clicked += (object sender, EventArgs e) => {
//
//				this.DismissViewController(true, null);
//
//			};
//			navItem.LeftBarButtonItem = cancelButton;
//
//			UIBarButtonItem doneButton = new UIBarButtonItem ();
//			doneButton.Title = "Done";
//			doneButton.Clicked += (object sender, EventArgs e) => {
//
//				EditBasketItem();
//				this.DismissViewController(true, null);
//
//			};
//			navItem.RightBarButtonItem = doneButton;
//
//			this.CustomNavBar.PushNavigationItem (navItem, true);
//		}
//
//		private void EditBasketItem()
//		{
//			System.Diagnostics.Debug.WriteLine ("Edit basket item" + this.BasketItem.Item.Description);
//		}
//	}
//}
//
