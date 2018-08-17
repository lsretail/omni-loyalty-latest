using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Hospitality.Menus
{
    public class MenuRepository : BaseRepository, IMenuRepository
    {
        public MobileMenu GetMobileMenu()
        {
            string methodName = "MenusGetAll";
            var jObject = new { id = string.Empty, lastVersion = string.Empty };
			MobileMenu Mmenu = base.PostData<MobileMenu>(jObject, methodName);
			return Mmenu;
        }
    }
}
