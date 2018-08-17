using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using Presentation.Activities.Base;
using Presentation.Activities.Menu;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Activities.Favorite
{
    public class FavoriteItemFragment : BaseFragment, IItemClickListener, IBroadcastObserver
    {
        private const int OpenMenuItem = 101;

        private RecyclerView headers;
        private View emptyView;

        private FavoriteItemAdapter adapter;
        private int columns;

        private List<MenuItem> Favorites
        {
            get
            {
                var items = new List<MenuItem>();

                foreach (var favorite in AppData.Favorites)
                {
                    if (favorite is MenuItem)
                    {
                        items.Add(favorite as MenuItem);
                    }
                }

                return items;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            columns = Resources.GetInteger(Resource.Integer.StaggeredGridColumnCount);

            var view = Inflate(inflater, Resource.Layout.FavoriteItemScreen, null);

            headers = view.FindViewById<RecyclerView>(Resource.Id.FavoriteItemScreenList);

            adapter = new FavoriteItemAdapter(Activity, this, columns);

            var layoutManager = new StaggeredGridLayoutManager(columns, StaggeredGridLayoutManager.Vertical);

            headers.SetLayoutManager(layoutManager);
            headers.SetItemAnimator(new DefaultItemAnimator());
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));
            headers.SetAdapter(adapter);

            adapter.SetMenuItems(Favorites);

            emptyView = view.FindViewById<View>(Resource.Id.FavoriteItemScreenEmptyView);

            SetVisibilities();

            return view;
        }

        private void SetVisibilities()
        {
            if (Favorites.Count == 0)
            {
                //headers.Visibility = ViewStates.Gone;
                emptyView.Visibility = ViewStates.Visible;
            }
            else
            {
                //headers.Visibility = ViewStates.Visible;
                emptyView.Visibility = ViewStates.Gone;
            }
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            if (type == ItemType.Item)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(MenuItemActivity));
                intent.PutExtra(BundleUtils.ItemId, id);
                intent.PutExtra(BundleUtils.MenuId, id2);
                intent.PutExtra(BundleUtils.Type, itemType);
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                ActivityUtils.StartActivityForResultWithAnimation(Activity, HospActivity.OpenMenuItemRequestCode, intent, view);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }

            adapter.SetMenuItems(Favorites);
            SetVisibilities();
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.FavoritesUpdated)
            {
                adapter.SetMenuItems(Favorites);
                SetVisibilities();
            }
        }
    }
}