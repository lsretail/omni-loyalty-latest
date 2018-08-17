using System;

namespace Presentation
{
	public class PickerChangedEventArgs : EventArgs
	{
		public string SelectedValue { get; set; }
	}
}