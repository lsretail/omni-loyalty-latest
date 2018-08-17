using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Devices
{
    public interface IDeviceLocalRepository
    {
        Device GetDevice();
        void SaveDevice(Device device);
    }
}
