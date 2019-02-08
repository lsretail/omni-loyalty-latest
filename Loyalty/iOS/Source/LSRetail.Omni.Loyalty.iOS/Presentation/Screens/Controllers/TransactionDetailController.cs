using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation
{
    public class TransactionDetailController : UIViewController
    {
        public TransactionDetailView rootView;
        private SalesEntry transaction;

        public TransactionDetailController(SalesEntry transaction)
        {
            this.transaction = transaction;
            rootView = new TransactionDetailView();
            rootView.GetTransaction += GetTransaction;
            rootView.PushToItemDetail += PushToItemDetail;
            Title = LocalizationUtilities.LocalizedString("TransactionDetails_Transaction", "Transaction");
        }

        private void PushToItemDetail(SalesEntryLine line)
        {
            UINavigationController nc = this.NavigationController;
            ItemDetailsController itemDetailsController = new ItemDetailsController(new LoyItem(line.ItemId), line.VariantId, line.UomId);
            //nc.PopToRootViewController(false);
            nc.PushViewController(itemDetailsController, false);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

            GetTransaction();
            this.View = this.rootView;
        }

        private async void GetTransaction()
        {
            Utils.UI.ShowLoadingIndicator();
            SalesEntry trans = await new Models.TransactionModel().GetTransaction(this.transaction);
            if (trans != null)
            {
                GetTransactionSuccess(trans);
            }
            else
            {
                GetTransactionFailure();
            }
        }

        public void GetTransactionSuccess(SalesEntry transaction)
        {
            Utils.UI.HideLoadingIndicator();
            this.transaction = transaction;
            this.rootView.UpdateData(this.transaction);
            this.rootView.HideErrorGettingDataView();
        }

        private void GetTransactionFailure()
        {
            Utils.UI.HideLoadingIndicator();
            this.rootView.ShowErrorGettingDataView();
        }
    }
}
