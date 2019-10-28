using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;

namespace Presentation.Activities.Checkout
{
    public class CheckoutShippingFragment : LoyaltyFragment, CompoundButton.IOnCheckedChangeListener, AdapterView.IOnItemSelectedListener, RadioGroup.IOnCheckedChangeListener, View.IOnClickListener, TextView.IOnEditorActionListener
    {
        private View shippingAddressContainer;
        private View shippingAddressHeader;
        //private View shippingAddressDivider;
        private EditText shippingAddressName;
        private EditText shippingAddressOne;
        private EditText shippingAddressTwo;
        private EditText shippingCity;
        private EditText shippingState;
        private EditText shippingPostCode;
        private EditText shippingCountry;

        private View paymentContainer;
        private View paymentHeader;
        //private View PaymentDivider;

        private EditText paymentCardNumber;
        private EditText paymentMM;
        private EditText paymentYYYY;
        private EditText paymentCVV;

        private View billingAddressContainer;
        private View billingAddressHeader;
        //private View billingAddressDivider;
        private View billingShipmentInformation;
        private CheckBox useShippingAddress;
        private EditText billingShippingAddressName;
        private EditText billingShippingAddressOne;
        private EditText billingShippingAddressTwo;
        private EditText billingShippingCity;
        private EditText billingShippingState;
        private EditText billingShippingPostCode;
        private EditText billingShippingCountry;

        public static CheckoutShippingFragment NewInstance()
        {
            var fragment = new CheckoutShippingFragment() { Arguments = new Bundle() };
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.CheckoutShippingScreen);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.CheckoutShippingScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            var addresses = new List<String>();
            addresses.Add(AppData.Device.UserLoggedOnToDevice.Name);
            addresses.Add("New");

            #region PAYMENT

            paymentContainer = view.FindViewById<View>(Resource.Id.CheckoutViewPaymentContainer);
            paymentHeader = view.FindViewById<View>(Resource.Id.CheckoutViewPaymentHeader);
            //PaymentDivider = view.FindViewById<View>(Resource.Id.CheckoutViewPaymentDivider);

            paymentCardNumber = view.FindViewById<EditText>(Resource.Id.CheckoutViewPaymentCardNumber);
            paymentMM = view.FindViewById<EditText>(Resource.Id.CheckoutViewPaymentCardMM);
            paymentYYYY = view.FindViewById<EditText>(Resource.Id.CheckoutViewPaymentCardYYYY);
            paymentCVV = view.FindViewById<EditText>(Resource.Id.CheckoutViewPaymentCardCVV);

            billingShipmentInformation = view.FindViewById<View>(Resource.Id.CheckoutViewBillingInformation);

            useShippingAddress = view.FindViewById<CheckBox>(Resource.Id.CheckoutViewUseShippingAddress);
            useShippingAddress.SetOnCheckedChangeListener(this);

            useShippingAddress.Checked = false;

            billingAddressContainer = view.FindViewById<View>(Resource.Id.CheckoutViewBillingContainer);
            billingAddressHeader = view.FindViewById<View>(Resource.Id.CheckoutViewBillingHeader);
            //billingAddressDivider = view.FindViewById<View>(Resource.Id.CheckoutViewBillingDivider);
            billingShippingAddressName = view.FindViewById<EditText>(Resource.Id.CheckoutViewBillingName);
            billingShippingAddressOne = view.FindViewById<EditText>(Resource.Id.CheckoutViewBillingAddressOne);
            billingShippingAddressTwo = view.FindViewById<EditText>(Resource.Id.CheckoutViewBillingAddressTwo);
            billingShippingCity = view.FindViewById<EditText>(Resource.Id.CheckoutViewBillingCity);
            billingShippingState = view.FindViewById<EditText>(Resource.Id.CheckoutViewBillingState);
            billingShippingPostCode = view.FindViewById<EditText>(Resource.Id.CheckoutViewBillingPostCode);
            billingShippingCountry = view.FindViewById<EditText>(Resource.Id.CheckoutViewBillingCountry);

