using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using System.Text.RegularExpressions;
using System.Linq;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class ManageAccountController : UIViewController
	{
		private RegistrationOrManageAccountView rootView;
		private List<Profile> Profiles;

		public ManageAccountController (List<Profile> profiles)
		{
			this.Title = LocalizationUtilities.LocalizedString("Account_ManageAccount", "Manage account");

			this.Profiles = profiles;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.rootView = new RegistrationOrManageAccountView(GetMemberContactAttributes(), this.Profiles, false);
			this.rootView.Confirm += ValidateData;
			this.View = this.rootView;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = this.BottomLayoutGuide.Length;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			this.rootView.FlashScrollIndicator();
		}

		private async void ValidateData(List<MemberContactAttributesDTO> memberContactAttributesDTO, List<Profile> profiles)
		{
			foreach(var memberAttribute in memberContactAttributesDTO)
			{
				if(memberAttribute.IsRequired && string.IsNullOrEmpty(memberAttribute.Value))
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_PleaseFillAllFields", "Please fill all required fields"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
					return;
				}
			}

			string errorMessage = string.Empty;

			string email = string.Empty;
			string name = string.Empty;

			if(MemberContactAttributes.Manage.FirstName.Item1)
				email = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Email).Value;

			if(MemberContactAttributes.Manage.FirstName.Item1)
				name = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Name).Value;

			Regex emailRegex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
			Match match = emailRegex.Match(email);

			if (!match.Success)
			{
				errorMessage = LocalizationUtilities.LocalizedString("Account_InvalidEmail", "Invalid email");
				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("General_Error", "Error"),
					errorMessage,
					LocalizationUtilities.LocalizedString("Account_Continue", "Continue")
				);
				//TODO : Set email field as first responder

				return;
			}

			RegexOptions options = RegexOptions.None;
			Regex nameRegex = new Regex(@"[ ]{2,}", options);

			if(MemberContactAttributes.Registration.FirstName.Item1 && MemberContactAttributes.Registration.LastName.Item1)
			{
				var trimmedName = name.Trim ();
				trimmedName = nameRegex.Replace(trimmedName, @" "); //Remove extra spaces
				string[] names = trimmedName.Split (null, 3);

				if(names.Count() < 2)
				{
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_LastNameRequired", "Last name is required"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);

					return;
				}
			}


			UpdateAccount(memberContactAttributesDTO, profiles);
		}

		private async void UpdateAccount(List<MemberContactAttributesDTO> memberContactAttributesDTO, List<Profile> profiles)
		{
			MemberContact contact = AppData.Device.UserLoggedOnToDevice.ShallowCopy();

			string email = string.Empty;
			string name = string.Empty;
			string address1 = string.Empty;
			string address2 = string.Empty;
			string city = string.Empty;
			string state = string.Empty;
			string postCode = string.Empty;
			string country = string.Empty;
			string phone = string.Empty;

			if(MemberContactAttributes.Manage.Email.Item1)
				email = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Email).Value;

			if(MemberContactAttributes.Manage.FirstName.Item1)
				name = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Name).Value;

			if(MemberContactAttributes.Manage.Address1.Item1)
				address1 = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Address1).Value;

			if(MemberContactAttributes.Manage.Address2.Item1)
				address2 = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Address2).Value;

			if(MemberContactAttributes.Manage.City.Item1)
				city = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.City).Value;

			if(MemberContactAttributes.Manage.State.Item1)
				state = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.State).Value;

			if(MemberContactAttributes.Manage.PostCode.Item1)
				postCode = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.PostCode).Value;

			if(MemberContactAttributes.Manage.Country.Item1)
				country = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Country).Value;

			if(MemberContactAttributes.Manage.Phone.Item1)
				phone = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Phone).Value;

			RegexOptions options = RegexOptions.None;
			Regex regex = new Regex(@"[ ]{2,}", options);

			contact.Email = email;

			var trimmedName = name.Trim ();
			trimmedName = regex.Replace(trimmedName, @" "); //Remove extra spaces
			string[] names = trimmedName.Split (null, 3);

			if(names.Length > 2)
			{
				contact.FirstName = names [0];
				contact.MiddleName = names [1];
				contact.LastName = names [2];
			}
			else if(names.Length == 2)
			{
				contact.FirstName = names [0];
				contact.LastName = names [1];
			}
			else
			{
				contact.FirstName = names [0];
			}

			contact.Phone = phone;

			contact.Addresses = new List<Address>();

            var address = new Address(contact.Id)
            {
                Id = contact.Id,
                Type = AddressType.Residential,
                Address1 = address1,
                Address2 = address2,
                City = city,
                PostCode = postCode,
                StateProvinceRegion = state,
                Country = country

                

            };
			contact.Addresses.Add(address);

			contact.LoggedOnToDevice = new Device("");
			Util.FillDeviceInfo(contact.LoggedOnToDevice);

			List<Profile> selectedProfiles = new List<Profile>();
			foreach(var profile in profiles)
			{
				if(profile.ContactValue)
					selectedProfiles.Add(profile);
			}

			contact.Profiles = selectedProfiles;

			Utils.UI.ShowLoadingIndicator();
            bool success = await new ContactModel().UpdateMemberContact(contact);
            if (success) 
				{
					Utils.UI.HideLoadingIndicator();
					await AlertView.ShowAlert(
					    this,
						LocalizationUtilities.LocalizedString("Account_AccountUpdated", "Account successfully updated"),
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
					this.NavigationController.PopViewController(true);
				}
            else
				{
					Utils.UI.HideLoadingIndicator();
				}
			
		}

		private List<MemberContactAttributesDTO> GetMemberContactAttributes()
		{
			List<MemberContactAttributesDTO> memberContactAttributes = new List<MemberContactAttributesDTO>();

			MemberContact contact = AppData.Device.UserLoggedOnToDevice;

			/* User is not allowed to modify the username
			if(MemberContactAttributes.Registration.Username)
			{
				MemberContactAttributesDTO usernameAttribute = new MemberContactAttributesDTO();
				usernameAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Username;
				usernameAttribute.Caption = LocalizationUtilities.LocalizedString("Account_Username", "Username");
				usernameAttribute.Placeholder = LocalizationUtilities.LocalizedString("Account_Required", "Required");
				usernameAttribute.IsPassword = false;
				usernameAttribute.IsRequired = true;
				usernameAttribute.Value = string.IsNullOrEmpty(contact.UserName) ? string.Empty : contact.UserName;

				memberContactAttributes.Add(usernameAttribute);
			}
			*/

			if(MemberContactAttributes.Manage.Email.Item1)
			{
				MemberContactAttributesDTO emailAttribute = new MemberContactAttributesDTO();
				emailAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Email;
				emailAttribute.Caption = LocalizationUtilities.LocalizedString("Account_Email", "Email");
				emailAttribute.Placeholder = MemberContactAttributes.Manage.Email.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				emailAttribute.IsPassword = false;
				emailAttribute.IsRequired = MemberContactAttributes.Manage.Email.Item2;
				emailAttribute.Value = string.IsNullOrEmpty(contact.Email) ? string.Empty : contact.Email;

				memberContactAttributes.Add(emailAttribute);
			}

			if(MemberContactAttributes.Manage.FirstName.Item1)
			{
				MemberContactAttributesDTO nameAttribute = new MemberContactAttributesDTO();
				nameAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Name;
				nameAttribute.Caption = LocalizationUtilities.LocalizedString("Account_Name", "Name");
				nameAttribute.Placeholder = MemberContactAttributes.Manage.FirstName.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				nameAttribute.IsPassword = false;
				nameAttribute.IsRequired = MemberContactAttributes.Manage.FirstName.Item2;
				nameAttribute.Value = string.IsNullOrEmpty(contact.Name) ? string.Empty : contact.Name;

				memberContactAttributes.Add(nameAttribute);
			}

			if(MemberContactAttributes.Registration.Gender.Item1)
			{
				MemberContactAttributesDTO genderAttribute = new MemberContactAttributesDTO();
				genderAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Gender;
				genderAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_Gender", "Gender");
				genderAttribute.Placeholder = MemberContactAttributes.Registration.Gender.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				genderAttribute.IsPassword = false;
				genderAttribute.IsRequired = MemberContactAttributes.Registration.Gender.Item2;

				memberContactAttributes.Add(genderAttribute);
			}

			if(MemberContactAttributes.Registration.DateOfBirth.Item1)
			{
				MemberContactAttributesDTO dateOfBirthAttribute = new MemberContactAttributesDTO();
				dateOfBirthAttribute.Type = MemberContactAttributesDTO.MemberAttributes.DateOfBirth;
				dateOfBirthAttribute.Caption = LocalizationUtilities.LocalizedString("Account_DateOfBirth", "Date of birth");
				dateOfBirthAttribute.Placeholder = MemberContactAttributes.Registration.DateOfBirth.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				dateOfBirthAttribute.IsPassword = false;
				dateOfBirthAttribute.IsRequired = MemberContactAttributes.Registration.DateOfBirth.Item2;

				memberContactAttributes.Add(dateOfBirthAttribute);
			}

			if(MemberContactAttributes.Manage.Address1.Item1)
			{
				MemberContactAttributesDTO address1Attribute = new MemberContactAttributesDTO();
				address1Attribute.Type = MemberContactAttributesDTO.MemberAttributes.Address1;
				address1Attribute.Caption = LocalizationUtilities.LocalizedString ("Account_AddressOne", "Address 1");
				address1Attribute.Placeholder = MemberContactAttributes.Manage.Address1.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				address1Attribute.IsPassword = false;
				address1Attribute.IsRequired = MemberContactAttributes.Manage.Address1.Item2;
				address1Attribute.Value = string.IsNullOrEmpty(contact.Addresses [0].Address1) ? string.Empty : contact.Addresses [0].Address1;

				memberContactAttributes.Add(address1Attribute);
			}

			if(MemberContactAttributes.Manage.Address2.Item1)
			{
				MemberContactAttributesDTO address2Attribute = new MemberContactAttributesDTO();
				address2Attribute.Type = MemberContactAttributesDTO.MemberAttributes.Address2;
				address2Attribute.Caption = LocalizationUtilities.LocalizedString ("Account_AddressTwo", "Address 2");
				address2Attribute.Placeholder = MemberContactAttributes.Manage.Address2.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				address2Attribute.IsPassword = false;
				address2Attribute.IsRequired = MemberContactAttributes.Manage.Address2.Item2;
				address2Attribute.Value = string.IsNullOrEmpty(contact.Addresses [0].Address2) ? string.Empty : contact.Addresses [0].Address2;

				memberContactAttributes.Add(address2Attribute);
			}

			if(MemberContactAttributes.Manage.City.Item1)
			{
				MemberContactAttributesDTO cityAttribute = new MemberContactAttributesDTO();
				cityAttribute.Type = MemberContactAttributesDTO.MemberAttributes.City;
				cityAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_City", "City");
				cityAttribute.Placeholder = MemberContactAttributes.Manage.City.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				cityAttribute.IsPassword = false;
				cityAttribute.IsRequired = MemberContactAttributes.Manage.City.Item2;
				cityAttribute.Value = string.IsNullOrEmpty(contact.Addresses [0].City) ? string.Empty : contact.Addresses [0].City;

				memberContactAttributes.Add(cityAttribute);
			}

			if(MemberContactAttributes.Manage.State.Item1)
			{
				MemberContactAttributesDTO stateAttribute = new MemberContactAttributesDTO();
				stateAttribute.Type = MemberContactAttributesDTO.MemberAttributes.State;
				stateAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_State", "State");
				stateAttribute.Placeholder = MemberContactAttributes.Manage.State.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				stateAttribute.IsPassword = false;
				stateAttribute.IsRequired = MemberContactAttributes.Manage.State.Item2;
				stateAttribute.Value = string.IsNullOrEmpty(contact.Addresses [0].StateProvinceRegion) ? string.Empty : contact.Addresses [0].StateProvinceRegion;

				memberContactAttributes.Add(stateAttribute);
			}

			if(MemberContactAttributes.Manage.PostCode.Item1)
			{
				MemberContactAttributesDTO postCodeAttribute = new MemberContactAttributesDTO();
				postCodeAttribute.Type = MemberContactAttributesDTO.MemberAttributes.PostCode;
				postCodeAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_PostCode", "PostCode");
				postCodeAttribute.Placeholder = MemberContactAttributes.Manage.PostCode.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				postCodeAttribute.IsPassword = false;
				postCodeAttribute.IsRequired = MemberContactAttributes.Manage.PostCode.Item2;
				postCodeAttribute.Value = string.IsNullOrEmpty(contact.Addresses [0].PostCode) ? string.Empty : contact.Addresses [0].PostCode;

				memberContactAttributes.Add(postCodeAttribute);
			}

			if(MemberContactAttributes.Manage.Country.Item1)
			{
				MemberContactAttributesDTO countryAttribute = new MemberContactAttributesDTO();
				countryAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Country;
				countryAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_Country", "Country");
				countryAttribute.Placeholder = MemberContactAttributes.Manage.Country.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				countryAttribute.IsPassword = false;
				countryAttribute.IsRequired = MemberContactAttributes.Manage.Country.Item2;
				countryAttribute.Value = string.IsNullOrEmpty(contact.Addresses [0].Country) ? string.Empty : contact.Addresses [0].Country;

				memberContactAttributes.Add(countryAttribute);
			}

			if(MemberContactAttributes.Manage.Phone.Item1)
			{
				MemberContactAttributesDTO phoneAttribute = new MemberContactAttributesDTO();
				phoneAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Phone;
				phoneAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_PhoneNumber", "Phone");
				phoneAttribute.Placeholder = MemberContactAttributes.Manage.Phone.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				phoneAttribute.IsPassword = false;
				phoneAttribute.IsRequired = MemberContactAttributes.Manage.Phone.Item2;
				phoneAttribute.Value = string.IsNullOrEmpty(contact.Phone) ? string.Empty : contact.Phone;

				memberContactAttributes.Add(phoneAttribute);
			}

			return memberContactAttributes;
		}
	}
}

