using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using Utils = Presentation.Util.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;

namespace Presentation.Activities.Checkout
{
    public class CheckoutTotalFragment : LoyaltyFragment, IRefreshableActivity, View.IOnClickListener, IBroadcastObserver
    {
        private BasketModel basketModel;
        private OneListModel oneListModel;
        private MemberContactModel memberContactModel;
        private ClickCollectModel clickCollectModel;

        private RecyclerView checkoutRecyclerView;
        private CheckoutAdapter adapter;

        private LinearLayout footerView;
        private View total;

        private View totalheader;
        //private View totalDivider;
        private ViewSwitcher totalViewSwitcher;
        private View totalProgressBar;
        private View totalContainer;
        private TextView totalSubtotal;
        private TextView totalShipping;
        private TextView totalVAT;
        private TextView totalDiscount;
        private TextView totalTotal;
        private ColoredButton totalOrder;
        private EditText email;

        private ShippingMedhod shippingMedhod;
        private PaymentType paymentType;
        private Address shippingAddress;
        private Address billingAddress;

        private string cardNumber;
        private string cardMM;
        private string cardYYYY;
        private string cardCVV;

        private bool needsCalculate;

        private OneList basket;
        private string storeId;
        private bool localBasket;

        public static CheckoutTotalFragment NewInstance()
        {
            var fragment = new CheckoutTotalFragment() { Arguments = new Bundle() };
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutScreen);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.CheckoutScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            basketModel = new BasketModel(Activity, this);
            oneListModel = new OneListModel(Activity, this);
            clickCollectModel = new ClickCollectModel(Activity, this);
            memberContactModel = new MemberContactModel(Activity);

            //todo
            adapter = new CheckoutAdapter(Activity);

            checkoutRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.CheckoutViewList);
            checkoutRecyclerView.SetLayoutManager(new StaggeredGridLayoutManager(Resources.GetInteger(Resource.Integer.StackedCardColumns), StaggeredGridLayoutManager.Vertical));
            checkoutRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));
            checkoutRecyclerView.HasFixedSize = true;

            checkoutRecyclerView.SetAdapter(adapter);

            footerView = Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutTotalFooterContainer) as LinearLayout;

            #region TOTAL

            total = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutTotalCard);

            totalSubtotal = total.FindViewById<TextView>(Resource.Id.CheckoutViewSubtotal);
            totalShipping = total.FindViewById<TextView>(Resource.Id.CheckoutViewShipping);
            totalVAT = total.FindViewById<TextView>(Resource.Id.CheckoutViewVAT);
            totalDiscount = total.FindViewById<TextView>(Resource.Id.CheckoutViewDiscount);
            totalTotal = total.FindViewById<TextView>(Resource.Id.CheckoutViewTotal);
            totalOrder = total.FindViewById<ColoredButton>(Resource.Id.CheckoutViewOrder);

            totalheader = total.FindViewById<View>(Resource.Id.CheckoutViewTotalHeader);
            //totalDivider = total.FindViewById<View>(Resource.Id.CheckoutViewTotalDivider);
            totalViewSwitcher = total.FindViewById<ViewSwitcher>(Resource.Id.CheckoutViewViewSwitcher);
            totalProgressBar = total.FindViewById<View>(Resource.Id.CheckoutViewTotalProgress);
            totalContainer = total.FindViewById<View>(Resource.Id.CheckoutViewTotalContainer);

            totalOrder.SetText(Resource.String.CheckoutViewOrder);
            totalOrder.SetOnClickListener(this);

            #endregion

            #region INFORMATION

            shippingMedhod = (ShippingMedhod)Arguments.GetInt(BundleConstants.DeliveryType);

            if (shippingMedhod == ShippingMedhod.ClickCollect)
            {
                storeId = Arguments.GetString(BundleConstants.StoreId);
                XmlSerializer serializer = new XmlSerializer(typeof(List<OrderLineAvailability>), new Type[] { });

                using (TextReader textReader = new StringReader(Arguments.GetString(BundleConstants.OrderLineAvailabilities)))
                {
                    //Console.WriteLine(textReader.ToString());

                    basket = new OneList("", clickCollectModel.CreateBasketItems((List<OrderLineAvailability>)serializer.Deserialize(textReader)), true);
                }

                localBasket = true;
            }
            else if (shippingMedhod == ShippingMedhod.HomeDelivery)
            {
                basket = AppData.Basket;

                shippingAddress = new Address();
                shippingAddress.Address1 = Arguments.GetString(BundleConstants.ShippingAddressOne);
                shippingAddress.Address2 = Arguments.GetString(BundleConstants.ShippingAddressTwo);
                shippingAddress.City = Arguments.GetString(BundleConstants.ShippingAddressCity);
                shippingAddress.StateProvinceRegion = Arguments.GetString(BundleConstants.ShippingAddressState);
                shippingAddress.PostCode = Arguments.GetString(BundleConstants.ShippingAddressPostCode);
                shippingAddress.Country = Arguments.GetString(BundleConstants.ShippingAddressCountry);

                var information = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutInformationCard);
                //var header = information.FindViewById<TextView>(Resource.Id.CheckoutViewInformationHeader);
                var heading = information.FindViewById<TextView>(Resource.Id.CheckoutViewInformationHeading);
                var description = information.FindViewById<TextView>(Resource.Id.CheckoutViewInformationDescription);

                //header.Text = GetString(Resource.String.CheckoutViewShippingType);
                heading.Text = GetString(Resource.String.CheckoutViewHomeDelivery);
                description.Text = Arguments.GetString(BundleConstants.ShippingName) + System.Environment.NewLine + shippingAddress.FormatAddress;

                footerView.AddView(information);
                //headers.AddFooterView(information, null, false);

                paymentType = (PaymentType)Arguments.GetInt(BundleConstants.PaymentType);

                if (paymentType == PaymentType.CreditCard)
                {
                    var informationPayment = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutInformationCard);

                    cardNumber = Arguments.GetString(BundleConstants.CardNumber);
                    cardMM = Arguments.GetString(BundleConstants.CardMM);
                    cardYYYY = Arguments.GetString(BundleConstants.CardYYYY);
                    cardCVV = Arguments.GetString(BundleConstants.CardCVV);

                    //informationPayment.FindViewById<TextView>(Resource.Id.CheckoutViewInformationHeader).Text = GetString(Resource.String.CheckoutViewPayment);
                    informationPayment.FindViewById<TextView>(Resource.Id.CheckoutViewInformationHeading).Text = GetString(Resource.String.CheckoutViewPayCreditCard);
                    informationPayment.FindViewById<TextView>(Resource.Id.CheckoutViewInformationDescription).Text = cardNumber + System.Environment.NewLine + string.Format(GetString(Resource.String.CheckoutViewExpires), cardMM, cardYYYY);

                    footerView.AddView(informationPayment);
                    //headers.AddFooterView(informationPayment, null, false);

                    var informationBilling = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutInformationCard);

                    billingAddress = new Address();
                    billingAddress.Address1 = Arguments.GetString(BundleConstants.BillingAddressOne);
                    billingAddress.Address2 = Arguments.GetString(BundleConstants.BillingAddressTwo);
                    billingAddress.City = Arguments.GetString(BundleConstants.BillingAddressCity);
                    billingAddress.StateProvinceRegion = Arguments.GetString(BundleConstants.BillingAddressState);
                    billingAddress.PostCode = Arguments.GetString(BundleConstants.BillingAddressPostCode);
                    billingAddress.Country = Arguments.GetString(BundleConstants.BillingAddressCountry);

                    informationBilling.FindViewById<TextView>(Resource.Id.CheckoutViewInformationHeading).Text = GetString(Resource.String.CheckoutViewHomeDelivery);
                    informationBilling.FindViewById<TextView>(Resource.Id.CheckoutViewInformationDescription).Text = Arguments.GetString(BundleConstants.ShippingName) + System.Environment.NewLine + billingAddress.FormatAddress;

                    footerView.AddView(informationBilling);
                    //headers.AddFooterView(informationBilling, null, false);
                }
            }

            #endregion

            #region EMAIL

            if (shippingMedhod == ShippingMedhod.ClickCollect)
            {
                var emailCard = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutEmailCard);

                email = emailCard.FindViewById<EditText>(Resource.Id.CheckoutViewEmail);
                email.Text = AppData.Device.UserLoggedOnToDevice.Email;

                footerView.AddView(emailCard);
                //headers.AddFooterView(emailCard, null, false);
            }

            #endregion

            footerView.AddView(total);
            //headers.AddFooterView(total, null, false);

            adapter.SetItems(Activity, basket.Items, null, footerView);

            CalculateBasket();

            return view;
        }

        private async void CalculateBasket()
        {
            Order basketCalcResponse = await oneListModel.OneListCalculate(basket);
            if (basketCalcResponse == null)
            {
                needsCalculate = true;

                totalheader.Visibility = ViewStates.Gone;
                totalViewSwitcher.Visibility = ViewStates.Gone;

                totalOrder.SetText(Resource.String.CheckoutViewCalculateTotal);
            }
            else
            {
                basket = new OneList(AppData.Basket.Id);
                basket.TotalNetAmount = basketCalcResponse.TotalNetAmount;
                basket.TotalAmount = basketCalcResponse.TotalAmount;
                basket.TotalTaxAmount = basket.TotalAmount - basket.TotalNetAmount;
                basket.TotalDiscAmount = basketCalcResponse.TotalDiscount;

                foreach (OrderLine basketLineCalcResponse in basketCalcResponse.OrderLines)
                {
                    basketLineCalcResponse.UomId = string.Empty;

                    OneListItem item = AppData.Basket.ItemGetByIds(basketLineCalcResponse.ItemId, basketLineCalcResponse.VariantId, basketLineCalcResponse.UomId);
                    if (item == null)
                        continue;

                    OneListItem basketItem = new OneListItem()
                    {
                        Item = item.Item,
                        DisplayOrderId = basketLineCalcResponse.LineNumber,
                        Quantity = basketLineCalcResponse.Quantity,
                        UnitOfMeasure = item.UnitOfMeasure,
                        VariantReg = item.VariantReg,
                        NetAmount = basketLineCalcResponse.NetAmount,
                        TaxAmount = basketLineCalcResponse.TaxAmount,
                        Amount = basketLineCalcResponse.NetAmount + basketLineCalcResponse.TaxAmount,
                        Price = basketLineCalcResponse.Price,
                        NetPrice = basketLineCalcResponse.NetPrice,
                        DiscountAmount = basketLineCalcResponse.DiscountAmount,
                        DiscountPercent = basketLineCalcResponse.DiscountPercent,
                    };
                    basket.Items.Add(basketItem);
                }

                foreach (OrderDiscountLine basketDiscLine in basketCalcResponse.OrderDiscountLines)
                {
                    OneListItem item = basket.Items.Find(i => i.DisplayOrderId == basketDiscLine.LineNumber / 10000);
                    if (item == null)
                        continue;

                    item.OnelistItemDiscounts.Add(new OneListItemDiscount()
                    {
                        DiscountType = basketDiscLine.DiscountType,
                        No = basketDiscLine.No,
                        OfferNumber = basketDiscLine.OfferNumber,
                        Description = basketDiscLine.Description,
                        DiscountAmount = basketDiscLine.DiscountAmount,
                        DiscountPercent = basketDiscLine.DiscountPercent,
                        PeriodicDiscGroup = basketDiscLine.PeriodicDiscGroup,
                        PeriodicDiscType = basketDiscLine.PeriodicDiscType
                    });
                }

                needsCalculate = false;

                totalheader.Visibility = ViewStates.Visible;
                totalViewSwitcher.Visibility = ViewStates.Visible;

                totalOrder.SetText(Resource.String.CheckoutViewOrder);

                totalSubtotal.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basket.TotalNetAmount);
                totalShipping.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basket.ShippingAmount);
                totalVAT.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basket.TotalTaxAmount);
                totalDiscount.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basket.TotalDiscAmount);
                totalTotal.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basket.TotalAmount);

                if (!localBasket)
                {
                    AppData.Basket = basket;
                }
            }
        }


        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
                (Activity as LoyaltyFragmentActivity).AddObserver(this);

            adapter.SetItems(Activity, basket.Items, null, footerView);
        }

        public override void OnPause()
        {
            if (Activity is LoyaltyFragmentActivity)
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);

            base.OnPause();
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if (totalViewSwitcher.CurrentView != totalProgressBar)
                {
                    totalViewSwitcher.ShowPrevious();
                }

                totalOrder.Visibility = ViewStates.Gone;
            }
            else
            {
                if (totalViewSwitcher.CurrentView != totalContainer)
                {
                    totalViewSwitcher.ShowNext();
                }

                totalOrder.Visibility = ViewStates.Visible;
            }
        }

        private async Task<bool> HomeDelivery()
        {
            var success = await basketModel.SendOrder(basket, AppData.Device, billingAddress, shippingAddress, paymentType, AppData.Device.UserLoggedOnToDevice.Environment.Currency.Id, AppData.Device.CardId, cardCVV, AppData.Device.UserLoggedOnToDevice.Name);

            if (success)
            {
                await memberContactModel.MemberContactGetPointBalance(AppData.Device.CardId);

                var upIntent = new Intent();
                upIntent.SetClass(Activity, typeof(HomeActivity));
                upIntent.AddFlags(ActivityFlags.ClearTop);
                upIntent.AddFlags(ActivityFlags.SingleTop);

                upIntent.PutExtra(BundleConstants.ChosenMenuBundleName, LoyaltyFragmentActivity.ActivityTypes.DefaultItem);

                StartActivity(upIntent);
                Activity.Finish();
                return true;
            }
            return false;
        }

        private async Task<bool> ClickAndCollect()
        {
            var success = await clickCollectModel.ClickCollectOrderCreate(basket, AppData.Device.UserLoggedOnToDevice.Id, AppData.Device.CardId, storeId, email.Text);

            if (success)
            {
                await memberContactModel.MemberContactGetPointBalance(AppData.Device.CardId);

                var upIntent = new Intent();
                upIntent.SetClass(Activity, typeof(HomeActivity));
                upIntent.AddFlags(ActivityFlags.ClearTop);
                upIntent.AddFlags(ActivityFlags.SingleTop);

                upIntent.PutExtra(BundleConstants.ChosenMenuBundleName, LoyaltyFragmentActivity.ActivityTypes.DefaultItem);

                StartActivity(upIntent);
                Activity.Finish();
                return true;
            }
            return false;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.CheckoutViewOrder:
                    if (needsCalculate)
                    {
                        CalculateBasket();
                    }
                    else
                    {
                        if (shippingMedhod == ShippingMedhod.ClickCollect)
                        {
                            ClickAndCollect();
                        }
                        else if (shippingMedhod == ShippingMedhod.HomeDelivery)
                        {
                            HomeDelivery();
                        }
                    }

                    break;
            }
        }

        public void BroadcastReceived(string action)
        {
            if (action == Utils.BroadcastUtils.BasketStateUpdated)
            {
                adapter.SetItems(Activity, basket.Items, null, footerView);
            }
        }
    }
}