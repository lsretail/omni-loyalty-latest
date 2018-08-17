using System;
using UIKit;
using Presentation.Screens;
using CoreGraphics;

namespace Presentation
{
	//Scroll view that zooms in on content when double tapped. 
	//pass in listener if you want to disable scrolling of parent view.
	public class ZoomImageView : UIScrollView
	{
		#region private variable
		private UIImageView image;
		#endregion

		#region interface
		public IZoomImageView listeners { get; set; }

		public interface IZoomImageView
		{
			void SetScroll(bool isZoomed);
		}
		#endregion

		public ZoomImageView(UIImageView img, nfloat x, nfloat y, nfloat width, nfloat height)
		{
			image = img;

			#region UIScrollView settings
			Frame = new CGRect(x, y, width, height);
			ContentSize = image.Image.Size;
			ScrollEnabled = false;
			MinimumZoomScale = 1f;
			MaximumZoomScale = 6f;
			BouncesZoom = true;
			ClipsToBounds = true;
			UITapGestureRecognizer doubleTap = new UITapGestureRecognizer();
			doubleTap.NumberOfTapsRequired = 2; // double tap
			doubleTap.AddTarget(() => { HandleDoubleTap(doubleTap); });
			AddGestureRecognizer(doubleTap);
			AddSubview(image);
			ViewForZoomingInScrollView = delegate
			{
				return this.image;
			};
			#endregion

		}

		//Zooms image in and out when double tapped
		private void HandleDoubleTap(UITapGestureRecognizer doubleTap)
		{

			#region zoom out
			if (ZoomScale > 1)
			{
				
				ScrollEnabled = false;
				SetZoomScale(1f, true);
				if (listeners != null)
					listeners.SetScroll(false);
			}
			#endregion

			#region zoom in
			else
			{
				ScrollEnabled = true;
				SetZoomScale(3f, true);
				if (listeners != null)
					listeners.SetScroll(true);
			}
			#endregion
		}
	}
}

