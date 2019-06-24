using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;

using Presentation.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Profiles;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;

namespace Presentation.Models
{
    public class ProfileModel : BaseModel
    {
        private readonly IRefreshableActivity refreshableActivity;
        private ProfileService profileService;

        public ProfileModel(Context context, IRefreshableActivity refreshableActivity) : base(context)
        {
            this.refreshableActivity = refreshableActivity;
        }

        public async Task<List<Profile>> ProfilesGetAll()
        {
            List<Profile> profiles = null;

            refreshableActivity.ShowIndicator(true);

            BeginWsCall();

            try
            {
                profiles = await profileService.GetAvailableProfilesAsync();
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            refreshableActivity.ShowIndicator(false);

            return profiles;
        }

        public async Task<List<Profile>> ProfilesGetByCardId(string cardId)
        {
            List<Profile> allProfiles = null;

            refreshableActivity.ShowIndicator(true);

            BeginWsCall();

            try
            {
                var profiles = await profileService.ProfilesGetByCardIdAsync(cardId);

                allProfiles = await profileService.GetAvailableProfilesAsync();
                        
                foreach (var memberProfile in profiles)
                {
                    foreach (var profile in allProfiles)
                    {
                        if (memberProfile.Id == profile.Id)
                        {
                            profile.ContactValue = memberProfile.ContactValue;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            return allProfiles;
        }

        protected override void CreateService()
        {
            profileService = new ProfileService(new ProfileRepository());
        }
    }
}