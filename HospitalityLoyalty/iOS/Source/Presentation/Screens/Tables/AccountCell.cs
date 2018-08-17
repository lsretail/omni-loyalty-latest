﻿using System;
using Presentation.Utils;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public class AccountCell : UITableViewCell
	{
		public static string Key = "ACCOUNTCELL";

		private UIView containerView;
		private UIImageView imageView;
		private UILabel lblName;
		private UILabel lblMemberScheme;
		private UILabel lblPointStatus;

		public AccountCell() : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.Default;

			this.containerView = new UIView();
			this.containerView.BackgroundColor = UIColor.Clear;

			this.imageView = new UIImageView();
			this.imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.imageView.ClipsToBounds = true;

			this.lblName = new UILabel();
			this.lblName.Font = UIFont.FromName("Helvetica", 18);
			this.lblName.TextAlignment = UITextAlignment.Left;
			this.lblName.BackgroundColor = UIColor.Clear;

			this.lblMemberScheme = new UILabel();
			this.lblMemberScheme.Font = UIFont.SystemFontOfSize(12);
			this.lblMemberScheme.TextAlignment = UITextAlignment.Left;
			this.lblMemberScheme.TextColor = UIColor.Gray;
			this.lblMemberScheme.BackgroundColor = UIColor.Clear;

			this.lblPointStatus = new UILabel();
			this.lblPointStatus.Font = UIFont.SystemFontOfSize(12);
			this.lblPointStatus.TextAlignment = UITextAlignment.Left;
			this.lblPointStatus.TextColor = UIColor.Gray;
			this.lblPointStatus.BackgroundColor = UIColor.Clear;
			if (!Presentation.Utils.Util.AppDelegate.ShowLoyaltyPoints)
				this.lblPointStatus.Hidden = true;

			this.containerView.AddSubview(this.imageView);
			this.containerView.AddSubview(this.lblName);
			this.containerView.AddSubview(this.lblMemberScheme);
			this.containerView.AddSubview(this.lblPointStatus);
			AddSubview(this.containerView);

			UpdataData();
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			const float margin = 5f;
			const float imageHeight = 30f;

			this.containerView.Frame = new CoreGraphics.CGRect(
				3 * margin,
				margin + 3f,
				this.Frame.Width - 3 * margin,
				this.Frame.Bottom - (2 * margin) - 6f
			);

			this.imageView.Frame = new CoreGraphics.CGRect(
				0,
				(this.containerView.Frame.Height - imageHeight) / 2,
				imageHeight,
				imageHeight
			);

			this.lblName.Frame = new CoreGraphics.CGRect(
				this.imageView.Frame.Right + 5 * margin,
				(this.containerView.Frame.Height - (18f + 18f + 20f)) / 2,
				this.containerView.Frame.Width,
				20f
			);

			this.lblMemberScheme.Frame = new CoreGraphics.CGRect(
				this.imageView.Frame.Right + 5 * margin,
				this.lblName.Frame.Bottom,
				this.containerView.Frame.Width,
				18f
			);

			this.lblPointStatus.Frame = new CoreGraphics.CGRect(
				this.imageView.Frame.Right + 5 * margin,
				this.lblMemberScheme.Frame.Bottom,
				this.containerView.Frame.Width,
				18f
			);
		}

		public void UpdataData()
		{
			this.lblName.Text = (!String.IsNullOrEmpty(AppData.Contact.FirstName) ? AppData.Contact.FirstName : AppData.Contact.UserName); ;
			this.lblMemberScheme.Text = GetMemberSchemeString();
			this.lblPointStatus.Text = GetPointBalanceString();
			this.imageView.Image = Utils.UI.GetColoredImage(Image.FromFile("/Icons/IconAccount.png"), AppColors.PrimaryColor);

			LayoutSubviews();
		}

		private string GetMemberSchemeString()
		{
			return AppData.Contact.Account.Scheme.Description + " " + LocalizationUtilities.LocalizedString("Account_Member", "member");
		}

		private string GetPointBalanceString()
		{
			return AppData.Contact.Account.PointBalance.ToString("N0") + " " + LocalizationUtilities.LocalizedString("Account_Points_Lowercase", "points");
		}
	}
}

