using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Profiles;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;

namespace Presentation.Models
{
    public class ProfileModel : BaseModel
	{
		private IProfileRepository profileRepository;
		private ProfileService profileService;

		public ProfileModel ()
		{
			profileRepository = new ProfileRepository ();
			profileService = new ProfileService (profileRepository);
		}

		public async Task<List<Profile>> GetAllProfiles()
		{
            try
            {
                List<Profile> profiles = await this.profileService.GetAvailableProfilesAsync();
                return profiles;
            }
            catch (Exception ex)
            {
                HandleException(ex, "ProfileModel.GetAllProfiles()", false);
                return null;

            }
		}


	}
}