            var paymentMethod = view.FindViewById<RadioGroup>(Resource.Id.CheckoutViewPaymentMethod);
            paymentMethod.SetOnCheckedChangeListener(this);

            var billingAddressSpinner = view.FindViewById<Spinner>(Resource.Id.CheckoutViewSavedBillingAddresses);

            var billingAddressAdapter = new SimpleSpinnerAdapter(Activity, Resource.Layout.CustomSpinnerItem, Resource.Layout.CustomSpinnerMultiLineItem, addresses);
            billingAddressSpinner.Adapter = billingAddressAdapter;
            billingAddressSpinner.SetSelection(0);
            billingAddressSpinner.OnItemSelectedListener = this;

            paymentMethod.Check(Resource.Id.CheckoutViewPayOnDelivery);

            #endregion

            #region SHIPPING

            shippingAddressContainer = view.FindViewById<View>(Resource.Id.CheckoutViewShippingContainer);
            shippingAddressHeader = view.FindViewById<View>(Resource.Id.CheckoutViewShippingHeader);
            //shippingAddressDivider = view.FindViewById<View>(Resource.Id.CheckoutViewShippingDivider);
            shippingAddressName = view.FindViewById<EditText>(Resource.Id.CheckoutViewShippingName);
            shippingAddressOne = view.FindViewById<EditText>(Resource.Id.CheckoutViewShippingAddressOne);
            shippingAddressTwo = view.FindViewById<EditText>(Resource.Id.CheckoutViewShippingAddressTwo);
            shippingCity = view.FindViewById<EditText>(Resource.Id.CheckoutViewShippingCity);
            shippingState = view.FindViewById<EditText>(Resource.Id.CheckoutViewShippingState);
            shippingPostCode = view.FindViewById<EditText>(Resource.Id.CheckoutViewShippingPostCode);
            shippingCountry = view.FindViewById<EditText>(Resource.Id.CheckoutViewShippingCountry);

            //var shippingMethod = view.FindViewById<RadioGroup>(Resource.Id.CheckoutViewShippingMethod);
            //shippingMethod.SetOnCheckedChangeListener(this);

            //shippingMethod.Check(Resource.Id.CheckoutViewInStorePickup);

            var addressSpinner = view.FindViewById<Spinner>(Resource.Id.CheckoutViewSavedAddresses);

            var addressAdapter = new SimpleSpinnerAdapter(Activity, Resource.Layout.CustomSpinnerItem, Resource.Layout.CustomSpinnerMultiLineItem, addresses);
            addressSpinner.Adapter = addressAdapter;
            addressSpinner.SetSelection(0);
            addressSpinner.OnItemSelectedListener = this;

            #endregion

            var nextButton = view.FindViewById<ColoredButton>(Resource.Id.CheckoutShippingViewNext);
            nextButton.SetOnClickListener(this);

            billingShippingCountry.SetOnEditorActionListener(this);

            return view;
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            switch (parent.Id)
            {
                case Resource.Id.CheckoutViewSavedAddresses:
                    if (position == 0)
                    {
                        var contact = AppData.Device.UserLoggedOnToDevice;

                        shippingAddressName.Text = contact.Name;

                        if (contact.Addresses != null && contact.Addresses.Count > 0)
                        {
                            shippingAddressOne.Text = contact.Addresses[0].Address1;
                            shippingAddressTwo.Text = contact.Addresses[0].Address2;
                            shippingCity.Text = contact.Addresses[0].City;
                            shippingState.Text = contact.Addresses[0].StateProvinceRegion;
                            shippingPostCode.Text = contact.Addresses[0].PostCode;
                            shippingCountry.Text = contact.Addresses[0].Country;
                        }
                    }
                    else
                    {
                        shippingAddressName.Text = string.Empty;
                        shippingAddressOne.Text = string.Empty;
                        shippingAddressTwo.Text = string.Empty;
                        shippingCity.Text = string.Empty;
                        shippingState.Text = string.Empty;
                        shippingPostCode.Text = string.Empty;
                        shippingCountry.Text = string.Empty;
                    }

                    break;

                case Resource.Id.CheckoutViewSavedBillingAddresses:
                    if (position == 0)
                    {
                        var contact = AppData.Device.UserLoggedOnToDevice;

                        billingShippingAddressName.Text = contact.Name;

                        if (contact.Addresses != null && contact.Addresses.Count > 0)
                        {
                            billingShippingAddressOne.Text = contact.Addresses[0].Address1;
                            billingShippingAddressTwo.Text = contact.Addresses[0].Address2;
                            billingShippingCity.Text = contact.Addresses[0].City;
                            billingShippingState.Text = contact.Addresses[0].StateProvinceRegion;
                            billingShippingPostCode.Text = contact.Addresses[0].PostCode;
                            billingShippingCountry.Text = contact.Addresses[0].Country;
                        }
                    }
                    else
                    {
                        billingShippingAddressName.Text = string.Empty;
                        billingShippingAddressOne.Text = string.Empty;
                        billingShippingAddressTwo.Text = string.Empty;
                        billingShippingCity.Text = string.Empty;
                        billingShippingState.Text = string.Empty;
                        billingShippingPostCode.Text = string.Empty;
                        billingShippingCountry.Text = string.Empty;
                    }

                    break;
            }
        }

