using System;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace Infrastructure.Data.SQLite.Devices
{
    internal class DeviceFactory
    {
        public Device BuildEntity(DeviceData deviceData)
        {
            if (deviceData == null)
                return new UnknownDevice();

            Device entity = new Device(deviceData.DeviceId);
            entity.Manufacturer = deviceData.Make;
            entity.Model = deviceData.Model;
            entity.SecurityToken = deviceData.SecurityToken;
            entity.CardId = deviceData.CardId;
           
            if (deviceData.UserLoggedOnToDevice == true)
            {
                entity.UserLoggedOnToDevice = new MemberContact(deviceData.ContactId);
                entity.UserLoggedOnToDevice.UserName = deviceData.UserName;
            }
            else
            {
                entity.UserLoggedOnToDevice = null;
            }
            
            return entity;
        }
    }
}
