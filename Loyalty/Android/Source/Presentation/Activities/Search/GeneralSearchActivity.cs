using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Util;

namespace Presentation.Activities.Search
{
    [Activity(Label = "@string/ActionbarSearch", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class GeneralSearchActivity : LoyaltyFragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BaseSinglePanelLayout);
            
            if (savedInstanceState == null)
            {
                var details = new GeneralSearchFragment();
                details.Arguments = Intent.Extras;

                //todo fragment
                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(Resource.Id.BasePanelLayoutContentFrameOne, details);
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
                case ResourceConstants.Home:
                    OnBackPressed();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}