        public void OnNothingSelected(AdapterView parent)
        {

        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            switch (group.Id)
            {
                case Resource.Id.CheckoutViewPaymentMethod:
                    switch (checkedId)
                    {
                        case Resource.Id.CheckoutViewPayOnDelivery:
                            paymentContainer.Visibility = ViewStates.Gone;
                            paymentHeader.Visibility = ViewStates.Gone;
                            //PaymentDivider.Visibility = ViewStates.Gone;

                            billingAddressContainer.Visibility = ViewStates.Gone;
                            billingAddressHeader.Visibility = ViewStates.Gone;
                            //billingAddressDivider.Visibility = ViewStates.Gone;

                            break;

                        case Resource.Id.CheckoutViewPayCreditCard:
                            paymentContainer.Visibility = ViewStates.Visible;
                            paymentHeader.Visibility = ViewStates.Visible;
                            //PaymentDivider.Visibility = ViewStates.Visible;

                            billingAddressContainer.Visibility = ViewStates.Visible;
                            billingAddressHeader.Visibility = ViewStates.Visible;
                            //billingAddressDivider.Visibility = ViewStates.Visible;

                            break;
                    }

                    break;
            }

        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            switch (buttonView.Id)
            {
                case Resource.Id.CheckoutViewUseShippingAddress:
                    if (isChecked)
                    {
                        billingShipmentInformation.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        billingShipmentInformation.Visibility = ViewStates.Visible;
                    }

                    break;
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.CheckoutShippingViewNext:
                    Next();
                    
                    break;
            }
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Done)
            {
                Next();
            }

