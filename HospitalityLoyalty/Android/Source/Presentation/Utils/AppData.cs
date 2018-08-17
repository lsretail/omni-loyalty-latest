using System;
using System.Collections.Generic;

using Infrastructure.Data.SQLite2.Baskets;
using Infrastructure.Data.SQLite2.MemberContacts;
using Infrastructure.Data.SQLite2.Menus;
using Infrastructure.Data.SQLite2.Transactions;
using LSRetail.Omni.Domain.DataModel.Base.Favorites;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.DataModel.Loyalty.Hospitality.Transactions;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Transactions;

namespace Presentation.Utils
{
    public class AppData
    {
        public enum Status
        {
            None = 0,
            Loading = 1,
            Failed = 2
        }

        private AppData()
        {
        }

        private static Basket basket;
        private static List<Transaction> transactions;
        private static MemberContact contact;
        private static MobileMenu mobileMenu;

        public static string SecurityToken {
            get
            {
                if (AppData.Contact == null)
                {
                    return string.Empty;
                }
                return Contact.LoggedOnToDevice.SecurityToken;
            }
        }

        //Data
        public static List<Store> Stores { get; set; }
        public static List<Advertisement> Advertisements { get; set; }
        public static List<IFavorite> Favorites { get; set; }

        //Settings
        public static bool HasBasket = true;
        public static bool HasLogin = true;
        public static bool CanPay = false;
        public static bool HasTransactionHistory = true;
        public static bool HasHome = true;
        public static bool HasOffersAndCoupons = true;
        public static bool HasPoints = true;

        public const int MaxItems = 99;

        //Status
        private static Status contactUpdateStatus = Status.None;
        private static Status menuUpdateStatus = Status.None;

        public static Status ContactUpdateStatus
        {
            get { return contactUpdateStatus; }
            set { contactUpdateStatus = value; }
        }

        public static Status MenuUpdateStatus
        {
            get { return menuUpdateStatus; }
            set { menuUpdateStatus = value; }
        }

        public static bool MenuNeedsUpdate
        {
            get { return menuUpdateStatus != Status.None; }
        }

        //Other
        public static Basket Basket
        {
            get
            {
                if (basket == null)
                {
                    try
                    {
                        basket = new LocalBasketService().GetBasket(new BasketRepository(), MobileMenu);

                        if(basket == null)
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        basket = new Basket();
                    }
                }

                return basket;
            }
            set { basket = value; }
        }

        public static List<Transaction> Transactions
        {
            get
            {
                if(transactions == null)
                {
                    try
                    {
                        transactions = new LocalTransactionService(new TransactionRepository()).GetTransactions();

                        if (transactions == null)
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        transactions = new List<Transaction>();
                    }
                }

                return transactions;
            }
            set { transactions = value; }
        }

        public static MemberContact Contact
        {
            get
            {
                if (contact == null)
                {
                    try
                    {
                        contact = new MemberContactService().GetContact(new ContactRepository());
                    }
                    catch (Exception)
                    {
                    }
                }

                return contact;
            }
            set { contact = value; }
        }

        public static MobileMenu MobileMenu
        {
            get
            {
                if (mobileMenu == null)
                {
                    try
                    {
                        mobileMenu = new LocalMenuService(new MenuRepository()).GetMobileMenu();
                    }
                    catch (Exception)
                    {
                    }
                }

                return mobileMenu;
            }
            set { mobileMenu = value; }
        }

        public static string FormatCurrency(decimal amount)
        {
            if (MobileMenu != null)
            {
                if (MobileMenu.Currency != null)
                {
                    return MobileMenu.Currency.FormatDecimal(amount);
                }
            }

            return amount.ToString();
        }
    }
}