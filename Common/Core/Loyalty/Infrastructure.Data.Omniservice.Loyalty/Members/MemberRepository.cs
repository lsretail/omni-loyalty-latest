
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members
{
    public class MemberRepository : BaseRepository, IMemberContactRepository
    {
        public MemberContact UserLogon(string userName, string password, string deviceId)
        {
            string methodName = "Login";
            var jObject = new { userName = userName, password = password, deviceId = deviceId };
            MemberContact contact = base.PostData<MemberContact>(jObject, methodName);

            base.SecurityToken = (string.IsNullOrWhiteSpace(contact.LoggedOnToDevice.SecurityToken) ? "" : contact.LoggedOnToDevice.SecurityToken);
            base.UniqueDeviceId = deviceId;
            ConfigStatic.UniqueDeviceId = deviceId;
            ConfigStatic.SecurityToken = base.SecurityToken;
            return contact;
        }

        public bool Logout(string userName, string deviceId)
        {
            string methodName = "Logout";
            var jObject = new { userName = userName, deviceId = deviceId };
            return base.PostData<bool>(jObject, methodName);
        }

        public bool ChangePassword(string userName, string newPassword, string oldPassword)
        {
            string methodName = "ChangePassword";
            var jObject = new { userName = userName, newPassword = newPassword, oldPassword = oldPassword };
            return base.PostData<bool>(jObject, methodName);
        }

        public bool ResetPassword(string userNameOrEmail, string resetCode, string newPassword)
        {
            //userName or email, plain resetCode and the new pwd
            string methodName = "ResetPassword";
            var jObject = new { userName = userNameOrEmail, resetCode = resetCode, newPassword = newPassword };
            return base.PostData<bool>(jObject, methodName);
        }

        public bool ResetPin(string contactId, string newPin)
        {
            string methodName = "ResetPin";
            var jObject = new { contactId = contactId, newPin = newPin };
            return base.PostData<bool>(jObject, methodName);
        }

        public MemberContact ContactGetByCardId(string cardId)
        {
            string methodName = "ContactGetByCardId";
            var jObject = new { cardId = cardId };
            return base.PostData<MemberContact>(jObject, methodName);
        }

        public MemberContact CreateMemberContact(MemberContact contact)
        {
            string methodName = "ContactCreate";
            var jObject = new { contact = contact };
            MemberContact contactCreated = base.PostData<MemberContact>(jObject, methodName);

            ConfigStatic.SecurityToken = (string.IsNullOrWhiteSpace(contactCreated.LoggedOnToDevice.SecurityToken) ? "" : contactCreated.LoggedOnToDevice.SecurityToken);
            return contactCreated;
        }

        public MemberContact UpdateMemberContact(MemberContact contact)
        {
            string methodName = "ContactUpdate";
            var jObject = new { contact = contact };
            return base.PostData<MemberContact>(jObject, methodName);
        }

        public long MemberContactGetPointBalance(string cardId)
        {
            string methodName = "CardGetPointBalance";
            var jObject = new { contactId = cardId };
            return base.PostData<long>(jObject, methodName);
        }

        public string ForgotPassword(string userNameOrEmail)
        {
            string methodName = "ForgotPassword";
            var jObject = new { userNameOrEmail = userNameOrEmail };
            return base.PostData<string>(jObject, methodName);
        }

        public bool DeviceSave(string deviceId, string deviceFriendlyName, string platform, string osVersion, string manufacturer, string model)
        {
            string methodName = "DeviceSave";
            var jObject = new { deviceId = deviceId, deviceFriendlyName = deviceFriendlyName, platform = platform, osVersion = osVersion, manufacturer = manufacturer, model = model };
            return base.PostData<bool>(jObject, methodName);
        }
    }
}
