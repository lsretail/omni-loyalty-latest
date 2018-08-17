using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Infrastructure.Data.SQLite2
{
	public enum SQLStatusCode
	{
		OK = 0,
		GetWebserviceDataError,
		SaveWebserviceDataError,
		GetFavoritesError,
		SaveFavoritesError,
		GetTransactionsError,
		SaveTransactionsError,
		GetContactError,
		SaveContactError,
		ClearContactError,
		GetBasketError,
		SaveBasketError,
		GetMobileMenuError,
		SaveMobileMenuError
	}
}
