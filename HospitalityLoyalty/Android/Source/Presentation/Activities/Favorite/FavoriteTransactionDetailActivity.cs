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
using Presentation.Utils;

namespace Presentation.Activities.Favorite
{
    [Activity(Label = "FavoriteTransactionDetailActivity", Theme = "@style/BaseThemeNoActionBar")]
    public class FavoriteTransactionDetailActivity : HospActivityNoStatusBar
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;

            ActivityType = ActivityTypes.Favorite;

            base.OnCreate(bundle);

            var fragment = new FavoriteTransactionDetailFragment();
            fragment.Arguments = Intent.Extras;

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.BaseActivityScreenContentFrame, fragment);
            ft.Commit();

            Title = string.Empty;
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
                    upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Favorites);

                    StartActivity(upIntent);

                    Finish();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}