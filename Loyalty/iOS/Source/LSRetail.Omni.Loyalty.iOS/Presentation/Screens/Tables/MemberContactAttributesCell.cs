using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace Presentation
{
	public class MemberContactAttributesCell : UITableViewCell
	{
		public static string KEY = "MEMBERCONTACTATTRIBUTESCELL";
		public int id;

		private UILabel captionLabel;
		public UITextField inputField;

		private MemberContactAttributesDTO memberContactAttributes;

		public delegate void TextFieldShouldReturnEventHandler(MemberContactAttributesCell cell);
		public event TextFieldShouldReturnEventHandler TextFieldShouldReturn;

		public delegate void TextFieldShouldBeginEditingEventHandler(MemberContactAttributesCell cell);
		public event TextFieldShouldBeginEditingEventHandler TextFieldShouldBeginEditing;

		public MemberContactAttributesCell () : base(UITableViewCellStyle.Default, KEY)
		{
			this.captionLabel = new UILabel();
			this.captionLabel.Font = UIFont.BoldSystemFontOfSize(14f);
			this.captionLabel.BackgroundColor = UIColor.Clear;
			this.ContentView.AddSubview(this.captionLabel);

			this.inputField = new UITextField();
			this.inputField.Font = UIFont.SystemFontOfSize(14f);
			this.inputField.BackgroundColor = UIColor.Clear;
			this.inputField.AutocorrectionType = UITextAutocorrectionType.No;
			this.inputField.AutocapitalizationType = UITextAutocapitalizationType.None;
			inputField.UserInteractionEnabled = true;

			CustomTextFieldDelegate inputFieldDelegate = new CustomTextFieldDelegate();
			inputFieldDelegate.TextFieldShouldReturn += () =>
			{
				if(this.TextFieldShouldReturn != null)
					this.TextFieldShouldReturn(this);
			};
			inputFieldDelegate.TextFieldShouldBeginEditing += () => 
			{
				if(this.TextFieldShouldBeginEditing != null)
					this.TextFieldShouldBeginEditing(this);
			};
			inputField.EditingChanged += (sender, e) => 
			{
				this.memberContactAttributes.Value = this.inputField.Text;
			};
			inputField.Delegate = inputFieldDelegate;

			this.ContentView.AddSubview(inputField);

			SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			const float margin = 15f;

			this.captionLabel.Frame = new CGRect(margin, 0f, 0.50 * (this.ContentView.Frame.Width - margin), this.ContentView.Frame.Height);
			this.inputField.Frame = new CGRect(this.captionLabel.Frame.Right + margin, 0f, 0.50 * (this.ContentView.Frame.Width - margin) - 2 * margin, this.ContentView.Frame.Height);
		}

		public void SetValues(int id, MemberContactAttributesDTO memberContactAttributes)
		{
			this.id = id;
			this.memberContactAttributes = memberContactAttributes;
			this.captionLabel.Text = this.memberContactAttributes.Caption;

			if(memberContactAttributes.Type == MemberContactAttributesDTO.MemberAttributes.DateOfBirth)
			{
				this.inputField.UserInteractionEnabled = false;

				// if user has set a date
				if(memberContactAttributes.DateTime != DateTime.MinValue)
				{
					SetValues(memberContactAttributes.DateTime);
				}
				else
				{
					this.inputField.Placeholder = this.memberContactAttributes.Placeholder;
					this.inputField.SecureTextEntry = this.memberContactAttributes.IsPassword;
				}
					
			}
			else
			{
				this.inputField.Text = this.memberContactAttributes.Value;
				this.inputField.Placeholder = this.memberContactAttributes.Placeholder;
				this.inputField.SecureTextEntry = this.memberContactAttributes.IsPassword;
			}
		}

		public void SetValues(DateTime date)
		{
			this.memberContactAttributes.DateTime = date;
			this.inputField.Text = date.ToString("dd. MMMMM yyyy");
		}

		private class CustomTextFieldDelegate : UITextFieldDelegate
		{
			public delegate void TextFieldShouldReturnEventHandler();
			public event TextFieldShouldReturnEventHandler TextFieldShouldReturn;

			public delegate void TextFieldShouldBeginEditingEventHandler();
			public event TextFieldShouldBeginEditingEventHandler TextFieldShouldBeginEditing;

			public override bool ShouldReturn (UITextField textField)
			{
				textField.ResignFirstResponder();

				if(this.TextFieldShouldReturn != null)
				{
					this.TextFieldShouldReturn();
				}

				return true;
			}

			public override bool ShouldBeginEditing (UITextField textField)
			{
				if(this.TextFieldShouldBeginEditing != null)
				{
					this.TextFieldShouldBeginEditing();
				}

				return true;
			}
		}
	}
}

