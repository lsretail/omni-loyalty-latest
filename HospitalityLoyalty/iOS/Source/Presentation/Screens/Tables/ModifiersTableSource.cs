using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using Presentation.Utils;
using System.Linq;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;

namespace Presentation.Screens
{
	public class ModifiersTableSource : UITableViewSource
	{
		private List<TableSection> TableSections;
		private MenuItem menuItem;
		private MenuService menuService;
		private nfloat headerHeight = 34f;
		private bool onlyShowRequiredModifiers;
		private bool showAddToBasketFooterView;

		public bool ContainsData { get { return (this.TableSections.Count > 0); } }

		private ModifiersView.IModifiersListeners listener;

		public ModifiersTableSource(ModifiersView.IModifiersListeners listener, MenuItem menuItem ,bool onlyShowRequiredModifiers, bool showAddToBasketFooterView)
		{
			this.listener = listener;
			this.menuItem = menuItem;
			this.onlyShowRequiredModifiers = onlyShowRequiredModifiers;
			this.showAddToBasketFooterView = showAddToBasketFooterView;
			this.TableSections = new List<TableSection> ();
			menuService = new MenuService();

			//testing
			if(menuService.HasAnyModifiers(AppData.MobileMenu, menuItem))
				BuildTableSections ();
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return this.TableSections.Count;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return (nint)this.TableSections[(int)section].cells.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return this.TableSections [indexPath.Section].cells [indexPath.Row];
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			// Handle checkboxcells here - tell the cell that it has been pressed

			UITableViewCell selectedCell = TableSections [indexPath.Section].cells [indexPath.Row];

			if (selectedCell is ModifiersTableCellCheckbox)
				(selectedCell as ModifiersTableCellCheckbox).CellPressed ();
			else if (selectedCell is DealLineItemTableCell)
				(selectedCell as DealLineItemTableCell).CellPressed();
		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return this.headerHeight;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			return BuildSectionsHeaderView(this.TableSections[(int)section].title);
		}

		public override nfloat GetHeightForFooter (UITableView tableView, nint section)
		{
			if(section == (this.TableSections.Count - 1) && this.showAddToBasketFooterView)
			{
				return 80f;
			}
			else
			{
				return 5f;
			}
		}

		public override UIView GetViewForFooter (UITableView tableView, nint section)
		{
			if(section == (this.TableSections.Count - 1) && this.showAddToBasketFooterView)
			{
				return BuildAddToBasketFooterView();
			}
			else
			{
				return new UIView();
			}
		}

		private void BuildTableSections()
		{
			if (this.menuItem is Product)
			{
				LoadProductToTableSection(this.menuItem as Product);
			} 
			else if (this.menuItem is Recipe) 
			{
				LoadRecipeToTableSection(this.menuItem as Recipe);
			} 
			else if (this.menuItem is MenuDeal) 
			{
				LoadDealToTableSection(this.menuItem as MenuDeal);
			}
		}

		private UIView BuildSectionsHeaderView(string title)
		{
			UIView headerView = new UIView();

			UILabel lblTitle = new UILabel();
			lblTitle.Font = UIFont.BoldSystemFontOfSize(16f);
			lblTitle.TextColor = UIColor.DarkGray;
			lblTitle.Text = title.ToUpper();
			headerView.AddSubview(lblTitle);

			nfloat margin = 10f;

			headerView.ConstrainLayout(() =>

				lblTitle.Frame.Top == headerView.Bounds.Top &&
				lblTitle.Frame.Left == headerView.Bounds.Left + margin &&
				lblTitle.Frame.Right == headerView.Bounds.Right &&
				lblTitle.Frame.Height == this.headerHeight
			);

			return headerView;
		}

		private UIView BuildAddToBasketFooterView()
		{
			UIView footerView = new UIView();

			UIButton btnAddToBasket = new UIButton();
			btnAddToBasket.SetTitle(LocalizationUtilities.LocalizedString("ItemDetails_AddToBasket", "Add to basket"), UIControlState.Normal);  
			btnAddToBasket.SetTitleColor(UIColor.White, UIControlState.Normal);
			btnAddToBasket.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnAddToBasket.Layer.CornerRadius = 2;
			btnAddToBasket.TouchUpInside += (object sender, EventArgs e) => {
				this.listener.AddToBasketButtonClicked();
			};

			UIImageView btnAddToBasketImageView = new UIImageView();
			btnAddToBasketImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnAddToBasketImageView.Image = Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.White);
			btnAddToBasketImageView.BackgroundColor = UIColor.Clear;
			btnAddToBasket.AddSubview(btnAddToBasketImageView);

			footerView.AddSubview(btnAddToBasket);

			nfloat xMargin = 20f;

			footerView.ConstrainLayout(() =>

				btnAddToBasket.Frame.Top == footerView.Bounds.Top + xMargin &&
				btnAddToBasket.Frame.Left == footerView.Bounds.Left + xMargin &&
				btnAddToBasket.Frame.Right == footerView.Bounds.Right - xMargin &&
				btnAddToBasket.Frame.Height == 40f

			);

