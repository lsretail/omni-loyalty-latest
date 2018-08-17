using System;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.UI
{
	public interface IBasketSelectedListener
	{
		void BasketSelected(MobileMenuNode menu);
	}
}
