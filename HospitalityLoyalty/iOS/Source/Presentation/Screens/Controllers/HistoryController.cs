using System;
using System.Linq;
using Domain.Transactions;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class HistoryController : UIViewController, HistoryView.IHistoryListeners
	{
		private HistoryView rootView;

		public HistoryController()
		{
			Title = LocalizationUtilities.LocalizedString("History_History", "History");
			rootView = new HistoryView(this);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			ReloadData();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.rootView.TopLayoutGuideLength = TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = BottomLayoutGuide.Length;

			View = rootView;
		}

		public void ReloadData()
		{
			rootView.UpdateData(AppData.Transactions.OrderByDescending(x => x.Date).ToList());
		}

		public void TransactionSelected(int index)
		{
			Transaction transaction = AppData.Transactions.OrderByDescending(x => x.Date).ToList()[index];
			TransactionDetailController transactionDetailController = new TransactionDetailController(transaction);
			this.NavigationController.PushViewController(transactionDetailController, true);
		}
	}
}

