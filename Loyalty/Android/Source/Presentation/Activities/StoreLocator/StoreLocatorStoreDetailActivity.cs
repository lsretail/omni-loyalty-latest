using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.StoreLocator
{
    [Activity(Label = "")]
    public class StoreLocatorStoreDetailActivity : LoyaltyFragmentActivity
    {
        private string currentStoreId;
        private const string StoreFragmentTag = "StoreFragmentTag";
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BaseSinglePanelLayout);

            Bundle extras = Intent.Extras;
            var storeId = extras.GetString(BundleConstants.StoreId);

            if (currentStoreId != storeId)
            {
                currentStoreId = storeId;

                if (bundle == null)
                {
                    var details = StoreLocatorStoreDetailFragment.NewInstance();
                    details.Arguments.PutString(BundleConstants.StoreId, storeId);

                    //todo fragment
                    var fragmentNotification = SupportFragmentManager.BeginTransaction();
                    fragmentNotification.Add(Resource.Id.BasePanelLayoutContentFrameOne, details, StoreFragmentTag);
                    fragmentNotification.Commit();
                }
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_clear_white_24dp);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

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
    }
}