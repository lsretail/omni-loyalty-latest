using System;
using CoreGraphics;
using UIKit;
using CoreAnimation;
using System.Timers;
using Foundation;
using ObjCRuntime;
using System.Linq;
using Presentation.Screens;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Utils
{
	public static class UI
	{
		private static UIView loadingIndicatorView { get; set; }
		private static AddedToBasketBannerView addedToBasketBannerView { get; set;}
		public static Timer bannerViewTimer;

		public static UIColor NavigationBarContentColor { get { return Utils.AppColors.PrimaryColor; } }
		public static UIColor NavigationBarTintColor { get { return UIColor.White; } }
		public static UIColor NavigationBarBackgroundColor { get { return UIColor.Clear; } }

		public static nfloat TabBarHeight { get; set; }

		static UI()
		{
			addedToBasketBannerView = new AddedToBasketBannerView();
			addedToBasketBannerView.BannerViewClicked += AddedToBasketBannerViewClicked;
			loadingIndicatorView = GetLoadingIndicatorView(new CGPoint(Utils.Util.AppDelegate.DeviceScreenWidth/2, Utils.Util.AppDelegate.DeviceScreenHeight/2), 100f, 70f, true);

			bannerViewTimer = new Timer(4000);
			bannerViewTimer.Elapsed += (object sender, ElapsedEventArgs e) => {

				HideAddedToBasketBannerView();
			};
		}

		public static void StyleNavigationBar(UINavigationBar bar)
		{
			bar.TitleTextAttributes = TitleTextAttributes(false);
			bar.BarTintColor = NavigationBarContentColor;
			bar.TintColor = UIColor.White;
			bar.Translucent = false;
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

		public static void ShowAddedToBasketBannerView(string textToDisplay, UIImage image)
		{
			new Foundation.NSObject().InvokeOnMainThread(() => {

				Utils.Util.AppDelegate.Window.AddSubview(addedToBasketBannerView);
				addedToBasketBannerView.Show(textToDisplay, image);
			});
		}

		public static void HideAddedToBasketBannerView()
		{
			new Foundation.NSObject ().InvokeOnMainThread (() => {
				addedToBasketBannerView.Hide(false);
			});
		}

		// White or dark title text
		public static UIStringAttributes TitleTextAttributes(bool darkText=false)
		{
			UIStringAttributes txtAttr = new UIStringAttributes();
			txtAttr.Font = UIFont.FromName("System",22f); //Arial
			txtAttr.ForegroundColor = UIColor.White;
			if (darkText)
				txtAttr.ForegroundColor = UIColor.DarkGray;
			return txtAttr;
		}

		// Custom colored title text
		public static UIStringAttributes TitleTextAttributes(UIColor textColor)
		{
			UIStringAttributes txtAttr = new UIStringAttributes();
			txtAttr.Font = UIFont.FromName("System",22f); //Arial
			txtAttr.ForegroundColor = textColor;
			return txtAttr;
		}

		/*
		public static UIBarButtonItem LeftBarButtonItem()
		{
			//usage
			//this.NavigationItem.SetLeftBarButtonItem(Utils.UI.LeftBarButtonItem(),true);

			UIBarButtonItem uiBtn = new UIBarButtonItem(" ", UIBarButtonItemStyle.Plain, null); // iOS7: Removed "Menu"
			uiBtn.Image = Utils.Image.FromFile("three_lines.png"); 
			return uiBtn;
		}
		*/


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

		public static int GetStringLineCount(string str)
		{
			return str.Split('\n').Length;
		}

		public static UIView GetLoadingIndicatorView(CGPoint centerPoint, float width, float height, bool darkBackground = false)
		{
			//Background view
			UIView backgroundView = new UIView();
			backgroundView.Frame = Utils.Util.AppDelegate.Window.Bounds;

			// Loading indicator container view

			UIView indicatorContainerView = new UIView();
			indicatorContainerView.Frame = new CGRect(centerPoint.X - width/2, centerPoint.Y - height/2, width, height);
			indicatorContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

			if (darkBackground)
				indicatorContainerView.BackgroundColor = Utils.AppColors.TransparentBlack2;
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
				loadingIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;

			loadingIndicator.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			loadingIndicator.StartAnimating();

			indicatorContainerView.AddSubview(loadingIndicator);
			backgroundView.AddSubview(indicatorContainerView);

			return backgroundView;
		}

		public static void StartNetworkActivityIndicator()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		public static void StopNetworkActivityIndicator()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
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

		public static void ResizeTextViewHeightToFitContent(UITextView tv)
		{
			CGSize newSizeThatFits = tv.SizeThatFits(tv.Frame.Size);
			CGRect tempFrame = tv.Frame;
			tempFrame.Size = new CGSize(tempFrame.Size.Width, newSizeThatFits.Height);   // Only adjust the height
			tv.Frame = tempFrame;
		}

		public static UIColor GetUIColorFromHexString(string hexString)
		{
			if (String.IsNullOrEmpty(hexString))
				return UIColor.White;

			int hexValue;

			if (hexString.StartsWith("#"))
			{
				hexValue = Convert.ToInt32(hexString.Replace("#", string.Empty), 16);
			}
			else
			{
				hexValue = Convert.ToInt32(hexString, 16);
			}

			return UIColor.FromRGB(
				(((float)((hexValue & 0xFF0000) >> 16))/255.0f),
				(((float)((hexValue & 0xFF00) >> 8))/255.0f),
				(((float)(hexValue & 0xFF))/255.0f)
			);
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
				UIImage image = new Models.ImageModel().GetImageByIdFromFile(imageId);
				if (image != null) 
				{
					imageView.Image = image;

					CATransition transition = new CATransition ();
					transition.Duration = 0.5f;
					transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
					transition.Type = CATransition.TransitionFade;
					imageView.Layer.AddAnimation (transition, null);
				}
			}
			else
			{
				new Models.ImageModel ().ImageGetById (imageId, imageSize, (dloadedImageView, dloadedImageViewDestinationId) => {

					if (dloadedImageViewDestinationId == destinationId)
					{
						// This is the correct image for this imageview, let's apply it

						imageView.Image = Utils.Image.FromBase64(dloadedImageView.Image);

						CATransition transition = new CATransition ();
						transition.Duration = 0.5f;
						transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
						transition.Type = CATransition.TransitionFade;
						imageView.Layer.AddAnimation (transition, null);
					}
				}, 
					() => { /* Failure, do nothing (for the moment at least) */ },
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

		/// <summary>
		/// Get a colored version of an image.
		/// </summary>
		/// <returns>A new version of an image, with the specified color.</returns>
		/// <param name="image">Image.</param>
		/// <param name="color">Color.</param>
		public static UIImage GetColoredImage(UIImage image, UIColor color)
		{
			if (image == null)
			{
				image = UIImage.FromBundle("CancelIcon");
			}
			UIImage coloredImage;

			// Start new image context
			UIGraphics.BeginImageContext(image.Size);

			using (CGContext context = UIGraphics.GetCurrentContext())
			{
				// Translate/flip the graphics context (for transforming from CG* coords to UI* coords)
				context.TranslateCTM(0, image.Size.Height);
				context.ScaleCTM(1.0f, -1.0f);

				var rect = new CGRect(0, 0, image.Size.Width, image.Size.Height);

				// Draw image (to get transparency mask)
				context.SetBlendMode(CGBlendMode.Normal);
				context.DrawImage(rect, image.CGImage);

				// Draw the color using the sourcein blend mode so its only drawn on the non-transparent pixels
				context.SetBlendMode(CGBlendMode.SourceIn);
				context.SetFillColor(color.CGColor);
				context.FillRect(rect);

				// Get the new image from the context
				coloredImage = UIGraphics.GetImageFromCurrentImageContext();

				UIGraphics.EndImageContext();
			}

			return coloredImage;
		}
	
		public static void BottomAlignTextViewText(UITextView textView)
		{
			nfloat topOffset = textView.Bounds.Size.Height - textView.ContentSize.Height;
			if (topOffset < 0.0f)
			{
				topOffset = 0.0f;
			}
			textView.ContentOffset = new CGPoint(textView.ContentOffset.X, -topOffset);
		}

		public static void ApplyBlurBackgroundToView(UIView view, UIView backgroundViewToBlur)
		{
			// This makes sure we have the coordinates relative to the backgroundView. Without this, the image drawn
			// for the button would be at the incorrect place of the background. 
			CGRect buttonRectInBGViewCoords = view.ConvertRectToView (view.Bounds, backgroundViewToBlur);
			UIGraphics.BeginImageContextWithOptions (view.Frame.Size, false, Util.AppDelegate.Window.Screen.Scale);

			// Make a new image of the backgroundView (basically a screenshot of the view)
			backgroundViewToBlur.DrawViewHierarchy (new CGRect (-buttonRectInBGViewCoords.X, -buttonRectInBGViewCoords.Y,
				backgroundViewToBlur.Frame.Width, backgroundViewToBlur.Frame.Height), true);
			UIImage newBGImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			// Apply the blur effect
			newBGImage = UIImageEffects.UIImageEffects.ApplyLightEffect(newBGImage);

			view.BackgroundColor = UIColor.FromPatternImage(newBGImage);
		}
			
		public static UIColor GetLighterVersionOfColor(UIColor colorToLighten, double factor = 0.2)
		{
			nfloat r, g, b, a;

			colorToLighten.GetRGBA(out r, out g, out b, out a);

			r = (float)Math.Min(r + factor, 1.0);
			g = (float)Math.Min(g + factor, 1.0);
			b = (float)Math.Min(b + factor, 1.0);

			return UIColor.FromRGBA(r,g,b,a);
		}

		public static void AddDropShadowToView(UIView view, float shadowRadius = 2.0f, float opacity = 0.8f)
		{
			view.Layer.ShadowOffset = new CGSize (0, 0);
			view.Layer.ShadowRadius = shadowRadius;
			view.Layer.ShadowOpacity = opacity;
			view.Layer.MasksToBounds = false;
			CoreGraphics.CGPath shadowPath = new CoreGraphics.CGPath();
			shadowPath.AddRect (view.Layer.Bounds);
			view.Layer.ShadowPath = shadowPath;
		}

		public static UIImage MapCellSizeToIcon(Presentation.Screens.CardCollectionCell.CellSizes cellSize)
		{
			switch (cellSize)
			{
			case Screens.CardCollectionCell.CellSizes.TallWide:
					return UIImage.FromBundle("ViewLargeCellsIcon");
			case Screens.CardCollectionCell.CellSizes.TallNarrow:
					return UIImage.FromBundle("ViewGridIcon");
			case Screens.CardCollectionCell.CellSizes.ShortWide:
					return UIImage.FromBundle("ViewSmallCellsIcon");
			default:
				return UIImage.FromBundle("ViewGridIcon");
			}
		}

		private static void AddedToBasketBannerViewClicked()
		{
			int basketIndex = Utils.Util.AppDelegate.RootTabBarController.controllersToShow.FindIndex(x => x is BasketController);
			Utils.Util.AppDelegate.RootTabBarController.SelectedIndex = basketIndex;
		}

		#region Hide/show status bar with animation - EXPERIMENT, not ready and not in use

		/*
		public static void HideStatusBarWithAnimation()
		{
			nfloat statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;
			UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide);

			CGRect topViewFrame = Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame;
			Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame = new CGRect(topViewFrame.X, topViewFrame.Y, topViewFrame.Width, topViewFrame.Height + statusBarHeight);
			CGRect topViewFrameLonger = Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame;

			UIView.Animate(0.25, () => {
			
				Util.AppDelegate.SlideoutNavCtrl.TopView.NavigationController.NavigationBar.Frame = Util.AppDelegate.SlideoutNavCtrl.TopView.NavigationController.NavigationBar.Bounds; 

				Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame = new CGRect(topViewFrameLonger.X, topViewFrameLonger.Y - statusBarHeight, topViewFrameLonger.Width, topViewFrameLonger.Height);

			});
		}
		*/

		/*
		public static void ShowStatusBarWithAnimation()
		{
			UIApplication.SharedApplication.SetStatusBarHidden(false, UIStatusBarAnimation.Slide);
			nfloat statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;

			CGRect topViewFrame = Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame;
			Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame = new CGRect(topViewFrame.X, topViewFrame.Y, topViewFrame.Width, topViewFrame.Height - statusBarHeight);
			CGRect topViewFrameShorter = Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame;

			UIView.Animate(0.25, () => {

				CGRect navBarFrame = Util.AppDelegate.SlideoutNavCtrl.TopView.NavigationController.NavigationBar.Frame;
				Util.AppDelegate.SlideoutNavCtrl.TopView.NavigationController.NavigationBar.Frame = 
					new CGRect(navBarFrame.X, navBarFrame.Y + statusBarHeight, navBarFrame.Width, navBarFrame.Height);
					
				Util.AppDelegate.SlideoutNavCtrl.TopView.View.Frame = new CGRect(topViewFrameShorter.X, topViewFrameShorter.Y + statusBarHeight, topViewFrameShorter.Width, topViewFrameShorter.Height);

			});
		}
		*/

		#endregion

		private class AddedToBasketBannerView : UIView
		{
			private UITextView textView;
			private UIImageView imageView;
			private UIView clickableView;
			nfloat frameWidth = Utils.Util.AppDelegate.Window.Frame.Width;

			public delegate void BannerViewClickedEventHandler();
			public event BannerViewClickedEventHandler BannerViewClicked;

			public AddedToBasketBannerView()
			{
				this.BackgroundColor = AppColors.TransparentBlack3;

				// Text view
				this.textView = new UITextView();
				this.textView.Editable = false;
				this.textView.BackgroundColor = UIColor.Clear;
				this.textView.TextColor = UIColor.White;
				this.textView.Font = UIFont.SystemFontOfSize(16);
				this.textView.TextAlignment = UITextAlignment.Center;

				// Image view
				this.imageView = new UIImageView();
				this.imageView.ContentMode = UIViewContentMode.ScaleAspectFill;

				//Clickable View
				clickableView  = new UIView();
				clickableView.BackgroundColor = UIColor.Clear;


				this.AddSubview(imageView);
				this.AddSubview(textView);
				this.AddSubview(clickableView);
			}

			private void Main()
			{
				// Intialize the frame for animation
				this.SetFrame ();

				// Set animation
				UIView.BeginAnimations("slideAnimation");
				UIView.SetAnimationDuration (1);

				// Set new frame position
				this.Frame = new CGRect (0, 0, frameWidth, 64f);

				// Commit animation
				UIView.CommitAnimations ();

				// event handler if clicked
				clickableView.AddGestureRecognizer (new UITapGestureRecognizer ( () => 
					{
						//open the basket controller
						if(this.BannerViewClicked != null)
						{
							this.BannerViewClicked();
						}

						this.Hide(true);
					}));
			}

			private void SetFrame()
			{
				// Set new frame position
				this.Frame = new CGRect (0, 0, frameWidth, 64f);
				this.clickableView.Frame = this.Frame;
				this.imageView.Frame = new CGRect (15f, 24f, 34f, 34f);
				this.textView.Frame = new CGRect (0f, 24f, frameWidth, 44f);
				this.Frame = new CGRect (0, -this.Frame.Height, this.Frame.Width, this.Frame.Height);
			}

			private void SetText(String text)
			{
				this.textView.Text = text;
			}

			private void ClearText ()
			{
				this.textView.Text = string.Empty;
			}

			private void SetImage(UIImage image)
			{
				this.imageView.Image = image;
			}

			public void Show (string textToDisplay, UIImage image)
			{
				SetText (textToDisplay);
				SetImage(image);
				Main ();
			}

			public void Hide (bool clicked)
			{
				System.Console.WriteLine("I am inside hide");

				new NSObject ().InvokeOnMainThread (() => {
					bannerViewTimer.Stop();
					if(!clicked)
					{
						// Animation to hide notification banner
						UIView.BeginAnimations("slideAnimation");
						UIView.SetAnimationDuration (2);

						// Intialize the frame for animation
						this.Frame = new CGRect (0, -this.Frame.Height, this.Frame.Width, this.Frame.Height);

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
				});
			}

			[Export("slideAnimationFinished")]
			void SlideStopped ()
			{
				this.RemoveFromSuperview ();
			}

		}
	}

	/*
	//UILoadingView stops them from interacting with the application until all of the data is loaded.
	// UILoadingView uiview = new UILoadingView();
	// uiview.Show();  
	// longrunning...
	// uiview.Hide();
	// JIJ when I used this the keyboard input textbox didn't work!
	public class UILoadingView : UIAlertView
	{
		private UIActivityIndicatorView activityIndicatorView;

		public new bool Visible {
			get;
			set;
		}

		public UILoadingView () : base ("", "", null, null, null)
		{
			base.Title = "Loading data";
			base.Message = "Please wait a moment";
			this.Visible = false;
			activityIndicatorView = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
			AddSubview (activityIndicatorView);
		}

		public UILoadingView (string title, string message) : base (title, message, null, null, null)
		{
			this.Visible = false;
			activityIndicatorView = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
			AddSubview (activityIndicatorView);
		}

		public new void Show ()
		{
			base.Show ();

			activityIndicatorView.Frame = new System.Drawing.RectangleF ((Bounds.Width / 2) - 15, Bounds.Height - 60, 30, 30);
			activityIndicatorView.StartAnimating ();
			this.Visible = true;
		}

		public void Hide ()
		{
			this.Visible = false;
			activityIndicatorView.StopAnimating ();

			BeginInvokeOnMainThread (delegate () {
				DismissWithClickedButtonIndex(0, true);
			});
		}
	}
	*/
}

