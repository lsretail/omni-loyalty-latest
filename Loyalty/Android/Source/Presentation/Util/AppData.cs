using System.Collections.Generic;

using Infrastructure.Data.SQLite.Devices;
using Infrastructure.Data.SQLite.MemberContacts;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;

namespace Presentation.Util
{
    public enum SocialMediaConnection
    {
        None,
        Google,
        Facebook,
    }

    public enum ShippingMedhod
    {
        ClickCollect = 0,
        HomeDelivery = 1
    }

    class AppData
    {
        private static List<Store> stores = new List<Store>();
        
        private AppData()
        {
        }

        public static Device Device
        {
            get
            {
                if (device == null)
                {
                    var service = new DeviceService(new DeviceRepository());

                    device = service.GetDevice();

                    if (!(device is UnknownDevice))
                    {
                        if (device.UserLoggedOnToDevice != null)
                        {
                            var contact = new MemberContactLocalService(new MemberContactRepository()).GetLocalMemberContact();

                            if (contact != null)
                            {
                                contact.LoggedOnToDevice = device;

                                device.CardId = contact.LoggedOnToDevice.CardId;
                                device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;
                                device.UserLoggedOnToDevice = contact;
                            }
                        }
                    }
                }

                return device;
            }
            set { device = value; }
        }

        public static List<ItemCategory> ItemCategories
        {
            get { return itemCategories; }
            set { itemCategories = value; }
        }

        public static List<Store> Stores
        {
            get { return stores; }
            set { stores = value; }
        }

        public static OneList Basket
        {
            get
            {
                if (Device == null || Device.UserLoggedOnToDevice == null)
                    return null;

                return Device.UserLoggedOnToDevice.Basket;
            }
            set
            {
                if (Device == null)
                    return;

                Device.UserLoggedOnToDevice.Basket = value;
            }
        }

        public static List<Advertisement> Advertisements { get; set; }
        public static bool IsDualScreen = false;

        private static Device device;
        private static List<ItemCategory> itemCategories;

        public static SocialMediaConnection SocialMediaConnection = SocialMediaConnection.None;
        public static List<ShippingMedhod> ShippingMethods = new List<ShippingMedhod>(){ShippingMedhod.ClickCollect, ShippingMedhod.HomeDelivery}; 
    }
}