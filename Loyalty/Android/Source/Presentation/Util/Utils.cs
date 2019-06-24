using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Presentation.Dialogs;
using ZXing;
using ZXing.QrCode;

using Format = Android.Graphics.Format;
using ImageView = Android.Widget.ImageView;
using static Android.Bluetooth.BluetoothClass;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Util
{
    public class Utils
    {
        public class ImageUtils
        {
            public class CircleDrawable : Drawable
            {
                Bitmap bmp;
                BitmapShader bmpShader;
                Paint paint;
                RectF oval;

                public CircleDrawable(Bitmap bmp)
                {
                    this.bmp = bmp;
                    this.bmpShader = new BitmapShader(bmp, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                    this.paint = new Paint() { AntiAlias = true };
                    this.paint.SetShader(bmpShader);
                    this.oval = new RectF();
                }

                public override void Draw(Canvas canvas)
                {
                    canvas.DrawOval(oval, paint);
                }

                protected override void OnBoundsChange(Rect bounds)
                {
                    base.OnBoundsChange(bounds);

                    var left = 0;
                    var top = 0;

                    if (bounds.Width() > bounds.Height())
                    {
                        left = (bounds.Width() - bounds.Height()) / 2;
                    }
                    else
                    {
                        top = (bounds.Height() - bounds.Width()) / 2;
                    }

                    oval.Set(left, top, bounds.Width() - left, bounds.Height() - top);
                }

                public override int IntrinsicWidth
                {
                    get
                    {
                        return bmp.Width;
                    }
                }

                public override int IntrinsicHeight
                {
                    get
                    {
                        return bmp.Height;
                    }
                }

                public override void SetAlpha(int alpha)
                {
                }

                public override int Opacity
                {
                    get 
                    {
                        return (int)Format.Opaque;
                    }
                }

                public override void SetColorFilter(ColorFilter cf)
                {

                }
            }

            public static ColorFilter GetColorFilter(Color color, PorterDuff.Mode mode = null)
            {
                if (mode == null)
                {
                    mode = PorterDuff.Mode.SrcAtop;
                }
                return new PorterDuffColorFilter(color, mode);
            }

            public static Color DarkenColor(Color color, double multiplier = 0.9d)
            {
                return new Color((int)(color.R * multiplier), (int)(color.G * multiplier), (int)(color.B * multiplier), color.A);
            }

            public static bool ColorIsEmpty(Color color)
            {
                return !(color.A == 0 && color.R == 0 && color.G == 0 && color.B == 0);
            }

            public static Color BlendColors(Color fromColor, Color toColor, float ratio)
            {
                return new Color((int)(fromColor.R * (1 - ratio) + toColor.R * ratio),
                                 (int)(fromColor.G * (1 - ratio) + toColor.G * ratio),
                                 (int)(fromColor.B * (1 - ratio) + toColor.B * ratio),
                                 (int)(fromColor.A * (1 - ratio) + toColor.A * ratio));
            }

            public static void CrossfadeImage(ImageView imageView, Bitmap image, View imageColorView = null, bool crossfade = true)
            {
                if (crossfade)
                {
                    var imageId = (string)imageView.Tag;
                    int fadeInDuration; // Configure time values here

                    fadeInDuration = 500;

                    imageView.Visibility = ViewStates.Invisible;
                    //Visible or invisible by default - this will apply when the animation ends

                    imageView.SetImageBitmap(image);

                    Animation fadeIn = new AlphaAnimation(0, 1);
                    fadeIn.Interpolator = new DecelerateInterpolator(); // add this
                    fadeIn.Duration = fadeInDuration;

                    AnimationSet animation = new AnimationSet(false); // change to false
                    animation.AddAnimation(fadeIn);
                    animation.RepeatCount = 1;

                    animation.AnimationEnd += (sender, args) =>
                    {
                        if (imageColorView != null) 
                            imageColorView.SetBackgroundColor(Color.Transparent);

                        imageView.Visibility = ViewStates.Visible;
                    };

                    imageView.Animation = animation;
                }
                else
                {
                    imageView.SetImageBitmap(image);
                    if (imageColorView != null) 
                        imageColorView.SetBackgroundColor(Color.Transparent);
                }
            }

            public static void CrossfadeImage(ImageView imageView, Drawable drawable, View imageColorView = null, bool crossfade = true)
            {
                if (crossfade)
                {
                    var imageId = (string)imageView.Tag;
                    int fadeInDuration; // Configure time values here

                    fadeInDuration = 500;

                    imageView.Visibility = ViewStates.Invisible;
                    //Visible or invisible by default - this will apply when the animation ends

                    imageView.SetImageDrawable(drawable);

                    Animation fadeIn = new AlphaAnimation(0, 1);
                    fadeIn.Interpolator = new DecelerateInterpolator(); // add this
                    fadeIn.Duration = fadeInDuration;

                    AnimationSet animation = new AnimationSet(false); // change to false
                    animation.AddAnimation(fadeIn);
                    animation.RepeatCount = 1;

                    animation.AnimationEnd += (sender, args) =>
                    {
                        if (imageColorView != null) 
                            imageColorView.SetBackgroundColor(Color.Transparent);

                        imageView.Visibility = ViewStates.Visible;
                    };

                    imageView.Animation = animation;
                }
                else
                {
                    imageView.SetImageDrawable(drawable);
                    if (imageColorView != null) 
                        imageColorView.SetBackgroundColor(Color.Transparent);
                }
            }

            public static void ClearImageView(ImageView image)
            {
                var prevBitmap = image.Drawable as BitmapDrawable;

                image.DestroyDrawingCache();
                image.Visibility = ViewStates.Visible;

                if (prevBitmap != null)
                {
                    if (prevBitmap.Bitmap != null)
                    {
                        prevBitmap.Bitmap.Recycle();
                    }

                    prevBitmap.SetCallback(null);
                    prevBitmap.InvalidateSelf();
                    image.InvalidateDrawable(prevBitmap);
                    prevBitmap.Dispose();
                    image.SetImageDrawable(null);
                    image.Invalidate();

                    //System.GC.Collect();
                    //image.ClearAnimation();

                    if (image.Animation != null)
                    {
                        (image.Animation as AnimationSet).Duration = 0;
                    }
                }
                image.SetImageResource(Resource.Color.transparent);
            }

            private static object locker = new object();
            public static Bitmap DecodeImage(string decodeString, bool tryAgain = true)
            {
                lock (locker)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(decodeString))
                        {
                            byte[] decodedString = Base64.Decode(decodeString, Base64Flags.Default);
                            return BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);
                        }
                        return null;
                    }
                    catch (Exception)
                    {
                        System.GC.Collect();
                        if (tryAgain)
                        {
                            return DecodeImage(decodeString, false);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            public static async Task<Bitmap> DecodeImageAsync(string decodeString)
            {
                return await Task.Run(() => DecodeImage(decodeString));
            }
        }

        public class ListUtils
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
                if (adapter == null)
                {
                    return null;
                }

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
        }

        public class LogUtils
        {
            public static void Log(string text)
            {
#if DEBUG
                Android.Util.Log.Debug("LS Retail Log", text);
#endif
            }
        }

        public class LocationUtils
        {
            public static bool UsePlayServices()
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return false;
                }
                return true;
            }
        }

        public class ViewPagerUtils
        {
            public class ParallaxPageTransformer : Java.Lang.Object, ViewPager.IPageTransformer
            {
                public const BuildVersionCodes MinVersion = BuildVersionCodes.IceCreamSandwich;

                float parallaxCoefficient;
                float distanceCoefficient;
                IEnumerable<int>[] viewLayers;

                public ParallaxPageTransformer(float parallaxCoefficient, float distanceCoefficient, params IEnumerable<int>[] viewLayers)
                {
                    this.parallaxCoefficient = parallaxCoefficient;
                    this.distanceCoefficient = distanceCoefficient;
                    this.viewLayers = viewLayers;
                }

                public void TransformPage(View page, float position)
                {
                    float coefficient = page.Width * parallaxCoefficient;
                    foreach (var layer in viewLayers)
                    {
                        foreach (var viewID in layer)
                        {
                            var v = page.FindViewById(viewID);
                            if (v != null)
                                v.TranslationX = coefficient * position;
                        }
                        coefficient *= distanceCoefficient;
                    }
                }
            }

            public class ZoomOutPageTransformer : Java.Lang.Object, ViewPager.IPageTransformer
            {
                public const BuildVersionCodes MinVersion = BuildVersionCodes.IceCreamSandwich;

                private const float MIN_SCALE = 0.85f;
                private const float MIN_ALPHA = 0.5f;

                public void TransformPage(View view, float position)
                {
                    int pageWidth = view.Width;
                    int pageHeight = view.Height;

                    if (position < -1)  // [-Infinity,-1)
                    {
                        // This page is way off-screen to the left.
                        view.Alpha = 0;
                    }
                    else if (position <= 1)  // [-1,1]
                    {
                        // Modify the default slide transition to shrink the page as well
                        float scaleFactor = Math.Max(MIN_SCALE, 1 - Math.Abs(position));
                        float vertMargin = pageHeight * (1 - scaleFactor) / 2;
                        float horzMargin = pageWidth * (1 - scaleFactor) / 2;

                        if (position < 0)
                        {
                            view.TranslationX = horzMargin - vertMargin / 2;
                        }
                        else
                        {
                            view.TranslationX = -horzMargin + vertMargin / 2;
                        }

                        // Scale the page down (between MIN_SCALE and 1)
                        view.ScaleX = scaleFactor;
                        view.ScaleY = scaleFactor;

                        // Fade the page relative to its size.
                        view.Alpha = MIN_ALPHA + (scaleFactor - MIN_SCALE) / (1 - MIN_SCALE) * (1 - MIN_ALPHA);
                    }
                    else  // (1,+Infinity]
                    {
                        // This page is way off-screen to the right.
                        view.Alpha = 0;
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
                view?.ViewTreeObserver.RemoveOnGlobalLayoutListener(listener);
            }

            public static View Inflate(LayoutInflater inflater, int resourceId, ViewGroup root = null, bool attachToParent = true, bool tryAgain = true)
            {
                try
                {
                    return inflater.Inflate(resourceId, root, attachToParent);
                }
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
        }

        public class SocialMediaUtils
        {
            public enum SocialMediaConnection
            {
                None,
                Google,
                Facebook
            }

            public static SocialMediaConnection CurrentSocialMediaConnection = SocialMediaConnection.None;
        }

        public static bool IsTablet(Context context)
        {
            bool xlarge = (((int)context.Resources.Configuration.ScreenLayout & (int)ScreenLayout.SizeMask) == 4);
            bool large = (((int)context.Resources.Configuration.ScreenLayout & (int)ScreenLayout.SizeMask) == (int)ScreenLayout.SizeLarge);
            return (xlarge || large);
        }

        public static void FillDeviceInfo(LSRetail.Omni.Domain.DataModel.Loyalty.Setup.Device device)
        {
            device.Manufacturer = Build.Manufacturer;
            device.Model = Build.Model;
            device.Platform = "Android";
            device.OsVersion = Build.VERSION.Release;
            device.DeviceFriendlyName = device.Manufacturer + device.Model;
        }

        public static string GetPhoneUUID(Context context)
        {
            var factory = new DeviceUuidFactory(context);
            return factory.getDeviceUuid();
        }

        public static long CalculateExpandableListItemId(int groupPosition, int childPosition)
        {
            return groupPosition * 10000 + childPosition;
        }

        public static bool CheckForPlayServices(Context context)
        {
            try
            {
                ApplicationInfo info = context.PackageManager.GetApplicationInfo("com.google.android.gms", 0);
                return true;
            }
            catch (PackageManager.NameNotFoundException)
            {
                //app doesnt exist
            }

            var dialog = new WarningDialog(context, context.GetString(Resource.String.PlayServiceUtilPlayServices))
                                        .SetPositiveButton(context.GetString(Resource.String.PlayServiceUtilInstall),
                                       () =>
                                       {
                                           try
                                           {
                                               Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=com.google.android.gms"));
                                               intent.AddFlags(ActivityFlags.ClearWhenTaskReset);
                                               intent.SetPackage("com.android.vending");
                                               context.StartActivity(intent);
                                           }
                                           catch (ActivityNotFoundException)
                                           {
                                               // Ok that didn't work, try the market method.
                                               try
                                               {
                                                   Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.google.android.gms"));
                                                   intent.AddFlags(ActivityFlags.ClearWhenTaskReset);
                                                   intent.SetPackage("com.android.vending");
                                                   context.StartActivity(intent);
                                               }
                                               catch (ActivityNotFoundException)
                                               {
                                                   // Ok, weird. Maybe they don't have any market app. Just show the website.

                                                   Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=com.google.android.gms"));
                                                   intent.AddFlags(ActivityFlags.ClearWhenTaskReset);
                                                   context.StartActivity(intent);
                                               }
                                           }
                                       });

            dialog.SetCancelable(true).SetNegativeButton(context.GetString(Resource.String.ApplicationCancel), () => { });
            dialog.Message = context.GetString(Resource.String.PlayServiceUtilRequiresPlayService);
            dialog.Show();
            return false;
        }

        public static void NotifyAdapterChanged(IListAdapter adapter)
        {
            if (adapter == null)
                return;

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

        public class TimeUtils
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            public static string DateTimeAgo(Context context, DateTime dateTime)
            {
                var ts = new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks);
                double delta = Math.Abs(ts.TotalSeconds);

                if (delta < 0)
                {
                    return string.Empty;    //not passed yet
                }
                if (delta < 1 * MINUTE)
                {
                    return ts.Seconds == 1 ? context.GetString(Resource.String.TimeUtilsSecondAgo) : ts.Seconds + " " + context.GetString(Resource.String.TimeUtilsSecondsAgo);
                }
                if (delta < 2 * MINUTE)
                {
                    return context.GetString(Resource.String.TimeUtilsMinuteAgo);
                }
                if (delta < 45 * MINUTE)
                {
                    return ts.Minutes + " " + context.GetString(Resource.String.TimeUtilsMinutesAgo);
                }
                if (delta < 90 * MINUTE)
                {
                    return context.GetString(Resource.String.TimeUtilsHourAgo);
                }
                if (delta < 24 * HOUR)
                {
                    return ts.Hours + " " + context.GetString(Resource.String.TimeUtilsHoursAgo);
                }
                if (delta < 48 * HOUR)
                {
                    return context.GetString(Resource.String.TimeUtilsYesterday);
                }
                if (delta < 30 * DAY)
                {
                    return ts.Days + " " + context.GetString(Resource.String.TimeUtilsDaysAgo);
                }
                if (delta < 12 * MONTH)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    return months <= 1 ? context.GetString(Resource.String.TimeUtilsMonthAgo) : months + " " + context.GetString(Resource.String.TimeUtilsMonthsAgo);
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    return years <= 1 ? context.GetString(Resource.String.TimeUtilsYearAgo) : years + " " + context.GetString(Resource.String.TimeUtilsYearsAgo);
                }
            }
        }

        public class PreferenceUtils
        {
            private const string myPrefs = "LsAndroidLibraryPrefs";
            public const string ShowUserNotifications = "ShowUserNotifications";
            public const string WSUrl = "WSUrl";
            public const string NavigationDrawerHasBeenShown = "NavigationDrawerHasBeenShown";
            public const string ShowListAsList = "ShowListAsList";
            public const string VersionCode = "VersionCode";
            public const string FcmRegistrationId = "FcmRegistrationId";

            public static bool GetBool(Context context, string key, bool defaultValue = false)
            {
                var settings = context.GetSharedPreferences(myPrefs, 0);
                return settings.GetBoolean(key, defaultValue);
            }

            public static void SetBool(Context context, string key, bool value)
            {
                var settings = context.GetSharedPreferences(myPrefs, 0);
                var editor = settings.Edit();
                editor.PutBoolean(key, value);
                editor.Commit();
            }

            public static string GetString(Context context, string key)
            {
                var settings = context.GetSharedPreferences(myPrefs, 0);
                return settings.GetString(key, string.Empty);
            }

            public static void SetString(Context context, string key, string value)
            {
                var settings = context.GetSharedPreferences(myPrefs, 0);
                var editor = settings.Edit();
                editor.PutString(key, value);
                editor.Commit();
            }

            public static int GetInt(Context context, string key)
            {
                var settings = context.GetSharedPreferences(myPrefs, 0);
                return settings.GetInt(key, 0);
            }

            public static void SetInt(Context context, string key, int value)
            {
                var settings = context.GetSharedPreferences(myPrefs, 0);
                var editor = settings.Edit();
                editor.PutInt(key, value);
                editor.Commit();
            }

            public static void Remove(Context context, string key)
            {
                var settings = context.GetSharedPreferences(myPrefs, 0);
                var editor = settings.Edit();
                editor.Remove(key);
                editor.Commit();
            }
        }

        public class QrCodeUtils
        {
            public static Bitmap GenerateQRCode(Context context, bool includeCoupons = true)
            {
                var background = new Color(ContextCompat.GetColor(context, Resource.Color.backgroundcolor));

                var height = 500;
                var width = 500;
                var writer = new QRCodeWriter();
                var matrix = writer.encode(GetQRCodeString(includeCoupons), BarcodeFormat.QR_CODE, width, height);

                // All are 0, or black, by default
                int[] pixels = new int[width * height];
                for (int y = 0; y < height; y++)
                {
                    int offset = y * width;
                    for (int x = 0; x < width; x++)
                    {
                        pixels[offset + x] = matrix[x, y] ? Color.Black : background;
                    }
                }

                Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);
                return bitmap;
            }

            public static string GetQRCodeString(bool includeCoupons = true)
            {
                var xml = string.Format("<mobiledevice><contactid>{0}</contactid><accountid>{1}</accountid><cardid>{2}</cardid>", AppData.Device.UserLoggedOnToDevice.Id, AppData.Device.UserLoggedOnToDevice.Account.Id, AppData.Device.CardId);
                if (includeCoupons && AppData.Device.UserLoggedOnToDevice.PublishedOffers.Count > 0)
                {
                    xml += "<coupons>";
                    foreach (var coupon in AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code == OfferDiscountType.Coupon))
                    {
                        if (coupon.Selected)
                            xml += string.Format("<cid>{0}</cid>", coupon.Id);
                    }

                    xml += "</coupons>";
                    xml += "<offers>";
                    foreach (var offer in AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon))
                    {
                        if (offer.Selected)
                            xml += string.Format("<oid>{0}</oid>", offer.Id);
                    }
                    xml += "</offers>";
                }

                xml += "</mobiledevice>";
                return xml;
            }
        }

        public class BroadcastUtils
        {
            public static readonly string DrawerOpened = "presentation.utils.drawerOpened";
            public static readonly string DrawerClosed = "presentation.utils.drawerClosed";

            public const string DomainModelUpdated = "Com.Lsretail.Omni.DomainUpdated";
            public const string ShoppingListDeleted = "Com.Lsretail.Omni.ShoppingListDeleted";
            public const string ShoppingListsUpdated = "Com.Lsretail.Omni.ShoppingListsUpdated";
            public const string ShoppingListUpdated = "Com.Lsretail.Omni.ShoppingListUpdated";
            public const string NotificationsUpdated = "Com.Lsretail.Omni.NotificationsUpdated";
            public const string OffersUpdated = "Com.Lsretail.Omni.OffersUpdated";
            public const string CouponsUpdated = "Com.Lsretail.Omni.CouponsUpdated";
            public const string ItemCategoriesUpdated = "Com.Lsretail.Omni.ItemCategoriesUpdated";
            public const string ItemCategoriesUpdateFailed = "Com.Lsretail.Omni.ItemCategoriesUpdateFailed";
            public const string BasketStateUpdated = "Com.Lsretail.Omni.BasketStateUpdated";
            public const string OpenBasket = "Com.Lsretail.Omni.OpenBasket";
            public const string AdvertisementsUpdated = "Com.Lsretail.Omni.AdvertisementsUpdated";
            public const string PointsUpdated = "Com.Lsretail.Omni.PointsUpdated";

            public static void SendBroadcast(Context context, string action)
            {
                var intent = new Intent();
                intent.SetAction(action);

                context.SendBroadcast(intent);
            }

            public static string[] BroadcastActions
            {
                get
                {
                    return new string[]
                    {
                        DrawerClosed,
                        DrawerOpened,
                        DomainModelUpdated,
                        ShoppingListDeleted,
                        ShoppingListsUpdated,
                        ShoppingListUpdated,
                        NotificationsUpdated,
                        OffersUpdated,
                        CouponsUpdated,
                        ItemCategoriesUpdated,
                        ItemCategoriesUpdateFailed,
                        BasketStateUpdated,
                        OpenBasket,
                        AdvertisementsUpdated,
                        PointsUpdated,
                    };
                }
            }
        }

        public class BundleUtils
        {

        }
    }
}
