using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;
using CoreAnimation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace Presentation
{
    public class HistoryView : BaseView
    {
        private UITableView transactionTableView;
        private UIRefreshControl refreshControl;
        private bool allowPullToRefresh = true;
        private UIView noDataView;
        private UILabel noDataText;

        private const float labelHeight = 20;

        public delegate void RefreshEventHandler();
        public event RefreshEventHandler refresh;

        public delegate void TransactionSelectedEventHandler(SalesEntry transaction);
        public event TransactionSelectedEventHandler transactionSelected;

        public HistoryView()
        {
            this.BackgroundColor = UIColor.White;

            this.transactionTableView = new UITableView();
            this.transactionTableView.Source = new TransactionHistoryTableSource();
            this.transactionTableView.BackgroundColor = AppColors.BackgroundGray;
            this.transactionTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            (this.transactionTableView.Source as TransactionHistoryTableSource).transactionSelected += (SalesEntry transaction) =>
            {
                if (transactionSelected != null)
                {
                    transactionSelected(transaction);
                }
            };
            this.AddSubview(transactionTableView);

            this.noDataView = new UIView();
            this.noDataView.BackgroundColor = UIColor.Clear;
            this.noDataView.Hidden = true;

            this.noDataText = new UILabel();
            this.noDataText.TextColor = UIColor.Gray;
            this.noDataText.TextAlignment = UITextAlignment.Center;
            this.noDataText.Font = UIFont.SystemFontOfSize(14);

            this.AddSubview(this.noDataView);
            this.noDataView.AddSubview(this.noDataText);

            this.refreshControl = new UIRefreshControl();
            this.refreshControl.ValueChanged += (object sender, EventArgs e) =>
            {
                if (refresh != null)
                    refresh();
            };
            if (this.allowPullToRefresh)
                this.transactionTableView.AddSubview(refreshControl);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.transactionTableView.Frame = new CoreGraphics.CGRect(
                0,
                0,
                this.Frame.Width,
                this.Frame.Height
            );

            this.noDataView.Frame = new CGRect(
                0,
                0,
                this.Frame.Width,
                this.Frame.Height
            );

            this.noDataText.Frame = new CGRect(
                0,
                this.noDataView.Frame.Height / 2 - labelHeight / 2,
                this.noDataView.Frame.Width,
                labelHeight
            );
        }

        public void RefreshDataSuccess()
        {
            System.Diagnostics.Debug.WriteLine("HistoryScreen.RefreshData success");
            this.refreshControl.EndRefreshing();
            RefreshWithAnimation();
        }

        public void RefreshDataFailure()
        {
            System.Diagnostics.Debug.WriteLine("HistoryScreen.RefreshData failure");
            this.refreshControl.EndRefreshing();
            RefreshNoDataView();
        }


        public void RefreshNoDataView()
        {
            if (this.transactionTableView.Source == null)
                return;

            if (!AppData.UserLoggedIn)
                ShowNoDataView(LocalizationUtilities.LocalizedString("History_NoData", "No previous transactions available"));
            else if (!(this.transactionTableView.Source as TransactionHistoryTableSource).HasData)
                ShowNoDataView(LocalizationUtilities.LocalizedString("History_NoData", "No previous transactions available"));
            else
                HideNoDataView();
        }

        #region Private Functions

        private void ShowNoDataView(string displayText)
        {
            this.noDataText.Text = displayText;
            this.noDataView.Hidden = false;
            this.noDataText.Hidden = false;
            this.transactionTableView.Hidden = true;
        }

        private void HideNoDataView()
        {
            this.noDataView.Hidden = true;
            this.noDataText.Hidden = true;
            this.transactionTableView.Hidden = false;
        }

        private void RefreshWithAnimation()
        {
            if (this.transactionTableView.Source == null)
                return;

            CATransition transition = new CATransition();
            transition.Duration = 0.3;
            transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
            transition.Type = CATransition.TransitionPush;
            transition.Subtype = CATransition.TransitionFade;
            transition.FillMode = CAFillMode.Both;

            this.transactionTableView.Layer.AddAnimation(transition, null);

            (this.transactionTableView.Source as TransactionHistoryTableSource).RefreshData();
            this.transactionTableView.ReloadData();

            RefreshNoDataView();
        }

        #endregion
    }
}
