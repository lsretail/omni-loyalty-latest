using System;
using UIKit;
using System.Collections.Generic;

namespace Presentation.Screens
{
	public class ImageSliderController : UIViewController, ImageSliderView.IImageSliderListeners
	{

		#region variables
		private List<UIImage> images;
		private nint displayImageIndex;
		private ImageSliderView rootView;
		#endregion

		#region constructor
		public ImageSliderController(List<UIImage> images, nint displayImageIndex)
		{
			this.AutomaticallyAdjustsScrollViewInsets = false;
			this.images = images;
			this.displayImageIndex = displayImageIndex;
			this.rootView = new ImageSliderView(displayImageIndex, this);

			this.Title = (this.displayImageIndex + 1).ToString() + " of " + this.images.Count.ToString();
		}
		#endregion

		#region overrides
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.Title = (this.displayImageIndex + 1).ToString() + " of " + this.images.Count.ToString();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			this.rootView.navBarHeight = this.NavigationController.NavigationBar.Frame.Height;

			this.View.BackgroundColor = UIColor.White;
			this.View = rootView;

		}
		#endregion

		#region interface implementation for ImagesView.IImagesListeners
		public UIImage GetImage(int index)
		{
			return images[index];
		}
		public List<UIImage> GetImages()
		{
			return images;
		}

		public void ChangeTitle(string newTitle)
		{
			this.Title = newTitle;
		}
		#endregion
	}
}

