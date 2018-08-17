using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Utils;
using Presentation.Views;
using Xamarin.ViewPagerIndicator;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Menu
{
    public class MenuItemModificationFragment : BaseFragment, IBroadcastObserver, AdapterView.IOnItemSelectedListener, Toolbar.IOnMenuItemClickListener
    {
        private MenuService menuService;

        private ListView headers;
        private Spinner addToBasketQty;
        private TextView price;
        private ViewPager pager;
        private Toolbar basketToolbar;

        public MenuItem Item { get; set; }
        public bool IsBasketItem { get; set; }

        public MenuDealLine DealLine { get; set; }
        public Action<decimal> OnAddToBasket { get; set; }
        public Action NextPage { get; set; }
        public int Index { get; set; }
        public decimal? Qty { get; set; }
        private bool requiredOnly;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            menuService = new MenuService();

            View view = null;

            if (Arguments != null)
            {
                requiredOnly = Arguments.GetBoolean(BundleUtils.RequiredOnly);
            }

            if (Item is Recipe || (requiredOnly && Item is MenuDeal))
            { 
                view = Inflate(inflater, Resource.Layout.MenuItemModificationScreen, null);

                var toolbar = view.FindViewById<Toolbar>(Resource.Id.MenuItemModificationScreenToolbar);
                (Activity as HospActivity).SetSupportActionBar(toolbar);

                view.FindViewById<TextView>(Resource.Id.MenuItemModificationItemTitle).Text = Item.Description;
                price = view.FindViewById<TextView>(Resource.Id.MenuItemModificationItemPrice);
                price.Text = AppData.FormatCurrency(Item.Price.Value);

                headers = view.FindViewById<ListView>(Resource.Id.MenuItemModificationList);

                //items = new List<SectionedListItem>();

                //CreateRecipeSectionItems(recipe);

                basketToolbar = view.FindViewById<Toolbar>(Resource.Id.MenuItemModificationBasketToolbar);
                addToBasketQty = view.FindViewById<Spinner>(Resource.Id.MenuItemModificationAddToBasketQty);

                if (Item is Recipe)
                {
                    var recipe = Item as Recipe;
                    headers.Adapter = new MenuItemModificationAdapter(Activity, recipe, requiredOnly);
                }
                else if (Item is MenuDeal)
                {
                    var deal = Item as MenuDeal;
                    headers.Adapter = new MenuItemModificationAdapter(Activity, deal);
                }
                
                // Create an ArrayAdapter using the string array and a default spinner layout
                var adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, Utils.Utils.GenerateQtyList(Activity));
                //var adapter = ArrayAdapter.FromArray(Activity, Resource.Array.QtyOptions, Android.Resource.Layout.SimpleSpinnerItem);
                // Specify the layout to use when the list of choices appears
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                // Apply the adapter to the spinner
                addToBasketQty.Adapter = adapter;

                addToBasketQty.Visibility = ViewStates.Visible;

                addToBasketQty.OnItemSelectedListener = this;

                addToBasketQty.SetSelection(Math.Max(0, (int)Qty - 1));

                if (IsBasketItem)
                {
                    basketToolbar.InflateMenu(Resource.Menu.AddToBasketSaveMenu);
                }
                else
                {
                    basketToolbar.InflateMenu(Resource.Menu.AddToBasketMenu);
                    basketToolbar.Menu.FindItem(Resource.Id.MenuViewEdit).SetVisible(false);
                }

                basketToolbar.SetOnMenuItemClickListener(this);
            }
            else if (Item is MenuDeal)
            {
                view = Inflate(inflater, Resource.Layout.MenuItemModificationDealScreen, null);

                var toolbar = view.FindViewById<Toolbar>(Resource.Id.MenuItemModificationScreenToolbar);
                (Activity as HospActivity).SetSupportActionBar(toolbar);

                var deal = Item as MenuDeal;

                view.FindViewById<TextView>(Resource.Id.MenuItemModificationItemTitle).Text = Item.Description;
                price = view.FindViewById<TextView>(Resource.Id.MenuItemModificationItemPrice);
                price.Text = AppData.FormatCurrency(menuService.GetItemFullPrice(AppData.MobileMenu, Item));

                pager = view.FindViewById<ViewPager>(Resource.Id.MenuItemModificationPager);

                pager.Adapter = new DealPagerAdapter(ChildFragmentManager, () => pager.SetCurrentItem(pager.CurrentItem + 1, true), OnAddToBasket, deal.DealLines, Qty, IsBasketItem);

                var pageIndicator = view.FindViewById<LinePageIndicator>(Resource.Id.MenuItemModificationIndicator);
                
                if (deal.DealLines.Count > 1)
                {
                    pageIndicator.SetViewPager(pager);
                }
                else
                {
                    pageIndicator.Visibility = ViewStates.Gone;
                }
            }
            else if (DealLine != null)
            {
                view = Inflate(inflater, Resource.Layout.MenuItemModificationDealLine, null);

                view.Tag = DealPagerAdapter.DealPagerPageTag + Index;

                headers = view.FindViewById<ListView>(Resource.Id.MenuItemModificationList);

                headers.Adapter = new MenuItemModificationAdapter(Activity, DealLine);

                basketToolbar = view.FindViewById<Toolbar>(Resource.Id.MenuItemModificationBasketToolbar);

                if (OnAddToBasket != null)
                {
                    addToBasketQty = view.FindViewById<Spinner>(Resource.Id.MenuItemModificationAddToBasketQty);

                    // Create an ArrayAdapter using the string array and a default spinner layout
                    var adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, Utils.Utils.GenerateQtyList(Activity));
                    //var adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.QtyOptions, Android.Resource.Layout.SimpleSpinnerItem);
                    // Specify the layout to use when the list of choices appears
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    // Apply the adapter to the spinner
                    addToBasketQty.Adapter = adapter;

                    addToBasketQty.Visibility = ViewStates.Visible;

                    addToBasketQty.OnItemSelectedListener = this;

                    addToBasketQty.SetSelection(Math.Max(0, (int)Qty - 1));

                    if (IsBasketItem)
                    {
                        basketToolbar.InflateMenu(Resource.Menu.AddToBasketSaveMenu);
                    }
                    else
                    {
                        basketToolbar.InflateMenu(Resource.Menu.AddToBasketMenu);
                        basketToolbar.Menu.FindItem(Resource.Id.MenuViewEdit).SetVisible(false);
                    }
                }
                else
                {
                    basketToolbar.InflateMenu(Resource.Menu.AddToBasketNextMenu);
                }

                basketToolbar.SetOnMenuItemClickListener(this);
            }
            
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (price != null && Activity is HospActivity)
                (Activity as HospActivity).AddObserver(this);
        }

        public override void OnPause()
        {
            if (price != null && Activity is HospActivity)
                (Activity as HospActivity).RemoveObserver(this);

            base.OnPause();
        }

        public bool GoBack()
        {
            if (pager != null && pager.CurrentItem != 0)
            {
                pager.SetCurrentItem(pager.CurrentItem - 1, true);
                return true;
            }

            return false;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewAdd:
                case Resource.Id.MenuViewSave:
                    if (OnAddToBasket != null)
                        OnAddToBasket(addToBasketQty.SelectedItemPosition + 1);
                    return true;

                case Resource.Id.MenuViewNext:
                    if (NextPage != null)
                        NextPage();
                    return true;
            }

            return false;
        }

        public void BroadcastReceived(string action)
        {
            var qty = 0m;

            if (pager != null)
            {
                var view = pager.FindViewWithTag(DealPagerAdapter.DealPagerPageTag + (pager.Adapter.Count - 1));

                if (view == null)
                {
                    qty = 1;
                }
                else
                {
                    qty = view.FindViewById<Spinner>(Resource.Id.MenuItemModificationAddToBasketQty).SelectedItemPosition + 1;
                }
            }
            else if(View != null)
            {
                qty = View.FindViewById<Spinner>(Resource.Id.MenuItemModificationAddToBasketQty).SelectedItemPosition + 1;
            }

            if (action == BroadcastUtils.ItemPriceChanged && price != null)
            {
                price.Text = AppData.FormatCurrency(qty * menuService.GetItemFullPrice(AppData.MobileMenu, Item));
            }         
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            BroadcastUtils.SendBroadcast(Activity, BroadcastUtils.ItemPriceChanged);
        }

        public void OnNothingSelected(AdapterView parent)
        {

        }
    }
}