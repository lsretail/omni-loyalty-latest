using System;
using System.Linq;
using Foundation;
using UIKit;
using Presentation.Screens;
using Presentation.Models;
using Presentation.Utils;
using Infrastructure.Data.SQLite2.Devices;
using System.Timers;
using CoreGraphics;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Firebase.CloudMessaging;
using UserNotifications;
using Firebase.Core;
using Firebase.InstanceID;

namespace Presentation
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate, IMessagingDelegate, IUNUserNotificationCenterDelegate
    {
        UIWindow window;

        public RootTabBarController RootTabBarController { get; set; }

        private const string XAMARIN_INSIGHTS_API_KEY = "[Please enter the API KEY here]";

		public override UIWindow Window
        {
            get
            {
                return this.window;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        // iOS flags that don't neccessary belong in Domain.EnabledItems
        public bool ShowLoyaltyPoints { get { return true; } }
        public bool UsePushNotifications { get { return true; } }
        public bool GetItemDetailsAtItemListScreen { get { return true; } }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            // Create database
            Infrastructure.Data.SQLite2.DB.CreateTables();

            //Initialize Web Service
            InitWebService();

			// Google firebase cloud messaging
            /*
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.Delegate = this;

            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }


            App.Configure();
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            */

            AppData.Device = new DeviceService(new DeviceRepository()).GetDevice();

            if (!(AppData.Device is UnknownDevice))
            {
                //Get Contact from WS
                if (AppData.UserLoggedIn)
                {
                    new Models.ContactModel().GetMemberContact(AppData.Device.UserLoggedOnToDevice.Id);

                    if (UsePushNotifications)
                    {
                        SendFcmToServer(InstanceId.SharedInstance.Token);
                    }
                }
            }

            this.RootTabBarController = new RootTabBarController();
            window.RootViewController = RootTabBarController;

            // Set status bar text color
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, false);

            // Make the window visible
            window.MakeKeyAndVisible();

#region Xamarin Insights

            // See: https://insights.xamarin.com/docs

            Xamarin.Insights.Initialize(XAMARIN_INSIGHTS_API_KEY);

            // This handles startup crashes
            // If the app crashes on startup there probably wasn't enough time to send the crash report to Insights.
            // This checks for pending crash reports and sends them to Insights.
            // Note that this uses a blocking call, so the app's startup time will be longer.
            Xamarin.Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
            {
                if (isStartupCrash)
                    Xamarin.Insights.PurgePendingCrashReports().Wait(); // This is a blocking call		
            };

#endregion

#region Push notifications

            // Process push notification that arrives when app is not running
            if (options != null && options.ContainsKey(new NSString("UIApplicationLaunchOptionsRemoteNotificationKey")))
            {
                NSDictionary pushNotification = options.ObjectForKey(new NSString("UIApplicationLaunchOptionsRemoteNotificationKey")) as NSDictionary;
                if (pushNotification != null)
                    HandlePushNotification(false, pushNotification);
            }

#endregion

            // We only display the Welcome PopUp View for the first time the app is runned
            if (!NSUserDefaults.StandardUserDefaults.BoolForKey(new NSString("WelcomeMsgPresentedLoyalty")))
            {
                WelcomePopUp welcomePopUp = new WelcomePopUp();
                welcomePopUp.WelcomeMsgDismissed = () =>
                {
                    NSUserDefaults.StandardUserDefaults.SetBool(true, "WelcomeMsgPresentedLoyalty");

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


            //Display the launch image for a little while longer - or just until the Login modal view controller is visable
            // - otherwise, the rootview appears for a little while, and then the login screen appears

            LaunchImageView launchImageView = new LaunchImageView();
            launchImageView.Frame = new CGRect(0f, 0f, this.window.Frame.Width, this.window.Frame.Height);
            this.window.AddSubview(launchImageView);

            UIView.Animate(
                0.1f,
                0.5f,
                UIViewAnimationOptions.TransitionNone,
                () =>
                {
                    //animation
                    launchImageView.Alpha = 0f;
                },
                () =>
                {
                    //completion
                    launchImageView.RemoveFromSuperview();
                }
            );

            return true;
        }

#region Push notifications 


        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            HandlePushNotification(
                application.ApplicationState.ToString() == "Active",
                userInfo
            );
        }

        private void HandlePushNotification(bool appIsActive, NSDictionary pushNotification)
        {
            if (pushNotification == null)
                return;

            System.Diagnostics.Debug.WriteLine("AppDelegate.HandlePushNotification() - Push notification received:" + System.Environment.NewLine + pushNotification.ToString());

            // Get the aps node
            NSDictionary aps = null;
            if (pushNotification.ContainsKey(new NSString("aps")))
            {
                aps = pushNotification.ObjectForKey(new NSString("aps")) as NSDictionary;
            }

            if (aps == null)
            {
                // The main notification content is null, abort
                return;
            }

            /* We don't need to update the badge manually, iOS does that for us.
			 * The only reason we need to get the badge number from the notification payload
			 * ourselves would be to use it for some other logic.
			// Update badge
			if (aps.ContainsKey(new NSString("badge")))
			{
				int badge = -1; 
				string sBadge = (aps.ObjectForKey(new NSString("badge")) as NSObject).ToString();
				if (int.TryParse(sBadge, out badge))
				{
					if (badge >= 0)
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
				}
			}
			*/

            // Get the alert text
            string alert = string.Empty;
            if (aps.ContainsKey(new NSString("alert")))
            {
                alert = aps.ObjectForKey(new NSString("alert")).ToString();
            }

            // Get the Omni ID
            string omniId = string.Empty;
            if (pushNotification.ContainsKey(new NSString("OmniId")))
            {
                omniId = pushNotification.ObjectForKey(new NSString("OmniId")).ToString();
            }

            if (appIsActive)
            {
                if (string.IsNullOrEmpty(alert))
                {
                    // We don't have any text to display to the user, let's abort
                    return;
                }

                // We want to show our custom notification banner
                UI.notificationBannerViewTimer = new Timer(5000);
                UI.notificationBannerViewTimer.Enabled = true;
                UI.notificationBannerViewTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    UI.HideNotificationBannerView();
                };
                UI.ShowNotificationBannerView(alert, omniId);
            }
            else
            {
                // We just opened the app, we want to go directly to the notification screen
                if (string.IsNullOrEmpty(omniId))
                {
                    // We don't know which notification to show to the user, let's abort
                    return;
                }
                Utils.Util.AppDelegate.PresentNotification(omniId);
            }
        }
        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            if(AppData.Device.UserLoggedOnToDevice != null){
                SendFcmToServer(fcmToken);
            }
        }

        private async void SendFcmToServer(string fcmToken)
        {
            await new ContactModel().RegisterForPushNotificationsInBackendServer(fcmToken);
        }
        // To receive notifications in foreground on iOS 10 devices.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            HandlePushNotification(false, notification.Request.Content.UserInfo);
        }

        // Receive data message on iOS 10 devices.
        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            HandlePushNotification(false, remoteMessage.AppData);
        }

        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // Do your magic to handle the notification data
            HandlePushNotification(application.ApplicationState.ToString() == "Active", userInfo);
        }
