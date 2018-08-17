using System.Collections.Generic;

using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Profiles;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members
{
    public class ProfileRepository : BaseRepository, IProfileRepository
    {
        public List<Profile> GetAvailableProfiles()
        {
            string methodName = "ProfilesGetAll";
            var jObject = "";
            return base.PostData<List<Profile>>(jObject, methodName);
        }

        public List<Profile> ProfilesGetByContactId(string contactId)
        {
            string methodName = "ProfilesGetByContactId";
            var jObject = new { contactId = contactId };
            return base.PostData<List<Profile>>(jObject, methodName);
        }
    }
}
