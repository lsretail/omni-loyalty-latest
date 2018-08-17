using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Utils;
using Xamarin.It.Sephiroth.Android.Library.Imagezoom.Graphics;

namespace Presentation.Activities.Menu
{
    [Activity(Label = "", Theme = "@style/BaseThemeNoActionBar")]
    public class MenuItemModificationActivity : HospActivityNoStatusBar
    {
        private const string MenuItemModificationFragmentTag = "MenuItemModificationFragmentTag";

        private MenuItem item;
        private BasketModel model;
        private MenuService menuService;

        protected override void OnCreate(Bundle bundle)
        {
            RightDrawer = true;

            ActivityType = ActivityTypes.Menu;

            base.OnCreate(bundle);

            model = new BasketModel(this);
            menuService = new MenuService();

            var fragment = new MenuItemModificationFragment();

            if (Intent.Extras.ContainsKey(BundleUtils.ItemId))
            {
                var itemId = Intent.Extras.GetString(BundleUtils.ItemId);
                var menuId = Intent.Extras.GetString(BundleUtils.MenuId);
                var itemType = (NodeLineType) Intent.Extras.GetInt(BundleUtils.Type);

                if (Intent.Extras.ContainsKey(BundleUtils.Qty))
                {
                    fragment.Qty = Intent.Extras.GetInt(BundleUtils.Qty);
                }
                else
                {
                    fragment.Qty = 1;
                }

                item = menuService.GetMenuItem(AppData.MobileMenu, itemId, itemType);

                fragment.OnAddToBasket = qty =>
                    {
                        model.AddItemToBasket(item, qty, false);

                        SetResult(Result.Ok);

                        Finish();
                    };
            }
            else if (Intent.Extras.ContainsKey(BundleUtils.BasketItemId))
            {
                var basketItemId = Intent.Extras.GetString(BundleUtils.BasketItemId);
                var basketItem = AppData.Basket.Items.FirstOrDefault(x => x.Id == basketItemId);

                item = basketItem.Item.Clone();

                fragment.Qty = basketItem.Quantity;
                fragment.OnAddToBasket = qty =>
                    {
                        model.UpdateBasketItem(basketItemId, item, qty);

                        SetResult(Result.Ok);

                        Finish();
                    };

                fragment.IsBasketItem = true;
            }

            if(bundle != null)
            {
                var serializer = new XmlSerializer(typeof(MenuItem), new Type[] { typeof(Recipe), typeof(Product), typeof(MenuDeal) });

                using (TextReader textReader = new StringReader(bundle.GetString("testing")))
                {
                    Console.WriteLine(textReader.ToString());

                    item = (MenuItem)serializer.Deserialize(textReader);
                }
            }

            (fragment as MenuItemModificationFragment).Item = item;

            fragment.Arguments = Intent.Extras;

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.BaseActivityScreenContentFrame, fragment, MenuItemModificationFragmentTag);
            ft.Commit();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            var serializer = new XmlSerializer(typeof(MenuItem), new Type[] { typeof(Recipe), typeof(Product), typeof(MenuDeal) });

            using (var textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, item);
                outState.PutString("testing", textWriter.ToString());
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnBackPressed()
        {
            var fragment = SupportFragmentManager.FindFragmentByTag(MenuItemModificationFragmentTag) as MenuItemModificationFragment;

            if (fragment != null && !fragment.GoBack())
            {
                base.OnBackPressed();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_clear_white_24dp);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:

                    Finish();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}