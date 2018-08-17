using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Presentation.Screens
{
	/// <summary>
	/// A view that can be repositioned on the y axis (animated). Draggable by user (if user dragging allowed) or in code.
	/// </summary>
	public class YDraggableView : UIView
	{
		private CGPoint lastIncrementPoint;
		private nfloat maxY;
		private nfloat minY;
		private nfloat newY;
		private bool userDraggingAllowed;

		// Note: In the iOS coordinate system, Y increases downwards. These max & min constants however refer to the numeric value of Y.
		public YDraggableView(float maxY, float minY, bool userDraggingAllowed = true)
		{
			this.maxY = maxY;
			this.minY = minY;
			this.userDraggingAllowed = userDraggingAllowed;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			if (!this.userDraggingAllowed)
				return;

			UITouch myTouch = touches.AnyObject as UITouch;
			this.lastIncrementPoint = myTouch.LocationInView (this.Superview);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			if (!this.userDraggingAllowed)
				return;

			UITouch myTouch = touches.AnyObject as UITouch;
			CGPoint touchLocation = myTouch.LocationInView (this.Superview);

			newY = this.Frame.Y + (touchLocation.Y - lastIncrementPoint.Y);

			//System.Diagnostics.Debug.WriteLine (this.minY + " " + this.maxY);
			//System.Diagnostics.Debug.WriteLine ("newy: " + newY + " ----- " + this.Frame.Y + " + (" + touchLocation.Y + " - " +  lastIncrementPoint.Y);

			if (newY > maxY || newY < minY)
			{
				lastIncrementPoint = touchLocation;
				return;
			}

			UIView.BeginAnimations ("Slide animation");
			this.Frame = new CGRect (this.Bounds.X, newY, this.Bounds.Width, this.Bounds.Height);
			UIView.CommitAnimations ();

			lastIncrementPoint = touchLocation;
		}

		public void ShootToYMinCoordinate()
		{
			UIView.BeginAnimations ("Slide to y min coordinate animation");
			this.Frame = new CGRect (this.Frame.X, this.minY, this.Frame.Width, this.Frame.Height);
			UIView.CommitAnimations ();
			this.lastIncrementPoint = this.Frame.Location;
		}

		public void ShootToYMaxCoordinate()
		{
			UIView.BeginAnimations ("Slide to y max coordinate animation");
			this.Frame = new CGRect (this.Frame.X, this.maxY, this.Frame.Width, this.Frame.Height);
			UIView.CommitAnimations ();
			this.lastIncrementPoint = this.Frame.Location;
		}
	}
}

