using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts
{
    public interface ILocalContactRepository
    {
        MemberContact GetContact();
        void SaveContact(MemberContact contact);
		void ClearContact();
    }
}
