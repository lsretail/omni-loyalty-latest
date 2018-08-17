using System;
using CoreGraphics;
using Foundation;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation
{
	public class AboutUsView : BaseView
	{
		private UIImageView imageView;
		private UITableView tableView;
		private UITextView textView;

		private ErrorGettingDataView errorGettingDataView;

		private readonly IAboutUsViewListener listener;

		public AboutUsView(IAboutUsViewListener lstnr)
		{
			listener = lstnr;
			BackgroundColor = Utils.AppColors.BackgroundGray;

			// Imageview
			imageView = new UIImageView();
			imageView.Image = Utils.Image.FromFile("/Branding/Standard/StoreBannerTransparent.png");
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.BackgroundColor = UIColor.Clear;
			AddSubview(imageView);

			// TextView
			textView = new UITextView();
			textView.Editable = false;
			textView.ShowsVerticalScrollIndicator = true;
			textView.BackgroundColor = Utils.AppColors.BackgroundGray;
			AddSubview(textView);

			// Tableview
			tableView = new UITableView();
			tableView.ScrollEnabled = true;
			tableView.ShowsVerticalScrollIndicator = true;
			tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			tableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			AddSubviews(imageView, textView, tableView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			float imageViewHeight = 110;
			float textViewHeight = 80;
			float textViewMargin = 10;

			this.imageView.Frame = new CGRect (
				0,
				this.TopLayoutGuideLength + 40f, 
				this.Frame.Width, 
				imageViewHeight
			);
			this.textView.Frame = new CGRect (
				textViewMargin, 
				this.imageView.Frame.Bottom, 
				this.Frame.Width - 2 * textViewMargin, 
				textViewHeight
			);
			this.tableView.Frame = new CGRect (
				0, 
				this.textView.Frame.Bottom,
				this.Frame.Width, 
				this.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength
			);
		}

		public void UpdateData (string contactUsInfoString)
		{
			this.tableView.Source = new AboutUsTableSource (contactUsInfoString);
			(this.tableView.Source as AboutUsTableSource).SetInfoText = SetInfoText;
			(this.tableView.Source as AboutUsTableSource).PhoneNumberLinePressed = PhoneNumberLinePressed;
			(this.tableView.Source as AboutUsTableSource).EmailLinePressed = EmailLinePressed;
			(this.tableView.Source as AboutUsTableSource).WebsiteLinePressed = WebsiteLinePressed;
			this.tableView.ReloadData ();
		}

		public void SetInfoText(string text)
		{
			this.textView.Text = text;
			if (text == string.Empty)
			{
				// No infotext to display - hide textview and move tableview up on the screen
				this.textView.Hidden = true;
				this.tableView.Frame = new CGRect(this.tableView.Frame.X, this.textView.Frame.Y, this.tableView.Frame.Width, this.tableView.Frame.Height);
			} 
		}

		public void PhoneNumberLinePressed(string phoneNumber)
		{
			if (phoneNumber != null && phoneNumber != string.Empty)
			{
				NSUrl callUrl = new NSUrl ("tel://" + phoneNumber);
				UIApplication.SharedApplication.OpenUrl (callUrl);
			}
		}

		public void EmailLinePressed(string email)
		{
			if (email != null && email != string.Empty)
			{
				NSUrl emailUrl = new NSUrl ("mailto:" + email);
				UIApplication.SharedApplication.OpenUrl (emailUrl);
			}
		}

		public void WebsiteLinePressed(string webUrl)
		{
			if (webUrl != null && webUrl != string.Empty)
			{
				if (!webUrl.StartsWith ("http://"))
					webUrl = "http://" + webUrl;

				NSUrl websiteUrl = new NSUrl (webUrl);
				UIApplication.SharedApplication.OpenUrl(websiteUrl);
			}
		}

		public void ShowErrorGettingDataView()
		{
			if (this.errorGettingDataView == null)
			{
				CGRect errorGettingDataViewFrame = new CGRect(0, this.TopLayoutGuideLength, this.Bounds.Width, this.Bounds.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength);
				this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame);
				this.errorGettingDataView.Retry += Retry;
				this.AddSubview(this.errorGettingDataView);
			}
			else
			{
				this.errorGettingDataView.Hidden = false;
			}
		}

		private void Retry(object sender, EventArgs e)
		{
			this.listener.GetAboutUsInformation();
		}

		public void HideErrorGettingDataView()
		{
			if (this.errorGettingDataView != null)
				this.errorGettingDataView.Hidden = true;
		}

		public interface IAboutUsViewListener
		{
			void GetAboutUsInformation();
		}
	}
}

