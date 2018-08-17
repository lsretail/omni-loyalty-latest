using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Hospitality.Menus;
using Newtonsoft.Json;
using Presentation.Utils;

namespace Presentation.Models
{
    public class MenuModel : BaseModel
    {
        private MenuService service;
        private IMenuRepository menuRepository;
        private LocalMenuService localMenuService;

        public MenuModel(Context context, IRefreshableActivity refreshableActivity = null)
            : base(context, refreshableActivity)
        {
            localMenuService = new LocalMenuService(new Infrastructure.Data.SQLite2.Menus.MenuRepository());
        }

        protected override void CreateService()
        {
            menuRepository = new MenuRepository();
            service = new MenuService();
        }

        public async void GetMenus()
        {
            Show(true);

            BeginWsCall();
            
            SetLoading(LoadingType.Menu, AppData.Status.Loading);

            try
            {
                var menu = await service.GetMobileMenuAsync(menuRepository);

                AppData.MobileMenu = menu;
                SaveMenu();

                SendBroadcast(BroadcastUtils.MenuUpdated);

                SetLoading(LoadingType.Menu, AppData.Status.None);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);
        }

        private async void SaveMenu()
        {
            try
            {
                await localMenuService.SyncMobileMenuAsync(AppData.MobileMenu);
            }
            catch (Exception ex)
            {
                HandleUIException(ex, false);
            }
        }
    }
}