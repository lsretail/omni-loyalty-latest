using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using CoreAnimation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace Presentation.Screens
{
	public class HomeView : BaseView
	{
		private UIScrollView containerScrollView;
		private UIScrollView headerPageView;
		private UIPageControl headerPageIndicator;
		private UIView memberInfoView;

		private UIButton btnShortcut1;
		private UIButton btnShortcut2;

		public System.Timers.Timer timer;
		private Action scrollToNextAd;

		private List<Advertisement> advertisements;
		public bool IsDataLoaded { get; set; }

		private readonly IHomeListeners listeners;

		private float headerPageViewHeight = Utils.Util.AppDelegate.DeviceScreenHeight <= 480f ? 220f : 270f;
		private float headerPageIndicatorHeight = 20f;

		public HomeView(IHomeListeners listeners)
		{
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			this.listeners = listeners;

			// Container scroll view
			this.containerScrollView = new UIScrollView();
			this.containerScrollView.BackgroundColor = UIColor.Clear;
			this.AddSubview(this.containerScrollView);

			this.btnShortcut1 = new UIButton();
			btnShortcut1.SetTitle(LocalizationUtilities.LocalizedString("Locations_Locations", "Locations"), UIControlState.Normal);
			btnShortcut1.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			if (Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				btnShortcut1.Font = UIFont.SystemFontOfSize(16f);

			btnShortcut1.BackgroundColor = UIColor.White;
			btnShortcut1.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			btnShortcut1.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			btnShortcut1.TouchUpInside += (sender, e) =>
			{
				if (this.listeners != null)
					this.listeners.ShortcutCellSelected(HomeTableSource.ShortcutIds.Locations);
				//Utils.Util.AppDelegate.SlideoutMenu.MenuItemPressedMenu();
			};

			UIImageView btnShortcut1ImageView = new UIImageView();
			btnShortcut1ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnShortcut1ImageView.Image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile("/Icons/IconLocations.png"), Utils.AppColors.PrimaryColor);
			btnShortcut1ImageView.BackgroundColor = UIColor.Clear;
			this.btnShortcut1.AddSubview(btnShortcut1ImageView);
			this.containerScrollView.AddSubview(this.btnShortcut1);


			this.btnShortcut2 = new UIButton();
			btnShortcut2.SetTitle(LocalizationUtilities.LocalizedString("Favorites_Favorites", "Favorites"), UIControlState.Normal);
			btnShortcut2.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			if (Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				btnShortcut2.Font = UIFont.SystemFontOfSize(16f);

			btnShortcut2.BackgroundColor = UIColor.White;
			btnShortcut2.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			btnShortcut2.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			btnShortcut2.TouchUpInside += (sender, e) =>
			{
				if (this.listeners != null)
					this.listeners.ShortcutCellSelected(HomeTableSource.ShortcutIds.Favorites);
			};

			UIImageView btnShortcut2ImageView = new UIImageView();
			btnShortcut2ImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnShortcut2ImageView.Image = Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), Utils.AppColors.PrimaryColor);
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

			// Member info view
			this.memberInfoView = new UIView();
			this.memberInfoView.BackgroundColor = Utils.AppColors.PrimaryColor;
			this.memberInfoView.UserInteractionEnabled = true;
			this.memberInfoView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				if (this.listeners != null)
				{
					this.listeners.MemberInfoPressed();
				}
			}));
			this.containerScrollView.AddSubview(this.memberInfoView);

			// Logo image view
			UIImageView logoImageView = new UIImageView();
			logoImageView.Tag = 100;
			logoImageView.BackgroundColor = UIColor.Clear;
			logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			logoImageView.Image = Utils.Image.FromFile("/Branding/Standard/homescreen_logo.png");
			logoImageView.ClipsToBounds = true;
			logoImageView.Layer.MasksToBounds = true;
			logoImageView.Layer.BorderWidth = 2f;
			logoImageView.Layer.BorderColor = UIColor.White.CGColor;
			this.memberInfoView.AddSubview(logoImageView);

			//Welcome label
			UILabel lblWelcome = new UILabel();
			lblWelcome.Tag = 150;
			lblWelcome.Text = LocalizationUtilities.LocalizedString("Home_Welcome", "Welcome");
			lblWelcome.BackgroundColor = UIColor.Clear;
			lblWelcome.TextColor = UIColor.White;
			lblWelcome.Font = UIFont.SystemFontOfSize(22f);
			this.memberInfoView.AddSubview(lblWelcome);

			//Sign in text view
			UITextView txtSignIn = new UITextView();
			txtSignIn.Tag = 160;
			txtSignIn.Text = LocalizationUtilities.LocalizedString("Home_SignIn", "");
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
		}


		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.containerScrollView.Frame = new CGRect(0, this.TopLayoutGuideLength, this.Bounds.Width, this.Bounds.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength);

			float margin = 10f;

			this.btnShortcut1.Frame = new CGRect(margin, margin, this.containerScrollView.Bounds.Width / 2 - margin - margin / 2, 40f);
			this.btnShortcut2.Frame = new CGRect(this.btnShortcut1.Frame.Right + margin, margin, this.btnShortcut1.Frame.Width, 40f);
			this.headerPageView.Frame = new CGRect(margin, this.btnShortcut1.Frame.Bottom + margin, this.containerScrollView.Bounds.Width - 2 * margin, this.headerPageViewHeight - margin);
			this.headerPageIndicator.Frame = new CGRect(0, this.headerPageView.Frame.Bottom - this.headerPageIndicatorHeight, this.headerPageView.Bounds.Width, this.headerPageIndicatorHeight);
			nfloat memberInfoViewHeight = this.containerScrollView.Frame.Height - margin - this.btnShortcut1.Frame.Height - margin - this.headerPageView.Frame.Height - 2 * margin;
			this.memberInfoView.Frame = new CGRect(margin, this.headerPageView.Frame.Bottom + margin, this.containerScrollView.Bounds.Width - 2 * margin, memberInfoViewHeight);

			// MemberInfoView layout

			UIView logoImageView = this.memberInfoView.ViewWithTag(100);
			float logoImageViewDimensions = 110f;
			if (Utils.Util.AppDelegate.DeviceScreenWidth > 320f)
				logoImageViewDimensions = 160f;

			logoImageView.Frame = new CGRect(20f, (this.memberInfoView.Bounds.Height - logoImageViewDimensions) / 2, logoImageViewDimensions, logoImageViewDimensions);
			logoImageView.Layer.CornerRadius = logoImageView.Frame.Size.Height / 2;

			UIView lblWelcome = this.memberInfoView.ViewWithTag(150);
			UIView txtSignIn = this.memberInfoView.ViewWithTag(160);
			UIView lblContactName = this.memberInfoView.ViewWithTag(200);
			UIView lblContactPoints = this.memberInfoView.ViewWithTag(300);


			lblContactName.Frame = new CGRect(logoImageView.Frame.Right + 20f, this.memberInfoView.Bounds.Height / 2 - 10f, this.memberInfoView.Bounds.Right - logoImageView.Frame.Right - 2 * 20f, 20f);
			txtSignIn.Frame = new CGRect(logoImageView.Frame.Right + 20f, this.memberInfoView.Bounds.Height / 2 - 10f, this.memberInfoView.Bounds.Right - logoImageView.Frame.Right - 2 * 20f, 80f);
			lblWelcome.Frame = new CGRect(logoImageView.Frame.Right + 20f, this.memberInfoView.Bounds.Height / 2 - 37f, this.memberInfoView.Bounds.Right - logoImageView.Frame.Right - 2 * 20f, 22f);
			lblContactPoints.Frame = new CGRect(lblContactName.Frame.Left, lblContactName.Frame.Bottom, lblContactName.Frame.Width, 20f);

			// Set containerscrollview content height
			nfloat containerScrollViewContentHeight = margin + this.btnShortcut1.Frame.Height + margin + this.headerPageView.Frame.Height + margin + this.memberInfoView.Frame.Height;
			this.containerScrollView.ContentSize = new CGSize(this.containerScrollView.Bounds.Width, containerScrollViewContentHeight);

			//Utils.UI.AddDropShadowToView(this.headerPageView);
		}

		public void LoadAdvertisements(List<Advertisement> advertisements)
		{
			this.advertisements = advertisements;
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

				adView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
				{
					if (this.listeners != null)
					{
						this.listeners.AdvertisementPressed(ad);
					}
				}));

				this.headerPageView.AddSubview(adView);
				pageOffset += pageWidth;
				pageCount++;
			}

			this.headerPageView.ContentSize = new CGSize(pageOffset, this.headerPageView.Bounds.Height);

			// Set up page indicator
			this.headerPageIndicator.Pages = pageCount;
			if (this.headerPageIndicator.Pages == 0)
				this.headerPageIndicator.Hidden = true;

			this.headerPageView.Scrolled += (object sender, EventArgs e) =>
			{

				int page = (int)Math.Floor((this.headerPageView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
				this.headerPageIndicator.CurrentPage = page;

				// Restart timer
				if (this.timer != null)
				{
					this.timer.Stop();
					this.timer.Start();
				}
			};

			this.scrollToNextAd = new Action(() =>
			{

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
			new Models.ImageModel().ImageGetById(imageId, imageSize, (dloadedImageView, destinationId) =>
			{

				imageView.Image = Utils.Image.FromBase64(dloadedImageView.Image);

				CATransition transition = new CATransition();
				transition.Duration = 0.5f;
				transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
				transition.Type = CATransition.TransitionFade;
				imageView.Layer.AddAnimation(transition, null);

			},
				() => { /* Failure, do nothing (for the moment at least) */ }
			);
		}

		public void RefreshLayout(bool LoggedIn, MemberContact Con)
		{
			this.SetNeedsLayout();
			this.LayoutIfNeeded();
			RefreshContactInfo(LoggedIn, Con);
		}

		private void RefreshContactInfo(bool LoggedIn, MemberContact Con)
		{
			if (LoggedIn)
				ShowContactInfo((!String.IsNullOrEmpty(Con.FirstName) ? Con.FirstName : Con.UserName), Con.Account.PointBalance);
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
			lblContactPoints.Text = points.ToString("N0") + " " + LocalizationUtilities.LocalizedString("Home_Points_Lowercase", "points");
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

		public interface IHomeListeners
		{
			void MemberInfoPressed();
			void AdvertisementPressed(Advertisement ad);
			void ShortcutCellSelected(HomeTableSource.ShortcutIds shortcutId);
		}
	}
}

