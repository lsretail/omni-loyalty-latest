using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Items
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class ItemGroupsActivity : LoyaltyFragmentActivity
    {
        private bool isBigScreen;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            SetContentView(Resource.Layout.BaseSinglePanelLayout);

            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var category = AppData.ItemCategories.FirstOrDefault(x => x.Id == Intent.Extras.GetString(BundleConstants.ItemCategoryId));

            Title = category.Description;

            var details = ItemGroupsFragment.NewInstance(); // Details

            details.Arguments = Intent.Extras;

            //todo fragment
            var itemGroupTransaction = SupportFragmentManager.BeginTransaction();
            itemGroupTransaction.Replace(Resource.Id.BasePanelLayoutContentFrameOne, details);
            itemGroupTransaction.Commit();
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        #region MENU

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);

            MenuInflater.Inflate(Resource.Menu.OpenSearchMenu, menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewSearch:

                    var intent = new Intent();
                    intent.SetClass(this, typeof(ItemSearchActivity));

                    StartActivity(intent);

                    return true;

                case ResourceConstants.Home:
                    var upIntent = new Intent();
                    upIntent.SetClass(this, typeof(HomeActivity));
                    upIntent.AddFlags(ActivityFlags.ClearTop);
                    upIntent.AddFlags(ActivityFlags.SingleTop);
                    upIntent.PutExtra(BundleConstants.ChosenMenuBundleName, LoyaltyFragmentActivity.ActivityTypes.Items);

                    StartActivity(upIntent);

                    Finish();

                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}