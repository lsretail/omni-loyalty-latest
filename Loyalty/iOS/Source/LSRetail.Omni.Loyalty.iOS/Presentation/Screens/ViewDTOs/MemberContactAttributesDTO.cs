using System;

namespace Presentation
{
	public class MemberContactAttributesDTO
	{
		public enum MemberAttributes
		{
			None = 0,
			Email = 1,
			Password = 2,
			ConfirmPassword = 3,
			Name = 4,
			Address1 = 5,
			Address2 = 6,
			City = 7,
			State = 8,
			PostCode = 9,
			Country = 10,
			Phone = 11,
			DateOfBirth = 12,
			Gender = 13,
			Username = 14
		}

		public MemberAttributes Type { get; set; }
		public string Caption { get; set; }
		public string Placeholder {get; set; }
		public bool IsPassword { get; set; }
		public bool IsRequired { get; set; }
		public string Value { get; set; }
		public DateTime DateTime { get; set; } // only used for DateOfBirth attribute

		public MemberContactAttributesDTO()
		{
			this.Type = MemberAttributes.None;
			this.Caption = string.Empty;
			this.Placeholder = string.Empty;
			this.IsPassword = false;
			this.IsRequired = false;
			this.Value = string.Empty;
			this.DateTime = new DateTime();
		}
	}
}

