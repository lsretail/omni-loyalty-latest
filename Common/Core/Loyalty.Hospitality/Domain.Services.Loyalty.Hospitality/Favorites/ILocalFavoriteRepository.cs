using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Favorites
{
    public interface ILocalFavoriteRepository
    {
        List<IFavorite> GetFavorites();
        void SaveFavorites(List<IFavorite> favorites);
    }
}
