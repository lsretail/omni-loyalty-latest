using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Utils;

namespace Presentation.Activities.Transaction
{
    [Activity(Label = "", Theme = "@style/BaseThemeNoActionBar")]
    public class TransactionDetailActivity : HospActivityNoStatusBar
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;

            ActivityType = ActivityTypes.Transactions;

            base.OnCreate(bundle);

            var fragment = new TransactionDetailFragment();
            fragment.Arguments = Intent.Extras;

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.BaseActivityScreenContentFrame, fragment);
            ft.Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    var upIntent = new Intent();
                    upIntent.SetClass(this, typeof(HomeActivity));
                    upIntent.AddFlags(ActivityFlags.ClearTop);
                    upIntent.AddFlags(ActivityFlags.SingleTop);
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Transactions);

                    StartActivity(upIntent);

                    Finish();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}