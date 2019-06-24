using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Presentation.Screens;

namespace Presentation.Utils
{
    public static class AppData
    {

        public static List<ItemCategory> ItemCategories { get; set; }
        public static List<Store> Stores { get; set; }
        public static Device Device { get; set; }
        public static bool UserLoggedIn { get { return Device.UserLoggedOnToDevice != null; } }
        public static CLLocationCoordinate2D DefaultLocationCoordinates { get { return new CLLocationCoordinate2D(64.145280, -21.907184); } } // Intersection of Sæbraut and Kringlumýrarbraut, Reykjavík

        public static bool ShouldRefreshPublishedOffers { get; set; }
        public static bool ShouldRefreshPoints { get; set; }

        public static CardCollectionCell.CellSizes CellSize
        {
            get
            {
                return Settings.GetItemListType();
            }
            set
            {
                Settings.SetItemListType(value);
            }
        }

        public static List<Notification> Notifications
        {
            get
            {
                if (UserLoggedIn)
                    return Device.UserLoggedOnToDevice.Notifications;
                else
                    return new List<Notification>();
            }
        }

        public static List<PublishedOffer> SelectedPublishedOffers
        {
            get
            {
                if (!UserLoggedIn || Device.UserLoggedOnToDevice.PublishedOffers.Count == 0)
                    return new List<PublishedOffer>();
                else
                    return Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Selected).ToList();
            }
        }

        static AppData()
        {
            ItemCategories = new List<ItemCategory>();
            Stores = new List<Store>();

        }
    }
}
