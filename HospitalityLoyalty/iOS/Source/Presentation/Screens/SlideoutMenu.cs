using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Screens
{
	/*
	public partial class SlideoutMenu : UIViewController
	{
		private float slideWidth;

		private List<UIViewController> screens;
		private HomeScreen homeScreen;
		private MenuScreen menuScreen;
		private LocationsScreen locationsScreen;
		private OffersAndCouponsScreen2 offersAndCouponsScreen;
		private HistoryScreen historyScreen;
		private FavoritesScreen favoritesScreen;
		private InformationScreen informationScreen;

		public List<UIViewController> Screens { get { return this.screens; } }
		public HomeScreen HomeScreen { get { return this.homeScreen; } }
		public MenuScreen MenuScreen { get { return this.menuScreen; } }
		public LocationsScreen LocationsScreen { get { return this.locationsScreen; } }
		public OffersAndCouponsScreen2 OffersAndCouponsScreen { get { return this.offersAndCouponsScreen; } }
		public HistoryScreen HistoryScreen { get { return this.historyScreen; } }
		public FavoritesScreen FavoritesScreen { get { return this.favoritesScreen; } }

		public SlideoutMenu () : base ("SlideoutMenu", null)
		{
			this.slideWidth = Utils.Util.AppDelegate.SlideoutNavCtrl.SlideWidth;
			InstantiateViewControllers ();
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
			this.NavigationController.NavigationBarHidden = true;

			BuildAccountView();
			this.AccountView.BackgroundColor = UIColor.Clear;

			this.MenuView.BackgroundColor = UIColor.Clear;
			this.MenuView.Source = new SlideoutMenuTableSource(this);
		
			this.MenuView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.AutomaticallyAdjustsScrollViewInsets = false; // Removes the whitespace iOS adds at the top of the scrollview
			this.MenuView.ScrollEnabled = false;

			this.View.BackgroundColor = Utils.AppColors.PrimaryColor;

			UIImageView backgroundViewToBlur = new UIImageView ();
			backgroundViewToBlur.Image = Utils.Image.FromFile ("/Branding/Standard/slideoutmenubackground2.png");
			backgroundViewToBlur.ContentMode = UIViewContentMode.BottomLeft;
			backgroundViewToBlur.ClipsToBounds = true;
			backgroundViewToBlur.Tag = 20;
			this.ViewNoStatusbar.AddSubview(backgroundViewToBlur);

			UIView backgroundView = new UIView ();
			backgroundView.BackgroundColor = UIColor.Clear;
			backgroundView.Tag = 10;
			this.ViewNoStatusbar.AddSubview(backgroundView);

			if(Utils.Util.GetOSVersion().Major >= 8)
			{
				var blur = UIBlurEffect.FromStyle (UIBlurEffectStyle.ExtraLight);
				var blurView = new UIVisualEffectView (blur);
				blurView.Tag = 30;
				backgroundView.Add (blurView);
			}
			else
			{
				UIView backgroundOverlay = new UIView();
				backgroundOverlay.BackgroundColor = Utils.AppColors.TransparentWhite2;
				backgroundOverlay.Tag = 30;
				this.ViewNoStatusbar.AddSubview(backgroundOverlay);
			}

			this.ViewNoStatusbar.BringSubviewToFront (this.AccountView);
			this.ViewNoStatusbar.BringSubviewToFront (this.MenuView);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
		
			UIView backgroundView = this.ViewNoStatusbar.ViewWithTag(10);
			backgroundView.Frame = new CGRect(0f, 0f, this.ViewNoStatusbar.Frame.Width, this.ViewNoStatusbar.Frame.Height);

			UIView backgroundViewToBlur = this.ViewNoStatusbar.ViewWithTag(20);
			backgroundViewToBlur.Frame = backgroundView.Frame;

			if(Utils.Util.GetOSVersion().Major >= 8)
			{
				UIView blurView = backgroundView.ViewWithTag(30);
				blurView.Frame = backgroundView.Bounds;
			}
			else
			{
				UIView backgroundOverlay = this.ViewNoStatusbar.ViewWithTag(30);
				backgroundOverlay.Frame = backgroundView.Frame;

				Utils.UI.ApplyBlurBackgroundToView(backgroundView, backgroundViewToBlur);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (AppData.ShouldRefreshPoints)
			{
				new ContactModel().ContactUpdatePointBalance(() => {

					AppData.ShouldRefreshPoints = false;
					RefreshSlideoutMenu();

				}, () => { /* Do nothing  });
			}
		}
			
		#region Menu item pressed functions

		public void MenuItemPressedHome()
		{
			this.NavigationController.PopToRootViewController (false);
			this.NavigationController.PushViewController (this.homeScreen, true);
		}

		public void MenuItemPressedMenu()
		{
			this.NavigationController.PopToRootViewController (false);
			this.NavigationController.PushViewController (this.menuScreen, true);
		}

		public void MenuItemPressedLocations()
		{
			this.NavigationController.PopToRootViewController (false);
			this.NavigationController.PushViewController (this.locationsScreen, true);
		}

		public void MenuItemPressedOffersAndCoupons()
		{
			this.NavigationController.PopToRootViewController (false);
			this.NavigationController.PushViewController (this.offersAndCouponsScreen, true);
		}

		public void MenuItemPressedHistory()
		{
			this.NavigationController.PopToRootViewController (false);
			this.NavigationController.PushViewController (this.historyScreen, true);
		}

		public void MenuItemPressedFavorites()
		{
			this.NavigationController.PopToRootViewController (false);
			this.NavigationController.PushViewController (this.favoritesScreen, true);
		}

		public void MenuItemPressedInformation()
		{
			this.NavigationController.PopToRootViewController(false);
			this.NavigationController.PushViewController(this.informationScreen, true);
		}

		public void MenuItemPressedLogin()
		{
			LoginScreen loginScreen = new LoginScreen ();
			loginScreen.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
			this.PresentViewController(new UINavigationController(loginScreen), true, null);
		}

		public void MenuItemPressedLogout()
		{
			var alert = new UIAlertView(
				NSBundle.MainBundle.LocalizedString("Account_Logout", "Log out"),
				NSBundle.MainBundle.LocalizedString("Account_AreYouSureLogout", "Are you sure you want to log out?"),
				null,
				NSBundle.MainBundle.LocalizedString("General_OK", "OK"),
				null);
			alert.AddButton(NSBundle.MainBundle.LocalizedString("General_Cancel", "Cancel"));
			alert.Clicked += (object sender, UIButtonEventArgs e) => {
				if (e.ButtonIndex == 0)
				{
					// OK pressed
					new ContactModel().Logout(
						() => {
						
							// Go to "home" screen - first screen in the screen list
							this.NavigationController.PopToRootViewController (false);
							this.NavigationController.PushViewController (this.screens.FirstOrDefault(), true);
					
							Utils.Util.AppDelegate.SlideoutMenu.RefreshSlideoutMenu();
							if (Utils.Util.AppDelegate.BasketEnabled)
								Utils.Util.AppDelegate.SlideoutBasket.Refresh();

						},
						() => {});
				}
			};
			alert.Show();
		}

		public void MenuItemPressedAccount()
		{
			if (AppData.UserLoggedIn)
			{
				AccountScreen accountScreen = new AccountScreen(AppData.Contact);
				accountScreen.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
				this.PresentViewController(new UINavigationController(accountScreen), true, null);
			}
			else
			{
				MenuItemPressedLogin();
			}
		}

		#endregion

		public void InstantiateViewControllers()
		{
			this.screens = new List<UIViewController>();

			// Home
			if (Utils.Util.AppDelegate.HomeScreenEnabled)
			{
				//this.homeScreen = new HomeScreen (new UICollectionViewFlowLayout());
				this.homeScreen = new HomeScreen();
				this.screens.Add(this.homeScreen);
			}

			// Menu
			this.menuScreen = new MenuScreen (new UICollectionViewFlowLayout(), null, NSBundle.MainBundle.LocalizedString("Menu_Menu", "Menu"), CardCollectionCell.CellSizes.TallWide, true);
			this.screens.Add(this.menuScreen);

			// Locations
			this.locationsScreen = new LocationsScreen (new UICollectionViewFlowLayout());
			this.screens.Add(this.locationsScreen);

			// Offers and coupons
			this.offersAndCouponsScreen = new OffersAndCouponsScreen2 (new UICollectionViewFlowLayout());
			this.screens.Add(this.offersAndCouponsScreen);

			// History
			this.historyScreen = new HistoryScreen();
			this.screens.Add(this.historyScreen);

			// Favorites
			this.favoritesScreen = new FavoritesScreen();
			this.screens.Add(this.favoritesScreen);

			//Information
			if(Utils.Util.AppDelegate.InformationScreenEnabled)
			{
				this.informationScreen = new InformationScreen();
				this.screens.Add(this.informationScreen);
			}
		}
			
		#region Build top view

		// The top view is the account view, at the top of the menu

		private void BuildAccountView()
		{
			if (AppData.UserLoggedIn && Utils.Util.AppDelegate.UseAccountViewInSlideoutMenu)
			{
				BuildAccountViewUserLoggedIn();
			}
			else
			{
				BuildAccountViewLogo();
			}
		}

		private void BuildAccountViewLogo()
		{
			// Just show the image

			float imageDimensions = 110f; //60f;

			UIImageView image = new UIImageView();
			image.Frame = new CGRect (slideWidth/2 - imageDimensions/2, 20f/*40f, imageDimensions, imageDimensions);
			image.Image = Utils.Image.FromFile ("/Branding/Standard/default_account_image2.png");
			image.BackgroundColor = UIColor.Clear; //Utils.AppColors.BackgroundGray;
			image.ContentMode = UIViewContentMode.ScaleAspectFill;
			image.ClipsToBounds = true;
			image.Layer.CornerRadius = image.Frame.Size.Height / 2;
			image.Layer.MasksToBounds = true;
			image.Layer.BorderWidth = 0;
			this.AccountView.AddSubview (image);
		}
			
		private void BuildAccountViewUserLoggedIn()
		{
			// Show the image as well as account details

			Domain.MemberContacts.Contact contact = AppData.Contact;

			float xMargin = 20f;
			float imageDimensions = 60f;

			// This could be used as a profile pic of the contact. For example from facebook login.
			UIImageView image = new UIImageView();
			image.Frame = new CGRect (slideWidth/2 - imageDimensions/2, 20f, imageDimensions, imageDimensions);
			image.Image = Utils.Image.FromFile ("/Branding/Standard/default_account_image2.png");
			image.BackgroundColor = Utils.AppColors.BackgroundGray;
			image.ContentMode = UIViewContentMode.ScaleAspectFill;
			image.ClipsToBounds = true;
			image.Layer.CornerRadius = image.Frame.Size.Height / 2;
			image.Layer.MasksToBounds = true;
			image.Layer.BorderWidth = 0;
			this.AccountView.AddSubview (image);

			UILabel accountTitle = new UILabel ();
			accountTitle.Frame = new CGRect (xMargin, image.Frame.Bottom, slideWidth - 2 * xMargin, 20);
			accountTitle.Text = contact.UserName;
			accountTitle.TextColor = Utils.AppColors.PrimaryColor;
			accountTitle.Font = UIFont.SystemFontOfSize (14);
			accountTitle.TextAlignment = UITextAlignment.Center;
			this.AccountView.AddSubview (accountTitle);

			UILabel accountDetails = new UILabel ();
			accountDetails.Frame = new CGRect (xMargin, accountTitle.Frame.Bottom, slideWidth - 2 * xMargin, 20);
			accountDetails.Text = AppData.Contact.Account.PointBalance.ToString() + " " + NSBundle.MainBundle.LocalizedString("Account_Points_Lowercase", "points");
			accountDetails.TextColor = Utils.AppColors.PrimaryColor;
			accountDetails.Font = UIFont.SystemFontOfSize (10);
			accountDetails.TextAlignment = UITextAlignment.Center;
			this.AccountView.AddSubview (accountDetails);

			UIView separator = new UIView ();
			float separatorHeight = 1f;
			separator.Frame = new CGRect (0f, this.AccountView.Frame.Bottom - 2*separatorHeight, this.ViewNoStatusbar.Frame.Width, separatorHeight);
			separator.BackgroundColor = Utils.AppColors.PrimaryColor;
			this.AccountView.AddSubview (separator);
		}

		private void RefreshAccountView()
		{
			foreach (UIView view in this.AccountView.Subviews)
			{
				view.RemoveFromSuperview();
			}

			BuildAccountView();
		}
			
		#endregion

		public void RefreshSlideoutMenu()
		{
			RefreshAccountView();
			(this.MenuView.Source as SlideoutMenuTableSource).BuildCellList();
			this.MenuView.ReloadData();
		}
	}
*/
}

