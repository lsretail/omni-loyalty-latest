using System;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus
{
    public class LocalMenuService
    {
        private ILocalMenuRepository repository;

        public LocalMenuService(ILocalMenuRepository repository)
        {
            this.repository = repository;
        }

        public MobileMenu GetMobileMenu()
        {
            return repository.GetMobileMenu();
        }

        public MobileMenu SyncMobileMenu(MobileMenu mobileMenu)
        {
            repository.SaveMobileMenu(mobileMenu);
            return repository.GetMobileMenu();
        }

        #region windows

        public async Task<MobileMenu> GetMobileMenuAsync()
        {
			return await Task.Run(() => GetMobileMenu()).ConfigureAwait(false);
        }

        public async Task<MobileMenu> SyncMobileMenuAsync(MobileMenu mobileMenu)
        {
            return await Task.Run(() => SyncMobileMenu(mobileMenu));
        }

        #endregion 
    }
}
