using System;
using UIKit;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using Foundation;

namespace Presentation
{
	public class HiddenSettingsView : BaseView
	{
		private UIButton btnOK;
		private UILabel urlLabel;
		private UITextField urlTextField;
		private UILabel lblVersion;

		public event EventHandler PingButtonClicked;

		public HiddenSettingsView ()
		{
			// UI elements
			this.urlLabel = new UILabel();
			this.urlLabel.Text = LocalizationUtilities.LocalizedString("Hidden_Settings_URL", "URL");
			this.urlLabel.Font = UIFont.SystemFontOfSize(14);
			this.urlLabel.TextAlignment = UITextAlignment.Left;
			this.urlLabel.TextColor = Utils.AppColors.PrimaryColor;

			this.urlTextField = new UITextField();
			this.urlTextField.Text = LocalizationUtilities.LocalizedString("Hidden_Settings_URL", "URL");
			this.urlTextField.Delegate = new CustomTextFieldDelegate();

			this.btnOK = new UIButton();
			this.btnOK.SetTitle(LocalizationUtilities.LocalizedString("HiddenSettings_Ping", "Ping"), UIControlState.Normal);
			this.btnOK.BackgroundColor = Utils.AppColors.PrimaryColor;
			this.btnOK.Layer.CornerRadius = 2;
			this.btnOK.TouchUpInside += (object sender, EventArgs e) => { PingButtonClicked?.Invoke(this, EventArgs.Empty);};
			

			this.lblVersion = new UILabel();
			this.lblVersion.TextAlignment = UITextAlignment.Center;

			this.AddSubview (this.urlLabel);
			this.AddSubview (this.urlTextField);
			this.AddSubview (this.btnOK);
			this.AddSubview (this.lblVersion);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			float margin = 20f;
			float buttonHeight = 50f;
			float buttonMargin = 20f;

			this.urlLabel.Frame = new CGRect(margin, this.TopLayoutGuideLength + 30f, this.Frame.Width - 2 * margin, 20f);
			this.urlTextField.Frame = new CGRect(margin, this.urlLabel.Frame.Bottom, this.Frame.Width - 2 * margin, 20f);
			this.btnOK.Frame = new CGRect(buttonMargin, this.urlTextField.Frame.Bottom + buttonHeight, this.Frame.Width - 2 * buttonMargin, buttonHeight);

			this.lblVersion.Frame = new CGRect(
				0,
				this.btnOK.Frame.Bottom + 30f,
				this.Frame.Width,
				20f
			);
		}

		public void UpdateData (string url)
		{
			urlTextField.Text = url;

            lblVersion.Text = string.Format(LocalizationUtilities.LocalizedString("HiddenSettings_Version", "Version {0}"), NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")]);
		}

		public string GetUrlTextField () 
		{
			return urlTextField.Text;
		}
	}

	class CustomTextFieldDelegate : UITextFieldDelegate
	{
		public override bool ShouldReturn (UITextField textField)
		{
			textField.EndEditing(true);
			return true;
		}
	}
}

