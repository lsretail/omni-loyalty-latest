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

namespace Presentation.Activities.Contact
{
    [Activity(Label = "@string/ActionBarChangePassword", Theme = "@style/BaseThemeNoActionBar")]
    public class ChangePasswordActivity : HospActivityNoStatusBar
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;

            ActivityType = ActivityTypes.None;

            base.OnCreate(bundle);

            if (bundle == null)
            {
                var fragment = new ChangePasswordFragment();
                fragment.Arguments = Intent.Extras;

                var ft = SupportFragmentManager.BeginTransaction();
                ft.Replace(Resource.Id.BaseActivityScreenContentFrame, fragment);
                ft.Commit();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}