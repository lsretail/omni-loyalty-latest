using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using Presentation.Screens;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Tables
{
	public class CustomMoreControllerTableSource : UITableViewSource
	{				
		private List<UIViewController> controllers;
		public bool isLoggedIn;


		public delegate void ControllerSelectedEventHandler(UIViewController controller);
		public ControllerSelectedEventHandler ControllerSelected;

		public CustomMoreControllerTableSource(List<UIViewController> controllers)
		{
			this.controllers = controllers;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.controllers.Count;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var controller = this.controllers [indexPath.Row];

			if (controller != null && controller.Title == LocalizationUtilities.LocalizedString ("Account_Account", "Account")) {

				AccountCell cell = tableView.DequeueReusableCell ("ACCOUNTCELL") as AccountCell;

				if (cell == null)
					cell = new AccountCell (); 

				cell.UpdataData();
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator; 

				return cell;
			}
			else {
				ImageCell cell = tableView.DequeueReusableCell ("IMAGECELL") as ImageCell;
				if (cell == null)
					cell = new ImageCell (); 
				cell.UpdateCell (controller.Title, GetImage(controller));
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				if (controller is LoginScreen && isLoggedIn)
				{
					cell.Hidden = true;
				}
				else {
					cell.Hidden = false;
				}
				return cell;
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{			
			var controller = this.controllers[indexPath.Row];

			if (controller != null)
			{
				if (this.ControllerSelected != null)
					this.ControllerSelected(controller);
			}

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			nfloat cellSize = 44f;
			var controller = this.controllers [indexPath.Row];

			if ( controller != null && ((controller is AccountController && !isLoggedIn) || (controller is LoginScreen && isLoggedIn)))
			{
				return 0;
			} 

			if (controller != null && controller.Title == LocalizationUtilities.LocalizedString("Account_Account", "Account")) 
			{
				return (float) 1.5 * cellSize;
			}

			return cellSize;
		}

		public UIImage GetImage( UIViewController uIViewController)
		{
			UIImage image = null;

			if (uIViewController.Title == LocalizationUtilities.LocalizedString ("SlideoutMenu_Items", "Items")) 
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/items.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString ("Basket_Basket", "Basket")) 	
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/ShoppingBasket.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("Notifications_Notifications", "Notifications"))	
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/notification.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("Location_Locations", "Locations") )
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/Locations.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("OffersAndCoupons_OffersAndCoupons", "Offers & coupons"))
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/offers.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("History_History", "History"))	
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/History.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString ("SlideoutMenu_Search", "Search"))
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/search.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("WishList_WishList", "Wish list"))
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/shoppingList.png");
			
			if (uIViewController.Title == LocalizationUtilities.LocalizedString("SlideoutMenu_ContactUs", "Contact us"))
				image = ImageUtilities.FromFile ("IconsForTabBar/Fullsize/Info.png");
			if(uIViewController.Title == LocalizationUtilities.LocalizedString("Account_Login", "Log in"))
				image = ImageUtilities.GetColoredImage ( ImageUtilities.FromFile ("IconsForTabBar/Fullsize/Account.png"), Utils.AppColors.PrimaryColor);
				
			if (image != null)
				return ImageUtilities.GetColoredImage (image, Utils.AppColors.PrimaryColor);
			else
			{
				return ImageUtilities.GetColoredImage ( ImageUtilities.FromFile ("IconsForTabBar/Fullsize/Home.png")  , Utils.AppColors.PrimaryColor);
			}
		}
	}
}

