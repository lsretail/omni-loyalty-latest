using System;
using System.Threading.Tasks;
using MonoTouch.Dialog;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using CoreGraphics;
using System.Text.RegularExpressions;
using Presentation.Models;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
	public partial class ManageAccountDialogController : DialogViewController
	{
		private EntryElement email;
		private EntryElement name;
		private EntryElement address1;
		private EntryElement address2;
		private EntryElement city;
		private EntryElement state;
		private EntryElement postCode;
		private EntryElement country;
		private EntryElement phoneNumber;

		public ManageAccountDialogController () : base (UITableViewStyle.Grouped, null)
		{
		}

		public void BuildAccountTable()
		{
			var contact = AppData.Contact;

			this.email = new EntryElement(LocalizationUtilities.LocalizedString("Account_Email", "Email"), String.Empty, contact.Email, false);
			email.AutocapitalizationType = UITextAutocapitalizationType.None;
			email.AutocorrectionType = UITextAutocorrectionType.No;

			this.name = new EntryElement (LocalizationUtilities.LocalizedString ("Account_Name", "Name"), String.Empty, contact.Name, false);
			this.address1 = new EntryElement (LocalizationUtilities.LocalizedString ("Account_AddressOne", "Address 1"), LocalizationUtilities.LocalizedString ("Account_Optional", "Optional"), contact.Addresses [0].Address1);
			this.address2 = new EntryElement (LocalizationUtilities.LocalizedString ("Account_AddressTwo", "Address 2"), LocalizationUtilities.LocalizedString ("Account_Optional", "Optional"), contact.Addresses [0].Address2);
			this.city = new EntryElement (LocalizationUtilities.LocalizedString ("Account_City", "City"), LocalizationUtilities.LocalizedString ("Account_Optional", "Optional"), contact.Addresses [0].City);
			this.state = new EntryElement (LocalizationUtilities.LocalizedString ("Account_State", "State"), LocalizationUtilities.LocalizedString ("Account_Optional", "Optional"), contact.Addresses [0].StateProvinceRegion);
			this.postCode = new EntryElement (LocalizationUtilities.LocalizedString ("Account_PostCode", "PostCode"), LocalizationUtilities.LocalizedString ("Account_Optional", "Optional"), contact.Addresses [0].PostCode);
			this.country = new EntryElement (LocalizationUtilities.LocalizedString ("Account_Country", "Country"), LocalizationUtilities.LocalizedString ("Account_Optional", "Optional"), contact.Addresses [0].Country);
			this.phoneNumber = new EntryElement (LocalizationUtilities.LocalizedString ("Account_PhoneNumber", "Phone"), LocalizationUtilities.LocalizedString ("Account_Optional", "Optional"), contact.Phone);

			// Footer view

			UIView footerView = new UIView();
			footerView.Frame = new CGRect(0, 0, this.TableView.Frame.Width, 100);
			footerView.BackgroundColor = UIColor.Clear;

			UIButton btnOK = new UIButton();
			btnOK.SetTitle(LocalizationUtilities.LocalizedString("Account_UpdateAccount", "Update account"), UIControlState.Normal);
			btnOK.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnOK.Frame = new CGRect(20, 20, footerView.Frame.Width - 2*20, 50);
			btnOK.Layer.CornerRadius = 2;
			btnOK.TouchUpInside += (object sender, EventArgs e) => {

				if(ValidateData().Result)
				{
					UpdateAccount();
				}
			};
			footerView.AddSubview(btnOK);


			// Table itself

			Root = new RootElement (LocalizationUtilities.LocalizedString("Account_ManageAccount", "Manage account"))
			{
				new Section (null, footerView)
				{
					email,
					name,
					address1,
					address2,
					city,
					state,
					postCode,
					country,
					phoneNumber
				}
			};

			this.TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
			this.TableView.ScrollEnabled = true;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UIBarButtonItem backButton = new UIBarButtonItem ();
			backButton.Title = LocalizationUtilities.LocalizedString("General_Back", "Back");
			backButton.Clicked += (object sender, EventArgs e) => {

				this.NavigationController.PopViewController(true);
				this.NavigationItem.LeftBarButtonItem = null;

			};
			this.NavigationItem.LeftBarButtonItem = backButton;

			var tap = new UITapGestureRecognizer ();
			tap.CancelsTouchesInView = false;
			tap.AddTarget (() =>{
				this.View.EndEditing (true);
			});
			this.View.AddGestureRecognizer (tap);

			// Background view
			// "Blur" effect - using a semi-transparent white overlay

			UIImageView blurredBackground = new UIImageView ();
			blurredBackground.Frame = new CGRect(0f, 0f, this.View.Frame.Width, this.View.Frame.Height);
			blurredBackground.Image = Utils.Image.FromFile ("/Branding/Standard/slideoutmenubackground2.png");
			blurredBackground.ContentMode = UIViewContentMode.BottomLeft;
			blurredBackground.ClipsToBounds = true;

			UIView whiteBackgroundOverlay = new UIView ();
			whiteBackgroundOverlay.Frame = new CGRect (0f, 0f, blurredBackground.Frame.Width, blurredBackground.Frame.Height);
			whiteBackgroundOverlay.BackgroundColor = Utils.AppColors.TransparentWhite3;
			blurredBackground.AddSubview (whiteBackgroundOverlay);

			TableView.BackgroundColor = UIColor.Clear;
			TableView.BackgroundView = blurredBackground;

			BuildAccountTable();

			RegisterKeyboardNotificationHandling();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		private async Task<bool> ValidateData()
		{
			string errorMessage = string.Empty;

			Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");

			Match match = regex.Match(email.Value);

			if (string.IsNullOrEmpty(email.Value))
			{
				errorMessage = email.Caption + LocalizationUtilities.LocalizedString("Account_EmptyError", " cannot be empty");
				email.BecomeFirstResponder(true);
			}
			else if (!match.Success)
			{
				errorMessage = LocalizationUtilities.LocalizedString("Account_InvalidEmail", "Invalid email");
				email.BecomeFirstResponder(true);
			}
			else if(string.IsNullOrEmpty(name.Value))
			{
				errorMessage = name.Caption + LocalizationUtilities.LocalizedString("Account_EmptyError", " cannot be empty");
				name.BecomeFirstResponder(true);
			}

			if (!string.IsNullOrEmpty(errorMessage))
			{
				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("General_Error", "Error"),
					errorMessage,
					LocalizationUtilities.LocalizedString("Account_Continue", "Continue")
				);
				return false;
			}

			return true;
		}

		private async void UpdateAccount()
		{
			MemberContact contact = AppData.Contact.ShallowCopy();

			contact.Email = email.Value;
			contact.Name = name.Value.Trim();
			contact.Phone = phoneNumber.Value;

			contact.Addresses = new List<Address>();

			var address = new Address();
			address.Address1 = address1.Value;
			address.Address2 = address2.Value;
			address.City = city.Value;
			address.StateProvinceRegion = state.Value;
			address.PostCode = postCode.Value;
			address.Country = country.Value;

			contact.Addresses.Add(address);

			Utils.UI.ShowLoadingIndicator();
			bool success = await new ContactModel().UpdateContact(contact);
			if (success) 
			{
				Utils.UI.HideLoadingIndicator();
				//Utils.Util.AppDelegate.SlideoutMenu.RefreshSlideoutMenu();
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

		private void RegisterKeyboardNotificationHandling()
		{


			// The tableview tended to end up in a weird location (way too low) after the keyboard is hidden.
			// Fix this by scrolling it to the desired location manually after the keyboard is hidden.
			UIKeyboard.Notifications.ObserveDidHide((sender, e) => {

				this.TableView.SetContentOffset(new CGPoint(0, 0), true);

			});
		}
	}
}

