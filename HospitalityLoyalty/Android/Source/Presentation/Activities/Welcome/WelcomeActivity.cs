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

namespace Presentation.Activities.Welcome
{
    [Activity(Label = "@string/Welcome", Theme = "@style/BaseThemeNoActionBar")]
    public class WelcomeActivity : HospActivityNoStatusBar
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = false;
            LeftDrawer = false;

            base.OnCreate(bundle);

            DisableDrawer((int)GravityFlags.Start);

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.BaseActivityScreenContentFrame, new WelcomeFragment());
            ft.Commit();
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
        }
    }
}