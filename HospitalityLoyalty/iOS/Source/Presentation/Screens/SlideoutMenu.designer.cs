// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Presentation.Screens
{
	[Register ("SlideoutMenu")]
	partial class SlideoutMenu
	{
		[Outlet]
		UIKit.UIView AccountView { get; set; }

		[Outlet]
		UIKit.UITableView MenuView { get; set; }

		[Outlet]
		UIKit.UIView ViewNoStatusbar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ViewNoStatusbar != null) {
				ViewNoStatusbar.Dispose ();
				ViewNoStatusbar = null;
			}

			if (AccountView != null) {
				AccountView.Dispose ();
				AccountView = null;
			}

			if (MenuView != null) {
				MenuView.Dispose ();
				MenuView = null;
			}
		}
	}
}
