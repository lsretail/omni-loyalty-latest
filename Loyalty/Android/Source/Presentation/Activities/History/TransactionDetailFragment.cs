using System;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Items;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation.Activities.History
{
    public class TransactionDetailFragment : LoyaltyFragment, IRefreshableActivity, IItemClickListener
    {
        private string transactionId;
        private LoyTransaction transaction;
        private TransactionModel model;

        private RecyclerView transactionDetailRecyclerView;
        private View loadingView;
        private View content;
        private ViewSwitcher switcher;

        private TransactionDetailAdapter adapter;

        public static TransactionDetailFragment NewInstance()
        {
            var transactionDetail = new TransactionDetailFragment() { Arguments = new Bundle() };
            return transactionDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            //progressDialog = new CustomProgressDialog(Activity);
            
            Bundle data = Arguments;
            transactionId = data.GetString(BundleConstants.TransactionId);

            model = new TransactionModel(Activity, this);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.TransactionDetail, null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.TransactionDetailScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.TransactionDetailViewSwitcher);
            content = view.FindViewById(Resource.Id.TransactionDetailViewContent);
            loadingView = view.FindViewById(Resource.Id.TransactionDetailViewLoadingSpinner);

            transactionDetailRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.TransactionDetailViewTransactionDetailList);
            transactionDetailRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
            transactionDetailRecyclerView.AddItemDecoration(new TransactionDetailAdapter.TransactionDetailItemDecoration(Activity));
            transactionDetailRecyclerView.HasFixedSize = true;

            adapter = new TransactionDetailAdapter(Activity, this);
            transactionDetailRecyclerView.SetAdapter(adapter);

            transaction = AppData.Device.UserLoggedOnToDevice.Transactions.FirstOrDefault(x => x.Id == transactionId);

            LoadTransaction();

            return view;
        }

        private async void LoadTransaction()
        {
            var loadedTransation = await model.GetTransactionByReceiptNo(transaction.ReceiptNumber);

            if (loadedTransation == null)
            {
                Activity.OnBackPressed();
            }
            else
            {
                this.transaction = loadedTransation;
                ShowDetails();
            }
        }

        public override void OnDestroyView()
        {
            if(model != null)
                model.Stop();

            base.OnDestroyView();
        }

        private void ShowLoading()
        {
            if(switcher.CurrentView != loadingView)
                switcher.ShowPrevious();
        }

        private void ShowContent()
        {
            if (switcher.CurrentView != content)
                switcher.ShowNext();
        }

        private void ShowDetails()
        {
            var dateHeader = View.FindViewById<TextView>(Resource.Id.TransactionDetailViewDateHeader);
            if (dateHeader == null)
            {
                dateHeader.Visibility = ViewStates.Gone;
            }
            else
            {
                dateHeader.Text = transaction.Date.Value.ToString("D");
            }

            adapter.SetTransaction(Activity, transaction);

            Crossfade();
        }

        private void Crossfade()
        {
            ShowContent();
        }

        public void ShowIndicator(bool show)
        {
            if(show)
                ShowLoading();
            else
                ShowContent();
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var intent = new Intent();

            var saleLine = transaction.SaleLines.FirstOrDefault(x => x.Id == id);

            if (saleLine != null && saleLine.Item != null)
            {
                intent.PutExtra(BundleConstants.ItemId, saleLine.Item.Id);

                if (AppData.IsDualScreen)
                    intent.PutExtra(BundleConstants.LoadContainer, true);

                intent.SetClass(Activity, typeof(ItemActivity));
                StartActivity(intent);
            }
        }
    }
}