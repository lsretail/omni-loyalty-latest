using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.Animation;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.Coupons;
using Presentation.Activities.Image;
using Presentation.Activities.Login;
using Presentation.Activities.Offers;
using Presentation.Activities.StoreLocator;
using Presentation.Adapters;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using Presentation.Views;

using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Fragment = Android.Support.V4.App.Fragment;
using ImageView = Android.Widget.ImageView;
using IOException = Java.IO.IOException;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;

namespace Presentation.Activities.Items
{
    public class ItemFragment : LoyaltyFragment, IRefreshableActivity, View.IOnClickListener, ViewTreeObserver.IOnGlobalLayoutListener, IItemClickListener, IBroadcastObserver
    {
        private CollapsingToolbarLayout collapsingToolbar;
        private Toolbar toolbar;

        private bool askedForLocationPermission = false;

        private ItemModel itemModel;
        private OfferModel offerModel;
        private BasketModel basketModel;
        private ShoppingListModel shoppingListModel;
        private DetailImagePager imagePager;

        private string itemId;
        private string barcode;
        public LoyItem Item { get; private set; }

        private VariantRegistration selectedVariant;
        private List<PublishedOffer> relatedPublishedOffers; 

        private TextView itemTitle;
        private TextView itemPrice;
        private TextView itemDetails;
        private TextView selectVariant;
        private View itemDetailsHeader;
        private Button shoppingListQty;
        private Android.Support.Design.Widget.FloatingActionButton wishListButton;
        private ProgressButton addToBasketButton;

        private View relatedPublishedOffersSpinner;
        private TextView relatedPublishedOffersHeader;
        private RecyclerView relatedPublishedOffersRecyclerView;
        private PublishedOfferAdapter relatedPublishedOfferAdapter;

        private ViewSwitcher switcher;
        private View contentView;
        private View loadingView;

        private int currentAddToWishListImage;

        public static Fragment NewInstance()
        {
            var itemGroup = new ItemFragment() { Arguments = new Bundle() };
            return itemGroup;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            HasOptionsMenu = true;

            Bundle data = Arguments;
            itemId = data.GetString(BundleConstants.ItemId);
            if(string.IsNullOrEmpty(itemId))
                barcode = data.GetString(BundleConstants.Barcode);

            View view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Item);

            toolbar = view.FindViewById<Toolbar>(Resource.Id.ItemScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            collapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.ItemScreenCollapsingToolbar);

            itemTitle = view.FindViewById<TextView>(Resource.Id.ItemViewItemTitle);
            itemPrice = view.FindViewById<TextView>(Resource.Id.ItemViewItemPrice);
            itemDetails = view.FindViewById<TextView>(Resource.Id.ItemViewItemDetailText);
            itemDetailsHeader = view.FindViewById<View>(Resource.Id.ItemViewItemDetailHeader);
            selectVariant = view.FindViewById<TextView>(Resource.Id.ItemViewVariants);

            shoppingListQty = view.FindViewById<Button>(Resource.Id.ItemViewChangeQty);
            wishListButton = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.ItemViewAddToWishlistFab);
            var decreaseButton = view.FindViewById<ImageButton>(Resource.Id.ItemViewDecreaseQty);
            var increaseButton = view.FindViewById<ImageButton>(Resource.Id.ItemViewIncreaseQty);
            addToBasketButton = view.FindViewById<ProgressButton>(Resource.Id.ItemViewAddToBasket);
            
            shoppingListQty.SetOnClickListener(this);
            decreaseButton.SetOnClickListener(this);
            increaseButton.SetOnClickListener(this);
            wishListButton.SetOnClickListener(this);

            addToBasketButton.SetOnClickListener(this);
            selectVariant.SetOnClickListener(this);

