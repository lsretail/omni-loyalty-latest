using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Presentation.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Notifications;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Offers;

namespace Presentation.Models
{
    class NotificationModel : BaseModel
    {
        private NotificationService service;
        private NotificationLocalService localService;

        public NotificationModel(Context context, IRefreshableActivity refreshableActivity)
            : base(context, refreshableActivity)
        {

            localService = new NotificationLocalService(new Infrastructure.Data.SQLite.Notifications.NotificationRepository());
        }

        public async Task<bool> UpdateNotification(Notification updateNotification, NotificationStatus status)
        {
            return await UpdateNotifications(new List<Notification>() {updateNotification}, status);
        }

        public async Task<bool> UpdateNotifications(List<Notification> updateNotifications, NotificationStatus status)
        {
            bool success = false;

            ShowIndicator(true);

            BeginWsCall();

            var updateNotificationIndexes = new List<int>();

            try
            {
                success = await service.UpdateStatusAsync(AppData.Device.UserLoggedOnToDevice.Id, updateNotifications.Select(x => x.Id).ToList(), status);

                if (success)
                {
                    foreach (var changedNotification in updateNotifications)
                    {
                        foreach (var notification in AppData.Device.UserLoggedOnToDevice.Notifications)
                        {
                            if (notification.Id == changedNotification.Id)
                            {
                                if (status == NotificationStatus.Closed)
                                {
                                    updateNotificationIndexes.Add(AppData.Device.UserLoggedOnToDevice.Notifications.IndexOf(notification));
                                    AppData.Device.UserLoggedOnToDevice.Notifications.Remove(notification);
                                }
                                else
                                {
                                    notification.Status = status;
                                }
                                break;
                            }
                        }
                    }

                    SaveLocalNotifications();

                    SendBroadcast(Utils.BroadcastUtils.NotificationsUpdated);

                    ShowIndicator(false);

                    if (status == NotificationStatus.Closed)
                    {
                        ShowSnackbar(
                            AddSnackbarAction(
                                CreateSnackbar(Context.GetString(Resource.String.ApplicatioItemDeleted)),
                                Context.GetString(Resource.String.ApplicationUndo), async view =>
                                {
                                    for (int i = 0; i < updateNotifications.Count; i++)
                                    {
                                        var updateNotification = updateNotifications[i];

                                        updateNotification.Status = NotificationStatus.Closed;

                                        AppData.Device.UserLoggedOnToDevice.Notifications.Insert(updateNotificationIndexes[i], updateNotification);
                                    }

                                    await UpdateNotifications(updateNotifications, NotificationStatus.New);
                                }));
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            return success;
        }

        public async Task GetNotificationsByContactId()
        {
            ShowIndicator(true);

            BeginWsCall();

            try
            {
                var notifications = await service.GetNotificationsAsync(AppData.Device.UserLoggedOnToDevice.Id, Int32.MaxValue);

                AppData.Device.UserLoggedOnToDevice.Notifications = notifications;

                SaveLocalNotifications();

                SendBroadcast(Utils.BroadcastUtils.NotificationsUpdated);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);
        }

        public void GetNotificationsByContactId(Action<List<Notification>> onSuccess)
        {
            //from background context

            BeginWsCall();

            var looper = Context.MainLooper;
            var handler = new Handler(looper);
            handler.Post(async () =>
                {
                    try
                    {
                        var notifications = await service.GetNotificationsAsync(AppData.Device.UserLoggedOnToDevice.Id, Int32.MaxValue);

                        if (notifications == null)
                        {
                            notifications = new List<Notification>();
                        }

                        onSuccess(notifications);
                    }
                    catch (Exception)
                    {
                        onSuccess(new List<Notification>());
                    }
                    
                });
        }

        private void SaveLocalNotifications()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += (sender, args) => localService.SaveNotifications(AppData.Device.UserLoggedOnToDevice.Notifications);

            worker.RunWorkerAsync();
        }

        protected override void CreateService()
        {
            service = new NotificationService(new NotificationRepository());
        }
    }
}