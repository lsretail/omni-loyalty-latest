using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Utils;

namespace Presentation.Adapters
{
    public class MenuItemModificationAdapter : BaseAdapter<SectionedListItem>, View.IOnClickListener
    {
        private MenuService menuService;
        private Context context;
        private MenuDeal deal;
        private MenuDealLine dealLine;
        private Recipe recipe;
        private bool required;

        private List<SectionedListItem> items = new List<SectionedListItem>();

        public MenuItemModificationAdapter(Context context, Recipe recipe, bool required)
        {
            menuService = new MenuService();

            this.context = context;
            this.recipe = recipe;
            this.required = required;

            CreateRecipeSectionItems(recipe);
        }

        public MenuItemModificationAdapter(Context context, MenuDealLine dealLine)
        {
            menuService = new MenuService();

            this.context = context;
            this.dealLine = dealLine;

            CreateDealLineSectionItems(dealLine);
        }

        public MenuItemModificationAdapter(Context context, MenuDeal deal, bool required = true)
        {
            menuService = new MenuService();

            this.context = context;
            this.deal = deal;
            this.required = required;

            CreateDealSectionItems(deal);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override SectionedListItem this[int position]
        {
            get { return items[position]; }
        }

        public override int GetItemViewType(int position)
        {
            return (int)GetLineType(position);
        }

        public override bool AreAllItemsEnabled()
        {
            return true;
        }

        public override bool IsEnabled(int position)
        {
            var lineType = GetLineType(position);

            if (lineType == LineTypes.Header || lineType == LineTypes.Counter || lineType == LineTypes.Normal)
            {
                return false;
            }
            return true;
        }

        public override int ViewTypeCount
        {
            get { return 5; }
        }

        private LineTypes GetLineType(int position)
        {
            var item = this[position];

            return GetLineType(item);
        }

        private LineTypes GetLineType(SectionedListItem item)
        {
            if (item is SectionedListHeaderItem)
            {
                return LineTypes.Header;
            }
            else if (item is SectionListLineItem)
            {
                if (item.SectionType == SectionTypes.Ingredient)
                {
                    var sectionListLineItem = (item as SectionListLineItem);
                    var ingredient = sectionListLineItem.Item as Ingredient;

                    var type = ingredient.GetModifierType();

                    if (type == ModifierType.Counter)
                    {
                        return LineTypes.Counter;
                    }
                    else if (type == ModifierType.Checkbox)
                    {
                        return LineTypes.CheckBox;
                    }
                }
                else if (item.SectionType == SectionTypes.MenuItem)
                {
                    return LineTypes.Radio;
                }
                else if (item.SectionType == SectionTypes.DealGroup || item.SectionType == SectionTypes.ProductGroup)
                {
                    var sectionListLineItem = (item as SectionListLineItem);
                    var modifier = sectionListLineItem.Item as Modifier;
                    var modifierGroup = sectionListLineItem.ItemParent as ModifierGroup;

                    var type = modifier.GetModifierType(modifierGroup);

                    if (type == ModifierType.Radio)
                    {
                        return LineTypes.Radio;
                    }
                    else if (type == ModifierType.Counter)
                    {
                        return LineTypes.Counter;
                    }
                    else if (type == ModifierType.Checkbox)
                    {
                        return LineTypes.CheckBox;
                    }
                }
            }

            return LineTypes.Normal;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this[position];

            if (item is SectionedListHeaderItem)
            {
                var header = item as SectionedListHeaderItem;

                if (convertView == null)
                {
                    var inflater = ((LayoutInflater) context.GetSystemService(Context.LayoutInflaterService));
                    convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.MenuItemModificationSectionHeader);
                }

                convertView.FindViewById<TextView>(Resource.Id.MenuItemModificationSectionHeaderDescription).Text = header.Description.ToUpper();
            }
            else
            {
                var lineItem = item as SectionListLineItem;
                var lineType = GetLineType(position);

                if (convertView == null)
                {
                    if (lineType == LineTypes.Normal)
                    {
                        var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                        convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ListViewItem);
                    }
                    else if (lineType == LineTypes.Counter)
                    {
                        var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                        convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ListViewCounterItem);

                        convertView.FindViewById<ImageButton>(Resource.Id.ListViewItemDecreaseQty).SetOnClickListener(this);
                        convertView.FindViewById<ImageButton>(Resource.Id.ListViewItemIncreaseQty).SetOnClickListener(this);
                    }
                    else if (lineType == LineTypes.CheckBox)
                    {
                        var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                        convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ListViewCheckboxItem);

                        convertView.SetOnClickListener(this);
                    }
                    else if (lineType == LineTypes.Radio)
                    {
                        var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                        convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ListViewRadioItem);

