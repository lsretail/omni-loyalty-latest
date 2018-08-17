using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using Presentation.Utils;

namespace Presentation.Screens
{
	/*
	public class SlideoutMenuTableSource : UITableViewSource
	{
		public static readonly NSString defaultKey = new NSString ("SlideoutMenuCell");

		private List<MenuItem> menuItems;

		private List<SlideoutMenuCell> tableCells;
		private Dictionary<SlideoutMenuCell, Action> cellToActionMap;

		public SlideoutMenuTableSource(SlideoutMenu menuController)
		{
			// NOTE
			// If we are using the account view, we use a dedicated login and logout cell
			// If we aren't using the account view, we use the account cell (which contains info on the logged in user)

			this.menuItems = new List<MenuItem>();

			MenuItem homeItem = new MenuItem()
			{
				ID = MenuItemIDs.home,
				Title = LocalizationUtilities.LocalizedString("Home_Home", "Home"),
				IconPath = "IconHome.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedHome(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(homeItem);

			MenuItem menuItem = new MenuItem()
			{
				ID = MenuItemIDs.menu,
				Title = LocalizationUtilities.LocalizedString("Menu_Menu", "Menu"),
				IconPath = "IconMenu.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedMenu(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(menuItem);

			MenuItem offersAndCouponsItem = new MenuItem()
			{
				ID = MenuItemIDs.offersAndCoupons,
				Title = LocalizationUtilities.LocalizedString("OffersAndCoupons_OffersAndCoupons", "Offers & coupons"),
				IconPath = "IconOC.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedOffersAndCoupons(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(offersAndCouponsItem);

			MenuItem favoritesItem = new MenuItem()
			{
				ID = MenuItemIDs.favorites,
				Title = LocalizationUtilities.LocalizedString("Favorites_Favorites", "Favorites"),
				IconPath = "IconFavoriteOFF.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedFavorites(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(favoritesItem);

			MenuItem historyItem = new MenuItem()
			{
				ID = MenuItemIDs.history,
				Title = LocalizationUtilities.LocalizedString("History_History", "History"),
				IconPath = "IconHistory.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedHistory(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(historyItem);

			MenuItem locationsItem = new MenuItem()
			{
				ID = MenuItemIDs.locations,
				Title = LocalizationUtilities.LocalizedString("Location_Locations", "Restaurants"),
				IconPath = "IconLocations.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedLocations(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(locationsItem);

			MenuItem informationItem = new MenuItem()
			{
				ID = MenuItemIDs.information,
				Title = LocalizationUtilities.LocalizedString("SlideoutMenu_Information", "About"),
				IconPath = "IconInfo.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedInformation(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(informationItem);

			MenuItem signInItem = new MenuItem()
			{
				ID = MenuItemIDs.login,
				Title = LocalizationUtilities.LocalizedString("Account_Login", "Log in"),
				IconPath = "IconAccount.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedLogin(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(signInItem);

			MenuItem signOutItem = new MenuItem()
			{
				ID = MenuItemIDs.logout,
				Title = LocalizationUtilities.LocalizedString("Account_Logout", "Log out"),
				IconPath = "IconAccount.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedLogout(); }),
				cellStyle = UITableViewCellStyle.Default
			};
			this.menuItems.Add(signOutItem);

			MenuItem accountItem = new MenuItem()
			{
				ID = MenuItemIDs.account,
				Title = LocalizationUtilities.LocalizedString("Account_Login", "Log in"), // NOTE: Title varies depending on whether user is logged in or not
				IconPath = "IconAccount.png",
				Color = UIColor.DarkGray,
				Action = new Action(() => { menuController.MenuItemPressedAccount(); }),
				cellStyle = UITableViewCellStyle.Subtitle
			};
			this.menuItems.Add(accountItem);

			BuildCellList();
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.tableCells.Count;
		}
			
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return this.tableCells[indexPath.Row];
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			SlideoutMenuCell cellPressed = this.tableCells[indexPath.Row];
			Action onPressAction = null;
			this.cellToActionMap.TryGetValue(cellPressed, out onPressAction);

			if (onPressAction != null)
				onPressAction();

			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 44f;
		}

		public void BuildCellList()
		{
			this.tableCells = new List<SlideoutMenuCell>();
			this.cellToActionMap = new Dictionary<SlideoutMenuCell, Action>();

			bool userLoggedIn = AppData.UserLoggedIn;
			bool useAccountView = Util.AppDelegate.UseAccountViewInSlideoutMenu;

			foreach (MenuItem menuItem in this.menuItems)
			{
				// Skip the home screen cell if the homescreen isn't enabled
				if (menuItem.ID == MenuItemIDs.home && !Utils.Util.AppDelegate.HomeScreenEnabled)
					continue;

				// Skip the offers and coupons screen if it isn't enabled
				if (menuItem.ID == MenuItemIDs.offersAndCoupons && !Utils.Util.AppDelegate.OffersAndCouponsEnabled)
					continue;

				// Skip the account cell if we are using the account view
				if (useAccountView && menuItem.ID == MenuItemIDs.account)
					continue;

				// Skip the information screen if it isn't enabled
				if(menuItem.ID == MenuItemIDs.information && !Utils.Util.AppDelegate.InformationScreenEnabled)
					continue;

				// Skip the login and logout cells if we aren't using the account view
				if (!useAccountView && (menuItem.ID == MenuItemIDs.login || menuItem.ID == MenuItemIDs.logout))
					continue;

				// Skip the login cell if a user is already logged in
				if (userLoggedIn && menuItem.ID == MenuItemIDs.login)
					continue;

				// Skip the logout cell if a user isn't logged in
				if (!userLoggedIn && menuItem.ID == MenuItemIDs.logout)
					continue;

				SlideoutMenuCell cell;

				if (menuItem.ID == MenuItemIDs.account)
				{
					cell = new SlideoutMenuCell();
					string title = GetAccountCellTitle();
					string detailText = GetAccountCellSubtitle();
					UIImage image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile (menuItem.IconPath), Utils.AppColors.PrimaryColor);

					cell.SetValues(image, title, detailText);
				}
				else
				{
					cell = new SlideoutMenuCell();

					string title = menuItem.Title;
					UIImage image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile (menuItem.IconPath), Utils.AppColors.PrimaryColor);

					cell.SetValues(image, title, string.Empty);
				}

				this.tableCells.Add(cell);
				this.cellToActionMap.Add(cell, menuItem.Action);
			}
		}

		private string GetAccountCellTitle()
		{
			if (AppData.UserLoggedIn)
				// Prefer first name, if no first name use username
				return (!String.IsNullOrEmpty(AppData.Contact.FirstName) ? AppData.Contact.FirstName : AppData.Contact.UserName);
			else
				return LocalizationUtilities.LocalizedString("Account_Login", "Log in");
		}

		private string GetAccountCellSubtitle()
		{
			if (AppData.UserLoggedIn && Util.AppDelegate.ShowLoyaltyPoints)
				return AppData.Contact.Account.PointBalance.ToString("N0") + " " + LocalizationUtilities.LocalizedString("Account_Points_Lowercase", "points");
			else
				return string.Empty;
		}
			
		public class MenuItem
		{
			public MenuItemIDs ID { get; set; }
			public string Title { get; set; }
			public string IconPath { get; set; }
			public UIColor Color { get; set; }
			public Action Action { get; set; }
			public UITableViewCellStyle cellStyle { get; set; } // Type of cell this menu item should be displayed in
		}

		public enum MenuItemIDs
		{
			home,
			locations,
			account,
			offersAndCoupons,
			menu,
			login,
			logout,
			history,
			favorites,
			information
		}

		public class SlideoutMenuCell : UITableViewCell
		{
			public static string Key = "SlideoutMenuCell";

			private UIImageView imageView;
			private UILabel lblTitle;
			private UILabel lblDetails;

			public SlideoutMenuCell() : base(UITableViewCellStyle.Default, Key)
			{
				this.BackgroundColor = UIColor.Clear;
				this.SelectionStyle = UITableViewCellSelectionStyle.Default;

				SetLayout();
			}

			public virtual void SetLayout()
			{
				this.imageView = new UIImageView();
				imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				imageView.ClipsToBounds = true;
				imageView.BackgroundColor = UIColor.Clear;
				this.ContentView.AddSubview(imageView);

				this.lblTitle = new UILabel();
				lblTitle.BackgroundColor = UIColor.Clear;
				lblTitle.TextColor = Utils.AppColors.PrimaryColor;
				this.ContentView.AddSubview(lblTitle);

				this.lblDetails = new UILabel();
				lblDetails.BackgroundColor = UIColor.Clear;
				lblDetails.TextColor = Utils.AppColors.PrimaryColor;
				lblDetails.Font = UIFont.SystemFontOfSize(12);
				this.ContentView.AddSubview(lblDetails);
			}

			public void SetValues(UIImage image, string title, string details)
			{
				this.imageView.Image = image;
				this.lblTitle.Text = title;
				this.lblDetails.Text = details;
			}

			public override void LayoutSubviews ()
			{
				base.LayoutSubviews ();

				nfloat margin = 10f;
				nfloat imageDimension = 30f;

				this.imageView.Frame = new CoreGraphics.CGRect(margin, 7f, imageDimension, imageDimension);

				if(string.IsNullOrEmpty(this.lblDetails.Text))
				{
					this.lblTitle.Frame = new CoreGraphics.CGRect(this.imageView.Frame.Right + 2*margin, 0, this.ContentView.Frame.Width - imageDimension - 2*margin, this.ContentView.Frame.Height);
				}
				else
				{
					this.lblTitle.Frame = new CoreGraphics.CGRect(this.imageView.Frame.Right + 2*margin, 2f, this.ContentView.Frame.Width - imageDimension - 2*margin, this.ContentView.Frame.Height - 22f);
					this.lblDetails.Frame = new CoreGraphics.CGRect(this.lblTitle.Frame.X, this.lblTitle.Frame.Bottom, this.ContentView.Frame.Width, 14f);
				}
			}
		}
	}
	*/
}

