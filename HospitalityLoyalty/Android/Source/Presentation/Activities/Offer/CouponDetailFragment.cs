using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Base;
using Presentation.Activities.Image;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using Xamarin.ViewPagerIndicator;
using ImageView = Android.Widget.ImageView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Offer
{
    public class CouponDetailFragment : BaseFragment, View.IOnClickListener
    {
        private PublishedOffer coupon;

        private ViewPager imagePager;
        private View imageHeader;
        private FloatingActionButton addToBasket;
        private LinearLayout qrCodeLayout;

        private BasketModel basketModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.CouponDetailScreen, null);

            var toolBar = view.FindViewById<Toolbar>(Resource.Id.CouponDetailScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolBar);

            basketModel = new BasketModel(Activity);

            var couponId = Arguments.GetString(BundleUtils.CouponId);
            coupon = AppData.Contact.PublishedOffers.FirstOrDefault(x => x.Id == couponId);

            var contentView = view.FindViewById(Resource.Id.CouponDetailScreenContent);
            contentView.SetMinimumHeight(Utils.Utils.ViewUtils.GetContentViewHeight(Activity));

            if (!AppData.HasBasket)
            {
                coupon.Selected = false;
            }

            view.FindViewById<TextView>(Resource.Id.CouponDetailScreenItemName).Text = coupon.Description;
            view.FindViewById<TextView>(Resource.Id.CouponDetailScreenItemDescription).Text = coupon.Details;

            var expirationDate = view.FindViewById<TextView>(Resource.Id.CouponDetailScreenItemExpiration);
            if (string.IsNullOrEmpty(coupon.ValidationText))
            {
                expirationDate.Visibility = ViewStates.Gone;
            }
            else
            {
                expirationDate.Text = coupon.ValidationText;
            }

            var couponCode = new BasketQrCode(AppData.MobileMenu);
            couponCode.Contact = AppData.Contact;

            couponCode.PublishedOffers = AppData.Contact.PublishedOffers.Where(x => x.Selected).ToList();

            var qrCode = view.FindViewById<ImageView>(Resource.Id.CouponDetailScreenQrCode);
            qrCode.SetImageBitmap(Utils.Utils.GenerateQrCode(couponCode.Serialize()));

            qrCodeLayout = view.FindViewById<LinearLayout>(Resource.Id.CouponDetailScreenQrLayout);

            addToBasket = view.FindViewById<FloatingActionButton>(Resource.Id.CouponDetailScreenAddRemove);
            
            imagePager = view.FindViewById<ViewPager>(Resource.Id.HeaderImageViewPager);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                imagePager.SetPageTransformer(true, new ZoomOutPageTransformer());
            }

            imageHeader = view.FindViewById<View>(Resource.Id.HeaderImageContainer);
            imageHeader.SetOnClickListener(this);

            var animationImageId = string.Empty;
            if (Arguments.ContainsKey(BundleUtils.AnimationImageId))
            {
                animationImageId = Arguments.GetString(BundleUtils.AnimationImageId);
            }

            imagePager.Adapter = new ImagePagerAdapter(ChildFragmentManager, coupon.Images, Resources.DisplayMetrics.WidthPixels, Resources.GetDimensionPixelSize(Resource.Dimension.HeaderImageHeight), animationImageId);

            var indicator = view.FindViewById<LinePageIndicator>(Resource.Id.HeaderImageIndicator);
            if (coupon.Images != null && coupon.Images.Count > 1)
            {
                indicator.SetViewPager(imagePager);
            }
            else
            {
                indicator.Visibility = ViewStates.Gone;
            }

            UpdateAddToBasketButton();
            addToBasket.SetOnClickListener(this);

            return view;
        }

        private void UpdateAddToBasketButton()
        {
            if (AppData.HasBasket)
            {
                var existingCoupon = AppData.Basket.PublishedOffers.FirstOrDefault(x => x.Id == coupon.Id);

                if (existingCoupon == null)
                {
                    addToBasket.SetImageResource(Resource.Drawable.ic_add_black_24dp);
                }
                else
                {
                    addToBasket.SetImageResource(Resource.Drawable.ic_remove_black_24dp);
                }
            }
            else
            {
                if (coupon.Selected)
                {
                    addToBasket.SetImageResource(Resource.Drawable.ic_remove_black_24dp);

                    qrCodeLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    addToBasket.SetImageResource(Resource.Drawable.ic_add_black_24dp);

                    qrCodeLayout.Visibility = ViewStates.Gone;
                }
            }

            var favoriteDrawable = DrawableCompat.Wrap(addToBasket.Drawable);
            DrawableCompat.SetTint(favoriteDrawable, new Color(ContextCompat.GetColor(Activity, Resource.Color.white87)));
            DrawableCompat.Unwrap(favoriteDrawable);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.CouponDetailScreenAddRemove:
                    basketModel.ToggleCoupon(coupon);

                    UpdateAddToBasketButton();
                    break;
            }
        }
    }
}