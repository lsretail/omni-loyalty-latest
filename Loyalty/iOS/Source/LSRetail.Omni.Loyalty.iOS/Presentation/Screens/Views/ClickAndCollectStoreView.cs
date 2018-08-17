using UIKit;
using Presentation.Screens;
using CoreGraphics;
using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation
{
    public class ClickAndCollectStoreView : BaseView
	{
		private UITableView clickAndCollectTableView;
		private ClickAndCollectStoreTableSource clickAndCollectTableViewSource;
		private ErrorGettingDataView errorGettingDataView;

		public delegate void GetDataEventHandler ();
		public event GetDataEventHandler GetData;

		public delegate void StoreInfoButtonPressedEventHandler (Store store);
		public event StoreInfoButtonPressedEventHandler StoreInfoButtonPressed;

		public delegate void StoreSelectedEventHandler (Store store);
		public event StoreSelectedEventHandler StoreSelected;

		public ClickAndCollectStoreView ()
		{
			this.clickAndCollectTableView = new UITableView();
			this.clickAndCollectTableView.Source = this.clickAndCollectTableViewSource;
			this.clickAndCollectTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.clickAndCollectTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			this.clickAndCollectTableView.Hidden = false;
			this.AddSubview(clickAndCollectTableView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.clickAndCollectTableView.Frame = new CGRect(0,0,this.Frame.Width,this.Frame.Height);
		}

		public void ShowErrorGettingDataView()
		{
			if (this.errorGettingDataView == null)
			{
				CGRect errorGettingDataViewFrame = new CGRect(0, this.TopLayoutGuideLength, this.Bounds.Width, this.Bounds.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength);
				this.errorGettingDataView = new ErrorGettingDataView (errorGettingDataViewFrame, () => {
					if (this.GetData != null)
					{
						this.GetData ();
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

		public void UpdateData (List<Store> stores) 
		{
			this.clickAndCollectTableViewSource = new ClickAndCollectStoreTableSource (stores);
			this.clickAndCollectTableViewSource.StoreInfoButtonPressed += (Store store) => 
			{
				if (this.StoreInfoButtonPressed != null)
				{
					this.StoreInfoButtonPressed (store);
				}
			};
			this.clickAndCollectTableViewSource.StoreSelected += (Store store) => 
			{
				if (this.StoreSelected != null)
				{
					this.StoreSelected (store);
				}
			};
			this.clickAndCollectTableView.Source = this.clickAndCollectTableViewSource;
			this.clickAndCollectTableView.ReloadData();
		}
	}
}

