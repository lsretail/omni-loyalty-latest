using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Presentation.Activities.Base;

namespace Presentation.Activities.Login
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation|ConfigChanges.ScreenSize)]
    public class LoginActivity : LoyaltyFragmentActivity
    {
        private const string LoginFragmentTag = "LoginFragmentTag";
        private bool isBigScreen;

        protected override void OnCreate(Bundle bundle)
        {
            HasMenu = false;
            HasLeftDrawer = false;
            HasRightDrawer = false;
            HasSocialMediaConnection = true;

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

            StartSocialMediaOnResume = false;

            if (bundle == null)
            {
                var details = LoginFragment.NewInstance();
                details.Arguments = Intent.Extras;

                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(Resource.Id.BasePanelLayoutContentFrameOne, details, LoginFragmentTag);
                fragmentNotification.Commit();
            }
        }
    }
}