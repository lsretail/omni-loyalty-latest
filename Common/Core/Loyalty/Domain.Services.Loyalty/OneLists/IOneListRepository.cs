using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace LSRetail.Omni.Domain.Services.Loyalty.OneLists
{
    public interface IOneListRepository
    {
        List<OneList> OneListGetByCardId(string cardId, ListType listType, bool includeLines);
        OneList OneListGetById(string oneListId, bool includeLines);
        OneList OneListSave(OneList oneList, bool calculate);
        Order OneListCalculate(OneList oneList);
        bool OneListDeleteById(string oneListId);
        OneList OneListItemModify(string onelistId, OneListItem item, bool remove, bool calculate);
        bool OneListLinking(string oneListId, string cardId, string email, LinkStatus status);
    }
}
