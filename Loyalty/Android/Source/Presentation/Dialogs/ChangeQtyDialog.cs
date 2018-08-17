using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Presentation.Adapters;
using Presentation.Util;

namespace Presentation.Dialogs
{
    public class ChangeQtyDialog : BaseAlertDialog, View.IOnKeyListener, View.IOnClickListener
    {
        private readonly Action<decimal> onOkButtonClicked;
        private View view;

        private EditText qtyToAdd;

        public ChangeQtyDialog(Context context, string title, decimal currentQty, Action<decimal> onOkButtonClicked)
            : base(context)
        {
            Title = title;
            Message = context.GetString(Resource.String.ShoppingListDetailViewEnterNewQuantity);

            this.onOkButtonClicked = onOkButtonClicked;
            view = Util.Utils.ViewUtils.Inflate(LayoutInflater, Resource.Layout.AlertDialogChangeQty);

            qtyToAdd = view.FindViewById<EditText>(Resource.Id.AlertDialogChangeQtyHowManyEditText);

            this.SetNegativeButton(context.GetString(Resource.String.ApplicationCancel), () => { });
            this.SetPositiveButton(context.GetString(Resource.String.ApplicationOk), ChangeQty);

            qtyToAdd.Text = currentQty.ToString();
            qtyToAdd.SelectAll();

            qtyToAdd.SetOnKeyListener(this);
            HideKeyboard();

            var decreaseButton = view.FindViewById<Button>(Resource.Id.AlertDialogChangeQtyReduceQty);
            var increaseButton = view.FindViewById<Button>(Resource.Id.AlertDialogChangeQtyIncreaseQty);

            decreaseButton.SetOnClickListener(this);
            increaseButton.SetOnClickListener(this);
        }

        private void HideKeyboard()
        {
            InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(qtyToAdd.WindowToken, 0);
        }

        public override void Show()
        {
            CreateDialog(view, true);
        }

        public bool OnKey(View v, Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Enter)
            {
                ChangeQty();
                return true;
            }

            return false;
        }

        public override void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.AlertDialogChangeQtyReduceQty:
                    var decreaseQty = 0m;
                    decimal.TryParse(qtyToAdd.Text, out decreaseQty);

                    decreaseQty -= 1;

                    if(decreaseQty > 0)
                        qtyToAdd.Text = decreaseQty.ToString();
            
                    qtyToAdd.SelectAll();
                    break;

                case Resource.Id.AlertDialogChangeQtyIncreaseQty:
                    var increaseQty = 0m;
                    decimal.TryParse(qtyToAdd.Text, out increaseQty);

                    qtyToAdd.Text = Math.Max(0, increaseQty + 1).ToString();
            
                    qtyToAdd.SelectAll();
                    break;

                default:
                    base.OnClick(v);
                    break;
            }
        }

        private void ChangeQty()
        {
            var qty = 0m;
            decimal.TryParse(qtyToAdd.Text, out qty);

            onOkButtonClicked(qty);

            this.Dismiss();
        }
    }
}