using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Widget;
using Infrastructure.Data.SQLite.Devices;
using Presentation.Dialogs;
using Presentation.Util;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;

namespace Presentation.Models
{
    class MemberContactModel : BaseModel
    {
        private MemberContactService service;
        private DeviceService deviceService;
        private MemberRepository repository;
        private MemberContactLocalService memberContactLocalService;
        private SharedService sharedService;

        private CustomProgressDialog ProgressDialog;

        public MemberContactModel(Context context, IRefreshableActivity refreshableActivity = null) : base(context, refreshableActivity)
        {
            memberContactLocalService = new MemberContactLocalService(new Infrastructure.Data.SQLite.MemberContacts.MemberContactRepository());

            if (Context != null)
                ProgressDialog = new CustomProgressDialog(context);
        }

        private async Task RegisterDevice()
        {
            BeginWsCall();

            ProgressDialog.Title = Context.GetString(Resource.String.MemberContactModelRegisterDevice);
            ProgressDialog.Message = Context.GetString(Resource.String.MemberContactModelRegisteringDevice);
            ProgressDialog.Show();

            var device = AppData.Device;
            if (string.IsNullOrEmpty(device.Manufacturer) || string.IsNullOrEmpty(device.Platform) || string.IsNullOrEmpty(device.OsVersion) || string.IsNullOrEmpty(device.Model))
                Utils.FillDeviceInfo(device);

            try
            {
                await service.DeviceSaveAsync(repository, device.Id, device.DeviceFriendlyName, device.Platform, device.OsVersion, device.Manufacturer, device.Model);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ProgressDialog.Dismiss();
        }

        public void Logout()
        {
            LogoutOnServer(AppData.Device);

            var deviceService = new DeviceService(new DeviceRepository());

            AppData.Basket.Clear();
            AppData.Device.UserLoggedOnToDevice = null;
            AppData.Device.SecurityToken = "";
            AppData.ItemCategories = null;
            AppData.Stores = null;
            SharedService.ClearImageCache();
            AppData.Advertisements = null;

            deviceService.SaveDevice(AppData.Device);
        }

        private async void LogoutOnServer(Device device)
        {
            BeginWsCall();

            try
            {
                await service.LogoutAsync(repository, device.UserLoggedOnToDevice.UserName, device.Id);
            }
            catch (Exception)
            {
                //supress
            }
        }

        public async Task<bool> Login(string username, string password, Action<string> onError)
        {
            bool success = false;

            BeginWsCall();

            ProgressDialog.Title = Context.GetString(Resource.String.MemberContactModelLogin);
            ProgressDialog.Message = string.Format(Context.GetString(Resource.String.MemberContactModelLoggingInUser), username);
            ProgressDialog.Show();

            try
            {
                var contact = await service.MemberContactLogonAsync(repository, username, password, AppData.Device.Id);

                AppData.Device.UserLoggedOnToDevice = contact;
                AppData.Basket = contact.Basket;
                AppData.Device.UserLoggedOnToDevice.Cards = contact.Cards;
                AppData.Device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;
                AppData.Device.CardId = contact.Cards[0].Id;

                SaveLocalMemberContact(contact);

                deviceService.SaveDevice(AppData.Device);

                await PushNotificationSave();

                if (string.IsNullOrEmpty(contact.LoggedOnToDevice.Manufacturer) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Platform) || string.IsNullOrEmpty(contact.LoggedOnToDevice.OsVersion) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Model))
                {
                    await RegisterDevice();
                }

                success = true;
            }
            catch (Exception ex)
            {
                var errorMessage = await HandleUIExceptionAsync(ex, showToastOnNetworkError: false, displayAlert: false);

                onError(errorMessage);
            }

            ProgressDialog.Dismiss();

            return success;
        }

