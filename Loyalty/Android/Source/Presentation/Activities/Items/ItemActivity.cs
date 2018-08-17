using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Presentation.Activities.Base;
using Presentation.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Items
{
    [Activity(Label = "")]
    public class ItemActivity : LoyaltyFragmentActivity// BaseSocialMediaActivity
    {
        private const string ItemFragmentTag = "ItemDetailFragmentTag";

        protected override void OnCreate(Bundle bundle)
        {
            HasSocialMediaConnection = true;

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BaseSinglePanelLayout);

            if (bundle == null)
            {
                var details = ItemFragment.NewInstance(); // Details         

                details.Arguments = Intent.Extras;

                var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.BasePanelLayoutContentFrameOne, details, ItemFragmentTag);
                fragmentTransaction.Commit();
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

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case ResourceConstants.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(menuItem);
        }

        #endregion
    }
}