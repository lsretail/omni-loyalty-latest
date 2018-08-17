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
    public class OfferDetailFragment : BaseFragment, View.IOnClickListener
    {
        private PublishedOffer offer;

        private ViewPager imagePager;
        private View imageHeader;
        private FloatingActionButton addToBasket;
        private LinearLayout qrCodeLayout;

        private BasketModel basketModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.OfferDetailScreen, null);

            var toolBar = view.FindViewById<Toolbar>(Resource.Id.OfferDetailScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolBar);

            basketModel = new BasketModel(Activity);

            var offerId = Arguments.GetString(BundleUtils.OfferId);
            offer = AppData.Contact.PublishedOffers.FirstOrDefault(x => x.Id == offerId);

            var contentView = view.FindViewById(Resource.Id.OfferDetailScreenContent);
            contentView.SetMinimumHeight(Utils.Utils.ViewUtils.GetContentViewHeight(Activity));

            if (!AppData.HasBasket)
            {
                offer.Selected = false;
            }

            view.FindViewById<TextView>(Resource.Id.OfferDetailScreenItemName).Text = offer.Description;
            view.FindViewById<TextView>(Resource.Id.OfferDetailScreenItemDescription).Text = offer.Details;

            var expirationDate = view.FindViewById<TextView>(Resource.Id.OfferDetailScreenItemExpiration);
            if (string.IsNullOrEmpty(offer.ValidationText))
            {
                expirationDate.Visibility = ViewStates.Gone;
            }
            else
            {
                expirationDate.Text = offer.ValidationText;
            }

            var offerCode = new BasketQrCode(AppData.MobileMenu);
            offerCode.Contact = AppData.Contact;

            //todo
            //offerCode.Coupons = AppData.Contact.Coupons;
            
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

            imagePager.Adapter = new ImagePagerAdapter(ChildFragmentManager, offer.Images, Resources.DisplayMetrics.WidthPixels, Resources.GetDimensionPixelSize(Resource.Dimension.HeaderImageHeight), animationImageId);

            var indicator = view.FindViewById<LinePageIndicator>(Resource.Id.HeaderImageIndicator);
            if (offer.Images != null && offer.Images.Count > 1)
            {
                indicator.SetViewPager(imagePager);
            }
            else
            {
                indicator.Visibility = ViewStates.Gone;
            }

            addToBasket = view.FindViewById<FloatingActionButton>(Resource.Id.OfferDetailScreenAddRemove);

            if (offer.Type == OfferType.PointOffer)
            {
                var qrCode = view.FindViewById<ImageView>(Resource.Id.OfferDetailScreenQrCode);
                qrCode.SetImageBitmap(Utils.Utils.GenerateQrCode(offerCode.Serialize()));

                qrCodeLayout = view.FindViewById<LinearLayout>(Resource.Id.OfferDetailScreenQrCodeContainer);
                
                UpdateAddToBasketButton();
                addToBasket.SetOnClickListener(this);
            }
            else
            {
                addToBasket.SetBackgroundResource(Resource.Color.transparent);
                ViewCompat.SetElevation(addToBasket, 0);
                addToBasket.Enabled = false;
                addToBasket.Visibility = ViewStates.Gone;
                addToBasket.TranslationX = 2000;
                view.FindViewById(Resource.Id.OfferDetailScreenQrCodeContainer).Visibility = ViewStates.Gone;
            }

            return view;
        }

        private void UpdateAddToBasketButton()
        {
            if (AppData.HasBasket)
            {
                var existingOffer = AppData.Basket.PublishedOffers.FirstOrDefault(x => x.Id == offer.Id);

                if (existingOffer == null)
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
                if (offer.Selected)
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
                case Resource.Id.OfferDetailScreenAddRemove:
                    basketModel.ToggleOffer(offer);

                    UpdateAddToBasketButton();

                    break;
            }
        }
    }
}