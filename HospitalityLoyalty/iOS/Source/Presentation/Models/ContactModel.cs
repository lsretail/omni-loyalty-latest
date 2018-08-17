using System;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.MemberContacts;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using Infrastructure.Data.SQLite2.MemberContacts;

namespace Presentation.Models
{
	public class ContactModel : BaseModel
	{
		private MemberContactService contactService;
		private MemberRepository contactRepository;
		private ContactRepository localContactRepository;

		public ContactModel()
		{
			contactService = new MemberContactService();
			contactRepository = new MemberRepository();
			localContactRepository = new ContactRepository();
		}


		public async Task<bool> Login(string email, string password)
		{
			try
			{
				MemberContact contact =  await contactService.MemberContactLogonAsync(contactRepository, email, password, "");
				AppData.Contact = contact;
				await SaveContactLocally();

				return true;
			}
			catch (Exception ex)
			{
				HandleException(ex, "ContactModel.Login()");
				return false;
			}
			finally
			{
				Utils.UI.StopNetworkActivityIndicator();
			}

		}

		public async Task<bool> Logout()
		{
			// TODO Communicate this to backend?
			try
			{
				await contactService.ClearContactAsync(localContactRepository);
				System.Diagnostics.Debug.WriteLine("Contact cleared locally successfully");
				AppData.Contact = null;

				return true;
				
			}

			catch (Exception ex)
			{
				HandleException(ex, "ContactModel.Logout()", true);
				return false;
			}

		}


		public async Task<bool> ContactCreate(string email, string password, string firstName)
		{
			try
			{
				var contact = new MemberContact()
				{
					UserName = email,
					Email = email,
					Password = password,
					FirstName = firstName
				};

				MemberContact newContact = await contactService.CreateMemberContactAsync(contactRepository, contact);
				AppData.Contact = newContact;
				await SaveContactLocally();

				return true;

			}
			catch (Exception ex)
			{
				HandleException(ex, "ContactModel.ContactCreate()");
				return false;
			}
			finally
			{
				Utils.UI.StopNetworkActivityIndicator();
			}

		}

		public async Task<bool> ContactUpdatePointBalance()
		{
			try
			{
				if (AppData.Contact != null)
				{

					long points = await contactService.MemberContactGetPointBalanceAsync(contactRepository, AppData.Contact.Id);

					System.Diagnostics.Debug.WriteLine("Contact point balance updated: " + points);
					AppData.Contact.Account.PointBalance = points;
					await SaveContactLocally();
					
				}
				return true;
			}

			catch (Exception ex)
			{
				
				HandleException(ex, "ContactModel.ContactGetPointBalance()", false);
				return false;
			}

				
		}

		private async Task SaveContactLocally()
		{
			try
			{
				MemberContact contact = await contactService.SyncContactAsync(localContactRepository, AppData.Contact);

				System.Diagnostics.Debug.WriteLine("Contact saved locally successfully");
			}
			catch (Exception ex)
			{
				HandleException(ex, "ContactModel.SaveContactLocally()", false);
			}
		}
			

		public async void LoadLocalContact()
		{
			try
			{
				MemberContact contact = await contactService.GetContactAsync(localContactRepository);

				AppData.Contact = contact;
				System.Diagnostics.Debug.WriteLine("Local contact fetched successfully");

			}
			catch (Exception ex)
			{
				HandleException(ex, "ContactModel.LoadLocalContact()", false);
			}
		
		}

		public async Task<bool> ChangePassword(string userName, string newPassword, string oldPassword)
		{
			try
			{
				bool success = await this.contactService.ChangePasswordAsync(contactRepository, userName, newPassword, oldPassword);
				if (success)
					return true;
				else
					throw new Exception(LocalizationUtilities.LocalizedString("Account_ErrorChangePassword", "Error changing password"));
			
			}
			catch (Exception ex)
			{
				HandleException(ex, "MemberContactModel.ChangePassword()", true);
				return false;
			}


		}

		public async Task<bool> UpdateContact(MemberContact contact)
		{
			try
			{
				MemberContact contactReturned = await this.contactService.UpdateMemberContactAsync(contactRepository, contact);

				AppData.Contact = contactReturned;
				await SaveContactLocally();
				return true;
			}
			catch (Exception ex)
			{
				HandleException(ex, "MemberContactModel.UpdateMemberContact()", true);
				return false;
			}

		}

		public async Task<bool> ForgotPasswordForDevice(string username)
		{
			try
			{
				bool success = await this.contactService.ForgotPasswordForDeviceAsync(contactRepository, username);
				return success;
			}
			catch (Exception ex)
			{
				HandleException(ex, "MemberContactModel.ForgotPasswordForDevice()", true);
				return false;
			}


		}

		public async Task<bool> ResetPassword(string userName, string resetCode, string newPassword)
		{
			try
			{
				bool success = await this.contactService.ResetPasswordAsync(contactRepository, userName, resetCode, newPassword);
				return success;

			}
			catch (Exception ex)
			{
				HandleException(ex, "MemberContactModel.ResetPasswordAsync()", true);
				return false;
			
			}
			
		}


	}
}