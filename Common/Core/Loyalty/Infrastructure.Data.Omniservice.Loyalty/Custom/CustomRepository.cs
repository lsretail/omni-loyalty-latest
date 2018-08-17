using System;
using System.Collections.Generic;

using LSRetail.Omni.Domain.Services.Loyalty.Custom;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Custom
{
    public class CustomRepository : BaseRepository, ICustomRepository
    {
        public string MyCustomFunction(string data)
        {
            string methodName = "MyCustomFunction";
            var jObject = new { data = data };
            return base.PostData<string>(jObject, methodName);
        }
    }
}
