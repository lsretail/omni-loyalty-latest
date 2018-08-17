using System;
using System.Collections.Generic;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using UIKit;

namespace Presentation
{
    public class AddressForm : UIView
	{
		private UIPickerView dropDown;
		private TextInput name;
		private TextInput adr1;
		private TextInput adr2;
		private TextInput city;
		private TextInput state;
		private TextInput postCode;
		private TextInput country;
		private MemberContact contact;

		private float margin = 10f;

		IList<string> drop;

		//Basket basket, Devices.Device device, Address billingAddress, Address shippingAddress, PaymentType paymentType, string currencyCode, string cardNumber, string cardCVV, string cardName)

		public AddressForm(MemberContact cntct)
		{
			BackgroundColor = UIColor.White;
			contact = cntct;

			drop = new List<string>
			{
				contact.Name,
				"New",
			};
			PickerModel model = new PickerModel(this.drop);
			model.PickerChanged += (sender, e) =>
			{
				ChangeFields(e.SelectedValue);
			};

			bool contactHasAdrValue = (contact.Addresses != null && contact.Addresses.Count > 0);

			dropDown = new UIPickerView();
			dropDown.ShowSelectionIndicator = true;
			dropDown.Model = model;
			       
			name = new TextInput("Name", "Required", contact.Name);
			adr1 = contactHasAdrValue ? new TextInput("Address 1", "Required", contact.Addresses[0].Address1) : new TextInput("Address 1", "Required", "");
			adr2 = new TextInput("Address 2", "", "");
			city = contactHasAdrValue ? new TextInput("City", "Required", contact.Addresses[0].City) : new TextInput("City", "Required", "");
			state = contactHasAdrValue ? new TextInput("State", "Required", contact.Addresses[0].StateProvinceRegion) : new TextInput("State", "Required", "");
			postCode = contactHasAdrValue ? new TextInput("Post Code", "Required", contact.Addresses[0].PostCode) : new TextInput("Post Code", "Required", "");
			country = contactHasAdrValue ? new TextInput("Country", "Required", contact.Addresses[0].Country) : new TextInput("Country", "Required", "");

			name.input.EditingChanged += ChangeContact;
			adr1.input.EditingChanged += ChangeContact;
			adr2.input.EditingChanged += ChangeContact;
			city.input.EditingChanged += ChangeContact;
			state.input.EditingChanged += ChangeContact;
			postCode.input.EditingChanged += ChangeContact;
			country.input.EditingChanged += ChangeContact;

			AddSubviews(dropDown, name, adr1, adr2, city, state, postCode, country);
		}

		void ChangeFields(string value)
		{
			if (value == contact.Name)
			{
				name.input.Text = contact.Name;


				adr1.input.Text = contact.Addresses[0].Address1;
				city.input.Text = contact.Addresses[0].City;
				state.input.Text = contact.Addresses[0].StateProvinceRegion;
				postCode.input.Text = contact.Addresses[0].PostCode;
				country.input.Text = contact.Addresses[0].Country;
			}
			else
			{
				name.input.Text = "";


				adr1.input.Text = "";
				city.input.Text = "";
				state.input.Text = "";
				postCode.input.Text = "";
				country.input.Text = "";
			}
		}

		public void ChangeContact(object o, EventArgs s) {
			bool isContact = (contact.Addresses != null && contact.Addresses.Count > 0);

			if (isContact)
			{
				isContact = adr1.input.Text == contact.Addresses[0].Address1 &&
							city.input.Text == contact.Addresses[0].City &&
							state.input.Text == contact.Addresses[0].StateProvinceRegion &&
							postCode.input.Text == contact.Addresses[0].PostCode &&
							country.input.Text == contact.Addresses[0].Country && 
				            name.input.Text == contact.Name;
			}


			if (dropDown.SelectedRowInComponent(0) != 1 && !isContact)
			{
				dropDown.Select(1, 0, true);
			}
			else if (isContact)
			{
				dropDown.Select(0, 0, true);
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			dropDown.Frame = new CGRect(margin, 0, Frame.Width - margin, 60f);
			name.Frame = new CGRect(margin, dropDown.Frame.Bottom, Frame.Width - margin, 60f);
			adr1.Frame = new CGRect(margin, name.Frame.Bottom, this.Frame.Width - margin, 60f);
			adr2.Frame = new CGRect(margin, adr1.Frame.Bottom, this.Frame.Width - margin, 60f);
			city.Frame = new CGRect(margin, adr2.Frame.Bottom, this.Frame.Width - margin, 60f);
			state.Frame = new CGRect(margin, city.Frame.Bottom, this.Frame.Width - margin, 60f);
			postCode.Frame = new CGRect(margin, state.Frame.Bottom, this.Frame.Width - margin, 60f);
			country.Frame = new CGRect(margin, postCode.Frame.Bottom, this.Frame.Width - margin, 60f);

		}
	}
	//Basket basket, Devices.Device device, Address billingAddress, Address shippingAddress, PaymentType paymentType, string currencyCode, string cardNumber, string cardCVV, string cardName)

}

