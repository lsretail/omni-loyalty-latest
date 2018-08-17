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
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Activities.Transaction
{
    public class TransactionFragment : BaseFragment, IItemClickListener, IRefreshableActivity, IBroadcastObserver
    {
        private TransactionModel model;

        private RecyclerView headers;
        private View emptyView;
        private View progress;

        private TransactionAdapter adapter;
        private int columns;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            columns = Resources.GetInteger(Resource.Integer.StaggeredGridColumnCount);

            var view = Inflate(inflater, Resource.Layout.TransactionScreen, null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.TransactionScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            headers = view.FindViewById<RecyclerView>(Resource.Id.TransactionScreenList);

            adapter = new TransactionAdapter(Activity, this, columns);

            var layoutManager = new StaggeredGridLayoutManager(columns, StaggeredGridLayoutManager.Vertical);

            headers.SetLayoutManager(layoutManager);
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));
            headers.SetItemAnimator(new DefaultItemAnimator());

            headers.SetAdapter(adapter);

            emptyView = view.FindViewById<View>(Resource.Id.TransactionScreenEmptyView);
            progress = view.FindViewById(Resource.Id.TransactionProgress);

            SetVisibilities();

            model = new TransactionModel(Activity, this);
            
            if (AppData.Transactions != null && AppData.Transactions.Count > 0)
            {
                LoadTransactions();
            }
            else
            {
                model.GetTransactions();
            }

            return view;
        }

        private void SetVisibilities()
        {
            if (AppData.Transactions.Count == 0)
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

        public override void OnResume()
        {
            base.OnResume();

            (Activity as HospActivity).AddObserver(this);
        }

        public override void OnPause()
        {
            (Activity as HospActivity).RemoveObserver(this);
            
            base.OnPause();
        }

        private void LoadTransactions()
        {
            var transactions = AppData.Transactions.OrderByDescending(transaction => transaction.Date).ToList();

            adapter.SetTransactions(transactions);

            adapter.NotifyDataSetChanged();
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            var intent = new Intent(Activity, typeof(TransactionDetailActivity));
            intent.PutExtra(BundleUtils.Id, id);

            //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view, Resources.GetString(Resource.String.TransitionImage)) });
            ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                headers.Visibility = ViewStates.Gone;
                progress.Visibility = ViewStates.Visible;
            }
            else
            {
                headers.Visibility = ViewStates.Visible;
                progress.Visibility = ViewStates.Gone;
            }
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.TransactionsUpdated)
            {
                LoadTransactions();
            }
        }
    }
}