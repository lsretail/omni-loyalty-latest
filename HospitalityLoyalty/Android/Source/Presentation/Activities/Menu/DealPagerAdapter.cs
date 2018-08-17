using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using Presentation.Activities.Base;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Object = Java.Lang.Object;

namespace Presentation.Activities.Menu
{
    public class DealPagerAdapter : FragmentStatePagerAdapter
    {
        public const string DealPagerPageTag = "DealPagerPageTag";

        private readonly Action<decimal> onAddToBasket;
        private readonly Action nextPage;
        private readonly List<MenuDealLine> dealLines;
        private readonly decimal? qty;
        private readonly bool isBasketItem;

        public DealPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public DealPagerAdapter(FragmentManager fm, Action nextPage, Action<decimal> onAddToBasket, List<MenuDealLine> dealLines, decimal? qty, bool isBasketItem) : base(fm)
        {
            this.onAddToBasket = onAddToBasket;
            this.nextPage = nextPage;
            this.dealLines = dealLines;
            this.qty = qty;
            this.isBasketItem = isBasketItem;
        }

        public override int Count
        {
            get { return dealLines.Count; }
        }

        public override Fragment GetItem(int position)
        {
            var menuItemModificationFragment = new MenuItemModificationFragment();
            menuItemModificationFragment.Index = position;
            menuItemModificationFragment.DealLine = dealLines[position];
            menuItemModificationFragment.IsBasketItem = isBasketItem;
            
            if (position == Count - 1)
            {
                menuItemModificationFragment.OnAddToBasket = onAddToBasket;
                menuItemModificationFragment.Qty = qty;
            }
            else
            {
                menuItemModificationFragment.NextPage = nextPage;
            }

            return menuItemModificationFragment;
        }
    }
}