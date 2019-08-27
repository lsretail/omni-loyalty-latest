using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace LSRetail.Omni.Domain.Services.Loyalty.OneLists
{
    public class OneListService
    {
        private IOneListRepository repository;

        public OneListService(IOneListRepository iRepo)
        {
            repository = iRepo;
        }

        public List<OneList> OneListGetByCardId(string cardId, ListType listType, bool includeLines)
        {
            return repository.OneListGetByCardId(cardId, listType, includeLines);
        }

        public OneList OneListGetById(string oneListId, bool includeLines)
        {
            return repository.OneListGetById(oneListId, includeLines);
        }

        public OneList OneListSave(OneList oneList, bool calculate)
        {
            return repository.OneListSave(oneList, calculate);
        }

        public Order OneListCalculate(OneList oneList)
        {
            return repository.OneListCalculate(oneList);
        }

        public bool OneListDeleteById(string oneListId)
        {
            return repository.OneListDeleteById(oneListId);
        }

        public OneList OneListItemModify(OneListItem item, bool remove, bool calculate)
        {
            return repository.OneListItemModify(item, remove, calculate);
        }

        public bool OneListLinking(string oneListId, string cardId, string email, LinkStatus status)
        {
            return repository.OneListLinking(oneListId, cardId, email, status);
        }

        public OneList OneListAddItem(MemberContact contact, ListType type, string cardId, LoyItem item, string variantId, string uomId, decimal qty)
        {
            var list = OneListGetByCardId(cardId, type, true).FirstOrDefault();

            if (list == null)
            {
                return null;
            }

            list.AddItem(new OneListItem(item.Id, qty, uomId, variantId));

            OneListSave(list, false);

            return list;
        }

        public bool OneListRemoveItem(MemberContact contact, ListType type, string cardId, OneListItem oneListItem)
        {
            var list = OneListGetByCardId(cardId, type, true).FirstOrDefault();

            if (list == null)
            {
                return false;
            }

            var existingItem = list.Items.FirstOrDefault(x => x.HaveTheSameItemAndVariant(oneListItem));

            list.Items.Remove(existingItem);

            contact.AddList(contact.Cards[0].Id, OneListSave(list, false), ListType.Wish);
            return true;
        }

        public async Task<List<OneList>> OneListGetByCardIdAsync(string cardId, ListType listType, bool includeLines)
        {
            return await Task.Run(() => OneListGetByCardId(cardId, listType, includeLines));
        }

        public async Task<OneList> OneListGetByIdAsync(string oneListId, bool includeLines)
        {
            return await Task.Run(() => OneListGetById(oneListId, includeLines));
        }

        public async Task<OneList> OneListSaveAsync(OneList oneList, bool calculate)
        {
            return await Task.Run(() => OneListSave(oneList, calculate));
        }

        public async Task<Order> OneListCalculateAsync(OneList oneList)
        {
            return await Task.Run(() => OneListCalculate(oneList));
        }

        public async Task<bool> OneListDeleteByIdAsync(string oneListId)
        {
            return await Task.Run(() => OneListDeleteById(oneListId));
        }

        public async Task<OneList> OneListItemModifyAsync(OneListItem item, bool remove, bool calculate)
        {
            return await Task.Run(() => OneListItemModify(item, remove, calculate));
        }

        public async Task<bool> OneListLinkingAsync(string oneListId, string cardId, string email, LinkStatus status)
        {
            return await Task.Run(() => OneListLinking(oneListId, cardId, email, status));
        }

        public async Task<OneList> OneListAddItemAsync(MemberContact contact, ListType type, string cardId, LoyItem item, string variantId, string uomId, decimal qty)
        {
            return await Task.Run(() => OneListAddItem(contact, type, cardId, item, variantId, uomId, qty));
        }

        public async Task<bool> OneListRemoveItemAsync(MemberContact contact, ListType type, string cardId, OneListItem oneListItem)
        {
            return await Task.Run(() => OneListRemoveItem(contact, type, cardId, oneListItem));
        }
    }
}
