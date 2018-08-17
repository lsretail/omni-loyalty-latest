using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using Domain.Advertisements;
using Domain.Menus;
using Domain.Images;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Screens
{
	public class HomeScreen2 : UIViewController
	{
		// TODO Get shortcuts from WS - Allow users to select the shortcut buttons?

		private UIScrollView containerScrollView;
		private UIScrollView headerPageView;
		private UIPageControl headerPageIndicator;
		//private UITableView shortcutTableView;
		private UIView memberInfoView;

		private UIButton btnShortcut1;
		private UIButton btnShortcut2;

		System.Timers.Timer timer;
		private Action scrollToNextAd;

		private List<Advertisement> advertisements;
		public bool IsDataLoaded { get; set; }

		private float headerPageViewHeight = Utils.Util.AppDelegate.DeviceScreenHeight <= 480f ? 220f : 290f;
		private float headerPageIndicatorHeight = 20f;
		//private float memberInfoViewHeight = 245f;  //140f

		public HomeScreen2 ()
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			this.Title = NSBundle.MainBundle.LocalizedString("Home_Home", "Home");
			this.NavigationController.NavigationBar.TitleTextAttributes = Utils.UI.TitleTextAttributes (false);
			this.NavigationController.NavigationBar.BarTintColor = Utils.AppColors.PrimaryColor;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.NavigationBar.TintColor = UIColor.White;
		
			if(IsDataLoaded)
				RefreshContactInfo();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = Utils.AppColors.BackgroundGray;

			// Container scroll view
			this.containerScrollView = new UIScrollView();
			this.containerScrollView.BackgroundColor = UIColor.Clear;
			this.View.AddSubview(this.containerScrollView);

			this.btnShortcut1 = new UIButton();
			btnShortcut1.SetTitle(NSBundle.MainBundle.LocalizedString("Menu_Menu", "Menu"), UIControlState.Normal);
			btnShortcut1.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			if(Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				btnShortcut1.Font = UIFont.SystemFontOfSize(16f);

			btnShortcut1.BackgroundColor = UIColor.White;
			btnShortcut1.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			btnShortcut1.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			btnShortcut1.TouchUpInside += (sender, e) => 
			{
				Utils.Util.AppDelegate.SlideoutMenu.MenuItemPressedMenu();
			};

			UIImageView btnShortcut1ImageView = new UIImageView();
			btnShortcut1ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnShortcut1ImageView.Image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconMenu.png"), Utils.AppColors.PrimaryColor);
			btnShortcut1ImageView.BackgroundColor = UIColor.Clear;
			this.btnShortcut1.AddSubview(btnShortcut1ImageView);
			this.containerScrollView.AddSubview(this.btnShortcut1);


			this.btnShortcut2 = new UIButton();
			btnShortcut2.SetTitle(NSBundle.MainBundle.LocalizedString("Favorites_Favorites", "Favorites"), UIControlState.Normal);
			btnShortcut2.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			if(Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				btnShortcut2.Font = UIFont.SystemFontOfSize(16f);

			btnShortcut2.BackgroundColor = UIColor.White;
			btnShortcut2.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			btnShortcut2.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			btnShortcut2.TouchUpInside += (sender, e) => 
			{				
				Utils.Util.AppDelegate.SlideoutMenu.MenuItemPressedFavorites();
			};

			UIImageView btnShortcut2ImageView = new UIImageView();
			btnShortcut2ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnShortcut2ImageView.Image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("IconFavoriteOFF.png"), Utils.AppColors.PrimaryColor);
			btnShortcut2ImageView.BackgroundColor = UIColor.Clear;
			this.btnShortcut2.AddSubview(btnShortcut2ImageView);
			this.containerScrollView.AddSubview(this.btnShortcut2);


			// Header page view
			this.headerPageView = new UIScrollView();
			this.headerPageView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.headerPageView.PagingEnabled = true;
			this.headerPageView.Bounces = false;
			this.headerPageView.ShowsHorizontalScrollIndicator = false;
			this.containerScrollView.AddSubview(this.headerPageView);

			// Header page indicator
			this.headerPageIndicator = new UIPageControl();
			this.headerPageIndicator.BackgroundColor = UIColor.Clear;
			this.containerScrollView.AddSubview(this.headerPageIndicator);

			// Shortcut table view
			/*
			this.shortcutTableView = new UITableView();
			this.shortcutTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.shortcutTableView.ScrollEnabled = false;
			this.shortcutTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.shortcutTableView.Source = new HomeScreen2TableSource(OnShortcutCellSelected, GetShortcutList());
			this.containerScrollView.AddSubview(this.shortcutTableView);
			*/

			// Member info view
			this.memberInfoView = new UIView();
			this.memberInfoView.BackgroundColor = Utils.AppColors.PrimaryColor;
			this.memberInfoView.UserInteractionEnabled = true;
			this.memberInfoView.AddGestureRecognizer(new UITapGestureRecognizer(() => { OnMemberInfoPressed(); }));
			this.containerScrollView.AddSubview(this.memberInfoView);

			// Logo image view
			UIImageView logoImageView = new UIImageView();
			logoImageView.Tag = 100;
			logoImageView.BackgroundColor = UIColor.Clear;
			logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			logoImageView.Image = Utils.Image.FromFile ("/Branding/Standard/homescreen_logo.png");
			logoImageView.ClipsToBounds = true;
			logoImageView.Layer.MasksToBounds = true;
			logoImageView.Layer.BorderWidth = 2f;
			logoImageView.Layer.BorderColor = UIColor.White.CGColor;
			this.memberInfoView.AddSubview(logoImageView);

			//Welcome label
			UILabel lblWelcome = new UILabel();
			lblWelcome.Tag = 150;
			lblWelcome.Text = NSBundle.MainBundle.LocalizedString("Home_Welcome", "Welcome");
			lblWelcome.BackgroundColor = UIColor.Clear;
			lblWelcome.TextColor = UIColor.White;
			lblWelcome.Font = UIFont.SystemFontOfSize(22f);
			this.memberInfoView.AddSubview(lblWelcome);

			//Sign in text view
			UITextView txtSignIn = new UITextView();
			txtSignIn.Tag = 160;
			txtSignIn.Text = NSBundle.MainBundle.LocalizedString("Home_SignIn", "");
			txtSignIn.BackgroundColor = UIColor.Clear;
			txtSignIn.TextColor = UIColor.White;
			txtSignIn.Font = UIFont.SystemFontOfSize(16f);
			txtSignIn.TextContainer.LineFragmentPadding = 0;
			txtSignIn.TextContainerInset = UIEdgeInsets.Zero;
			txtSignIn.UserInteractionEnabled = false;
			txtSignIn.Hidden = true;
			this.memberInfoView.AddSubview(txtSignIn);

			// Contact name label
			UILabel lblContactName = new UILabel();
			lblContactName.Tag = 200;
			lblContactName.BackgroundColor = UIColor.Clear;
			lblContactName.TextColor = UIColor.White;
			lblContactName.Hidden = true;
			lblContactName.UserInteractionEnabled = false;
			this.memberInfoView.AddSubview(lblContactName);

			// Contact point label
			UILabel lblContactPoints = new UILabel();
			lblContactPoints.Tag = 300;
			lblContactPoints.BackgroundColor = UIColor.Clear;
			lblContactPoints.TextColor = UIColor.White;
			lblContactPoints.Font = UIFont.SystemFontOfSize(12);
			lblContactPoints.Hidden = true;
			lblContactPoints.UserInteractionEnabled = false;
			this.memberInfoView.AddSubview(lblContactPoints);

			SetRightBarButtonItems();

			GetData();
		}
			
		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			this.containerScrollView.Frame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);

			float margin = 10f;

			// We want to fill the length of the screen, at minimum. Let's set a minimum shortcuttableviewheight to guarantee that.
			/*
			nfloat minimumShortcutTableViewHeight = this.containerScrollView.Bounds.Height - this.headerPageViewHeight - this.memberInfoViewHeight - margin;
			nfloat shortcutTableViewHeight = this.shortcutTableView.ContentSize.Height;	// The height needed to show all the tableview contents (no scrolling)
			if (shortcutTableViewHeight < minimumShortcutTableViewHeight)
				shortcutTableViewHeight = minimumShortcutTableViewHeight;
			*/


			this.btnShortcut1.Frame = new CGRect(margin, margin, this.containerScrollView.Bounds.Width/2 - margin - margin/2, 40f);
			this.btnShortcut2.Frame = new CGRect(this.btnShortcut1.Frame.Right + margin, margin, this.btnShortcut1.Frame.Width, 40f);
			this.headerPageView.Frame = new CGRect(margin, this.btnShortcut1.Frame.Bottom + margin, this.containerScrollView.Bounds.Width - 2 * margin, this.headerPageViewHeight - margin);
			this.headerPageIndicator.Frame = new CGRect(0, this.headerPageView.Frame.Bottom - this.headerPageIndicatorHeight, this.headerPageView.Bounds.Width, this.headerPageIndicatorHeight);
			//this.shortcutTableView.Frame = new CGRect(0, this.headerPageIndicator.Frame.Bottom, this.containerScrollView.Bounds.Width, shortcutTableViewHeight);
			nfloat memberInfoViewHeight = this.containerScrollView.Frame.Height - margin - this.btnShortcut1.Frame.Height - margin - this.headerPageView.Frame.Height - 2*margin;
			this.memberInfoView.Frame = new CGRect(margin, this.headerPageView.Frame.Bottom + margin, this.containerScrollView.Bounds.Width - 2 * margin, memberInfoViewHeight);

			// MemberInfoView layout

			UIView logoImageView = this.memberInfoView.ViewWithTag(100);
			float logoImageViewDimensions = 110f;
			if(Utils.Util.AppDelegate.DeviceScreenWidth > 320f)
				logoImageViewDimensions = 160f;
			
			logoImageView.Frame = new CGRect(20f, (this.memberInfoView.Bounds.Height - logoImageViewDimensions) / 2, logoImageViewDimensions, logoImageViewDimensions);
			logoImageView.Layer.CornerRadius = logoImageView.Frame.Size.Height / 2;


			UIView lblWelcome = this.memberInfoView.ViewWithTag(150);
			UIView txtSignIn = this.memberInfoView.ViewWithTag(160);
			UIView lblContactName = this.memberInfoView.ViewWithTag(200);
			UIView lblContactPoints = this.memberInfoView.ViewWithTag(300);


			lblContactName.Frame = new CGRect(logoImageView.Frame.Right + 20f, this.memberInfoView.Bounds.Height/2 - 10f, this.memberInfoView.Bounds.Right - logoImageView.Frame.Right - 2 * 20f, 20f);
			txtSignIn.Frame = new CGRect(logoImageView.Frame.Right + 20f, this.memberInfoView.Bounds.Height/2 - 10f, this.memberInfoView.Bounds.Right - logoImageView.Frame.Right - 2 * 20f, 80f);
			lblWelcome.Frame = new CGRect(logoImageView.Frame.Right + 20f, this.memberInfoView.Bounds.Height/2 - 37f, this.memberInfoView.Bounds.Right - logoImageView.Frame.Right - 2 * 20f, 22f);
			lblContactPoints.Frame = new CGRect(lblContactName.Frame.Left, lblContactName.Frame.Bottom, lblContactName.Frame.Width, 20f);




			// Set containerscrollview content height
			//nfloat containerScrollViewContentHeight = this.headerPageView.Frame.Height + margin + this.shortcutTableView.Frame.Height + this.memberInfoView.Frame.Height;
			nfloat containerScrollViewContentHeight = margin + this.btnShortcut1.Frame.Height + margin + this.headerPageView.Frame.Height + margin + this.memberInfoView.Frame.Height;
			this.containerScrollView.ContentSize = new CGSize(this.containerScrollView.Bounds.Width, containerScrollViewContentHeight);

			//Utils.UI.AddDropShadowToView(this.headerPageView);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (this.timer != null)
				this.timer.Start();
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			if (this.timer != null)
				this.timer.Stop();
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			if (Utils.Util.AppDelegate.BasketEnabled)
				barButtonItemList.Add(Utils.UI.GetBasketBarButtonItem());

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		public void RefreshLayout()
		{
			this.View.SetNeedsLayout();
			this.View.LayoutIfNeeded();
			RefreshContactInfo();
		}

		private void RefreshContactInfo()
		{
			if (AppData.UserLoggedIn)
				ShowContactInfo((!String.IsNullOrEmpty(AppData.Contact.FirstName) ? AppData.Contact.FirstName : AppData.Contact.UserName), AppData.Contact.Account.PointBalance);
			else
				HideContactInfo();
		}

		private void ShowContactInfo(string name, long points)
		{
			UILabel lblContactName = this.memberInfoView.ViewWithTag(200) as UILabel;
			UILabel lblContactPoints = this.memberInfoView.ViewWithTag(300) as UILabel;
			UITextView txtSignIn = this.memberInfoView.ViewWithTag(160) as UITextView;

			txtSignIn.Hidden = true;
			lblContactName.Hidden = false;
			if (Utils.Util.AppDelegate.ShowLoyaltyPoints)
				lblContactPoints.Hidden = false;

			lblContactName.Text = name;
			lblContactPoints.Text = points.ToString("N0") + " " + NSBundle.MainBundle.LocalizedString("Home_Points_Lowercase", "points");
		}

		private void HideContactInfo()
		{
			UILabel lblContactName = this.memberInfoView.ViewWithTag(200) as UILabel;
			UILabel lblContactPoints = this.memberInfoView.ViewWithTag(300) as UILabel;
			UITextView txtSignIn = this.memberInfoView.ViewWithTag(160) as UITextView;

			txtSignIn.Hidden = false;
			lblContactName.Hidden = true;
			lblContactPoints.Hidden = true;
		}

		private void OnMemberInfoPressed()
		{
			if (AppData.UserLoggedIn)
			{
				AccountScreen accountScreen = new AccountScreen(AppData.Contact);
				accountScreen.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
				this.PresentViewController(new UINavigationController(accountScreen), true, null);
			}
			else
			{
				LoginScreen loginScreen = new LoginScreen();
				loginScreen.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
				this.PresentViewController(new UINavigationController(loginScreen), true, null);
			}
		}

		public void OnShortcutCellSelected(HomeScreen2TableSource.ShortcutIds shortcutId)
		{
			System.Diagnostics.Debug.WriteLine("Selected shortcut: " + shortcutId.ToString());

			switch (shortcutId)
			{
			case HomeScreen2TableSource.ShortcutIds.Locations:
				this.NavigationController.PushViewController(new LocationsScreen (new UICollectionViewFlowLayout()), true);
				return;
			case HomeScreen2TableSource.ShortcutIds.Menu:
				this.NavigationController.PushViewController(new MenuScreen (new UICollectionViewFlowLayout(), null, NSBundle.MainBundle.LocalizedString("Menu_Menu", "Menu"), CardCollectionCell.CellSizes.TallNarrow, true), true);
				return;
			case HomeScreen2TableSource.ShortcutIds.OffersAndCoupons:
				this.NavigationController.PushViewController(new OffersAndCouponsScreen2 (new UICollectionViewFlowLayout()), true);
				return;
			case HomeScreen2TableSource.ShortcutIds.History:
				this.NavigationController.PushViewController(new HistoryScreen2(), true);
				return;
			case HomeScreen2TableSource.ShortcutIds.Favorites:
				this.NavigationController.PushViewController(new FavoritesScreen(), true);
				return;
			default:
				return;
			}
		}

		public void OnAdvertisementPressed(Advertisement ad)
		{
			System.Diagnostics.Debug.WriteLine("Ad pressed: " + ad.Description);

			if (ad.AdType == AdvertisementType.None)
			{
				// Just an image, do nothing
			}
			else if (ad.AdType == AdvertisementType.ItemId)
			{
				if (AppData.MobileMenu == null)
					return;
					
				MenuItem menuItem = null;

				foreach (Menu menuToSearch in AppData.MobileMenu.Menus)
				{
					menuItem = menuToSearch.GetMenuItem(ad.AdValue, MenuNode.ItemType.ItemOrRecipe);
					if (menuItem != null)
						break;
				}

				if (menuItem == null)
				{
					System.Diagnostics.Debug.WriteLine("COULDN'T FIND MENUITEM: " + ad.AdValue);
					return;
				}

				// We have a MenuItem to display
				
				ItemDetailsScreen detailsScreen = new ItemDetailsScreen (menuItem);
				this.NavigationController.PushViewController (detailsScreen, true);
			}
			else if (ad.AdType == AdvertisementType.Deal)
			{
				if (AppData.MobileMenu == null)
					return;

				MenuItem menuItem = null;

				foreach (Menu menuToSearch in AppData.MobileMenu.Menus)
				{
					menuItem = menuToSearch.GetMenuItem(ad.AdValue, MenuNode.ItemType.Deal);
					if (menuItem != null)
						break;
				}

				if (menuItem == null)
				{
					System.Diagnostics.Debug.WriteLine("COULDN'T FIND MENUITEM: " + ad.AdValue);
					return;
				}

				// We have a MenuItem to display

				ItemDetailsScreen detailsScreen = new ItemDetailsScreen (menuItem);
				this.NavigationController.PushViewController (detailsScreen, true);
			}
			else if (ad.AdType == AdvertisementType.MenuNodeId)
			{
				if (AppData.MobileMenu == null)
					return;
					
				MenuNode menuNode = null;

				foreach (Menu menuToSearch in AppData.MobileMenu.Menus)
				{
					menuNode = menuToSearch.GetMenuNode(ad.AdValue);
					if (menuNode != null)
						break;
				}
					
				if (menuNode == null)
				{
					System.Diagnostics.Debug.WriteLine("COULDN'T FIND MENUNODE: " + ad.AdValue);
					return;
				}

				// We have a MenuNode to display

				// NOTE: The MenuNode should always be a MenuGroupNode - if we want to advertise an item then we'd use an item advertisement
				if (menuNode is MenuGroupNode)
				{
					MenuGroupNode myGroupNode = menuNode as MenuGroupNode;
					MenuScreen menuScreen = new MenuScreen (new UICollectionViewFlowLayout(), myGroupNode.MenuNodes, myGroupNode.Description, CardCollectionCell.CellSizes.TallWide, false);
					this.NavigationController.PushViewController (menuScreen, true);
				} 
			}
			else if (ad.AdType == AdvertisementType.Url)
			{
				WebViewScreen webViewScreen = new WebViewScreen(ad.AdValue);
				this.NavigationController.PushViewController (webViewScreen, true);
			}
		}

		private List<HomeScreen2TableSource.ShortcutIds> GetShortcutList()
		{
			// TODO Get this list from webservice
			List<HomeScreen2TableSource.ShortcutIds> shortcutList = new List<HomeScreen2TableSource.ShortcutIds>();
			shortcutList.Add(HomeScreen2TableSource.ShortcutIds.Menu);
			shortcutList.Add(HomeScreen2TableSource.ShortcutIds.Favorites);
			return shortcutList;
		}

		private void LoadAdvertisements()
		{
			// TODO Check advertisement expiration date ... filter out expired ads

			// Load advertisements to header page view

			int pageCount = 0;
			nfloat pageOffset = 0;
			nfloat pageWidth = this.headerPageView.Bounds.Width;

			foreach (var ad in this.advertisements)
			{
				AdvertisementView adView = new AdvertisementView(new CGRect(pageOffset, 0, this.headerPageView.Bounds.Width, this.headerPageView.Bounds.Height), this.headerPageIndicatorHeight);

				if (String.IsNullOrEmpty(ad.Description))
					adView.HideDescriptionText();
				else
					adView.SetDescriptionText(ad.Description);

				adView.AdImageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(ad.ImageView.AvgColor);
				LoadImageToImageView(ad.ImageView.Id, adView.AdImageView, new ImageSize(700, 500));

				adView.AddGestureRecognizer(new UITapGestureRecognizer(() => { OnAdvertisementPressed(ad); }));

				this.headerPageView.AddSubview(adView);
				pageOffset += pageWidth;
				pageCount++;
			}

			this.headerPageView.ContentSize = new CGSize(pageOffset, this.headerPageView.Bounds.Height);

			// Set up page indicator
			this.headerPageIndicator.Pages = pageCount;
			if (this.headerPageIndicator.Pages == 0)
				this.headerPageIndicator.Hidden = true;

			this.headerPageView.Scrolled += (object sender, EventArgs e) => {
			
				int page = (int)Math.Floor((this.headerPageView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
				this.headerPageIndicator.CurrentPage = page;

				// HACK: We're scrolling away from the dropshadow, let's add it continuously as we scroll
				//Utils.UI.AddDropShadowToView(this.headerPageView);

				// Restart timer
				if (this.timer != null)
				{
					this.timer.Stop();
					this.timer.Start();
				}
			};
				
			this.scrollToNextAd = new Action(() => { 
			
				int currentPage = (int)Math.Floor((this.headerPageView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
				int nextPage = currentPage + 1;
				if (nextPage > pageCount - 1)
					nextPage = 0;

				System.Diagnostics.Debug.WriteLine("Ads - currentpage, nextpage: " + currentPage.ToString() + " " + nextPage.ToString());

				this.headerPageView.ScrollRectToVisible(new CGRect(nextPage * pageWidth, 0, this.headerPageView.Bounds.Width, this.headerPageView.Bounds.Height), true);
			});

			SetupAdAutoScroll();
		}

		private void SetupAdAutoScroll()
		{
			this.timer = new System.Timers.Timer(5000);
			timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => { InvokeOnMainThread(() => { this.scrollToNextAd(); }); };
			timer.Enabled = true;
		}
			
		private void LoadImageToImageView(string imageId, UIImageView imageView, ImageSize imageSize)
		{
			new Models.ImageModel().ImageGetById (imageId, imageSize, (dloadedImageView, destinationId) => {
			
				imageView.Image = Utils.Image.FromBase64(dloadedImageView.Image);

				CATransition transition = new CATransition ();
				transition.Duration = 0.5f;
				transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				transition.Type = CATransition.TransitionFade;
				imageView.Layer.AddAnimation (transition, null);

			}, 
				() => { /* Failure, do nothing (for the moment at least) */ }
			);
		}

		private void GetData()
		{
			Utils.UI.ShowLoadingIndicator();

			new AdvertisementModel().AdvertisementsGetById(string.Empty, string.Empty,  
				(adList) => { 
					System.Diagnostics.Debug.WriteLine("Get advertisements success, count: " + adList.Count.ToString());
					this.advertisements = adList;
					Utils.UI.HideLoadingIndicator();
					LoadAdvertisements();
				},
				() => { 
					Utils.UI.HideLoadingIndicator();
				});
		}
			
		private class AdvertisementView : UIView
		{
			private UILabel lblText;
			private UIView overlayView;
			private UIImageView adImageView;

			public UIImageView AdImageView { get { return this.adImageView; } }

			private float pageIndicatorHeight;

			public AdvertisementView(CGRect frame, float pageIndicatorHeight)
			{
				this.Frame = frame;
				this.pageIndicatorHeight = pageIndicatorHeight;
				this.Layer.MasksToBounds = true;
				SetLayout();
			}

			private void SetLayout()
			{
				// Image view
				this.adImageView = new UIImageView();
				this.adImageView.Frame = this.Bounds;
				this.adImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
				this.AddSubview(this.adImageView);

				// Overlay view
				this.overlayView = new UIView ();
				float overlayViewHeight = (float)Math.Floor (this.Bounds.Height / 8) + this.pageIndicatorHeight;
				this.overlayView.Frame = new CGRect (0, this.Bounds.Bottom - overlayViewHeight, this.Bounds.Width, overlayViewHeight);
				this.overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
				this.AddSubview (this.overlayView);

				// Text label
				this.lblText = new UILabel();
				this.lblText.Frame = new CGRect(5f, 0f, overlayView.Bounds.Width - 2 * 5f, overlayView.Bounds.Height - this.pageIndicatorHeight);
				this.lblText.TextColor = UIColor.White;
				this.lblText.Font = UIFont.FromName ("Helvetica", 14);
				this.lblText.TextAlignment = UITextAlignment.Left;
				this.lblText.BackgroundColor = UIColor.Clear;
				overlayView.AddSubview (this.lblText);
			}

			public void SetDescriptionText(string text)
			{
				this.lblText.Text = text;
			}

			public void ShowDescriptionText()
			{
				this.overlayView.Hidden = false;
			}

			public void HideDescriptionText()
			{
				this.overlayView.Hidden = true;
			}
		}

		/* NOT IN USE - USE SCROLLVIEW WITH PAGING ENABLED INSTEAD
		// TODO Move to separate .cs file
		private class PagingScrollView : UIScrollView
		{
			private float pageWidth { get { return this.Bounds.Width; } } 
			private float pageOffset;
			private int pageCount;
			private int currentPage;

			public SizeF PageSize { get { return new SizeF(this.Bounds.Width, this.Bounds.Height); } }

			private PointF currentPoint;
			private PointF currentOffset;
			private PointF beginPoint;
			private PointF newPoint;

			private UIPageControl pageIndicator;

			public PagingScrollView()
			{
				this.pageOffset = 0f;
				this.pageCount = 0;
				this.currentPage = 0;
				this.pageIndicator = new UIPageControl();
				this.AddSubview(this.pageIndicator);
				SetupScrollHandling();
			}
				
			public void AddViewToNewPage (UIView view)
			{
				view.Frame = new RectangleF(this.pageOffset, 0f, view.Bounds.Width, view.Bounds.Height);
				this.pageOffset += this.pageWidth;
				this.ContentSize = new SizeF(this.pageOffset, this.Bounds.Height);
				this.AddSubview(view);

				this.pageCount++;
			}

			/// <summary>
			/// Setup our own scroll handling
			/// </summary>
			private void SetupScrollHandling()
			{
				this.ScrollEnabled = false;
				UIPanGestureRecognizer panRecognizer = new UIPanGestureRecognizer();
				panRecognizer.AddTarget(() => { HandleScroll(panRecognizer); });
				this.AddGestureRecognizer(panRecognizer);
			}
				
			private void HandleScroll(UIPanGestureRecognizer recognizer)
			{
				if (recognizer.State == UIGestureRecognizerState.Began)
				{
					this.beginPoint = recognizer.TranslationInView (this);
					this.currentOffset.X = this.ContentOffset.X;
				}

				if (recognizer.State != (UIGestureRecognizerState.Ended | UIGestureRecognizerState.Cancelled |UIGestureRecognizerState.Failed))
				{
					this.newPoint = recognizer.TranslationInView (this);
					this.currentPoint.X = this.beginPoint.X - this.newPoint.X + this.currentOffset.X;
					this.SetContentOffset (this.currentPoint, false);
				}

				if (recognizer.State == UIGestureRecognizerState.Ended)
				{
					float scrollLength = this.beginPoint.X - this.newPoint.X;

					if (scrollLength >= 60f)
					{
						// Scrolling forward

						if (this.currentPage < (this.pageCount - 1))
						{
							// Scroll to next page

							this.currentPoint.X = (this.currentPage + 1) * this.pageWidth;
							this.SetContentOffset (this.currentPoint, true);
							this.currentPage++;
							this.pageIndicator.CurrentPage = this.currentPage;
						}
						else
						{
							// Reached end - can't scroll further - stay on the current page

							this.currentPoint.X = this.currentPage * this.pageWidth;
							this.SetContentOffset (this.currentPoint, true);
						}
					}
					else if (scrollLength <= -60f)
					{
						// Scrolling backwards

						if (this.currentPage != 0) 
						{
							// Scroll to previous page

							this.currentPoint.X = (this.currentPage - 1) * this.pageWidth;
							this.SetContentOffset (this.currentPoint, true);
							this.currentPage--;
							this.pageIndicator.CurrentPage = this.currentPage;
						}
						else
						{
							// Reached end - can't scroll further - stay on the current page

							this.currentPoint.X = this.currentPage * this.pageWidth;
							this.SetContentOffset (this.currentPoint, true);
						}
					}
					else
					{
						// Not scrolling long enough to switch page - stay on the current page

						this.currentPoint.X = this.currentPage * this.pageWidth;
						this.SetContentOffset (this.currentPoint, true);
					}
				}
			}
		}
		*/
	}
}

