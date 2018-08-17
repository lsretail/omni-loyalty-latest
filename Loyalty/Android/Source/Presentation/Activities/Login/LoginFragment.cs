
using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Infrastructure.Data.SQLite.Devices;
using Java.Lang;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using Presentation.Activities.Account;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Activities.Settings;
using Presentation.Models;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Login
{
    public class LoginFragment : LoyaltyFragment, IRefreshableActivity, TextView.IOnEditorActionListener, View.IOnClickListener, ITextWatcher
    {
        private const int CreateAccountRequestCode = 0;

        private MemberContactModel memberContactModel;
        private DeviceService deviceService;

        private TextInputLayout usernameInputLayout;
        private TextInputLayout passwordInputLayout;
        private EditText username;
        private EditText password;
        private TextView errorMessage;
        private ViewSwitcher viewSwitcher;
        private View progressView;
        private View contentView;

        private bool isInsideApp = false;
        private bool isLoggingIn = false;

        public static LoginFragment NewInstance()
        {
            var loginDetail = new LoginFragment() { Arguments = new Bundle() };
            return loginDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Login);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.LoginScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            HasOptionsMenu = true;

            memberContactModel = new MemberContactModel(Activity);

            deviceService = new DeviceService(new DeviceRepository());

            usernameInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.LoginScreenUsernameInputLayout);
            passwordInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.LoginScreenPasswordInputLayout);

            username = view.FindViewById<EditText>(Resource.Id.LoginScreenUsername);
            password = view.FindViewById<EditText>(Resource.Id.LoginScreenPassword);
            errorMessage = view.FindViewById<TextView>(Resource.Id.LoginViewErrorText);

            if (Arguments != null && Arguments.ContainsKey(BundleConstants.ErrorMessage))
            {
                errorMessage.Text = Arguments.GetString(BundleConstants.ErrorMessage);
                errorMessage.Visibility = ViewStates.Visible;
            }

            username.SetOnEditorActionListener(this);
            password.SetOnEditorActionListener(this);

            var loginButton = view.FindViewById<ColoredButton>(Resource.Id.LoginViewLoginButton);
            loginButton.SetOnClickListener(this);

            var createAccountButton = view.FindViewById<ColoredButton>(Resource.Id.LoginViewCreateAccountButton);
            createAccountButton.SetOnClickListener(this);

            var forgotPassword = view.FindViewById<TextView>(Resource.Id.LoginViewForgotPassword);
            forgotPassword.SetOnClickListener(this);

            viewSwitcher = view.FindViewById<ViewSwitcher>(Resource.Id.LoginViewSwitcher);
            progressView = view.FindViewById<View>(Resource.Id.LoginViewLoadingSpinner);
            contentView = view.FindViewById<View>(Resource.Id.LoginViewContent); 

            if (Arguments != null)
                isInsideApp = Arguments.GetBoolean(BundleConstants.IsInsideApp);

            if (isInsideApp)
            {
                ShowError(GetString(Resource.String.LoginViewInvalidSecurityToken));
            }
            else
            {
                SavePushNotification();
            }

            if (savedInstanceState != null)
            {
                username.Text = savedInstanceState.GetString(BundleConstants.Username);
                password.Text = savedInstanceState.GetString(BundleConstants.Password);
                errorMessage.Text = savedInstanceState.GetString(BundleConstants.ErrorMessage);

                if (!string.IsNullOrEmpty(errorMessage.Text))
                {
                    errorMessage.Visibility = ViewStates.Visible;
                }
                else
                {
                    errorMessage.Visibility = ViewStates.Gone;
                }

                isLoggingIn = savedInstanceState.GetBoolean(BundleConstants.IsLoggingIn);
                if (isLoggingIn)
                {
                    Logon();
                }
            }

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            username.AddTextChangedListener(this);
            password.AddTextChangedListener(this);
        }

        public override void OnPause()
        {
            username.RemoveTextChangedListener(this);
            password.RemoveTextChangedListener(this);

            base.OnPause();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString(BundleConstants.Username, username.Text);
            outState.PutString(BundleConstants.Password, password.Text);
            outState.PutString(BundleConstants.ErrorMessage, errorMessage.Text);
            outState.PutBoolean(BundleConstants.IsLoggingIn, isLoggingIn);
        }

        public override void OnDestroyView()
        {
            memberContactModel.Stop();
            
            base.OnDestroyView();
        }

        private async void SavePushNotification()
        {
            await memberContactModel.PushNotificationSave(PushStatus.Disabled);
        }

        private void GoToMainScreen()
        {
            isLoggingIn = false;

            var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity);
            var prefEditor = sharedPreferences.Edit();
            prefEditor.PutString(PreferenceConstants.UserId, AppData.Device.UserLoggedOnToDevice.Id);
            prefEditor.PutString(PreferenceConstants.SecurityToken, AppData.Device.SecurityToken);
            prefEditor.Commit();

            if (EnabledItems.ForceLogin || Activity is HomeActivity)
            {
                if (Activity is HomeActivity)
                {
                    (Activity as LoyaltyFragmentActivity).CheckDrawerStatus();
                    (Activity as HomeActivity).SelectItem(LoyaltyFragmentActivity.ActivityTypes.DefaultItem);
                }
            }
            else
            {
                Activity.Finish();
            }
        }

        private async void Logon()
        {
            isLoggingIn = true;

            this.errorMessage.Visibility = ViewStates.Gone;

            View view = Activity.CurrentFocus;
            if (view != null)
            {
                var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }

#if DEBUG
            if (string.IsNullOrEmpty(username.Text) && string.IsNullOrEmpty(password.Text))
            {
                username.Text = "tom";
                password.Text = "tom.1";  //tom.1
            }
#endif

            if (string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(password.Text))
            {
                if (string.IsNullOrEmpty(username.Text))
                {
                    usernameInputLayout.Error = GetString(Resource.String.LoginViewUserNameEmpty);
                    usernameInputLayout.ErrorEnabled = true;
                }

                if (string.IsNullOrEmpty(password.Text))
                {
                    passwordInputLayout.Error = GetString(Resource.String.LoginViewPasswordEmpty);
                    passwordInputLayout.ErrorEnabled = true;
                }
            }
            else
            {
                var success = await memberContactModel.Login(username.Text, password.Text, ShowError);

                if (success)
                {
                    GoToMainScreen();
                }
            }
        }

        private void ShowError(string errorMessage)
        {
            isLoggingIn = false;

            username.ClearFocus();
            password.ClearFocus();

            try
            {
                InputMethodManager inputManager = (InputMethodManager) Activity.GetSystemService(InputMethodService.InputMethodService);
                inputManager.HideSoftInputFromWindow(Activity.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
            }
            catch (Exception)
            {
                //View has now finished initializing and keyboard manipulations will not work
            }

            this.errorMessage.Visibility = ViewStates.Visible;
            this.errorMessage.Text = errorMessage;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Next)
            {
                password.RequestFocus();
                return true;
            }
            else if (actionId == ImeAction.Done)
            {
                Logon();
                return true;
            }
            return false;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == CreateAccountRequestCode)
            {
                if (resultCode == (int) Result.Ok)
                {
                    deviceService.SaveDevice(AppData.Device);

                    GoToMainScreen();
                    //Toast.MakeText(this, GetString(Resource.String.LogonViewNetworkCreateAccountSuccessful), ToastLength.Long).Show();
                }
            }
        }

        public void AfterTextChanged(IEditable s)
        {
            if (usernameInputLayout.ErrorEnabled && !string.IsNullOrEmpty(username.Text))
            {
                usernameInputLayout.ErrorEnabled = false;
            }

            if (passwordInputLayout.ErrorEnabled && !string.IsNullOrEmpty(password.Text))
            {
                passwordInputLayout.ErrorEnabled = false;
            }
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.LoginViewLoginButton:
                    Logon();
                    break;

                case Resource.Id.LoginViewForgotPassword:
                    var forgotPasswordIntent = new Intent();
                    forgotPasswordIntent.SetClass(Activity, typeof (ForgotPasswordActivity));
                    StartActivity(forgotPasswordIntent);
                    break;

                case Resource.Id.LoginViewCreateAccountButton:
                    var createIntentintent = new Intent();

                    createIntentintent.SetClass(Activity, typeof(AccountActivity));
                    StartActivityForResult(createIntentintent, CreateAccountRequestCode);
                    break;
            }
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if(viewSwitcher.CurrentView == contentView)
                    viewSwitcher.ShowNext();
            }
            else
            {
                if(viewSwitcher.CurrentView == progressView)
                    viewSwitcher.ShowPrevious();
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

#if CHANGEWS
            inflater.Inflate(Resource.Menu.ChangeWSMenu, menu);
#endif
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.ChangeWS:
                    var intent = new Intent();

                    intent.SetClass(Activity, typeof(SettingsActivity));
                    StartActivity(intent);

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}