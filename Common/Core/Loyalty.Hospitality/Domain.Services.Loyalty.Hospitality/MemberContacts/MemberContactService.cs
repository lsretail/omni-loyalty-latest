using System;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts
{
    public class MemberContactService : Loyalty.MemberContacts.MemberContactService
    {
        public MemberContactService() : base()
        {
        }

        public MemberContact GetContact(ILocalContactRepository repository)
        {
            return repository.GetContact();
        }

        public MemberContact SyncContact(ILocalContactRepository repository, MemberContact contact)
        {
            repository.SaveContact(contact);
            return contact;
        }

        public void ClearContact(ILocalContactRepository repository)
        {
            repository.ClearContact();
        }

        #region Async

        public async Task<MemberContact> GetContactAsync(ILocalContactRepository repository)
        {
            return await Task.Run(() => GetContact(repository));
        }

        public async Task<MemberContact> SyncContactAsync(ILocalContactRepository repository, MemberContact contact)
        {
            return await Task.Run(() => SyncContact(repository, contact));
        }

        public async Task ClearContactAsync(ILocalContactRepository repository)
        {
            await Task.Run(() => ClearContact(repository));
        }

        #endregion
    }
}
