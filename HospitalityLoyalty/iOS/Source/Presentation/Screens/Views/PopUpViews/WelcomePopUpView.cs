using System;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class WelcomePopUpView : PopUpView
	{
		private UIImageView welcomeImage;
		private UITextView welcomeTitle;
		private UITextView welcomeText;
		private UIButton btnNext;

		public const float margin = 20f;

		public delegate void WelcomeMsgDismissedEventHandler();
		public WelcomeMsgDismissedEventHandler WelcomeMsgDismissed;

		public WelcomePopUpView() : base(true)
		{
			this.BackgroundColor = AppColors.PrimaryColor;

			this.welcomeImage = new UIImageView();
			this.welcomeImage.BackgroundColor = UIColor.Clear;
			this.welcomeImage.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.welcomeImage.Image = Image.FromFile("/Branding/Standard/homescreen_logo.png");
			this.welcomeImage.ClipsToBounds = true;
			this.welcomeImage.Layer.MasksToBounds = true;
			this.welcomeImage.Layer.BorderWidth = 2f;
			this.welcomeImage.Layer.BorderColor = UIColor.White.CGColor;
			this.welcomeImage.Layer.CornerRadius = 40f;
			this.AddSubview(welcomeImage);

			this.welcomeTitle = new UITextView();
			this.welcomeTitle.UserInteractionEnabled = true;
			this.welcomeTitle.Editable = false;
			this.welcomeTitle.Text = LocalizationUtilities.LocalizedString("Welcome_Title", "Welcome");
			this.welcomeTitle.TextColor = UIColor.White;
			this.welcomeTitle.Font = UIFont.SystemFontOfSize(40);
			this.welcomeTitle.TextAlignment = UITextAlignment.Left;
			this.welcomeTitle.BackgroundColor = UIColor.Clear;
			this.AddSubview(welcomeTitle);

			this.welcomeText = new UITextView();
			this.welcomeText.UserInteractionEnabled = true;
			this.welcomeText.Editable = false;
			this.welcomeText.Text = LocalizationUtilities.LocalizedString("Welcome_WelcomeText", "");
			this.welcomeText.TextColor = UIColor.White;
			this.welcomeText.Font = UIFont.SystemFontOfSize(18);
			this.welcomeText.TextAlignment = UITextAlignment.Left;
			this.welcomeText.BackgroundColor = UIColor.Clear;
			this.AddSubview(welcomeText);


			this.btnNext = new UIButton();
			this.btnNext.SetTitle(LocalizationUtilities.LocalizedString("Welcome_NextPage", "Next"), UIControlState.Normal);
			this.btnNext.BackgroundColor = UIColor.White;
			this.btnNext.SetTitleColor(AppColors.PrimaryColor, UIControlState.Normal);
			this.btnNext.Layer.CornerRadius = 2;
			this.btnNext.TouchUpInside += (object sender, EventArgs e) =>
			{
				if (this.WelcomeMsgDismissed != null)
				{
					this.WelcomeMsgDismissed();
				}
			};
			this.AddSubview(btnNext);
		}


		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.ConstrainLayout(() =>
				this.welcomeImage.Frame.Top == this.Bounds.Top + 2 * margin &&
				this.welcomeImage.Frame.Left == this.Bounds.Left + margin &&
				this.welcomeImage.Frame.Width == 80f &&
				this.welcomeImage.Frame.Height == 80f &&

				this.welcomeTitle.Frame.Top == welcomeImage.Frame.Bottom &&
				this.welcomeTitle.Frame.Left == this.Bounds.Left + margin &&
				this.welcomeTitle.Frame.Right == this.Bounds.Right - margin &&
				this.welcomeTitle.Frame.Height == 60f &&

				this.welcomeText.Frame.Top == welcomeTitle.Frame.Bottom &&
				this.welcomeText.Frame.Left == this.Bounds.Left + margin &&
				this.welcomeText.Frame.Right == this.Bounds.Right - margin &&
				this.welcomeText.Frame.Bottom == btnNext.Frame.Top - margin &&

				this.btnNext.Frame.Bottom == this.Bounds.Bottom - 20f &&
				this.btnNext.Frame.Left == this.Bounds.Left + 20f &&
				this.btnNext.Frame.Right == this.Bounds.Right - 20f &&
				this.btnNext.Frame.Height == 50f
			);
		}
	}
}

