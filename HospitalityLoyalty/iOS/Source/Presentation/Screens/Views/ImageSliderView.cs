using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace Presentation.Screens
{
	//Full Screen ImageSlider thats able to zoom images.
	public class ImageSliderView : BaseView, ZoomImageView.IZoomImageView
	{
		#region private variables
		//Slider
		private UIScrollView scrollView;
		//Image to display on appearing.
		private nint displayImageIndex;
		//Num of images to populate slider
		private nint numOfImgs;
		#endregion
		#region public variables
		public nfloat navBarHeight;
		#endregion
		#region interface
		private readonly IImageSliderListeners listeners;

		public interface IImageSliderListeners
		{
			UIImage GetImage(int index);
			List<UIImage> GetImages();
			void ChangeTitle(string newTitle);
		}
		#endregion

		public ImageSliderView(nint displayImageIndex, IImageSliderListeners listeners)
		{
			this.displayImageIndex = displayImageIndex;
			this.listeners = listeners;
			this.numOfImgs = listeners.GetImages().Count;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			scrollView = new UIScrollView
			{
				Frame = new CGRect(0, 0, this.Frame.Width, this.Frame.Height),
				ContentSize = new CGSize(this.Frame.Width * numOfImgs, this.Frame.Height),
				BackgroundColor = UIColor.White,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				PagingEnabled = true
			};

			this.scrollView.Scrolled += OnSwipe;

			this.AddSubview(scrollView);

			SetupSlides();

			Scroll();
		}

		//Populates slider with zoom images.
		private void SetupSlides()
		{
			nfloat width = this.Frame.Width;
			nfloat height = this.Frame.Height + navBarHeight;

			for (int i = 0; i < numOfImgs; i++)
			{
				nfloat x = this.Frame.Width * i;

				UIImageView img = new UIImageView(this.listeners.GetImage(i));
				img.Frame = new CGRect(0, 0, width, height);
				img.ContentMode = UIViewContentMode.ScaleAspectFit;


				ZoomImageView imgContainer = new ZoomImageView(img, x, -navBarHeight, width, height);
				imgContainer.listeners = this;
				scrollView.AddSubview(imgContainer);
			}
		}

		//Set controllers navigation title based on horizontal scroll position of slider.
		private void OnSwipe(object obj, EventArgs ev)
		{
			int page = Convert.ToInt32(Math.Floor(scrollView.ContentOffset.X / scrollView.Frame.Width));
			if (page > -1 && page < this.listeners.GetImages().Count)
				this.listeners.ChangeTitle((page + 1).ToString() + " of " + this.listeners.GetImages().Count.ToString());
		}

		//Sets scroll to selected image of previous screen.
		private void Scroll()
		{
			if (this.displayImageIndex > 0)
			{
				this.scrollView.ContentOffset = new CGPoint(this.Frame.Width * this.displayImageIndex, 0);
			}
		}

		//ZoomImageView interface function, Disables scroll on slider when image is zoomed
		public void SetScroll(bool isZoomed)
		{
			if (isZoomed)
			{
				this.scrollView.ScrollEnabled = false;
			}
			else
			{
				this.scrollView.ScrollEnabled = true;
			}
		}
	}
}

