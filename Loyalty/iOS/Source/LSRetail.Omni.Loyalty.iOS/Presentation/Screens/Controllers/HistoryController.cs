using UIKit;
using Presentation.Utils;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace Presentation
{
    public class HistoryController : UIViewController
    {
        private HistoryView rootView;

        public HistoryController()
        {
            this.Title = LocalizationUtilities.LocalizedString("History_History", "History");
            this.rootView = new HistoryView();
            this.rootView.transactionSelected += TransactionSelected;
            this.rootView.refresh += refresh;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UI.StyleNavigationBar(this.NavigationController.NavigationBar);
            SetRightBarButtonItems();

            this.View = this.rootView;
        }

        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (AppData.UserLoggedIn)
            {
                bool success = await new Models.TransactionModel().GetTransactionHeaders(AppData.Device.UserLoggedOnToDevice.Id);
                if (success)
                    RefreshDataSuccess();
                else
                    RefreshDataFailure();
            }
            else
            {
                this.rootView.RefreshNoDataView();
            }
        }

        private async void refresh()
        {
            if (AppData.UserLoggedIn)
            {
                System.Diagnostics.Debug.WriteLine("Refreshing transactions ...");
                bool success = await new Models.TransactionModel().GetTransactionHeaders(AppData.Device.UserLoggedOnToDevice.Id);

                if (success)
                {
                    RefreshDataSuccess();
                }
                else
                {
                    RefreshDataFailure();
                }
            }
            else
            {
                RefreshDataFailure();
            }
        }

        private void RefreshDataSuccess()
        {
            this.rootView.RefreshDataSuccess();
        }

        private void RefreshDataFailure()
        {
            this.rootView.RefreshDataFailure();
        }

        private void SetRightBarButtonItems()
        {
            List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();
            this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
        }

        private void TransactionSelected(SalesEntry transaction)
        {
            // TODO: Not display platforms that are not mobile?
            TransactionDetailController transactionDetailController = new TransactionDetailController(transaction);
            this.NavigationController.PushViewController(transactionDetailController, true);
        }
    }
}
