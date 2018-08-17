using System;
using System.Threading.Tasks;
using Presentation.Utils;
using Infrastructure.Data.SQLite2.Devices;

using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.Services.Base.Loyalty;

namespace Presentation.Models
{
    public class ContactModel : BaseModel
    {
        private MemberContactService memberContactService;
        private MemberRepository memberRepository;
        private DeviceService deviceService;
        private ISharedRepository sharedRepository;
        private SharedService sharedSevice;

        public ContactModel()
        {
            this.memberContactService = new MemberContactService();
            this.memberRepository = new MemberRepository();
            this.deviceService = new DeviceService(new DeviceRepository());
            this.sharedRepository = new SharedRepository();
            this.sharedSevice = new SharedService(sharedRepository);
        }

        public async void GetMemberContact(string contactId)
        {
            try
            {
                MemberContact memberContact = await this.memberContactService.MemberContactByIdAsync(memberRepository, contactId);
                if (memberContact != null)
                {
                    AppData.Device = memberContact.LoggedOnToDevice;
                    AppData.Device.UserLoggedOnToDevice = memberContact;
                    this.deviceService.SaveDevice(AppData.Device);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.GetMemberContact()", false);
            }
        }

        public async Task<bool> MemberContactLogon(string userId, string password, string deviceId)
        {
            try
            {
                MemberContact memberContact = await this.memberContactService.MemberContactLogonAsync(memberRepository, userId, password, deviceId);
                if (memberContact != null)
                {
                    AppData.Device = memberContact.LoggedOnToDevice;
                    AppData.Device.UserLoggedOnToDevice = memberContact;
                    if (memberContact.Card != null)
                        AppData.Device.CardId = memberContact.Card.Id;
                    this.deviceService.SaveDevice(AppData.Device);
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.MemberContactLogon()", true);
                return false;
            }
        }

        public async Task<bool> CreateMemberContact(MemberContact memberContact)
        {
            try
            {
                MemberContact memberContactReturned = await this.memberContactService.CreateMemberContactAsync(memberRepository, memberContact);
                if (memberContactReturned != null)
                {
                    AppData.Device = memberContactReturned.LoggedOnToDevice;
                    AppData.Device.UserLoggedOnToDevice = memberContactReturned;
                    this.deviceService.SaveDevice(AppData.Device);
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.CreateMemberContact()", true);
                return false;
            }
        }

        public async Task<bool> UpdateMemberContact(MemberContact memberContact)
        {
            try
            {
                MemberContact memberContactReturned = await this.memberContactService.UpdateMemberContactAsync(memberRepository, memberContact);
                if (memberContactReturned != null)
                {
                    AppData.Device = memberContactReturned.LoggedOnToDevice;
                    AppData.Device.UserLoggedOnToDevice = memberContactReturned;
                    this.deviceService.SaveDevice(AppData.Device);
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.UpdateMemberContact()", true);
                return false;
            }
        }

        public async Task<bool> MemberContactLogout(string userName, string deviceId)
        {
            try
            {
                await this.memberContactService.LogoutAsync(memberRepository, userName, deviceId);

                AppData.Device.UserLoggedOnToDevice = null;
                AppData.Device.SecurityToken = "";
                AppData.Device.CardId = "";
                this.deviceService.SaveDevice(AppData.Device);
            }
            catch (Exception ex)
            {
                //If Error logging out, we ignore it
                HandleException(ex, "MemberContactModel.MemberContactLogout()", false);

                AppData.Device.UserLoggedOnToDevice = null;
                AppData.Device.SecurityToken = "";
                AppData.Device.CardId = "";
                this.deviceService.SaveDevice(AppData.Device);
            }
            return true;
        }

        public async Task<bool> MemberContactUpdatePointBalance()
        {
            if (AppData.Device.UserLoggedOnToDevice == null)
                return false;

            try
            {
                long points = await this.memberContactService.MemberContactGetPointBalanceAsync(memberRepository, AppData.Device.UserLoggedOnToDevice.Id);
                if (!(points < 0))
                {
                    System.Diagnostics.Debug.WriteLine("Contact point balance updated: " + points);
                    AppData.Device.UserLoggedOnToDevice.Account.PointBalance = points;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "ContactModel.ContactGetPointBalance()", false);
                return false;
            }
        }

        public async Task<bool> ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                bool success = await this.memberContactService.ChangePasswordAsync(memberRepository, userName, newPassword, oldPassword);
                if (success)
                {
                    return true;
                }
                else
                {
                    throw new Exception(LocalizationUtilities.LocalizedString("Account_ErrorChangePassword", "Error changing password"));
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.ChangePassword()", true);
                return false;
            }
        }

        public async Task<OmniEnvironment> GetEnvironment()
        {
            try
            {
                OmniEnvironment environment = await this.sharedSevice.GetEnvironmentAsync();
                if (environment != null)
                {
                    return environment;
                }
                return null;
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.GetEnvironment()", false);
                return null;
            }
        }

        public async Task<bool> ForgotPasswordForDevice(string username)
        {
            try
            {
                return await this.memberContactService.ForgotPasswordForDeviceAsync(memberRepository, username);
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.ForgotPasswordForDevice()", true);
                return false;
            }
        }

        public async Task<bool> ResetPassword(string userName, string resetCode, string newPassword)
        {
            try
            {
                return await this.memberContactService.ResetPasswordAsync(memberRepository, userName, resetCode, newPassword);
            }
            catch (Exception ex)
            {
                HandleException(ex, "MemberContactModel.ResetPasswordAsync()", true);
                return false;
            }
        }

        public async Task RegisterForPushNotificationsInBackendServer(string notificationRegistrationId)
        {
            PushNotificationRequest req = new PushNotificationRequest()
            {
                Id = notificationRegistrationId,
                DeviceId = Util.PhoneId,
                Application = PushApplication.Loyalty,
                Platform = PushPlatform.Apple,
                Status = PushStatus.Enabled
            };

            try
            {
                bool success = await this.sharedSevice.PushNotificationSaveAsync(req);
                if (success)
                {
                    System.Diagnostics.Debug.WriteLine("Registered for push notifications in backend server");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to register for push notifications in backend server");
                HandleException(ex, "ContactModel.RegisterForPushNotifications()", false);
            }
        }
    }
}

