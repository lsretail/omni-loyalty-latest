using System.Linq;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace Presentation.Utils
{
	public class MenuUtils
	{
		public static int DefaultMenu
		{
			get
			{
				if (AppData.MobileMenu == null)
					return -1;

				for (int i = 0; i < AppData.MobileMenu.MenuNodes.Count; i++)
				{
					if (AppData.MobileMenu.MenuNodes[i].DefaultMenu)
						return i;
				}

				return -1;
			}
		}

		public static Menu GetMenuById(string menuId)
		{
			return AppData.MobileMenu.MenuNodes.FirstOrDefault(X => X.Id == menuId);
		}

		public static int GetMenuPositionById(string menuId)
		{
			if (AppData.MobileMenu.MenuNodes == null)
				return -1;

			for (int i = 0; i < AppData.MobileMenu.MenuNodes.Count; i++)
			{
				if (AppData.MobileMenu.MenuNodes[i].Id == menuId)
					return i;
			}

			return -1;
		}
	}
}