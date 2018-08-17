﻿using System;
using CoreGraphics;
using Presentation.Utils;
using UIKit;
using Foundation;

namespace Presentation.UI
{
	public class BaseCollectionViewController : UICollectionViewController
	{
		public BaseCollectionViewController( ) : base(new UICollectionViewFlowLayout())
		{
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			if ((IsMovingFromParentViewController || IsBeingDismissed) && ParentViewController == null)
			{
				MarkAsDismissed();
			}
		}

		public void MarkAsDismissed()
		{
			if (View != null)
			{
				View.Dispose();
				View = null;
			}

			ViewDismissed();
		}

		protected void ViewDismissed()
		{
			// For child classes to implement
		}

	}
}
