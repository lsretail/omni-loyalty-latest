using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus
{
    public interface ILocalMenuRepository
    {
        MobileMenu GetMobileMenu();
        void SaveMobileMenu(MobileMenu mobileMenu);
    }
}
