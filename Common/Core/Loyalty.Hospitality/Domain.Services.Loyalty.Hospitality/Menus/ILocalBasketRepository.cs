using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus
{
    public interface ILocalBasketRepository
    {
        Basket GetBasket();
        void SaveBasket(Basket basket);
    }
}
