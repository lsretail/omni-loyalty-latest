using System.Threading.Tasks;

using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.Services.Loyalty.MemberContacts
{
    public class MemberContactService
    {
        public MemberContact MemberContactLogon(IMemberContactRepository memberContactRepository, string userName, string password, string deviceId)
        {
            return memberContactRepository.UserLogon(userName, password, deviceId);
        }

        public MemberContact MemberContactById(IMemberContactRepository memberContactRepository, string contactId)
        {
            return memberContactRepository.GetById(contactId);
        }

		public MemberContact CreateMemberContact(IMemberContactRepository memberContactRepository, MemberContact memberContact)
        {
            return memberContactRepository.CreateMemberContact(memberContact);
        }

		public MemberContact UpdateMemberContact(IMemberContactRepository memberContactRepository, MemberContact memberContact)
        {
            return memberContactRepository.UpdateMemberContact(memberContact);
        }

		public long MemberContactGetPointBalance(IMemberContactRepository memberContactRepository, string contactId)
		{
			return memberContactRepository.MemberContactGetPointBalance(contactId);
		}

        public bool ChangePassword(IMemberContactRepository memberContactRepository, string userName, string newPassword, string oldPassword)
        {
            return memberContactRepository.ChangePassword(userName, newPassword, oldPassword);
        }

        public bool ResetPin(IMemberContactRepository memberContactRepository, string contactId, string newPin)
        {
            return memberContactRepository.ResetPin(contactId, newPin);
        }

        public void Logout(IMemberContactRepository memberContactRepository, string userName, string deviceId)
        {
            memberContactRepository.Logout(userName, deviceId);
        }

        public bool DeviceSave(IMemberContactRepository memberContactRepository, string deviceId, string deviceFriendlyName, string platform, string osVersion, string manufacturer, string model)
        {
            return memberContactRepository.DeviceSave(deviceId, deviceFriendlyName, platform, osVersion, manufacturer, model);
        }

		public bool ForgotPasswordForDevice(IMemberContactRepository memberContactRepository, string userName)
		{
			return memberContactRepository.ForgotPasswordForDevice(userName);
		}

		public bool ResetPassword(IMemberContactRepository memberContactRepository, string userName, string resetCode, string newPassword)
		{
			return memberContactRepository.ResetPassword(userName, resetCode, newPassword);
		}

        public async Task<MemberContact> CreateMemberContactAsync(IMemberContactRepository memberContactRepository, MemberContact memberContact)
        {
            return await Task.Run(() => CreateMemberContact(memberContactRepository, memberContact));
        }

        public async Task<MemberContact> MemberContactLogonAsync(IMemberContactRepository memberContactRepository, string userName, string password, string deviceId)
        {
            return await Task.Run(() => MemberContactLogon(memberContactRepository, userName, password, deviceId));
        }

        public async Task<MemberContact> MemberContactByIdAsync(IMemberContactRepository memberContactRepository, string contactId)
        {
            return await Task.Run(() => MemberContactById(memberContactRepository, contactId));
        }

        public async Task<MemberContact> UpdateMemberContactAsync(IMemberContactRepository memberContactRepository, MemberContact memberContact)
        {
            return await Task.Run( () => UpdateMemberContact(memberContactRepository, memberContact));
        }

        public async Task<long> MemberContactGetPointBalanceAsync(IMemberContactRepository memberContactRepository, string contactId) 
        {
            return await Task.Run( () => MemberContactGetPointBalance(memberContactRepository, contactId));
        }

        public async Task<bool> ChangePasswordAsync(IMemberContactRepository memberContactRepository, string userName, string newPassword, string oldPassword)
        {
            return await Task.Run( () => ChangePassword(memberContactRepository, userName, newPassword, oldPassword));
        }

        public async Task LogoutAsync(IMemberContactRepository memberContactRepository, string userName, string deviceId)
        {
            await Task.Run(() => Logout(memberContactRepository, userName, deviceId));
        }

        public async Task<bool> DeviceSaveAsync(IMemberContactRepository memberContactRepository, string deviceId, string deviceFriendlyName, string platform, string osVersion, string manufacturer, string model)
        {
            return await Task.Run(() => DeviceSave(memberContactRepository, deviceId, deviceFriendlyName, platform, osVersion, manufacturer, model));
        }
        
        public async Task<bool> ForgotPasswordForDeviceAsync(IMemberContactRepository memberContactRepository, string userName)
        {
            return await Task.Run(() => ForgotPasswordForDevice(memberContactRepository, userName));
        }

        public async Task<bool> ResetPasswordAsync(IMemberContactRepository memberContactRepository, string userName, string resetCode, string newPassword)
        {
            return await Task.Run(() => ResetPassword(memberContactRepository, userName, resetCode, newPassword));
        }
    }
}
