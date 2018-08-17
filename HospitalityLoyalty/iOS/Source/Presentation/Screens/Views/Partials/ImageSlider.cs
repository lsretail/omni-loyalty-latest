using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using UIKit;
namespace Presentation
{
	public class ImageSlider : UIScrollView
	{
		private int slides;
		public ImageSlider(int slds)
		{
			//Scroll settings
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			PagingEnabled = true;
			AlwaysBounceVertical = false;

			//number of slides to be set up
			slides = slds;

		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			ContentSize = new CGSize(Frame.Width * slides, Frame.Height);
		}

		//Calculates position of next image to be added. 
		//Adds some transistion animation to.
		public void SetupSlide(UIImage image)
		{
			//size and coords of slide.
			int index = Subviews.Length;
			nfloat y = 0,
			x = this.Frame.Width * index,
			width = Frame.Width,
			height = Frame.Height;

			//Setup image
			UIImageView img = new UIImageView();
			img.Image = image;
			CATransition transition = new CATransition();
			transition.Duration = 0.5f;
			transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			transition.Type = CATransition.TransitionFade;
			img.Layer.AddAnimation(transition, null);

			img.Frame = new CGRect(x, y, Frame.Width, Frame.Height);
			AddSubview(img);
		}

	}
}

