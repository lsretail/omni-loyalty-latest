using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace Presentation.Utils
{
	public static class AppData
	{
		public static string SecurityToken = "";
		public static MobileMenu MobileMenu { get; set; }
		public static List<Store> Stores { get; set;}
		public static MemberContact Contact { get; set; }
		public static bool UserLoggedIn { get { return Contact != null; } }
		public static Basket Basket;
		public static CLLocationCoordinate2D DefaultLocationCoordinates { get { return new CLLocationCoordinate2D (64.145280, -21.907184); } } // Intersection of Sæbraut and Kringlumýrarbraut, Reykjavík

		public static bool ShouldRefreshPublishedOffers { get; set; }
		public static bool ShouldRefreshPoints { get; set; }
		public static bool MobileMenuWasLoadedFromServer { get; set; }

		public static List<PublishedOffer> SelectedPublishedOffers 
		{ 
			get 
			{
				if (Contact == null)
				{
					return new List<PublishedOffer>();
				}
				else
				{
					return Contact.PublishedOffers.Where(x => x.Selected).ToList();
				}
			}
		}
			
		public static List<Transaction> Transactions { get; set; }
		public static List<IFavorite> Favorites { get; set; }

		static AppData()
		{
			MobileMenu = new MobileMenu();
			Stores = new List<Store>();
			Basket = new Basket();
			Transactions = new List<Transaction>();
			Favorites = new List<IFavorite>();
		}

		public static string FormatCurrency(decimal amount)
		{
			if (MobileMenu != null)
			{
				if (MobileMenu.Currency != null)
				{
					return MobileMenu.Currency.FormatDecimal(amount);
				}
			}

			return amount.ToString();
		}
	}
}