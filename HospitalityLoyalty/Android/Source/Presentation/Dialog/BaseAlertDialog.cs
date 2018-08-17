using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace Presentation.Dialog
{
    public class BaseAlertDialog : Java.Lang.Object, View.IOnClickListener, IDialogInterfaceOnDismissListener
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public bool Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        protected bool KeyboardIsShowing;

        protected View BaseView { get; set; }
        protected LayoutInflater LayoutInflater;
        protected Context Context { get; set; }

        private AlertDialog.Builder Builder;
        private AlertDialog dialog;

        private Action positiveClick;
        private Action negativeClick;
        private Action neutralClick;
        private bool delay = false;

        public BaseAlertDialog(Context context)
        {
            Context = context;
            LayoutInflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);

            BaseView = Utils.Utils.ViewUtils.Inflate(LayoutInflater, Resource.Layout.AlertDialogBase);

            Builder = new AlertDialog.Builder(context);
            //Builder = new AlertDialog.Builder(new ContextThemeWrapper(context, Resource.Style.CustomDialog));
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
                SetPositiveButton(Context.Resources.GetString(Resource.String.Ok), null);
            }

            var frameLayout = BaseView.FindViewById<FrameLayout>(Resource.Id.AlertDialogBaseViewContent);
            if (view != null)
                frameLayout.AddView(view);
            else
                frameLayout.Visibility = ViewStates.Gone;

            dialog = Builder.Create();
            dialog.SetView(BaseView, 0, 0, 0, 0);

            if (showKeyboard)
                dialog.Window.SetSoftInputMode(SoftInput.StateVisible);
            else
                dialog.Window.SetSoftInputMode(SoftInput.StateHidden);

            KeyboardIsShowing = showKeyboard;

            dialog.SetOnDismissListener(this);

            dialog.Show();
        }

        public BaseAlertDialog SetPositiveButton(string positiveButtonText, Action positiveClick)
        {
            this.positiveClick = positiveClick;

            var button = BaseView.FindViewById<Button>(Resource.Id.AlertDialogBasePositiveButton);

            button.Text = positiveButtonText.ToUpper();

            button.SetOnClickListener(this);

            button.Visibility = ViewStates.Visible;

            return this;
        }

        public BaseAlertDialog SetNegativeButton(string negativeButtonText, Action negativeClick)
        {
            this.negativeClick = negativeClick;

            var button = BaseView.FindViewById<Button>(Resource.Id.AlertDialogBaseNegativeButton);

            button.Text = negativeButtonText.ToUpper();

            button.SetOnClickListener(this);

            button.Visibility = ViewStates.Visible;

            return this;
        }

        public BaseAlertDialog SetNeutralButton(string neutralButtonText, Action neutralClick)
        {
            this.neutralClick = neutralClick;

            var button = BaseView.FindViewById<Button>(Resource.Id.AlertDialogBaseNeutralButton);

            button.Text = neutralButtonText.ToUpper();

            button.SetOnClickListener(this);

            button.Visibility = ViewStates.Visible;

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
                    if (positiveClick != null)
                        positiveClick();
                    break;

                case Resource.Id.AlertDialogBaseNegativeButton:
                    if (negativeClick != null)
                        negativeClick();
                    break;

                case Resource.Id.AlertDialogBaseNeutralButton:
                    if (neutralClick != null)
                        neutralClick();
                    break;
            }

            //if(Delay)
            //    await Task.Delay(300);          //TODO fix for WindowSoftInputMode so keyboard resize screen wont screw up favtorites screen,  https://code.google.com/p/android/issues/detail?id=176187

            Dismiss();
        }

        protected void ToggleKeyboard()
        {
            if (dialog == null)
                return;

            InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            imm.ToggleSoftInput(ShowFlags.Implicit, 0);

            KeyboardIsShowing = !KeyboardIsShowing;
        }

        public virtual void OnDismiss(IDialogInterface dialog)
        {
        }
    }
}