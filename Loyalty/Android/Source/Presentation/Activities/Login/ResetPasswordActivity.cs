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
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Login
{
    [Activity(Label = "@string/ActionBarResetPassword")]
    public class ResetPasswordActivity : LoyaltyFragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            HasRightDrawer = false;
            HasLeftDrawer = false;

            base.OnCreate(bundle);

            var fragment = new ResetPasswordFrament();
            fragment.Arguments = Intent.Extras;

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.BaseActivityScreenContentFrame, fragment);
            ft.Commit();
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
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