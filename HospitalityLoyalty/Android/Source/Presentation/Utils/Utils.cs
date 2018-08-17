using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using ZXing;
using ZXing.QrCode;

namespace Presentation.Utils
{
    public class Utils
    {
        public static void NotifyAdapterChanged(IListAdapter adapter)
        {
            if (adapter is BaseAdapter)
            {
                (adapter as BaseAdapter).NotifyDataSetChanged();
            }
            else if (adapter is HeaderViewListAdapter)
            {
                NotifyAdapterChanged((adapter as HeaderViewListAdapter).WrappedAdapter);
            }
            else
            {
                throw new Exception();
            }
        }

        public static BaseAdapter GetBaseAdapter(IListAdapter adapter)
        {
            if (adapter is BaseAdapter)
            {
                return adapter as BaseAdapter;
            }
            else if (adapter is HeaderViewListAdapter)
            {
                return GetBaseAdapter((adapter as HeaderViewListAdapter).WrappedAdapter);
            }
            else
            {
                throw new Exception();
            }
        }

        public static Bitmap GenerateQrCode(string code)
        {
            var height = 1000;
            var width = 1000;
            var writer = new QRCodeWriter();
            var matrix = writer.encode(code, BarcodeFormat.QR_CODE, width, height);

            int[] pixels = new int[width * height];
            // All are 0, or black, by default
            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    pixels[offset + x] = matrix[x, y] ? Color.Black : Color.Transparent;            //WHITE ??
                }
            }

            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);

            return bitmap;
        }

        public static ColorFilter GetColorFilter(Color color)
        {
            return new PorterDuffColorFilter(color, PorterDuff.Mode.SrcAtop);
        }

        public static string[] GenerateQtyList(Context context)
        {
            var qtyList = new string[AppData.MaxItems];

            for (int i = 0; i < AppData.MaxItems; i++ )
            {
                qtyList[i] = string.Format(context.Resources.GetString(Resource.String.QtyN), (i + 1).ToString());
            }

            return qtyList;
        }

        public static string GenerateItemExtraInfo(Context context, MenuService menuService, MobileMenu mobileMenu, MenuItem item)
        {
            var extraInfo = string.Empty;

            if (item is Recipe)
            {
                var recipe = item as Recipe;

                foreach (var ingredient in recipe.Ingredients)
                {
                    extraInfo += GenerateIngredientExtraInfo(context, menuService, mobileMenu, ingredient);
                }

                foreach (var productModifierGroup in recipe.ProductModifierGroups)
                {
                    foreach (var productModifier in productModifierGroup.ProductModifiers)
                    {
                        extraInfo += GenerateModifierExtraInfo(context, productModifierGroup, productModifier);
                    }
                }
            }
            else if (item is MenuDeal)
            {
                var deal = item as MenuDeal;

                foreach (var dealLine in deal.DealLines)
                {
                    var dealLineItem = dealLine.DealLineItems.FirstOrDefault(x => x.ItemId == dealLine.SelectedId);

                    if (dealLineItem != null)
                    {
                        if (dealLineItem.Quantity > 1)
                        {
                            extraInfo += dealLineItem.Quantity.ToString("0.##") + " ";
                        }

                        extraInfo += dealLineItem.MenuItem.Description;

                        if (dealLineItem.PriceAdjustment != 0m)
                        {
                            extraInfo += " (" + AppData.FormatCurrency(dealLineItem.PriceAdjustment) + ")";
                        }

                        extraInfo += System.Environment.NewLine;

                        if (dealLineItem.MenuItem is Recipe)
                        {
                            var recipeInfo = GenerateItemExtraInfo(context, menuService, mobileMenu, dealLineItem.MenuItem);

                            if (!string.IsNullOrEmpty(recipeInfo))
                            {
                                extraInfo += recipeInfo + System.Environment.NewLine;
                            }
                        }
                    }

                    foreach (var dealModifierGroup in dealLine.DealModifierGroups)
                    {
                        foreach (var dealModifier in dealModifierGroup.DealModifiers)
                        {
                            extraInfo += GenerateModifierExtraInfo(context, dealModifierGroup, dealModifier);
                        }
                    }
                }
            }

            return extraInfo.TrimEnd(System.Environment.NewLine.ToCharArray());
        }

        public static string GenerateIngredientExtraInfo(Context context, MenuService menuService, MobileMenu mobileMenu, Ingredient ingredient)
        {
            string extraInfo = string.Empty;

            if (ingredient.Quantity == 0 && ingredient.Quantity != ingredient.OriginalQuantity)
            {
                extraInfo = string.Format(context.Resources.GetString(Resource.String.BasketExcluded), menuService.GetItem(mobileMenu, ingredient.Id)?.Description) + System.Environment.NewLine;
            }
            else if (ingredient.Quantity > ingredient.OriginalQuantity)
            {
                extraInfo = string.Format(context.Resources.GetString(Resource.String.BasketExtra), ingredient.Quantity - ingredient.OriginalQuantity, menuService.GetItem(mobileMenu, ingredient.Id)?.Description) + System.Environment.NewLine;
            }
            else if (ingredient.Quantity < ingredient.OriginalQuantity)
            {
                extraInfo = string.Format(context.Resources.GetString(Resource.String.BasketReduced), ingredient.OriginalQuantity - ingredient.Quantity, menuService.GetItem(mobileMenu, ingredient.Id)?.Description) + System.Environment.NewLine;
            }

            return extraInfo;
        }

        public static string GenerateModifierExtraInfo(Context context, ModifierGroup modifierGroup, Modifier modifier)
        {
            string extraInfo = string.Empty;

            var modifierType = modifier.GetModifierType(modifierGroup);

            if ((modifierType == ModifierType.Radio || modifierType == ModifierType.Checkbox) && modifier.Quantity > modifier.OriginalQty)
            {
                extraInfo = string.Format(context.GetString(Resource.String.BasketModifier), modifier.Description);
            }
            else
            {
                if (modifier.Quantity > modifier.OriginalQty)
                {
                    extraInfo = string.Format(context.Resources.GetString(Resource.String.BasketExtra), modifier.Quantity - modifier.OriginalQty, modifier.Description);
                }
                else if (modifier.Quantity < modifier.OriginalQty)
                {
                    extraInfo = string.Format(context.Resources.GetString(Resource.String.BasketReduced), modifier.OriginalQty - modifier.Quantity, modifier.Description);
                }    
            }

            var price = modifier.Price*(modifier.Quantity - modifier.OriginalQty);
            if (price != 0m)
            {
                extraInfo += " (" + AppData.FormatCurrency(price) + ")";
            }

            if (!string.IsNullOrEmpty(extraInfo))
            {
                extraInfo += System.Environment.NewLine;
            }
            
            return extraInfo;
        }

        public static string GetPhoneUUID(Context context)
        {
            var factory = new DeviceUuidFactory(context);
            return factory.getDeviceUuid();
        }

        public class ViewPagerUtils
        {
            public class ImageTransformPager : Java.Lang.Object, ViewPager.IPageTransformer
            {
                public const BuildVersionCodes MinVersion = BuildVersionCodes.IceCreamSandwich;
                
                public ImageTransformPager()
                {
                }

	            public void TransformPage (View page, float position)
	            {
                    if (position < -1)  // [-Infinity,-1)
                    {
                        // This page is way off-screen to the left.
                        page.Alpha = 0;
                    }
                    else if (position <= 1)  // [-1,1]
                    {
                        page.Alpha = 1;

                        var imageView = page.FindViewById(Resource.Id.HeaderImageContainer);

                        if (imageView != null)
                        {
                            imageView.TranslationX = -position * (page.Width / 2); //Half the normal speed
                        }
                    }
                    else  // (1,+Infinity]
                    {
                        // This page is way off-screen to the right.
                        page.Alpha = 0;
                    }
	            }
            }
        }

        public class ViewUtils
        {
            public static void AddOnGlobalLayoutListener(View view, ViewTreeObserver.IOnGlobalLayoutListener listener)
            {
                if (view == null)
                    return;

                view.ViewTreeObserver.AddOnGlobalLayoutListener(listener);
            }

            public static void RemoveOnGlobalLayoutListener(View view, ViewTreeObserver.IOnGlobalLayoutListener listener)
            {
                if (view == null)
                    return;

                view.ViewTreeObserver.RemoveOnGlobalLayoutListener(listener);
            }

            public static View Inflate(LayoutInflater inflater, int resourceId, ViewGroup root = null, bool tryAgain = true)
            {
                try
                {
                    return inflater.Inflate(resourceId, root, false);
                }
                //catch (OutOfMemoryException oome)
                catch (Exception)
                {
                    if (tryAgain)
                    {
                        GC.Collect();
                        return Inflate(inflater, resourceId, root, false);
                    }
                    throw;
                }
            }

            public static int GetContentViewHeight(Context context)
            {
                return context.Resources.DisplayMetrics.HeightPixels - context.Resources.GetDimensionPixelSize(Resource.Dimension.ActionBarHeight) - (int)Math.Ceiling(25 * context.Resources.DisplayMetrics.Density);
            }
        }
    }
}