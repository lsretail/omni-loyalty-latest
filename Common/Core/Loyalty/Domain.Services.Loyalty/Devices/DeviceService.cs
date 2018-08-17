using System;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Devices
{
    public class DeviceService
    {
        private IDeviceLocalRepository iDeviceLocalRepository;

        public DeviceService(IDeviceLocalRepository iRepo)
        {
            iDeviceLocalRepository = iRepo;
        }

        public Device GetDevice()
        {
            return iDeviceLocalRepository.GetDevice();
        }

        public void SaveDevice(Device device)
        {
            iDeviceLocalRepository.SaveDevice(device);
        }

        public async Task<Device> GetDeviceAsync()
        {
            return await Task.Run(() => GetDevice());
        }

        public async Task SaveDeviceAsync(Device device)
        {
            await Task.Run(() => SaveDevice(device));
        }
    }
}
