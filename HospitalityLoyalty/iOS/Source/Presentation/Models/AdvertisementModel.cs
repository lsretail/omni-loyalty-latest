using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Presentation.Utils;

namespace Presentation.Models
{
	public class AdvertisementModel : BaseModel
	{
		private SharedService advertisementService;
		private SharedRepository advertismentRepository;

		public AdvertisementModel()
		{
			advertismentRepository = new SharedRepository();
			this.advertisementService = new SharedService(advertismentRepository);
		}

		public async Task<List<Advertisement>> AdvertisementsGetById(string id, string contactId)
		{

			try
			{
				List<Advertisement> adList = await advertisementService.AdvertisementsGetByIdAsync(id, contactId);
				return adList;

			}
			catch (Exception exception)
			{
				HandleException(exception, "AdvertisementModel.AdvertisementsGetById()", false);
				return null;
			}
			
		}

	}
}