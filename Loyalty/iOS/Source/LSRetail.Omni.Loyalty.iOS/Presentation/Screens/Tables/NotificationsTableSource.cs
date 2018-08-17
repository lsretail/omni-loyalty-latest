using System;
using System.Collections.Generic;
using Presentation.Utils;
using UIKit;
using Foundation;
using System.Linq;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Tables
{
    public class NotificationsTableSource : UITableViewSource
	{
		private List<Notification> notifications;
		public bool HasData { get { return this.notifications.Count > 0; } }

		public delegate void DeleteNotificationEventHandler(string id);
		public delegate void NotificationSelectedEventHandler(Notification notification);

		public event DeleteNotificationEventHandler DeleteNotification; 
		public event NotificationSelectedEventHandler NotificationSelected;

		public NotificationsTableSource ()
		{
			this.notifications = AppData.Notifications;
			RefreshData();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.notifications.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			NotificationsCell cell = tableView.DequeueReusableCell (NotificationsCell.Key) as NotificationsCell;
			if (cell == null)
				cell = new NotificationsCell();

			cell.DeleteNotification = btnDeleteNotificationOnClick;
			Notification notification = this.notifications [indexPath.Row];

			// Image
			ImageView imageView = notification.Images.FirstOrDefault();
			string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
			string imageId = (imageView != null ? imageView.Id : string.Empty);

            cell.SetValues(indexPath.Row, notification.Description, notification.Details, imageAvgColor, imageId);
            //prime= description
            //sec =detail
			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			Notification notification = this.notifications[indexPath.Row];
			if(NotificationSelected != null){
				NotificationSelected (notification);				
			}
			tableView.DeselectRow(indexPath, true);
		}

		public void HandleDeleteButtonPress(int cellIndexPathRow)
		{
			Notification notification = this.notifications[cellIndexPathRow];
			//this.controller.DeleteNotification(notification);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return NotificationsCell.GetCellHeight ();
		}

		public void RefreshData()
		{
			//TODO : Get notifications from WS?
			this.notifications = AppData.Notifications;
		}

		protected void btnDeleteNotificationOnClick( int row)
		{
			Notification notification = this.notifications[row];
			if(DeleteNotification != null)
			{
				DeleteNotification (notification.Id);				
			}
		}

	 /// Delete notification
		/*
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle) {
			case UITableViewCellEditingStyle.Delete:
				// remove the item from the underlying data source
				notifications.RemoveAt (indexPath.Row);
				btnDeleteNotificationOnClick (indexPath.Row);
				// delete the row from the table
				tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				break;
			case UITableViewCellEditingStyle.None:
				Console.WriteLine ("CommitEditingStyle:None called");
				break;
			}
		}
		*/
	}
}

