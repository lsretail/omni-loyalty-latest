using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;
using Presentation.Tables;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation
{
    public class NotificationsView : BaseView
	{
		private UITableView tblNotifications;
		private UIView vwNoData;
		private UILabel lblNoData;
		private UIRefreshControl refreshControl;
		private bool allowPullToRefresh = true;

		private float labelHeight = 20;
	
		public delegate void DeleteNotificationEventHandler(string id);
		public delegate void RefreshNotificationsEventHandler(Action onSuccess, Action onFailure);
		public delegate void NotificationSelectedEventHandler(Notification notification);
			
		public event DeleteNotificationEventHandler DeleteNotification; 	
		public event RefreshNotificationsEventHandler RefreshNotifications;
		public event NotificationSelectedEventHandler NotificationSelected;

		public NotificationsView ()
		{
			this.tblNotifications = new UITableView ()
			{
				BackgroundColor = AppColors.BackgroundGray,
				SeparatorStyle = UITableViewCellSeparatorStyle.None,
				Hidden = false
			};

			NotificationsTableSource notificationTableSource = new NotificationsTableSource ();
			notificationTableSource.DeleteNotification += DeleteNotificationOnClick;
			notificationTableSource.NotificationSelected += OnNotificationSelected;
			this.tblNotifications.Source = notificationTableSource;

			this.vwNoData = new UIView();
			this.vwNoData.BackgroundColor = UIColor.Clear;

			this.lblNoData = new UILabel();
			this.lblNoData.TextColor = UIColor.Gray;
			this.lblNoData.TextAlignment = UITextAlignment.Center;
			this.lblNoData.Font = UIFont.SystemFontOfSize(14);
			this.vwNoData.AddSubview(lblNoData);

			this.refreshControl = new UIRefreshControl();
			this.refreshControl.ValueChanged += RefreshControlValueChanged;
			if (this.allowPullToRefresh)
				this.tblNotifications.AddSubview(refreshControl);

			AddSubview(this.vwNoData);
			AddSubview (this.tblNotifications);	
		}
			

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

            this.tblNotifications.Frame = new CGRect (0, this.TopLayoutGuideLength, this.Frame.Width, this.Frame.Height - this.TopLayoutGuideLength);
			this.lblNoData.Frame = new CGRect(0, vwNoData.Bounds.Height / 2 - labelHeight / 2, vwNoData.Bounds.Width, labelHeight);
			this.vwNoData.Frame = new CGRect(0, 0, this.Bounds.Width, this.Bounds.Height);
		}
			
		#region Event Handlers
		protected void DeleteNotificationOnClick (string id)
		{
			if (DeleteNotification != null)
			{
				DeleteNotification (id);				
			}
		}

		protected void RefreshControlValueChanged(object sender, EventArgs e)
		{
			if (RefreshNotifications != null) 
			{
				RefreshNotifications (RefreshDataSuccess, RefreshDataFailure);
			}
		}

		protected void OnNotificationSelected (Notification notification)
		{
			if (NotificationSelected != null)
			{
				NotificationSelected (notification);
			}
		}
		#endregion

		#region Public Functions

		public void RefreshData()
		{
			(this.tblNotifications.Source as NotificationsTableSource).RefreshData();
			this.tblNotifications.ReloadData();

			RefreshNoDataView ();
		}

		#endregion

		#region Private Functions

		private void RefreshNoDataView()
		{
			if (this.tblNotifications.Source == null)
				return;

			if (!AppData.UserLoggedIn)
				ShowNoDataView(LocalizationUtilities.LocalizedString("Notification_LoginNotifications", "Please log in to see notifications."));
			else if (!(this.tblNotifications.Source as NotificationsTableSource).HasData)
				ShowNoDataView(LocalizationUtilities.LocalizedString("Notification_NoNotifications", "No notifications."));
			else
				HideNoDataView();
		}

		private void ShowNoDataView(string displayText)
		{
			this.vwNoData.Hidden = false;
			this.tblNotifications.Hidden = true;
			this.lblNoData.Text = displayText;
		}

		private void HideNoDataView()
		{
			this.vwNoData.Hidden = true;
			this.tblNotifications.Hidden = false;
		}

		private void RefreshDataSuccess()
		{
			System.Diagnostics.Debug.WriteLine ("NotificationScreen.RefreshData success");
			this.refreshControl.EndRefreshing();
			RefreshWithAnimation();
		}

		private void RefreshDataFailure()
		{
			System.Diagnostics.Debug.WriteLine ("NotificationScreen.RefreshData failure");
			this.refreshControl.EndRefreshing();
			RefreshNoDataView();
		}

		private void RefreshWithAnimation()
		{
			if (this.tblNotifications.Source == null)
				return;

			CATransition transition = new CATransition ();
			transition.Duration = 0.3;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			transition.Type = CATransition.TransitionPush;
			transition.Subtype = CATransition.TransitionFade;
			transition.FillMode = CAFillMode.Both;

			this.tblNotifications.Layer.AddAnimation (transition, null);
			(this.tblNotifications.Source as NotificationsTableSource).RefreshData();
			this.tblNotifications.ReloadData();

			RefreshNoDataView ();
		}

		#endregion
	}
}

