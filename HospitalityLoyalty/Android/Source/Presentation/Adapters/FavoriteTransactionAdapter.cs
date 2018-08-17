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
using Domain.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Adapters
{
    class FavoriteTransactionAdapter : BaseRecyclerAdapter
    {
        private Context context;

        private ImageModel imageModel;

        private readonly IItemClickListener listener;
        private List<Transaction> transactions;
        private ImageSize imageSize;

        public FavoriteTransactionAdapter(Context context, IItemClickListener listener, int columns)
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
            get { return transactions.Count; }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var favoriteTransactionViewHolder = viewHolder as FavoriteTransactionViewHolder;
            var transaction = transactions[position];

            favoriteTransactionViewHolder.Title.Text = transaction.Name;
            favoriteTransactionViewHolder.SubTitle.Text = string.Format(context.Resources.GetString(Resource.String.FavoriteTransactionItems), transaction.SaleLines.Count); ;
            favoriteTransactionViewHolder.Price.Text = transaction.Amount;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.FavoriteTransactionCardItem, parent);

            var vh = new FavoriteTransactionViewHolder(view, (pos) =>
            {
                var transaction = transactions[pos];
                
                listener.ItemClicked(ItemType.Transaction, transaction.Id, "", 0, view);
            });

            return vh;
        }

        private class FavoriteTransactionViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int> itemClicked;

            public TextView Title { get; set; }
            public TextView SubTitle { get; set; }
            public TextView Price { get; set; }

            public FavoriteTransactionViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public FavoriteTransactionViewHolder(View view, Action<int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.FavoriteTransactionListItemViewTitle);
                SubTitle = view.FindViewById<TextView>(Resource.Id.FavoriteTransactionListItemViewSubtitle);
                Price = view.FindViewById<TextView>(Resource.Id.FavoriteTransactionListItemViewPrice);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {

                itemClicked(AdapterPosition);
            }
        }

        /*public FavoriteTransactionAdapter(IItemClickListener listener, Context context) : base(listener, context)
        {
            LineTypes.Add(LineType.MediumWrappedCards);

            Items = new List<Card<Transaction>>();
        }

        public void SetFavoriteTransactions()
        {
            Items.Clear();

            foreach (var favorite in AppData.Favorites)
            {
                if (favorite is Transaction)
                {
                    var favoriteTransaction = favorite as Transaction;

                    Items.Add(new Card<Transaction>() { ContentType = Card<Transaction>.CardContentType.FavoriteTransaction, Item = favoriteTransaction });
                }
            }

            NotifyDataSetChanged();
        }

        public override void OnClick(View v)
        {
            var tag = (string)v.Tag;

            var row = GetRowFromTag(tag);
            var col = GetColFromTag(tag);

            var item = GetItem(row, col);

            Listener.ItemClicked(ItemType.Transaction, item.Item.Id, "", v);
        }

        public override int GetViewType(int position)
        {
            return LineTypes.IndexOf(LineType.MediumWrappedCards);
        }

        public override void FillCard(int row, int col, Card<Transaction>.CardType cardType, View view)
        {
            if (view == null)
                return;

            var card = GetItem(row, col);

            if (card == null)
            {
                view.Visibility = ViewStates.Invisible;

                view.SetOnClickListener(null);
            }
            else
            {
                view.Visibility = ViewStates.Visible;

                view.SetOnClickListener(this);

                view.Tag = GenerateTag(row, col);

                TextView title = null;
                TextView subTitle = null;
                TextView price = null;

                if (card.ContentType == Card<Transaction>.CardContentType.FavoriteTransaction)
                {
                    var transaction = card.Item;

                    title = view.FindViewById<TextView>(Resource.Id.CardFavoriteTransactionTitle);
                    subTitle = view.FindViewById<TextView>(Resource.Id.CardFavoriteTransactionSubTitle);
                    price = view.FindViewById<TextView>(Resource.Id.CardFavoriteTransactionPrice);

                    if(title != null)
                        title.Text = transaction.Date.Value.ToShortDateString();

                    if (subTitle != null)
                        subTitle.Text = string.Format(Context.Resources.GetString(Resource.String.FavoriteTransactionItems), transaction.SaleLines.Count);

                    if (price != null)
                        price.Text = transaction.Amount;
                }
            }
        }*/
    }
}