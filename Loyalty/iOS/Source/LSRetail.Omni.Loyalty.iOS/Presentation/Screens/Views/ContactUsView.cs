using System;
using UIKit;
using CoreGraphics;
using Foundation;
using Presentation.Screens;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
	public class ContactUsView : BaseView
	{
		private UIImageView imageView;
		private UITableView tableView;
		private UITextView textView;
		private ErrorGettingDataView errorGettingDataView;

		public delegate void GetDataEventHandler ();
		public event GetDataEventHandler GetData;

		public ContactUsView ()
		{
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			// Imageview
			this.imageView = new UIImageView();
			this.imageView.Image = ImageUtilities.FromFile("/Branding/Standard/StoreBannerTransparent.png");
			this.imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.imageView.BackgroundColor = UIColor.Clear;

			// TextView
			this.textView = new UITextView();
			this.textView.Editable = false;
			this.textView.ShowsVerticalScrollIndicator = true;
			this.textView.BackgroundColor = Utils.AppColors.BackgroundGray;

			// Tableview
			this.tableView = new UITableView();
			this.tableView.ScrollEnabled = true;
			this.tableView.ShowsVerticalScrollIndicator = true;
			this.tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.tableView.BackgroundColor = Utils.AppColors.BackgroundGray;
		
			this.AddSubview(this.imageView);
			this.AddSubview(this.textView);
			this.AddSubview(this.tableView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

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
				this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame, GetDataFromController);
				this.AddSubview(this.errorGettingDataView);
			}
			else
			{
				this.errorGettingDataView.Hidden = false;
			}
		}

		public void GetDataFromController ()
		{
			if (GetData != null) 
			{
				GetData ();	
			}
		}

		public void HideErrorGettingDataView()
		{
			if (this.errorGettingDataView != null)
				this.errorGettingDataView.Hidden = true;
		}

		public void UpdateData (string contactUsInfoString)
		{
			this.tableView.Source = new ContactUsTableSource (contactUsInfoString);
			(this.tableView.Source as ContactUsTableSource).SetInfoText = SetInfoText;
			(this.tableView.Source as ContactUsTableSource).PhoneNumberLinePressed = PhoneNumberLinePressed;
			(this.tableView.Source as ContactUsTableSource).EmailLinePressed = EmailLinePressed;
			(this.tableView.Source as ContactUsTableSource).WebsiteLinePressed = WebsiteLinePressed;
			this.tableView.ReloadData ();
		}
	}
}

