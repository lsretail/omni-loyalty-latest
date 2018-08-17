using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation
{
    public class MemberContactProfilesTableSource : UITableViewSource
	{
		private UIView headerView;
		private List<Profile> Profiles;
		private nfloat cellHeight = 44f;
		private nfloat headerHeight = 28f;

		public MemberContactProfilesTableSource (List<Profile> Profiles)
		{
			this.Profiles = Profiles;
			BuildHeaderView();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.Profiles.Count;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return this.cellHeight;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.CellAt(indexPath);
			Profile profile = this.Profiles[indexPath.Row];

			if(profile.ContactValue)
			{
				cell.Accessory = UITableViewCellAccessory.None;
				profile.ContactValue = false;
			}
			else
			{
				cell.Accessory = UITableViewCellAccessory.Checkmark;
				profile.ContactValue = true;
			}

		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			MemberContactProfilesCell cell = tableView.DequeueReusableCell(MemberContactProfilesCell.KEY) as MemberContactProfilesCell;
			if (cell == null)
				cell = new MemberContactProfilesCell();

			string caption = this.Profiles[indexPath.Row].Description;
			bool selected = this.Profiles[indexPath.Row].ContactValue;

			cell.SetValues(
				indexPath.Row,
				caption,
				selected
			);

			return cell;
		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return this.headerHeight;
		}


		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			return this.headerView;
		}

		private void BuildHeaderView()
		{
			this.headerView = new UIView();

			UILabel lblTitle = new UILabel();
			lblTitle.Font = UIFont.SystemFontOfSize(14f);
			lblTitle.TextColor = UIColor.DarkGray;
			lblTitle.Text = LocalizationUtilities.LocalizedString("Account_Profiles", "Choose profiles").ToUpper();
			this.headerView.AddSubview(lblTitle);

			nfloat margin = 15f;

			headerView.ConstrainLayout(() =>

				lblTitle.Frame.Top == headerView.Bounds.Top &&
				lblTitle.Frame.Left == headerView.Bounds.Left + margin &&
				lblTitle.Frame.Right == headerView.Bounds.Right &&
				lblTitle.Frame.Height == this.headerHeight
			);
		}

		public nfloat GetRequiredTableViewHeight()
		{
			return this.Profiles.Count * this.cellHeight + this.headerHeight;
		}
	}
}

