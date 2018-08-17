using System;
using System.Collections.Generic;
using System.Linq;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Favorites;
using Infrastructure.Data.SQLite2.Favorites;
using Presentation.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;

namespace Presentation.Models
{
	public class FavoriteModel : BaseModel
	{
		private LocalFavoriteService localFavoriteService;

		public FavoriteModel()
		{
			this.localFavoriteService = new LocalFavoriteService(new FavoriteRepository());
		}
			
		public async void LoadLocalFavorites()
		{
			try
			{
				List<IFavorite> favorites = await this.localFavoriteService.GetFavoritesAsync();

				AppData.Favorites = favorites;
				System.Diagnostics.Debug.WriteLine("Favorites fetched successfully");
					

			}

			catch(Exception exception) 
			{
				HandleException (exception, "FavoriteModel.GetFavorites()", false);
			}

		}

		public async void SyncFavoritesLocally()
		{
			try
			{
				List<IFavorite> favorites = await this.localFavoriteService.SyncFavoritesAsync(AppData.Favorites);
					
				AppData.Favorites = favorites;
				System.Diagnostics.Debug.WriteLine("Favorites synced successfully");
			}

			catch(Exception exception) 
			{
				HandleException(exception, "FavoriteModel.SyncFavorites()", true);
			}

		}

		public bool IsFavorite(IFavorite favorite)
		{
			return AppData.Favorites.Any(x => x.Equals(favorite));
		}

		public void ToggleFavorite(IFavorite favorite)
		{
			var isFavorite = IsFavorite(favorite);

			if (isFavorite)
			{
				AppData.Favorites.RemoveAll(x => x.Equals(favorite));
			}
			else
			{
				AppData.Favorites.Add(favorite);
			}

			System.Diagnostics.Debug.WriteLine("Favoritecount: " + AppData.Favorites.Count.ToString());
				
			SyncFavoritesLocally();
		}

		public IFavorite EditFavorite(IFavorite favorite, string newName)
		{
			IFavorite localFavorite = null;
			var isFavorite = IsFavorite(favorite);

			if (isFavorite)
			{
				localFavorite = AppData.Favorites.SingleOrDefault(x => x.Equals(favorite));
				localFavorite.Name = newName;
			}

			SyncFavoritesLocally();

			return localFavorite;
		}


		/* Async functions - Not in use
		public async void LoadLocalFavorites()
		{
			try
			{
				var favorites = await this.localFavoriteService.GetFavoritesAsync();

				AppData.Favorites = favorites;
				System.Diagnostics.Debug.WriteLine("Favorites fetched successfully");
			}
			catch (Exception ex)
			{
				HandleException (ex, "FavoriteModel.GetFavorites()", false);
			}
		}

		public async void SyncFavoritesLocally()
		{
			try
			{
				var favorites = await this.localFavoriteService.SyncFavoritesAsync(AppData.Favorites);

				AppData.Favorites = favorites;
				System.Diagnostics.Debug.WriteLine("Favorites synced successfully");
			}
			catch (Exception ex)
			{
				HandleException(ex, "FavoriteModel.SyncFavorites()", true);
			}
		}
		*/
	}
}