using System;

using Android.App;
using Android.OS;
using Presentation.Activities.Base;

namespace Presentation.Activities.Welcome
{
    [Activity()]
    public class WelcomeActivity : LoyaltyFragmentActivity
    {
        private bool isBigScreen;

        protected override void OnCreate(Bundle bundle)
        {
            HasMenu = false;
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
                var details = WelcomeFragment.NewInstance();

                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(Resource.Id.BasePanelLayoutContentFrameOne, details);
                fragmentNotification.Commit();
            }
        }
    }
}