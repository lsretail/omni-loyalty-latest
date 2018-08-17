using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Util;
//using Xamarin.FacebookBinding;
//using Xamarin.FacebookBinding.Model;
//using Xamarin.FacebookBinding.Widget;
using Exception = Java.Lang.Exception;

namespace Presentation.Activities.Base
{
    public abstract class BaseSocialMediaActivity// : LoyaltyFragmentActivity, IRefreshableActivity, IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener, LoginButton.IUserInfoChangedCallback, Session.IStatusCallback, Request.ICallback, WebDialog.IOnCompleteListener
    {
        #if FALSE
        private class PendingAction
        {
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string Details { get; set; }
            public string ImageLink { get; set; }
        }

        private string AccountName { get; set; }
        private string name { get; set; }
        private string MiddleName { get; set; }
        private string LastName { get; set; }
        private PendingAction pendingAction;


        protected bool StartSocialMediaOnResume { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StartSocialMediaOnResume = true;

            uiHelper = new UiLifecycleHelper(this, this); 
            uiHelper.OnCreate(savedInstanceState);

            //TODO
            /*if (savedInstanceState != null)
            {
                string name = savedInstanceState.GetString(PENDING_ACTION_BUNDLE_KEY);
                pendingAction = (PendingAction)Enum.Parse(typeof(PendingAction), name);
            }*/

            plusClient = new PlusClient.Builder(this, this, this)
                .SetActions("http://schemas.google.com/AddActivity", "http://schemas.google.com/BuyActivity")
                .SetScopes(Scopes.PlusLogin).Build();

            var session = Session.ActiveSession;


            //TODO if loggen in
            /*if (session == null)
            {
                if (savedInstanceState != null)
                {
                    session = Session.RestoreSession(this, null, this, savedInstanceState);
                }
                if (session == null)
                {
                    session = new Session(this);
                }
                Session.ActiveSession = session;
                if (session.State == SessionState.CreatedTokenLoaded)
                {
                    session.OpenForRead(new Session.OpenRequest(this).SetCallback(this).SetPermissions(new List<string>(){"email"}));
                }
            }*/
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (StartSocialMediaOnResume)
            {
                if (AppData.SocialMediaConnection == SocialMediaConnection.Google)
                    plusClient.Connect();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (AppData.SocialMediaConnection == SocialMediaConnection.Google)
                plusClient.Disconnect();
        }

        protected override void OnResume()
        {
            base.OnResume();

            var session = Session.ActiveSession;
            if (session != null && (session.IsOpened || session.IsClosed))
            {
                Call(session, session.State, null);
            }

            uiHelper.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            uiHelper.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiHelper.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            uiHelper.OnSaveInstanceState(outState);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            ShowIndicator(false);

            if (requestCode == ResolveGooglePlusLoginRequestCode && resultCode == Result.Ok)
                Connect(SocialMediaConnection.Google);
            else
            {
                uiHelper.OnActivityResult(requestCode, (int)resultCode, data);
            }
        }

        public void Connect(SocialMediaConnection connection, bool openPublishConnection = false)
        {
            if(connection == SocialMediaConnection.None)
                return;

            switch (connection)
            {
                case SocialMediaConnection.Google:
                    if(!plusClient.IsConnected && !plusClient.IsConnecting)
                        plusClient.Connect();
                    break;

                case SocialMediaConnection.Facebook:
                    var session = Session.ActiveSession;

                    if (session == null)
                    {
                        session = new Session.Builder(this).SetApplicationId(GetString(Resource.String.AppId)).Build();
                        Session.ActiveSession = session;
                    }
                    else if (!session.IsOpened || (openPublishConnection && !session.Permissions.Contains("publish_actions")))
                    {
                        if (openPublishConnection)
                            session.OpenForPublish(new Session.OpenRequest(this).SetPermissions(new List<string>() { "publish_actions" }));
                        else
                            session.OpenForRead(new Session.OpenRequest(this).SetPermissions(new List<string>(){"email"}));
                    }
                    else
                    {
                        new Request(session, "/me", null, HttpMethod.Get, this).ExecuteAsync();
                    }
                    break;
            }
        }

        private void ClearData()
        {
            AccountName = string.Empty;
            name = string.Empty;
            MiddleName = string.Empty;
            LastName = string.Empty;
        }

        private void SocialMediaConnected(SocialMediaConnection connection)
        {
            AppData.SocialMediaConnection = connection;

            var firstName = string.Empty;
            var middleName = string.Empty;
            var lastName = string.Empty;

            var accountName = AccountName;

            if (!string.IsNullOrEmpty(name))
                firstName = name;

            if (!string.IsNullOrEmpty(MiddleName))
                firstName = MiddleName;

            if (!string.IsNullOrEmpty(LastName))
                firstName = LastName;

            SocialMediaConnected(accountName, firstName, middleName, lastName);
        }

        public void Share(SocialMediaConnection connection, string title, string subtitle, string details, string imageLink)
        {
            pendingAction = null;
            switch (connection)
            {
                case SocialMediaConnection.Google:
                    if (!plusClient.IsConnected)
                    {
                        pendingAction = new PendingAction(){Title = title, SubTitle = subtitle, Details = details, ImageLink = imageLink};
                        Connect(connection);
                    }
                    else
                    {
                        var builder = new PlusShare.Builder(this, plusClient);

                        builder.SetText("Lemon Cheesecake recipe")
                               .SetType("text/plain")
                               .SetContentDeepLinkId("/cheesecake/lemon", /** Deep-link identifier */
                                                     "Lemon Cheesecake recipe", /** Snippet title */
                                                     "A tasty recipe for making lemon cheesecake.",
                                                     /** Snippet description */
                                                     Android.Net.Uri.Parse(
                                                         "https://lh4.googleusercontent.com/-bm7Hl_MjWLo/UqXbHvoLUVI/AAAAAAAAAA0/8Tw4cBGtJ5w/w500-h450-no/2013+-+1"));
                        /*builder.SetStream(Android.Net.Uri.Parse(MediaStore.Images.Media.InsertImage(ContentResolver, Util.Utils.DecodeImage(image), item.Description, item.Details)));*/

                        StartActivityForResult(builder.Intent, 0);
                    }
                    break;
                case SocialMediaConnection.Facebook:
                    var session = Session.ActiveSession;

                    if (session == null || !session.IsOpened || !session.Permissions.Contains("publish_actions"))
                    {
                        pendingAction = new PendingAction() { Title = title, SubTitle = subtitle, Details = details };
                        Connect(SocialMediaConnection.Facebook, true);
                    }
                    else
                    {
                        var bundle = new Bundle();
                        bundle.PutString("name", title);
                        bundle.PutString("caption", subtitle);
                        bundle.PutString("description", details);
                        bundle.PutString("link", "http://www.lsretail.com/products/ls-omni");
                        bundle.PutString("picture", imageLink);
    
                        var dialog = new WebDialog.FeedDialogBuilder(this, session, bundle).Build();
                        dialog.OnCompleteListener = this;
                        dialog.Show();

                    }
                    break;
            }
        }

        protected abstract void SocialMediaConnected(string accountName, string firstName, string middleName, string lastName);

        public abstract void ShowIndicator(bool show);

        #region PLUS CLIENT

        private ConnectionResult connectionResult;
        private PlusClient plusClient;

        protected const int ResolveGooglePlusLoginRequestCode = 9000;

        public void OnConnected(Bundle p0)
        {
            if(AppData.SocialMediaConnection != SocialMediaConnection.Google)
                ClearData();

            AppData.SocialMediaConnection = SocialMediaConnection.Google;

            AccountName = plusClient.AccountName;

            if (plusClient.CurrentPerson != null && plusClient.CurrentPerson.HasName)
            {
                if (plusClient.CurrentPerson.Name.HasGivenName)
                    name = plusClient.CurrentPerson.Name.GivenName;

                if (plusClient.CurrentPerson.Name.HasMiddleName)
                    MiddleName = plusClient.CurrentPerson.Name.MiddleName;

                if (plusClient.CurrentPerson.Name.HasFamilyName)
                    LastName = plusClient.CurrentPerson.Name.FamilyName;

            }

            if (pendingAction != null)
            {
                Share(SocialMediaConnection.Google, pendingAction.Title, pendingAction.SubTitle, pendingAction.Details, pendingAction.ImageLink);
            }

            SocialMediaConnected(AccountName, name, MiddleName, LastName);
        }

        public void OnDisconnected()
        {
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            if (result.HasResolution)
            {
                try
                {
                    result.StartResolutionForResult(this, ResolveGooglePlusLoginRequestCode);
                }
                catch (IntentSender.SendIntentException e)
                {
                    plusClient.Connect();
                }
            }
            // Save the result and resolve the connection failure upon a user click.
            connectionResult = result;
        }

        #endregion

        #region FACEBOOK
        
        protected UiLifecycleHelper uiHelper;

        public void OnUserInfoFetched(IGraphUser user)
        {
            if (AppData.SocialMediaConnection != SocialMediaConnection.Facebook)
                ClearData();
            AppData.SocialMediaConnection = SocialMediaConnection.Facebook;

            Session session = Session.ActiveSession;

            if (user != null)
            {
                AccountName = user.GetProperty("email").ToString();
                name = user.name;
                MiddleName = user.MiddleName;
                LastName = user.LastName;

                SocialMediaConnected(AccountName, name, MiddleName, LastName);
            }
        }

        public void Call(Session session, SessionState state, Exception exception)
        {
            if (pendingAction != null &&         //TODO posting
                (exception is FacebookOperationCanceledException ||
                exception is FacebookAuthorizationException))
            {       //TODO cancelled action???
                /*new AlertDialog.Builder(this)
                    .SetTitle(Resource.String.cancelled)
                        .SetMessage(Resource.String.permission_not_granted)
                        .SetPositiveButton(Resource.String.ok, (object sender, DialogClickEventArgs e) => { })
                        .Show();
                pendingAction = PendingAction.NONE;*/
            }
            else if (state == SessionState.OpenedTokenUpdated || state == SessionState.Opened)
            {
                HandlePendingAction();
            }
        }

        public void OnCompleted(Response response)
        {
            AccountName = response.GraphObject.GetProperty("email").ToString();
            name = (string) response.GraphObject.GetProperty("first_name");
            MiddleName = (string)response.GraphObject.GetProperty("middle_name");
            LastName = (string)response.GraphObject.GetProperty("last_name");

            SocialMediaConnected(SocialMediaConnection.Facebook);
        }

        private void HandlePendingAction()
        {
            PendingAction previouslyPendingAction = pendingAction;
            // These actions may re-set pendingAction if they are still pending, but we assume they
            // will succeed.
            pendingAction = null;

            if(previouslyPendingAction != null)
            {
                Share(SocialMediaConnection.Facebook, previouslyPendingAction.Title, previouslyPendingAction.SubTitle, previouslyPendingAction.Details, previouslyPendingAction.ImageLink);
            }
        }

        public void OnComplete(Bundle bundle, FacebookException exception)
        {
            var x = exception;
        }

        #endregion

        #region TWITTER

        #endregion
#endif
    }
}