			return footerView;
		}

		private void LoadProductToTableSection (Product myProduct)
		{
			// Do something?
			// No, we add products directly to the basket. They shouldn't have any modifiers.

			if (menuService.HasAnyModifiers(AppData.MobileMenu, myProduct))
				System.Diagnostics.Debug.WriteLine("This product has modifiers! Should show them ...");
		}

		private void LoadRecipeToTableSection (Recipe myRecipe)
		{
			// Recipes can contain ingredients and recipeLines, which in turn contain productmodifiergroups which contain productmodifiers

			TableSection section;

			if (myRecipe.Ingredients.Count > 0 && !this.onlyShowRequiredModifiers) 
			{
				section = new TableSection ();
				section.title = LocalizationUtilities.LocalizedString("Modifiers_Ingredients", "Ingredients");	// TODO Get this title from webservice data somehow?

				foreach (Ingredient ingredient in myRecipe.Ingredients) 
				{
					IngredientItem item = menuService.GetItem(AppData.MobileMenu, ingredient.Id);
					if (item == null)
						continue;
					
					if (ingredient.GetModifierType() == ModifierType.None) //if (ingredient.MaxQty == ingredient.MinQty)
					{
						section.cells.Add (new ModifiersTableCell (item.Description, ingredient));
					} 
					else if (ingredient.GetModifierType() == ModifierType.Checkbox) //else if (ingredient.MaxQty == 1 && ingredient.MinQty == 0) 
					{
						section.cells.Add (new ModifiersTableCellCheckbox (this.listener, item.Description, ingredient, null));
					} 
					else if (ingredient.GetModifierType() == ModifierType.Counter) 
					{
						section.cells.Add (new ModifiersTableCellStepper (this.listener, item.Description, ingredient, null));
					}
				}

				this.TableSections.Add (section);
			}

			foreach (ProductModifierGroup prodModGroup in myRecipe.ProductModifierGroups)
			{
				if (!ShouldShowModifierGroup(prodModGroup))
					continue;

				section = new TableSection ();
				section.title = prodModGroup.Description;

				foreach (ProductModifier prodMod in prodModGroup.ProductModifiers) 
				{
					
					/* TODO Might be a refactoring bug.
					 if (prodMod.Item == null)
						continue;
						*/
					if (!ShouldShowModifier(prodMod, prodModGroup))
						continue;

					if (prodMod.GetModifierType(prodModGroup) == ModifierType.None)//if (prodMod.MaxQty == prodMod.MinQty)
					{
						section.cells.Add (new ModifiersTableCell(prodMod.Description, prodMod));
					} 
					else if (prodMod.GetModifierType(prodModGroup) == ModifierType.Checkbox)//else if (prodMod.MaxQty == 1 && prodMod.MinQty == 0) 
					{
						if (prodModGroup.Selected == 0 && prodModGroup.MinimumSelection == 1)
						{
							// No item selected, lets select the default one
							if (prodMod.Id == prodModGroup.DefaultItemId)
								prodMod.Quantity = prodMod.MaximumSelection;
						}

						section.cells.Add (new ModifiersTableCellCheckbox (this.listener, prodMod.Description, prodMod, prodModGroup));
					}
					else if (prodMod.GetModifierType(prodModGroup) == ModifierType.Radio)
					{
						if (prodModGroup.Selected == 0 && prodModGroup.MinimumSelection == 1)
						{
							// No item selected, lets select the default one
							if (prodMod.Id == prodModGroup.DefaultItemId)
								prodMod.Quantity = prodMod.MaximumSelection;
						}

						section.cells.Add (new ModifiersTableCellCheckbox (this.listener, prodMod.Description, prodMod, prodModGroup));
					} 
					else if (prodMod.GetModifierType(prodModGroup) == ModifierType.Counter)
					{
						section.cells.Add (new ModifiersTableCellStepper (this.listener, prodMod.Description, prodMod, prodModGroup));
					}
				}

				this.TableSections.Add (section);
			}

		}

