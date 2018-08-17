using System;
using System.Linq;
using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.Animation;
using Android.Support.V7.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.Image;
using Presentation.Activities.Items;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using ImageView = Android.Widget.ImageView;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation.Activities.Coupons
{
    public class CouponDetailFragment : LoyaltyFragment, View.IOnClickListener, ViewTreeObserver.IOnGlobalLayoutListener, IBroadcastObserver, IRefreshableActivity, IItemClickListener
    {
        private string couponId;
        private PublishedOffer coupon;
        private OfferModel offerModel;
        private ItemModel itemModel;

        private List<LoyItem> relatedItems; 

        //private ColoredButton useCouponButton;
        private TextView couponText;
        private TextView couponSecondary;
        private TextView couponDetail;
        private TextView expiryDate;
        private View couponDetailHeader;
        private Android.Support.Design.Widget.FloatingActionButton useCouponFab;
        private DetailImagePager detailImagePager;

        private View relatedItemsSpinner;
        private TextView relatedItemsHeader;
        private RecyclerView relatedItemsRecyclerView;
        private RelatedItemAdapter relatedItemsAdapter;

        public static CouponDetailFragment NewInstance()
        {
            var couponDetail = new CouponDetailFragment() { Arguments = new Bundle() };
            return couponDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstance)
        {
            Bundle data = Arguments;

            offerModel = new OfferModel(Activity);
            itemModel = new ItemModel(Activity, this);

            couponId = data.GetString(BundleConstants.CouponId);

            coupon = AppData.Device.UserLoggedOnToDevice.PublishedOffers.FirstOrDefault(x => x.Id == couponId);

            HasOptionsMenu = true;

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CouponDetail , null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.CouponDetailScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            couponText = view.FindViewById<TextView>(Resource.Id.CouponTitleText);
            couponSecondary = view.FindViewById<TextView>(Resource.Id.CouponBodyText);
            expiryDate = view.FindViewById<TextView>(Resource.Id.CouponExpiresText);
            couponDetail = view.FindViewById<TextView>(Resource.Id.CouponDetailText);
            couponDetailHeader = view.FindViewById<View>(Resource.Id.CouponDetailHeader);
            //useCouponButton = view.FindViewById<ColoredButton>(Resource.Id.CouponUse);
            useCouponFab = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.CouponUseFab);

            //useCouponButton.SetOnClickListener(this);
            useCouponFab.SetOnClickListener(this);

            relatedItemsHeader = view.FindViewById<TextView>(Resource.Id.CouponRelatedItemsHeader);
            relatedItemsSpinner = view.FindViewById(Resource.Id.CouponRelatedItemsLoadingSpinner);
            relatedItemsRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.CouponRelatedItems);
            relatedItemsAdapter = new RelatedItemAdapter(Activity, this);

            relatedItemsRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
            relatedItemsRecyclerView.HasFixedSize = true;
            relatedItemsRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.HorizontalItemDecoration(Activity));
            relatedItemsRecyclerView.SetAdapter(relatedItemsAdapter);

            Utils.ViewUtils.AddOnGlobalLayoutListener(view, this);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).AddObserver(this);
            }
        }

        public override void OnPause()
        {
            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public void OnGlobalLayout()
        {
            SetCouponData();

            Utils.ViewUtils.RemoveOnGlobalLayoutListener(View, this);
        }

        public void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.CouponsUpdated:
                case Utils.BroadcastUtils.DomainModelUpdated:
                    RefreshCoupon();
                    break;
            }
        }

        private async void SetCouponData()
        {
            couponText.Text = coupon.Description;
            couponSecondary.Text = coupon.Details;

            if (false/*coupon.HasDetails*/)
            {
                couponDetail.Text = coupon.Details;
                couponDetail.Visibility = ViewStates.Visible;
                couponDetailHeader.Visibility = ViewStates.Visible;
            }
            else
            {
                couponDetail.Visibility = ViewStates.Gone;
                couponDetailHeader.Visibility = ViewStates.Gone;
            }
            
            if (coupon.ExpirationDate.HasValue)
                expiryDate.Text = string.Format(GetString(Resource.String.DetailViewExpires), coupon.ExpirationDate.Value.ToShortTimeString(), coupon.ExpirationDate.Value.ToShortDateString());
            else
                expiryDate.Text = GetString(Resource.String.DetailViewNeverExpires);

            SetCouponButtonText();

            if (detailImagePager == null)
            {
                detailImagePager = new DetailImagePager(View, ChildFragmentManager, coupon.Images);
            }

            var publishedOffer = await itemModel.ItemsGetByPublishedOfferIdAsync(coupon.Id);

            relatedItems = publishedOffer;

            if (relatedItems != null && relatedItems.Count > 0)
            {
                relatedItemsAdapter.SetItems(relatedItems);

                relatedItemsHeader.Visibility = ViewStates.Visible;
                relatedItemsRecyclerView.Visibility = ViewStates.Visible;
            }
            else
            {
                relatedItemsHeader.Visibility = ViewStates.Gone;
                relatedItemsRecyclerView.Visibility = ViewStates.Gone;
            }
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var intent = new Intent();
            intent.SetClass(Activity, typeof(ItemActivity));
            intent.PutExtra(BundleConstants.ItemId, id);

            StartActivity(intent);
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                relatedItemsSpinner.Visibility = ViewStates.Visible;
            }
            else
            {
                relatedItemsSpinner.Visibility = ViewStates.Gone;
            }
        }

        public void RefreshCoupon()
        {
            coupon = AppData.Device.UserLoggedOnToDevice.PublishedOffers.FirstOrDefault(x => x.Id == couponId);

            SetCouponData();

            Activity.SupportInvalidateOptionsMenu();
        }

        public void OnClick(View v)
        {
            if (!coupon.Selected)
            {
                var interpolator = new OvershootInterpolator();

                var scaleAnimatorX = ObjectAnimator.OfFloat(useCouponFab,
                    "scaleX", 0.5f, 1f);
                var scaleAnimatorY = ObjectAnimator.OfFloat(useCouponFab,
                    "scaleY", 0.5f, 1f);

                scaleAnimatorX.SetInterpolator(interpolator);
                scaleAnimatorY.SetInterpolator(interpolator);

                var animatorSetXY = new AnimatorSet();
                animatorSetXY.PlayTogether(scaleAnimatorX, scaleAnimatorY);

                animatorSetXY.Start();
            }

            offerModel.ToggleOffer(coupon);
        }

        private void SetCouponButtonText()
        {
            if (coupon.Selected)
            {
                useCouponFab.SetImageResource(Resource.Drawable.ic_action_remove);
            }
            else
            {
                useCouponFab.SetImageResource(Resource.Drawable.ic_action_new);
            }
        }
    }
}