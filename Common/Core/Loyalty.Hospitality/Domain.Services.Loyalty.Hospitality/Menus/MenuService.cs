using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Menu;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus
{
	public class MenuService
	{
		public MenuService()
		{
		}

		public MobileMenu GetMobileMenu(IMenuRepository repository)
		{
			return repository.GetMobileMenu();
		}

		public List<MobileMenuNode> GetMobileMenuNodes(Menu menu)
		{
			var menuNodes = new List<MobileMenuNode>();
			foreach (var menuNode in menu.MenuNodes)
			{
				menuNodes.Add(new MobileMenuNode(menuNode.Id)
				{
					Description = menuNode.Description,
					DisplayOrder = menuNode.DisplayOrder,
					Image = menuNode.Image,
					MenuGroupNodes = menuNode.MenuGroupNodes,
					MenuNodeLines = menuNode.MenuNodeLines,
					NodeType = MobileMenuNodeType.Group,
					PriceGroup = menuNode.PriceGroup,
					ValidationEndTime = menuNode.ValidationEndTime,
					ValidationEndTimeAfterMidnight = menuNode.ValidationEndTimeAfterMidnight,
					ValidationStartTime = menuNode.ValidationStartTime,
					ValidationTimeWithinBounds = menuNode.ValidationTimeWithinBounds,
				});
			}

			return menuNodes;
		}

		public List<MobileMenuNode> GetMobileMenuNodes(MobileMenu mobileMenu, MenuNode menuNode)
		{
			var menuNodes = new List<MobileMenuNode>();
			foreach (var groupMenuNode in menuNode.MenuGroupNodes)
			{
				menuNodes.Add(new MobileMenuNode(menuNode.Id)
				{
					Description = groupMenuNode.Description,
					DisplayOrder = groupMenuNode.DisplayOrder,
					Image = groupMenuNode.Image,
					MenuGroupNodes = groupMenuNode.MenuGroupNodes,
					MenuNodeLines = groupMenuNode.MenuNodeLines,
					NodeType = MobileMenuNodeType.Group,
					PriceGroup = groupMenuNode.PriceGroup,
					ValidationEndTime = groupMenuNode.ValidationEndTime,
					ValidationEndTimeAfterMidnight = groupMenuNode.ValidationEndTimeAfterMidnight,
					ValidationStartTime = groupMenuNode.ValidationStartTime,
					ValidationTimeWithinBounds = groupMenuNode.ValidationTimeWithinBounds,
				});
			}

			foreach (var menuNodeLine in menuNode.MenuNodeLines)
			{
				var item = GetMenuItem(mobileMenu, menuNodeLine.Id, menuNodeLine.NodeLineType);
				menuNodes.Add(new MobileMenuNode(item.Id)
				{
					Description = item.Description,
					DisplayOrder = menuNodeLine.DisplayOrder,
					Image = (item.Images != null && item.Images.Count() > 0) ? item.Images[0] : new DataModel.Base.Retail.ImageView(),
					NodeType = MobileMenuNodeType.Item,
					NodeLineType = menuNodeLine.NodeLineType,
				});
			}

			return menuNodes.OrderBy(x => x.DisplayOrder).ToList();
		}

		public bool ContainsItem(Menu menu, string itemId, NodeLineType itemType)
		{
			foreach (var menuNode in menu.MenuNodes)
			{
				var exists = ContainsItem(menuNode, itemId, itemType);

				if (exists)
				{
					return true;
				}
			}

			return false;
		}

		public bool HasAnyModifiers(MobileMenu mobileMenu, MenuItem item)
		{
			if (item is MenuDeal)
			{
				var deal = (MenuDeal)item;

				foreach (var dealLine in deal.DealLines)
				{
					if (dealLine.DealLineItems.Count > 1)
					{
						return true;
					}

					foreach (var dealModifierGroup in dealLine.DealModifierGroups)
					{
						if (dealModifierGroup.DealModifiers.Count > 0)
						{
							return true;
						}
					}
				}
			}
			else if (item is Recipe)
			{
				var recipe = (Recipe)item;

				if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
				{
					return true;
				}

				foreach (var productModifierGroupId in recipe.ProductModifierGroupIds)
				{
					var productModifierGroup = GetProductModifierGroup(mobileMenu, productModifierGroupId);

					if (productModifierGroup.ProductModifiers.Count > 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool HasAnyRequiredModifers(MobileMenu mobileMenu, MenuItem item)
		{
			if (item is MenuDeal)
			{
				var deal = (MenuDeal)item;

				foreach (var dealLine in deal.DealLines)
				{
					if (dealLine.DealLineItems.Count > 1)
					{
						return true;
					}

					foreach (var dealModifierGroup in dealLine.DealModifierGroups)
					{
						if (dealModifierGroup.RequiredSelection)
						{
							return true;
						}
					}
				}
			}
			else if (item is Recipe)
			{
				var recipe = (Recipe)item;

				foreach (var productModifierGroupId in recipe.ProductModifierGroupIds)
				{
					var productModifierGroup = GetProductModifierGroup(mobileMenu, productModifierGroupId);

					if (productModifierGroup.RequiredSelection)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool HasAllRequiredModifiers(MobileMenu mobileMenu, MenuItem item)
		{
			if (item is MenuDeal)
			{
				var deal = (MenuDeal)item;

				foreach (var dealLine in deal.DealLines)
				{
					foreach (var dealModifierGroup in dealLine.DealModifierGroups)
					{
						if (!dealModifierGroup.RequiredSelection)
						{
							return false;
						}
					}
				}
			}
			else if (item is Recipe)
			{
				var recipe = (Recipe)item;

				if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
				{
					return false;
				}

				foreach (var productModifierGroupId in recipe.ProductModifierGroupIds)
				{
					var productModifierGroup = GetProductModifierGroup(mobileMenu, productModifierGroupId);

					if (!productModifierGroup.RequiredSelection)
					{
						return false;
					}
				}
			}

			return true;
		}

		public decimal GetItemFullPrice(MobileMenu mobileMenu, MenuItem item)
		{
			var price = item.Price.Value;

			if (item is MenuDeal)
			{
				var deal = (MenuDeal)item;

				foreach (var dealLine in deal.DealLines)
				{
					var dealLinePriceAdjustment = 0m;

					foreach (var dealModifierGroup in dealLine.DealModifierGroups)
					{
						foreach (var dealModifier in dealModifierGroup.DealModifiers)
						{
							dealLinePriceAdjustment += dealModifier.Price * (dealModifier.Quantity - dealModifier.OriginalQty);
						}
					}

					var dealLineItem = dealLine.DealLineItems.FirstOrDefault(x => x.ItemId == dealLine.SelectedId);

					if (dealLineItem != null)
					{
						dealLinePriceAdjustment += dealLineItem.Quantity * dealLineItem.PriceAdjustment;

						var recipe = GetRecipe(mobileMenu, dealLineItem.ItemId);

						if (recipe != null)
						{
							foreach (var productModifierGroupId in recipe.ProductModifierGroupIds)
							{
								var productModifierGroup = GetProductModifierGroup(mobileMenu, productModifierGroupId);

								foreach (var productModifier in productModifierGroup.ProductModifiers)
								{
									price += productModifier.Price * (productModifier.Quantity - productModifier.OriginalQty);
								}
							}
						}
					}

					price += dealLinePriceAdjustment;
				}

				return price;
			}
			else if (item is Recipe)
			{
				var recipe = (Recipe)item;

				foreach (var productModifierGroupId in recipe.ProductModifierGroupIds)
				{
					var productModifierGroup = GetProductModifierGroup(mobileMenu, productModifierGroupId);

					foreach (var productModifier in productModifierGroup.ProductModifiers)
					{
						price += productModifier.Price * (productModifier.Quantity - productModifier.OriginalQty);
					}
				}

				return price;
			}

			return item.Price.Value;
		}

		public MenuNode GetMenuGroupNode(Menu menu, string nodeId)
		{
			foreach (var menuNode in menu.MenuNodes)
			{
				var node = GetMenuGroupNode(menuNode, nodeId);

				if (node != null)
					return node;
			}

			return null;
		}

		public IngredientItem GetItem(MobileMenu menu, string itemId)
		{
			var item = menu.Items.FirstOrDefault(x => x.Id == itemId);

			if (item == null)
			{
				var product = GetProduct(menu, itemId);

				if (product != null)
				{
					item = new IngredientItem(itemId);
					item.Description = product.Description;
				}
				else
				{
					var recipe = GetRecipe(menu, itemId);

					if (recipe != null)
					{
						item = new IngredientItem(itemId);
						item.Description = recipe.Description;
					}
				}
			}

			return item;
		}

		public MenuItem GetMenuItem(MobileMenu menu, string itemId, bool searchInDeals)
		{
			MenuItem menuItem = null;

			if (searchInDeals)
			{
				menuItem = GetDeal(menu, itemId);
			}

			if (menuItem == null)
			{
				menuItem = GetProduct(menu, itemId);
				if (menuItem == null)
				{
					menuItem = GetRecipe(menu, itemId);
				}
			}
			return menuItem;
		}

		public MenuItem GetMenuItem(MobileMenu menu, string itemId, NodeLineType lineType)
		{
			MenuItem menuItem = null;

			if (lineType == NodeLineType.Deal)
			{
				menuItem = GetDeal(menu, itemId);
			}
			else if (lineType == NodeLineType.Recipe)
			{
				menuItem = GetRecipe(menu, itemId);
			}
			else if (lineType == NodeLineType.Product)
			{
				menuItem = GetProduct(menu, itemId);
			}

			return menuItem;
		}

		public ProductModifierGroup GetProductModifierGroup(MobileMenu mobileMenu, string productModifierGroupId)
		{
			var productModifierGroup = mobileMenu.ProductModifierGroups.FirstOrDefault(i => i.Id == productModifierGroupId);

			return productModifierGroup;
		}

		public DealModifierGroup GetDealModifierGroup(MobileMenu mobileMenu, string dealModifierGroupId)
		{
			var dealModifierGroup = mobileMenu.DealModifierGroups.FirstOrDefault(i => i.Id == dealModifierGroupId);

			return dealModifierGroup;
		}

		public NodeLineType GetItemType(MenuItem item)
		{
			if (item is MenuDeal)
			{
				return NodeLineType.Deal;
			}
			else if (item is Recipe)
			{
				return NodeLineType.Recipe;
			}
			return NodeLineType.Product;
		}

		#region private

		private bool ContainsItem(MenuNode menuNode, string itemId, NodeLineType itemType)
		{
			foreach (var menuGroupNode in menuNode.MenuGroupNodes)
			{
				var exists = ContainsItem(menuGroupNode, itemId, itemType);

				if (exists)
				{
					return true;
				}
			}

			foreach (var menuNodeMenuNodeLine in menuNode.MenuNodeLines)
			{
				if (menuNodeMenuNodeLine.Id == itemId && menuNodeMenuNodeLine.NodeLineType == itemType)
				{
					return true;
				}
			}

			return false;
		}

		private MenuNode GetMenuGroupNode(MenuNode menuNode, string nodeId)
		{
			if (menuNode.NodeIsItem && menuNode.Id == nodeId)
				return menuNode;

			if (!menuNode.NodeIsItem && menuNode.MenuGroupNodes != null)
			{
				foreach (var menuGroupNode in menuNode.MenuGroupNodes)
				{
					var node = GetMenuGroupNode(menuGroupNode, nodeId);

					if (node != null && !node.NodeIsItem)
						return node;
				}
			}

			return null;
		}

		private Product GetProduct(MobileMenu menu, string itemId)
		{
			Product product = menu.Products.FirstOrDefault(i => i.Id == itemId);

			return product;
		}

		private Recipe GetRecipe(MobileMenu menu, string itemId)
		{
			Recipe recipe = menu.Recipes.FirstOrDefault(i => i.Id == itemId);

			if (recipe != null)
			{
				recipe = (Recipe)recipe.Clone();

				foreach (var productModifierGroupId in recipe.ProductModifierGroupIds)
				{
					var productModifierGroup = GetProductModifierGroup(menu, productModifierGroupId).Clone();

					recipe.ProductModifierGroups.Add(productModifierGroup);
				}
			}

			return recipe;
		}

		private MenuDeal GetDeal(MobileMenu menu, string itemId)
		{
			MenuDeal deal = menu.Deals.FirstOrDefault(i => i.Id == itemId);

			if (deal == null)
			{
				deal = new MenuDeal(itemId);
			}
			else
			{
				deal = (MenuDeal)deal.Clone();

				foreach (var dealLine in deal.DealLines)
				{
					foreach (var dealModifierGroupId in dealLine.DealModifierGroupIds)
					{
						var dealModifierGroup = GetDealModifierGroup(menu, dealModifierGroupId).Clone();

						if (dealModifierGroup.MinimumSelection == 1 && dealModifierGroup.MaximumSelection == 1 && dealModifierGroup.DealModifiers.Count > 0)
						{
							dealModifierGroup.DealModifiers[0].Quantity = 1;
						}

						dealLine.DealModifierGroups.Add(dealModifierGroup);
					}

					foreach (var dealLineItem in dealLine.DealLineItems)
					{
						dealLineItem.MenuItem = GetMenuItem(menu, dealLineItem.ItemId, false);
					}
				}
			}

			return deal;
		}

		#endregion

		#region Async

		public async Task<MobileMenu> GetMobileMenuAsync(IMenuRepository repository)
		{
			return await Task.Run(() => GetMobileMenu(repository));
		}

		#endregion
	}
}
