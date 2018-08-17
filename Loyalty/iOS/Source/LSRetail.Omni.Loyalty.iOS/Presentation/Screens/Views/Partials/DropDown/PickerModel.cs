using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
namespace Presentation
{
	public class PickerModel : UIPickerViewModel
	{
		private readonly IList<string> values;

		public event EventHandler<PickerChangedEventArgs> PickerChanged;

		public PickerModel(IList<string> values)
		{
			this.values = values;
		}

		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}


		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return values.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			return values[(int)row];
		}

		public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
		{
			return 40f;
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			if (this.PickerChanged != null)
			{
				this.PickerChanged(this, new PickerChangedEventArgs { SelectedValue = values[(int)row] });
			}
		}
	}
}

