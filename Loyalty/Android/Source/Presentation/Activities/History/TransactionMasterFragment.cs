using System;
using System.Collections.Generic;

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
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation.Activities.History
{
    public class TransactionMasterFragment : LoyaltyFragment, IRefreshableActivity, SwipeRefreshLayout.IOnRefreshListener
    {
        public IItemClickListener ItemClickListener { get; set; }

        private RecyclerView transactionList;
        private SwipeRefreshLayout refreshLayout;
        private TransactionAdapter adapter;

        private TransactionModel model;
        private bool isBigScreen;

        public static TransactionMasterFragment NewInstance()
        {
            var transactionFragment = new TransactionMasterFragment() { Arguments = new Bundle() };
            return transactionFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);
            
            HasOptionsMenu = true;

            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Transaction);

            model = new TransactionModel(Activity, this);

            transactionList = view.FindViewById<RecyclerView>(Resource.Id.TransactionViewTransactionList);
            transactionList.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
            transactionList.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            transactionList.HasFixedSize = true;

            adapter = new TransactionAdapter(Activity, ItemClickListener);
            adapter.SetTransactions(AppData.Device.UserLoggedOnToDevice.TransactionOrderedByDate);

            transactionList.SetAdapter(adapter);

            refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.TransactionViewRefreshLayout);

            refreshLayout.SetColorSchemeResources(Resource.Color.transactions, Resource.Color.accent, Resource.Color.transactions, Resource.Color.accent);
            refreshLayout.SetOnRefreshListener(this);

            return view;
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

        public override void OnDestroyView()
        {
            if(model != null)
                model.Stop();

            base.OnDestroyView();
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
                AppData.Device.UserLoggedOnToDevice.Transactions = loadedTransactions;
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
                    intent.SetClass(Activity, typeof (GeneralSearchActivity));
                    intent.PutExtra(BundleConstants.SearchType, (int) SearchType.Transaction);

                    StartActivity(intent);

                    return true;
            }

            return false;
        }
    }
}