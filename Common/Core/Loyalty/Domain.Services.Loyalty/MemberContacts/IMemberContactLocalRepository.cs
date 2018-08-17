using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.Services.Loyalty.MemberContacts
{
    public interface IMemberContactLocalRepository
    {
        MemberContact GetLocalMemberContact();
        void SaveMemberContact(MemberContact memberContact);
    }
}
