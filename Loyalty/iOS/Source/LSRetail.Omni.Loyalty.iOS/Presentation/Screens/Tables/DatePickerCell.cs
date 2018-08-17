using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace Presentation
{
	public class DatePickerCell : UITableViewCell
	{
		public static string KEY = "DATEPICKERCELL";
		private UIDatePicker datePicker;

		public delegate void DatePickerValueChangedEventHandler(DateTime date);
		public event DatePickerValueChangedEventHandler DatePickerValueChanged;

		// guide: http://masteringios.com/blog/2013/11/18/ios-7-in-line-uidatepicker-part-2/2/

		public DatePickerCell () : base(UITableViewCellStyle.Default, KEY)
		{
			this.datePicker = new UIDatePicker();
			this.datePicker.Mode = UIDatePickerMode.Date;
			this.datePicker.MaximumDate = NSDate.Now;
			this.datePicker.ValueChanged += (sender, e) => 
			{
				DateTime date = Utils.Util.NSDateToDateTime((sender as UIDatePicker).Date);

				if(this.DatePickerValueChanged != null)
					this.DatePickerValueChanged(date);
			};
			this.datePicker.Hidden = true;
			this.ContentView.AddSubview(this.datePicker);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.datePicker.Frame = this.ContentView.Frame;
		}

		public void SetValues(DateTime date)
		{
			// if date has not been set yet
			if(date == DateTime.MinValue)
				date = DateTime.Now;

			NSDate nsDate = Utils.Util.DateTimeToNSDate(date);

			this.datePicker.SetDate(nsDate, true);
		}

		public void ShowDatePicker()
		{
			this.datePicker.Hidden = false;
			this.datePicker.Alpha = 0f;

			UIView.Animate(0.25, 
				() =>
				{
					this.datePicker.Alpha = 1f;
				}
			);
		}

		public void HideDatePicker()
		{
			UIView.Animate(0.25, 
				() =>
				{
					this.datePicker.Alpha = 0f;
				},
				() =>
				{
					this.datePicker.Hidden = true;
				}
			);
		}
	}
}

