using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Util;
using Android.Support.V4.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Notifications
{
    [Activity(Label = "", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class NotificationDetailActivity : LoyaltyFragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BaseSinglePanelLayout);

            Bundle extras = Intent.Extras;

            if (bundle == null)
            {
                var details = NotificationDetailFragment.NewInstance();
                details.Arguments = extras;

                //todo fragment
                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                //fragmentNotification.Add(Android.Resource.Id.Content, details);
                fragmentNotification.Add(Resource.Id.BasePanelLayoutContentFrameOne, details);
                fragmentNotification.Commit();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_clear_white_24dp);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        #region MENU

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case ResourceConstants.Home:
                    Finish();

                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}