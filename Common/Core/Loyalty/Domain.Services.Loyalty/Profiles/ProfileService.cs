using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Profiles
{
    public class ProfileService
    {
        private IProfileRepository iProfileRepository;

        public ProfileService(IProfileRepository iRepo)
        {
            iProfileRepository = iRepo;
        }

        public List<Profile> GetAvailableProfiles()
        {
            return iProfileRepository.GetAvailableProfiles();
        }
        public List<Profile> ProfilesGetByContactId(string contactId)
        {
            return iProfileRepository.ProfilesGetByContactId(contactId);
        }

        public async Task<List<Profile>> GetAvailableProfilesAsync()
        {
            return await Task.Run(() => GetAvailableProfiles());
        }

        public async Task<List<Profile>> ProfilesGetByContactIdAsync(string contactId)
        {
            return await Task.Run(() => ProfilesGetByContactId(contactId));
        }
    }
}
