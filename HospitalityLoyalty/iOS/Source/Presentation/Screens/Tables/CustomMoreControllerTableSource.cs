using System;
using System.Collections.Generic;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class CustomMoreControllerTableSource : UITableViewSource
	{
		private List<UIViewController> controllers;

		public delegate void ControllerSelectedEventHandler(UIViewController controller);
		public ControllerSelectedEventHandler ControllerSelected;

		public CustomMoreControllerTableSource(List<UIViewController> controllers)
		{
			this.controllers = controllers;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.controllers.Count;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var controller = this.controllers[indexPath.Row];

			if (controller != null && controller.Title == LocalizationUtilities.LocalizedString("Account_Account", "Account"))
			{

				AccountCell cell = tableView.DequeueReusableCell("ACCOUNTCELL") as AccountCell;

				if (cell == null)
					cell = new AccountCell();

				cell.UpdataData();
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				return cell;
			}
			else
			{
				ImageCell cell = tableView.DequeueReusableCell("IMAGECELL") as ImageCell;
				if (cell == null)
					cell = new ImageCell();
				cell.UpdateCell(controller.Title, GetImage(controller));
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				return cell;
			}
		}


		public UIImage GetImage(UIViewController uIViewController)
		{
			UIImage image = null;

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("Menu_Menu", "Menu"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconMenu.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("OffersAndCoupons_OffersAndCoupons", "Offers & coupons"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconOC.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("Favorites_Favorites", "Favorites"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconFavorite.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("History_History", "History"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconHistory.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("Locations_Locations", "Restaurants"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconLocations.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("Account_Account", "Account"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconAccount.png");

			if (uIViewController.Title == LocalizationUtilities.LocalizedString("Account_Login", "Log in"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconAccount.png");
			if (uIViewController.Title == LocalizationUtilities.LocalizedString("AboutUs_Title", "About us"))
				image = Image.FromFile("/IconsForTabBar/TabBarIconInfo.png");

			if (image != null)
				return Utils.UI.GetColoredImage(image, AppColors.PrimaryColor);
			else
			{
				return Utils.UI.GetColoredImage(Image.FromFile("/IconsForTabBar/TabBarIconHome.png"), AppColors.PrimaryColor);
			}
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			nfloat cellSize = 44f;
			var controller = this.controllers[indexPath.Row];

			if (controller != null && controller.Title == LocalizationUtilities.LocalizedString("Account_Account", "Account"))
			{
				return (float)2 * cellSize;
			}
			return cellSize;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var controller = this.controllers[indexPath.Row];

			if (controller != null)
			{
				if (this.ControllerSelected != null)
					this.ControllerSelected(controller);
			}

			tableView.DeselectRow(indexPath, true);
		}
	}
}

