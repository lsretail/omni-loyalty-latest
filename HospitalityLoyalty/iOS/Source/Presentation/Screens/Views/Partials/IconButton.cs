using System;
using UIKit;
using CoreGraphics;

namespace Presentation
{
	public class IconButton : UIButton
	{
		private UIImageView btnAddToBasketImageView;
		//IconButton, basic button that is used on multiple places
		public IconButton(string label, UIImage img)
		{
			SetTitle(label, UIControlState.Normal);
			SetTitleColor(UIColor.White, UIControlState.Normal);
			if (Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				Font = UIFont.SystemFontOfSize(16f);

			BackgroundColor = Utils.AppColors.PrimaryColor;
			Layer.CornerRadius = 2;

			btnAddToBasketImageView = new UIImageView();
			btnAddToBasketImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnAddToBasketImageView.Image = img;
			btnAddToBasketImageView.BackgroundColor = UIColor.Clear;
			AddSubview(btnAddToBasketImageView);
		}
	}
}

