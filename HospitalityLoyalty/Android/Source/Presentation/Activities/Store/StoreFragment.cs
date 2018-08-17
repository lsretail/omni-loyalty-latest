using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.SlideControl;
using Presentation.Adapters;
using Presentation.Dialog;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;

namespace Presentation.Activities.Store
{
    public class StoreFragment : BaseFragment, IItemClickListener, IRefreshableActivity, View.IOnClickListener
    {
        private StoreModel model;
        private StoreAdapter adapter;
        private int columns;

        private RecyclerView headers;
        private View progressBar;
        private FloatingActionButton mapButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //HasOptionsMenu = true;

            columns = Resources.GetInteger(Resource.Integer.GridColumnCount);

            var view = Inflate(inflater, Resource.Layout.StoreScreen, null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.StoreScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            headers = view.FindViewById<RecyclerView>(Resource.Id.StoreScreenList);
            progressBar = view.FindViewById(Resource.Id.StoreScreenProgress);
            mapButton = view.FindViewById<FloatingActionButton>(Resource.Id.StoreScreenMap);

            adapter = new StoreAdapter(Activity, this, columns);

            var layoutManager = new GridLayoutManager(Activity, columns);//, StaggeredGridLayoutManager.Vertical);
            layoutManager.SetSpanSizeLookup(new GridSpanSizeLookup(adapter, columns));

            headers.SetLayoutManager(layoutManager);
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));

            headers.SetAdapter(adapter);

            model = new StoreModel(Activity, this);

            if (AppData.Stores != null && AppData.Stores.Count > 0)
            {
                LoadStores();
            }
            else
            {
                model.GetStores(LoadStores);
            }

            mapButton.SetOnClickListener(this);

            return view;
        }

        private void LoadStores()
        {
            adapter.SetStores(AppData.Stores);
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view = null, string animationImageId = "")
        {
            if (type == ItemType.Store)
            {
                var intent = new Intent(Activity, typeof(StoreDetailActivity));
                intent.PutExtra(BundleUtils.Id, id);
                intent.PutExtra(BundleUtils.Ids, AppData.Stores.Select(x => x.Id).ToArray());
                intent.PutExtra(BundleUtils.ViewType, (int)SlideControlFragment.ViewTypes.Store);
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.StoreListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
                //StartActivity(intent);
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.StoreScreenMap:
                    if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                    {
                        OpenMap();
                    }
                    else
                    {
                        RequestPermissions(new string[] {Manifest.Permission.AccessFineLocation }, 0);
                    }
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            foreach (var grantResult in grantResults)
            {
                if (grantResult != Permission.Granted)
                {
                    ShowPermissionError();
                    return;
                }
            }

            OpenMap();
        }

        private void ShowPermissionError()
        {
            var dialog = new WarningDialog(Activity, "");
            dialog.Message = Resources.GetString(Resource.String.StoreAppNeedsLocationPermission);
            dialog.SetPositiveButton(Resources.GetString(Android.Resource.String.Ok), () => { });
            dialog.SetNegativeButton(Resources.GetString(Resource.String.OpenSettings), () =>
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                var uri = Android.Net.Uri.FromParts("package", Activity.PackageName, null);
                intent.SetData(uri);
                StartActivityForResult(intent, 1);
            });
            dialog.Show();
        }

        private void OpenMap()
        {
            var intent = new Intent(Activity, typeof(StoreMapActivity));
            if (AppData.Stores != null)
                intent.PutExtra(BundleUtils.StoreIds, AppData.Stores.Select(x => x.Id).ToArray());

            StartActivity(intent);
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                progressBar.Visibility = ViewStates.Visible;
                headers.Visibility = ViewStates.Gone;
                mapButton.Visibility = ViewStates.Gone;
            }
            else
            {
                progressBar.Visibility = ViewStates.Gone;
                headers.Visibility = ViewStates.Visible;
                mapButton.Visibility = ViewStates.Visible;
            }
        }
    }
}