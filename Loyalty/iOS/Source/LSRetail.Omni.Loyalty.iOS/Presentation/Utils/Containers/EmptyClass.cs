using System;

namespace Presentation.Utils.Containers
{
	public class ContactUsItem
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public Action Action { get; set; }
	}
}

