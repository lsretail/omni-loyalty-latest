using System;
using CoreGraphics;
using System.Collections.Generic;
using UIKit;
using CoreAnimation;
using Presentation.Models;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class ImageCarouselView : UIScrollView
	{
		private nfloat pageWidth 
		{ 
			get 
			{ 
				if (this.Superview != null) 
					return this.Superview.Bounds.Width;
				else
					return 0f; 
			} 
		}

		private List<Tuple<ImageView, UIImageView>> imageViewTuples;
		private UIViewContentMode contentMode;

		public ImageCarouselView(UIViewContentMode contentMode = UIViewContentMode.ScaleAspectFill)
		{
			this.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.ShowsHorizontalScrollIndicator = false;
			this.ShowsVerticalScrollIndicator = false;
			this.PagingEnabled = true;
			this.imageViewTuples = new List<Tuple<ImageView, UIImageView>> ();
			this.ScrollEnabled = true;
			this.contentMode = contentMode;
			this.UseImageAverageColorAsBackgroundColor = true;
		}
			
		public bool UseImageAverageColorAsBackgroundColor { get; set; }

		private List<ImageView> imageViews;
		public List<ImageView> ImageViews
		{
			get
			{
				return this.imageViews;
			}
			set
			{
				this.imageViews = value;
				BuildImageViewTuples(this.imageViews);
				LoadImages();
			}
		}

		private void BuildImageViewTuples(List<ImageView> imageViews)
		{
			this.imageViewTuples = new List<Tuple<ImageView, UIImageView>>();

			foreach (ImageView imageView in imageViews)
			{
				var imageViewTuple = new Tuple<ImageView, UIImageView>(
					imageView, 
					new UIImageView()
					{ 
						BackgroundColor = this.UseImageAverageColorAsBackgroundColor ? ColorUtilities.GetUIColorFromHexString(imageView.AvgColor) : UIColor.Clear,
						ContentMode = this.contentMode
					}
				);

				this.imageViewTuples.Add(imageViewTuple);
				this.AddSubview(imageViewTuple.Item2);
			}
		}

		private void LoadImages()
		{
			ImageModel imageModel = new ImageModel();

			foreach (var imageViewTuple in this.imageViewTuples)
			{
				if(imageViewTuple.Item1.LoadFromFile == false)
				{
					imageModel.ImageGetById(imageViewTuple.Item1.Id, new ImageSize(700, 500),
						(x, destinationId) => {

							imageViewTuple.Item2.Image = ImageUtilities.FromBase64(x.Image);

							CATransition transition = new CATransition ();
							transition.Duration = 0.5f;
							transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
							transition.Type = CATransition.TransitionFade;
							imageViewTuple.Item2.Layer.AddAnimation (transition, null);

						},
						() => {},
						string.Empty
					);	
				}
				else
				{
                    UIImage image = imageModel.GetImageByIdFromFile(imageViewTuple.Item1.Id);
                    if (image != null) 
						{
							imageViewTuple.Item2.Image = image;

							CATransition transition = new CATransition ();
							transition.Duration = 0.5f;
							transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
							transition.Type = CATransition.TransitionFade;
							imageViewTuple.Item2.Layer.AddAnimation (transition, null);
						}
						else{}
					
				}
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			nfloat xOffset = 0f;

			foreach (var imageViewTuple in this.imageViewTuples)
			{
				imageViewTuple.Item2.Frame = new CGRect(xOffset, 0, pageWidth, this.Bounds.Height);
				xOffset += pageWidth;
			}

			this.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
			this.ContentSize = new CGSize(this.imageViewTuples.Count * this.pageWidth, this.Bounds.Height);
		}
			
		public void ScrollToImageWithIndex(nint index, bool animated)
		{
			if (index >= this.imageViews.Count || index < 0)
				return;

			//this.SetContentOffset(new CGPoint(index * this.pageWidth, 0), animated);		
			this.ScrollRectToVisible(new CGRect(index * this.pageWidth, 0, this.Frame.Width, this.Frame.Height), animated); 
		}

		public UIImageView GetUIImageViewAtIndex(int index)
		{
			if (index > this.imageViewTuples.Count - 1)
				return null;

			return this.imageViewTuples[index].Item2;								
		}
	}
}