#endregion

        public UIInterfaceOrientation DeviceOrientation { get { return UIApplication.SharedApplication.StatusBarOrientation; } }

        public nfloat DeviceScreenWidth { get { return UIScreen.MainScreen.Bounds.Width; } }
        public nfloat DeviceScreenHeight { get { return UIScreen.MainScreen.Bounds.Height; } }
        public nfloat ScreenHeight4Inch { get { return 568; } }
        public nfloat ScreenHeight35Inch { get { return 480; } }
        public nfloat StatusbarPlusNavbarHeight { get { return 64f; } }
        public nfloat StatusbarHeight { get { return 20f; } }

        public static void InitWebService()
        {
            var url = Settings.GetBaseURL();
            var uniqueId = Utils.Util.PhoneId;
            var languageCode = NSLocale.CurrentLocale.CountryCode;

            LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.InitWebService(
                uniqueId, 
                LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.AppType.Loyalty, 
                url, 
                0, 
                languageCode);
        }

        public void PresentNotification(string notificationId)
        {
            if (AppData.Device.UserLoggedOnToDevice == null)
                return;

            // Let's reset the badge
            // We are now handling a notification, so the notification counter on the badge should be reset
            // (we are using a simple approach to the badge numbers ... the backend isn't tracking the numbers, always sends "1"
            // so if we receive a notification while the app isn't running, we will display "1" and reset it the next time we process a push notification)
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            Notification notification = AppData.Device.UserLoggedOnToDevice.Notifications.FirstOrDefault(x => x.Id == notificationId);

            if (notification != null)
            {
                // Show notification details screen

                var notificationDetailsController = new NotificationDetailsController(notification);
                Utils.UI.AddDismissSelfButtonToController(notificationDetailsController, false);
                this.RootTabBarController.PresentViewController(new UINavigationController(notificationDetailsController), true, null);
            }
            else
            {
                // Show notification list screen

                var notificationsScreen = new NotificationsController();
                Utils.UI.AddDismissSelfButtonToController(notificationsScreen, false);
                this.RootTabBarController.PresentViewController(new UINavigationController(notificationsScreen), true, null);
            }
        }
    }
}

