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
using Presentation.Activities.Home;
using Presentation.Util;

namespace Presentation.Activities.Contact
{
    [Activity(Label = "@string/ActionbarContactUs")]
    public class ContactUsActivity : LoyaltyFragmentActivity
    {
        private const string ContactUsFragmentTag = "ContactUsFragmentTag";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (bundle == null)
            {
                var detail = ContactUsFragment.NewInstance();

                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(ContentId, detail, ContactUsFragmentTag);
                fragmentNotification.Commit();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case ResourceConstants.Home:
                    var upIntent = new Intent();
                    upIntent.SetClass(this, typeof(HomeActivity));
                    upIntent.AddFlags(ActivityFlags.ClearTop);
                    upIntent.AddFlags(ActivityFlags.SingleTop);
                    upIntent.PutExtra(BundleConstants.ChosenMenuBundleName, LoyaltyFragmentActivity.ActivityTypes.DefaultItem);

                    StartActivity(upIntent);

                    Finish();

                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}