using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.StoreLocator;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using IItemClickListener = Presentation.Util.IItemClickListener;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation.Activities.Checkout
{
    public class CheckoutFragment : LoyaltyFragment, IRefreshableActivity, IBroadcastObserver, View.IOnClickListener, IItemClickListener, AdapterView.IOnItemSelectedListener
    {
        private BasketModel basketModel;
        private StoreModel storeModel;
        private ClickCollectModel clickCollectModel;

        private RecyclerView checkoutRecyclerView;
        private CheckoutAdapter adapter;

        private View total;

        private View storeHeader;
        private Spinner storeSpinner;
		private SimpleSpinnerAdapter clickCollectAdapter;
		private SimpleSpinnerAdapter shippingMethodAdapter;
        private TextView unavailableItems;
        private TextView totalTotal;
        private ColoredButton totalOrder;

        private List<Store> clickCollectStores;
        private List<OrderLineAvailability> orderLineAvailabilities;
        private Spinner shippingMethods;
        private ImageButton showStoresOnMap;
        private View paymentContainer;

        private ViewSwitcher storeSwitcher;
        private View storeProgress;
        private View storeContent;

        private ShippingMedhod currentShippingMedhod
        {
            get { return AppData.ShippingMethods[shippingMethods.SelectedItemPosition]; }
        }

        public static CheckoutFragment NewInstance()
        {
            var fragment = new CheckoutFragment() { Arguments = new Bundle() };
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstance)
        {
            if (container == null)
            {
                return null;
            }

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutScreen);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.CheckoutScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            basketModel = new BasketModel(Activity, this);
            storeModel = new StoreModel(Activity, this);
            clickCollectModel = new ClickCollectModel(Activity, this);

            adapter = new CheckoutAdapter(Activity);

            checkoutRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.CheckoutViewList);
            checkoutRecyclerView.SetLayoutManager(new StaggeredGridLayoutManager(Resources.GetInteger(Resource.Integer.StackedCardColumns), StaggeredGridLayoutManager.Vertical));
            checkoutRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            checkoutRecyclerView.HasFixedSize = true;

            checkoutRecyclerView.SetAdapter(adapter);

            #region TOTAL

            total = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutShippingMethodHeader, checkoutRecyclerView, false);
            //total = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutShippingMethodHeader);

            storeSwitcher = total.FindViewById<ViewSwitcher>(Resource.Id.CheckoutShippingMethodHeaderViewStoresViewSwitcher);
            storeContent = total.FindViewById(Resource.Id.CheckoutShippingMethodHeaderViewStoresContent);
            storeProgress = total.FindViewById(Resource.Id.CheckoutShippingMethodHeaderViewStoresProgress);
            paymentContainer = total.FindViewById(Resource.Id.CheckoutShippingMethodHeaderViewPayment);

            storeHeader = total.FindViewById(Resource.Id.CheckoutShippingMethodHeaderViewStoreHeader);
            storeSpinner = total.FindViewById<Spinner>(Resource.Id.CheckoutShippingMethodHeaderViewStores);

            unavailableItems = total.FindViewById<TextView>(Resource.Id.CheckoutShippingMethodHeaderViewUnavailableItems);

            shippingMethods = total.FindViewById<Spinner>(Resource.Id.CheckoutShippingMethodHeaderViewShippingMethod);
            var shippingMethodNames = new List<string>();
            foreach (var shippingMethod in AppData.ShippingMethods)
            {
                if (shippingMethod == ShippingMedhod.ClickCollect)
                {
                    shippingMethodNames.Add(GetString(Resource.String.CheckoutViewClickCollect));
                }
                else if (shippingMethod == ShippingMedhod.HomeDelivery)
                {
                    shippingMethodNames.Add(GetString(Resource.String.CheckoutViewHomeDelivery));
                }
            }
			shippingMethodAdapter = new SimpleSpinnerAdapter(Activity,
                                                                                  Resource.Layout.CustomSpinnerItem,
                                                                                  Resource.Layout.CustomSpinnerMultiLineItem,
                                                                                  shippingMethodNames);
            shippingMethods.Adapter = shippingMethodAdapter;
            shippingMethods.OnItemSelectedListener = this;

            totalTotal = total.FindViewById<TextView>(Resource.Id.CheckoutTotalHeaderViewTotal);
            totalOrder = total.FindViewById<ColoredButton>(Resource.Id.CheckoutTotalHeaderViewOrder);
            totalOrder.SetText(Resource.String.ApplicationNext);
            totalOrder.SetOnClickListener(this);

            showStoresOnMap = total.FindViewById<ImageButton>(Resource.Id.CheckoutShippingMethodHeaderViewStoresOnMap);
            showStoresOnMap.SetOnClickListener(this);
            showStoresOnMap.SetColorFilter(Util.Utils.ImageUtils.GetColorFilter(new Color(ContextCompat.GetColor(Activity, Resource.Color.accent))));

            #endregion

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
                (Activity as LoyaltyFragmentActivity).AddObserver(this);

            if (AppData.Stores == null || AppData.Stores.Count == 0)
            {
                LoadStores(true);
            }
            else
            {
                LoadStores(false);
            }

            AppData.Basket.CalculateBasket();

            NotifyCheckoutChanged();
        }

        public override void OnPause()
        {
            if (Activity is LoyaltyFragmentActivity)
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);

            base.OnPause();
        }

        private async void LoadStores(bool loadFromServer)
        {
            if (loadFromServer)
            {
                await storeModel.GetAllStores();
            }

            clickCollectStores = AppData.Stores.Where(x => x.IsClickAndCollect).ToList();

            clickCollectAdapter = new SimpleSpinnerAdapter(Activity, Resource.Layout.CustomSpinnerItem, Resource.Layout.CustomSpinnerMultiLineItem, clickCollectStores.Select(x => x.Description).ToList());
            storeSpinner.Adapter = clickCollectAdapter;
            storeSpinner.OnItemSelectedListener = this;

            if (clickCollectStores != null && clickCollectStores.Count > 0)
            {
                storeSpinner.SetSelection(0);
            }

            if (!loadFromServer)
            {
                ClickAndCollectStoreChanged(0);
            }
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if (clickCollectStores == null)
                {
                    if (storeSwitcher.CurrentView != storeProgress)
                    {
                        storeSwitcher.ShowNext();
                    }
                }
                else
                {
                    adapter.IsLoading = show;
                }
            }
            else
            {
                if (storeSwitcher.CurrentView != storeContent)
                {
                    storeSwitcher.ShowPrevious();
                }
                adapter.IsLoading = show;
            }
        }

        private void UpdatePayment(List<OneListItem> basketItems = null)
        {
            if (basketItems == null)
            {
                basketItems = CreateBasketItems();
            }

            Activity.RunOnUiThread(() =>
            {
                totalTotal.Text = GetString(Resource.String.CheckoutViewTotal) + " ~" + AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(OneList.CalculateBasket(basketItems));
            });
        }

        public void BroadcastReceived(string action)
        {
            if (action == Utils.BroadcastUtils.BasketStateUpdated)
            {
                NotifyCheckoutChanged();
            }
        }

        public void NotifyCheckoutChanged()
        {
            if (AppData.Basket.Items.Count > 0)
            {
                var basketItems = CreateBasketItems();

                //(Utils.ListUtils.GetBaseAdapter(headers.Adapter) as CheckoutAdapter).CreateItems(basketItems);
                adapter.SetItems(Activity, basketItems, total);

                UpdatePayment(basketItems);
            }
            else
            {
                Activity.Finish();
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.CheckoutShippingMethodHeaderViewStoresOnMap:
                    if (Util.Utils.CheckForPlayServices(Activity))
                    {
                        var stores = clickCollectStores;

                        string[] storeIds = new string[stores.Count];

                        for (int i = 0; i < stores.Count; i++)
                        {
                            storeIds[i] = stores[i].Id;
                        }

                        var intent = new Intent();
                        intent.SetClass(Activity, typeof(StoreLocatorMapActivity));
                        
                        intent.PutExtra(BundleConstants.StoreIds, storeIds);
                        intent.PutExtra(BundleConstants.ClickReturnId, true);

                        StartActivityForResult(intent, 1);
                    }
                    break;

                case Resource.Id.CheckoutTotalHeaderViewOrder:
                    if (ValidateData())
                    {
                        if (AppData.ShippingMethods[shippingMethods.SelectedItemPosition] == ShippingMedhod.ClickCollect)
                        {
                            var intent = new Intent();
                            intent.SetClass(Activity, typeof(CheckoutTotalActivity));

                            intent.PutExtra(BundleConstants.DeliveryType, (int)ShippingMedhod.ClickCollect);
                            intent.PutExtra(BundleConstants.StoreId, clickCollectStores[storeSpinner.SelectedItemPosition].Id);

                            var serializer = new XmlSerializer(typeof(List<OrderLineAvailability>), new Type[] { });

                            using (var textWriter = new StringWriter())
                            {
                                serializer.Serialize(textWriter, orderLineAvailabilities);
                                intent.PutExtra(BundleConstants.OrderLineAvailabilities, textWriter.ToString());
                            }

                            StartActivityForResult(intent, 0);
                        }
                        else if (AppData.ShippingMethods[shippingMethods.SelectedItemPosition] == ShippingMedhod.HomeDelivery)
                        {
                            var intent = new Intent();
                            intent.SetClass(Activity, typeof(CheckoutShippingActivity));
                            StartActivityForResult(intent, 0);
                        }
                    }
                    break;
            }
        }

        private bool ValidateData()
        {
            foreach (var basketItem in AppData.Basket.Items)
            {
                if (basketItem.Quantity == 0)
                {
                    Toast.MakeText(Activity, string.Format(GetString(Resource.String.CheckoutViewCantHaveZeroQty), basketItem.ItemDescription), ToastLength.Short).Show();
                    return false;
                }
            }

            return true;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == 0 && resultCode == (int)Result.Ok)
            {
                Activity.SetResult(Result.Ok);
                Activity.Finish();
            }
            else if (requestCode == 1 && resultCode == (int) Result.Ok)
            {
                var storeId = data.Extras.GetString(BundleConstants.StoreId);

                var index = clickCollectStores.FindIndex(x => x.Id == storeId);

                storeSpinner.SetSelection(index);

                //listener is not active yet
                OnItemSelected(storeSpinner, View, index, index);
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            switch (parent.Id)
            {
                case Resource.Id.CheckoutShippingMethodHeaderViewShippingMethod:
                    ShippingMethodChanged(position);
                    break;

				case Resource.Id.CheckoutShippingMethodHeaderViewStores:
				    ClickAndCollectStoreChanged(position);
                    break;
            }
        }

        private void ShippingMethodChanged(int position)
        {
            var shippingMethod = AppData.ShippingMethods[position];
            shippingMethodAdapter.setSelectedPosition(position);
            if (shippingMethod == ShippingMedhod.ClickCollect)
            {
                storeSwitcher.Visibility = ViewStates.Visible;

                this.paymentContainer.Visibility = ViewStates.Gone;
                this.totalOrder.Visibility = ViewStates.Gone;

                adapter.SetItems(Activity, new List<OneListItem>(), total);
                //(Util.Utils.ListUtils.GetBaseAdapter(headers.Adapter) as CheckoutAdapter).Clear();

                if (clickCollectStores != null && clickCollectStores.Count > 0)
                    OnItemSelected(storeSpinner, storeSpinner.SelectedView, storeSpinner.SelectedItemPosition, storeSpinner.SelectedItemId);
            }
            else if (shippingMethod == ShippingMedhod.HomeDelivery)
            {
                storeSwitcher.Visibility = ViewStates.Gone;
                unavailableItems.Visibility = ViewStates.Gone;

                adapter.SetItems(Activity, AppData.Basket.Items, total);
                //(Util.Utils.ListUtils.GetBaseAdapter(headers.Adapter) as CheckoutAdapter).CreateItems();

                this.paymentContainer.Visibility = ViewStates.Visible;
                this.totalOrder.Visibility = ViewStates.Visible;

                UpdatePayment();
            }
        }

        private void ClickAndCollectStoreChanged(int position)
        {
            adapter.SetItems(Activity, new List<OneListItem>(), total);
            //(Util.Utils.ListUtils.GetBaseAdapter(headers.Adapter) as CheckoutAdapter).Clear();
            this.clickCollectAdapter.setSelectedPosition(position);
            var store = clickCollectStores[position];

            ClickAndCollectOrderAvailabilityCheck(store.Id);
        }

        private async void ClickAndCollectOrderAvailabilityCheck(string storeId)
        {
            var orderAvailability = await clickCollectModel.OrderAvailabilityCheck(storeId);

            if (orderAvailability != null)
            {
                orderLineAvailabilities = orderAvailability;

                var basketItems = CreateBasketItems();

                adapter.SetItems(Activity, basketItems, total);

                UpdatePayment(basketItems);
            }
        }

        public void OnNothingSelected(AdapterView parent)
        {
        }

        public List<OneListItem> CreateBasketItems()
        {
            if (currentShippingMedhod == ShippingMedhod.ClickCollect)
            {
                var basketItems = new List<OneListItem>();
                var unavailableItemsId = new List<string>();

                if (orderLineAvailabilities == null)
                    return basketItems;

                foreach (var orderLineAvailability in orderLineAvailabilities)
                {
                    OneListItem basketItem = AppData.Basket.ItemGetByIds(orderLineAvailability.ItemId, orderLineAvailability.VariantId, orderLineAvailability.UomId);
                    if (basketItem == null)
                        continue;

                    if (orderLineAvailability.Quantity > 0)
                    {
                        OneListItem availableBasketItem = new OneListItem()
                        {
                            Id = basketItem.Id,
                            ItemId = basketItem.ItemId,
                            ItemDescription = basketItem.ItemDescription,
                            UnitOfMeasureId = basketItem.UnitOfMeasureId,
                            VariantId = basketItem.VariantId,
                            VariantDescription = basketItem.VariantDescription,
                            Image = basketItem.Image,
                            Quantity = basketItem.Quantity,
                            NetPrice = basketItem.NetPrice,
                            Price = basketItem.Price,
                            NetAmount = basketItem.NetAmount,
                            Amount = basketItem.Amount,
                            TaxAmount = basketItem.TaxAmount,
                            DiscountAmount = basketItem.DiscountAmount
                        };

                        if (basketItem.Quantity > orderLineAvailability.Quantity)
                        {
                            unavailableItemsId.Add((basketItem.Quantity - orderLineAvailability.Quantity).ToString("G29") + " " +
                                                 basketItem.ItemDescription);
                            availableBasketItem.Quantity = orderLineAvailability.Quantity;
                        }

                        basketItems.Add(availableBasketItem);
                    }
                    else
                    {
                        unavailableItemsId.Add(basketItem.ItemDescription);
                    }
                }

                if (basketItems.Count == 0)
                {
                    this.unavailableItems.Visibility = ViewStates.Visible;
                    this.paymentContainer.Visibility = ViewStates.Gone;
                    this.totalOrder.Visibility = ViewStates.Gone;

                    this.unavailableItems.Text = string.Format(GetString(Resource.String.CheckoutViewNoItemsAvailable), clickCollectStores[storeSpinner.SelectedItemPosition].Description);
                }
                else if (unavailableItemsId.Count == 0)
                {
                    this.unavailableItems.Visibility = ViewStates.Gone;
                    this.paymentContainer.Visibility = ViewStates.Visible;
                    this.totalOrder.Visibility = ViewStates.Visible;
                }
                else if (unavailableItemsId.Count == 1)
                {
                    this.unavailableItems.Visibility = ViewStates.Visible;
                    this.paymentContainer.Visibility = ViewStates.Visible;
                    this.totalOrder.Visibility = ViewStates.Visible;

                    this.unavailableItems.Text = string.Format(GetString(Resource.String.CheckoutViewItemNotAvailable), unavailableItemsId[0], clickCollectStores[storeSpinner.SelectedItemPosition].Description);
                }
                else
                {
                    this.unavailableItems.Visibility = ViewStates.Visible;
                    this.paymentContainer.Visibility = ViewStates.Visible;
                    this.totalOrder.Visibility = ViewStates.Visible;
                    this.unavailableItems.Text = string.Format(GetString(Resource.String.CheckoutViewItemsNotAvailable), clickCollectStores[storeSpinner.SelectedItemPosition].Description);

                    foreach (var unavailableItem in unavailableItemsId)
                    {
                        this.unavailableItems.Text += System.Environment.NewLine + "\t" + unavailableItem;
                    }
                }

                return basketItems;
            }

            return AppData.Basket.Items;
        } 
    }
}