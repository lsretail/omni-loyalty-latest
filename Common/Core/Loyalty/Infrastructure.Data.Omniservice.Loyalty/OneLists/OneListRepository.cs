using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.Services.Loyalty.OneLists;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.OneLists
{
    public class OneListRepository : BaseRepository, IOneListRepository
    {
        public List<OneList> OneListGetByCardId(string cardId, ListType listType, bool includeLines)
        {
            string methodName = "OneListGetByCardId";
            var jObject = new { cardId = cardId, listType = listType, includeLines = includeLines };
            return base.PostData<List<OneList>>(jObject, methodName);
        }

        public OneList OneListGetById(string oneListId, bool includeLines)
        {
            string methodName = "OneListGetById";
            var jObject = new { oneListId = oneListId, includeLines = includeLines };
            return base.PostData<OneList>(jObject, methodName);
        }

        public OneList OneListSave(OneList oneList, bool calculate)
        {
            string methodName = "OneListSave";
            var jObject = new { oneList = oneList, calculate = calculate };
            return base.PostData<OneList>(jObject, methodName);
        }

        public Order OneListCalculate(OneList oneList)
        {
            string methodName = "OneListCalculate";
            var jObject = new { oneList = oneList };
            return base.PostData<Order>(jObject, methodName);
        }

        public bool OneListDeleteById(string oneListId)
        {
            string methodName = "OneListDeleteById";
            var jObject = new { oneListId = oneListId };
            return base.PostData<bool>(jObject, methodName);
        }

        public OneList OneListItemModify(OneListItem item, bool remove, bool calculate)
        {
            string methodName = "OneListItemModify";
            var jObject = new { item = item, remove = remove, calculate = calculate };
            return base.PostData<OneList>(jObject, methodName);
        }

        public bool OneListLinking(string oneListId, string cardId, string email, LinkStatus status)
        {
            string methodName = "OneListLinking";
            var jObject = new { oneListId = oneListId, cardId = cardId, email = email, status = status };
            return base.PostData<bool>(jObject, methodName);
        }
    }
}
