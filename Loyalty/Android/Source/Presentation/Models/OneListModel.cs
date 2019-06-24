using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.Services.Loyalty.OneLists;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.OneLists;
using Presentation.Util;

namespace Presentation.Models
{
    public class OneListModel : BaseModel
    {
        private OneListService oneListService;

        public OneListModel(Context context, IRefreshableActivity refreshableActivity = null) : base(context, refreshableActivity)
        {
        }

        public async Task<List<OneList>> OneListGetByCardId(string cardId, ListType listType, bool includeLines)
        {
            BeginWsCall();
            return await oneListService.OneListGetByCardIdAsync(cardId, listType, includeLines);
        }

        public async Task<OneList> OneListGetById(string contactId, ListType listType, bool includeLines)
        {
            BeginWsCall();
            return await oneListService.OneListGetByIdAsync(contactId, listType, includeLines);
        }

        public async Task<OneList> OneListSave(OneList oneList, bool calculate)
        {
            BeginWsCall();
            oneList.StoreId = "S0013"; 
            return await oneListService.OneListSaveAsync(oneList, calculate);
        }

        public async Task<bool> OneListDeleteById(string oneListId, ListType listType)
        {
            BeginWsCall();
            return await oneListService.OneListDeleteByIdAsync(oneListId, listType);
        }

        public async Task<Order> OneListCalculate(OneList oneList)
        {
            ShowIndicator(true);
            AppData.Basket.State = BasketState.Calculating;
            oneList.StoreId = "S0013";
            oneList.CardId = AppData.Device.CardId;
            BeginWsCall();
            Order order = await oneListService.OneListCalculateAsync(oneList);

            if (AppData.Basket.State == BasketState.Calculating)
            {
                AppData.Basket.State = BasketState.Normal;
                SendBroadcast(Utils.BroadcastUtils.BasketStateUpdated);
            }

            ShowIndicator(false);
            return order;
        }

        protected override void CreateService()
        {
            oneListService = new OneListService(new OneListRepository());
        }
    }
}