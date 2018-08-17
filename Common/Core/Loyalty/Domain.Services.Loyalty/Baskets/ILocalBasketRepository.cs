using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace LSRetail.Omni.Domain.Services.Loyalty.Baskets
{
	public interface ILocalBasketRepository
	{
		OneList GetBasket();
		void SaveBasket(OneList basket);
	}
}
