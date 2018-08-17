using System;
using System.Linq;

using Android.OS;
using Android.Views;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.Image;
using Presentation.Models;
using Presentation.Util;
using ImageView = Android.Widget.ImageView;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.Notifications
{
    public class NotificationDetailFragment : LoyaltyFragment, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private string notificationId;
        private View view;
        private Notification notification;
        private NotificationModel notificationModel;

        public static NotificationDetailFragment NewInstance()
        {
            var notificationDetail = new NotificationDetailFragment() {Arguments = new Bundle()};
            return notificationDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Bundle data = Arguments;

            notificationModel = new NotificationModel(Activity, null);

            notificationId = data.GetString(BundleConstants.NotificationId);

            view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.NotificationDetail);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.NotificationDetailScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            var primaryText = view.FindViewById<TextView>(Resource.Id.NotificationDetailViewPrimaryText);
            var secondaryText = view.FindViewById<TextView>(Resource.Id.NotificationDetailViewSecondaryText);
            var expirationDate = view.FindViewById<TextView>(Resource.Id.NotificationDetailViewExpirationDate); 

            notification = AppData.Device.UserLoggedOnToDevice.Notifications.FirstOrDefault(n => n.Id == notificationId);

            primaryText.Text = notification.Description;
            secondaryText.Text = notification.Details;

            if (notification.ExpiryDate.HasValue)
                expirationDate.Text = string.Format(GetString(Resource.String.DetailViewExpires), notification.ExpiryDate.Value.ToShortTimeString(), notification.ExpiryDate.Value.ToShortDateString());
            else
                expirationDate.Text = GetString(Resource.String.DetailViewNeverExpires);
            
            Util.Utils.ViewUtils.AddOnGlobalLayoutListener(view, this);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            UpdateNotification();
        }

        private async void UpdateNotification()
        {
            await notificationModel.UpdateNotification(notification, NotificationStatus.Read);
        }

        public void OnGlobalLayout()
        {
            new DetailImagePager(View, ChildFragmentManager, notification.Images);

            Util.Utils.ViewUtils.RemoveOnGlobalLayoutListener(View, this);
        }
    }
}