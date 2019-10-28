using System;

using Android.Content;
using Android.Views;
using Android.Widget;

using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Dialogs
{
    public class VariantDialog : BaseAlertDialog, AdapterView.IOnItemSelectedListener, IRefreshableActivity
    {
        private View view;

        private BasketModel basketModel;

        private TextView titleOne;
        private TextView titleTwo;
        private TextView titleThree;
        private TextView titleFour;
        private TextView titleFive;
        private TextView titleSix;

        private Spinner variantSpinnerOne;
        private Spinner variantSpinnerTwo;
        private Spinner variantSpinnerThree;
        private Spinner variantSpinnerFour;
        private Spinner variantSpinnerFive;
        private Spinner variantSpinnerSix;

        private ViewSwitcher viewSwitcher;
        private View contentView;
        private View progressView;

        private Button changeQty;

        private OneListItem basketItem;
        private LoyItem item;

        private Action<VariantRegistration, decimal> onOkButtonClicked;

        public VariantDialog(Context context, OneListItem basketItem, LoyItem item, Action<VariantRegistration, decimal> onOkButtonClicked) : base(context)
        {
            this.basketItem = basketItem;
            Initialize(item, basketItem.VariantId, basketItem.Quantity, onOkButtonClicked);
        }

        public VariantDialog(Context context, LoyItem item, VariantRegistration selectedvariant, Action<VariantRegistration, decimal> onOkButtonClicked) : base(context)
        {
            Initialize(item, selectedvariant.Id, 1, onOkButtonClicked);
        }

        private void Initialize(LoyItem item, string variantId, decimal qty, Action<VariantRegistration, decimal> onOkButtonClicked)
        {
            basketModel = new BasketModel(Context, this);

            this.item = item;

            Title = Context.GetString(Resource.String.ItemViewSelectVariation);

            view = Util.Utils.ViewUtils.Inflate(LayoutInflater, Resource.Layout.AlertDialogVariant);

            titleOne = view.FindViewById<TextView>(Resource.Id.VariantTitleOne);
            titleTwo = view.FindViewById<TextView>(Resource.Id.VariantTitleTwo);
            titleThree = view.FindViewById<TextView>(Resource.Id.VariantTitleThree);
            titleFour = view.FindViewById<TextView>(Resource.Id.VariantTitleFour);
            titleFive = view.FindViewById<TextView>(Resource.Id.VariantTitleFive);
            titleSix = view.FindViewById<TextView>(Resource.Id.VariantTitleSix);

            variantSpinnerOne = view.FindViewById<Spinner>(Resource.Id.VariantSpinnerOne);
            variantSpinnerTwo = view.FindViewById<Spinner>(Resource.Id.VariantSpinnerTwo);
            variantSpinnerThree = view.FindViewById<Spinner>(Resource.Id.VariantSpinnerThree);
            variantSpinnerFour = view.FindViewById<Spinner>(Resource.Id.VariantSpinnerFour);
            variantSpinnerFive = view.FindViewById<Spinner>(Resource.Id.VariantSpinnerFive);
            variantSpinnerSix = view.FindViewById<Spinner>(Resource.Id.VariantSpinnerSix);

            viewSwitcher = view.FindViewById<ViewSwitcher>(Resource.Id.VariantViewSwitcher);
            contentView = view.FindViewById<View>(Resource.Id.VariantViewContent);
            progressView = view.FindViewById<View>(Resource.Id.VariantLoadingSpinner);

            var qtyLayout = view.FindViewById(Resource.Id.VariantQty);

            if (basketItem == null)
            {
                qtyLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                this.SetNeutralButton(
                Context.GetString(Resource.String.ApplicationDelete), () => { });
            }

            changeQty = view.FindViewById<Button>(Resource.Id.VariantChangeQty);
            var decreaseQty = view.FindViewById<ImageButton>(Resource.Id.VariantDecreaseQty);
            var increaseQty = view.FindViewById<ImageButton>(Resource.Id.VariantIncreaseQty);

            changeQty.Text = FormatQty(qty);

            decreaseQty.SetOnClickListener(this);
            changeQty.SetOnClickListener(this);
            increaseQty.SetOnClickListener(this);

            this.onOkButtonClicked = onOkButtonClicked;
            this.SetNegativeButton(Context.GetString(Resource.String.ApplicationCancel), () => { });
            this.SetPositiveButton(Context.GetString(Resource.String.ApplicationOk), ChangeVariant);

            VariantRegistration varreg = null;
            if (string.IsNullOrEmpty(variantId))
            {
                if (this.item.VariantsRegistration.Count > 0)
                    varreg = this.item.VariantsRegistration[0];
            }
            else
            {
                varreg = item.VariantsRegistration.Find(v => v.Id == variantId);
            }

            if (varreg != null)
            {
                VariantExt.SetIsSelectedFromVariantReg(this.item.VariantsExt, varreg);

                if (this.item.VariantsExt.Count > 0)
                {
                    InitializeSpinner(this.item.VariantsExt[0], titleOne, variantSpinnerOne);
                }

                if (this.item.VariantsExt.Count > 1)
                {
                    InitializeSpinner(this.item.VariantsExt[1], titleTwo, variantSpinnerTwo);
                }

                if (this.item.VariantsExt.Count > 2)
                {
                    InitializeSpinner(this.item.VariantsExt[2], titleThree, variantSpinnerThree);
                }

                if (this.item.VariantsExt.Count > 3)
                {
                    InitializeSpinner(this.item.VariantsExt[3], titleFour, variantSpinnerFour);
                }

                if (this.item.VariantsExt.Count > 4)
                {
                    InitializeSpinner(this.item.VariantsExt[4], titleFive, variantSpinnerFive);
                }

                if (this.item.VariantsExt.Count > 5)
                {
                    InitializeSpinner(this.item.VariantsExt[5], titleSix, variantSpinnerSix);
                }
            }
        }

        public override void Show()
        {
            CreateDialog(view);
        }

        private void InitializeSpinner(VariantExt variantExt, TextView title, Spinner spinner)
        {
            var adapter = new ColorSpinnerAdapter(Context, Resource.Layout.CustomSpinnerItem, Resource.Layout.CustomSpinnerMultiLineItem, variantExt.Values);
            spinner.Adapter = adapter;
            spinner.SetSelection(variantExt.Values.FindIndex(x => x.IsSelected));
            spinner.OnItemSelectedListener = this;
            spinner.Visibility = ViewStates.Visible;
            spinner.Enabled = true;

            title.Text = variantExt.Code;
            title.Visibility = ViewStates.Visible;
        }

        protected override async void PositiveButtonClicked()
        {
            VariantRegistration chosenVariant = VariantRegistration.GetVariantRegistrationFromVariantExts(item.VariantsExt, item.VariantsRegistration);

            if (basketItem == null)
            {
                base.PositiveButtonClicked();
            }
            else
            {
                decimal qty;
                decimal.TryParse(changeQty.Text, out qty);

                await basketModel.EditItem(basketItem.Id, qty, chosenVariant);

                base.PositiveButtonClicked();
                ChangeVariant();
            }
        }

        protected override async void NeutralButtonClicked()
        {
            await basketModel.DeleteItem(basketItem);

            base.NeutralButtonClicked();
        }

        private void ChangeVariant()
        {
            VariantRegistration variantRegistration = VariantRegistration.GetVariantRegistrationFromVariantExts(item.VariantsExt, item.VariantsRegistration);
            if (variantRegistration != null)
            {
                decimal qty;
                decimal.TryParse(changeQty.Text, out qty);
                onOkButtonClicked(variantRegistration, qty);
            }
        }

        private void ResetSpinners(int spinnerIndex, int selectedVariantIndex)
        {
            item.VariantsExt[spinnerIndex].Values.ForEach(variant => variant.IsSelected = false);
            item.VariantsExt[spinnerIndex].Values[selectedVariantIndex].IsSelected = true;

            for (int x = 0; x < item.VariantsExt.Count; x++)
            {
                Spinner spin = null;
                switch (x)
                {
                    case 0: spin = variantSpinnerOne; break;
                    case 1: spin = variantSpinnerTwo; break;
                    case 2: spin = variantSpinnerThree; break;
                    case 3: spin = variantSpinnerFour; break;
                    case 4: spin = variantSpinnerFive; break;
                    case 5: spin = variantSpinnerSix; break;
                }

                var adapter = spin.Adapter as ArrayAdapter;

                if (adapter != null) 
                    adapter.NotifyDataSetChanged();
            }
            
        }

        public override void OnClick(View v)
        {
            base.OnClick(v);

            var qty = 1m;
            decimal.TryParse(changeQty.Text, out qty);

            switch (v.Id)
            {
                case Resource.Id.VariantDecreaseQty:
                    var decreasedQty = (qty - 1);
                    if (decreasedQty > 0)
                        changeQty.Text = decreasedQty.ToString();
                    break;

                case Resource.Id.VariantChangeQty:
                    var changeQtyDialog = new ChangeQtyDialog(Context, item.Description, qty, newQty =>
                    {
                        if (newQty > 0)
                            changeQty.Text = newQty.ToString();
                    });
                    changeQtyDialog.Show();
                    break;

                case Resource.Id.VariantIncreaseQty:
                    changeQty.Text = (qty + 1).ToString();
                    break;
            }
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            switch (parent.Id)
            {
                case Resource.Id.VariantSpinnerOne:
                    ResetSpinners(0, position);
                    break;

                case Resource.Id.VariantSpinnerTwo:
                    ResetSpinners(1, position);
                    break;

                case Resource.Id.VariantSpinnerThree:
                    ResetSpinners(2, position);
                    break;

                case Resource.Id.VariantSpinnerFour:
                    ResetSpinners(3, position);
                    break;

                case Resource.Id.VariantSpinnerFive:
                    ResetSpinners(4, position);
                    break;

                case Resource.Id.VariantSpinnerSix:
                    ResetSpinners(5, position);
                    break;
            }
        }

        public void OnNothingSelected(AdapterView parent)
        {
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if (viewSwitcher.CurrentView != progressView)
                {
                    viewSwitcher.ShowNext();
                    HideButtons();
                }
            }
            else
            {
                if (viewSwitcher.CurrentView != contentView)
                {
                    viewSwitcher.ShowPrevious();
                    ShowButtons();
                }
            }
        }

        private string FormatQty(decimal qty)
        {
            if (item.UnitOfMeasures != null && item.UnitOfMeasures.Count > 0)
            {
                return qty.ToString("N" + item.UnitOfMeasures[0].Decimals);
            }
            else
            {
                return qty.ToString("N0");
            }
        }
    }
}
