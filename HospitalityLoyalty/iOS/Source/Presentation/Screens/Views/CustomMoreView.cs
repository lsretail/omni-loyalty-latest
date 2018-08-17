using System;
using System.Collections.Generic;
using UIKit;

namespace Presentation.Screens
{
	public class CustomMoreView : UIView
	{
		private UITableView tblControllers;

		public delegate void OnControllerSelectedEventHandler(UIViewController controller);
		public event OnControllerSelectedEventHandler OnControllerSelected;


		public CustomMoreView(List<UIViewController> controllers)
		{
			this.tblControllers = new UITableView();
			this.tblControllers.Source = new CustomMoreControllerTableSource(controllers);
			(this.tblControllers.Source as CustomMoreControllerTableSource).ControllerSelected += ControllerSelected;

			AddSubview(this.tblControllers);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			this.tblControllers.Frame = new CoreGraphics.CGRect(0, 0, this.Frame.Width, this.Frame.Height);

		}

		public void ControllerSelected(UIViewController controller)
		{
			if (OnControllerSelected != null)
			{
				OnControllerSelected(controller);
			}
		}

		public void Refresh()
		{
			this.tblControllers.ReloadData();
		}
	}
}

