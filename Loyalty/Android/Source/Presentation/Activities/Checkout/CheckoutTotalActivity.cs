using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Util;

namespace Presentation.Activities.Checkout
{
    [Activity(Label = "@string/ActionbarCheckout")]
    public class CheckoutTotalActivity : LoyaltyFragmentActivity
    {
        private const string CheckoutTotalFragmentTag = "CheckoutTotalFragmentTag";

        protected override void OnCreate(Bundle onSavedInstance)
        {
            HasRightDrawer = false;

            base.OnCreate(onSavedInstance);

            if (onSavedInstance == null)
            {
                var details = CheckoutTotalFragment.NewInstance();
                details.Arguments = Intent.Extras;

                var ft = SupportFragmentManager.BeginTransaction();
                ft.Add(ContentId, details, CheckoutTotalFragmentTag);
                ft.Commit();
            }
        }

        public override void SetSupportActionBar(Android.Support.V7.Widget.Toolbar toolbar)
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
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}