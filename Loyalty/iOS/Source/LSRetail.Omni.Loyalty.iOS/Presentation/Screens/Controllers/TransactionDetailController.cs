using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation
{
    public class TransactionDetailController : UIViewController
    {
        public TransactionDetailView rootView;
        private LoyTransaction transaction;

        public TransactionDetailController(LoyTransaction transaction)
        {
            this.transaction = transaction;
            rootView = new TransactionDetailView();
            rootView.GetTransaction += GetTransaction;
            rootView.PushToItemDetail += PushToItemDetail;
            Title = LocalizationUtilities.LocalizedString("TransactionDetails_Transaction", "Transaction");
        }

        private void PushToItemDetail(LoySaleLine line)
        {
            UINavigationController nc = this.NavigationController;
            ItemDetailsController itemDetailsController = new ItemDetailsController(line.Item, line.VariantReg.Id, line.Uom.Id);
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
            LoyTransaction trans = await new Models.TransactionModel().GetTransaction(this.transaction);
            if (trans != null)
            {
                GetTransactionSuccess(trans);
            }
            else
            {
                GetTransactionFailure();
            }
        }

        public void GetTransactionSuccess(LoyTransaction transaction)
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

