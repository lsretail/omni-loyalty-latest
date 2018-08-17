using System;
using UIKit;

namespace Presentation.Screens
{
	public class BaseView : UIView
	{
		public nfloat TopLayoutGuideLength { get; set; }
		public nfloat BottomLayoutGuideLength { get; set; }

		public BaseView()
		{
			BackgroundColor = Presentation.Utils.AppColors.BackgroundGray;
		}
	}
}

