using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Profiles
{
    public interface IProfileRepository
    {
        List<Profile> GetAvailableProfiles();
        List<Profile> ProfilesGetByCardId(string contactId);
    }
}
