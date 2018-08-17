using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation.Adapters
{
    public class TransactionDetailAdapter : BaseRecyclerAdapter
    {
        private readonly IItemClickListener listener;
        private List<object> transactionLines = new List<object>();

        public TransactionDetailAdapter(Context context, IItemClickListener listener)
        {
            this.listener = listener;
        }

        public void SetTransaction(Context context, LoyTransaction transaction)
        {
            transactionLines.Clear();

            foreach (var headerLine in transaction.TransactionHeaders)
            {
                transactionLines.Add(headerLine);
            }

            transactionLines.Add(new ItemHeaderLine());

            foreach (var saleLine in transaction.SaleLines)
            {
                transactionLines.Add(saleLine);
            }
            
            //NET
            var netTotalItem = new TotalItem()
            {
                Description = context.GetString(Resource.String.TransactionDetailViewTotalWithoutVAT),
                Total = transaction.NetAmount
            };

            //TAX
            foreach (var taxLine in transaction.TaxLines)
            {
                netTotalItem.Description += System.Environment.NewLine + taxLine.TaxDesription;
                netTotalItem.Total += System.Environment.NewLine + taxLine.TaxAmount;
            }

            transactionLines.Add(netTotalItem);

            //TOTAL
            transactionLines.Add(new TotalItem()
            {
                Description = context.GetString(Resource.String.TransactionDetailViewTotal),
                Total = transaction.Amount
            });


            //DISCOUNT
            transactionLines.Add(new TotalItem()
            {
                Description = context.GetString(Resource.String.TransactionDetailViewDiscountTotal),
                Total = transaction.DiscountAmount
            });

            //TENDERS
            var tenderTotalItem = new TotalItem();
            foreach (var tenderLine in transaction.TenderLines)
            {
                tenderTotalItem.Description = tenderLine.Description + Environment.NewLine;
                tenderTotalItem.Total = tenderLine.Amount + Environment.NewLine;
            }

            tenderTotalItem.Description = tenderTotalItem.Description.TrimEnd(Environment.NewLine.ToCharArray());
            tenderTotalItem.Total = tenderTotalItem.Total.TrimEnd(Environment.NewLine.ToCharArray());

            transactionLines.Add(tenderTotalItem);

            foreach (var footerLine in transaction.TransactionFooters)
            {
                transactionLines.Add(footerLine);
            }

            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get
            {
                if (transactionLines == null)
                    return 0;
                return transactionLines.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
            var item = transactionLines[position];

            if (item is LoyTransactionHeader)
            {
                return 0;
            }

            if (item is ItemHeaderLine)
            {
                return 1;
            }

            if (item is LoySaleLine)
            {
                return 2;
            }

            if (item is TotalItem)
            {
                return 3;
            }

            if (item is LoyTransactionFooter)
            {
                return 4;
            }

            return -1;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            if (viewHolder is HeaderLineViewHolder)
            {
                var headerLineViewHolder = viewHolder as HeaderLineViewHolder;
                var headerLine = transactionLines[position] as LoyTransactionHeader;

                if (headerLineViewHolder == null || headerLine == null)
                {
                    return;
                }

                headerLineViewHolder.Title.Text = headerLine.HeaderDescription;
            }
            if (viewHolder is SaleLineViewHolder)
            {
                var saleLineViewHolder = viewHolder as SaleLineViewHolder;
                var saleLine = transactionLines[position] as LoySaleLine;

                if (saleLineViewHolder == null || saleLine == null)
                {
                    return;
                }

                saleLineViewHolder.Title.Text = saleLine.Item.Description;

                string qtyText = saleLine.Quantity.ToString();
                if (saleLine.Uom != null)
                    qtyText += " " + saleLine.Uom.ShortDescription;

                saleLineViewHolder.Qty.Text = qtyText;
                saleLineViewHolder.Price.Text = saleLine.Amount;

                

                if (saleLine.VariantReg != null)
                {
                    saleLineViewHolder.Variant.Visibility = ViewStates.Visible;
                    var variantText = string.Empty;

                    if (!string.IsNullOrWhiteSpace(saleLine.VariantReg.Dimension1))
                        variantText += saleLine.VariantReg.Dimension1;
                    if (!string.IsNullOrWhiteSpace(saleLine.VariantReg.Dimension2))
                        variantText += ", " + saleLine.VariantReg.Dimension2;
                    if (!string.IsNullOrWhiteSpace(saleLine.VariantReg.Dimension3))
                        variantText += ", " + saleLine.VariantReg.Dimension3;
                    if (!string.IsNullOrWhiteSpace(saleLine.VariantReg.Dimension4))
                        variantText += ", " + saleLine.VariantReg.Dimension4;
                    if (!string.IsNullOrWhiteSpace(saleLine.VariantReg.Dimension5))
                        variantText += ", " + saleLine.VariantReg.Dimension5;
                    if (!string.IsNullOrWhiteSpace(saleLine.VariantReg.Dimension6))
                        variantText += ", " + saleLine.VariantReg.Dimension6;

                    saleLineViewHolder.Variant.Text = variantText;
                }
                else
                {
                    saleLineViewHolder.Variant.Text = string.Empty;
                    saleLineViewHolder.Variant.Visibility = ViewStates.Gone;
                }
                if (saleLine.DiscountAmt > 0)
                {
                    saleLineViewHolder.Discount.Visibility = ViewStates.Visible;

                    var discountText = string.Empty;
                    var discountStrFormat = saleLineViewHolder.Discount.Context.GetString(Resource.String.TransactionDetailViewItemDiscount);
                    discountText += string.Format(discountStrFormat, saleLine.DiscountAmount);

                    saleLineViewHolder.Discount.Text = discountText.TrimEnd(System.Environment.NewLine.ToCharArray());
                }
                else
                {
                    saleLineViewHolder.Discount.Text = string.Empty;
                    saleLineViewHolder.Discount.Visibility = ViewStates.Gone;
                }
            }
            if (viewHolder is TotalItemViewHolder)
            {
                var totalItemViewHolder = viewHolder as TotalItemViewHolder;
                var totalItem = transactionLines[position] as TotalItem;

                if (totalItemViewHolder == null || totalItem == null)
                {
                    return;
                }

                totalItemViewHolder.Title.Text = totalItem.Description;
                totalItemViewHolder.Amount.Text = totalItem.Total;
            }
            if (viewHolder is FooterLineViewHolder)
            {
                var footerLineViewHolder = viewHolder as FooterLineViewHolder;
                var footerLine = transactionLines[position] as LoyTransactionFooter;

                if (footerLineViewHolder == null || footerLine == null)
                {
                    return;
                }

                footerLineViewHolder.Title.Text = footerLine.FooterDescription;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder vh = null;
            if (viewType == 0)
            {
                View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionDetailListHeaderItems, parent, false);

                vh = new HeaderLineViewHolder(view); 
            }
            else if (viewType == 1)
            {
                View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionDetailListItemHeader, parent, false);

                vh = new HeaderLineViewHolder(view); 
            }
            else if (viewType == 2)
            {
                View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionDetailListItem, parent, false);

                vh = new SaleLineViewHolder(view, (type, pos) =>
                {
                    var saleLine = transactionLines[pos] as LoySaleLine;

                    listener.ItemClicked((int)ItemClickType.ShoppingListLine, saleLine.Id, string.Empty, view);
                });    
            }
            else if (viewType == 3)
            {
                View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionDetailListTotalItem, parent, false);

                vh = new TotalItemViewHolder(view); 
            }
            else if (viewType == 4)
            {
                View view = Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.TransactionDetailListFooterItems, parent, false);

                vh = new FooterLineViewHolder(view); 
            }

            return vh;
        }

        public enum TransactionDetailItemType
        {
            Item = 0,
        }

        public class HeaderLineViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }

            public HeaderLineViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public HeaderLineViewHolder(View view)
                : base(view)
            {
                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailViewSubTitle);
            }
        }

        public class ItemHeaderLineViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }
            public TextView Qty { get; set; }
            public TextView Price { get; set; }

            public ItemHeaderLineViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public ItemHeaderLineViewHolder(View view)
                : base(view)
            {
                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailViewItemNameHeader);
                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailViewItemQtyHeader);
                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailViewItemAmountHeader);
            }
        }

        public class SaleLineViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<int, int> itemClicked;

            public TextView Title { get; set; }
            public TextView Qty { get; set; }
            public TextView Price { get; set; }
            public TextView Subtitle { get; set; }
            public TextView Discount { get; set; }
            public TextView Variant { get; set; }

            public SaleLineViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public SaleLineViewHolder(View view, Action<int, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;

                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewItemName);
                Qty = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewItemQty);
                Price = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewItemAmount);
                //Subtitle = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewDiscount);
                Discount = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewDiscounts);
                Variant = view.FindViewById<TextView>(Resource.Id.TransactionDetailListItemViewVariant);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    default:
                        itemClicked((int)TransactionDetailItemType.Item, AdapterPosition);
                        break;
                }
            }
        }

        public class TotalItemViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }
            public TextView Amount { get; set; }

            public TotalItemViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public TotalItemViewHolder(View view)
                : base(view)
            {
                Title = view.FindViewById<TextView>(Resource.Id.TransactionFooterViewHeader);
                Amount = view.FindViewById<TextView>(Resource.Id.TransactionFooterViewDetail);
            }
        }

        public class FooterLineViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }

            public FooterLineViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public FooterLineViewHolder(View view)
                : base(view)
            {
                Title = view.FindViewById<TextView>(Resource.Id.TransactionDetailViewFooter);
            }
        }

        private class ItemHeaderLine { }

        private class TotalItem
        {
            public string Description { get; set; }
            public string Total { get; set; }
        }

        public class TransactionDetailItemDecoration : RecyclerView.ItemDecoration
        {
            private int space;

            public TransactionDetailItemDecoration(Context context)
            {
                space = context.Resources.GetDimensionPixelSize(Resource.Dimension.OneDP);
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                if (parent.GetChildAdapterPosition(view) != 0)
                {
                    outRect.Bottom = space;
                }
            }
        }
    }
}