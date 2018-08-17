using System;
using Foundation;
using UIKit;
using Presentation.Screens;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation
{
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		public RootTabBarController RootTabBarController { get; set; }

		public override UIWindow Window 
		{
			get 
			{
				return this.window;
			}
			set 
			{
				throw new NotImplementedException ();
			}
		}
	
		// Enabled items
		public bool HomeScreenEnabled { get { return true; } }
		public bool BasketEnabled { get { return true; } }
		public bool UseAccountViewInSlideoutMenu { get { return false; } }
		public bool ShowLoyaltyPoints { get { return true; } }
		public bool OffersAndCouponsEnabled { get { return true; } }
		public bool AboutUsControllerEnabled { get { return true; } }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// Create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			// Create database
			Infrastructure.Data.SQLite2.DB.CreateTables();

			//Initialize Web Service
			InitWebService();

			this.RootTabBarController = new RootTabBarController();
			window.RootViewController = RootTabBarController;

			// Make the status bar text white
			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.LightContent, false);

			// Load data from DB
			// OMNI-4096 - BasketView needs the mobile menu in order to display itself properly; so make sure it is loaded before attempting to display it
			new MenuModel().LoadLocalMenuAsync().Wait();
			new TransactionModel().LoadLocalTransactions();
			new FavoriteModel().LoadLocalFavorites();
			new ContactModel().LoadLocalContact();
			if (this.BasketEnabled)
				new Models.BasketModel().LoadLocalBasket();

			// Make the window visible
			window.MakeKeyAndVisible ();

			// We only display the Welcome PopUp View for the first time the app is runned
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey(new NSString("WelcomeMsgPresentedHospLoy")))
			{
				WelcomePopUpView welcomePopUp = new WelcomePopUpView();
				welcomePopUp.WelcomeMsgDismissed = () =>
				{
					NSUserDefaults.StandardUserDefaults.SetBool(true, "WelcomeMsgPresentedHospLoy");
					welcomePopUp.HideWithAnimation();
					welcomePopUp.RemoveFromSuperview();
				};
				this.window.AddSubview(welcomePopUp);

				nfloat popupMargin = 20f;

				welcomePopUp.SetFrame(
					new CoreGraphics.CGRect(
						popupMargin,
						popupMargin,
						this.window.Frame.Width - 2 * popupMargin,
						this.window.Frame.Height - 2 * popupMargin
					)
				);

				welcomePopUp.ShowWithAnimation();
			}
		
			return true;
		}
			
		public UIInterfaceOrientation DeviceOrientation { get { return UIApplication.SharedApplication.StatusBarOrientation; } }

		public nfloat DeviceScreenWidth { get { return UIScreen.MainScreen.Bounds.Width; } }
		public nfloat DeviceScreenHeight { get { return UIScreen.MainScreen.Bounds.Height; } }
		public nfloat ScreenHeight4Inch { get { return 568; } }
		public nfloat ScreenHeight35Inch { get { return 480; } }
		public nfloat StatusbarPlusNavbarHeight { get { return 64f; } }

		public static void InitWebService()
		{
			var url = Settings.GetBaseURL ();
			var uniqueId = Utils.Util.PhoneId;
			LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(
				uniqueId,
				LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.HospitalityLoyalty, 
				url);
		}
	}
}

