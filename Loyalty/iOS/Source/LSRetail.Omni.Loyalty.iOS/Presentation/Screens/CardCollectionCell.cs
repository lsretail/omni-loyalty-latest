using System;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreAnimation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	/// <summary>
	/// General card collection cell. Two different layouts, depending on cell height.
	/// </summary>
	public class CardCollectionCell : UICollectionViewCell
	{
		public static readonly NSString TallNarrowCellKey = new NSString ("CardCollectionCellTallNarrow");
		public static readonly NSString TallWideCellKey = new NSString ("CardCollectionCellTallWide");
		public static readonly NSString ShortNarrowCellKey = new NSString ("CardCollectionCellShortNarrow");
		public static readonly NSString ShortWideCellKey = new NSString ("CardCollectionCellShortWide");

		public object objectOnDisplay;
		public Action<object> onSelected;
		public int Id;
		protected CellSizes size;
		protected bool shouldRefreshLayout = false;

		[Export ("initWithFrame:")]
		public CardCollectionCell (CGRect frame) : base (frame)
		{
			SetLayout();
			//AddDropShadow ();
		}

		/// <summary>
		/// Set values for this cell instance, what should it display? NOTE: Should give a new version of this method in subclasses.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="objectToDisplay">Object to display.</param>
		/// <param name="onSelected">On selected.</param>
		/// <param name="size">Size.</param>
		/// <param name="text">Text.</param>
		/// <param name="imageColorHex">Image color hex.</param>
		/// <param name="imageId">Image identifier.</param>
		/// <param name="localImage">If set to <c>true</c> local image.</param>
		public void SetValues (int id, object objectToDisplay, Action<object> onSelected, CardCollectionCell.CellSizes size, string text, string imageColorHex, string imageId, bool localImage)
		{
			this.Id = id;
			this.objectOnDisplay = objectToDisplay;
			this.onSelected = onSelected;

			if (size != this.size)
				shouldRefreshLayout = true;
				
			this.size = size;

			if (shouldRefreshLayout)
				SetLayout();

			UILabel lblText = (UILabel)this.ContentView.ViewWithTag (300);
			lblText.Text = text;

			UIImageView imageView = (UIImageView)this.ContentView.ViewWithTag (100);
			imageView.BackgroundColor = ColorUtilities.GetUIColorFromHexString (imageColorHex);
			imageView.Layer.RemoveAllAnimations();
			imageView.Image = null;

			LoadImageToImageView(imageId, localImage, imageView);
		}
			
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			this.onSelected(this.objectOnDisplay);
		}
	
		private void AddDropShadow()
		{
			this.Layer.ShadowOffset = new CGSize (1, 1);
			this.Layer.ShadowRadius = 2.0f;
			this.Layer.ShadowOpacity = 0.8f;
			CoreGraphics.CGPath shadowPath = new CoreGraphics.CGPath();
			shadowPath.AddRect (this.Layer.Bounds);
			this.Layer.ShadowPath = shadowPath;
		}
			
		protected virtual void SetLayout()
		{
			// TODO: Define what layout we want to use explicitly. Now we determine layouts depending on cell heights.

			ClearSubviews();

			if (this.size == CellSizes.TallNarrow)
				SetLayoutTall();
			else if (this.size == CellSizes.TallWide)
				SetLayoutTall();
			else if (this.size == CellSizes.ShortNarrow)
				SetLayoutShort();
			else if (this.size == CellSizes.ShortWide)
				SetLayoutShort();
		}

		private void SetLayoutTall()
		{
			// Image view
			UIImageView imageView = new UIImageView ();
			imageView.Frame = this.ContentView.Frame;
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;
			this.ContentView.AddSubview (imageView);

			// Overlay view
			UIView overlayView = new UIView ();
			int overlayViewHeight = (int)Math.Floor (this.ContentView.Bounds.Height / 4);
			overlayView.Frame = new CGRect (this.ContentView.Bounds.X, this.ContentView.Bounds.Height - overlayViewHeight, this.ContentView.Bounds.Width, overlayViewHeight);
			overlayView.BackgroundColor = Utils.AppColors.TransparentBlack;
			overlayView.Tag = 200;
			this.ContentView.AddSubview (overlayView);

			/* Gradient - NOT IN USE
			// Add gradient to overlayview
			MonoTouch.CoreAnimation.CAGradientLayer gradientLayer = new MonoTouch.CoreAnimation.CAGradientLayer ();
			gradientLayer.Frame = overlayView.Bounds;
			MonoTouch.CoreGraphics.CGColor[] colors = new MonoTouch.CoreGraphics.CGColor[2];
			colors [0] = UIColor.Clear.CGColor;
			colors [1] = Utils.AppColors.TransparentBlack.CGColor;
			gradientLayer.Colors = colors;
			gradientLayer.EndPoint = new PointF (0.5f, 1f);
			overlayView.Layer.InsertSublayer (gradientLayer, 0);
			*/

			// Text label
			UILabel lblText = new UILabel ();
			float margin = 5f;
			lblText.Frame = new CGRect(overlayView.Frame.X + margin, overlayView.Frame.Y, overlayView.Frame.Width - 2 * margin, overlayView.Frame.Height);
			lblText.TextColor = UIColor.White;
			lblText.Font = UIFont.FromName ("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.Tag = 300;
			lblText.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview (lblText);
		}

		private void SetLayoutShort()
		{
			// Image view
			UIImageView imageView = new UIImageView ();
			imageView.Frame = new CGRect(0, 0, this.ContentView.Frame.Width /4, this.ContentView.Frame.Height);
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;
			this.ContentView.AddSubview (imageView);

			// Text container view
			UIView textContainerView = new UIView ();
			textContainerView.Frame = new CGRect (imageView.Frame.Right, 0, this.ContentView.Bounds.Width - imageView.Frame.Right, this.ContentView.Frame.Height);
			textContainerView.BackgroundColor = UIColor.White;
			textContainerView.Tag = 200;
			this.ContentView.AddSubview (textContainerView);

			// Text label
			UILabel lblText = new UILabel ();
			float margin = 10f;
			lblText.Frame = new CGRect(textContainerView.Frame.X + margin, textContainerView.Frame.Y, textContainerView.Frame.Width - 2 * margin, textContainerView.Frame.Height);
			lblText.TextColor = Utils.AppColors.TextColor;
			lblText.Font = UIFont.FromName ("Helvetica", 14);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.BackgroundColor = UIColor.Clear;
			lblText.Tag = 300;
			this.ContentView.AddSubview (lblText);
		}
			
		protected void LoadImageToImageView(string imageId, bool localImage, UIImageView imageView)
		{
			// Note:
			// If we use the DequeueReusableCell() method of getting cells for display, iOS only uses a couple of memory addresses for cells, which it reuses as needed.
			// SetValues() is however run for every cell that is "scrolled over". If the user scrolls fast, and the SetValues() operation takes
			// a long time, it is possible that the cell in the memory address that prompted the SetValues() operation has changed.
			// So we might see the cell flash with the wrong information as the first SetValues() operation for that memory address finishes, 
			// and then get updated to the right information as the second SetValues() operation acting on that memory address finishes.
			// We fix this by supplying the ImageGetById thread with the ID of the cell that calls it. When the thread returns to a cell instance, it returns both an image
			// and the ID it was given. We then compare the ID the thread returns with the ID of the cell it returned to, and apply the image only if they match.

			if (localImage)
			{
                UIImage image = new Models.ImageModel().GetImageByIdFromFile(imageId);
                if (image != null){

					imageView.Image = image;

					CATransition transition = new CATransition ();
					transition.Duration = 0.5f;
					transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
					transition.Type = CATransition.TransitionFade;
					imageView.Layer.AddAnimation (transition, null);
				}
                else { /* Failure, do nothing (for the moment at least) */ }
			}
			else
			{
                var imageSize = new ImageSize(700, 500);
                new Models.ImageModel ().ImageGetById (imageId, imageSize, (dloadedImageView, destinationId) => {

					if (destinationId == this.Id.ToString())
					{
						// This is the correct image for this cell, let's apply it

						imageView.Image = ImageUtilities.FromBase64(dloadedImageView.Image);

						CATransition transition = new CATransition ();
						transition.Duration = 0.5f;
						transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
						transition.Type = CATransition.TransitionFade;
						imageView.Layer.AddAnimation (transition, null);
					}
				}, 
					() => { /* Failure, do nothing (for the moment at least) */ },
					this.Id.ToString());
			}
		}

		protected void ClearSubviews()
		{
			foreach (UIView subview in this.ContentView.Subviews)
				subview.RemoveFromSuperview();
		}

		public static bool IsCellSizeWide (CellSizes size)
		{
			if (size == CellSizes.ShortWide || size == CellSizes.TallWide)
				return true;
			else
				return false;
		}

		public static CellSizes GetNextCellSizeInCycle(List<CellSizes> cellSizeList, CellSizes currentSize)
		{
			int currentIndex = cellSizeList.IndexOf(currentSize);
			int nextIndex = ++currentIndex;
			if (nextIndex > cellSizeList.Count - 1)
				nextIndex = 0;
			return cellSizeList[nextIndex];
		}

		public enum CellSizes
		{
			TallNarrow,
			ShortNarrow,
			TallWide,
			ShortWide
		}
	}
}