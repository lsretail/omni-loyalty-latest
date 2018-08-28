using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Presentation.Utils;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class RegistrationController : UIViewController
	{
		private RegistrationOrManageAccountView rootView;

		private string PasswordPolicy;
		private List<Profile> Profiles;

		public delegate void RegisterSuccessDelegate();
		private RegisterSuccessDelegate RegisterSuccess;

		public RegistrationController (List<Profile> profiles, string passwordPolicy, RegisterSuccessDelegate registerSuccess)
		{
			this.Title = LocalizationUtilities.LocalizedString("Account_CreateAccount", "Create account");

			this.Profiles = profiles;
			this.PasswordPolicy = passwordPolicy;
			this.RegisterSuccess = registerSuccess;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.rootView = new RegistrationOrManageAccountView(GetMemberContactAttributes(), this.Profiles, true, this.PasswordPolicy);
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
				// if the field is required but empty and not the date field
				if(memberAttribute.IsRequired && string.IsNullOrEmpty(memberAttribute.Value) && memberAttribute.DateTime != DateTime.MinValue)
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

			string username = string.Empty;
			string email = string.Empty;
			string password = string.Empty;
			string confirmPassword = string.Empty;
			string name = string.Empty;
			string address1 = string.Empty;
			string address2 = string.Empty;
			string city = string.Empty;
			string state = string.Empty;
			string postCode = string.Empty;
			string country = string.Empty;
			string phone = string.Empty;

			if(MemberContactAttributes.Registration.Email.Item1)
				email = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Email).Value;

			if(MemberContactAttributes.Registration.Username)
				username = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Username).Value;
			else
				username = email;
			
			password = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Password).Value;
			confirmPassword = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.ConfirmPassword).Value;

			if(MemberContactAttributes.Registration.FirstName.Item1)
				name = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Name).Value;

			if(MemberContactAttributes.Registration.Address1.Item1)
				address1 = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Address1).Value;

			if(MemberContactAttributes.Registration.Address2.Item1)
				address2 = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Address2).Value;

			if(MemberContactAttributes.Registration.City.Item1)
				city = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.City).Value;

			if(MemberContactAttributes.Registration.State.Item1)
				state = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.State).Value;

			if(MemberContactAttributes.Registration.PostCode.Item1)
				postCode = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.PostCode).Value;

			if(MemberContactAttributes.Registration.Country.Item1)
				country = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Country).Value;

			if(MemberContactAttributes.Registration.Phone.Item1)
				phone = memberContactAttributesDTO.SingleOrDefault(x => x.Type == MemberContactAttributesDTO.MemberAttributes.Phone).Value;
			

			if (password != confirmPassword)
			{
				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("Account_PasswordMismatch", "Password mismatch"),
					LocalizationUtilities.LocalizedString("Account_ProvidedPasswordsDontMatch", "The provided passwords do not match"),
					LocalizationUtilities.LocalizedString("General_OK", "OK")
				);
				return;
			}

			Regex emailRegex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");

			if(!emailRegex.Match(email).Success)
			{
				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("Account_InvalidEmail", "Invalid email"),
					string.Empty,
					LocalizationUtilities.LocalizedString("General_OK", "OK")
				);
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

			CreateContact(username, email, password, name, address1, address2, city, state, postCode, country, phone, profiles);
		}

		private async void CreateContact(string username, string email, string password, string name, string address1, string address2, string city, string state, string postCode, string country, string phone, List<Profile> profiles)
		{
			System.Diagnostics.Debug.WriteLine("Creating contact: " + email + ", pass: " + password + ", name: " + name);
			MemberContact contact = new MemberContact ();

			RegexOptions options = RegexOptions.None;
			Regex regex = new Regex(@"[ ]{2,}", options);

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

			contact.UserName = username;
			contact.Password = password;
			contact.Email = email;
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

           // contact.Addresses.Add(address);

			contact.LoggedOnToDevice = new Device ("");
			Util.FillDeviceInfo(contact.LoggedOnToDevice);

			contact.Profiles = profiles;

			Utils.UI.ShowLoadingIndicator();
            bool success = await new ContactModel().CreateMemberContact(contact);
            if (success) 
				{
					Utils.UI.HideLoadingIndicator();

					if(this.RegisterSuccess != null)
					{
						this.RegisterSuccess();
					}
				}
            else
				{
					Utils.UI.HideLoadingIndicator();
				}
			
		}

		private List<MemberContactAttributesDTO> GetMemberContactAttributes()
		{
			List<MemberContactAttributesDTO> memberContactAttributes = new List<MemberContactAttributesDTO>();

			if(MemberContactAttributes.Registration.Username)
			{
				MemberContactAttributesDTO usernameAttribute = new MemberContactAttributesDTO();
				usernameAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Username;
				usernameAttribute.Caption = LocalizationUtilities.LocalizedString("Account_Username", "Username");
				usernameAttribute.Placeholder = LocalizationUtilities.LocalizedString("Account_Required", "Required");
				usernameAttribute.IsPassword = false;
				usernameAttribute.IsRequired = true;

				memberContactAttributes.Add(usernameAttribute);
			}

			if(MemberContactAttributes.Registration.Email.Item1)
			{
				MemberContactAttributesDTO emailAttribute = new MemberContactAttributesDTO();
				emailAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Email;
				emailAttribute.Caption = LocalizationUtilities.LocalizedString("Account_Email", "Email");
				emailAttribute.Placeholder = MemberContactAttributes.Registration.Email.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				emailAttribute.IsPassword = false;
				emailAttribute.IsRequired = MemberContactAttributes.Registration.Email.Item2;

				memberContactAttributes.Add(emailAttribute);
			}
				
			MemberContactAttributesDTO passwordAttribute = new MemberContactAttributesDTO();
			passwordAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Password;
			passwordAttribute.Caption = LocalizationUtilities.LocalizedString("Account_Password", "Password");
			passwordAttribute.Placeholder = LocalizationUtilities.LocalizedString("Account_Required", "Required");
			passwordAttribute.IsPassword = true;
			passwordAttribute.IsRequired = true;
			memberContactAttributes.Add(passwordAttribute);

			MemberContactAttributesDTO confirmPasswordAttribute = new MemberContactAttributesDTO();
			confirmPasswordAttribute.Type = MemberContactAttributesDTO.MemberAttributes.ConfirmPassword;
			confirmPasswordAttribute.Caption = LocalizationUtilities.LocalizedString("Account_ConfirmPassword", "Confirm password");
			confirmPasswordAttribute.Placeholder = LocalizationUtilities.LocalizedString("Account_Required", "Required");
			confirmPasswordAttribute.IsPassword = true;
			confirmPasswordAttribute.IsRequired = true;
			memberContactAttributes.Add(confirmPasswordAttribute);

			if(MemberContactAttributes.Registration.FirstName.Item1)
			{
				MemberContactAttributesDTO nameAttribute = new MemberContactAttributesDTO();
				nameAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Name;
				nameAttribute.Caption = LocalizationUtilities.LocalizedString("Account_Name", "Name");
				nameAttribute.Placeholder = MemberContactAttributes.Registration.FirstName.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				nameAttribute.IsPassword = false;
				nameAttribute.IsRequired = MemberContactAttributes.Registration.FirstName.Item2;

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

			if(MemberContactAttributes.Registration.Address1.Item1)
			{
				MemberContactAttributesDTO address1Attribute = new MemberContactAttributesDTO();
				address1Attribute.Type = MemberContactAttributesDTO.MemberAttributes.Address1;
				address1Attribute.Caption = LocalizationUtilities.LocalizedString ("Account_AddressOne", "Address 1");
				address1Attribute.Placeholder = MemberContactAttributes.Registration.Address1.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				address1Attribute.IsPassword = false;
				address1Attribute.IsRequired = MemberContactAttributes.Registration.Address1.Item2;

				memberContactAttributes.Add(address1Attribute);
			}

			if(MemberContactAttributes.Registration.Address2.Item1)
			{
				MemberContactAttributesDTO address2Attribute = new MemberContactAttributesDTO();
				address2Attribute.Type = MemberContactAttributesDTO.MemberAttributes.Address2;
				address2Attribute.Caption = LocalizationUtilities.LocalizedString ("Account_AddressTwo", "Address 2");
				address2Attribute.Placeholder = MemberContactAttributes.Registration.Address2.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				address2Attribute.IsPassword = false;
				address2Attribute.IsRequired = MemberContactAttributes.Registration.Address2.Item2;

				memberContactAttributes.Add(address2Attribute);
			}

			if(MemberContactAttributes.Registration.City.Item1)
			{
				MemberContactAttributesDTO cityAttribute = new MemberContactAttributesDTO();
				cityAttribute.Type = MemberContactAttributesDTO.MemberAttributes.City;
				cityAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_City", "City");
				cityAttribute.Placeholder = MemberContactAttributes.Registration.City.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				cityAttribute.IsPassword = false;
				cityAttribute.IsRequired = MemberContactAttributes.Registration.City.Item2;

				memberContactAttributes.Add(cityAttribute);
			}

			if(MemberContactAttributes.Registration.State.Item1)
			{
				MemberContactAttributesDTO stateAttribute = new MemberContactAttributesDTO();
				stateAttribute.Type = MemberContactAttributesDTO.MemberAttributes.State;
				stateAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_State", "State");
				stateAttribute.Placeholder = MemberContactAttributes.Registration.State.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				stateAttribute.IsPassword = false;
				stateAttribute.IsRequired = MemberContactAttributes.Registration.State.Item2;

				memberContactAttributes.Add(stateAttribute);
			}

			if(MemberContactAttributes.Registration.PostCode.Item1)
			{
				MemberContactAttributesDTO postCodeAttribute = new MemberContactAttributesDTO();
				postCodeAttribute.Type = MemberContactAttributesDTO.MemberAttributes.PostCode;
				postCodeAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_PostCode", "PostCode");
				postCodeAttribute.Placeholder = MemberContactAttributes.Registration.PostCode.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				postCodeAttribute.IsPassword = false;
				postCodeAttribute.IsRequired = MemberContactAttributes.Registration.PostCode.Item2;

				memberContactAttributes.Add(postCodeAttribute);
			}

			if(MemberContactAttributes.Registration.Country.Item1)
			{
				MemberContactAttributesDTO countryAttribute = new MemberContactAttributesDTO();
				countryAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Country;
				countryAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_Country", "Country");
				countryAttribute.Placeholder = MemberContactAttributes.Registration.Country.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				countryAttribute.IsPassword = false;
				countryAttribute.IsRequired = MemberContactAttributes.Registration.Country.Item2;

				memberContactAttributes.Add(countryAttribute);
			}

			if(MemberContactAttributes.Registration.Phone.Item1)
			{
				MemberContactAttributesDTO phoneAttribute = new MemberContactAttributesDTO();
				phoneAttribute.Type = MemberContactAttributesDTO.MemberAttributes.Phone;
				phoneAttribute.Caption = LocalizationUtilities.LocalizedString ("Account_PhoneNumber", "Phone");
				phoneAttribute.Placeholder = MemberContactAttributes.Registration.Phone.Item2 ? LocalizationUtilities.LocalizedString("Account_Required", "Required") : LocalizationUtilities.LocalizedString("Account_Optional", "Optional");
				phoneAttribute.IsPassword = false;
				phoneAttribute.IsRequired = MemberContactAttributes.Registration.Phone.Item2;

				memberContactAttributes.Add(phoneAttribute);
			}

			return memberContactAttributes;
		}	
	}		
}

