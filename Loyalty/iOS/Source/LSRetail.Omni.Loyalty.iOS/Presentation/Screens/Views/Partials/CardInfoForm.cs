using System;
using UIKit;
using CoreGraphics;
namespace Presentation
{
	public class CardInfoForm : UIView
	{
		UITextField cardNumber;
		UITextField month;
		UITextField year;
		UITextField cvv;
		private float margin = 10f;

		public CardInfoForm()
		{
			BackgroundColor = UIColor.White;
			cardNumber = new UITextField()
			{
				Placeholder = "Card number",
				KeyboardType = UIKeyboardType.NumberPad
			};

			cardNumber.AddSubview(new UIView
			{
				BackgroundColor = Utils.AppColors.PrimaryColor
			});
			month = new UITextField()
			{
				Placeholder = "MM",
				KeyboardType = UIKeyboardType.NumberPad
			};
			month.AddSubview(new UIView
			{
				BackgroundColor = Utils.AppColors.PrimaryColor
			});
			year = new UITextField() 
			{ 
				Placeholder = "YYYY",
				KeyboardType = UIKeyboardType.NumberPad
			};
			year.AddSubview(new UIView
			{
				BackgroundColor = Utils.AppColors.PrimaryColor
			});
			cvv = new UITextField()
			{
				Placeholder = "CVV",
				KeyboardType = UIKeyboardType.NumberPad
			};
			cvv.AddSubview(new UIView
			{
				BackgroundColor = Utils.AppColors.PrimaryColor
			});
			AddSubviews(cardNumber, month, year, cvv);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			cardNumber.Frame = new CGRect(10f, 0, Frame.Width - 20f, 60f);
			foreach (var child in cardNumber.Subviews)
			{

				child.Frame = new CGRect(0, cardNumber.Frame.Height - margin, cardNumber.Frame.Width - margin, 2f);
			}
			month.Frame = new CGRect(10f, cardNumber.Frame.Bottom, Frame.Width / 3, 60f);
			foreach (var child in month.Subviews)
			{

				child.Frame = new CGRect(0, month.Frame.Height - margin, month.Frame.Width - margin, 2f);
			}
			year.Frame = new CGRect(month.Frame.Right, cardNumber.Frame.Bottom, Frame.Width / 3, 60f);
			foreach (var child in year.Subviews)
			{

				child.Frame = new CGRect(0, year.Frame.Height - margin, year.Frame.Width - margin, 2f);
			}
			cvv.Frame = new CGRect(year.Frame.Right, cardNumber.Frame.Bottom, Frame.Width / 3, 60f);
			foreach (var child in cvv.Subviews)
			{

				child.Frame = new CGRect(0, cvv.Frame.Height - margin, cvv.Frame.Width - margin, 2f);
			}
		}
	}
}

