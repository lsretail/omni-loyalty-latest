using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Menu
{
    public class BasketFragment : BaseFragment, IBroadcastObserver, View.IOnClickListener, Toolbar.IOnMenuItemClickListener, IItemClickListener
    {
        private const int UpdateBasketItemRequestCode = 201;

        //private ListView headers;
        private RecyclerView headers;
        private TextView total;
        private View emptyView;
        private Toolbar toolbar;

        private BasketAdapter adapter;

        private BasketModel basketModel;
        private FavoriteModel favoriteModel;
        private TransactionModel transactionModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.BasketScreen, null);

            basketModel = new BasketModel(Activity);
            favoriteModel = new FavoriteModel(Activity);
            transactionModel = new TransactionModel(Activity);

            HasOptionsMenu = true;

            toolbar = view.FindViewById<Toolbar>(Resource.Id.BasketScreenToolbar);
            toolbar.Title = GetString(Resource.String.Basket);
            toolbar.InflateMenu(Resource.Menu.BasketOpenMenu);
            toolbar.SetOnMenuItemClickListener(this);

            headers = view.FindViewById<RecyclerView>(Resource.Id.BasketScreenList);
            headers.SetLayoutManager(new LinearLayoutManager(Activity));
            headers.SetItemAnimator(new DefaultItemAnimator());
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));

            //headers.EmptyView = view.FindViewById<View>(Resource.Id.BasketScreenEmptyView);
            emptyView = view.FindViewById<View>(Resource.Id.BasketScreenEmptyView);

            //headers.ChoiceMode = ChoiceMode.None;

            view.FindViewById<ColoredButton>(Resource.Id.BasketScreenStartOrdering).SetOnClickListener(this);
            view.FindViewById<ColoredButton>(Resource.Id.BasketScreenCheckout).SetOnClickListener(this); 

            adapter = new BasketAdapter(
                Activity, 
                this,
                (pos) =>
                {
                    headers.SmoothScrollToPosition(pos);
                });
            headers.SetAdapter(adapter);

            total = view.FindViewById<TextView>(Resource.Id.BasketScreenTotal);

            SetFavoriteButton();

            return view;
        }

        public void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            switch (type)
            {
                case ItemType.UpdateBasket:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(MenuItemModificationActivity));
                    intent.PutExtra(BundleUtils.BasketItemId, id);

                    StartActivityForResult(intent, UpdateBasketItemRequestCode);

                    break;
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == UpdateBasketItemRequestCode)
            {
                if (resultCode == (int) Result.Ok)
                {
                    BaseModel.ShowToast(View, Resource.String.BasketItemUpdated);
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void SetFavoriteButton()
        {
            var favoriteButton = toolbar.Menu.FindItem(Resource.Id.MenuViewFavorite);

            if (AppData.Basket.Items != null && AppData.Basket.Items.Count > 0)
            {
                if (favoriteModel.IsFavorite(transactionModel.CreateTransaction()))
                {
                    favoriteButton.SetIcon(Resource.Drawable.ic_favorite_white_24dp);
                    favoriteButton.SetTitle(Resource.String.MenuItemRemoveFromFavorites);
                }
                else
                {
                    favoriteButton.SetIcon(Resource.Drawable.ic_favorite_border_white_24dp);
                    favoriteButton.SetTitle(Resource.String.MenuItemAddToFavorites);
                }

                favoriteButton.SetVisible(true);
            }
            else
            {
                favoriteButton.SetVisible(false);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is HospActivity)
                (Activity as HospActivity).AddObserver(this);

            Update(true);
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
                (Activity as HospActivity).RemoveObserver(this);

            base.OnPause();
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.BasketUpdated || action == BroadcastUtils.BasketItemInserted || action == BroadcastUtils.FavoritesUpdated || action == BroadcastUtils.FavoritesUpdatedInList)
            {
                Update(true);

                Activity.SupportInvalidateOptionsMenu();
            }
            if (action == BroadcastUtils.BasketItemChanged || action == BroadcastUtils.BasketItemDeleted || action == BroadcastUtils.BasketPriceUpdated)
            {
                Update(false);

                Activity.SupportInvalidateOptionsMenu();
            }
            if (action == BroadcastUtils.DrawerOpened || action == BroadcastUtils.DrawerClosed)
            {
                Activity.SupportInvalidateOptionsMenu();
            }
        }

        private void Update(bool updateListItems)
        {
            if (updateListItems)
            {
                adapter.CreateItems();
                headers.GetAdapter().NotifyDataSetChanged();
            }

            UpdateTotal();

            SetFavoriteButton();

            if (adapter.ItemCount == 0)
            {
                headers.Visibility = ViewStates.Gone;
                emptyView.Visibility = ViewStates.Visible;
            }
            else
            {
                headers.Visibility = ViewStates.Visible;
                emptyView.Visibility = ViewStates.Gone;
            }
        }

        private void UpdateMenu()
        {
            toolbar.Menu.FindItem(Resource.Id.MenuViewClearBasket).SetVisible(AppData.Basket.Items.Count != 0);
        }

        private void UpdateTotal()
        {
            if (AppData.MobileMenu == null)
            {
                total.Text = AppData.Basket.Amount.ToString();
            }
            else
            {
                total.Text = AppData.FormatCurrency(AppData.Basket.Amount); 
            }

            UpdateMenu();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            inflater.Inflate(Resource.Menu.BasketMenu, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewBasket:
                    if (Activity is HospActivity && (Activity as HospActivity).RightDrawer)
                    {
                        if ((Activity as HospActivity).IsOpen((int)GravityFlags.End))
                        {
                            (Activity as HospActivity).CloseDrawer((int)GravityFlags.End);
                        }
                        else
                        {
                            (Activity as HospActivity).OpenDrawer((int)GravityFlags.End);
                        }
                    }

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewClearBasket:
                    //AppData.Basket.Clear();
                    basketModel.ClearBasket(true);
                    Update(true);

                    UpdateMenu();
                    return true;

                case Resource.Id.MenuViewFavorite:
                    ToggleFavorite();
                    return true;
            }

            return false;
        }

        private async void ToggleFavorite()
        {
            await favoriteModel.ToggleFavorite(transactionModel.CreateTransaction());

            SetFavoriteButton();
        }

        public void OnClick(View v)
        {
            bool closeDrawer = false;

            switch (v.Id)
            {
                case Resource.Id.BasketScreenStartOrdering:
                    if (Activity is HomeActivity)
                    {
                        (Activity as HomeActivity).SelectItem(HospActivity.ActivityTypes.Menu);
                    }
                    else
                    {
                        var upIntent = new Intent();
                        upIntent.SetClass(Activity, typeof (HomeActivity));
                        upIntent.AddFlags(ActivityFlags.ClearTop);
                        upIntent.AddFlags(ActivityFlags.SingleTop);
                        upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int) BundleUtils.ChosenMenu.Menu);

                        StartActivity(upIntent);

                        Activity.Finish();
                    }

                    closeDrawer = true;
                    break;

                case Resource.Id.BasketScreenCheckout:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof (Checkout.ConfirmCheckoutActivity));

                    ActivityUtils.StartActivityWithAnimation(Activity, intent, v);

                    closeDrawer = true;

                    break;
            }

            if (closeDrawer && Activity is HospActivity && (Activity as HospActivity).RightDrawer)
            {
                if ((Activity as HospActivity).IsOpen((int)GravityFlags.End))
                {
                    (Activity as HospActivity).CloseDrawer((int)GravityFlags.End);
                }
            }
        }
    }
}