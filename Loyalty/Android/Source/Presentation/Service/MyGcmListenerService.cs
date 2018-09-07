using System.Linq;

using Android.OS;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Graphics;
using Android.Support.V4.App;

using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Service
{
    [Service(Exported = false)]
    [IntentFilter(new string[] { GoogleCloudMessaging.IntentFilterActionReceive })]
    public class MyGcmListenerService : GcmListenerService
    {
        private const string Tag = "MyGcmListenerService";

        public override void OnMessageReceived(string @from, Bundle data)
        {
            Utils.LogUtils.Log("Message Received");
            //TODO show notification
            //Push Notification arrived - print out the keys/values
            if (string.IsNullOrEmpty(Utils.PreferenceUtils.GetString(this, Utils.PreferenceUtils.FcmRegistrationId)))
                return;

            var model = new NotificationModel(this, null);
            model.GetNotificationsByContactId(
                (notifications) =>
            {
                    if (notifications.Count > 0)
                {
                    var unreadNotifications = notifications.Where(x => x.Status == NotificationStatus.New).ToList();

                    var manager = (NotificationManager)GetSystemService(Context.NotificationService);
                    var count = unreadNotifications.Count;

                    var notificationText = count == 1
                        ? GetString(Resource.String.NotificationBarNotification)
                        : GetString(Resource.String.NotificationBarNotifications);
                    NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this);
                    notificationBuilder.SetContentTitle(GetString(Resource.String.ApplicationTitle));
                    notificationBuilder.SetContentText(
                        string.Format(GetString(Resource.String.NotificationBarContentText), count,
                            notificationText));
                    notificationBuilder.SetContentInfo(count.ToString());

                    //var startIntent = new Intent(this, typeof(Activities.Notifications.NotificationActivity));
                    //startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);
                    var startIntent = new Intent(this, typeof(Activities.StartUp.StartUp));
                    startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);

                    var pendingIntent = PendingIntent.GetActivity(this, 0, startIntent,
                        PendingIntentFlags.CancelCurrent);

                    notificationBuilder.SetContentIntent(pendingIntent);
                    notificationBuilder.SetSmallIcon(Resource.Drawable.ic_stat_loyalty);
                    notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_launcher));
                    //notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_stat_loyalty));

                    notificationBuilder.SetDefaults((int)NotificationDefaults.All);
                    notificationBuilder.SetAutoCancel(true);

                        //todo image
                        /*if (count == 1 && !string.IsNullOrEmpty(notifications[0].Image)) //BIG PICTURE STYLE
                        {
                            var style = new NotificationCompat.BigPictureStyle();

                            style.SetBigContentTitle(GetString(Resource.String.NotificationBarTitle));
                            style.SetSummaryText(notifications[0].PrimaryText);

                            style.BigPicture(Utils.DecodeImage(notifications[0].Image));

                            notificationBuilder.SetStyle(style);
                        }*/
                        //else //INBOX STYLE
                        {
                            var style = new NotificationCompat.InboxStyle();

                            style.SetBigContentTitle(GetString(Resource.String.ApplicationTitle));
                            style.SetSummaryText(
                                string.Format(GetString(Resource.String.NotificationBarContentText),
                                    count, notificationText));

                            foreach (var notification in unreadNotifications)
                            {
                                style.AddLine(notification.Description);
                            }

                            notificationBuilder.SetStyle(style);
                        }

                        manager.Notify(0, notificationBuilder.Build());
                    }
                    else
                    {
                        var manager = (NotificationManager)GetSystemService(Context.NotificationService);

                        var notificationText = GetString(Resource.String.NotificationBarNotifications);
                        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this);
                        notificationBuilder.SetContentTitle(GetString(Resource.String.ApplicationTitle));
                        notificationBuilder.SetContentText(string.Format(GetString(Resource.String.NotificationBarContentText), "", notificationText).Replace("  ", " "));

                        //var startIntent = new Intent(this, typeof(Activities.Notifications.NotificationActivity));
                        //startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);
                        var startIntent = new Intent(this, typeof(Activities.StartUp.StartUp));
                        startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);

                        var pendingIntent = PendingIntent.GetActivity(this, 0, startIntent,
                            PendingIntentFlags.CancelCurrent);

                        notificationBuilder.SetContentIntent(pendingIntent);
                        notificationBuilder.SetSmallIcon(Resource.Drawable.ic_stat_loyalty);
                        notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_launcher));
                        //notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_stat_loyalty));

                        notificationBuilder.SetDefaults((int)NotificationDefaults.All);
                        notificationBuilder.SetAutoCancel(true);

                        manager.Notify(0, notificationBuilder.Build());
                    }
                });
        }
    }
}
 