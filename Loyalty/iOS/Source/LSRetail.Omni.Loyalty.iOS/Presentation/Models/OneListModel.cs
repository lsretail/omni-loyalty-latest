using System;
using System.Collections.Generic;

using Presentation.Models;
using System.Threading.Tasks;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.OneLists;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.Services.Loyalty.OneLists;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace Presentation
{
    public class OneListModel : BaseModel
    {
        OneListService service;

        public OneListModel()
        {
            service = new OneListService(new OneListRepository());
        }

        public async Task<List<OneList>> OneListGetByCardId(string cardId, ListType listType, bool includeLines)
        {
            try
            {
                return await this.service.OneListGetByCardIdAsync(cardId, listType, includeLines);
            }
            catch (Exception ex)
            {
                base.HandleException(ex, "OneListModel.OneListGetByContactId", false);
                throw;
            }
        }

        public async Task<OneList> OneListGetById(string contactId, ListType listType, bool includeLines)
        {
            try
            {
                return await this.service.OneListGetByIdAsync(contactId, listType, includeLines);
            }
            catch (Exception ex)
            {
                base.HandleException(ex, "OneListModel.OneListGetByContactId", false);
                throw;
            }
        }

        public async Task<OneList> OneListSave(OneList oneList, bool calculate)
        {
            try
            {
                oneList.StoreId = "S0013";
                return await this.service.OneListSaveAsync(oneList, calculate);
            }
            catch (Exception ex)
            {
                base.HandleException(ex, "OneListModel.OneListSave", false);
                throw;
            }
        }

        public async Task<Order> OneListCalculate(OneList oneList)
        {
            try
            {
                return await this.service.OneListCalculateAsync(oneList);
            }
            catch (Exception ex)
            {
                base.HandleException(ex, "OneListModel.OneListCalculate", false);
                throw;
            }
        }

        public async Task<bool> OneListDeleteById(string oneListId, ListType listType)
        {
            try
            {
                return await this.service.OneListDeleteByIdAsync(oneListId, listType);
            }
            catch (Exception ex)
            {
                base.HandleException(ex, "OneListModel.OneListDeleteById()", false);
                return false;
            }
        }
    }
}
