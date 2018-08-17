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

namespace Presentation.Activities.Image
{
    [Activity(Label = "", Theme = "@style/BaseThemeNoActionBar")]
    public class FullScreenImageActivity : HospActivityNoStatusBar
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;

            base.OnCreate(bundle);

            BaseFragment fragment;

            if (Intent.Extras.ContainsKey(BundleUtils.ImageId))
            {
                fragment = new FullScreenImageFragment();
                fragment.Arguments = Intent.Extras;
            }
            else
            {
                fragment = new FullScreenImagePagerFragment();
                fragment.Arguments = Intent.Extras;
            }

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