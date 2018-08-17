using System;
using System.Linq;

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using ImageView = Android.Widget.ImageView;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Activities.Items
{
    public class ItemGroupsFragment : CardListFragment, IItemClickListener, View.IOnClickListener
    {
        private RecyclerView productGroupRecyclerView;
        private CollapsingToolbarLayout collapsingToolbar;
        private ImageView imageHeader;
        private View imageHeaderContainer;
        private Toolbar toolbar;

        private ItemGroupAdapter adapter;
        private ItemCategory itemCategory;

        private ImageModel imageModel;

        public static ItemGroupsFragment NewInstance()
        {
            var itemGroup = new ItemGroupsFragment() { Arguments = new Bundle() };
            return itemGroup;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            Bundle data = Arguments;
            var categoryId = data.GetString(BundleConstants.ItemCategoryId);
            itemCategory = AppData.ItemCategories.First(x => x.Id == categoryId);

            imageModel = new ImageModel(Activity, null);

            var view = base.OnCreateView(inflater, container, savedInstanceState);

            adapter = new ItemGroupAdapter(Activity, this, BaseRecyclerAdapter.ListItemSize.Normal);

            productGroupRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.ItemGroupViewList);
            SetLayoutManager(ShowAsList);
            productGroupRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            productGroupRecyclerView.HasFixedSize = true;

            productGroupRecyclerView.SetAdapter(adapter);
            adapter.SetProductGroups(itemCategory.ProductGroups);

            toolbar = view.FindViewById<Toolbar>(Resource.Id.ItemGroupScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            collapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.ItemGroupScreenCollapsingToolbar);
            imageHeader = view.FindViewById<ImageView>(Resource.Id.ItemGroupScreenHeader);
            imageHeaderContainer = view.FindViewById<View>(Resource.Id.ItemGroupImageContainer);

            if (itemCategory.Images != null && itemCategory.Images.Count > 0)
            {
                imageHeaderContainer.SetOnClickListener(this);

                imageHeaderContainer.SetBackgroundColor(Color.ParseColor(itemCategory.Images[0].GetAvgColor()));

                LoadImage();
            }

            collapsingToolbar.SetTitle(itemCategory.Description);

            return view;
        }

        private async void LoadImage()
        {
            var image = await imageModel.ImageGetById(itemCategory.Images[0].Id, new ImageSize(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.WidthPixels));

            if (image != null)
            {
                imageHeader.SetImageBitmap(Utils.ImageUtils.DecodeImage(image.Image));
                imageHeader.Activated = true;
            }
        }

        public override View CreateView(LayoutInflater inflater)
        {
            return Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ItemGroups);
        }

        public void SetLayoutManager(bool showAsList)
        {
            if (showAsList)
            {
                productGroupRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.Normal);
            }
            else
            {
                productGroupRecyclerView.SetLayoutManager(new GridLayoutManager(Activity, Resources.GetInteger(Resource.Integer.CardColumns), LinearLayoutManager.Vertical, false));
                adapter.SetCardMode(Activity, BaseRecyclerAdapter.ListItemSize.SmallCard);
            }
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var intent = new Intent();

            intent.PutExtra(BundleConstants.ItemCategoryId, itemCategory.Id);
            intent.PutExtra(BundleConstants.ItemgroupId, id);

            intent.SetClass(Activity, typeof(ItemsActivity));
            StartActivity(intent);
        }

        public void OnClick(View v)
        {
            //todo image click
        }
    }
}