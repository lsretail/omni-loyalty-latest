using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
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

        public OneList OneListGetById(string oneListId, ListType listType, bool includeLines)
        {
            return repository.OneListGetById(oneListId, listType, includeLines);
        }

        public OneList OneListSave(OneList oneList, bool calculate)
        {
            return repository.OneListSave(oneList, calculate);
        }

        public Order OneListCalculate(OneList oneList)
        {
            return repository.OneListCalculate(oneList);
        }

        public bool OneListDeleteById(string oneListId, ListType listType)
        {
            return repository.OneListDeleteById(oneListId, listType);
        }

        public async Task<List<OneList>> OneListGetByCardIdAsync(string cardId, ListType listType, bool includeLines)
        {
            return await Task.Run(() => OneListGetByCardId(cardId, listType, includeLines));
        }

        public async Task<OneList> OneListGetByIdAsync(string oneListId, ListType listType, bool includeLines)
        {
            return await Task.Run(() => OneListGetById(oneListId, listType, includeLines));
        }

        public async Task<OneList> OneListSaveAsync(OneList oneList, bool calculate)
        {
            return await Task.Run(() => OneListSave(oneList, calculate));
        }

        public async Task<Order> OneListCalculateAsync(OneList oneList)
        {
            return await Task.Run(() => OneListCalculate(oneList));
        }

        public async Task<bool> OneListDeleteByIdAsync(string oneListId, ListType listType)
        {
            return await Task.Run(() => OneListDeleteById(oneListId, listType));
        }
    }
}