            return false;
        }

        private bool ValidateData()
        {
            if (View != null)
            {
                //if (View.FindViewById<RadioButton>(Resource.Id.CheckoutViewHomeDelivery).Checked)
                {
                    if (string.IsNullOrEmpty(shippingAddressName.Text) || string.IsNullOrEmpty(shippingAddressOne.Text) || string.IsNullOrEmpty(shippingCity.Text) || string.IsNullOrEmpty(shippingState.Text) || string.IsNullOrEmpty(shippingPostCode.Text) || string.IsNullOrEmpty(shippingCountry.Text) || string.IsNullOrEmpty(shippingAddressName.Text))
                    {
                        Toast.MakeText(Activity, Resource.String.CheckoutViewAllRequiredFieldsMustBeFilled, ToastLength.Short).Show();
                        return false;
                    }
                }

                if (View.FindViewById<RadioButton>(Resource.Id.CheckoutViewPayCreditCard).Checked)
                {
                    if (string.IsNullOrEmpty(paymentCardNumber.Text) || string.IsNullOrEmpty(paymentMM.Text) || string.IsNullOrEmpty(paymentYYYY.Text) || string.IsNullOrEmpty(paymentCVV.Text))
                    {
                        Toast.MakeText(Activity, Resource.String.CheckoutViewMustEnterCreditCardInfo, ToastLength.Short).Show();
                        return false;
                    }

                    if (!useShippingAddress.Checked)
                    {
                        if (string.IsNullOrEmpty(billingShippingAddressName.Text) || string.IsNullOrEmpty(billingShippingAddressOne.Text) || string.IsNullOrEmpty(billingShippingCity.Text) || string.IsNullOrEmpty(billingShippingPostCode.Text) || string.IsNullOrEmpty(billingShippingCountry.Text))
                        {
                            Toast.MakeText(Activity, Resource.String.CheckoutViewAllRequiredFieldsMustBeFilled, ToastLength.Short).Show();
                            return false;
                        }
                    }
                }
            }      

            return true;
        }

        private void Next()
        {
            if (ValidateData())
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof (CheckoutTotalActivity));

                intent.PutExtra(BundleConstants.DeliveryType, (int) ShippingMedhod.HomeDelivery);

                intent.PutExtra(BundleConstants.ShippingName, shippingAddressName.Text);
                intent.PutExtra(BundleConstants.ShippingAddressOne, shippingAddressOne.Text);
                intent.PutExtra(BundleConstants.ShippingAddressTwo, shippingAddressTwo.Text);
                intent.PutExtra(BundleConstants.ShippingAddressCity, shippingCity.Text);
                intent.PutExtra(BundleConstants.ShippingAddressState, shippingState.Text);
                intent.PutExtra(BundleConstants.ShippingAddressPostCode, shippingPostCode.Text);
                intent.PutExtra(BundleConstants.ShippingAddressCountry, shippingCountry.Text);

                if (View.FindViewById<RadioButton>(Resource.Id.CheckoutViewPayCreditCard).Checked)
                {
                    intent.PutExtra(BundleConstants.PaymentType, (int) LoyPaymentType.CreditCard);
                }
                else if (View.FindViewById<RadioButton>(Resource.Id.CheckoutViewPayOnDelivery).Checked)
                {
                    intent.PutExtra(BundleConstants.PaymentType, (int)LoyPaymentType.PayOnDelivery);
                }

                if (useShippingAddress.Checked)
                {
                    intent.PutExtra(BundleConstants.BillingName, shippingAddressName.Text);
                    intent.PutExtra(BundleConstants.BillingAddressOne, shippingAddressOne.Text);
                    intent.PutExtra(BundleConstants.BillingAddressTwo, shippingAddressTwo.Text);
                    intent.PutExtra(BundleConstants.BillingAddressCity, shippingCity.Text);
                    intent.PutExtra(BundleConstants.ShippingAddressState, shippingState.Text);
                    intent.PutExtra(BundleConstants.BillingAddressPostCode, shippingPostCode.Text);
                    intent.PutExtra(BundleConstants.BillingAddressCountry, shippingCountry.Text);
                }
                else
                {
                    intent.PutExtra(BundleConstants.BillingName, billingShippingAddressName.Text);
                    intent.PutExtra(BundleConstants.BillingAddressOne, billingShippingAddressOne.Text);
                    intent.PutExtra(BundleConstants.BillingAddressTwo, billingShippingAddressTwo.Text);
                    intent.PutExtra(BundleConstants.BillingAddressCity, billingShippingCity.Text);
                    intent.PutExtra(BundleConstants.ShippingAddressState, billingShippingState.Text);
                    intent.PutExtra(BundleConstants.BillingAddressPostCode, billingShippingPostCode.Text);
                    intent.PutExtra(BundleConstants.BillingAddressCountry, billingShippingCountry.Text);
                }

                intent.PutExtra(BundleConstants.CardNumber, paymentCardNumber.Text);
                intent.PutExtra(BundleConstants.CardMM, paymentMM.Text);
                intent.PutExtra(BundleConstants.CardYYYY, paymentYYYY.Text);
                intent.PutExtra(BundleConstants.CardCVV, paymentCVV.Text);

                StartActivityForResult(intent, 0);
            }
            else
            {
                //SHOW ERROR
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == 0 && resultCode == (int) Result.Ok)
            {
                Activity.SetResult(Result.Ok);
                Activity.Finish();
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}