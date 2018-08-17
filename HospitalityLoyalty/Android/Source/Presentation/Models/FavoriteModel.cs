using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Infrastructure.Data.SQLite2.Favorites;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Favorites;
using Presentation.Utils;

namespace Presentation.Models
{
    public class FavoriteModel : BaseModel
    {
        private LocalFavoriteService favoriteService;

        public FavoriteModel(Context context, IRefreshableActivity refreshableActivity = null) : base(context, refreshableActivity)
        {
        }

        protected override void CreateService()
        {
            favoriteService = new LocalFavoriteService(new FavoriteRepository());
        }

        public async Task SyncFavorites(bool inListUpdate = false)
        {
            Show(true);

            BeginWsCall();

            try
            {
                var favorites = await favoriteService.SyncFavoritesAsync(AppData.Favorites);

                AppData.Favorites = favorites;

                if (inListUpdate)
                {
                    SendBroadcast(BroadcastUtils.FavoritesUpdatedInList);
                }
                else
                {
                    SendBroadcast(BroadcastUtils.FavoritesUpdated);
                }
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);
        }

        public bool IsFavorite(IFavorite favorite)
        {
            BeginWsCall();
            
            if (AppData.Favorites == null)
                AppData.Favorites = favoriteService.GetFavorites();

            return AppData.Favorites.Any(x => x.Equals(favorite));
        }

        public async Task RenameFavorites(IFavorite favorite, string name)
        {
            favorite.Name = name;
            await SyncFavorites(true);

            //var favs = favoriteService.GetFavorites();
        }

        public async Task ToggleFavorite(IFavorite favorite, bool inListUpdate = false)
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

            if (inListUpdate)
            {
                SendBroadcast(BroadcastUtils.FavoritesUpdatedInList);
            }
            else
            {
                SendBroadcast(BroadcastUtils.FavoritesUpdated);
            }

            await SyncFavorites(inListUpdate);
        }
    }
}