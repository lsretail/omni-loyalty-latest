using UIKit;
using CoreGraphics;
using CoreAnimation;
using Presentation.Models;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class ImageZoomView : BaseView
	{
		private ImageView imageView;
		private UIImageView uiImageView;
		private ZoomScrollView zoomScrollView;

		public ImageZoomView (ImageView imageView)
		{
			this.imageView = imageView;
			this.BackgroundColor = UIColor.White;

			this.zoomScrollView = new ZoomScrollView();
			this.zoomScrollView.BackgroundColor = UIColor.White;
			this.zoomScrollView.UserInteractionEnabled = true;
			this.zoomScrollView.ShowsHorizontalScrollIndicator = false;
			this.zoomScrollView.ShowsVerticalScrollIndicator = false;
			this.zoomScrollView.ScrollEnabled = true;
			this.zoomScrollView.MinimumZoomScale = 1f;
			this.zoomScrollView.MaximumZoomScale = 6f;
			this.zoomScrollView.BouncesZoom = true;
			this.zoomScrollView.ClipsToBounds = true;
			this.zoomScrollView.ViewForZoomingInScrollView = delegate {
				return this.uiImageView;
			};				
			this.zoomScrollView.AddGestureRecognizer(
				new UITapGestureRecognizer(
					(tapAction) => 
					{ 
						if (this.zoomScrollView.ZoomScale > 1)
							this.zoomScrollView.SetZoomScale(1f, true);
						else
							this.zoomScrollView.SetZoomScale(3f, true);				
					}
				)
				{ NumberOfTapsRequired = 2 }
			);				
			this.AddSubview(this.zoomScrollView);

			this.uiImageView = new UIImageView();
			this.uiImageView.BackgroundColor = UIColor.White;
			this.uiImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			this.zoomScrollView.Add(this.uiImageView);

			LoadImage();
		}
			
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.zoomScrollView.Frame = new CGRect(0, this.TopLayoutGuideLength, this.Frame.Width, this.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength);
			this.zoomScrollView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
			this.uiImageView.Frame = this.zoomScrollView.Frame;
		}
			
		private void LoadImage()
		{			
			new ImageModel().ImageGetById(
				this.imageView.Id, 
				new ImageSize(700, 500),
				(x, destinationId) => {

					this.uiImageView.Image = ImageUtilities.FromBase64(x.Image);

					CATransition transition = new CATransition ();
					transition.Duration = 0.5f;
					transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
					transition.Type = CAAnimation.TransitionFade;
					this.uiImageView.Layer.AddAnimation (transition, null);

				},
				() => {},
				string.Empty
			);
		}
			
		private class ZoomScrollView : UIScrollView
		{
			public override void LayoutSubviews ()
			{
				base.LayoutSubviews ();

				// Center the image as it becomes smaller than the size of the screen
				var boundsSize = this.Bounds.Size;

				// Get the subview that is being zoomed
				UIView subview = this.ViewForZoomingInScrollView (this);

				if (subview != null) 
				{
					var frameToCenter = subview.Frame;

					// Center horizontally
					if (frameToCenter.Size.Width < boundsSize.Width)
						frameToCenter.X = (boundsSize.Width - frameToCenter.Size.Width)/2;
					else
						frameToCenter.X = 0;

					// Center vertically
					if (frameToCenter.Size.Height < boundsSize.Height)
						frameToCenter.Y = (boundsSize.Height - frameToCenter.Size.Height)/2;
					else
						frameToCenter.Y = 0;

					subview.Frame = frameToCenter;
				}
			}
		}
	}
}

