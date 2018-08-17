using System;
using UIKit;
using Presentation.Utils;
using Foundation;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public class WelcomeView : UIView
	{
		private UIImageView welcomeImage;
		private UITextView welcomeTitle;
		private UITextView welcomeText;
		private UIButton btnNext;

		const float margin = 20f;

		private readonly IWelcomeListeners listeners;

		public WelcomeView(WelcomeController ctrl)
		{
			this.BackgroundColor = Utils.AppColors.PrimaryColor;

			this.listeners = ctrl;
			this.welcomeImage = new UIImageView();
			welcomeImage.BackgroundColor = UIColor.Clear;
			welcomeImage.ContentMode = UIViewContentMode.ScaleAspectFit;
			welcomeImage.Image = Utils.Image.FromFile("/Branding/Standard/homescreen_logo.png");
			welcomeImage.ClipsToBounds = true;
			welcomeImage.Layer.MasksToBounds = true;
			welcomeImage.Layer.BorderWidth = 2f;
			welcomeImage.Layer.BorderColor = UIColor.White.CGColor;
			welcomeImage.Layer.CornerRadius = 40f;
			this.AddSubview(welcomeImage);

			this.welcomeTitle = new UITextView();
			welcomeTitle.UserInteractionEnabled = true;
			welcomeTitle.Editable = false;
			welcomeTitle.Text = LocalizationUtilities.LocalizedString("Welcome_Title", "Welcome");
			welcomeTitle.TextColor = UIColor.White;
			welcomeTitle.Font = UIFont.SystemFontOfSize(40);
			welcomeTitle.TextAlignment = UITextAlignment.Left;
			welcomeTitle.BackgroundColor = UIColor.Clear;
			this.AddSubview(welcomeTitle);

			this.welcomeText = new UITextView();
			welcomeText.UserInteractionEnabled = true;
			welcomeText.Editable = false;
			welcomeText.Text = LocalizationUtilities.LocalizedString("Welcome_WelcomeText", "");
			welcomeText.TextColor = UIColor.White;
			welcomeText.Font = UIFont.SystemFontOfSize(18);
			welcomeText.TextAlignment = UITextAlignment.Left;
			welcomeText.BackgroundColor = UIColor.Clear;
			this.AddSubview(welcomeText);


			this.btnNext = new UIButton();
			btnNext.SetTitle(LocalizationUtilities.LocalizedString("Welcome_NextPage", "Next"), UIControlState.Normal);
			btnNext.BackgroundColor = UIColor.White;
			btnNext.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			btnNext.Layer.CornerRadius = 2;
			btnNext.TouchUpInside += (object sender, EventArgs e) =>
			{
				if (this.listeners != null)
				{
					this.listeners.Next();
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

		public interface IWelcomeListeners
		{
			void Next();
		}
	}

}

