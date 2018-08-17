using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class HomeView : BaseView
	{
		private const float HEADER_HEIGHT = 280f;

		/*
		private ImageCarouselView imageCarousel;
		private UIPageControl imageCarouselPageControl;
		private UIScrollView scrollView;
		private UIView imageWindowView;
		*/
		private UIImageView bannerImageView;
		private UIView containerView;
		private UIImageView imageView;

		private UIView signInContainerView;
		private UIView infoContainerView;
		private UILabel lblInfoTitle;
		private UITextView tvInfoContent;
		private UILabel lblName;
		private UILabel lblUserName;
		private UILabel lblMemberScheme;
		private UILabel lblPointStatus;
		private UIButton btnShortcut1;
		private UIButton btnShortcut2;
		private UIImageView btnShortcut1ImageView;
		private UIImageView btnShortcut2ImageView;


		public delegate void ImageSelectedEventHandler(List<ImageView> imageViews, nint selectedImageViewIndex);
		public delegate void ContainerViewClickedEventHandler ();
		public delegate void btnShortcutPressedEventHandler ();

		//public event ImageSelectedEventHandler ImageSelected;
		public event ContainerViewClickedEventHandler ContainerViewClicked;
		public event btnShortcutPressedEventHandler btnShortcut1Pressed;
		public event btnShortcutPressedEventHandler btnShortcut2Pressed;

		public HomeView ()
		{
			this.BackgroundColor = Utils.AppColors.BackgroundGray;;

			/*
			this.imageCarousel = new ImageCarouselView();
			//this.imageCarousel.ContentMode = UIViewContentMode.ScaleToFill;

			this.imageCarouselPageControl = new UIPageControl();
			this.imageCarouselPageControl.HidesForSinglePage = true;
			this.imageCarouselPageControl.CurrentPageIndicatorTintColor = UIColor.DarkGray;
			this.imageCarouselPageControl.PageIndicatorTintColor = UIColor.LightGray;

			this.imageWindowView = new UIView();
			this.imageWindowView.BackgroundColor = UIColor.Clear;
			this.imageWindowView.AddGestureRecognizer(
				new UIPanGestureRecognizer(
					(panRecognizer) => HandleImageWindowDrag(panRecognizer)
				)
			);
			*/
			this.bannerImageView = new UIImageView ();
			this.bannerImageView.BackgroundColor = UIColor.Clear;
			this.bannerImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.bannerImageView.ClipsToBounds = true;
			this.bannerImageView.Layer.MasksToBounds = true;

			this.containerView = new UIView ();
			this.containerView.BackgroundColor = Utils.AppColors.PrimaryColor;
			this.containerView.AddGestureRecognizer(
				new UITapGestureRecognizer(
					(tapRecognizer) => HandleContainerViewTap (tapRecognizer)										
				)
			);

			this.infoContainerView = new UIView ();
			this.infoContainerView.BackgroundColor = UIColor.Clear;

			this.imageView = new UIImageView ();
		    this.imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.imageView.ClipsToBounds = true;
			this.imageView.BackgroundColor = UIColor.Clear;

			this.btnShortcut1ImageView = new UIImageView();
			this.btnShortcut1ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			this.btnShortcut1ImageView.BackgroundColor = UIColor.Clear;

			this.btnShortcut2ImageView = new UIImageView();
			this.btnShortcut2ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			this.btnShortcut2ImageView.BackgroundColor = UIColor.Clear;

			this.btnShortcut1 = new UIButton();
			this.btnShortcut1.SetTitleColor(UIColor.Black, UIControlState.Normal);
			if(Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				this.btnShortcut1.Font = UIFont.SystemFontOfSize(16f);
			this.btnShortcut1.BackgroundColor = UIColor.White;
			this.btnShortcut1.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			this.btnShortcut1.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			this.btnShortcut1.TouchUpInside += (sender, e) => 
			{
				if(btnShortcut1Pressed != null)
				{
					btnShortcut1Pressed ();
				}
			};

			this.btnShortcut2 = new UIButton();
			this.btnShortcut2.SetTitleColor(UIColor.Black, UIControlState.Normal);
			if(Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				this.btnShortcut2.Font = UIFont.SystemFontOfSize(16f);
			this.btnShortcut2.BackgroundColor = UIColor.White;
			this.btnShortcut2.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			this.btnShortcut2.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			this.btnShortcut2.TouchUpInside += (sender, e) => 
			{
				if(btnShortcut2Pressed != null)
				{
					btnShortcut2Pressed ();
				}
			};

			this.btnShortcut1ImageView = new UIImageView();
			this.btnShortcut1ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			this.btnShortcut1ImageView.BackgroundColor = UIColor.Clear;

			this.btnShortcut2ImageView = new UIImageView();
			this.btnShortcut2ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			this.btnShortcut2ImageView.BackgroundColor = UIColor.Clear;

			this.lblInfoTitle = new UILabel ();
			this.lblInfoTitle.BackgroundColor = UIColor.Clear;
			this.lblInfoTitle.UserInteractionEnabled = false;		
			this.lblInfoTitle.Font = UIFont.BoldSystemFontOfSize (18);
			this.lblInfoTitle.TextAlignment = UITextAlignment.Left;
			this.lblInfoTitle.TextColor = Utils.AppColors.SoftWhite;

			this.tvInfoContent = new UITextView ();
			this.tvInfoContent.BackgroundColor = UIColor.Clear;
			this.tvInfoContent.UserInteractionEnabled = false;
			this.tvInfoContent.AllowsEditingTextAttributes = false;
			this.tvInfoContent.TextAlignment = UITextAlignment.Left;
			this.tvInfoContent.TextColor = Utils.AppColors.SoftWhite;

			this.signInContainerView = new UIView ();
			this.signInContainerView.BackgroundColor = UIColor.Clear;

			this.lblName = new UILabel ();
			this.lblName.TextColor = Utils.AppColors.SoftWhite;
			this.lblName.Font = UIFont.SystemFontOfSize (18);
			this.lblName.TextAlignment = UITextAlignment.Left;

			this.lblUserName = new UILabel ();
			this.lblUserName.TextColor = Utils.AppColors.SoftWhite;
			this.lblUserName.Font = UIFont.SystemFontOfSize (12);
			this.lblUserName.TextAlignment = UITextAlignment.Left;
			this.lblUserName.BackgroundColor = UIColor.Clear;

			this.lblMemberScheme = new UILabel ();
			this.lblMemberScheme.TextColor = Utils.AppColors.SoftWhite;
			this.lblMemberScheme.Font = UIFont.SystemFontOfSize (12);
			this.lblMemberScheme.TextAlignment = UITextAlignment.Left;
			this.lblMemberScheme.UserInteractionEnabled = false;
			this.lblMemberScheme.BackgroundColor = UIColor.Clear;
			this.lblMemberScheme.Lines = 1;

			this.lblPointStatus = new UILabel ();
			this.lblPointStatus.TextColor = Utils.AppColors.SoftWhite;
			this.lblPointStatus.Font = UIFont.SystemFontOfSize (12);
			this.lblPointStatus.TextAlignment = UITextAlignment.Left;
		
			//this.AddSubview(this.imageCarousel);
			//this.AddSubview(this.imageCarouselPageControl);
			//this.AddSubview (this.imageWindowView);
			this.AddSubview (this.bannerImageView);
			this.AddSubview (this.containerView);
			this.AddSubview (this.btnShortcut1);
			this.AddSubview (this.btnShortcut2);
			this.btnShortcut1.AddSubview(btnShortcut1ImageView);	
			this.btnShortcut2.AddSubview(btnShortcut2ImageView);
			this.containerView.AddSubview (this.imageView);
			this.containerView.AddSubview (this.infoContainerView);
			this.containerView.AddSubview (this.signInContainerView);
			this.signInContainerView.AddSubview (this.lblInfoTitle);
			this.signInContainerView.AddSubview (this.tvInfoContent);
			this.infoContainerView.AddSubview (this.lblName);
			this.infoContainerView.AddSubview (this.lblUserName);
			this.infoContainerView.AddSubview (this.lblMemberScheme);
			this.infoContainerView.AddSubview (this.lblPointStatus);

		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			nfloat xMargin = 10f;
			nfloat yMargin = 10f;
			nfloat pageContentHeight = this.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength;

			#region image
			/*
			this.imageCarousel.Frame = new CGRect(
				0, 
				this.TopLayoutGuideLength, 
				this.Frame.Width, 
				HEADER_HEIGHT
			);

			this.imageCarouselPageControl.Frame = new CGRect(
				0, 
				this.imageCarousel.Frame.Bottom, 
				this.imageCarousel.Frame.Width, 
				pageControlHeight
			);

			if (this.imageCarouselPageControl.Pages <= 1)
			{
				this.imageCarouselPageControl.Frame = new CGRect(
					0, 
					this.imageCarousel.Frame.Bottom, 
					this.imageCarousel.Frame.Width, 
					0f
				);	// Hide the page control
			}

			this.imageWindowView.Frame = new CGRect(
				0, 
				this.TopLayoutGuideLength, 
				this.Frame.Width,
				HEADER_HEIGHT + this.imageCarouselPageControl.Frame.Height
			);

			*/
			#endregion


			this.btnShortcut1.Frame = new CGRect(
				xMargin, 
				this.TopLayoutGuideLength + yMargin, 
				this.Frame.Width/2 - xMargin - xMargin/2, 
				40f
			);

			this.btnShortcut2.Frame = new CGRect(
				this.btnShortcut1.Frame.Right + xMargin, 
				this.TopLayoutGuideLength + yMargin, 
				this.btnShortcut1.Frame.Width,
				40f
			);

			this.containerView.Frame = new CGRect (
				xMargin,
				this.Frame.Bottom - this.BottomLayoutGuideLength - pageContentHeight / 3 - 30f,
				this.Frame.Width - 2 * xMargin,
				pageContentHeight / 3 + 20f
			);

			this.bannerImageView.Frame = new CGRect (
				0,
				this.btnShortcut1.Frame.Bottom + yMargin,
				this.Frame.Width,
				pageContentHeight - containerView.Frame.Height - btnShortcut1.Frame.Height - 4 * yMargin
			);
				
			this.imageView.Frame = new CGRect (
				2 * xMargin,
				(this.containerView.Frame.Height - (this.containerView.Frame.Width/2 - 4 * xMargin))/2,
				this.containerView.Frame.Width/2 - 4 * xMargin,
				this.containerView.Frame.Width/2 - 4 * yMargin
			);

			this.imageView.Layer.CornerRadius = this.imageView.Frame.Size.Height / 2;

			if( AppData.UserLoggedIn)
			{
				this.signInContainerView.Hidden = true;
				this.infoContainerView.Hidden = false;

				this.infoContainerView.Frame = new CGRect (
					this.imageView.Frame.Width + 4 * xMargin,
					this.imageView.Frame.Y,
					this.containerView.Frame.Width / 2,
					this.containerView.Frame.Height - 2* yMargin
				);

				this.lblName.Frame = new CGRect (
					0,
					0,
					this.containerView.Frame.Width,
					30f
				);

				this.lblUserName.Frame = new CGRect (
					0,
					lblName.Frame.Bottom,
					this.containerView.Frame.Width,
					20f
				);

				this.lblMemberScheme.Frame = new CGRect (
					0,
					lblUserName.Frame.Bottom,
					this.containerView.Frame.Width,
					20f
				);

				this.lblPointStatus.Frame = new CGRect (
					0,
					lblMemberScheme.Frame.Bottom,
					this.containerView.Frame.Bottom,
					20f
				);

			}
			else
			{
				this.signInContainerView.Hidden = false;
				this.infoContainerView.Hidden = true;

				this.signInContainerView.Frame = new CGRect (
					this.imageView.Frame.Width + 4 * xMargin,
					this.imageView.Frame.Y,
					this.containerView.Frame.Width/2,
					this.containerView.Frame.Height - 2* yMargin
				);

				this.lblInfoTitle.Frame = new CGRect (
					0,
					2 * yMargin,
					this.signInContainerView.Frame.Width,
					20f
				);

				this.tvInfoContent.Frame = new CGRect (
					0,
					this.lblInfoTitle.Frame.Bottom,
					this.signInContainerView.Frame.Width,
					this.signInContainerView.Frame.Height - this.lblInfoTitle.Frame.Height - 2 * yMargin
				);
			}
		}

		private void HandleContainerViewTap (UITapGestureRecognizer tap)
		{
			if(ContainerViewClicked != null)
			{
				ContainerViewClicked ();
			}	
		}

		#region Image window gesture stuff
		/*
		// We must forward gestures made on the image window to the image carousel below it.

		private CGPoint currentImgPoint;
		private CGPoint currentImgOffset;
		private CGPoint beginningPoint;
		private CGPoint newPoint;

		private void HandleImageWindowTap(UITapGestureRecognizer tap)
		{						
			if (this.ImageSelected != null)
				this.ImageSelected(this.imageCarousel.ImageViews, this.imageCarouselPageControl.CurrentPage);
		}

		private void HandleImageWindowDrag(UIPanGestureRecognizer recognizer)
		{
			if (recognizer.State == UIGestureRecognizerState.Began)
			{
				this.beginningPoint = recognizer.TranslationInView (this.imageWindowView);
				this.currentImgOffset.X = this.imageCarousel.ContentOffset.X;
			}

			if (recognizer.State != (UIGestureRecognizerState.Ended | UIGestureRecognizerState.Cancelled | UIGestureRecognizerState.Failed))
			{
				this.newPoint = recognizer.TranslationInView (this.imageWindowView);
				this.currentImgPoint.X = this.beginningPoint.X - this.newPoint.X + this.currentImgOffset.X;
				this.imageCarousel.SetContentOffset (this.currentImgPoint, false);
			}

			if (recognizer.State == UIGestureRecognizerState.Ended)
			{
				nfloat length = this.beginningPoint.X - this.newPoint.X;

				if (length >= 60f)
				{
					if (this.imageCarouselPageControl.Pages != (this.imageCarouselPageControl.CurrentPage + 1)) 
					{
						this.currentImgPoint.X = (this.imageCarouselPageControl.CurrentPage + 1) * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
						this.imageCarouselPageControl.CurrentPage = this.imageCarouselPageControl.CurrentPage + 1;
					}
					else
					{
						this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else if (length <= -60f)
				{
					if (this.imageCarouselPageControl.CurrentPage != 0) 
					{
						this.currentImgPoint.X = (this.imageCarouselPageControl.CurrentPage - 1) * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
						this.imageCarouselPageControl.CurrentPage = this.imageCarouselPageControl.CurrentPage - 1;
					}
					else
					{
						this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
						this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else
				{
					this.currentImgPoint.X = this.imageCarouselPageControl.CurrentPage * imageCarousel.Frame.Width;
					this.imageCarousel.SetContentOffset (this.currentImgPoint, true);
				}
			}
		}
		*/
		#endregion

		public void UpdateData (string btnShortcut1Title, UIImage btnShortcut1Image, string btnShortcut2Title, UIImage btnShortcut2Image)
		{
			this.btnShortcut1.SetTitle(btnShortcut1Title, UIControlState.Normal);
			this.btnShortcut1ImageView.Image = ImageUtilities.GetColoredImage ( btnShortcut1Image, Utils.AppColors.PrimaryColor);

			this.btnShortcut2.SetTitle(btnShortcut2Title, UIControlState.Normal);
			this.btnShortcut2ImageView.Image = ImageUtilities.GetColoredImage ( btnShortcut2Image, Utils.AppColors.PrimaryColor);

			this.imageView.Image = ImageUtilities.FromFile ("/Branding/Standard/lsretailHomeLogo.png");
			this.bannerImageView.Image = ImageUtilities.FromFile ("/Branding/Standard/StoreBanner.png");
			//this.imageCarousel.ImageViews = GetMockImages ();

			if(AppData.UserLoggedIn)
			{
				this.lblName.Text = AppData.Device.UserLoggedOnToDevice.Name;
				this.lblUserName.Text = AppData.Device.UserLoggedOnToDevice.UserName;
				this.lblMemberScheme.Text = GetMemberSchemeString ();
				this.lblPointStatus.Text = GetPointBalanceString ();
			}
			else
			{
				this.lblInfoTitle.Text = LocalizationUtilities.LocalizedString("Home_Welcome", "Welcome");
				this.tvInfoContent.Text = LocalizationUtilities.LocalizedString ("Home_SignIn", "Sign in to get points for each purchase");
			}

			LayoutSubviews ();
		}

		private List<ImageView> GetMockImages()
		{
			List<ImageView> imageViews = new List<ImageView> ();
			imageViews.Add (new ImageView("/Branding/Standard/StoreBannerTransparent.png"));
			imageViews.Add (new ImageView("/Branding/Standard/StoreBanner.png"));

			foreach( var view in imageViews)
			{
				view.LoadFromFile = true;
			}

			return imageViews;
		}

		private string GetMemberSchemeString()
		{
			return AppData.Device.UserLoggedOnToDevice.Account.Scheme.Description + " " + LocalizationUtilities.LocalizedString("Account_Member", "member");
		}

		private string GetPointBalanceString()
		{
			return AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0") + " " + LocalizationUtilities.LocalizedString("Account_Points_Lowercase", "points");
		}
	}
}
	