            relatedPublishedOffersHeader = view.FindViewById<TextView>(Resource.Id.ItemViewItemRelatedOffersHeader);
            relatedPublishedOffersSpinner = view.FindViewById(Resource.Id.ItemViewRelatedOffersLoadingSpinner);
            relatedPublishedOffersRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.ItemViewItemRelatedOffers);
            relatedPublishedOfferAdapter = new PublishedOfferAdapter(Activity, this);

            relatedPublishedOffersRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
            relatedPublishedOffersRecyclerView.HasFixedSize = true;
            relatedPublishedOffersRecyclerView.SetAdapter(relatedPublishedOfferAdapter);

            if (!EnabledItems.HasBasket)
            {
                addToBasketButton.Visibility = ViewStates.Gone;
            }

            if (!EnabledItems.HasBasket)
            {
                view.FindViewById(Resource.Id.ItemViewQtyContainer).Visibility = ViewStates.Gone;
            }

            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.ItemViewSwitcher);
            contentView = view.FindViewById(Resource.Id.ItemViewContent);
            loadingView = view.FindViewById(Resource.Id.ItemViewLoadingSpinner);

            itemModel = new ItemModel(Activity, this);
            offerModel = new OfferModel(Activity, this);
            basketModel = new BasketModel(Activity, this);
            shoppingListModel = new ShoppingListModel(Activity, this);

            if (bundle != null)
            {
                shoppingListQty.Text = bundle.GetDouble(BundleConstants.ItemAddToShoppingQty).ToString();
            }

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

        public void OnGlobalLayout()
        {
            StartLoadingItem();

            Util.Utils.ViewUtils.RemoveOnGlobalLayoutListener(View, this);
        }

        public void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.ShoppingListDeleted:
                case Utils.BroadcastUtils.ShoppingListUpdated:
                case Utils.BroadcastUtils.ShoppingListsUpdated:
                    SetWishListButton();
                    break;
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if (shoppingListQty != null)
            {
                var qty = 1d;
                Double.TryParse(shoppingListQty.Text, out qty);
                outState.PutDouble(BundleConstants.ItemAddToShoppingQty, qty);
            }
        }

        public override void OnDestroyView()
        {
            if(itemModel != null)
                itemModel.Stop();

            base.OnDestroyView();
        }

        private async void StartLoadingItem()
        {
            LoyItem item = null;

            if (!string.IsNullOrEmpty(itemId))
            {
                item = await itemModel.GetItemById(itemId);
            }
            else
            {
                item = await itemModel.GetItemByBarcode(barcode);
            }

            if (item == null)
            {
                GoBackOnNetworkError();
            }
            else
            {
                ItemLoadSuccess(item);
            }
        }

        private void GoBackOnNetworkError()
        {
            Activity.OnBackPressed();
        }

        private void ItemLoadSuccess(LoyItem item)
        {
            this.Item = item;

            LoadItem();
        }

        public void ShowIndicator(bool show)
        {
            if (Item == null || switcher.CurrentView == loadingView)
            {
                if (show)
                    ShowLoading();
                else
                    ShowContent();
            }
            else
            {
                if (show)
                {
                    addToBasketButton.State = ProgressButton.ProgressButtonState.Loading;
                }
                else
                {
                    addToBasketButton.State = ProgressButton.ProgressButtonState.Normal;
                    relatedPublishedOffersSpinner.Visibility = ViewStates.Gone;
                }
            }
        }

        private void ShowLoading()
        {
            if (switcher.CurrentView != loadingView)
                switcher.ShowPrevious();
        }

        private void ShowContent()
        {
            if (switcher.CurrentView != contentView)
                switcher.ShowNext();
        }

        private void LoadItem()
        {
            Activity.SupportInvalidateOptionsMenu();

            itemTitle.Text = Item.Description;
            if (!string.IsNullOrEmpty(Item.Details))
            {
                itemDetails.Text = Item.Details;
            }
            else
            {
                itemDetails.Visibility = ViewStates.Gone;
                itemDetailsHeader.Visibility = ViewStates.Gone;
            }

            if (Item.Prices.Count > 0)
                itemPrice.Text = Item.Prices[0].Amount;
            else
                itemPrice.Visibility = ViewStates.Gone;

            Bundle data = Arguments;

            string selectedVariantId = data.GetString(BundleConstants.SelectedVariantId);

            
            if (Item.VariantsRegistration.Count > 0)
            {
                if (selectedVariant == null)
                {
                    if (Item.VariantsRegistration?.Count > 0)
                    {
                        selectedVariantId = Item.VariantsRegistration[0].Id;

                    }

                    if (!string.IsNullOrEmpty(selectedVariantId))
                    {
                        selectedVariant = Item.VariantsRegistration.FirstOrDefault(x => x.Id == selectedVariantId);

                        if (selectedVariant != null)
                        {
                            VariantExt.SetIsSelectedFromVariantReg(Item.VariantsExt, selectedVariant);
                            selectVariant.Text = selectedVariant.ToString();
                        }
                    }
                }
            }
            else
            {
                selectVariant.Visibility = ViewStates.Gone;
            }

            SetWishListButton();

            LoadImage();

            LoadRelatedPublishedOffers();
        }

        private async void LoadRelatedPublishedOffers()
        {
            if (AppData.Device.UserLoggedOnToDevice == null)
            {
                ShowIndicator(false);
                return;
            }

            relatedPublishedOffers = await offerModel.GetPublishedOffersByItemId(Item.Id, AppData.Device.UserLoggedOnToDevice.Card.Id);

            if (relatedPublishedOffers != null && relatedPublishedOffers.Count > 0)
            {
                relatedPublishedOfferAdapter.SetOffers(Activity, relatedPublishedOffers);

                relatedPublishedOffersHeader.Visibility = ViewStates.Visible;
                relatedPublishedOffersRecyclerView.Visibility = ViewStates.Visible;
            }
            else
            {
                relatedPublishedOffersHeader.Visibility = ViewStates.Gone;
                relatedPublishedOffersRecyclerView.Visibility = ViewStates.Gone;
            }
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var selectedOffer = relatedPublishedOffers.FirstOrDefault(x => x.Id == id);

            if(selectedOffer == null)
                return;

            if (selectedOffer.Code == OfferDiscountType.Coupon)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof (CouponDetailActivity));
                intent.PutExtra(BundleConstants.CouponId, selectedOffer.Id);
                StartActivity(intent);
            }
            else
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(OfferDetailActivity));
                intent.PutExtra(BundleConstants.OfferId, selectedOffer.Id);
                StartActivity(intent);
            }
        }

        private async void AddToBasket(decimal qty)
        {
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(LoginActivity));
                intent.PutExtra(BundleConstants.ErrorMessage, GetString(Resource.String.ApplicationMustBeLoggedIn));

                StartActivity(intent);

                return;
            }

            OneListItem basketItem = new OneListItem() 
            {
                Item = Item.ShallowCopy(), 
                Quantity = qty
            };

            if (selectedVariant != null)
                basketItem.VariantReg = selectedVariant;

            if (basketItem.VariantReg == null && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
            {
                BaseModel.ShowStaticSnackbar(BaseModel.CreateStaticSnackbar(Activity,GetString(Resource.String.ItemViewPickVariant)));
                SelectVariant();
            }
            else
            {
                await basketModel.AddItemToBasket(basketItem);
                addToBasketButton.State = ProgressButton.ProgressButtonState.Done;
            }
        }

        private async Task<bool> AddToWishList()
        {
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof (LoginActivity));
                intent.PutExtra(BundleConstants.ErrorMessage, GetString(Resource.String.ApplicationMustBeLoggedIn));

                StartActivity(intent);

                return false;
            }
            else
            {
                var line = new OneListItem() {Item = Item.ShallowCopy(), Quantity = 1};

                if (selectedVariant != null)
                    line.VariantReg = selectedVariant;

                if (line.VariantReg == null && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
                {
                    BaseModel.ShowStaticSnackbar(BaseModel.CreateStaticSnackbar(Activity, GetString(Resource.String.ItemViewPickVariant)));
                    return false;
                }
                else
                {
                    await shoppingListModel.AddItemToWishList(line);
                }

                return true;
            }
        }

        private void ViewAvailability()
        {
            if(Item == null)
                return;

            var intent = new Intent();

            var itemDescription = Item.Description;
            var variantId = "";
            if (selectedVariant != null)
            {
                itemDescription += " - " + selectedVariant;
                variantId = selectedVariant.Id;
            }

            if (string.IsNullOrEmpty(variantId) && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
            {
                BaseModel.ShowStaticSnackbar(BaseModel.CreateStaticSnackbar(Activity, GetString(Resource.String.ItemViewPickVariant)));
                Activity.RunOnUiThread(() => { });
            }
            else
            {
                intent.PutExtra(BundleConstants.ItemId, Item.Id);
                intent.PutExtra(BundleConstants.VariantId, variantId);
                intent.PutExtra(BundleConstants.ItemDescription, itemDescription);

                intent.SetClass(Activity, typeof(StoreLocatorActivity));
                StartActivity(intent);
            }
        }

        private void CalculatePrice()
        {
            string selectedUomId = "";
            string selectedVariantId = "";
            
            if (selectedVariant != null)
                selectedVariantId = selectedVariant.Id;

            itemPrice.Text = Item.PriceFromVariantsAndUOM(selectedVariantId, selectedUomId);

            if(!string.IsNullOrEmpty(itemPrice.Text))
                itemPrice.Visibility = ViewStates.Visible;
            else
                itemPrice.Visibility = ViewStates.Gone;

            SetWishListButton();
        }

        private bool ItemIsInWishList()
        {
            VariantRegistration selectedVar = null;
            UnitOfMeasure selectedUnitOfMeasure = null;

            if (selectedVariant != null)
            {
                selectedVar = selectedVariant;
            }

            return shoppingListModel.ItemIsInWishList(Item, selectedVar, selectedUnitOfMeasure);
        }

        private void ReverseWishListButton()
        {
           if (currentAddToWishListImage == Resource.Drawable.ic_favorite_outline_24dp)
            {
                wishListButton.SetImageResource(Resource.Drawable.ic_favorite_24dp);
            }
            else
            {
                wishListButton.SetImageResource(Resource.Drawable.ic_favorite_outline_24dp);
            }
        }

        private void SetWishListButton()
        {
            if (ItemIsInWishList())
            {
                currentAddToWishListImage = Resource.Drawable.ic_favorite_24dp;
                wishListButton.SetImageResource(Resource.Drawable.ic_favorite_24dp);
            }
            else
            {
                currentAddToWishListImage = Resource.Drawable.ic_favorite_outline_24dp;
                wishListButton.SetImageResource(Resource.Drawable.ic_favorite_outline_24dp);
            }
        }

        private void SelectVariant()
        {
            var varDialog = new VariantDialog(Activity, Item, selectedVariant, (newSelectedVariant, qty) =>
            {
                if (newSelectedVariant != null)
                {
                    selectedVariant = newSelectedVariant;
                    selectVariant.Text = selectedVariant.ToString();
                    CalculatePrice();
                    LoadImage();
                }
            });
            varDialog.Show();
        }

        public void OnClick(View v)
        {
            var qty = 1m;
            decimal.TryParse(shoppingListQty.Text, out qty);

            switch (v.Id)
            {
                case Resource.Id.ItemViewDecreaseQty:
                    var decreasedQty = (qty - 1);
                    if (decreasedQty > 0)
                        shoppingListQty.Text = decreasedQty.ToString();

                    break;
                case Resource.Id.ItemViewIncreaseQty:
                    shoppingListQty.Text = (qty + 1).ToString();

                    break;
                case Resource.Id.ItemViewChangeQty:
                    var changeQtyDialog = new ChangeQtyDialog(Activity, Item.Description, qty, newQty =>
                        {
                            if (newQty > 0)
                                shoppingListQty.Text = newQty.ToString();
                        });
                    changeQtyDialog.Show();

                    break;

                case Resource.Id.ItemViewAddToBasket:
                    if (addToBasketButton.State == ProgressButton.ProgressButtonState.Normal)
                    {
                        AddToBasket(qty);
                    }
                    else if (addToBasketButton.State == ProgressButton.ProgressButtonState.Done)
                    {
                        if (Activity is LoyaltyFragmentActivity)
                        {
                            (Activity as LoyaltyFragmentActivity).OpenDrawer((int)GravityFlags.End);
                        }

                        addToBasketButton.State = ProgressButton.ProgressButtonState.Normal;
                    }
                    break;

                case Resource.Id.ItemViewVariants:
                    SelectVariant();
                    break;

                case Resource.Id.ItemViewAddToWishlistFab:
                    AddToWishListClicked();
                    break;
            }
        }

        private async void AddToWishListClicked()
        {
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(LoginActivity));
                intent.PutExtra(BundleConstants.ErrorMessage, GetString(Resource.String.ApplicationMustBeLoggedIn));

                StartActivity(intent);

                return;
            }

            if (!ItemIsInWishList())
            {
                if (await AddToWishList())
                {
                    var interpolator = new OvershootInterpolator();

                    var scaleAnimatorX =
                        ObjectAnimator.OfFloat(wishListButton,
                            "scaleX", 0.5f, 1f);
                    var scaleAnimatorY =
                        ObjectAnimator.OfFloat(wishListButton,
                            "scaleY", 0.5f, 1f);

                    scaleAnimatorX.SetInterpolator(interpolator);
                    scaleAnimatorY.SetInterpolator(interpolator);

                    var animatorSetXY = new AnimatorSet();
                    animatorSetXY.PlayTogether(scaleAnimatorX, scaleAnimatorY);

                    animatorSetXY.Start();
                }

                ReverseWishListButton();
            }
            else
            {
                VariantRegistration selectedVar = null;
                UnitOfMeasure selectedUnitOfMeasure = null;

                if (selectedVariant != null)
                {
                    selectedVar = selectedVariant;
                }

                var existingItem = AppData.Device.UserLoggedOnToDevice.WishList.ItemGetByIds(Item.Id, selectedVar?.Id, selectedUnitOfMeasure?.Id);
                if (existingItem != null)
                    await shoppingListModel.DeleteWishListLine(existingItem.Id);
            }
        }

        private void LoadImage()
        {
            VariantRegistration variant = null;
            if (selectedVariant != null)
                variant = selectedVariant;

            var images = new List<LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView>();
            
            if (variant != null)
            {
                variant.Images.ForEach(images.Add);
            }

            Item.Images.ForEach(imageView =>
                {
                    if (images.FirstOrDefault(x => x.Id == imageView.Id) == null)
                    {
                        images.Add(imageView);
                    }
                });

            imagePager = new DetailImagePager(View, ChildFragmentManager, images);
        }

        #region MENU

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.ItemMenu, menu);

            if (Activity is LoyaltyFragmentActivity && (Activity as LoyaltyFragmentActivity).HasSocialMediaConnection)
            {
                inflater.Inflate(Resource.Menu.ShareMenu, menu);
            }

            if (!EnabledItems.HasStoreLocator)
            {
                menu.RemoveItem(Resource.Id.MenuViewStock);
            }
                
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewStock:
                    ViewAvailability();
                    return true;

                case Resource.Id.MenuViewShare:
                    if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        var bmpUri = GetLocalBitmapUri();
                        if (bmpUri != null)
                        {
                            // Construct a ShareIntent with link to image
                            Intent shareIntent = new Intent();
                            shareIntent.SetAction(Intent.ActionSend);
                            shareIntent.PutExtra(Intent.ExtraSubject, Item.Description);
                            shareIntent.PutExtra(Intent.ExtraText, Item.Details);
                            shareIntent.PutExtra(Intent.ExtraStream, bmpUri);
                            shareIntent.SetType("*/*");
                            // Launch sharing dialog for image
                            StartActivity(Intent.CreateChooser(shareIntent, GetString(Resource.String.MenuViewShare)));
                        }
                    }
                    else
                    {
                        if (askedForLocationPermission)
                        {
                            var dialog = new WarningDialog(Activity, "");
                            dialog.Message = GetString(Resource.String.ItemViewCannotAccessExternalStorage);
                            dialog.SetPositiveButton(GetString(Android.Resource.String.Ok), () => { });
                            dialog.SetNegativeButton(GetString(Resource.String.ApplicationOpenSettings), () =>
                            {
                                Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                                var uri = Android.Net.Uri.FromParts("package", Activity.PackageName, null);
                                intent.SetData(uri);
                                StartActivityForResult(intent, 1);
                            });
                            dialog.Show();
                        }
                        else
                        {
                            askedForLocationPermission = true;

                            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                            {
                                RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 0);
                            }
                        }
                    }
                    
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public Android.Net.Uri GetLocalBitmapUri()
        {
            var imageView = imagePager.GetFirstImage();

            if(imageView == null)
                return null;

            // Extract Bitmap from ImageView drawable
            Drawable drawable = imageView.Drawable;
            Bitmap bmp = null;
            if (drawable is BitmapDrawable){
               bmp = ((BitmapDrawable) imageView.Drawable).Bitmap;
            } else {
               return null;
            }
            // Store image to default external storage directory
            Android.Net.Uri bmpUri = null;

            try
            {
                File file =  new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads), Item.Description.Replace(" ", "") + ".png");
                file.ParentFile.Mkdirs();

                file.Delete();

                using (var os = new FileStream(file.Path, FileMode.CreateNew))
                {
                    bmp.Compress(Bitmap.CompressFormat.Png, 90, os);
                }

                bmpUri = Android.Net.Uri.FromFile(file);
            }
            catch (IOException)
            {
            }

            return bmpUri;
        }

        #endregion
    }
}