using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Presentation.Utils
{
    public class BundleUtils
    {
        public static readonly string ChosenMenuBundleName = "ChosenMenuBundleName";
        public static readonly string Id = "Id";
        public static readonly string Ids = "Ids";
        public static readonly string GroupId = "GroupId";
        public static readonly string MenuId = "MenuId";
        public static readonly string NodeId = "NodeId";
        public static readonly string Type = "Type";
        public static readonly string ParentNodeId = "ParentNodeId";
        public static readonly string ChildNodeId = "ChildNodeId";
        public static readonly string ItemId = "ItemId";
        public static readonly string CouponId = "CouponId";
        public static readonly string OfferId = "OfferId";
        public static readonly string TransactionId = "TransactionId";
        public static readonly string SaleLineId = "SaleLineId";
        public static readonly string BasketItemId = "BasketItemId";
        public static readonly string FavoriteId = "FavoriteId";
        public static readonly string ImageColor = "ImageColor";
        public static readonly string ImageHeight = "ImageHeight";
        public static readonly string ImageWidth = "ImageWidth";
        public static readonly string StoreIds = "StoreIds";
        public static readonly string Title = "Title";
        public static readonly string ActivityType = "ActivityType";
        public static readonly string ImageId = "ImageId";
        public static readonly string ImageIds = "ImageIds";
        public static readonly string StartingPos = "StartingPos";
        public static readonly string RequiredOnly = "RequiredOnly";
        public static readonly string Email = "Email";
        public static readonly string Qty = "Qty";
        public static readonly string ViewType = "ViewType";
        public static readonly string AnimationImageId = "AnimationImageId";

        public enum ChosenMenu
        {
            Login = 0,
            Menu = 1,
            OffersAndCoupons = 2,
            Locations = 3,
            Home = 4,
            Transactions = 5,
            Logout = 6,
            DefaultMenu = 7,
            Favorites = 8,
        }
    }
}