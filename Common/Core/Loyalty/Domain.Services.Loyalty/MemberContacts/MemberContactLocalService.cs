using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.Services.Loyalty.MemberContacts
{
    public class MemberContactLocalService
    {
        private IMemberContactLocalRepository repository;

        public MemberContactLocalService(IMemberContactLocalRepository iRepo)
        {
            repository = iRepo;
        }

        public MemberContact GetLocalMemberContact()
        {
            return repository.GetLocalMemberContact();
        }

        public void SaveMemberContact(MemberContact memberContact)
        {
            repository.SaveMemberContact(memberContact);
        }
    }
}
