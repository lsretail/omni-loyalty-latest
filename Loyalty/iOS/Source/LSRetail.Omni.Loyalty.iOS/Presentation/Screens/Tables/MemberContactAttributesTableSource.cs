using System;
using UIKit;
using System.Collections.Generic;
using Foundation;

namespace Presentation
{
    public class MemberContactAttributesTableSource : UITableViewSource
	{
		private UITableView tableView;
		private List<MemberContactAttributesDTO> memberContactAttributes;
		private NSIndexPath datePickerIndexPath;
		private bool isDatePickerIsShowing;
		private nfloat cellHeight = 44f;
		private nfloat datePickerHeight = 162f;

		public UITextField activeTextField;  // refrence to the active textField - refrence it if we need to hide the keyboard

		public delegate void ResizeTableViewEventHandler();
		public event ResizeTableViewEventHandler ResizeTableView;

		public MemberContactAttributesTableSource (UITableView tableView, List<MemberContactAttributesDTO> memberContactAttributesDTO)
		{
			this.tableView = tableView;
			this.memberContactAttributes = memberContactAttributesDTO;
			this.datePickerIndexPath = HasDatePicker() ? GetDatePickerIndexPath() : NSIndexPath.FromRowSection(100, 0);
			this.isDatePickerIsShowing = false;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return HasDatePicker() ? this.memberContactAttributes.Count + 1 : this.memberContactAttributes.Count;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if(indexPath.Row == this.datePickerIndexPath.Row && indexPath.Section == this.datePickerIndexPath.Section)
			{
				return this.isDatePickerIsShowing ? this.datePickerHeight : 0f;
			}
			else
			{
				return this.cellHeight;
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if(indexPath.Row == this.datePickerIndexPath.Row && indexPath.Section == this.datePickerIndexPath.Section)
			{
				// do nothing
			}
			else
			{
				int attributePosition = (indexPath.Row < this.datePickerIndexPath.Row) ? indexPath.Row : indexPath.Row - 1;

				MemberContactAttributesDTO memberContactAttribute = this.memberContactAttributes[attributePosition];

				if(memberContactAttribute.Type == MemberContactAttributesDTO.MemberAttributes.DateOfBirth)
				{
					// if keyboard is visable - hide it before presenting date picker
					if(this.activeTextField != null && this.activeTextField.IsFirstResponder)
					{
						this.activeTextField.ResignFirstResponder();
					}

					if(this.isDatePickerIsShowing)
					{
						HideInlineDatePicker();
					}
					else
					{
						ShowInlineDatePicker();
					}

					this.tableView.ReloadData();
				}
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// if it's the indexpath for the date picker
			if(indexPath.Row == this.datePickerIndexPath.Row && indexPath.Section == this.datePickerIndexPath.Section)
			{
				DatePickerCell cell = tableView.DequeueReusableCell(DatePickerCell.KEY) as DatePickerCell;
				if (cell == null)
				{
					cell = new DatePickerCell();
					cell.DatePickerValueChanged += DatePickerValueChanged;
					cell.ClipsToBounds = true;
				}

				MemberContactAttributesDTO dateOfBirthAttribute = this.memberContactAttributes[indexPath.Row - 1];
				cell.SetValues(dateOfBirthAttribute.DateTime);

				return cell;
			}
			else
			{
				int attributePosition = (indexPath.Row < this.datePickerIndexPath.Row) ? indexPath.Row : indexPath.Row - 1;
				MemberContactAttributesDTO memberContactAttribute = this.memberContactAttributes[attributePosition];

				if(memberContactAttribute.Type == MemberContactAttributesDTO.MemberAttributes.Gender)
				{
					MemberContactGenderCell cell = tableView.DequeueReusableCell(MemberContactGenderCell.KEY) as MemberContactGenderCell;
					if (cell == null)
						cell = new MemberContactGenderCell();

					cell.SeparatorInset = UIEdgeInsets.Zero;

					cell.SetValues(
						this.memberContactAttributes[attributePosition]
					);

					return cell;
				}
				else
				{
					MemberContactAttributesCell cell = tableView.DequeueReusableCell(MemberContactAttributesCell.KEY) as MemberContactAttributesCell;
					if (cell == null)
						cell = new MemberContactAttributesCell();

					cell.TextFieldShouldReturn += CellTextFieldShouldReturn;
					cell.TextFieldShouldBeginEditing += CellTextFieldShouldBeginEditing;
					cell.SeparatorInset = UIEdgeInsets.Zero;

					cell.SetValues(
						indexPath.Row,
						this.memberContactAttributes[attributePosition]
					);

					return cell;
				}
			}
		}

		public nfloat GetRequiredTableViewHeight()
		{
			if(this.isDatePickerIsShowing)
			{
				return (this.memberContactAttributes.Count) * this.cellHeight + this.datePickerHeight;
			}
			else
			{
				return this.memberContactAttributes.Count * this.cellHeight;
			}
		}

		private void CellTextFieldShouldReturn(MemberContactAttributesCell cell)
		{
			// If cell is not the last cell in the list - shift focus to the next cell
			if(cell.id < this.memberContactAttributes.Count)
			{
				NSIndexPath currentIndexPath = this.tableView.IndexPathForCell(cell);
				NSIndexPath nextIndexPath = NSIndexPath.FromRowSection(currentIndexPath.Row + 1, currentIndexPath.Section);

				MemberContactAttributesCell nextCell = this.tableView.CellAt(nextIndexPath) as MemberContactAttributesCell;

				if(nextCell != null)
				{
					nextCell.inputField.BecomeFirstResponder();
				}
				else
				{
					cell.inputField.ResignFirstResponder();
				}
			}
			else
			{
				cell.inputField.ResignFirstResponder();
			}
		}

		private void CellTextFieldShouldBeginEditing(MemberContactAttributesCell cell)
		{
			this.activeTextField = cell.inputField;
		}

		#region DatePicker

		private bool HasDatePicker()
		{
			return this.memberContactAttributes.Exists(x => x.Type == MemberContactAttributesDTO.MemberAttributes.DateOfBirth);
		}

		private void ShowInlineDatePicker()
		{
			this.isDatePickerIsShowing = true;

			if(this.ResizeTableView != null)
			{
				this.ResizeTableView();
			}

			nint dateOfBirthRow = this.memberContactAttributes.FindIndex(x => x.Type == MemberContactAttributesDTO.MemberAttributes.DateOfBirth);

			DatePickerCell datePickerCell = this.tableView.CellAt(NSIndexPath.FromRowSection(dateOfBirthRow + 1, 0)) as DatePickerCell;
			datePickerCell.ShowDatePicker();
		}

		private void HideInlineDatePicker()
		{
			this.isDatePickerIsShowing = false;

			if(this.ResizeTableView != null)
			{
				this.ResizeTableView();
			}

			nint dateOfBirthRow = this.memberContactAttributes.FindIndex(x => x.Type == MemberContactAttributesDTO.MemberAttributes.DateOfBirth);
			DatePickerCell datePickerCell = this.tableView.CellAt(NSIndexPath.FromRowSection(dateOfBirthRow + 1, 0)) as DatePickerCell;

			datePickerCell.HideDatePicker();
		}

		private NSIndexPath GetDatePickerIndexPath()
		{
			nint datePickerRow = this.memberContactAttributes.FindIndex(x => x.Type == MemberContactAttributesDTO.MemberAttributes.DateOfBirth) + 1;

			return NSIndexPath.FromRowSection(datePickerRow, 0);
		}

		private void DatePickerValueChanged(DateTime date)
		{
			nint dateOfBirthRow = this.memberContactAttributes.FindIndex(x => x.Type == MemberContactAttributesDTO.MemberAttributes.DateOfBirth);
			MemberContactAttributesCell dateOfBirthCell = this.tableView.CellAt(NSIndexPath.FromRowSection(dateOfBirthRow, 0)) as MemberContactAttributesCell;

			dateOfBirthCell.SetValues(date);
		}

		#endregion
	}
}

