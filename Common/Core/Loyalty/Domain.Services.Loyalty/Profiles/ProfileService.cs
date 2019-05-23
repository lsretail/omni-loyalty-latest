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

        public List<Profile> ProfilesGetByCardId(string cardId)
        {
            return iProfileRepository.ProfilesGetByCardId(cardId);
        }

        public async Task<List<Profile>> GetAvailableProfilesAsync()
        {
            return await Task.Run(() => GetAvailableProfiles());
        }

        public async Task<List<Profile>> ProfilesGetByCardIdAsync(string cardId)
        {
            return await Task.Run(() => ProfilesGetByCardId(cardId));
        }
    }
}
