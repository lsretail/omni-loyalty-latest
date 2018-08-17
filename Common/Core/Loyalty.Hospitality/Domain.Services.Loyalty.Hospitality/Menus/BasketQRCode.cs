using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus
{
	public class BasketQrCode
	{
	    private MobileMenu mobileMenu;

		private List<BasketQrCodePublishedOffer> publishedOffers; 
	    private List<BasketQrCodeProduct> products;
	    private List<BasketQrCodeRecipe> recipes;
        private List<BasketQrCodeDeal> deals;
	    private BasketQrCodeContact contact;
	    private BasketQrCodeCard card;

        public BasketQrCode(MobileMenu mobileMenu)
	    {
	        this.mobileMenu = mobileMenu;

			publishedOffers = new List<BasketQrCodePublishedOffer>();
			products = new List<BasketQrCodeProduct>();
			recipes = new List<BasketQrCodeRecipe>();
			deals = new List<BasketQrCodeDeal>();
	    }

	    public List<PublishedOffer> PublishedOffers
		{
			set
			{
				publishedOffers.Clear();

				foreach (var offer in value)
				{
					publishedOffers.Add(new BasketQrCodePublishedOffer() { Id = offer.Id });
				}
			}
		}

        public List<BasketItem> Items
        {
            set
            {
                products.Clear();
                recipes.Clear();
                deals.Clear();

                foreach (var basketItem in value)
                {
                    if (basketItem.Item is Product)
                    {
                        products.Add(new BasketQrCodeProduct(basketItem.Item as Product, basketItem.Quantity));
                    }
                    else if (basketItem.Item is Recipe)
                    {
                        recipes.Add(new BasketQrCodeRecipe(mobileMenu, basketItem.Item as Recipe, basketItem.Quantity));
                    }
                    else if (basketItem.Item is MenuDeal)
                    {
                        deals.Add(new BasketQrCodeDeal(mobileMenu, basketItem.Item as MenuDeal, basketItem.Quantity));
                    }
                }
            }
        } 

        public MemberContact Contact 
        { 
            set
            {
                contact = new BasketQrCodeContact()
                    {
                        Id = value.Id
                    };

                card = new BasketQrCodeCard()
                    {
                        Id = value.Card.Id
                    };
            } 
        }

		public string Serialize()
		{
			var builder = new StringBuilder();
			builder.Append("<order>");

			// Products
		    if (products.Count > 0)
		    {
                builder.Append("<products>");

		        foreach (var product in products)
		        {
                    builder.Append(product.Serialize());
		        }

                builder.Append("</products>");
		    }

			// Recipes
			if (recipes.Count > 0)
            {
                builder.Append("<recipes>");

                foreach (var recipe in recipes)
                {
                    builder.Append(recipe.Serialize());
                }

                builder.Append("</recipes>");
            }
			// Deals
			if (deals.Count > 0)
            {
                builder.Append("<deals>");

                foreach (var deal in deals)
                {
                    builder.Append(deal.Serialize());
                }

                builder.Append("</deals>");
            }
			//saletype
            //TODO

			// Offers
			if (publishedOffers.Count > 0)
			{
				builder.Append("<offers>");

				foreach (var basketQrCodePublishedOffer in publishedOffers)
				{
					builder.Append(basketQrCodePublishedOffer.Serialize());
				}

				builder.Append("</offers>");
			}

			// Contact
			if (contact != null)
			{
                builder.Append(contact.Serialize());
			}

			// Card
            if (card != null)
            {
                builder.Append(card.Serialize());
            }

			builder.Append("</order>");
			return builder.ToString();
		}
	}

    public class BasketQrCodeProduct
    {
        public string Id { get; set; }
        public decimal Qty { get; set; }

        public BasketQrCodeProduct(Product product, decimal qty)
        {
            Id = product.Id;
            Qty = qty;
        }

        public string Serialize()
        {
            return string.Format(@"<product><id>{0}</id><qty>{1}</qty></product>", Id, Qty.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class BasketQrCodeRecipe
    {
        private MenuService menuService;

        public string Id { get; set; }
        public decimal Qty { get; set; }
        public List<BasketQrCodeIngredient> Ingredients { get; set; }
        public List<BasketQrCodeProductModifierGroup> ProductModifierGroups { get; set; }

        public BasketQrCodeRecipe(MobileMenu mobileMenu, Recipe recipe, decimal qty)
        {
            menuService = new MenuService();

            Id = recipe.Id;
            Qty = qty;

            Ingredients = new List<BasketQrCodeIngredient>();
            foreach (var ingredient in recipe.Ingredients)
            {
                Ingredients.Add(new BasketQrCodeIngredient(ingredient));
            }

            ProductModifierGroups = new List<BasketQrCodeProductModifierGroup>();

            foreach (var productModifierGroupId in recipe.ProductModifierGroupIds)
            {
                ProductModifierGroups.Add(new BasketQrCodeProductModifierGroup(menuService.GetProductModifierGroup(mobileMenu, productModifierGroupId)));
            }
        }

        public string Serialize()
        {
            var builder = new StringBuilder();
            builder.Append("<recipe>");

            builder.Append(string.Format(@"<id>{0}</id><qty>{1}</qty>", Id, Qty.ToString(CultureInfo.InvariantCulture)));

            //ingredients
            if (Ingredients.Count > 0)
            {
                builder.Append("<ingredients>");

                foreach (var ingredient in Ingredients)
                {
                    builder.Append(ingredient.Serialize());
                }

                builder.Append("</ingredients>");
            }

            //productModifiers
            if (ProductModifierGroups.Count > 0)
            {
                builder.Append("<productmodifiergroups>");

                foreach (var productModifierGroup in ProductModifierGroups)
                {
                    builder.Append(productModifierGroup.Serialize());
                }

                builder.Append("</productmodifiergroups>");
            }

            builder.Append("</recipe>");
            return builder.ToString();
        }
    }

    public class BasketQrCodeIngredient
    {
        public string Id { get; set; }
        public decimal Qty { get; set; }

        public BasketQrCodeIngredient(Ingredient ingredient)
        {
            Id = ingredient.Id;
            Qty = ingredient.Quantity;
        }

        public string Serialize()
        {
            return string.Format("<ingredient><itemid>{0}</itemid><qty>{1}</qty></ingredient>", Id, Qty.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class BasketQrCodeProductModifierGroup
    {
        public string Id { get; set; }
        public List<BasketQrCodeProductModifier> ProductModifiers { get; set; }

        public BasketQrCodeProductModifierGroup(ProductModifierGroup productModifierGroup)
        {
            Id = productModifierGroup.Id;

            ProductModifiers = new List<BasketQrCodeProductModifier>();
            //productModifierGroup.ProductModifiers.ForEach(modifier => ProductModifiers.Add(new BasketQrCodeProductModifier(modifier)));

            foreach (var productModifier in productModifierGroup.ProductModifiers)
            {
                ProductModifiers.Add(new BasketQrCodeProductModifier(productModifier));
            }
        }

        public string Serialize()
        {
            var builder = new StringBuilder();
            builder.Append("<productmodifiergroup>");

            builder.Append(string.Format(@"<itemid>{0}</itemid>", Id));

            //deal lines
            if (ProductModifiers.Count > 0)
            {
                builder.Append("<productmodifiers>");

                foreach (var productModifier in ProductModifiers)
                {
                    builder.Append(productModifier.Serialize());
                }

                builder.Append("</productmodifiers>");
            }

            builder.Append("</productmodifiergroup>");
            return builder.ToString();
        }
    }

    public class BasketQrCodeProductModifier
    {
        public string Id { get; set; }
        public decimal Qty { get; set; }

        public BasketQrCodeProductModifier(ProductModifier productModifier)
        {
            Id = productModifier.Id;
            Qty = productModifier.Quantity;
        }

        public string Serialize()
        {
            return string.Format(@"<productmodifier><itemid>{0}</itemid><qty>{1}</qty></productmodifier>", Id, Qty.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class BasketQrCodeDeal
    {
        public string Id { get; set; }
        public decimal Qty { get; set; }
        public List<BasketQrCodeDealLine> DealLines { get; set; }

        public BasketQrCodeDeal(MobileMenu mobileMenu, MenuDeal deal, decimal qty)
        {
            Id = deal.Id;
            Qty = qty;

            DealLines = new List<BasketQrCodeDealLine>();
            //deal.DealLines.ForEach(dealLine => DealLines.Add(new BasketQrCodeDealLine(dealLine)));

            foreach (var dealLine in deal.DealLines)
            {
                DealLines.Add(new BasketQrCodeDealLine(mobileMenu, dealLine));
            }
        }

        public string Serialize()
        {
            var builder = new StringBuilder();
            builder.Append("<deal>");

            builder.Append(string.Format(@"<id>{0}</id><qty>{1}</qty>", Id, Qty.ToString(CultureInfo.InvariantCulture)));

            //deal lines
            if (DealLines.Count > 0)
            {
                builder.Append("<deallines>");

                foreach (var dealLine in DealLines)
                {
                    builder.Append(dealLine.Serialize());
                }

                builder.Append("</deallines>");
            }

            builder.Append("</deal>");
            return builder.ToString();
        }
    }

    public class BasketQrCodeDealLine
    {
        private MenuService menuService;

        public string Id { get; set; }
        public List<BasketQrCodeProduct> Products { get; set; }
        public List<BasketQrCodeRecipe> Recipes { get; set; }
        public List<BasketQrCodeDealModifier> DealModifiers { get; set; }

        public BasketQrCodeDealLine(MobileMenu mobileMenu, MenuDealLine dealLine)
        {
            menuService = new MenuService();

            Id = dealLine.Id;

            Products = new List<BasketQrCodeProduct>();
            Recipes = new List<BasketQrCodeRecipe>();
            foreach (var dealLineItem in dealLine.DealLineItems)
            {
                var menuItem = menuService.GetMenuItem(mobileMenu, dealLineItem.ItemId, true);
                int qty = 0;

                if (menuItem.Id == dealLine.SelectedId)
                    qty = 1;

                if (menuItem is Product)
                {
                    Products.Add(new BasketQrCodeProduct(menuItem as Product, qty));
                }
                else if (menuItem is Recipe)
                {
                    Recipes.Add(new BasketQrCodeRecipe(mobileMenu, menuItem as Recipe, qty));
                }
            }

            DealModifiers = new List<BasketQrCodeDealModifier>();
            foreach (var dealModifierGroup in dealLine.DealModifierGroups)
            {
                foreach (var dealModifier in dealModifierGroup.DealModifiers)
                {
                    DealModifiers.Add(new BasketQrCodeDealModifier(dealModifier));
                }
            }
        }

        public string Serialize()
        {
            var builder = new StringBuilder();
            builder.Append("<dealline>");

            builder.Append(string.Format(@"<id>{0}</id>", Id));

            //products
            if (Products.Count > 0)
            {
                builder.Append("<products>");

                foreach (var product in Products)
                {
                    builder.Append(product.Serialize());
                }

                builder.Append("</products>");
            }

            //recipes
            if (Recipes.Count > 0)
            {
                builder.Append("<recipes>");

                foreach (var recipe in Recipes)
                {
                    builder.Append(recipe.Serialize());
                }

                builder.Append("</recipes>");
            }

            //deal modifiers
            if (DealModifiers.Count > 0)
            {
                builder.Append("<dealmodifiers>");

                foreach (var dealModifier in DealModifiers)
                {
                    builder.Append(dealModifier.Serialize());
                }

                builder.Append("</dealmodifiers>");
            }

            builder.Append("</dealline>");
            return builder.ToString();
        }
    }

    public class BasketQrCodeDealModifier
    {
        public string Id { get; set; }
        public decimal Qty { get; set; }

        public BasketQrCodeDealModifier(DealModifier dealModifier)
        {
            Id = dealModifier.Id;
            Qty = dealModifier.Quantity;
        }

        public string Serialize()
        {
            return string.Format(@"<dealmodifier><itemid>{0}</itemid><qty>{1}</qty></dealmodifier>", Id, Qty.ToString(CultureInfo.InvariantCulture));
        }
    }

	public class BasketQrCodePublishedOffer
	{
		public string Id { get; set; }

		public string Serialize()
		{
			return string.Format(@"<publishedOffer><id>{0}</id></publishedOffer>", Id);
		}
	}

    public class BasketQrCodeContact
    {
        public string Id { get; set; }

        public string Serialize()
        {
            return string.Format(@"<loyaltycontact><id>{0}</id></loyaltycontact>", Id);
        }
    }
    
	public class BasketQrCodeCard
    {
        public string Id { get; set; }

        public string Serialize()
        {
            return string.Format(@"<loyaltycard><id>{0}</id></loyaltycard>", Id);
        }
    }

	public class OrderQRCode
	{
		public string Id { get; set; }

		public string Serialize()
		{
			return string.Format(@"<mobilehosploy><id>{0}</id></mobilehosploy>", Id);
		}
	}
}