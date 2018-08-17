using System;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class HiddenSettingsView : BaseView
	{
		private UIButton btnApply;
		private UILabel urlLabel;
		private UITextField urlTextField;
		private UILabel lblVersion;

		private readonly IHiddenSettingsListeners listener;

		public HiddenSettingsView(IHiddenSettingsListeners listener)
		{
			this.listener = listener;
			// UI elements
			this.urlLabel = new UILabel();
			urlLabel.Text = LocalizationUtilities.LocalizedString("Hidden_Settings_URL", "URL");
			urlLabel.Font = UIFont.SystemFontOfSize(14);
			urlLabel.TextAlignment = UITextAlignment.Left;
			urlLabel.TextColor = AppColors.PrimaryColor;
			this.AddSubview(this.urlLabel);

			this.urlTextField = new UITextField();
			urlTextField.Text = LocalizationUtilities.LocalizedString("Hidden_Settings_URL", "URL");
			urlTextField.Delegate = new CustomTextFieldDelegate();
			this.AddSubview(urlTextField);

			this.btnApply = new UIButton();
			btnApply.SetTitle(LocalizationUtilities.LocalizedString("HiddenSettings_Ping", "Ping"), UIControlState.Normal);
			btnApply.BackgroundColor = AppColors.PrimaryColor;
			btnApply.Layer.CornerRadius = 2;
			btnApply.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.PingButtonClicked();
			};
			this.AddSubview(btnApply);

			this.lblVersion = new UILabel();
			this.lblVersion.TextAlignment = UITextAlignment.Center;
			this.AddSubview(this.lblVersion);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			float margin = 20f;
			float buttonHeight = 50f;
			float buttonMargin = 20f;

			this.urlLabel.Frame = new CGRect(
				margin,
				this.TopLayoutGuideLength + 30f,
				this.Frame.Width - 2 * margin, 20f
			);

			this.urlTextField.Frame = new CGRect(
				margin,
				this.urlLabel.Frame.Bottom,
				this.Frame.Width - 2 * margin,
				20f
			);

			this.btnApply.Frame = new CGRect(
				buttonMargin,
				this.urlTextField.Frame.Bottom + 2 * margin,
				this.Frame.Width - 2 * buttonMargin,
				buttonHeight
			);

			this.lblVersion.Frame = new CGRect(
				0,
				this.btnApply.Frame.Bottom + 30f,
				this.Frame.Width,
				20f
			);
		}

		public void UpdateData(string url)
		{
			this.urlTextField.Text = url;
			this.lblVersion.Text = "Version " + NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")];
		}

		public string GetUrlText()
		{
			return this.urlTextField.Text;
		}

		public interface IHiddenSettingsListeners
		{
			void PingButtonClicked();
		}

		class CustomTextFieldDelegate : UITextFieldDelegate
		{
			public override bool ShouldReturn(UITextField textField)
			{
				textField.EndEditing(true);
				return true;
			}
		}
	}
}

