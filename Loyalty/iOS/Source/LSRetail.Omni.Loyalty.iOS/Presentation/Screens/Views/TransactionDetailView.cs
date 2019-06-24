using UIKit;
using Presentation.Screens;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace Presentation
{
    public class TransactionDetailView : BaseView
    {
        private UITableView transDetailsTableView;
        private ErrorGettingDataView errorGettingDataView;

        public delegate void GetTransactionEventHandler();
        public GetTransactionEventHandler GetTransaction;
        private TransactionDetailsTableSource tableSource;

        public delegate void PushToItemDetailEventHandler(SalesEntryLine line);
        public event PushToItemDetailEventHandler PushToItemDetail;

        public TransactionDetailView()
        {
            this.transDetailsTableView = new UITableView();
            this.transDetailsTableView.BackgroundColor = UIColor.White;
            this.transDetailsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.AddSubview(this.transDetailsTableView);
        }

        private void PushToItemDetail2(SalesEntryLine line)
        {
            PushToItemDetail?.Invoke(line);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.transDetailsTableView.Frame = new CGRect(
                0,
                0,
                this.Frame.Width,
                this.Frame.Height
            );
        }

        public void UpdateData(SalesEntry transaction)
        {
            if (tableSource == null)
            {
                tableSource = new TransactionDetailsTableSource(transaction);
                this.transDetailsTableView.Source = tableSource;
                tableSource.PushToItemDetail += PushToItemDetail2;
            }

            tableSource.SetData(transaction);
            this.transDetailsTableView.Source = tableSource;
            this.transDetailsTableView.ReloadData();//must reload
        }

        public void ShowErrorGettingDataView()
        {
            if (this.errorGettingDataView == null)
            {
                CGRect errorGettingDataViewFrame = new CGRect(
                    0,
                    this.TopLayoutGuideLength,
                    this.Frame.Width,
                    this.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength
                );
                this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame, () =>
                {
                    if (GetTransaction != null)
                    {
                        GetTransaction();
                    }
                });
                this.AddSubview(this.errorGettingDataView);
            }
            else
            {
                this.errorGettingDataView.Hidden = false;
            }
        }

        public void HideErrorGettingDataView()
        {
            if (this.errorGettingDataView != null)
                this.errorGettingDataView.Hidden = true;
        }
    }
}
