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
using Domain.Transactions;
using Presentation.Activities.Base;
using Presentation.Activities.Menu;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Activities.Transaction
{
    public class TransactionDetailFragment : BaseFragment, IItemClickListener, IRefreshableActivity, IBroadcastObserver
    {
        private BasketModel model;
        private TransactionModel transactionModel;
        private FavoriteModel favoriteModel;

        private RecyclerView headers;

        private LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions.Transaction transaction;
        //private ImageButton favoriteButton;

        private TransactionDetailAdapter adapter;
        private int columns;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            columns = Resources.GetInteger(Resource.Integer.StaggeredGridColumnCount);

            var view = Inflate(inflater, Resource.Layout.TransactionDetailScreen, null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.TransactionDetailScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            model = new BasketModel(Activity);
            transactionModel = new TransactionModel(Activity, this);
            favoriteModel = new FavoriteModel(Activity);

            var transactionId = Arguments.GetString(BundleUtils.Id);

            transaction = AppData.Transactions.FirstOrDefault(x => x.Id == transactionId);

            headers = view.FindViewById<RecyclerView>(Resource.Id.TransactionDetailScreenList);

            adapter = new TransactionDetailAdapter(Activity, this, columns);

            var layoutManager = new StaggeredGridLayoutManager(columns, StaggeredGridLayoutManager.Vertical);

            headers.SetLayoutManager(layoutManager);
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));
            headers.SetItemAnimator(new DefaultItemAnimator());

            headers.SetAdapter(adapter);

            HasOptionsMenu = true;

            LoadSaleLines();

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        private void LoadSaleLines()
        {
            adapter.SetTransaction(transaction);
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            if (type == ItemType.Transaction)
            {
                var saleLine = transaction.SaleLines.FirstOrDefault(x => x.Id == id);

                model.AddItemToBasket(saleLine.Item, saleLine.Quantity);
            }
            else if (type == ItemType.Transactions)
            {
                model.AddSaleLinesToBasket(transaction.SaleLines);
            }
            else if (type == ItemType.TransactionDetail)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(MenuItemActivity));
                intent.PutExtra(BundleUtils.TransactionId, transaction.Id);
                intent.PutExtra(BundleUtils.SaleLineId, id); 
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);
                
                ActivityUtils.StartActivityForResultWithAnimation(Activity, HospActivity.OpenMenuItemRequestCode, intent, view);
            }
        }

        public void ShowIndicator(bool show)
        {
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            if (transaction.Date.Value.AddDays(2) > DateTime.Now)
            {
                inflater.Inflate(Resource.Menu.TransactionDetailMenu, menu);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuItemQrCode:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof (Checkout.CheckoutActivity));
                    intent.PutExtra(BundleUtils.TransactionId, transaction.Id);
                    StartActivity(intent);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.FavoritesUpdated || action == BroadcastUtils.FavoritesUpdatedInList)
            {
                adapter.NotifyDataSetChanged();
            }
        }
    }
}