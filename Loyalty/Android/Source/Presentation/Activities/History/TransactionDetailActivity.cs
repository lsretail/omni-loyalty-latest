using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Presentation.Activities.Base;
using Presentation.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.History
{
    [Activity(Label = "@string/ActionbarTransaction")]
    public class TransactionDetailActivity : LoyaltyFragmentActivity
    {
        private bool isBigScreen;

        protected override void OnCreate(Bundle bundle)
        {
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
                var details = TransactionDetailFragment.NewInstance(); // Details

                details.Arguments = Intent.Extras;

                //todo fragment
                var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                //fragmentTransaction.Add(Android.Resource.Id.Content, details);
                fragmentTransaction.Add(Resource.Id.BasePanelLayoutContentFrameOne, details);
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