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

namespace Presentation.Activities.Menu
{
    [Activity(Label = "", Theme = "@style/BaseThemeNoActionBar")]
    public class MenuNodeActivity : HospActivityNoStatusBar
    {
        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;

            ActivityType = ActivityTypes.Menu;

            base.OnCreate(bundle);

            Fragment fragment = null;

            fragment = new MenuNodeFragment();
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
                    if (Intent.Extras.ContainsKey(BundleUtils.MenuId) && Intent.Extras.ContainsKey(BundleUtils.NodeId))
                    {
                        var upIntent = new Intent();
                        upIntent.SetClass(this, typeof(HomeActivity));
                        upIntent.AddFlags(ActivityFlags.ClearTop);
                        upIntent.AddFlags(ActivityFlags.SingleTop);

                        var menuId = Intent.Extras.GetString(BundleUtils.MenuId);

                        upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.Menu);
                        upIntent.PutExtra(BundleUtils.MenuId, menuId);

                        if (Intent.Extras.ContainsKey(BundleUtils.ParentNodeId))
                        {
                            upIntent.PutExtra(BundleUtils.NodeId, Intent.Extras.GetString(BundleUtils.ParentNodeId));   
                        }

                        StartActivity(upIntent);

                        Finish();    
                    }
                    
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}