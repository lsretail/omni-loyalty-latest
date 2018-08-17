using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace LSRetail.Omni.Domain.Services.Loyalty.Baskets
{
	public class LocalBasketService
	{
		private ILocalBasketRepository repository;

		public LocalBasketService(ILocalBasketRepository iRepo)
		{
			repository = iRepo;
		}

		public OneList GetBasket()
		{
			return repository.GetBasket();
		}

		public OneList SyncBasket(OneList basket)
		{
			repository.SaveBasket(basket);
			return basket;
		}

		public async Task<OneList> GetBasketAsync()
        {
            return await Task.Run(() => GetBasket());
		}

		public async Task<OneList> SyncBasketAsync(OneList basket)
		{
            return await Task.Run(() => SyncBasket(basket));
		}
	}
}
