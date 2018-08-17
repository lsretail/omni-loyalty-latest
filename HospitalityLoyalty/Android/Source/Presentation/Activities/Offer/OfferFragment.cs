using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;

namespace Presentation.Activities.Offer
{
    public class OfferFragment : BaseFragment, IRefreshableActivity, IItemClickListener, SwipeRefreshLayout.IOnRefreshListener, IBroadcastObserver
    {
        private RecyclerView headers;
        private SwipeRefreshLayout refreshView;
        private View emptyView;

        private OfferModel model;
        private OfferAdapter adapter;
        private int columns;
        private GridLayoutManager layoutManager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            columns = Resources.GetInteger(Resource.Integer.GridColumnCount);

            var view = Inflate(inflater, Resource.Layout.OfferScreen, null);

            model = new OfferModel(Activity, this);

            headers = view.FindViewById<RecyclerView>(Resource.Id.OfferScreenList);

            adapter = new OfferAdapter(Activity, this, columns);

            layoutManager = new GridLayoutManager(Activity, columns);//, StaggeredGridLayoutManager.Vertical);
            layoutManager.SetSpanSizeLookup(new GridSpanSizeLookup(adapter, columns));

            headers.SetLayoutManager(layoutManager);
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));

            headers.SetAdapter(adapter);

            adapter.SetOffers(AppData.Contact.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList());

            refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.OfferScreenListRefreshContainer);
            refreshView.SetColorSchemeResources(Resource.Color.accent);
            refreshView.SetOnRefreshListener(this);

            emptyView = view.FindViewById<View>(Resource.Id.OfferScreenEmptyView);

            SetVisibilities();

            return view;
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            if (type == ItemType.Offer)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(OfferDetailActivity));
                intent.PutExtra(BundleUtils.OfferId, id);
                intent.PutExtra(BundleUtils.Ids, adapter.GetIds().ToArray());
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.CouponListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }

            adapter.SetOffers(AppData.Contact.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList());
            SetVisibilities();
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public void OnRefresh()
        {
            model.PublishedOffersGetByCardId();
        }

        public void ShowIndicator(bool show)
        {
            refreshView.Refreshing = show;

            if (Activity is HospActivity)
            {
                if (show)
                {
                    (Activity as HospActivity).ShowLoadingMessage(Resources.GetString(Resource.String.OffersRefreshingOffers));
                }
                else
                {
                    (Activity as HospActivity).RemoveLoadingMessage();
                }
            }
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.ContactUpdated || action == BroadcastUtils.OffersUpdated)
            {
                adapter.SetOffers(AppData.Contact.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList());
                SetVisibilities();
            }
        }

        public void SetVisibilities()
        {
            if (AppData.Contact == null || AppData.Contact.PublishedOffers.Count(x => x.Code != OfferDiscountType.Coupon) == 0)
            {
                emptyView.Visibility = ViewStates.Visible;
            }
            else
            {

                emptyView.Visibility = ViewStates.Gone;
            }
        }
    }
}