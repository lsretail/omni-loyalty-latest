using System;
using MonoTouch.Dialog;
using Presentation.Utils;
using Foundation;

namespace Presentation.Utils
{
	public class Settings
	{
		private static string BaseURL =  "BaseURL";
		private static string DEFAULT_MENU_CELL_STYLE = "0";
		private static string KEY_MENU_CELL_STYLE_ID = "CellStyle";

		public Settings ()
		{
		}

		public static string GetBaseURL()
		{
			var prefs = NSUserDefaults.StandardUserDefaults;

			if (prefs.StringForKey (BaseURL) != null)
				return prefs.StringForKey (BaseURL);
			else
				return LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils.DefaultUrlLoyalty;
		}

		public static void SetBaseURL(string url)
		{
			NSUserDefaults.StandardUserDefaults.SetString (url,BaseURL);
		}

		public static string GetStyleId()
		{
			var prefs = NSUserDefaults.StandardUserDefaults;

			string setting = prefs.StringForKey(KEY_MENU_CELL_STYLE_ID);

			if (setting == null)
			{
				prefs.SetString(DEFAULT_MENU_CELL_STYLE, KEY_MENU_CELL_STYLE_ID);
				setting = prefs.StringForKey(KEY_MENU_CELL_STYLE_ID);
			}

			return setting;
		}

		public static void SetStyleId(string id)
		{
			var prefs = NSUserDefaults.StandardUserDefaults;

			prefs.SetString(id, KEY_MENU_CELL_STYLE_ID);
		}
	}
}

