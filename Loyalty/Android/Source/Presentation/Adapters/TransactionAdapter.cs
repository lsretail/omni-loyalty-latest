using System;
using System.Collections.Generic;

using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Presentation.Util;
using Object = Java.Lang.Object;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation.Adapters
{
    public class TransactionAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private List<LoyTransaction> transactions;

        public TransactionAdapter(Context context, IItemClickListener listener)
        {
            this.listener = listener;
        }

        public void SetTransactions(List<LoyTransaction> transactions)
        {
            this.transactions = transactions;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get
            {
                if (transactions == null)
                    return 0;
                return transactions.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var transactionViewHolder = viewHolder as TransactionViewHolder;
            var transaction = transactions[position];

            if (transactionViewHolder == null || transaction == null)
            {
                return;
            }

            if (transaction.Date.HasValue)
            {
                transactionViewHolder.Title.Text = transaction.Date.Value.ToString("f");
            }

            transactionViewHolder.Subtitle.Text = transaction.Store.Description;
            transactionViewHolder.Price.Text = transaction.Amount;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionHeaderListItem, parent, false);

            var vh = new TransactionViewHolder(view, (type, pos) =>
            {
                var transaction = transactions[pos];

                listener.ItemClicked((int)ItemClickType.ShoppingListLine, transaction.Id, string.Empty, view);
            });

            return vh;
        }

        public enum TransactionItemType
        {
            Item = 0,
        }

        public class TransactionViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int, int> itemClicked;

            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public TextView Price { get; set; }

            public TransactionViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public TransactionViewHolder(View view, Action<int, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.TransactionHeaderListItemViewTransactionTime);
                Subtitle = view.FindViewById<TextView>(Resource.Id.TransactionHeaderListItemViewTransactionStore);
                Price = view.FindViewById<TextView>(Resource.Id.TransactionHeaderListItemViewTransactionTotal);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    default:
                        itemClicked((int)TransactionItemType.Item, AdapterPosition);
                        break;
                }
            }
        }
    }
}