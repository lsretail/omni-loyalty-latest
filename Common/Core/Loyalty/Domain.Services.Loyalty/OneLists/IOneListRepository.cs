using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace LSRetail.Omni.Domain.Services.Loyalty.OneLists
{
    public interface IOneListRepository
    {
        List<OneList> OneListGetByContactId(string contactId, ListType listType, bool includeLines);
        List<OneList> OneListGetByCardId(string cardId, ListType listType, bool includeLines);
        OneList OneListGetById(string oneListId, ListType listType, bool includeLines);
        OneList OneListSave(OneList oneList, bool calculate);
        Order OneListCalculate(OneList oneList);
        bool OneListDeleteById(string oneListId, ListType listType);
    }
}
