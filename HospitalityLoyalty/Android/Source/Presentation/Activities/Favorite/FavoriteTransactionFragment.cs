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
using Presentation.Activities.Base;
using Presentation.Activities.Menu;
using Presentation.Adapters;
using Presentation.Utils;

namespace Presentation.Activities.Favorite
{
    public class FavoriteTransactionFragment : BaseFragment, IItemClickListener, IBroadcastObserver
    {
        private RecyclerView headers;
        private View emptyView;

        private FavoriteTransactionAdapter adapter;
        private int columns;

        private List<LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions.Transaction> Favorites
        {
            get
            {
                var items = new List<LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions.Transaction>();

                foreach (var favorite in AppData.Favorites)
                {
                    if (favorite is LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions.Transaction)
                    {
                        items.Add(favorite as LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions.Transaction);
                    }
                }

                return items;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            columns = Resources.GetInteger(Resource.Integer.StaggeredGridColumnCount);

            var view = Inflate(inflater, Resource.Layout.FavoriteTransactionScreen, null);

            headers = view.FindViewById<RecyclerView>(Resource.Id.FavoriteTransactionScreenList);

            adapter = new FavoriteTransactionAdapter(Activity, this, columns);

            var layoutManager = new StaggeredGridLayoutManager(columns, StaggeredGridLayoutManager.Vertical);

            headers.SetLayoutManager(layoutManager);
            headers.SetItemAnimator(new DefaultItemAnimator());
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));

            headers.SetAdapter(adapter);

            adapter.SetTransactions(Favorites);

            emptyView = view.FindViewById<View>(Resource.Id.FavoriteTransactionScreenEmptyView);

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
            if (type == ItemType.Transaction)
            {
                 var intent = new Intent();
                intent.SetClass(Activity, typeof(FavoriteTransactionDetailActivity));
                intent.PutExtra(BundleUtils.FavoriteId, id);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view, Resources.GetString(Resource.String.TransitionImage)) });
                ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }

            adapter.SetTransactions(Favorites);

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
                adapter.SetTransactions(Favorites);
                SetVisibilities();
            }
            else if (action == BroadcastUtils.FavoritesUpdatedInList)
            {
                SetVisibilities();
            }
        }
    }
}