                        convertView.SetOnClickListener(this);
                    }
                }

                convertView.Tag = position;

                var description = convertView.FindViewById<TextView>(Resource.Id.ListViewItemDescription);
                var details = convertView.FindViewById<TextView>(Resource.Id.ListViewItemDetails);
                var qty = convertView.FindViewById<Button>(Resource.Id.ListViewItemChangeQty);
                var isChecked = convertView.FindViewById<CheckBox>(Resource.Id.ListViewItemChecked);
                var isSelected = convertView.FindViewById<RadioButton>(Resource.Id.ListViewItemSelected);

                if (lineItem.Item is Ingredient)
                {
                    var ingredient = lineItem.Item as Ingredient;

                    if (description != null)
                    {
                        var ingredientItem = menuService.GetItem(AppData.MobileMenu, ingredient.Id);
                        description.Text = ingredientItem.Description;
                    }    
                    if (details != null)
                    {
                        details.Visibility = ViewStates.Gone;
                    }
                    if (qty != null)
                    {
                        qty.Text = ingredient.Quantity.ToString("0" + CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator + "##");
                    }
                    if (isChecked != null)
                    {
                        isChecked.Checked = ingredient.Quantity == 1;
                    }
                }
                else if (lineItem.Item is Modifier)
                {
                    var modifier = lineItem.Item as Modifier;

                    if (description != null)
                    {
                        var modifierItem = menuService.GetItem(AppData.MobileMenu, modifier.Id);

                        if (string.IsNullOrEmpty(modifier.Description) && modifierItem != null)
                        {
                            description.Text = modifierItem.Description;
                        }
                        else
                        {
                            description.Text = modifier.Description;
                        }
                    }
                    if (details != null)
                    {
                        if (modifier.Price == 0m)
                        {
                            details.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            details.Text = AppData.FormatCurrency(modifier.Price);
                            details.Visibility = ViewStates.Visible;
                        }
                    }
                    if (qty != null)
                    {
                        qty.Text = modifier.Quantity.ToString("0" + CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator + "##");
                    }
                    if (isChecked != null)
                    {
                        isChecked.Checked = modifier.Quantity == 1;
                    }
                    if (isSelected != null)
                    {
                        isSelected.Checked = modifier.Quantity == 1;
                    }
                }
                else if (lineItem.Item is MenuDealLineItem)
                {
                    var dealLineItem = lineItem.Item as MenuDealLineItem;

                    if (description != null)
                    {
                        description.Text = string.Empty;

                        if (dealLineItem.Quantity > 1)
                        {
                            description.Text = dealLineItem.Quantity.ToString("0.##") + " ";
                        }

                        description.Text += dealLineItem.MenuItem.Description;
                    }
                    if (details != null)
                    {
                        if (dealLineItem.PriceAdjustment == 0m)
                        {
                            details.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            details.Text = AppData.FormatCurrency(dealLineItem.PriceAdjustment);
                            details.Visibility = ViewStates.Visible;
                        }
                    }
                    if (isSelected != null)
                    {
                        isSelected.Checked = lineItem.Selected;
                    }
                }
            }

            return convertView;
        }

        public void OnClick(View v)
        {
            var changePos = 0;

            switch (v.Id)
            {
                case Resource.Id.ListViewItemDecreaseQty:
                    changePos = (int)(v.Parent as ViewGroup).Tag;

                    var decreaseItem = items[changePos];

                    if (decreaseItem is SectionListLineItem)
                    {
                        var sectionLineItem = decreaseItem as SectionListLineItem;

                        if (sectionLineItem.Item is Ingredient)
                        {
                            ItemQtyChanged(sectionLineItem, (sectionLineItem.Item as Ingredient).Quantity - 1);
                        }
                        else if (sectionLineItem.Item is Modifier)
                        {
                            ItemQtyChanged(sectionLineItem, (sectionLineItem.Item as Modifier).Quantity - 1);
                        }
                    }

                    break;


                case Resource.Id.ListViewItemIncreaseQty:
                    changePos = (int)(v.Parent as ViewGroup).Tag;

                    var increaseItem = items[changePos];

                    if (increaseItem is SectionListLineItem)
                    {
                        var sectionLineItem = increaseItem as SectionListLineItem;

                        if (sectionLineItem.Item is Ingredient)
                        {
                            ItemQtyChanged(sectionLineItem, (sectionLineItem.Item as Ingredient).Quantity + 1);
                        }
                        else if (sectionLineItem.Item is Modifier)
                        {
                            ItemQtyChanged(sectionLineItem, (sectionLineItem.Item as Modifier).Quantity + 1);
                        }
                    }

                    break;

                case Resource.Id.ListViewItemCheckboxContainer:
                    changePos = (int)v.Tag;

                    var checkboxItem = items[changePos];

                    if (checkboxItem is SectionListLineItem)
                        ItemChecked(checkboxItem as SectionListLineItem);

                    break;

                case Resource.Id.ListViewItemRadioContainer:
                    changePos = (int)v.Tag;

                    var radioItem = items[changePos];

                    if (radioItem is SectionListLineItem)
                        ItemChecked(radioItem as SectionListLineItem);

                    break;
            }   
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();

            if (items != null && context != null)
            {
                BroadcastUtils.SendBroadcast(context, BroadcastUtils.ItemPriceChanged);
            }
        }

        private void ItemChecked(SectionListLineItem item)
        {
            var lineType = GetLineType(item);

            if (lineType == LineTypes.CheckBox)
            {
                if (item.SectionType == SectionTypes.Ingredient)
                {
                    var ingredient = item.Item as Ingredient;

                    if (ingredient != null)
                    {
                        ingredient.Quantity = (ingredient.Quantity == 1) ? 0 : 1;
                    }
                }
                else if (item.SectionType == SectionTypes.ProductGroup || item.SectionType == SectionTypes.DealGroup)
                {
                    if (item.ItemParent is ModifierGroup)
                    {
                        var productModifierGroup = item.ItemParent as ModifierGroup;
                        var productModifier = item.Item as Modifier;

                        if (productModifier != null)
                        {
                            if (productModifier.Quantity == 0)
                            {
                                if (!productModifierGroup.HasSelectionRestriction || productModifierGroup.Selected < productModifierGroup.MaximumSelection)
                                    productModifier.Quantity = 1;
                                else
                                {
                                    Toast.MakeText(context, string.Format(context.Resources.GetString(Resource.String.MenuItemMaximumSelected), productModifierGroup.MaximumSelection), ToastLength.Short).Show();
                                }
                            }
                            else
                            {
                                productModifier.Quantity = 0;
                            }
                        }
                    }
                }
            }
            else if (lineType == LineTypes.Radio)
            {
                if (item.SectionType == SectionTypes.ProductGroup || item.SectionType == SectionTypes.DealGroup)
                {
                    if (item.ItemParent is ModifierGroup)
                    {
                        var productModifierGroup = item.ItemParent as ModifierGroup;
                        var productModifier = item.Item as Modifier;

                        if (productModifier != null)
                        {
                            productModifierGroup.Reset();
                            productModifier.Quantity = 1;
                        }
                    }
                }
                else if (item.SectionType == SectionTypes.MenuItem)
                {
                    if (item.Item is MenuDealLineItem)
                    {
                        DealLineItemChanged((string) item.ItemParent, (item.Item as MenuDealLineItem).ItemId);
                    }
                }
            }

            NotifyDataSetChanged();
        }

        private void ItemQtyChanged(SectionListLineItem item, decimal newQty)
        {
            var lineType = GetLineType(item);

            if (lineType == LineTypes.Counter)
            {
                if (item.SectionType == SectionTypes.ProductGroup || item.SectionType == SectionTypes.DealGroup)
                {
                    if (item.ItemParent is ModifierGroup)
                    {
                        var productModifierGroup = item.ItemParent as ModifierGroup;
                        var productModifier = item.Item as Modifier;

                        if (productModifier != null)
                        {
                            var legalQty = productModifierGroup.NewQty(productModifier, newQty);

                            if (legalQty != newQty)
                            {
                                if (legalQty < newQty)
                                {
                                    var maxSelection = productModifierGroup.MaximumSelection;
                                    if (legalQty == productModifier.MaximumSelection)
                                    {
                                        maxSelection = legalQty;
                                    }

                                    Toast.MakeText(context, string.Format(context.Resources.GetString(Resource.String.MenuMaximumSelection), maxSelection), ToastLength.Short).Show();
                                }
                                else
                                {
                                    var minSelection = productModifierGroup.MinimumSelection;
                                    if (legalQty == productModifier.MinimumSelection)
                                    {
                                        minSelection = legalQty;
                                    }

                                    Toast.MakeText(context, string.Format(context.Resources.GetString(Resource.String.MenuMinimumSelection), minSelection), ToastLength.Short).Show();
                                }
                            }

                            productModifier.Quantity = legalQty;
                        }
                    }
                }
                else if (item.SectionType == SectionTypes.Ingredient)
                {
                    var ingredient = item.Item as Ingredient;

                    if (ingredient != null)
                    {
                        if (newQty > ingredient.MaximumQuantity)
                        {
                            ingredient.Quantity = ingredient.MaximumQuantity;
                            Toast.MakeText(context, string.Format(context.Resources.GetString(Resource.String.MenuMaximumSelection), ingredient.MaximumQuantity), ToastLength.Short).Show();
                        }
                        else if (newQty < ingredient.MinimumQuantity)
                        {
                            ingredient.Quantity = ingredient.MinimumQuantity;
                            Toast.MakeText(context, string.Format(context.Resources.GetString(Resource.String.MenuMinimumSelection), ingredient.MinimumQuantity), ToastLength.Short).Show();
                        }
                        else
                        {
                            ingredient.Quantity = newQty;
                        }
                    }
                }
            }

            NotifyDataSetChanged();
        }

        private void DealLineItemChanged(string dealLineId, string selectedId)
        {
            if (deal != null)
            {
                CreateDealSectionItems(deal, dealLineId, selectedId);
            }
            if (dealLine != null)
            {
                CreateDealLineSectionItems(dealLine, selectedId);
            }
        }

        private void CreateIngredientItems(Recipe recipe)
        {
            if (!required && (recipe.Ingredients != null && recipe.Ingredients.Count > 0))
            {
                items.Add(new SectionedListHeaderItem() { SectionType = SectionTypes.Ingredient, Description = context.Resources.GetString(Resource.String.MenuItemIngredients) });

                foreach (var ingredient in recipe.Ingredients)
                {
                    items.Add(new SectionListLineItem() { SectionType = SectionTypes.Ingredient, Item = ingredient });
                }
            }
        }

        private void CreateDealSectionItems(MenuDeal deal, string changedItemDealLineId = "", string changedItemSelectedId = "")
        {
            if(deal == null || deal.DealLines == null || deal.DealLines.Count == 0)
                return;

            for (int i = 0; i < deal.DealLines.Count; i++)
            {
                var clear = i == 0;
                var dealLine = deal.DealLines[i];
                string selectedId = string.Empty;

                if (dealLine.Id == changedItemDealLineId)
                {
                    selectedId = changedItemSelectedId;
                }

                CreateDealLineSectionItems(dealLine, selectedId, clear);
            }
        }

        private void CreateDealLineSectionItems(MenuDealLine dealLine, string selectedId = "", bool clear = true)
        {
            if (clear)
                items.Clear();

            if (dealLine != null && dealLine.DealLineItems != null && dealLine.DealLineItems.Count != 0)
            {
                if (string.IsNullOrEmpty(selectedId))
                {
                    //selectedId = dealLine.DealLineItems[0].DealLineMenuItem.Id;
                    selectedId = dealLine.SelectedId;
                }

                dealLine.SelectedId = selectedId;

                items.Add(new SectionedListHeaderItem()
                {
                    SectionType = SectionTypes.MenuItem,
                    Description = dealLine.Description
                });

                foreach (var dealLineItem in dealLine.DealLineItems)
                {
                    items.Add(new SectionListLineItem()
                    {
                        SectionType = SectionTypes.MenuItem,
                        Selected = dealLineItem.MenuItem.Id == selectedId,
                        Item = dealLineItem,
                        ItemParent = dealLine.Id
                    });
                }

                var selectedItem = dealLine.DealLineItems.FirstOrDefault(x => x.MenuItem.Id == selectedId);

                if (selectedItem != null && selectedItem.MenuItem is Recipe)
                {
                    CreateRecipeSectionItems(selectedItem.MenuItem as Recipe, false);
                }
            }

            foreach (var dealModifierGroup in dealLine.DealModifierGroups)
            {
                if (!required || dealModifierGroup.RequiredSelection)
                {
                    if (string.IsNullOrEmpty(dealModifierGroup.Description))
                    {
                        items.Add(new SectionedListHeaderItem() { SectionType = SectionTypes.DealGroup, Description = context.Resources.GetString(Resource.String.MenuItemModifiers) });
                    }
                    else
                    {
                        items.Add(new SectionedListHeaderItem() { SectionType = SectionTypes.DealGroup, Description = dealModifierGroup.Description });
                    }

                    foreach (var dealModifier in dealModifierGroup.DealModifiers)
                    {
                        //required does not affect deal modifiers
                        //if (!required || dealModifier.RequiredSelection)
                        {
                            items.Add(new SectionListLineItem()
                            {
                                SectionType = SectionTypes.DealGroup,
                                Item = dealModifier,
                                ItemParent = dealModifierGroup
                            });   
                        }
                    }   
                }
            }
        }

        private void CreateRecipeSectionItems(Recipe recipe, bool clear = true)
        {
            if(clear)
                items.Clear();

            CreateIngredientItems(recipe);
            
            foreach (var productModifierGroup in recipe.ProductModifierGroups)
            {
                if (!required || productModifierGroup.RequiredSelection)
                {
                    var productDescription = string.Empty;

                    if (!string.IsNullOrEmpty(recipe.Description))
                    {
                        productDescription = recipe.Description + " - ";
                    }

                    if (string.IsNullOrEmpty(productModifierGroup.Description))
                    {
                        items.Add(new SectionedListHeaderItem() { SectionType = SectionTypes.ProductGroup, Description = productDescription + context.Resources.GetString(Resource.String.MenuItemModifiers) });
                    }
                    else
                    {
                        items.Add(new SectionedListHeaderItem() { SectionType = SectionTypes.ProductGroup, Description = productDescription + productModifierGroup.Description });
                    }

                    foreach (var productModifier in productModifierGroup.ProductModifiers)
                    {
                        //required does not affect product modifiers
                        //if (!required || productModifier.RequiredSelection)
                        {
                            items.Add(new SectionListLineItem()
                            {
                                SectionType = SectionTypes.ProductGroup,
                                Item = productModifier,
                                ItemParent = productModifierGroup
                            });   
                        }
                    }
                }
            }
        }
    }
}