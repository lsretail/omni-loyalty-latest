using System;
using UIKit;

namespace Presentation
{
	public class MemberContactProfilesCell : UITableViewCell
	{
		public static string KEY = "MEMBERCONTACTPROFILESSCELL";
		protected int id;

		public MemberContactProfilesCell () : base(UITableViewCellStyle.Default, KEY)
		{
			this.TextLabel.Font = UIFont.SystemFontOfSize(14f);
			SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public void SetValues(int id, string title, bool selected)
		{
			this.id = id;
			this.TextLabel.Text = title;

			if(selected)
			{
				Accessory = UITableViewCellAccessory.Checkmark;
			}
		}
	}
}

