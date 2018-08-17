using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Util;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace Presentation.Activities.StoreLocator
{
    [Activity(Label = "@string/ActionbarStores", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class StoreLocatorActivity : LoyaltyFragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			if (SupportActionBar != null)
			{
				SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			}
            if (savedInstanceState == null)
            {
                var details = new StoreLocatorFragment();
                details.Arguments = Intent.Extras;

                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(ContentId, details);
                fragmentNotification.Commit();
            }
        }

        #region MENU

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case ResourceConstants.Home:
                    OnBackPressed();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}