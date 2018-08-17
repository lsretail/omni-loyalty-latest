using System;
using Presentation.Utils;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Hospitality.Menus;

namespace Presentation.Models
{
	public class MenuModel : BaseModel
	{
		private MenuService menuService;
		private LocalMenuService localMenuService;
		private IMenuRepository menuRepository;

		public MenuModel ()
		{
			this.menuService = new MenuService ();
			this.localMenuService = new LocalMenuService(new Infrastructure.Data.SQLite2.Menus.MenuRepository());
			this.menuRepository = new MenuRepository();
		}

		public async Task<bool> GetMobileMenu()
		{
			try
			{
				MobileMenu menu = await this.menuService.GetMobileMenuAsync(menuRepository);
					
				AppData.MobileMenu = menu;
				await SaveMenuLocally();

				AppData.MobileMenuWasLoadedFromServer = true;

				return true;
			}
			catch(Exception ex) 
			{
				HandleException(ex, "MenuModel.GetMobileMenu()", false);
				return false;
			}
		}

		private async Task SaveMenuLocally()
		{
			try
			{
				MobileMenu menu = await this.localMenuService.SyncMobileMenuAsync(AppData.MobileMenu);

				System.Diagnostics.Debug.WriteLine("MobileMenu saved locally");
			}
				
			catch(Exception exception) 
			{
				HandleException(exception, "MenuModel.SaveMenuLocally()", false);
			}
		}

		public async Task LoadLocalMenuAsync()
		{
			try
			{
				MobileMenu menu = await this.localMenuService.GetMobileMenuAsync().ConfigureAwait(false);

				if (menu != null)
				{
					AppData.MobileMenu = menu;
					System.Diagnostics.Debug.WriteLine("Local mobilemenu fetched successfully");
				}
			}
			catch(Exception ex) 
			{
				HandleException(ex, "MenuModel.LoadLocalMenu()", false);
			}
		}
	}
}