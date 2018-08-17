using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Util;
using ImageView = Android.Widget.ImageView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Items
{
    [Activity(Label = "@string/ActionbarSearch")]
    public class ItemSearchActivity : LoyaltyFragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (savedInstanceState == null)
            {
                var searchFragment = new ItemSearchFragment();
                searchFragment.Arguments = Intent.Extras;

                var ft = SupportFragmentManager.BeginTransaction();
                ft.Replace(ContentId, searchFragment);
                ft.Commit();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
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