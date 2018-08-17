using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base;
using Presentation.Utils;

namespace Presentation.Models
{
    public abstract class BaseModel
    {
        protected enum LoadingType
        {
            Contact = 0,
            Menu = 1,
        }

        private string securityTokenInUse;
        private Context applicationContext;
        private readonly IRefreshableActivity refreshableActivity;

        protected Context Context;
        protected bool Stopped { get; private set; }

        protected BaseModel(Context context, IRefreshableActivity refreshableActivity)
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
            if (string.IsNullOrEmpty(securityTokenInUse) || AppData.SecurityToken != securityTokenInUse)
            {
                CreateService();
            }

            this.securityTokenInUse = AppData.SecurityToken;
        }

        protected abstract void CreateService();

        protected void ShowToast(int messageResource)
        {
            var msg = Context.Resources.GetString(messageResource);
            
            ShowToast(msg);

            //ShowToast();
        }

        protected void ShowToast(string message)
        {
            var view = ((Activity) Context).FindViewById(Resource.Id.BaseActivityScreenDrawerLayout);
            //View rootView = ((Activity) Context).Window.DecorView;//.FindViewById(Android.Resource.Id.Content);

            if (view == null)
            {
                return;
            }

            using (var snackbar = Snackbar.Make(view, message, Snackbar.LengthShort))
                snackbar.Show();

            //Toast.MakeText(applicationContext, message, ToastLength.Short).Show();
        }

        public static void ShowToast(View view, int messageResource)
        {
            using (var snackbar = Snackbar.Make(view, messageResource, Snackbar.LengthShort))
                snackbar.Show();
        }

        protected string HandleUIException(Exception ex, bool displayAlert = true)
        {
            if (ex is AggregateException)
            {
                return HandleUIException(ex.InnerException);
            }

            LogUtils.Log(ex.GetType().ToString());
            LogUtils.Log(ex.ToString());
            LogUtils.Log(ex.Message);
            LogUtils.Log(ex.StackTrace);

            string msg = string.Empty;

            if (Context != null)
            {
                msg = Context.Resources.GetString(Resource.String.ModelGenericException);

                if (ex is LSOmniException)
                {
                    var statusCode = ((LSOmniException) ex).StatusCode;

                    if (statusCode == StatusCode.AuthFailed)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelAuthenticationFailed);
                    }
                    else if(statusCode == StatusCode.CommunicationFailure)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelNetworkException);
                    }
                    else if (statusCode == StatusCode.DeviceIsBlocked)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelDeviceBlocked);
                    }
                    else if(statusCode == StatusCode.AccessNotAllowed)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelAccessNotAllowed);
                    }
                    else if (statusCode == StatusCode.EmailExists)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelEmailExists);
                    }
                    else if(statusCode == StatusCode.UserNameExists)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelEmailExists);
                    }
                    else if (statusCode == StatusCode.UserNameInvalid)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelEmailInvalid);
                    }
                    else if(statusCode == StatusCode.PasswordInvalid)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelPasswordInvalid);
                    }
                    else if (statusCode == StatusCode.SecurityTokenInvalid || statusCode == StatusCode.UserNotLoggedIn)
                    {
                        msg = Context.Resources.GetString(Resource.String.ModelSecurityToken);
                        displayAlert = false;
                    }
                }

                if (Context != null && displayAlert)
                {
                    ShowToast(msg);
                }
            }

            return msg;
        }

        protected void SendBroadcast(string action)
        {
            applicationContext.SendBroadcast(new Intent(action));
        }

        protected void Show(bool show)
        {
            if(refreshableActivity != null)
                refreshableActivity.ShowIndicator(show);
        }

        public void Stop()
        {
            Stopped = true;
        }

        protected void SetLoading(LoadingType type, AppData.Status status)
        {
            switch (type)
            {
                case LoadingType.Contact:
                    AppData.ContactUpdateStatus = status;
                    break;
            }
        }
    }
}