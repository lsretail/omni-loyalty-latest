using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Menu;
using Presentation.Activities.Offer;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;

namespace Presentation.Activities.Checkout
{
    public class ConfirmCheckoutFragment : BaseFragment, IItemClickListener, IBroadcastObserver
    {private RecyclerView headers;

        private ConfirmCheckoutAdapter adapter;
        private int columns;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            columns = Resources.GetInteger(Resource.Integer.StaggeredGridColumnCount);

            var view = Inflate(inflater, Resource.Layout.ConfirmCheckoutScreen, null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.ConfirmCheckoutScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            headers = view.FindViewById<RecyclerView>(Resource.Id.ConfirmCheckoutScreenList);

            adapter = new ConfirmCheckoutAdapter(Activity, this, columns);

            var layoutManager = new StaggeredGridLayoutManager(columns, StaggeredGridLayoutManager.Vertical);

            headers.SetLayoutManager(layoutManager);
            headers.SetItemAnimator(new DefaultItemAnimator());
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));
            headers.SetAdapter(adapter);

            adapter.SetBasket(AppData.Basket);

            UpdatePrice();

            return view;
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            if (type == ItemType.Coupon)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(CouponDetailActivity));
                intent.PutExtra(BundleUtils.CouponId, id);
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.ConfirmCheckoutListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
            }
            else if (type == ItemType.Offer)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(OfferDetailActivity));
                intent.PutExtra(BundleUtils.OfferId, id);
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.ConfirmCheckoutListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
            }
            else if (type == ItemType.Item)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(MenuItemActivity));
                intent.PutExtra(BundleUtils.ItemId, id);
                intent.PutExtra(BundleUtils.MenuId, id2);
                intent.PutExtra(BundleUtils.Type, itemType);
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.ConfirmCheckoutListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                //ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
                ActivityUtils.StartActivityForResultWithAnimation(Activity, HospActivity.OpenMenuItemRequestCode, intent, view);
            }
            else if (type == ItemType.Checkout)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(CheckoutActivity));

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

            Update();
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.BasketUpdated ||
                action == BroadcastUtils.BasketItemChanged ||
                action == BroadcastUtils.BasketItemDeleted ||
                action == BroadcastUtils.BasketItemInserted)
            {
                Update();
            }
            else if (action == BroadcastUtils.BasketPriceUpdated)
            {
                UpdatePrice();
            }
        }

        private void Update()
        {
            adapter.SetBasket(AppData.Basket);
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            if (AppData.Basket.Items.Count == 0)
            {
                Activity.Finish();
            }
            else
            {
                adapter.UpdatePrice(AppData.FormatCurrency(AppData.Basket.Amount));
            }
        }
    }
}