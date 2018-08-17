using System;
using System.Linq;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Models;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class ModifiersController : UIViewController, ModifiersView.IModifiersListeners
	{
		private ModifiersView rootView;
		protected decimal basketItemQuantity;
		private bool onlyShowRequiredModifiers;
		private bool showAddToBasketBtn;

		private MenuItem menuItemToModify;
		public MenuItem MenuItem { get { return this.menuItemToModify; } }
		public MenuService menuService;

		public delegate void AddToBasketEventHandler();
		public event AddToBasketEventHandler OnAddToBasketPressed;

		public ModifiersController(MenuItem menuItem, bool onlyShowRequiredModifiers, bool showAddToBasketBtn)
		{
			menuService = new MenuService();
			this.rootView = new ModifiersView(this);
			this.menuItemToModify = menuItem;
			this.onlyShowRequiredModifiers = onlyShowRequiredModifiers;
			this.showAddToBasketBtn = showAddToBasketBtn;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.View = this.rootView;
			this.rootView.UpdateData(this.menuItemToModify, this.basketItemQuantity, this.onlyShowRequiredModifiers, this.showAddToBasketBtn);
		}

		public decimal GetFullPrice(MenuItem item)
		{
			return menuService.GetItemFullPrice(AppData.MobileMenu, item) * basketItemQuantity;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			this.rootView.ToggleNoModifiersView();
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		public void ChangeObjectQuantity(int newQuantity, object myObject, object myObjectGroup = null)
		{
			if (myObject is Modifier && myObjectGroup is ModifierGroup)
			{
				ChangeModifierQuantity(myObject as Modifier, myObjectGroup as ModifierGroup, newQuantity);
			}
			else if (myObject is Ingredient)
			{
				ChangeIngredientQuantity(myObject as Ingredient, newQuantity);
			}
			else if (myObject is MenuDealLineItem && myObjectGroup is MenuDealLine)
			{
				SelectDealLineItem(myObject as MenuDealLineItem, myObjectGroup as MenuDealLine);
				this.rootView.RefreshTableView();
			}

			this.rootView.RefreshPriceLabel(AppData.MobileMenu.Currency.FormatDecimal(menuService.GetItemFullPrice(AppData.MobileMenu, MenuItem)));
		}

		private void SelectDealLineItem(MenuDealLineItem item, MenuDealLine dealLine)
		{
			// TODO ?
			// Note: There is no way to skip a deal line item (i.e. select no item) with the current implementation (or select more than one)
			// Should change the domain model, give deal line items a quantity and use that instead?

			dealLine.SelectedId = item.MenuItem.Id;
		}

		private void ChangeModifierQuantity(Modifier modifier, ModifierGroup group, decimal newQuantity)
		{
			if (modifier.GetModifierType(group) == ModifierType.Radio)
			{
				// We have to mimic the behaviour of radio buttons
				// i.e. deselect all other modifiers in the group before selecting the specified item

				if (group is ProductModifierGroup && modifier is ProductModifier)
				{
					foreach (Modifier mod in (group as ProductModifierGroup).ProductModifiers.Where(x => x != (modifier as ProductModifier)).ToList())
					{
						mod.Quantity = mod.MinimumSelection;
					}
				}
				else if (group is DealModifierGroup && modifier is DealModifier)
				{
					foreach (Modifier mod in (group as DealModifierGroup).DealModifiers.Where(x => x != (modifier as DealModifier)).ToList())
					{
						mod.Quantity = mod.MinimumSelection;
					}
				}
			}

			decimal verifiedNewQuantity = group.NewQty(modifier, newQuantity);
			modifier.Quantity = verifiedNewQuantity;
			System.Diagnostics.Debug.WriteLine("New modifier quantity :" + modifier.Quantity.ToString());
		}

		private void ChangeIngredientQuantity(Ingredient ingredient, decimal newQuantity)
		{
			decimal verifiedNewQty = newQuantity > ingredient.MaximumQuantity ? ingredient.MaximumQuantity : (newQuantity < ingredient.MinimumQuantity ? ingredient.MinimumQuantity : newQuantity);//ingredient.NewQty(newQuantity);
			ingredient.Quantity = verifiedNewQty;
			System.Diagnostics.Debug.WriteLine("Ingredient quantity set: " + ingredient.Id + " QTY " + ingredient.Quantity.ToString());
		}

		public void IncreaseBasketItemQuantity()
		{
			this.basketItemQuantity++;
			this.rootView.RefreshBasketItemQuantityLabel(this.basketItemQuantity.ToString());
			this.rootView.RefreshPriceLabel(AppData.MobileMenu.Currency.FormatDecimal(GetFullPrice(menuItemToModify)));
		}

		public void DecreaseBasketItemQuantity()
		{
			if (this.basketItemQuantity > 1)
				this.basketItemQuantity--;
			this.rootView.RefreshBasketItemQuantityLabel(this.basketItemQuantity.ToString());
			this.rootView.RefreshPriceLabel(AppData.MobileMenu.Currency.FormatDecimal(GetFullPrice(menuItemToModify)));
		}

		public void LoadImageToItemImageView(string imageId, Action<ImageView> onSuccess)
		{
			new ImageModel().ImageGetById(imageId, new ImageSize(700, 500),
				(x, destinationId) =>
				{
					onSuccess(x);
				},
			  	() => { }
			);
		}

		public void AddToBasketButtonClicked() 
		{
			OnAddToBasketPressed?.Invoke();
		}
	}
}