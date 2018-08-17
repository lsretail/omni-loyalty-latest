using System;
using UIKit;
using CoreGraphics;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
	//Inline Quantity counter.
	public class InlineQtyModifier : UIView
	{
		public UILabel qty;
		public UIButton plus;
		public UIButton minus;
		private UILabel label; 

		public InlineQtyModifier(int num)
		{
			label = new UILabel();
			label.Text = LocalizationUtilities.LocalizedString("ItemDetails_Quantity", "Quantity") + ":";
			label.TextColor = Utils.AppColors.PrimaryColor;
			label.TextAlignment = UITextAlignment.Left;
			label.Font = UIFont.SystemFontOfSize(16f);

			minus = new UIButton();
			minus.SetTitle("-", UIControlState.Normal);
			minus.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			minus.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			minus.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			minus.Font = UIFont.SystemFontOfSize(30f);

			qty = new UILabel();
			qty.Text = num.ToString();
			qty.TextAlignment = UITextAlignment.Center;

			plus = new UIButton();
			plus.SetTitle("+", UIControlState.Normal);
			plus.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
			plus.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			plus.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			plus.Font = UIFont.SystemFontOfSize(25f);

			AddSubviews(label, minus, qty, plus);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			label.Frame = new CGRect(Frame.Left + 10, 0, Frame.Width / 2, Frame.Height);
			plus.Frame = new CGRect(Frame.Right - Frame.Height/2 - 10, Frame.Height / 4, Frame.Height/2, Frame.Height/2);
			qty.Frame = new CGRect(plus.Frame.Left - Frame.Height/2, 0, Frame.Height / 2, Frame.Height);
			minus.Frame = new CGRect(qty.Frame.Left - Frame.Height/2, Frame.Height / 4, Frame.Height/2, Frame.Height/2);

		}
	}
}

