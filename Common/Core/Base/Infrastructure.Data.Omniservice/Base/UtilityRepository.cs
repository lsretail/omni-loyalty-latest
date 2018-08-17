using System;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Base
{
    public class UtilityRepository : BaseRepository
    {
        public string Ping(int timeout)
        {
            string methodName = "Ping";
            var jObject = new {  };
            return base.PostData<string>(jObject, methodName, timeout);
        }
    }
}
