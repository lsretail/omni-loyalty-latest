using System;
using System.Threading.Tasks;
using Presentation.Models;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Utils;
namespace Presentation
{
    public class DebugModel : BaseModel
	{
        
 
		public DebugModel ()
		{
           
           
		}

        public string Ping ()
		{
            var utils = new LSRetail.Omni.Infrastructure.Data.Omniservice.Utils.Utils();
            try
            {
                string message = utils.PingServer();
                return message;

            }
            catch (Exception exception)
            {
                base.HandleException(exception, "DebugModel.Ping()", false);
                throw;
				
            }

	
		}
	}
}

