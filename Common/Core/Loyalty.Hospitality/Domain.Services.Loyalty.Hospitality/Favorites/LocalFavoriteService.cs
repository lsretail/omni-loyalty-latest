using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Favorites;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Favorites
{
    public class LocalFavoriteService
    {
        private ILocalFavoriteRepository localFavoriteRepository;

        public LocalFavoriteService(ILocalFavoriteRepository localFavoriteRepository)
        {
            this.localFavoriteRepository = localFavoriteRepository;
        }

        public List<IFavorite> GetFavorites()
        {
            return localFavoriteRepository.GetFavorites();
        }

        public List<IFavorite> SyncFavorites(List<IFavorite> favorites)
        {
            localFavoriteRepository.SaveFavorites(favorites);

            return favorites;
        }

        #region Async

        public async Task<List<IFavorite>> GetFavoritesAsync()
        {
            return await Task.Run(() => GetFavorites());
        }

        public async Task<List<IFavorite>> SyncFavoritesAsync(List<IFavorite> favorites)
        {
            return await Task.Run(() => SyncFavorites(favorites));
        }

        #endregion

    }
}
