using System;
using CoreGraphics;
using UIKit;
using CoreAnimation;
using ObjCRuntime;
using Foundation;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Utils
{
    public static class UI
	{
		private static BannerView addedToBasketBannerView { get; set;}
		private static BannerView addedToWishListBannerView { get; set; }

		private static NotificationBannerView notificationBannerView { get; set;}
		public static Timer notificationBannerViewTimer;

		private static UIView loadingIndicatorView { get; set; }

		public static UIColor NavigationBarContentColor { get { return Utils.AppColors.PrimaryColor; } }
		public static UIColor NavigationBarBackgroundColor { get { return UIColor.Clear; } }

		static UI()
		{
			addedToBasketBannerView = new BannerView();
			addedToBasketBannerView.BannerViewClicked += AddedToBasketBannerViewClicked;

			addedToWishListBannerView = new BannerView();
			addedToWishListBannerView.BannerViewClicked += AddedToWishListBannerViewClicked;

			notificationBannerView = new NotificationBannerView ();
			notificationBannerView.NotificationPressed += (notificationId) => {

				System.Diagnostics.Debug.WriteLine("Notification banner pressed, notification with ID: " + notificationId);
				Utils.Util.AppDelegate.PresentNotification(notificationId);

			};
			loadingIndicatorView = GetLoadingIndicatorView(new CGPoint(Utils.Util.AppDelegate.DeviceScreenWidth/2, Utils.Util.AppDelegate.DeviceScreenHeight/2), 100f, 70f, true);
		}

		public static void StyleNavigationBar(UINavigationBar bar)
		{
			bar.TitleTextAttributes = TextUtilities.TitleTextAttributes(NavigationBarContentColor);
			//bar.BarTintColor = NavigationBarBackgroundColor;
			bar.Translucent = true;
			bar.TintColor = NavigationBarContentColor;	
		}

		public static void ShowLoadingIndicator()
		{
			new Foundation.NSObject().InvokeOnMainThread(() => {
			
				Utils.Util.AppDelegate.Window.AddSubview(loadingIndicatorView);
			});

		}

		public static void HideLoadingIndicator()
		{
			new Foundation.NSObject().InvokeOnMainThread(() => {

				loadingIndicatorView.RemoveFromSuperview();
			});
		}

		public static void ShowNetworkActivityIndicator()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		public static void HideNetworkActivityIndicator()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		public static void ShowAddedToBasketBannerView(string textToDisplay, UIImage image)
		{
			new Foundation.NSObject().InvokeOnMainThread(() => {

				Utils.Util.AppDelegate.Window.AddSubview(addedToBasketBannerView);
				addedToBasketBannerView.Show(textToDisplay, image);
			});
		}

		private static void AddedToBasketBannerViewClicked()
		{
			int basketIndex = Utils.Util.AppDelegate.RootTabBarController.controllersToShow.FindIndex(x => x is BasketController);
			Utils.Util.AppDelegate.RootTabBarController.SelectedIndex = basketIndex;
		}

		public static void ShowAddedToWishListBannerView(string textToDisplay, UIImage image)
		{
			new Foundation.NSObject().InvokeOnMainThread(() => {

				Utils.Util.AppDelegate.Window.AddSubview(addedToWishListBannerView);
				addedToWishListBannerView.Show(textToDisplay, image);
			});
		}

		private static void AddedToWishListBannerViewClicked()
		{
			int wishListIndex = Utils.Util.AppDelegate.RootTabBarController.controllersToShow.FindIndex(x => x is WishListController);

			if(wishListIndex > 3)
			{
				CustomMoreController moreController = Utils.Util.AppDelegate.RootTabBarController.controllersForTabBar[4] as CustomMoreController;

				Utils.Util.AppDelegate.RootTabBarController.SelectedIndex = 4; //go to more controller
				moreController.PresentController(wishListIndex - 4);
			}
			else
			{
				Utils.Util.AppDelegate.RootTabBarController.SelectedIndex = wishListIndex;
			}
		}
			
		public static void ShowNotificationBannerView(string textToDisplay, string notificationID)
		{
			new Foundation.NSObject().InvokeOnMainThread(() => {

				//NotificationBanner.RemoveFromSuperViewOnHide = true;
				Utils.Util.AppDelegate.Window.AddSubview(notificationBannerView);
				notificationBannerView.Show(textToDisplay, notificationID);
			});
		}

		public static void HideNotificationBannerView()
		{
			new Foundation.NSObject ().InvokeOnMainThread (() => {
				notificationBannerView.Hide(false);
			});
		}
			
		

		public static UIView GetLoadingIndicatorView(CGPoint centerPoint, float width, float height, bool darkBackground = false, bool whiteIndicator = false)
		{
			//Background view
			UIView backgroundView = new UIView();
			backgroundView.Frame = Utils.Util.AppDelegate.Window.Bounds;

			// Loading indicator container view

			UIView indicatorContainerView = new UIView();
			indicatorContainerView.Frame = new CGRect(centerPoint.X - width/2, centerPoint.Y - height/2, width, height);
			indicatorContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

			if (darkBackground)
				indicatorContainerView.BackgroundColor = Utils.AppColors.TransparentBlack3;
			else
				indicatorContainerView.BackgroundColor = UIColor.Clear;

			indicatorContainerView.Layer.CornerRadius = 5;

			// Loading indicator

			float indicatorViewDimensions = 30f;
			UIActivityIndicatorView loadingIndicator = new UIActivityIndicatorView();
			loadingIndicator.Frame = new CGRect(indicatorContainerView.Frame.Width/2 - indicatorViewDimensions/2, indicatorContainerView.Frame.Height/2 - indicatorViewDimensions/2, indicatorViewDimensions, indicatorViewDimensions);

			if (darkBackground)
				loadingIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
			else
				if(whiteIndicator)
					loadingIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
				else
					loadingIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;

			loadingIndicator.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			loadingIndicator.StartAnimating();

			indicatorContainerView.AddSubview(loadingIndicator);
			backgroundView.AddSubview(indicatorContainerView);

			return backgroundView;
		}

		public static void SetLoadingIndicator(UIView view ,bool whiteIndicator)
		{
			/*
			UIView indicatorContainerView = new UIView();
			indicatorContainerView.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);
			indicatorContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			indicatorContainerView.Tag = 666;

			indicatorContainerView.Layer.CornerRadius = 5;
			*/

			float indicatorViewDimensions = 30f;
			UIActivityIndicatorView loadingIndicator = new UIActivityIndicatorView();
			loadingIndicator.Tag = 666;
			loadingIndicator.Frame = new CGRect(view.Frame.Width/2 - indicatorViewDimensions/2, view.Frame.Height/2 - indicatorViewDimensions/2, indicatorViewDimensions, indicatorViewDimensions);

			if(whiteIndicator)
				loadingIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
			else
				loadingIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;

			loadingIndicator.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			loadingIndicator.StartAnimating();

			view.AddSubview(loadingIndicator);
		}



		public static void StartNetworkActivityIndicator()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		public static void StopNetworkActivityIndicator()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		

		/// <summary>
		/// Loads an image with the specified ID to the specified image view.
		/// </summary>
		/// <param name="imageId">Image identifier.</param>
		/// <param name="localImage">If set to <c>true</c> then get the image from file.</param>
		/// <param name="imageView">Image view.</param>
		/// <param name="imageSize">Image size.</param>
		/// <param name="destinationId">Destination identifier. Can be used to further identify the destination imageview. Useful when using reusable cells in e.g. a TableView.</param>
		public static void LoadImageToImageView(string imageId, bool localImage, UIImageView imageView, ImageSize imageSize, string destinationId = null)
		{
			if (localImage)
			{
               UIImage x = new Models.ImageModel().GetImageByIdFromFile(imageId);
                if (x != null) {

					imageView.Image = x;

					CATransition transition = new CATransition ();
					transition.Duration = 0.5f;
					transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
					transition.Type = CATransition.TransitionFade;
					imageView.Layer.AddAnimation (transition, null);
				}
					else
                { // Failure, do nothing (for the moment at least) 
				}
			}
			else
			{
				new Models.ImageModel ().ImageGetById (imageId, imageSize, (dloadedImageView, dloadedImageViewDestinationId) => {

					if (dloadedImageViewDestinationId == destinationId)
					{
						// This is the correct image for this imageview, let's apply it

						imageView.Image = ImageUtilities.FromBase64(dloadedImageView.Image);

						CATransition transition = new CATransition ();
						transition.Duration = 0.5f;
						transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
						transition.Type = CATransition.TransitionFade;
						imageView.Layer.AddAnimation (transition, null);
					}
				}, 
					() => { // Failure, do nothing (for the moment at least) 
								},
					destinationId
				);
			}
		}


		public static void AddFadeTransitionToView(UIView view, double duration = 0.5f)
		{
			CATransition transition = new CATransition ();
			transition.Duration = duration;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			transition.Type = CATransition.TransitionFade;
			view.Layer.AddAnimation (transition, null);
		}
			

		public static UIImage MapCellSizeToIcon(Presentation.Screens.CardCollectionCell.CellSizes cellSize)
		{
			switch (cellSize)
			{
			case Presentation.Screens.CardCollectionCell.CellSizes.TallWide:
                    return UIImage.FromBundle("ViewLargeCellsIcon");
			case Presentation.Screens.CardCollectionCell.CellSizes.TallNarrow:
                    return UIImage.FromBundle("ViewGridIcon");
			case Presentation.Screens.CardCollectionCell.CellSizes.ShortWide:
                    return UIImage.FromBundle("ViewSmallCellsIcon");

			default:
                    return UIImage.FromBundle("ViewSmallCellsIcon");
			}
		}

		

		public static nfloat GetLabelHeight(string labelText, UIFont font)
		{
			// Let's get the height of the label by creating a templabel that is exactly like the one used and then apply SizeToFit().

			UILabel tempLabel = new UILabel();
			tempLabel.Text = labelText;
			tempLabel.Font = font;
			tempLabel.Lines = Utils.Util.GetStringLineCount(labelText);
			tempLabel.SizeToFit();
			return tempLabel.Frame.Height;
		}

		public static void AddColorGradientToView(UIView view, UIColor color1, UIColor color2, float endPointX = 0.5f, float endPointY = 0.2f)
		{			
			CoreAnimation.CAGradientLayer gradientLayer = new CoreAnimation.CAGradientLayer ();
			gradientLayer.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);
			CoreGraphics.CGColor[] colors = new CoreGraphics.CGColor[2];
			colors [0] = color1.CGColor;
			colors [1] = color2.CGColor;
			gradientLayer.Colors = colors;
			gradientLayer.EndPoint = new CGPoint (endPointX, endPointY);
			view.Layer.InsertSublayer(gradientLayer, 0);
		}

		public static UIImage GetLaunchImage()
		{
			if(UIScreen.MainScreen.Bounds.Height == 480)
				return UIImage.FromFile(@"LaunchImage@2x.png"); //iPhone4/4s - 3.5 inches
			else if(UIScreen.MainScreen.Bounds.Height == 568)
				return UIImage.FromFile(@"LaunchImage-700-568h@2x.png"); //iPhone 5/5s - 4.0 inches
			else if(UIScreen.MainScreen.Bounds.Height == 667)
				return UIImage.FromFile(@"LaunchImage-568h@2x.png"); //iPhone 6 - 4.7 inches
			else if(UIScreen.MainScreen.Bounds.Height == 736)
				return UIImage.FromFile(@"LaunchImage-700-568h@2x.png"); //iPhone 6+ - 5.5 inches
			else
				return null;
		}

		

		public static void AddDismissSelfButtonToController(UIViewController controller, bool addOnLeftSide)
		{
			UIBarButtonItem closeButton = new UIBarButtonItem ();
			closeButton.Title = LocalizationUtilities.LocalizedString("General_Close", "Close");
			closeButton.Clicked += (object sender, EventArgs e) => 
			{
				controller.DismissViewController(true, null);
			};
				
			if (controller.NavigationItem != null)
			{
				if (addOnLeftSide)
				{
					// Add on the left side

					if (controller.NavigationItem.LeftBarButtonItems != null && controller.NavigationItem.LeftBarButtonItems.Count() > 0)
					{	
						// There are some bar button items already present, must add this new button to the list
						List<UIBarButtonItem> currentLeftBarButtonItems = new List<UIBarButtonItem>();
						foreach (var currentItem in controller.NavigationItem.LeftBarButtonItems)
							currentLeftBarButtonItems.Add(currentItem);

						// Add the new button to the list
						currentLeftBarButtonItems.Add(closeButton);

						controller.NavigationItem.SetLeftBarButtonItems(currentLeftBarButtonItems.ToArray(), false);
					}
					else
					{						
						controller.NavigationItem.LeftBarButtonItem = closeButton;
					}					
				}
				else
				{
					// Add on the right side

					if (controller.NavigationItem.RightBarButtonItems != null && controller.NavigationItem.RightBarButtonItems.Count() > 0)
					{	
						// There are some bar button items already present, must add this new button to the list
						List<UIBarButtonItem> currentRightBarButtonItems = new List<UIBarButtonItem>();
						foreach (var currentItem in controller.NavigationItem.RightBarButtonItems)
							currentRightBarButtonItems.Add(currentItem);

						// Add the new button to the list
						currentRightBarButtonItems.Add(closeButton);

						controller.NavigationItem.SetRightBarButtonItems(currentRightBarButtonItems.ToArray(), false);
					}
					else
					{						
						controller.NavigationItem.RightBarButtonItem = closeButton;
					}
				}
			}
		}

		private class NotificationBannerView : UIView
		{
			private UILabel lblText;
			private UIImageView image;
			private UIView clickableView;
			private string notificationID;
			nfloat frameWidth = Utils.Util.AppDelegate.Window.Frame.Width;
			nfloat frameHeight = Utils.Util.AppDelegate.StatusbarPlusNavbarHeight;

			public delegate void NotificationPressedEventHandler(string notificationId);
			public event NotificationPressedEventHandler NotificationPressed;

			public NotificationBannerView()
			{
				this.BackgroundColor = UIColor.Black;

				// Text label
				this.lblText = new UILabel();
				this.lblText.BackgroundColor = UIColor.Clear;
				this.lblText.TextColor = UIColor.White;
				this.lblText.Font = UIFont.SystemFontOfSize(15);

				// Image view
				this.image = new UIImageView();
				this.image.Image = UIImage.FromBundle("Icon-Small.png");
				this.image.ContentMode = UIViewContentMode.ScaleAspectFill;

				//Clickable View
				clickableView  = new UIView();
				clickableView.BackgroundColor = UIColor.Clear;
				clickableView.AddGestureRecognizer (
					new UITapGestureRecognizer( 
						() => 
						{
							if (this.NotificationPressed != null)
								this.NotificationPressed(notificationID);

							// Disable the timer
							notificationBannerViewTimer.Enabled = false;
							this.Hide(true);
						}
					)
				);

				this.AddSubview(image);
				this.AddSubview(lblText);
				this.AddSubview(clickableView);
			}
				
			private void SetLayout()
			{				
				this.Frame = new CGRect (0, 0, frameWidth, frameHeight);
				this.clickableView.Frame = new CGRect(0, 0, this.Frame.Width, this.Frame.Height);

				nfloat topMargin = Utils.Util.AppDelegate.StatusbarHeight;
				nfloat xMargin = 15f;
				nfloat contentHeight = this.Frame.Height - topMargin;

				float imageDimensions = 20f;
				this.image.Frame = new CGRect(xMargin, topMargin + contentHeight/2 - imageDimensions/2, imageDimensions, imageDimensions);

				this.lblText.Frame = new CGRect(
					this.image.Frame.Right + xMargin,
					this.image.Frame.Top,
					this.Frame.Width - this.image.Frame.Right - 2 * xMargin,
					20f
				);

				// Hide it above the screen
				this.Frame = new CGRect (0, -this.Frame.Height, this.Frame.Width, this.Frame.Height);
			}

			private void SetText(String text)
			{				
				this.lblText.Text = text;
			}

			private void ClearText ()
			{
				this.lblText.Text = string.Empty;
			}

			private void SetNotificationID(string notificationID)
			{
				this.notificationID = notificationID;
			}

			public void Show (string textToDisplay, string notificationID)
			{
				SetNotificationID(notificationID);
				SetText(textToDisplay);

				SetLayout();

				// Animate into view
				UIView.BeginAnimations("slideAnimation");
				UIView.SetAnimationDuration (1);
				this.Frame = new CGRect (0, 0, frameWidth, frameHeight);	// Set new frame position (so it is visible)
				UIView.CommitAnimations ();
			}

			public void Hide(bool clicked)
			{
				new NSObject ().InvokeOnMainThread (() => {
					if(!clicked)
					{
						// Animation to hide notification banner
						UIView.BeginAnimations("slideAnimation");
						UIView.SetAnimationDuration (2);

						// Intialize the frame for animation
						this.Frame = new CGRect (0, -this.Frame.Height, this.Frame.Width, this.Frame.Height);
						notificationBannerViewTimer.Enabled = false;

						//Set up the animation delegate
						UIView.SetAnimationDelegate (this);
						UIView.SetAnimationDidStopSelector (new Selector ("slideAnimationFinished"));

						// Commit animation
						UIView.CommitAnimations ();
					}
					else
					{
						this.RemoveFromSuperview();
					}
					notificationBannerViewTimer.Enabled = false;
				});
			}

			[Export("slideAnimationFinished")]
			void SlideStopped ()
			{
				this.RemoveFromSuperview ();
			}
		}			
	}
}

