using System;
using UIKit;
using CoreGraphics;

namespace Presentation
{
	public class PopUpView : BaseView
	{
		private bool isVisible;

		private bool showShadeUnderView;
		private UIView shadeView;
		private UIColor shadeColor { get { return Utils.AppColors.TransparentBlack2; } }

		public bool IsVisible { get { return this.isVisible; } }

		public PopUpView (bool showShadeUnderView = false)
		{
			this.showShadeUnderView = showShadeUnderView;

			if (this.showShadeUnderView)
				this.shadeView = new UIView();
		}

		public void SetFrame(CGRect frameWhenVisible, bool startHidden = true)
		{
			this.Frame = frameWhenVisible;

			if(startHidden)
			{
				this.Hidden = true;
			}
			else
			{
				this.Hidden = false;
			}

			if (this.showShadeUnderView && this.shadeView != null)
			{
				if (startHidden)
				{
					this.shadeView.BackgroundColor = UIColor.Clear;
					this.shadeView.Hidden = true;
				}
				else
				{
					this.shadeView.BackgroundColor = shadeColor;
					this.shadeView.Hidden = false;
				}

				if (this.Superview != null)
				{
					this.shadeView.Frame = new CGRect(0, 0, this.Superview.Frame.Width, this.Superview.Frame.Height);
					this.Superview.InsertSubviewBelow(this.shadeView, this);
				}
			}
		}

		public void ShowWithAnimation(double animationDuration = 0.0)
		{
			// Note: Changes to UIView.Hidden can't be animated, have to unhide it here, before the animation,
			// but let's make it transparent, and then animate the transition to the shade color
			if (this.showShadeUnderView && this.shadeView != null)
			{
				this.shadeView.BackgroundColor = UIColor.Clear;
				this.shadeView.Hidden = false;
			}

			UIView.Animate(
				animationDuration, 
				() =>
				{
					this.isVisible = true;
					this.Hidden = false;

					if (this.showShadeUnderView && this.shadeView != null)
						this.shadeView.BackgroundColor = shadeColor;
				},
				() => {}
			);
		}

		public void HideWithAnimation(double animationDuration = 0.0)
		{
			UIView.Animate(
				animationDuration, 
				() =>
				{
					this.isVisible = false;
					this.Hidden = true;

					if (this.showShadeUnderView && this.shadeView != null)
						this.shadeView.BackgroundColor = UIColor.Clear;
				},
				() => 
				{
					if (this.showShadeUnderView && this.shadeView != null)
						this.shadeView.Hidden = true;
				}
			);
		}
	}
}

