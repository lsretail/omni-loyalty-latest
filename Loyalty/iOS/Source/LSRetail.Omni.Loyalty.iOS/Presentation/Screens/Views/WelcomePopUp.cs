using System;
using UIKit;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class WelcomePopUp : PopUpView
    {
        private UITextView applicationTitle;
        private UITextView welcomeTitle;
        private UITextView welcomeText;
        private UIButton btnNext;

        public delegate void WelcomeMsgDismissedEventHandler();
        public WelcomeMsgDismissedEventHandler WelcomeMsgDismissed;

        public WelcomePopUp() : base(true)
        {
            this.BackgroundColor = Utils.AppColors.BackgroundGray;

            this.applicationTitle = new UITextView();
            applicationTitle.UserInteractionEnabled = true;
            applicationTitle.Editable = false;
            applicationTitle.Text = LocalizationUtilities.LocalizedString("Welcome_ApplicationTitle", "Quality Store");
            applicationTitle.TextColor = UIColor.Black;
            applicationTitle.Font = UIFont.SystemFontOfSize(14);
            applicationTitle.TextAlignment = UITextAlignment.Left;
            applicationTitle.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.AddSubview(applicationTitle);

            this.welcomeTitle = new UITextView();
            welcomeTitle.UserInteractionEnabled = true;
            welcomeTitle.Editable = false;
            welcomeTitle.Text = LocalizationUtilities.LocalizedString("Welcome_Title", "Welcome");
            welcomeTitle.TextColor = Utils.AppColors.PrimaryColor;
            welcomeTitle.Font = UIFont.SystemFontOfSize(40);
            welcomeTitle.TextAlignment = UITextAlignment.Left;
            welcomeTitle.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.AddSubview(welcomeTitle);

            this.welcomeText = new UITextView();
            welcomeText.UserInteractionEnabled = true;
            welcomeText.Editable = false;
            welcomeText.Text = LocalizationUtilities.LocalizedString("Welcome_WelcomeText", "");
            welcomeText.TextColor = UIColor.Black;
            welcomeText.Font = UIFont.SystemFontOfSize(18);
            welcomeText.TextAlignment = UITextAlignment.Left;
            welcomeText.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.AddSubview(welcomeText);

            this.btnNext = new UIButton();
            btnNext.SetTitle(LocalizationUtilities.LocalizedString("General_OK", "OK"), UIControlState.Normal);
            btnNext.BackgroundColor = Utils.AppColors.PrimaryColor;
            btnNext.Layer.CornerRadius = 2;
            btnNext.TouchUpInside += (object sender, EventArgs e) =>
            {

                if (WelcomeMsgDismissed != null)
                {
                    WelcomeMsgDismissed();
                }
            };
            this.AddSubview(btnNext);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            const float margin = 10f;
            const float applicationTitleHeight = 50f;
            const float welcomeTitleHeight = 60f;
            const float btnHeight = 50f;

            this.applicationTitle.Frame = new CGRect(
                margin,
                margin,
                this.Frame.Width - 2 * margin,
                applicationTitleHeight
            );

            this.welcomeTitle.Frame = new CGRect(
                margin,
                this.applicationTitle.Frame.Bottom + margin,
                this.Frame.Width - 2 * margin,
                welcomeTitleHeight
            );

            this.btnNext.Frame = new CGRect(
                margin,
                this.Frame.Bottom - 3 * margin - btnHeight,
                this.Frame.Width - 2 * margin,
                btnHeight
            );

            this.welcomeText.Frame = new CGRect(
                margin,
                this.welcomeTitle.Frame.Bottom,
                this.Frame.Width - 2 * margin,
                this.btnNext.Frame.Top - margin - this.welcomeTitle.Frame.Bottom
            );
        }
    }
}

