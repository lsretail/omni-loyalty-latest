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

namespace Presentation.Activities.Checkout
{
    [Activity(Label = "", Theme = "@style/BaseThemeNoActionBar")]
    public class CheckoutActivity : HospActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = false;

            ActivityType = ActivityTypes.None;

            base.OnCreate(bundle);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            var fragment = new CheckoutFragment();
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