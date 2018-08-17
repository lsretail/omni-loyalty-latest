using System;
using UIKit;

namespace Presentation
{
	public class BaseView : UIView
	{			
		public nfloat TopLayoutGuideLength { get; set; }
		public nfloat BottomLayoutGuideLength { get; set;}

		public BaseView ()
		{
			BackgroundColor = Utils.AppColors.BackgroundGray;
		}
	}
}

