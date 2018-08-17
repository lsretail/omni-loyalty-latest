using System;
using MonoTouch.Dialog;
using System.Drawing;
using Presentation.Utils;
using Foundation;
using Presentation.Screens;


namespace Presentation.Utils
{
	public class Settings
	{
		private static string BaseURL =  "BaseURL";
		private static string BaseCellSize = "CellSize";

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

		public static CardCollectionCell.CellSizes GetItemListType()
		{
			var itemListType = NSUserDefaults.StandardUserDefaults.StringForKey(BaseCellSize);

			if (itemListType != null) 
			{
				switch (itemListType)
				{
				case "ShortNarrow":
					return CardCollectionCell.CellSizes.ShortNarrow;
				case "ShortWide":
					return CardCollectionCell.CellSizes.ShortWide;
				case "TallNarrow":
					return CardCollectionCell.CellSizes.TallNarrow;
				case "TallWide":
					return CardCollectionCell.CellSizes.TallWide;
				default:
					return CardCollectionCell.CellSizes.TallWide;
				}
			}
			else
				return CardCollectionCell.CellSizes.TallWide;
		}

		public static void SetItemListType(CardCollectionCell.CellSizes cellSize)
		{
			switch (cellSize)
			{
			case CardCollectionCell.CellSizes.ShortNarrow:
				NSUserDefaults.StandardUserDefaults.SetString ("ShortNarrow", BaseCellSize);
				break;
			case CardCollectionCell.CellSizes.ShortWide:
				NSUserDefaults.StandardUserDefaults.SetString ("ShortWide", BaseCellSize);
				break;
			case CardCollectionCell.CellSizes.TallNarrow:
				NSUserDefaults.StandardUserDefaults.SetString ("TallNarrow", BaseCellSize);
				break;
			case CardCollectionCell.CellSizes.TallWide:
				NSUserDefaults.StandardUserDefaults.SetString ("TallWide ", BaseCellSize);
				break;
			}
		}
	}
}

