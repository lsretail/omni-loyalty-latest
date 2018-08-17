// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Presentation.Screens
{
    [Register ("EditBasketItemScreen")]
    partial class EditBasketItemScreen
    {
        [Outlet]
        UIKit.UINavigationBar CustomNavBar { get; set; }


        [Outlet]
        UIKit.UIView ViewNoStatusBar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CustomNavBar != null) {
                CustomNavBar.Dispose ();
                CustomNavBar = null;
            }

            if (ViewNoStatusBar != null) {
                ViewNoStatusBar.Dispose ();
                ViewNoStatusBar = null;
            }
        }
    }
}