        public async Task<string> ForgotPasswordForDeviceAsync(string userName)
        {
            string success = string.Empty;

            ShowIndicator(true);

            BeginWsCall();

            try
            {
                success = await service.ForgotPasswordAsync(repository, userName);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return success;
        }

        public async Task<bool> ResetPassword(string userName, string resetCode, string newPassword)
        {
            bool success = false;
            ShowIndicator(true);

            BeginWsCall();

            try
            {
                success = await service.ResetPasswordAsync(repository, userName, resetCode, newPassword);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return success;
        }

        public async Task UserGetByCardId(string cardId, IRefreshableActivity refreshableActivity, int retryCounter = 3)
        {
            ShowIndicator(true);

            BeginWsCall();

            try
            {
                MemberContact contact = await service.MemberContactByCardIdAsync(repository, cardId);
                contact.Basket.CalculateBasket();

                AppData.Device.UserLoggedOnToDevice = contact;
                AppData.Basket = contact.Basket;
                AppData.Device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;

                SendBroadcast(Utils.BroadcastUtils.DomainModelUpdated);

                deviceService.SaveDevice(AppData.Device);

                SaveLocalMemberContact(contact);

                retryCounter = 0;
            }
            catch (Exception ex)
            {
                if (retryCounter == 0)
                {
                    await HandleUIExceptionAsync(ex, showAsToast: true);
                }
                else
                {
                    await UserGetByCardId(cardId, refreshableActivity, --retryCounter);
                }
            }

            ShowIndicator(false);
        }

        public async Task<bool> CreateMemberContact(MemberContact newContact)
        {
            BeginWsCall();

            ProgressDialog.Title = Context.GetString(Resource.String.AccountViewCreateAccount);
            ProgressDialog.Message = Context.GetString(Resource.String.AccountViewCreatingNewAccount);
            ProgressDialog.Show();

            try
            {
                var contact = await service.CreateMemberContactAsync(repository, newContact);

                contact.LoggedOnToDevice.UserLoggedOnToDevice = contact;
                AppData.Device = contact.LoggedOnToDevice;

                SaveLocalMemberContact(contact);

                await PushNotificationSave();
            }
            finally
            {
                ProgressDialog.Dismiss();
            }
            return true;
        }

        public async Task<bool> UpdateMemberContact(MemberContact updateContact)
        {
            BeginWsCall();

            ProgressDialog.Title = Context.GetString(Resource.String.AccountViewUpdateAccount);
            ProgressDialog.Message = Context.GetString(Resource.String.AccountViewUpdatingExistingAccount);
            ProgressDialog.Show();

            try
            {
                var contact = await service.UpdateMemberContactAsync(repository, updateContact);

                contact.LoggedOnToDevice.UserLoggedOnToDevice = contact;

                AppData.Device = contact.LoggedOnToDevice;

                SaveLocalMemberContact(contact);

                SendBroadcast(Utils.BroadcastUtils.DomainModelUpdated);

                Toast.MakeText(Context, Resource.String.AccountViewUpdateSuccess, ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
            finally
            {
                ProgressDialog.Dismiss();
            }

            return true;
        }

        public async Task MemberContactGetPointBalance(string cardId)
        {
            BeginWsCall();

            try
            {
                var points = await service.MemberContactGetPointBalanceAsync(repository, cardId);

                AppData.Device.UserLoggedOnToDevice.Account.PointBalance = points;

                SendBroadcast(Utils.BroadcastUtils.PointsUpdated);

                SaveLocalMemberContact(AppData.Device.UserLoggedOnToDevice);
            }
            catch (Exception)
            {
                //supress
            }
        }

        public async Task<bool> ChangePassword(string username, string newPass, string oldPass)
        {
            bool success = false;

            BeginWsCall();

            ProgressDialog.Title = Context.GetString(Resource.String.ChangePasswordChangePassword);
            ProgressDialog.Message = Context.GetString(Resource.String.ChangePasswordChangingPassword);
            ProgressDialog.Show();

            try
            {
                success = await service.ChangePasswordAsync(repository, username, newPass, oldPass);
                if (success)
                {
                    ShowToast(Resource.String.ChangePasswordChangePasswordSuccess);
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ProgressDialog.Dismiss();

            return success;
        }

        public void PushNotificationSave(string gcmId, PushStatus status = PushStatus.Enabled)
        {
            //called from background context

            BeginWsCall();

            var looper = Context.MainLooper;
            var handler = new Handler(looper);
            handler.Post(
                async () =>
                {
                    var success = await sharedService.PushNotificationSaveAsync(
                        new PushNotificationRequest()
                        {
                            Application = PushApplication.Loyalty,
                            DeviceId = new DeviceUuidFactory(Context).getDeviceUuid(),
                            Id = gcmId,
                            Platform = PushPlatform.Android,
                            Status = status
                        });

                    if (success)
                    {
                        if (status == PushStatus.Enabled)
                        {
                            var versionCode = Context.PackageManager.GetPackageInfo(Context.PackageName, 0).VersionCode;

                            Utils.PreferenceUtils.SetInt(Context, Utils.PreferenceUtils.VersionCode, versionCode);
                            Utils.PreferenceUtils.SetString(Context, Utils.PreferenceUtils.FcmRegistrationId, gcmId);
                        }
                    }
                });
        }

        public async Task PushNotificationSave(PushStatus status = PushStatus.Enabled)
        {
            var token = Utils.PreferenceUtils.GetString(Context, Utils.PreferenceUtils.FcmRegistrationId);
            if (string.IsNullOrEmpty(token))
                return;

            BeginWsCall();

            try
            {
                await sharedService.PushNotificationSaveAsync(
                    new PushNotificationRequest()
                    {
                        Application = PushApplication.Loyalty,
                        DeviceId = new DeviceUuidFactory(Context).getDeviceUuid(),
                        Id = token,
                        Platform = PushPlatform.Android,
                        Status = status
                    });
            }
            catch (Exception)
            {
                //supress
            }
        }

        public void SaveLocalMemberContact(MemberContact contact)
        {
            memberContactLocalService.SaveMemberContact(contact);
        }

        protected override void CreateService()
        {
            service = new MemberContactService();
            repository = new MemberRepository();
            deviceService = new DeviceService(new DeviceRepository());
            sharedService = new SharedService(new SharedRepository());
        }
    }
}
