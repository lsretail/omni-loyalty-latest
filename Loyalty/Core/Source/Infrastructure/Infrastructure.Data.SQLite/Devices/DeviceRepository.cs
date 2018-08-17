using System;

using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;

namespace Infrastructure.Data.SQLite.Devices
{
    public class DeviceRepository : IDeviceLocalRepository
    {
        private object locker = new object();

        public DeviceRepository()
        {
            DBHelper.OpenDBConnection();
        }

        public Device GetDevice()
        {
			OfferData n = new OfferData ();
            DeviceData deviceData = new DeviceData();
            lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                DeviceFactory factory = new DeviceFactory();
                return factory.BuildEntity(DBHelper.DBConnection.Table<DeviceData>().FirstOrDefault());
            }
        }

        public void SaveDevice(Device device)
        {
            lock (locker)
            {
                DeviceData deviceData = new DeviceData();
                deviceData.DeviceId = device.Id;
                deviceData.Make = device.Manufacturer;
                deviceData.Model = device.Model;
                deviceData.CardId = device.CardId;
                deviceData.SecurityToken = device.SecurityToken;

                if (device.UserLoggedOnToDevice == null)
                {
                    deviceData.UserLoggedOnToDevice = false;
                    deviceData.UserName = string.Empty;
                    deviceData.ContactId = string.Empty;
                }
                else
                {
                    deviceData.UserLoggedOnToDevice = true;
                    deviceData.UserName = device.UserLoggedOnToDevice.UserName;
                    deviceData.ContactId = device.UserLoggedOnToDevice.Id;
                }
                
                DeviceData deviceExist = DBHelper.DBConnection.Table<DeviceData>().FirstOrDefault();
                if (deviceExist != null)
                {
                    deviceData.ID = deviceExist.ID;
                    DBHelper.DBConnection.Update(deviceData);
                }
                else
                {
                    DBHelper.DBConnection.Insert(deviceData);
                }
            }
        }
    }
}
