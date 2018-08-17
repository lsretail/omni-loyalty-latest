using System;

namespace Presentation
{
	public class SearchPopUpDto
	{
		public bool Item { get; set; }
		public bool Offer { get; set; }
		public bool Coupon { get; set; }
		public bool Notification { get; set; }
		public bool History { get; set; }
		public bool ShoppingList { get; set; }
		public bool Store { get; set; }

		public SearchPopUpDto()
		{
			Item = true;
			Offer = true;
			Coupon = true;
			Notification = true;
			History = true;
			ShoppingList = true;
			Store = true;
		}
	}
}

