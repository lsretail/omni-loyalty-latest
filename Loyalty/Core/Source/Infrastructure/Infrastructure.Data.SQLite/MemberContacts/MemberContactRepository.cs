using System;

using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;
using Infrastructure.Data.SQLite.Notifications;
using Infrastructure.Data.SQLite.Offers;
using Infrastructure.Data.SQLite.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace Infrastructure.Data.SQLite.MemberContacts
{
    public class MemberContactRepository : IMemberContactLocalRepository
    {
        private object locker = new object();

        public MemberContactRepository()
        {
            DBHelper.OpenDBConnection();
        }

        public MemberContact GetLocalMemberContact()
        {
            lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                var factory = new MemberContactFactory();

                var contactData = DBHelper.DBConnection.Table<MemberContactData>().FirstOrDefault();

                MemberContact contact = null;

                if (contactData != null)
                {
                    var notificationRepository = new NotificationRepository();
                    var offerRepository = new OfferRepository();                    
                    var transactionRepository = new TransactionRepository();

                    contact = factory.BuildEntity(contactData);

                    contact.Notifications = notificationRepository.GetLocalNotifications();
                    contact.PublishedOffers = offerRepository.GetLocalPublishedOffers();                                           
                    contact.Transactions = transactionRepository.GetLocalTransactions();
                }

                return contact;
            }
        }

        public void SaveMemberContact(MemberContact memberContact)
        {
            lock (locker)
            {
                var factory = new MemberContactFactory();

                var contact = factory.BuildEntity(memberContact);

                DBHelper.DBConnection.DeleteAll<MemberContactData>();
                DBHelper.DBConnection.Insert(contact);
                
                var notificationRepository = new NotificationRepository();

                notificationRepository.SaveNotifications(memberContact.Notifications);

                var offerRepository = new OfferRepository();

                offerRepository.SavePublishedOffers(memberContact.PublishedOffers);
				                
                var transactionRepository = new TransactionRepository();

                transactionRepository.SaveTransactions(memberContact.Transactions);
            }
        }
    }
}
