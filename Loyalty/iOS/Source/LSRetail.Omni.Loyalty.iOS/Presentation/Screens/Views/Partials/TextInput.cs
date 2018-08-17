using System;
using CoreGraphics;
using UIKit;

namespace Presentation
{
	public class TextInput : UIView
	{
		private UILabel lbl;
		public UITextField input;
		private float margin = 10f;

		public TextInput(string label, string placeholder, string value)
		{
			lbl = new UILabel
			{
				Text = label,
				TextColor = Utils.AppColors.PrimaryColor
			};
			input = new UITextField
			{
				Text = value,
			
				Placeholder = placeholder,
			};

			input.AddSubview(new UIView
			{
				BackgroundColor = Utils.AppColors.PrimaryColor
			});

			AddSubviews(lbl, input);

		}


		public override void LayoutSubviews()
		{
			
			base.LayoutSubviews();
			lbl.Frame = new CGRect(0, 0, Frame.Width, 20f);

			input.Frame = new CGRect(margin, lbl.Frame.Bottom, Frame.Width - margin, 40f);
			foreach (var child in input.Subviews)
			{

				child.Frame = new CGRect(0, input.Frame.Height - margin, input.Frame.Width - margin, 2f);
			}
		}
		public string getValue()
		{
			return input.Text;
		}
	}
}

