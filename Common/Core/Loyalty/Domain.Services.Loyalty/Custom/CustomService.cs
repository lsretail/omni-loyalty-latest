using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSRetail.Omni.Domain.Services.Loyalty.Custom
{
    public class CustomService
    {
        private ICustomRepository iCustomRepository;

        public CustomService(ICustomRepository iRepo)
        {
            iCustomRepository = iRepo;
        }

        public string MyCustomFunction(string data)
        {
            return iCustomRepository.MyCustomFunction(data);
        }

        public async Task<string> MyCustomFunctionAsync(string data)
        {
            return await Task.Run(() => MyCustomFunction(data));
        }
    }
}
