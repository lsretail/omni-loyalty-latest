// TODO Delete this. We're using OffersAndCouponsScreen2 now.

//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//
//namespace Presentation.Screens
//{
//	public partial class OffersAndCouponsScreen : UIViewController
//	{
//		//UILabel lblTitle;
//
//		public OffersAndCouponsScreen () : base ("OffersAndCouponsScreen", null)
//		{
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
//			/*
//			this.lblTitle = new UILabel ();//  new UILabel (new RectangleF (0, TopLayoutGuide.Length, this.View.Frame.Width, 40));
//			this.lblTitle.Text = "You have no offers or coupons.";
//			this.lblTitle.TextColor = Utils.AppColors.PrimaryColor;
//			this.lblTitle.TextAlignment = UITextAlignment.Center;
//			this.View.AddSubview (lblTitle);*/
//		}
//
//		public override void ViewWillAppear (bool animated)
//		{
//			base.ViewWillAppear (animated);
//
//			// Navigationbar
//			this.Title = "Offers and coupons";	// TODO Localization
//			this.NavigationController.NavigationBar.SetTitleTextAttributes (Utils.UI.TitleTextAttributes (false));
//			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
//			this.NavigationController.NavigationBar.Translucent = false;
//			this.NavigationController.NavigationBar.TintColor = UIColor.White;
//		}
//
//		public override void ViewDidLayoutSubviews ()
//		{
//			base.ViewDidLayoutSubviews ();
//
//			//this.lblTitle.Frame = new RectangleF (0, TopLayoutGuide.Length, this.View.Frame.Width, 40);
//		}
//	}
//}
//
