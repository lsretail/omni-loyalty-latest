using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Adapters
{
    public class TransactionAdapter : BaseRecyclerAdapter
    {
        private Context context;

        private ImageModel imageModel;

        private readonly IItemClickListener listener;
        private List<Transaction> transactions;
        private ImageSize imageSize;

        public TransactionAdapter(Context context, IItemClickListener listener, int columns)
        {
            this.context = context;
            this.listener = listener;
            imageModel = new ImageModel(context);

            var dimension = context.Resources.DisplayMetrics.WidthPixels / columns;
            imageSize = new ImageSize(dimension, dimension);
        }

        public void SetTransactions(List<Transaction> transactions)
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

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var favoriteTransactionViewHolder = viewHolder as TransactionViewHolder;
            var transaction = transactions[position];

            favoriteTransactionViewHolder.Title.Text = transaction.DateToFullShortFormat;
            favoriteTransactionViewHolder.SubTitle.Text = string.Format(context.Resources.GetString(Resource.String.FavoriteTransactionItems), transaction.SaleLines.Count); ;
            favoriteTransactionViewHolder.Price.Text = transaction.Amount;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionCardItem, parent);

            var vh = new TransactionViewHolder(view, (pos) =>
            {
                var transaction = transactions[pos];

                listener.ItemClicked(ItemType.Transaction, transaction.Id, "", 0, view);
            });

            return vh;
        }

        private class TransactionViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }
            public TextView Price { get; set; }

            public TransactionViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public TransactionViewHolder(View view, Action<int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.TransactionListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.TransactionListItemViewSubtitle);
                Price = view.FindViewById<TextView>(Resource.Id.TransactionListItemViewPrice);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {

                itemClicked(AdapterPosition);
            }
        }
    }
}