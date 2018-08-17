using UIKit;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class ImageZoomController : UIViewController
	{
		private ImageZoomView rootView;

		public ImageZoomController (ImageView imageView)
		{
			this.Title = LocalizationUtilities.LocalizedString("ImageZoom_Image", "Image");
			this.rootView = new ImageZoomView(imageView);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Experiencing some layout issues if we set the view as the rootview, so let's just add the rootview to the view instead
			this.View.AddSubview(this.rootView);	
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			this.rootView.Frame = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
		}			
	}
}

