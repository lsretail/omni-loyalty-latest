using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base;
using Presentation.Activities.Login;
using Presentation.Dialogs;
using Presentation.Util;
using LSRetail.Omni.GUIExtensions.Android.Dialog;

namespace Presentation.Models
{
    public abstract class BaseModel
    {
        private IRefreshableActivity refreshableActivity;
        private string securityTokenInUse;
        private Context applicationContext;
        
        protected Context Context;
        protected bool Stopped { get; private set; }

        protected BaseModel(Context context, IRefreshableActivity refreshableActivity = null)
        {
            this.Context = context;
            this.refreshableActivity = refreshableActivity;

            if (Context != null)
            {
                applicationContext = Context.ApplicationContext;
            }
        }

        protected void BeginWsCall()
        {
            if (string.IsNullOrEmpty(securityTokenInUse) || AppData.Device.SecurityToken != securityTokenInUse)
            {
                CreateService();
            }

            this.securityTokenInUse = AppData.Device.SecurityToken;
        }

        protected abstract void CreateService();

        protected void ShowToast(int messageResource)
        {
            ShowToast(Context.GetString(messageResource));
        }

        protected async Task ShowErrorAsync(string message)
        {
            var dialog = new AsyncDialog(Context);
            dialog.SetMessage(message);
            dialog.SetPositiveButton(Resource.String.ApplicationOk);

            await dialog.Show();
        }

        protected void ShowToast(string message, ToastLength toastLength = ToastLength.Short)
        {
            Toast.MakeText(Context, message, toastLength).Show();
        }

        protected void ShowToast(int resId, ToastLength toastLength = ToastLength.Short)
        {
            Toast.MakeText(Context, resId, toastLength).Show();
        }

        protected Snackbar CreateSnackbar(string message, int duration = Snackbar.LengthLong)
        {
            var view = ((Activity)Context).FindViewById(Resource.Id.BaseActivityScreenDrawerLayout);
            //View rootView = ((Activity) Context).Window.DecorView;//.FindViewById(Android.Resource.Id.Content);

            if (view == null)
            {
                return null;
            }

            return Snackbar.Make(view, message, duration);
        }

        protected Snackbar AddSnackbarAction(Snackbar snackbar, string message, Action<View> action)
        {
            if (snackbar == null)
                return null;

            snackbar.SetAction(message, action);

            return snackbar;
        }

        protected void ShowSnackbar(Snackbar snackbar)
        {
            if (snackbar == null)
                return;

            using (snackbar)
            {
                snackbar.Show();
            }
        }

        public static Snackbar CreateStaticSnackbar(Context context, string message, int duration = Snackbar.LengthLong)
        {
            var view = ((Activity)context).FindViewById(Resource.Id.BaseActivityScreenDrawerLayout);
            //View rootView = ((Activity) Context).Window.DecorView;//.FindViewById(Android.Resource.Id.Content);

            if (view == null)
            {
                return null;
            }

            return Snackbar.Make(view, message, duration);
        }

        public static Snackbar AddStaticSnackbarAction(Snackbar snackbar, string message, Action<View> action)
        {
            if (snackbar == null)
                return null;

            snackbar.SetAction(message, action);

            return snackbar;
        }

        public static void ShowStaticSnackbar(Snackbar snackbar)
        {
            if (snackbar == null)
                return;

            using (snackbar)
            {
                snackbar.Show();
            }
        }

        public async Task<string> HandleUIExceptionAsync(Exception ex, bool displayAlert = true, bool showAsToast = false, bool showToastOnNetworkError = true, bool checkSecurityTokenException = true)
        {
            ex = GetInnerException(ex);

            Log.Debug("Model Error", ex.GetType().ToString());
            Log.Debug("Model Error", ex.ToString());
            Log.Debug("Model Error", ex.Message);
            Log.Debug("Model Error", ex.StackTrace);

            string msg = string.Empty;

            if (Context != null)
            {
                msg = Context.GetString(Resource.String.ModelGenericException);

                if (ex is LSOmniException)
                {
                    var lsOmniException = (LSOmniException) ex;

                    if (lsOmniException.StatusCode == StatusCode.AuthFailed)
                    {
                        msg = Context.GetString(Resource.String.ModelAuthenticationFailed);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.CommunicationFailure)
                    {
                        showAsToast = true;
                        msg = Context.GetString(Resource.String.ModelNetworkException);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.AccessNotAllowed)
                    {
                        msg = Context.GetString(Resource.String.ModelAccessNotAllowed);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.EmailExists)
                    {
                        msg = Context.GetString(Resource.String.ModelEmailExists);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.EmailInvalid)
                    {
                        msg = Context.GetString(Resource.String.AccountViewEmailInvalid);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.UserNameExists)
                    {
                        msg = Context.GetString(Resource.String.AccountViewUserNameExists);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.UserNameInvalid)
                    {
                        msg = Context.GetString(Resource.String.AccountViewUserNameInvalid);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.PasswordInvalid)
                    {
                        msg = Context.GetString(Resource.String.AccountViewPasswordInvalid);
                    }
                    else if (lsOmniException.StatusCode == StatusCode.SecurityTokenInvalid || lsOmniException.StatusCode == StatusCode.UserNotLoggedIn)
                    {
                        msg = Context.GetString(Resource.String.ModelSecurityToken);

                        if (checkSecurityTokenException)
                        {
                            displayAlert = false;
                            var intent = new Intent();
                            intent.SetClass(Context, typeof (LoginActivity));
                            intent.PutExtra(BundleConstants.IsInsideApp, true);
                            Context.StartActivity(intent);
                        }
                    }
                }
                // 
                if (Context != null && displayAlert)
                {
                    if (showAsToast)
                    {
                        ShowToast(msg);
                    }
                    else
                    {
                        await ShowErrorAsync(msg);
                    }
                }
            }

            return msg;
        }

        protected void SendBroadcast(string action)
        {
            applicationContext.SendBroadcast(new Intent(action));
        }

        public void ShowIndicator(bool show)
        {
            if (Context != null && !Stopped && refreshableActivity != null)
            {
                refreshableActivity.ShowIndicator(show);
            }
        }

        private Exception GetInnerException(Exception exception)
        {
            if (exception is AggregateException)
            {
                return exception.InnerException;
            }

            return exception;
        }

        public void Stop()
        {
            Stopped = true;
        }
    }
}