using System;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Activities.Search;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.Offers
{
    public class OfferFragment : CardListFragment, IRefreshableActivity, SwipeRefreshLayout.IOnRefreshListener, IBroadcastObserver, IItemClickListener
    {
        private RecyclerView offeRecyclerView;
        private SwipeRefreshLayout refreshLayout;

        private OfferAdapter adapter;
        private OfferModel model;

        public static OfferFragment NewInstance()
        {
            var offerFragment = new OfferFragment() { Arguments = new Bundle() };
            return offerFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstance)
        {
            if (container == null)
            {
                return null;
            }

            HasOptionsMenu = true;

            var view = base.OnCreateView(inflater, container, savedInstance);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.OfferScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            model = new OfferModel(Activity, this);

            ShowAsList = Util.Utils.PreferenceUtils.GetBool(Activity, Util.Utils.PreferenceUtils.ShowListAsList);

            refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.OfferViewRefreshLayout);

            adapter = new OfferAdapter(Activity, this, OnAddRemoveClicked, BaseRecyclerAdapter.ListItemSize.Normal);
            adapter.SetOffers(Activity, AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList());

            offeRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.OfferViewCouponList);
            SetLayoutManager(ShowAsList);
            offeRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            offeRecyclerView.HasFixedSize = true;

            offeRecyclerView.SetAdapter(adapter);

            refreshLayout.SetColorSchemeResources(Resource.Color.offers, Resource.Color.accent, Resource.Color.offers, Resource.Color.accent);
            refreshLayout.SetOnRefreshListener(this);

            //Util.Utils.ViewUtils.AddOnGlobalLayoutListener(view, this);

            return view;
        }

        public override View CreateView(LayoutInflater inflater)
        {
            return Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Offers, null);
        }

        private void OnAddRemoveClicked(string offerId)
        {
            var offer = AppData.Device.UserLoggedOnToDevice.PublishedOffers.FirstOrDefault(x => x.Id == offerId);

            if (offer != null)
            {
                model.ToggleOffer(offer);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).AddObserver(this);
            }

            RefreshOfferList();
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
                    RefreshOfferList();
                    break;
            }
        }

        public override void OnDestroyView()
        {
            model.Stop();
            base.OnDestroyView();
        }

        public void SetLayoutManager(bool showAsList)
        {
            if (showAsList && !IsBigScreen)
            {
                offeRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.Normal);
            }
            else
            {
                var manager = new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.CardColumns), LinearLayoutManager.Vertical, false);
                manager.SetSpanSizeLookup(new BaseRecyclerAdapter.GridSpanSizeLookup(Activity, adapter));
                offeRecyclerView.SetLayoutManager(manager);
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.SmallCard);
            }
        }

        public void RefreshOfferList()
        {
            adapter.SetOffers(Activity, AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon).ToList());
        }

        public void OnRefresh()
        {
            RefreshOffers();
        }

        private async void RefreshOffers()
        {
            await model.GetOffersByCardId(AppData.Device.CardId);
        }

        public void ShowIndicator(bool show)
        {
            refreshLayout.Refreshing = show;
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var intent = new Intent();
            intent.PutExtra(BundleConstants.OfferId, id);
            intent.SetClass(Activity, typeof(OfferDetailActivity));
            StartActivity(intent);
        }

        #region MENU
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.GenericSearchMenu, menu);
            inflater.Inflate(Resource.Menu.OfferMenu, menu);

            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewGenerateQR:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(QRCode.QRCodeActivity));

                    StartActivity(intent);

                    return true;

                case Resource.Id.MenuViewSearch:
                    var searchIntent = new Intent();
                    searchIntent.SetClass(Activity, typeof(GeneralSearchActivity));
                    searchIntent.PutExtra(BundleConstants.SearchType, (int)SearchType.Offer);

                    StartActivity(searchIntent);

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}