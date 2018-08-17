using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;
using Presentation.Activities.Login;

using Presentation.Utils;
using MemberContactService = LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts.MemberContactService;

namespace Presentation.Models
{
    public class ContactModel : BaseModel
    {
        private MemberContactService contactService;
        private IMemberContactRepository contactRepository;
        private ILocalContactRepository localContactRepository;

        public ContactModel(Context context, IRefreshableActivity refreshableActivity = null)
            : base(context, refreshableActivity)
        {
            localContactRepository = new Infrastructure.Data.SQLite2.MemberContacts.ContactRepository();
        }

        protected override void CreateService()
        {
            contactService = new MemberContactService();
            contactRepository = new MemberRepository();
        }

        public async void ContactCreate(string name, string email, string password, string alternateId = "")
        {
            Show(true);
            SetLoading(LoadingType.Contact, AppData.Status.Loading);

            BeginWsCall();

            try
            {
                var contact = await contactService.CreateMemberContactAsync(contactRepository,
                    new MemberContact()
                    {
                        FirstName = name,
                        UserName = email,
                        Email = email,
                        AlternateId = alternateId,
                        Password = password
                    });

                AppData.Contact = contact;
                Show(false);
                SetLoading(LoadingType.Contact, AppData.Status.None);

                SendBroadcast(BroadcastUtils.ContactUpdated);
                await SaveContact();
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
                SetLoading(LoadingType.Contact, AppData.Status.Failed);
            }

            Show(false);
        }

        public async Task<bool> ContactUpdate(string name, string email, string alternateId = "")
        {
            bool success = false;

            Show(true);
            SetLoading(LoadingType.Contact, AppData.Status.Loading);

            BeginWsCall();

            try
            {
                MemberContact contact = AppData.Contact.ShallowCopy();

                contact.Email = email;
                contact.Name = name;
                contact.AlternateId = alternateId;

                var updatedContact = await contactService.UpdateMemberContactAsync(contactRepository, contact);

                AppData.Contact = updatedContact;
                Show(false);
                SetLoading(LoadingType.Contact, AppData.Status.None);

                SendBroadcast(BroadcastUtils.ContactUpdated);
                await SaveContact();

                success = true;
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
                SetLoading(LoadingType.Contact, AppData.Status.Failed);
            }

            Show(false);

            return success;
        }

        public async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {
            bool success = false;

            Show(true);

            BeginWsCall();

            try
            {
                success = await contactService.ChangePasswordAsync(contactRepository,
                    AppData.Contact.UserName,
                    newPassword,
                    oldPassword);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);

            return success;
        }

        public async void Login(string email, string password)
        {
            Show(true);
            SetLoading(LoadingType.Contact, AppData.Status.Loading);

            BeginWsCall();

            try
            {
                var contact = await contactService.MemberContactLogonAsync(contactRepository,email, password, "Android Device Id");

                AppData.Contact = contact;
                Show(false);
                SetLoading(LoadingType.Contact, AppData.Status.None);

                SendBroadcast(BroadcastUtils.ContactUpdated);
                await SaveContact();
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
                SetLoading(LoadingType.Contact, AppData.Status.Failed);
            }

            Show(false);
        }

        public async void ContactGetById(string contactId)
        {
            Show(true);
            SetLoading(LoadingType.Contact, AppData.Status.Loading);

            BeginWsCall();

            try
            {
                var contact = await contactService.MemberContactByIdAsync(contactRepository, contactId);

                AppData.Contact = contact;
                Show(false);
                SetLoading(LoadingType.Contact, AppData.Status.None);

                SendBroadcast(BroadcastUtils.ContactUpdated);
                await SaveContact();
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
                SetLoading(LoadingType.Contact, AppData.Status.Failed);
            }

            Show(false);
        }

        public async void ContactGetPointBalance(string contactId)
        {
            Show(true);

            BeginWsCall();

            try
            {
                var points = await contactService.MemberContactGetPointBalanceAsync(contactRepository, contactId);

                if (AppData.Contact != null)
                {
                    AppData.Contact.Account.PointBalance = points;
                    SendBroadcast(BroadcastUtils.ContactPointsUpdated);
                    await SaveContact();
                }
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);
        }

        public async Task<bool> ForgotPasswordForDeviceAsync(string userName)
        {
            bool success = false;

            Show(true);

            BeginWsCall();

            try
            {
                success = await contactService.ForgotPasswordForDeviceAsync(contactRepository, userName);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }

            Show(false);

            return success;
        }

        public async Task<bool> ResetPassword(string userName, string resetCode, string newPassword)
        {
            bool success = false;

            Show(true);

            BeginWsCall();

            try
            {
                success = await contactService.ResetPasswordAsync(contactRepository, userName, resetCode, newPassword);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
                throw;
            }

            Show(false);

            return success;
        }

        public async Task Logout()
        {
			BeginWsCall();

			try
            {
                await contactService.ClearContactAsync(localContactRepository);

                AppData.Contact = null;
                SendBroadcast(BroadcastUtils.ContactPointsUpdated);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }
        }

        private async Task SaveContact()
        {
            try
            {
                await contactService.SyncContactAsync(localContactRepository, AppData.Contact);
            }
            catch (Exception ex)
            {
                HandleUIException(ex);
            }
            
        }
    }
}