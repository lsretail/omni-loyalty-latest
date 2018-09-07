using System.Linq;

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
    [BroadcastReceiver(Exported = true, Permission = "com.google.android.c2dm.permission.SEND")]
    [IntentFilter(new string[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmReceiver : GcmReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (string.IsNullOrEmpty(Utils.PreferenceUtils.GetString(context, Utils.PreferenceUtils.FcmRegistrationId)))
                return;

            if (AppData.Device.UserLoggedOnToDevice == null)
                return;

            var model = new NotificationModel(context, null);
            model.GetNotificationsByContactId(
                (notifications) =>
                {
                    if (notifications.Count > 0)
                    {
                        var unreadNotifications = notifications.Where(x => x.Status == NotificationStatus.New).ToList();

                        var manager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                        var count = unreadNotifications.Count;

                        var notificationText = count == 1
                            ? context.GetString(Resource.String.NotificationBarNotification)
                            : context.GetString(Resource.String.NotificationBarNotifications);
                        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(context);
                        notificationBuilder.SetContentTitle(context.GetString(Resource.String.ApplicationTitle));
                        notificationBuilder.SetContentText(
                            string.Format(context.GetString(Resource.String.NotificationBarContentText), count,
                                notificationText));
                        notificationBuilder.SetContentInfo(count.ToString());

                        //var startIntent = new Intent(this, typeof(Activities.Notifications.NotificationActivity));
                        //startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);
                        var startIntent = new Intent(context, typeof(Activities.StartUp.StartUp));
                        startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);

                        var pendingIntent = PendingIntent.GetActivity(context, 0, startIntent,
                            PendingIntentFlags.CancelCurrent);

                        notificationBuilder.SetContentIntent(pendingIntent);
                        notificationBuilder.SetSmallIcon(Resource.Drawable.ic_stat_loyalty);
                        notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_launcher));
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

                            style.SetBigContentTitle(context.GetString(Resource.String.ApplicationTitle));
                            style.SetSummaryText(
                                string.Format(context.GetString(Resource.String.NotificationBarContentText),
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
                        var manager = (NotificationManager)context.GetSystemService(Context.NotificationService);

                        var notificationText = context.GetString(Resource.String.NotificationBarNotifications);
                        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(context);
                        notificationBuilder.SetContentTitle(context.GetString(Resource.String.ApplicationTitle));
                        notificationBuilder.SetContentText(string.Format(context.GetString(Resource.String.NotificationBarContentText), "", notificationText).Replace("  ", " "));

                        //var startIntent = new Intent(this, typeof(Activities.Notifications.NotificationActivity));
                        //startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);
                        var startIntent = new Intent(context, typeof(Activities.StartUp.StartUp));
                        startIntent.PutExtra(BundleConstants.LoadNotificationsFromService, true);

                        var pendingIntent = PendingIntent.GetActivity(context, 0, startIntent,
                            PendingIntentFlags.CancelCurrent);

                        notificationBuilder.SetContentIntent(pendingIntent);
                        notificationBuilder.SetSmallIcon(Resource.Drawable.ic_stat_loyalty);
                        notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_launcher));
                        //notificationBuilder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_stat_loyalty));

                        notificationBuilder.SetDefaults((int)NotificationDefaults.All);
                        notificationBuilder.SetAutoCancel(true);

                        manager.Notify(0, notificationBuilder.Build());
                    }
                });
        }
    }
}