using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Presentation.Activities.Base;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Account
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation)]
    public class ChangePasswordActivity : LoyaltyFragmentActivity
    {
        private bool isBigScreen;
        private const string ChangePasswordFragmentTag = "ChangePasswordFragment";

        protected override void OnCreate(Bundle bundle)
        {
            HasMenu = false;
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
                var details = ChangePasswordFragment.NewInstance();

                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(Resource.Id.BasePanelLayoutContentFrameOne, details, ChangePasswordFragmentTag);
                fragmentNotification.Commit();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Canceled);
                    Finish();
                    return true;
            }

            return false;
        }
    }
}