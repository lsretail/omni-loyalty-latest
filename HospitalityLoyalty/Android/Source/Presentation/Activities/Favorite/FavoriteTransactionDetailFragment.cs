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
using Presentation.Adapters;
using Presentation.Dialog;
using Presentation.Models;
using Presentation.Utils;

namespace Presentation.Activities.Favorite
{
    public class FavoriteTransactionDetailFragment : BaseFragment, IItemClickListener, IBroadcastObserver
    {
        private RecyclerView headers;
        private LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions.Transaction transaction;

        private BasketModel model;
        private FavoriteModel favoriteModel;

        private FavoriteTransactionDetailAdapter adapter;
        private int columns;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;

            columns = Resources.GetInteger(Resource.Integer.StaggeredGridColumnCount);

            var favoriteId = Arguments.GetString(BundleUtils.FavoriteId);
            transaction = AppData.Favorites.FirstOrDefault(x => x.Id == favoriteId) as LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions.Transaction;

            var view = Inflate(inflater, Resource.Layout.FavoriteTransactionDetailScreen, null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.FavoriteTransactionDetailScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            model = new BasketModel(Activity);
            favoriteModel = new FavoriteModel(Activity);

            headers = view.FindViewById<RecyclerView>(Resource.Id.FavoriteTransactionDetailScreenList);

            adapter = new FavoriteTransactionDetailAdapter(Activity, this, columns);

            var layoutManager = new StaggeredGridLayoutManager(columns, StaggeredGridLayoutManager.Vertical);
            
            headers.SetLayoutManager(layoutManager);
            headers.SetItemAnimator(new DefaultItemAnimator());
            headers.AddItemDecoration(new Utils.DividerItemDecoration(Activity));

            headers.SetAdapter(adapter);

            adapter.SetTransaction(transaction);

            return view;
        }

        public async void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "")
        {
            if (type == ItemType.Item)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(MenuItemActivity));

                var item = transaction.SaleLines.FirstOrDefault(x => x.Id == id).Item;

                intent.PutExtra(BundleUtils.ItemId, item.Id);
                intent.PutExtra(BundleUtils.MenuId, item.MenuId);
                intent.PutExtra(BundleUtils.Type, itemType);
                intent.PutExtra(BundleUtils.AnimationImageId, animationImageId);

                //ActivityUtils.StartActivityWithAnimation(Activity, intent, new[] { new Android.Support.V4.Util.Pair(view.FindViewById(Resource.Id.FavoriteTransactionDetailListItemViewItemImage), Resources.GetString(Resource.String.TransitionImage)) });
                //ActivityUtils.StartActivityWithAnimation(Activity, intent, view);
                ActivityUtils.StartActivityForResultWithAnimation(Activity, HospActivity.OpenMenuItemRequestCode, intent, view);
            }
            else if (type == ItemType.Delete)
            {
                await favoriteModel.ToggleFavorite(transaction);
                Activity.OnBackPressed();
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }

            adapter.SetTransaction(transaction);
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            inflater.Inflate(Resource.Menu.FavoriteTransactionDetailMenu, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuItemEdit:
                    var dialog = new EditTextDialog(Activity, Resources.GetString(Resource.String.FavoriteChangeName));
                    dialog.EditText.Hint = Resources.GetString(Resource.String.FavoriteNewName);
                    dialog.SetPositiveButton(Resources.GetString(Resource.String.Ok), async () =>
                    {
                        await favoriteModel.RenameFavorites(transaction, dialog.EditText.Text);
                        adapter.NotifyItemChanged(0);
                    }).SetNegativeButton(Resources.GetString(Resource.String.Cancel), () => { });
                    dialog.Show();

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.FavoritesUpdated)
            {
                adapter.SetTransaction(transaction);
            }
        }
    }
}