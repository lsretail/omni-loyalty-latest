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
using Presentation.Activities.SlideControl;
using Presentation.Utils;

using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Offer
{
    [Activity(Label = "", Theme = "@style/BaseDetailTheme")]
    public class OfferDetailActivity : HospActivityNoStatusBar
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;

            ActivityType = ActivityTypes.OfferAndCoupons;

            base.OnCreate(bundle);

            Fragment  fragment = new OfferDetailFragment();
            fragment.Arguments = Intent.Extras;

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.BaseActivityScreenContentFrame, fragment);
            ft.Commit();
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_clear_white_24dp);
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