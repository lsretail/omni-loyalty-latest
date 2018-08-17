using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Presentation.Activities.Base;
using Presentation.Util;

namespace Presentation.Activities.Image
{
    [Activity(Label = "")]
    public class FullScreenImageActivity : LoyaltyFragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            LoyaltyFragment fragment;

            fragment = new FullScreenImageFragment();
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
                    OnBackPressed();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}