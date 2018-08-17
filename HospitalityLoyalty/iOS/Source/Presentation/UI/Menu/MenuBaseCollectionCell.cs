using System;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreAnimation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.UI
{
	/// <summary>
	/// General card collection cell. Two different layouts, depending on cell height.
	/// </summary>
	public class MenuBaseCollectionCell : UICollectionViewCell
	{

		public MobileMenuNode menu;

		[Export("initWithFrame:")]
		public MenuBaseCollectionCell(CGRect frame) : base(frame)
		{
		}

		public virtual void SetValue(MobileMenuNode menu){
			this.menu = menu;
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
				if (image != null)
				{

					imageView.Image = image;

					CATransition transition = new CATransition();
					transition.Duration = 0.5f;
					transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
					transition.Type = CATransition.TransitionFade;
					imageView.Layer.AddAnimation(transition, null);
				}
			}

			else
			{
				new Models.ImageModel().ImageGetById(imageId, new ImageSize(700, 500), (dloadedImageView, destinationId) =>
				{


					imageView.Image = Utils.Image.FromBase64(dloadedImageView.Image);

					CATransition transition = new CATransition();
					transition.Duration = 0.5f;
					transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
					transition.Type = CATransition.TransitionFade;
					imageView.Layer.AddAnimation(transition, null);
				}, null);
			}
		}

		public virtual void SetPrice(string itemPrice)
		{
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