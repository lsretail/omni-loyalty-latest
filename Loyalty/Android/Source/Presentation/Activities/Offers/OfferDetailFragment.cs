using System;
using System.Collections.Generic;
using System.Linq;

using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.Image;
using Presentation.Activities.Items;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using ImageView = Android.Widget.ImageView;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;

namespace Presentation.Activities.Offers
{
    public class OfferDetailFragment : LoyaltyFragment, View.IOnClickListener, ViewTreeObserver.IOnGlobalLayoutListener, IBroadcastObserver, IItemClickListener, IRefreshableActivity
    {
        private string offerId;
        private PublishedOffer offer;
        private OfferModel offerModel;
        private ItemModel itemModel;

        private List<LoyItem> relatedItems; 

        private TextView offerText;
        private TextView offerSecondaryText;
        private TextView expiryDate;
        private TextView offerDetailText;
        private View offerDetailHeader;
        private Android.Support.Design.Widget.FloatingActionButton useFabButton;
        private DetailImagePager detailImagePager;

        private View relatedItemsSpinner;
        private TextView relatedItemsHeader;
        private RecyclerView relatedItemsRecyclerView;
        private RelatedItemAdapter relatedItemsAdapter;

        public static OfferDetailFragment NewInstance()
        {
            var offerDetail = new OfferDetailFragment() { Arguments = new Bundle() };
            return offerDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstance)
        {
            Bundle data = Arguments;

            offerModel = new OfferModel(Activity);
            itemModel = new ItemModel(Activity, this);

            offerId = data.GetString(BundleConstants.OfferId);

            offer = AppData.Device.UserLoggedOnToDevice.PublishedOffers.FirstOrDefault(o => o.Id == offerId);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.OfferDetail);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.OfferDetailScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            offerText = view.FindViewById<TextView>(Resource.Id.OfferTitleText);
            offerSecondaryText = view.FindViewById<TextView>(Resource.Id.OfferBodyText);
            expiryDate = view.FindViewById<TextView>(Resource.Id.OfferExpiresText);
            offerDetailText = view.FindViewById<TextView>(Resource.Id.OfferDetailText);
            offerDetailHeader = view.FindViewById<View>(Resource.Id.OfferDetailHeader);
            useFabButton = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.OfferUseFab);

            useFabButton.SetOnClickListener(this);
            
            relatedItemsHeader = view.FindViewById<TextView>(Resource.Id.OfferRelatedItemsHeader);
            relatedItemsSpinner = view.FindViewById(Resource.Id.OfferRelatedItemsLoadingSpinner);
            relatedItemsRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.OfferRelatedItems);
            relatedItemsAdapter = new RelatedItemAdapter(Activity, this);

            relatedItemsRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
            relatedItemsRecyclerView.HasFixedSize = true;
            relatedItemsRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.HorizontalItemDecoration(Activity));
            relatedItemsRecyclerView.SetAdapter(relatedItemsAdapter);

            relatedItemsRecyclerView.VerticalScrollBarEnabled = false;

            Util.Utils.ViewUtils.AddOnGlobalLayoutListener(view, this);

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

        public void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.OffersUpdated:
                case Utils.BroadcastUtils.DomainModelUpdated:
                    RefreshOffer();
                    break;
            }
        }

        public void OnGlobalLayout()
        {
            SetOfferData();

            Util.Utils.ViewUtils.RemoveOnGlobalLayoutListener(View, this);
        }

        private async void SetOfferData()
        {
            offerText.Text = offer.Description;
            offerSecondaryText.Text = offer.Details;

            //todo
            if (false/*offer.HasDetails*/)
            {
                offerDetailText.Text = offer.Details;
                offerDetailText.Visibility = ViewStates.Visible;
                offerDetailHeader.Visibility = ViewStates.Visible;
            }
            else
            {
                offerDetailText.Visibility = ViewStates.Gone;
                offerDetailHeader.Visibility = ViewStates.Gone;
            }

            if (offer.ExpirationDate.HasValue)
                expiryDate.Text = string.Format(GetString(Resource.String.DetailViewExpires), offer.ExpirationDate.Value.ToShortTimeString(), offer.ExpirationDate.Value.ToShortDateString());
            else
                expiryDate.Text = GetString(Resource.String.DetailViewNeverExpires);

            SetOfferButtonText();

            if (offer.Type != OfferType.PointOffer)
            {
                useFabButton.Visibility = ViewStates.Gone;

                var padding = Resources.GetDimensionPixelSize(Resource.Dimension.BasePadding);

                offerText.SetPadding(padding, offerText.PaddingTop, offerText.PaddingRight, offerText.PaddingBottom);
                expiryDate.SetPadding(padding, expiryDate.PaddingTop, expiryDate.PaddingRight, expiryDate.PaddingBottom);
                offerSecondaryText.SetPadding(padding, offerSecondaryText.PaddingTop, offerSecondaryText.PaddingRight, offerSecondaryText.PaddingBottom);
            }

            if (detailImagePager == null)
            {
                detailImagePager = new DetailImagePager(View, ChildFragmentManager, offer.Images);
            }

            relatedItems = await itemModel.ItemsGetByPublishedOfferIdAsync(offer.Id);

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
            intent.SetClass(Activity, typeof (ItemActivity));
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

        public void RefreshOffer()
        {
            offer = AppData.Device.UserLoggedOnToDevice.PublishedOffers.FirstOrDefault(x => x.Id == offerId);

            SetOfferData();

            Activity.InvalidateOptionsMenu();
        }

        public void OnClick(View v)
        {
            if (!offer.Selected)
            {
                var interpolator = new OvershootInterpolator();

                var scaleAnimatorX = ObjectAnimator.OfFloat(useFabButton,
                    "scaleX", 0.5f, 1f);
                var scaleAnimatorY = ObjectAnimator.OfFloat(useFabButton,
                    "scaleY", 0.5f, 1f);

                scaleAnimatorX.SetInterpolator(interpolator);
                scaleAnimatorY.SetInterpolator(interpolator);

                var animatorSetXY = new AnimatorSet();
                animatorSetXY.PlayTogether(scaleAnimatorX, scaleAnimatorY);

                animatorSetXY.Start();
            }

            offerModel.ToggleOffer(offer);
            SetOfferButtonText();
        }

        private void SetOfferButtonText()
        {
            if (offer.Selected)
            {
                useFabButton.SetImageResource(Resource.Drawable.ic_action_remove);
            }
            else
            {
                useFabButton.SetImageResource(Resource.Drawable.ic_action_new);
            }
        }
    }
}