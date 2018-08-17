using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace Presentation.Dialogs
{
    public class BaseAlertDialog : Java.Lang.Object, View.IOnClickListener
    {
        private AlertDialog.Builder Builder;
        private AlertDialog dialog;

        private Action positiveClick;
        private Action negativeClick;
        private Action neutralClick;

        private Button positiveButton;
        private Button negativeButton;
        private Button neutralButton;

        protected bool KeyboardIsShowing;

        protected View BaseView { get; set; }
        protected LayoutInflater LayoutInflater;
        protected Context Context { get; set; }

        public string Message { get; set; }
        public string Title { get; set; }
        public bool DismissOnClick { get; set; }

        public BaseAlertDialog(Context context)
        {
            DismissOnClick = true;

            Context = context;
            LayoutInflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);

            BaseView = Util.Utils.ViewUtils.Inflate(LayoutInflater, Resource.Layout.AlertDialogBase);

            Builder = new AlertDialog.Builder(context);
        }

        protected void CreateDialog()
        {
            CreateDialog(null);
        }

        public virtual void Show()
        {
            CreateDialog();
        }

        protected void CreateDialog(View view, bool showKeyboard = false)
        {
            if (string.IsNullOrEmpty(Title))
            {
                BaseView.FindViewById(Resource.Id.AlertDialogBaseViewTitle).Visibility = ViewStates.Gone;
            }
            else
            {
                BaseView.FindViewById<TextView>(Resource.Id.AlertDialogBaseViewTitle).Text = Title;
            }

            if (!string.IsNullOrEmpty(Message))
            {
                BaseView.FindViewById<TextView>(Resource.Id.AlertDialogBaseViewMessage).Text = Message;
            }
            else
            {
                BaseView.FindViewById<TextView>(Resource.Id.AlertDialogBaseViewMessage).Visibility = ViewStates.Gone;
            }

            if (BaseView.FindViewById<Button>(Resource.Id.AlertDialogBasePositiveButton).Visibility == ViewStates.Gone)
            {
                SetPositiveButton(Context.GetString(Resource.String.ApplicationOk), null);
            }

            var frameLayout = BaseView.FindViewById<FrameLayout>(Resource.Id.AlertDialogBaseViewContent);
            if(view != null)
                frameLayout.AddView(view);
            else
                frameLayout.Visibility = ViewStates.Gone;

            dialog = Builder.Create();
            dialog.SetView(BaseView, 0, 0, 0, 0);

            if(showKeyboard)
                dialog.Window.SetSoftInputMode(SoftInput.StateVisible);
            else
                dialog.Window.SetSoftInputMode(SoftInput.StateHidden);

            KeyboardIsShowing = showKeyboard;

            dialog.Show();
        }

        public BaseAlertDialog SetPositiveButton(string positiveButtonText, Action positiveClick)
        {
            positiveButton = BaseView.FindViewById<Button>(Resource.Id.AlertDialogBasePositiveButton);

            this.positiveClick = positiveClick;

            positiveButton.Text = positiveButtonText.ToUpper();

            positiveButton.SetOnClickListener(this);

            positiveButton.Visibility = ViewStates.Visible;

            return this;
        }

        public BaseAlertDialog SetNegativeButton(string negativeButtonText, Action negativeClick)
        {
            negativeButton = BaseView.FindViewById<Button>(Resource.Id.AlertDialogBaseNegativeButton);

            this.negativeClick = negativeClick;

            negativeButton.Text = negativeButtonText.ToUpper();

            negativeButton.SetOnClickListener(this);

            negativeButton.Visibility = ViewStates.Visible;

            return this;
        }

        public BaseAlertDialog SetNeutralButton(string neutralButtonText, Action neutralClick)
        {
            neutralButton = BaseView.FindViewById<Button>(Resource.Id.AlertDialogBaseNeutralButton);

            this.neutralClick = neutralClick;

            neutralButton.Text = neutralButtonText.ToUpper();

            neutralButton.SetOnClickListener(this);

            neutralButton.Visibility = ViewStates.Visible;

            return this;
        }

        public BaseAlertDialog SetCancelable(bool cancelable)
        {
            Builder.SetCancelable(cancelable);
            return this;
        }

        public void Dismiss()
        {
            dialog.Dismiss();
        }

        public virtual void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.AlertDialogBasePositiveButton:
                    PositiveButtonClicked();
                    break;

                case Resource.Id.AlertDialogBaseNegativeButton:
                    NegativeButtonClicked();
                    break;

                case Resource.Id.AlertDialogBaseNeutralButton:
                    NeutralButtonClicked();
                    break;
            }
        }

        protected virtual void PositiveButtonClicked()
        {
            if (positiveClick != null)
                positiveClick();

            if (DismissOnClick)
                Dismiss();
        }

        protected virtual void NegativeButtonClicked()
        {
            if (negativeClick != null)
                negativeClick();

            if (DismissOnClick)
                Dismiss();
        }

        protected virtual void NeutralButtonClicked()
        {
            if (neutralClick != null)
                neutralClick();

            if (DismissOnClick)
                Dismiss();
        }

        protected void ShowButtons()
        {
            BaseView.FindViewById(Resource.Id.AlertDialogBaseButtonBar).Visibility = ViewStates.Visible;
        }

        protected void HideButtons()
        {
            BaseView.FindViewById(Resource.Id.AlertDialogBaseButtonBar).Visibility = ViewStates.Gone;
        }

        protected void ToggleKeyboard()
        {
            if (dialog == null)
                return;

            InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            imm.ToggleSoftInput(ShowFlags.Implicit, 0);

            KeyboardIsShowing = !KeyboardIsShowing;
        }
    }
}