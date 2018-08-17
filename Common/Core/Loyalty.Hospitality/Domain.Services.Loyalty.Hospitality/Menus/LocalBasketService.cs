using System;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus
{
    public class LocalBasketService
    {
        public LocalBasketService()
        {
        }

        public Basket GetBasket(ILocalBasketRepository repository, MobileMenu mobileMenu)
        {
            var basket = repository.GetBasket();

            if (mobileMenu != null && basket != null)
            {
                CalculateBasket(mobileMenu, basket);
            }

            return basket;
        }

        public Basket SyncBasket(ILocalBasketRepository repository, Basket basket)
        {
            repository.SaveBasket(basket);
            return basket;
        }

        public decimal GetBasketItemFullPrice(MobileMenu mobileMenu, BasketItem basketItem)
        {
            var menuService = new MenuService();

            var itemPrice = menuService.GetItemFullPrice(mobileMenu, basketItem.Item);

            return itemPrice * basketItem.Quantity;
        }

        public void CalculateBasket(MobileMenu mobileMenu, Basket basket)
        {
            var basketAmount = 0m;

            foreach (var item in basket.Items)
            {
                basketAmount += GetBasketItemFullPrice(mobileMenu, item);
            }

            basket.Amount = basketAmount;

            basket.State = Basket.BasketState.Dirty;
        }

        #region windows

        public async Task<Basket> GetBasketAsync(ILocalBasketRepository repository, MobileMenu mobileMenu)
        {
            return await Task.Run(() => GetBasket(repository, mobileMenu));
        }

        public async Task<Basket> SyncBasketAsync(ILocalBasketRepository repository, Basket basket)
        {
            return await Task.Run(() => SyncBasket(repository, basket));
        }

        #endregion


    }
}
