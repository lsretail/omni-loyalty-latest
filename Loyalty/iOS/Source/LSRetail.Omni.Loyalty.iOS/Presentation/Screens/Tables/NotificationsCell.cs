using System;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;
using UIKit;

namespace Presentation.Tables
{
	public class NotificationsCell : UITableViewCell
	{
		public static string Key = "NotificationsTableViewCell";

		protected int id;

		private const float labelHeight = 20f;
		private const float margin = 5f;
		private const float doubleMargin = 10f;
		private const float buttonDimensions = 40f;
		private const float interCellSpacing = 10f;
		private const float imageViewHeight = 60f;

		private UIView vwCustomContent;
		private UIImageView ivwNotification;
		private UILabel lblTitle;
		private UILabel lblExtraInfo;
		private UIButton btnDelete;

		public delegate void DeleteNotificationDelegate(int id);
		public DeleteNotificationDelegate DeleteNotification; 

		public NotificationsCell ( ) : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			this.vwCustomContent = new UIView();
			this.vwCustomContent.BackgroundColor = UIColor.White;
			this.vwCustomContent.Tag = 1;
			this.ContentView.AddSubview(vwCustomContent);

			this.ivwNotification = new UIImageView();
			this.ivwNotification.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.ivwNotification.ClipsToBounds = true;
			this.ivwNotification.BackgroundColor = UIColor.Purple;
			this.vwCustomContent.AddSubview(ivwNotification);

			this.lblTitle = new UILabel();
			this.lblTitle.BackgroundColor = UIColor.Clear;
			this.lblTitle.TextColor = Utils.AppColors.TextColor;
			this.vwCustomContent.AddSubview(lblTitle);

			this.lblExtraInfo = new UILabel();
			this.lblExtraInfo.BackgroundColor = UIColor.Clear;
			this.lblExtraInfo.TextColor = UIColor.Gray;
			this.lblExtraInfo.Font = UIFont.SystemFontOfSize(12f);
			this.vwCustomContent.AddSubview(lblExtraInfo);

			this.btnDelete = new UIButton();
            this.btnDelete.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.Red), UIControlState.Normal);
			this.btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.btnDelete.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			this.btnDelete.BackgroundColor = UIColor.Clear;
			this.btnDelete.TouchUpInside += (object sender, EventArgs e) => { 
				btnDeleteNotificationOnClick();
				//this.btnDelete.SetImage(GetDeleteButtonIcon(), UIControlState.Normal);
			};

			this.vwCustomContent.AddSubview (this.btnDelete);
		}
			
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.vwCustomContent.Frame = new CoreGraphics.CGRect (
				this.ContentView.Frame.X, 
				this.ContentView.Frame.Y + doubleMargin, 
				this.ContentView.Frame.Width, 
				this.ContentView.Frame.Height - doubleMargin
			);
			this.ivwNotification.Frame = new CoreGraphics.CGRect (
				doubleMargin, 
				doubleMargin, 
				imageViewHeight, 
				imageViewHeight
			);
			this.lblTitle.Frame = new CoreGraphics.CGRect (
				this.ivwNotification.Frame.Right + doubleMargin,
                this.ivwNotification.Frame.Y + 2*doubleMargin, 
				this.vwCustomContent.Frame.Width - (this.ivwNotification.Frame.Width + (6 * margin)) , 
				labelHeight
			);
			this.lblExtraInfo.Frame = new CoreGraphics.CGRect (
				this.ivwNotification.Frame.Right + doubleMargin, 
				this.lblTitle.Frame.Bottom, 
				this.vwCustomContent.Frame.Width - (this.ivwNotification.Frame.Width + (6 * margin)),
				labelHeight
			);
			this.btnDelete.Frame = new CoreGraphics.CGRect (
				this.Frame.Width - (buttonDimensions + doubleMargin),
                this.ivwNotification.Frame.Y,
                buttonDimensions,
                buttonDimensions - 10f
			);
		}
			
		protected void btnDeleteNotificationOnClick()
		{
			if(DeleteNotification != null)
			{
				DeleteNotification (this.id);				
			}
		}

		private UIImage GetDeleteButtonIcon()
		{
			return ImageUtilities.GetColoredImage (ImageUtilities.FromFile ("iconDeleteWhite.png"), UIColor.Gray);
		}


		public void SetValues(int id, string title, string extraInfo, string imageAvgColorHex, string imageId)
		{
			this.id = id;
			lblTitle.Text = title;
			lblExtraInfo.Text = extraInfo;

			if (String.IsNullOrEmpty(imageAvgColorHex))
				imageAvgColorHex = "E0E0E0"; // Default to light gray
			ivwNotification.BackgroundColor = ColorUtilities.GetUIColorFromHexString(imageAvgColorHex);

			Utils.UI.LoadImageToImageView(imageId, false, ivwNotification, new ImageSize(100, 100), this.id.ToString());
		}

		public static float GetCellHeight ()
		{
			return 2 * doubleMargin + (2 * labelHeight) + buttonDimensions;
		}
	}
}

