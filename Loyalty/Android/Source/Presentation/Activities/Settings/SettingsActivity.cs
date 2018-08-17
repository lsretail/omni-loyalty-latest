using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Util;

namespace Presentation.Activities.Settings
{
    [Activity(Label = "@string/ActionbarSettings", LaunchMode = LaunchMode.SingleTask)]
    public class SettingsActivity : LoyaltyFragmentActivity
    {
        private bool isBigScreen;

        protected override void OnCreate(Bundle bundle)
        {
            HasLeftDrawer = false;
            HasRightDrawer = false;

            base.OnCreate(bundle);

            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            if (isBigScreen)
            {
                SetContentView(Resource.Layout.BaseBigScreenSinglePaneLayout);
            }
            else
            {
                SetContentView(Resource.Layout.BaseSinglePanelLayout);
            }

            if (bundle == null)
            {
                var details = SettingsFragment.NewInstance();
                details.Arguments = Intent.Extras;

                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(Resource.Id.BasePanelLayoutContentFrameOne, details);
                fragmentNotification.Commit();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:

                    Finish();

                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}