		private void LoadDealToTableSection(MenuDeal myDeal)
		{
			// Deals contain deallines ... each dealline contains a list of deallineitems and dealmodifier groups that we want to display, each deallineitem can also
			// contain its own modifiergroups that we also want to display
			// A deallineitem contains a menuitem and priceadjustment

			// TODO:
			// UITableView can't handle more than two levels of nesting, section -> row
			// Now we just show every dealLine in the same tableview ... we should use nested tableviews or a pagingviewcontroller or something?
			// At least separate the dealLines in the tableview clearly

			TableSection section;

			foreach (MenuDealLine dealLine in myDeal.DealLines)
			{
				if (dealLine.DealLineItems.Count > 0)
				{
					section = new TableSection ();
					section.title = dealLine.Description;

					// TODO: Remove. This is now handled in core.
					// Let's select the default item if no item is selected
					//					if (string.IsNullOrEmpty(dealLine.SelectedId))
					//						dealLine.SelectedId = dealLine.DefaultItemId;
					//					// If there still isn't any item selected (i.e. no default item) let's select the first item
					//					if (string.IsNullOrEmpty(dealLine.SelectedId))
					//						dealLine.SelectedId = dealLine.DealLineItems.FirstOrDefault().DealLineMenuItem.Id;

					foreach (MenuDealLineItem dealLineItem in dealLine.DealLineItems)
					{
						if (dealLineItem.MenuItem == null)
							continue;

						section.cells.Add(new DealLineItemTableCell(this.listener, dealLineItem.MenuItem.Description, dealLineItem, dealLine));
					}

					this.TableSections.Add (section);
				}

				// Show modifiers for the selected deal line item
				MenuDealLineItem selectedDealLineItem = dealLine.DealLineItems.Where(x => x.MenuItem.Id == dealLine.SelectedId).FirstOrDefault();
				if (selectedDealLineItem != null && selectedDealLineItem.MenuItem != null)
				{
					if (selectedDealLineItem.MenuItem is Product)
						LoadProductToTableSection(selectedDealLineItem.MenuItem as Product);
					else if (selectedDealLineItem.MenuItem is Recipe)
						LoadRecipeToTableSection(selectedDealLineItem.MenuItem as Recipe);
					else if (selectedDealLineItem.MenuItem is MenuDeal)
						LoadDealToTableSection(selectedDealLineItem.MenuItem as MenuDeal);	// Note: Recursion! TODO: Can a deal contain another deal? Should we allow this?
				}

				foreach (DealModifierGroup dealModGroup in dealLine.DealModifierGroups)
				{
					if (!ShouldShowModifierGroup(dealModGroup))
						continue;

					section = new TableSection ();
					section.title = dealModGroup.Description;

					foreach (DealModifier dealMod in dealModGroup.DealModifiers) 
					{
						/* TODO: Might be a refactoring bug.
						if (dealMod.Item == null)
							continue;
						*/
						if (!ShouldShowModifier(dealMod, dealModGroup))
							continue;

						if (dealMod.GetModifierType(dealModGroup) == ModifierType.None)//if (dealMod.MaxQty == dealMod.MinQty)
						{
							section.cells.Add (new ModifiersTableCell(dealMod.Description, dealMod));
						} 
						else if (dealMod.GetModifierType(dealModGroup) == ModifierType.Checkbox)//else if (dealMod.MaxQty == 1 && dealMod.MinQty == 0) 
						{
							if (dealModGroup.Selected == 0 && dealModGroup.MinimumSelection == 1)
							{
								// No item selected, lets select the default one
								if (dealMod.Id == dealModGroup.DefaultItemId)
									dealMod.Quantity = dealMod.MaximumSelection;
							}
							section.cells.Add (new ModifiersTableCellCheckbox (this.listener,dealMod.Description, dealMod, dealModGroup));
						} 
						else if (dealMod.GetModifierType(dealModGroup) == ModifierType.Radio)
						{
							if (dealModGroup.Selected == 0 && dealModGroup.MinimumSelection == 1)
							{
								// No item selected, lets select the default one
								if (dealMod.Id == dealModGroup.DefaultItemId)
									dealMod.Quantity = dealMod.MaximumSelection;
							}
							section.cells.Add (new ModifiersTableCellCheckbox (this.listener,dealMod.Description, dealMod, dealModGroup));
						} 
						else if (dealMod.GetModifierType(dealModGroup) == ModifierType.Counter)
						{
							section.cells.Add (new ModifiersTableCellStepper (this.listener, dealMod.Description, dealMod, dealModGroup));
						}
					}

					this.TableSections.Add (section);
				}
			}
		}

		private bool ShouldShowModifier(Modifier modifier, ModifierGroup group)
		{
			if (!this.onlyShowRequiredModifiers)
				return true;

			if (modifier.IsSelectionRequired(group))
				return true;

			return false;
		}

		private bool ShouldShowModifierGroup(ModifierGroup group)
		{
			if (!this.onlyShowRequiredModifiers)
				return true;

			if (group.RequiredSelection)
				return true;

			if (group is ProductModifierGroup)
			{
				if ((group as ProductModifierGroup).ProductModifiers.Any(x => x.RequiredSelection))
					return true;
			}
			else if (group is DealModifierGroup)
			{
				if ((group as DealModifierGroup).DealModifiers.Any(x => x.RequiredSelection))
					return true;
			}

			return false;
		}

		public void RefreshCellDecorations()
		{
			foreach (TableSection section in this.TableSections)
			{
				foreach (UITableViewCell cell in section.cells)
				{
					if (cell is ModifiersTableCellCheckbox)
						(cell as ModifiersTableCellCheckbox).RefreshCheckmark ();
					if (cell is ModifiersTableCellStepper)
						(cell as ModifiersTableCellStepper).RefreshStepperValues ();
					if (cell is DealLineItemTableCell)
						(cell as DealLineItemTableCell).RefreshCheckmark();
				}
			}
		}

		public void RefreshTableData()
		{
			this.TableSections.Clear();
			BuildTableSections();
		}

		private class TableSection
		{
			public string title;
			public List<UITableViewCell> cells = new List<UITableViewCell> ();
		}
	}
}

