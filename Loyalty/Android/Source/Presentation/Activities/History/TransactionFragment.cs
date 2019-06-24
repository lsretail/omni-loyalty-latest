using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Activities.Search;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.History
{
    public class TransactionFragment : CardListFragment, IRefreshableActivity, IItemClickListener, SwipeRefreshLayout.IOnRefreshListener
    {
        private RecyclerView transactionList;
        private SwipeRefreshLayout refreshLayout;
        private TransactionAdapter adapter;

        private TransactionModel model;

        public static TransactionMasterFragment NewInstance()
        {
            var transactionFragment = new TransactionMasterFragment() { Arguments = new Bundle() };
            return transactionFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            HasOptionsMenu = true;

            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            var view = base.OnCreateView(inflater, container, bundle);
            refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.TransactionViewRefreshLayout);
            transactionList = view.FindViewById<RecyclerView>(Resource.Id.TransactionViewTransactionList);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.TransactionScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            model = new TransactionModel(Activity, this);

            transactionList.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            transactionList.HasFixedSize = true;

            adapter = new TransactionAdapter(Activity, this);

            SetLayoutManager();

            transactionList.SetAdapter(adapter);

            refreshLayout.SetColorSchemeResources(Resource.Color.transactions, Resource.Color.accent, Resource.Color.transactions, Resource.Color.accent);
            refreshLayout.SetOnRefreshListener(this);

            if (AppData.Device.UserLoggedOnToDevice.SalesEntries?.Count > 0)
            {
                adapter.SetTransactions(AppData.Device.UserLoggedOnToDevice.TransactionOrderedByDate);
            }
            else
            {
                OnRefresh();
            }

            return view;
        }

        public void SetLayoutManager()
        {
            var manager = new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.StackedCardColumns), LinearLayoutManager.Vertical, false);
            manager.SetSpanSizeLookup(new BaseRecyclerAdapter.GridSpanSizeLookup(Activity, adapter));
            transactionList.SetLayoutManager(manager);
        }

        public override View CreateView(LayoutInflater inflater)
        {
            return Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Transaction);
        }

        public override void OnSaveInstanceState(Bundle bundle)
        {
            base.OnSaveInstanceState(bundle);
        }

        public override void OnResume()
        {
            base.OnResume();

            UpdateTransaction();
        }

        public void UpdateTransaction()
        {
            adapter.SetTransactions(AppData.Device.UserLoggedOnToDevice.TransactionOrderedByDate);
        }

        public void ShowIndicator(bool show)
        {
            refreshLayout.Refreshing = show;
        }

        public async void OnRefresh()
        {
            var loadedTransactions = await model.GetTransactionsByContactId(AppData.Device.UserLoggedOnToDevice.Id);

            if (loadedTransactions != null)
            {
                AppData.Device.UserLoggedOnToDevice.SalesEntries = loadedTransactions;
                UpdateTransaction();
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            inflater.Inflate(Resource.Menu.GenericSearchMenu, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewSearch:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(GeneralSearchActivity));
                    intent.PutExtra(BundleConstants.SearchType, (int)SearchType.SalesEntry);

                    StartActivity(intent);

                    return true;
            }

            return false;
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var transaction = AppData.Device.UserLoggedOnToDevice.SalesEntries.FirstOrDefault(x => x.Id == id);

            var intent = new Intent();

            intent.PutExtra(BundleConstants.TransactionId, transaction.Id);

            intent.SetClass(Activity, typeof(TransactionDetailActivity));
            StartActivity(intent);
        }
    }
}