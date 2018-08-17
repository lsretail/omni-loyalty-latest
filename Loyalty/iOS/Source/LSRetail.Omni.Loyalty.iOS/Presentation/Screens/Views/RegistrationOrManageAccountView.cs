using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation
{
    public class RegistrationOrManageAccountView : BaseView
	{
		private UIScrollView containerView;
		private UITableView attributesTableView;
		private MemberContactAttributesTableSource attributesTableViewSource;
		private UIView attributesTopHairLine;
		private UIView attributesBottomHairLine;
		private UILabel lblPasswordPolicy;

		private UITableView profilesTableView;
		private MemberContactProfilesTableSource profilesTableViewSource;
		private UIView profilesTopHairLine;
		private UIView profilesBottomHairLine;

		private UIButton btnConfirm;

		private bool isRegistration;  // is this a view for registration controller or manage account controller

		public delegate void ConfirmEventHandler(List<MemberContactAttributesDTO> memberContactAttributesDTO, List<Profile> profiles);
		public event ConfirmEventHandler Confirm;

		public RegistrationOrManageAccountView (List<MemberContactAttributesDTO> memberContactAttributesDTO, List<Profile> profiles, bool isRegistration, string passwordPolicy = "")
		{
			this.isRegistration = isRegistration;
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			// Let's add a tap gesture recognizer to dismiss the keyboard when clicking away from a text input field
			var endEditingTapRecognizer = new UITapGestureRecognizer(() => this.EndEditing(true));
			endEditingTapRecognizer.CancelsTouchesInView = false;	// Set this to false so we still forward the gesture down the subview chain
			this.AddGestureRecognizer(endEditingTapRecognizer);

			this.containerView = new UIScrollView();
			this.AddSubview(this.containerView);

			this.attributesTableView = new UITableView();
			this.attributesTableViewSource = new MemberContactAttributesTableSource(this.attributesTableView, memberContactAttributesDTO);
			this.attributesTableViewSource.ResizeTableView += this.SetNeedsLayout;
			this.attributesTableView.Source = this.attributesTableViewSource;
			this.attributesTableView.ScrollEnabled = false;
			this.attributesTableView.BackgroundColor = UIColor.Clear;
			this.containerView.AddSubview(this.attributesTableView);

			this.attributesTopHairLine = new UIView();
			this.attributesTopHairLine.BackgroundColor = this.attributesTableView.SeparatorColor;
			this.containerView.AddSubview(this.attributesTopHairLine);

			this.attributesBottomHairLine = new UIView();
			this.attributesBottomHairLine.BackgroundColor = this.attributesTableView.SeparatorColor;
			this.containerView.AddSubview(this.attributesBottomHairLine);

			this.lblPasswordPolicy = new UILabel()
			{
				Text = passwordPolicy,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize(13),
				TextColor = Utils.AppColors.PrimaryColor
			};
			this.containerView.AddSubview(lblPasswordPolicy);

			if(profiles.Count > 0)
			{
				this.profilesTableView = new UITableView();
				this.profilesTableViewSource = new MemberContactProfilesTableSource(profiles);
				this.profilesTableView.Source = this.profilesTableViewSource;
				this.profilesTableView.ScrollEnabled = false;
				this.profilesTableView.BackgroundColor = UIColor.Clear;
				this.containerView.AddSubview(this.profilesTableView);

				this.profilesTopHairLine = new UIView();
				this.profilesTopHairLine.BackgroundColor = this.attributesTableView.SeparatorColor;
				this.containerView.AddSubview(this.profilesTopHairLine);

				this.profilesBottomHairLine = new UIView();
				this.profilesBottomHairLine.BackgroundColor = this.attributesTableView.SeparatorColor;
				this.containerView.AddSubview(this.profilesBottomHairLine);
			}

			this.btnConfirm = new UIButton ();
			this.btnConfirm.BackgroundColor = Utils.AppColors.PrimaryColor;

			if(this.isRegistration)
			{
				this.btnConfirm.SetTitle (LocalizationUtilities.LocalizedString("Account_CreateAccount", "Create account"), UIControlState.Normal);
			}
			else
			{
				this.btnConfirm.SetTitle (LocalizationUtilities.LocalizedString("Account_UpdateAccount", "Update account"), UIControlState.Normal);
			}

			this.btnConfirm.SetTitleColor (UIColor.White, UIControlState.Normal);
			this.btnConfirm.Layer.CornerRadius = 3f;
			this.btnConfirm.TouchUpInside += (object sender, EventArgs e) => 
			{ 
				if(this.Confirm != null)
					this.Confirm(memberContactAttributesDTO, profiles);
			};
			this.containerView.AddSubview(this.btnConfirm);

			RegisterKeyboardNotificationHandling();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			nfloat margin = 20f;
			nfloat hairLineHeight = 0.5f;
			nfloat btnHeight = 44f;
			nfloat passwordPolicyHeight = 20f;

			this.containerView.Frame = this.Frame;
			this.containerView.ContentInset = UIEdgeInsets.Zero;

			this.attributesTableView.Frame = new CGRect(0f, this.TopLayoutGuideLength + margin, this.Frame.Width, this.attributesTableViewSource.GetRequiredTableViewHeight());
			this.attributesTableView.ContentInset = UIEdgeInsets.Zero;

			this.attributesTopHairLine.Frame = new CGRect(0f, this.attributesTableView.Frame.Top, this.Frame.Width, hairLineHeight);
			this.attributesBottomHairLine.Frame = new CGRect(0f, this.attributesTableView.Frame.Bottom - hairLineHeight, this.Frame.Width, hairLineHeight);

			if(this.lblPasswordPolicy.Text != string.Empty)
				this.lblPasswordPolicy.Frame = new CGRect(0f, this.attributesTableView.Frame.Bottom + margin/4, this.attributesTableView.Frame.Width, passwordPolicyHeight);
			else
				passwordPolicyHeight = 0f;

			if(this.profilesTableView != null)
			{
				this.profilesTableView.Frame = new CGRect(0f, this.attributesTableView.Frame.Bottom + margin + 0.5 * passwordPolicyHeight, this.Frame.Width, this.profilesTableViewSource.GetRequiredTableViewHeight());
				this.profilesTableView.ContentInset = UIEdgeInsets.Zero;

				this.profilesTopHairLine.Frame = new CGRect(0f, this.profilesTableView.Frame.Top + this.profilesTableViewSource.GetHeightForHeader(this.profilesTableView, 0), this.Frame.Width, hairLineHeight);
				this.profilesBottomHairLine.Frame = new CGRect(0f, this.profilesTableView.Frame.Bottom - hairLineHeight, this.Frame.Width, hairLineHeight);
			}

			this.btnConfirm.Frame = new CGRect(margin, this.profilesTableView != null ? this.profilesTableView.Frame.Bottom + margin : this.attributesTableView.Frame.Bottom + margin + passwordPolicyHeight, this.Frame.Width - 2*margin, btnHeight);

			this.containerView.ContentSize = new CGSize(this.Frame.Width, this.btnConfirm.Frame.Bottom + margin + this.BottomLayoutGuideLength);
		}
			
		public void FlashScrollIndicator()
		{
			this.containerView.FlashScrollIndicators();
		}

		private void RegisterKeyboardNotificationHandling()
		{
			UIKeyboard.Notifications.ObserveWillShow((sender, e) => {

				var keyboard = UIKeyboard.FrameBeginFromNotification(e.Notification);
				var keyboardHeight = keyboard.Height;

				var contentInsets = new UIEdgeInsets(0, 0, keyboardHeight, 0);
				this.containerView.ContentInset = contentInsets;
				this.containerView.ScrollIndicatorInsets = contentInsets;
			});

			UIKeyboard.Notifications.ObserveDidHide((sender, e) => {

				this.containerView.ContentInset = UIEdgeInsets.Zero;
				this.containerView.ScrollIndicatorInsets = UIEdgeInsets.Zero;

			});
		}
	}
}

